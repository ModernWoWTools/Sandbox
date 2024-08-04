using Framework.Constants.Net;
using Framework.Misc;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using Framework.ObjectDefines;
using System.Collections.Generic;

namespace Framework.Network.Packets
{
    [System.Reflection.Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    public class PacketReader : BinaryReader
    {
        public ClientMessage Opcode { get; set; }
        public uint Size { get; set; }
        public byte[] Storage { get; set; }

        public PacketReader(byte[] data, bool stuff)  : base(new MemoryStream(data))
        {
        }

        public PacketReader(byte[] data, bool worldPacket = true, bool initiated = true, bool raw = false) : base(new MemoryStream(data))
        {
            if (raw)
                return;

            if (!initiated)
            {
                Opcode = (ClientMessage)this.ReadUInt16();

                Storage = new byte[data.Length];
                Buffer.BlockCopy(data, 0, Storage, 0, (int)data.Length);
                return;
            }
            if (worldPacket)
            {
                Size = this.Read<uint>();
                {
                    ReadUInt64(); // wth...
                    ReadUInt32(); // ok...
                    Opcode = (ClientMessage)this.ReadUInt16();

                    //if (Opcode == ClientMessage.TransferInitiate)
                    //Size = (ushort)((size % 0x100) + (size / 0x100));
                    //else
                    //Size = size;

                    Storage = new byte[Size - 2];
                    Buffer.BlockCopy(data, 6 + 8 + 4, Storage, 0, (int)Size - 2);
                }
            }
            else
            { 
                Size = this.Read<uint>();
                {
                    ReadUInt64(); // wth...
                    ReadUInt32(); // ok...
                    Opcode = (ClientMessage)this.ReadUInt16();

                    //if (Opcode == ClientMessage.TransferInitiate)
                    //Size = (ushort)((size % 0x100) + (size / 0x100));
                    //else
                    //Size = size;

                    Storage = new byte[Size - 2];
                    if (Size > 2)
                        Buffer.BlockCopy(data, 6 + 8 + 4, Storage, 0, (int)Size - 2);
                }
            }
        }

        public T Read<T>()
        {
            return Extensions.Read<T>(this);
        }

        public SmartGuid ReadGuid()
        {
            var rawBytes = new List<byte>();
            var sgudi = new SmartGuid();
            byte gLength = this.Read<byte>();
            byte b = this.Read<byte>();

            rawBytes.Add(gLength);
            rawBytes.Add(b);

            sgudi.Guid = GetSmartGuid(gLength, ref rawBytes);

            if (b > 0)
            {
                sgudi.HighGuid = GetSmartGuid(b, ref rawBytes);
            }

            sgudi.Raw = rawBytes.ToArray();

            return sgudi;
        }

        public ulong ReadSmartGuid()
        {
            var rawBytes = new List<byte>();
            byte gLength = this.Read<byte>();
            byte b = this.Read<byte>();
            ulong arg_21_0 = GetSmartGuid(gLength, ref rawBytes);
            if (b > 0)
            {
                GetSmartGuid(b, ref rawBytes);
            }

            rawBytes = null;

            return arg_21_0;
        }
        public ulong GetSmartGuid(byte gLength, ref List<byte> rawList)
        {
            ulong num = 0uL;
            for (byte b = 0; b < 8; b += 1)
            {
                if ((1 << (int)b & (int)gLength) != 0)
                {
                    var val = Read<byte>();

                    rawList.Add(val);

                    num |= (ulong)((long)((long)val << (int)(b * 8)));
                }
            }
            return num;
        }
        public string ReadCString()
        {
            StringBuilder tmpString = new StringBuilder();
            char tmpChar = base.ReadChar();
            char tmpEndChar = Convert.ToChar(Encoding.UTF8.GetString(new byte[] { 0 }), CultureInfo.InvariantCulture);

            while (tmpChar != tmpEndChar)
            {
                tmpString.Append(tmpChar);
                tmpChar = base.ReadChar();
            }

            return tmpString.ToString();
        }

        public string ReadString(uint count)
        {
            byte[] stringArray = ReadBytes(count);
            return Encoding.UTF8.GetString(stringArray);
        }

        public byte[] ReadBytes(uint count)
        {
            return base.ReadBytes((int)count);
        }

        public string ReadStringFromBytes(uint count)
        {
            byte[] stringArray = ReadBytes(count);
            Array.Reverse(stringArray);

            return UTF8Encoding.UTF8.GetString(stringArray);
        }

        public string ReadIPAddress()
        {
            byte[] ip = new byte[4];

            for (int i = 0; i < 4; ++i)
                ip[i] = this.Read<byte>();

            return ip[0] + "." + ip[1] + "." + ip[2] + "." + ip[3];
        }

        public string ReadAccountName()
        {
            StringBuilder nameBuilder = new StringBuilder();

            byte nameLength = this.Read<byte>();
            char[] name = new char[nameLength];

            for (int i = 0; i < nameLength; i++)
            {
                name[i] = base.ReadChar();
                nameBuilder.Append(name[i]);
            }

            return nameBuilder.ToString().ToUpper(CultureInfo.InvariantCulture);
        }

        public void Skip(int count)
        {
            base.BaseStream.Position += count;
        }
    }
}
