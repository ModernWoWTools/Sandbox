/*
 * Copyright (C) 2012-2014 Arctium Emulation <http://arctium.org>
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using AuthServer.Packets;
using Bgs.Protocol;
using Framework.Constants.Misc;
using Framework.Cryptography.BNet;
using Framework.Database.Auth.Entities;
using Framework.Logging;
using Google.Protobuf;

namespace AuthServer.Network
{
    public class AuthSession : IDisposable
    {
        public uint ClientBuild;
        public Account Account { get; set; }
        public List<Module> Modules { get; set; }
        public SRP6a SecureRemotePassword { get; set; }
        public BNetCrypt Crypt { get; set; }
        public bool IsMounted;

        Socket client;
        NetworkStream nStream;
        SslStream sslStream;
        byte[] dataBuffer = new byte[0x800];

        public AuthSession(Socket socket)
        {
            client = socket;
        }

        public string GetCInfo()
        {
            var endPoint = client.RemoteEndPoint as IPEndPoint;
            return endPoint.Address + ":" + endPoint.Port;
        }

        public void Accept()
        {
            nStream = new NetworkStream(client);
            sslStream = new SslStream(nStream, false, App_CertificateValidation);

            var cert = new X509Certificate(Globals.CertData);
            //var chain = X509Chain.Create().Build(cert);
            SslServerAuthenticationOptions op = new SslServerAuthenticationOptions();
            op.ApplicationProtocols = new List<SslApplicationProtocol>();
            op.ApplicationProtocols.Add(SslApplicationProtocol.Http11);
            op.ApplicationProtocols.Add(SslApplicationProtocol.Http2);
            op.CertificateRevocationCheckMode = X509RevocationMode.NoCheck;
            op.ClientCertificateRequired = false;
            op.EnabledSslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Tls13;
            op.RemoteCertificateValidationCallback = App_CertificateValidation;
            op.ServerCertificate = cert;
            op.AllowRenegotiation = true;
            sslStream.AuthenticateAsServer(op);

            try
            {
                var ctr = 0;

                do
                {
                    ctr = sslStream.Read(dataBuffer, 0, dataBuffer.Length);

                    var temp = new byte[ctr];
                    Array.Copy(dataBuffer, temp, ctr);

                    if (temp == null || temp.Length == 0)
                    {
                        client.Dispose();
                        return;
                    }

                    var bytesToRead = temp.Length;


                    while (bytesToRead != 0)
                    {


                        var length = (temp[0] << 8) | temp[1];
                        var header = Header.Parser.ParseFrom(new CodedInputStream(temp, 2, length));
                        var msgData = new byte[header.Size];

                        Array.Copy(temp, 2 + length, msgData, 0, msgData.Length);
                        PacketManager.InvokeHandler(this, header, msgData);

                        var next = new byte[temp.Length - (2 + length + msgData.Length)];

                        Array.Copy(temp, 2 + length + msgData.Length, next, 0, next.Length);

                        temp = next;

                        bytesToRead -= (2 + length + msgData.Length);
                    }
                } while (ctr != 0);
            }
            catch (Exception)
            {
                //Console.WriteLine(ex);
                nStream?.Dispose();
                sslStream?.Dispose();
                client?.Dispose();
                Dispose();
            }

        }


        bool App_CertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public void Send(byte[] rawdata) => sslStream.Write(rawdata);

        uint token = 0;

        public void Send(IMessage message)
        {
            try
            {
                var pData = message.ToByteArray();
                var header = new Header();

                // Server answer
                header.Token = token++;
                header.ServiceId = 0xFE;
                header.Size = (uint)pData.Length;
                

                var pHeader = header.ToByteArray();
                var dataToSend = new byte[2];

                dataToSend[0] = ((byte)((pHeader.Length >> 8)));
                dataToSend[1] = ((byte)((pHeader.Length & 0xff)));

                dataToSend = dataToSend.Concat(pHeader).Concat(pData).ToArray();

                if (dataToSend.Length != (2 + pHeader.Length + pData.Length))
                    Log.Message(LogType.Error, $"Wrong data length for '{message.ToString()}';");
                else
                    sslStream.Write(dataToSend);
            }
            catch (SocketException ex)
            {
                Log.Message(LogType.Error, "{0}", ex.Message);

                client.Close();
            }
        }

        public void Send(IMessage message, uint serviceHash, uint methodId)
        {
            try
            {
                var pData = message.ToByteArray();
                var header = new Header();

                // Server answer
                header.Token = token++;
                header.ServiceId = 0;
                header.ServiceHash = serviceHash;
                header.MethodId = methodId;
                header.Size = (uint)pData.Length;


                var pHeader = header.ToByteArray();
                var dataToSend = new byte[2];

                dataToSend[0] = ((byte)((pHeader.Length >> 8)));
                dataToSend[1] = ((byte)((pHeader.Length & 0xff)));

                dataToSend = dataToSend.Concat(pHeader).Concat(pData).ToArray();

                if (dataToSend.Length != (2 + pHeader.Length + pData.Length))
                    Log.Message(LogType.Error, $"Wrong data length for '{message.ToString()}';");
                else
                    sslStream.Write(dataToSend);
            }
            catch (Exception ex)
            {
                Log.Message(LogType.Error, "{0}", ex.Message);

                client.Close();
            }
        }

        void SendCompleted(object sender, SocketAsyncEventArgs e)
        {
        }

        public void Dispose()
        {
            client?.Dispose();

            client = null;
            Account = null;
            SecureRemotePassword = null;
        }
    }
}
