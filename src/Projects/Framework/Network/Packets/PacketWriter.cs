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

using Framework.Constants.Net;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using Framework.ObjectDefines;
using Framework.Constants;

namespace Framework.Network.Packets
{
    [System.Reflection.Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    public class PacketWriter : BinaryWriter
    {
        public ServerMessage Opcode { get; set; }
        public uint Size { get; set; }

        public PacketWriter() : base(new MemoryStream()) { }
        public PacketWriter(ServerMessage message, bool isWorldPacket = true) : base(new MemoryStream())
        {
            WritePacketHeader(message, isWorldPacket);
        }

        protected void WritePacketHeader(ServerMessage opcode, bool isWorldPacket = true)
        {
            Opcode = opcode;

            //WriteUInt8(0);
            //WriteUInt8(0);


            if (!isWorldPacket)
            {
                WriteUInt32(0);
                WriteUInt64(0); // wth...
                WriteUInt32(0); // like... wtf?!?!
                WriteUInt16((ushort)opcode);
            }
            else
            {
                WriteUInt32(0);
                WriteUInt64(0); // wth...
                WriteUInt32(0); // like... wtf?!?!
                WriteUInt16((ushort)opcode);

            }
        }

        public byte[] ReadDataToSend(bool isAuthPacket = false)
        {
            byte[] data = new byte[BaseStream.Length];
            Seek(0, SeekOrigin.Begin);

            for (int i = 0; i < BaseStream.Length; i++)
                data[i] = (byte)BaseStream.ReadByte();

            Size = (uint)data.Length - 4 - 8 - 4;
            if (!isAuthPacket)
            {
                var size = BitConverter.GetBytes(Size);

                data[0] = size[0];
                data[1] = size[1];
                data[2] = size[2];
                data[3] = size[3];
            }

            return data;
        }

        public void WriteInt8(sbyte data)
        {
            base.Write(data);
        }

        public void WriteInt16(short data)
        {
            base.Write(data);
        }

        public void WriteInt32(int data)
        {
            base.Write(data);
        }

        public void WriteInt64(long data)
        {
            base.Write(data);
        }

        public void WriteUInt8(byte data)
        {
            base.Write(data);
        }

        public void WriteUInt16(ushort data)
        {
            base.Write(data);
        }

        public void WriteUInt32(uint data)
        {
            base.Write(data);
        }

        public void WriteUInt64(ulong data)
        {
            base.Write(data);
        }

        public void WriteFloat(float data)
        {
            base.Write(data);
        }

        public void WriteDouble(double data)
        {
            base.Write(data);
        }

        public void WriteCString(string data)
        {
            byte[] sBytes = UTF8Encoding.UTF8.GetBytes(data);

            WriteBytes(sBytes);
            WriteUInt8(0);
        }

        public void WriteString(string data, bool nullIfEmpty = true)
        {
            byte[] sBytes = UTF8Encoding.UTF8.GetBytes(data);

            if (sBytes.Length == 0 && nullIfEmpty)
                sBytes = new byte[1];
            
            WriteBytes(sBytes);
        }

        public void WriteUnixTime(int minus = 0)
        {
            WriteInt64(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        }

        public void WritePackedTime(int hours = 0)
        {
            DateTime currentDate = DateTime.Now.AddHours(hours);
            var val = Convert.ToUInt32((currentDate.Year - 2000) << 24 | (currentDate.Month - 1) << 20 | (currentDate.Day - 1) << 14 | (int)currentDate.DayOfWeek << 11 | currentDate.Hour << 6 | currentDate.Minute);

            WriteUInt32(val);
        }

        public void WriteSmartGuid(SmartGuid guid)
        {
            byte loLength, hiLength, wLoLength, wHiLength;

            var loGuid = GetPackedGuid(guid.Guid, out loLength, out wLoLength);
            var hiGuid = GetPackedGuid(guid.HighGuid, out hiLength, out wHiLength);

            WriteUInt8(loLength);
            WriteUInt8(hiLength);
            Write(loGuid, 0, wLoLength);
            Write(hiGuid, 0, wHiLength);
        }
        public void WriteSmartGuid(ulong guid, ObjectType type, SmartGuid sGuid = null)
        {
            if (guid != 0)
            {
                byte length = 0;
                byte length2 = 0;
                byte wlen, wlen2;
                var pGuid = GetSmartGuid(guid, out length, out wlen);
                var pGuid2 = GetSmartGuid(sGuid == null ? 0x080F280000000000 : sGuid.HighGuid, out length2, out wlen2);
                WriteUInt8(length);
                WriteUInt8((byte)(length2));
                WriteBytes(pGuid, wlen);
                WriteBytes(pGuid2, wlen2);
            }
            else
            {
                WriteUInt8(0);
                WriteUInt8(0);
            }
            // WriteGuid(0);
        }
        public byte[] GetSmartGuid(ulong guid, out byte gLength, out byte written)
        {
            byte[] packedGuid = new byte[8];
            byte gul = 0;
            byte length = 0;

            for (byte i = 0; guid != 0; i++)
            {
                if ((guid & 0xFF) != 0)
                {
                    gul |= (byte)(1 << i);
                    packedGuid[length] = (byte)(guid & 0xFF);
                    ++length;
                }

                guid >>= 8;
            }

            gLength = gul;
            written = length;

            return packedGuid;
        }
        public void WriteSmartGuid(ulong guid, GuidType type = GuidType.Player)
        {
            var g = new SmartGuid { Type = type, CreationBits = guid, RealmId = 1 };

            if (guid == 0)
                g = new SmartGuid();
            WriteSmartGuid(g);
        }
        byte[] GetPackedGuid(ulong guid, out byte gLength, out byte written)
        {
            var packedGuid = new byte[8];
            byte gLen = 0;
            byte length = 0;

            for (byte i = 0; guid != 0; i++)
            {
                if ((guid & 0xFF) != 0)
                {
                    gLen |= (byte)(1 << i);
                    packedGuid[length] = (byte)(guid & 0xFF);
                    ++length;
                }

                guid >>= 8;
            }

            gLength = gLen;
            written = length;

            return packedGuid;
        }


        public void WriteBytes(byte[] data, int count = 0)
        {
            if (count == 0)
                Write(data);
            else
                Write(data, 0, count);
        }

        public void WriteBitArray(BitArray buffer, int len)
        {
            var bufferArray = new byte[Convert.ToInt32((buffer.Length + 8) / 8) + 1];

            if (len > bufferArray.Length)
                bufferArray = bufferArray.Concat(new byte[len - bufferArray.Length]).ToArray();

            buffer.CopyTo(bufferArray, 0);

            Write(bufferArray, 0, len);
        }

        public void WriteUInt32Pos(uint data, int pos)
        {
            Seek(pos, SeekOrigin.Begin);

            WriteUInt32(data);

            Seek((int)BaseStream.Length - 1, SeekOrigin.Begin);
        }
    }
}
