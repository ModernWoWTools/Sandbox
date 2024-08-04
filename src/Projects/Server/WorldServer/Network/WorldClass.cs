/*
 * Copyright (C) 2012-2014 Arctium <http://arctium.org>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using AuthServer.WorldServer.Managers;
using Framework.Constants.Misc;
using Framework.Constants.Net;
using Framework.Cryptography.WoW;
using Framework.Database.Auth.Entities;
using Framework.Logging;
using Framework.Network.Packets;
using System;
using System.Collections;
using System.Net.Sockets;
using AuthServer.Game.Entities;
using AuthServer.WorldServer.Game.Packets;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Framework.Misc;
using AuthServer.Game.Packets.PacketHandler;
using System.Text;
using AuthServer.Game.PacketHandler;

namespace AuthServer.Network
{
    public sealed class WorldClass : IDisposable
    {

        public bool IsFlying { get; set; }
        public float Flightspeed { get; set; } = 7f;
        public float RunSpeed { get; set; } = 7f;
        public ulong TargetGuid { get; set; }

        public Account Account { get; set; }
        public Character Character { get; set; }

        public static WorldNetwork world;
        public Socket clientSocket;
        public Queue PacketQueue;
        public WoWCrypt Crypt;

        byte[] DataBuffer;
        public bool initiated = false;

        public WorldClass()
        {
            DataBuffer = new byte[0x40000];
            PacketQueue = new Queue();
            Crypt = new WoWCrypt();
        }

        public void OnData()
        {
            PacketReader packet;

            if (PacketQueue.Count > 0)
                packet = (PacketReader)PacketQueue.Dequeue();
            else
                packet = new PacketReader(DataBuffer, false, initiated);

            PacketManager.InvokeHandler(ref packet, this);
        }

        public void OnConnect()
        {
            clientSocket.BeginReceive(DataBuffer, 0, DataBuffer.Length, SocketFlags.None, Receive, null);

            var transferInitiate = new PacketWriter();

            transferInitiate.WriteString("WORLD OF WARCRAFT CONNECTION - SERVER TO CLIENT - V2");

            var bufff = transferInitiate.ReadDataToSend(true).Concat(new byte[] { 0x0A }).ToArray();

            clientSocket.Send(bufff, 0, bufff.Length, SocketFlags.None);

        }

        public void Receive(IAsyncResult result)
        {
            try
            {
                var recievedBytes = clientSocket.EndReceive(result);
                if (recievedBytes != 0)
                {

                    if (!initiated)
                    {
                        if (recievedBytes == 1 && DataBuffer[0] == 0x0A || DataBuffer[recievedBytes - 1] == 0x0A)
                        {
                            var pktdata = new PacketReader(Encoding.ASCII.GetBytes("WORLD OF WARCRAFT CONNECTION - CLIENT TO SERVE"), false, false);

                            AuthenticationHandler.HandleAuthChallenge(ref pktdata, this);
                        }
                    }
                    else if (Crypt.IsInitialized)
                    {
                        while (recievedBytes > 0)
                        {
                            Decode(ref DataBuffer);

                            var length = BitConverter.ToUInt16(DataBuffer, 0) + 4;

                            var packetData = new byte[length];
                            Buffer.BlockCopy(DataBuffer, 0, packetData, 0, length);

                            PacketReader packet = new PacketReader(packetData, true);
                            PacketQueue.Enqueue(packet);

                            recievedBytes -= length;
                            Buffer.BlockCopy(DataBuffer, length, DataBuffer, 0, recievedBytes);

                            OnData();
                        }
                    }
                    else
                        OnData();

                    clientSocket?.BeginReceive(DataBuffer, 0, DataBuffer.Length, SocketFlags.None, Receive, null);
                }
            }
            catch (Exception ex)
            {
                CharacterHandler.chatRunning = false;

                Log.Message(LogType.Error, "{0}", ex.StackTrace);
            }
        }

        void Decode(ref byte[] data)
        {
            Crypt.Decrypt(data, 4);
        }

        public static uint Adler32(byte[] data)
        {
            var a = 0xD8F1u;
            var b = 0x9827u;

            for (var i = 0; i < data.Length; i++)
            {
                a = (a + data[i]) % 0xFFF1;
                b = (b + a) % 0xFFF1;
            }

            return (b << 16) + a;
        }

        public bool Authed;

        public void Send(ref PacketWriter packet, bool fakeSend = false)
        {
            var buffer = packet.ReadDataToSend();

            try
            {
                Log.Message(LogType.Packet, "Send Opcode: {0} (0x{0:X}), Length: {1}", packet.Opcode, packet.Size);
                //Log.Message(LogType.Packet, $"Data ({packet.Opcode}): {BitConverter.ToString(buffer).Replace("-", " ")}");

                if (Crypt.IsInitialized)
                {
                    if (buffer.Length > 0xFFFF)
                    {
                        var reset = new PacketWriter(ServerMessage.ResetCompressionContext);
                        Send(ref reset);
                        var compression = new PacketWriter(ServerMessage.Compression);

                        var msg = BitConverter.GetBytes((ushort)packet.Opcode);
                        var data = new byte[buffer.Length - 4];

                        Buffer.BlockCopy(buffer, 4, data, 0, data.Length);

                        data[0] = msg[0];
                        data[1] = msg[1];

                        byte[] CompressedData = null;

                        var UncompressedSize = data.Length;
                        var UncompressedAdler = Adler32(data);

                        // Compress.
                        using (var ms = new MemoryStream())
                        {
                            using (var ds = new DeflateStream(ms, CompressionLevel.Fastest))
                            {
                                ds.Write(data, 0, data.Length);
                                ds.Flush();
                            }

                            CompressedData = ms.ToArray();
                        }

                        CompressedData[0] -= 1;

                        if ((CompressedData[CompressedData.Length - 1] & 8) == 8)
                            CompressedData = CompressedData.Combine(new byte[1]);

                        CompressedData = CompressedData.Combine(new byte[] { 0x00, 0x00, 0xFF, 0xFF });
                        var CompressedAdler = Adler32(CompressedData);

                        compression.Write(UncompressedSize);
                        compression.Write(UncompressedAdler);
                        compression.Write(CompressedAdler);
                        compression.Write(CompressedData);

                        buffer = compression.ReadDataToSend();

                    }

                    var size = BitConverter.GetBytes(buffer.Length - 4);

                    buffer[0] = size[0];
                    buffer[1] = size[1];
                    buffer[2] = size[2];
                    buffer[3] = size[3];

                    Crypt.Encrypt(buffer, 4);
                }
                else
                {/*
                    if ((int)packet.Opcode == (int)ServerMessage.ObjectUpdate && buffer.Length >= 0x1000)
                    {
                        Log.Message(LogType.Normal, "Compressing packet.");

                        var reset = new PacketWriter(ServerMessage.ResetCompressionContext);
                        var resetBuffer = reset.ReadDataToSend();

                        clientSocket.Send(resetBuffer, 0, resetBuffer.Length, SocketFlags.None);
                        
                        var compression = new PacketWriter(ServerMessage.Compression);

                        var msg = BitConverter.GetBytes((ushort)packet.Opcode);
                        var data = new byte[buffer.Length - 16];

                        Buffer.BlockCopy(buffer, 16, data, 0, data.Length);

                        byte[] CompressedData = null;

                        var UncompressedSize = data.Length;
                        var UncompressedAdler = Adler32(data);

                        // Compress.
                        using (var ms = new MemoryStream())
                        {
                            using (var ds = new DeflateStream(ms, CompressionLevel.Fastest))
                            {
                                ds.Write(data, 0, data.Length);
                                ds.Flush();
                            }

                            CompressedData = ms.ToArray();
                        }

                        //CompressedData[0] -= 1;

                        if ((CompressedData[CompressedData.Length - 1] & 8) == 8)
                            CompressedData = CompressedData.Combine(new byte[1]);

                        CompressedData = CompressedData.Combine(new byte[] { 0x00, 0x00, 0xFF, 0xFF });
                        var CompressedAdler = Adler32(CompressedData);

                        compression.Write(UncompressedSize);
                        compression.Write(UncompressedAdler);
                        compression.Write(CompressedAdler);
                        compression.Write(CompressedData);

                        buffer = compression.ReadDataToSend();
                    }
                    else */
                    if (Authed && packet.Opcode != ServerMessage.ResetCompressionContext && buffer.Length > int.MaxValue)
                    {
                        Log.Message(LogType.Debug, "Compressing packet.");
                        var reset = new PacketWriter(ServerMessage.ResetCompressionContext);
                        Send(ref reset);

                        var compression = new PacketWriter(ServerMessage.Compression);

                        var msg = BitConverter.GetBytes((ushort)packet.Opcode);
                        var data = new byte[buffer.Length - 16];

                        Buffer.BlockCopy(buffer, 16, data, 0, data.Length);

                        data[0] = msg[0];
                        data[1] = msg[1];

                        byte[] CompressedData = null;

                        var UncompressedSize = data.Length;
                        var UncompressedAdler = Adler32(data);

                        // Compress.
                        using (var ms = new MemoryStream())
                        {
                            using (var ds = new DeflateStream(ms, CompressionLevel.Fastest))
                            {
                                ds.Write(data, 0, data.Length);
                                ds.Flush();
                            }

                            CompressedData = ms.ToArray();
                        }

                        CompressedData[0] -= 1;

                        if ((CompressedData[CompressedData.Length - 1] & 8) == 8)
                            CompressedData = CompressedData.Combine(new byte[1]);

                        CompressedData = CompressedData.Combine(new byte[] { 0x00, 0x00, 0xFF, 0xFF });
                        var CompressedAdler = Adler32(CompressedData);

                        compression.Write(UncompressedSize);
                        compression.Write(UncompressedAdler);
                        compression.Write(CompressedAdler);
                        compression.Write(CompressedData);

                        buffer = compression.ReadDataToSend();

                    }
                }

                //PacketLog.Write<ServerMessage>((ushort)packet.Opcode, buffer, clientSocket.RemoteEndPoint);
                clientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);

                packet.Flush();
            }
            catch (Exception ex)
            {
                Log.Message(LogType.Error, "{0}", ex.Message);
                Log.Message(LogType.Error, "{0}", ex.StackTrace);
                Log.Message();

                CharacterHandler.chatRunning = false;
            }
        }

        public void Dispose()
        {
            clientSocket?.Close();
            clientSocket?.Dispose();
            clientSocket = null;
            Crypt.Dispose();
        }
    }

    public sealed class WorldClass2 : IDisposable
    {
        public Account Account { get; set; }
        public Character Character { get; set; }

        public static WorldNetwork world;
        public Socket clientSocket;
        public Queue PacketQueue;
        public WoWCrypt Crypt;

        byte[] DataBuffer;
        public bool initiated = false;

        public WorldClass2()
        {
            DataBuffer = new byte[0x40000];
            PacketQueue = new Queue();
            Crypt = new WoWCrypt();
        }

        public void OnData()
        {
            PacketReader packet;

            if (PacketQueue.Count > 0)
                packet = (PacketReader)PacketQueue.Dequeue();
            else
                packet = new PacketReader(DataBuffer, false, initiated);

            PacketManager.InvokeHandler(ref packet, this);
        }

        public void OnConnect()
        {
            clientSocket.BeginReceive(DataBuffer, 0, DataBuffer.Length, SocketFlags.None, Receive, null);

            var transferInitiate = new PacketWriter();

            transferInitiate.WriteString("WORLD OF WARCRAFT CONNECTION - SERVER TO CLIENT - V2");

            var bufff = transferInitiate.ReadDataToSend(true).Concat(new byte[] { 0x0A }).ToArray();

            clientSocket.Send(bufff, 0, bufff.Length, SocketFlags.None);
        }

        public void Receive(IAsyncResult result)
        {
            try
            {
                var recievedBytes = clientSocket.EndReceive(result);
                if (recievedBytes != 0)
                {
                    if (!initiated)
                    {
                        if (recievedBytes == 1 && DataBuffer[0] == 0x0A || DataBuffer[recievedBytes - 1] == 0x0A)
                        {
                            var pktdata = new PacketReader(Encoding.ASCII.GetBytes("WORLD OF WARCRAFT CONNECTION - CLIENT TO SERVE"), false, false);

                            AuthenticationHandler.HandleAuthChallenge(ref pktdata, this);
                        }
                    }

                    else if (Crypt.IsInitialized)
                    {
                        while (recievedBytes > 0)
                        {
                            Decode(ref DataBuffer);

                            var length = BitConverter.ToUInt16(DataBuffer, 0) + 4;

                            var packetData = new byte[length];
                            Buffer.BlockCopy(DataBuffer, 0, packetData, 0, length);

                            PacketReader packet = new PacketReader(packetData, true, initiated);
                            PacketQueue.Enqueue(packet);

                            recievedBytes -= length;
                            Buffer.BlockCopy(DataBuffer, length, DataBuffer, 0, recievedBytes);

                            OnData();
                        }
                    }
                    else
                        OnData();

                    clientSocket?.BeginReceive(DataBuffer, 0, DataBuffer.Length, SocketFlags.None, Receive, null);
                }
            }
            catch (Exception ex)
            {
                Log.Message(LogType.Error, "{0}", ex.StackTrace);

                if (Character != null)
                    Manager.WorldMgr.DeleteSession(Character.Guid);

                CharacterHandler.chatRunning = false;
            }
        }

        void Decode(ref byte[] data)
        {
            Crypt.Decrypt(data, 4);

        }

        public void Send(ref PacketWriter packet)
        {
            var buffer = packet.ReadDataToSend();

            try
            {
                Log.Message(LogType.Packet, "Send[2] Opcode: {0} (0x{0:X}), Length: {1}", packet.Opcode, packet.Size);
                //Log.Message(LogType.Packet, $"Data ({packet.Opcode}): {BitConverter.ToString(buffer).Replace("-", " ")}");


                {/*
                    if ((int)packet.Opcode == (int)ServerMessage.ObjectUpdate && buffer.Length >= 0x1000)
                    {
                        Log.Message(LogType.Normal, "Compressing packet.");

                        var reset = new PacketWriter(ServerMessage.ResetCompressionContext);
                        var resetBuffer = reset.ReadDataToSend();

                        clientSocket.Send(resetBuffer, 0, resetBuffer.Length, SocketFlags.None);
                        
                        var compression = new PacketWriter(ServerMessage.Compression);

                        var msg = BitConverter.GetBytes((ushort)packet.Opcode);
                        var data = new byte[buffer.Length - 16];

                        Buffer.BlockCopy(buffer, 16, data, 0, data.Length);

                        byte[] CompressedData = null;

                        var UncompressedSize = data.Length;
                        var UncompressedAdler = Adler32(data);

                        // Compress.
                        using (var ms = new MemoryStream())
                        {
                            using (var ds = new DeflateStream(ms, CompressionLevel.Fastest))
                            {
                                ds.Write(data, 0, data.Length);
                                ds.Flush();
                            }

                            CompressedData = ms.ToArray();
                        }

                        //CompressedData[0] -= 1;

                        if ((CompressedData[CompressedData.Length - 1] & 8) == 8)
                            CompressedData = CompressedData.Combine(new byte[1]);

                        CompressedData = CompressedData.Combine(new byte[] { 0x00, 0x00, 0xFF, 0xFF });
                        var CompressedAdler = Adler32(CompressedData);

                        compression.Write(UncompressedSize);
                        compression.Write(UncompressedAdler);
                        compression.Write(CompressedAdler);
                        compression.Write(CompressedData);

                        buffer = compression.ReadDataToSend();
                    }
                    else */
                    if (packet.Opcode != ServerMessage.ResetCompressionContext && buffer.Length > int.MaxValue)
                    {
                        Log.Message(LogType.Debug, "Compressing packet.");
                        var reset = new PacketWriter(ServerMessage.ResetCompressionContext);
                        Send(ref reset);

                        var compression = new PacketWriter(ServerMessage.Compression);

                        var msg = BitConverter.GetBytes((ushort)packet.Opcode);
                        var data = new byte[buffer.Length - 16];

                        Buffer.BlockCopy(buffer, 16, data, 0, data.Length);

                        data[0] = msg[0];
                        data[1] = msg[1];

                        byte[] CompressedData = null;

                        var UncompressedSize = data.Length;
                        var UncompressedAdler = Adler32(data);

                        // Compress.
                        using (var ms = new MemoryStream())
                        {
                            using (var ds = new DeflateStream(ms, CompressionLevel.Fastest))
                            {
                                ds.Write(data, 0, data.Length);
                                ds.Flush();
                            }

                            CompressedData = ms.ToArray();
                        }

                        CompressedData[0] -= 1;

                        if ((CompressedData[CompressedData.Length - 1] & 8) == 8)
                            CompressedData = CompressedData.Combine(new byte[1]);

                        CompressedData = CompressedData.Combine(new byte[] { 0x00, 0x00, 0xFF, 0xFF });
                        var CompressedAdler = Adler32(CompressedData);

                        compression.Write(UncompressedSize);
                        compression.Write(UncompressedAdler);
                        compression.Write(CompressedAdler);
                        compression.Write(CompressedData);

                        buffer = compression.ReadDataToSend();

                    }
                }

                //PacketLog.Write<ServerMessage>((ushort)packet.Opcode, buffer, clientSocket.RemoteEndPoint);
                clientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);

                packet.Flush();
            }
            catch (Exception ex)
            {
                Log.Message(LogType.Error, "{0}", ex.Message);
                Log.Message(LogType.Error, "{0}", ex.StackTrace);
                Log.Message();

                CharacterHandler.chatRunning = false;
            }
        }
        public static uint Adler32(byte[] data)
        {
            var a = 0xD8F1u;
            var b = 0x9827u;

            for (var i = 0; i < data.Length; i++)
            {
                a = (a + data[i]) % 0xFFF1;
                b = (b + a) % 0xFFF1;
            }

            return (b << 16) + a;
        }
        public void Dispose()
        {
            clientSocket.Dispose();
            clientSocket = null;
            Crypt.Dispose();
        }
    }
}
