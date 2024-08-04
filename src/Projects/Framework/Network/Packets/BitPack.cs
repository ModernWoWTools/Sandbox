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

using System;
using System.Globalization;
using System.Text;

namespace Framework.Network.Packets
{
    [System.Reflection.Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    public class BitPack
    {
        PacketWriter writer;

        byte[] GuidBytes;
        byte[] GuildGuidBytes;
        byte[] TransportGuidBytes;
        byte[] TargetGuidBytes;

        byte BitPosition { get; set; }
        byte BitValue { get; set; }

        public ulong Guid { set { GuidBytes = BitConverter.GetBytes(value); } }
        public ulong GuildGuid { set { GuildGuidBytes = BitConverter.GetBytes(value); } }
        public ulong TargetGuid { set { TargetGuidBytes = BitConverter.GetBytes(value); } }
        public ulong TransportGuid { set { TransportGuidBytes = BitConverter.GetBytes(value); } }

        public BitPack(PacketWriter writer, ulong guid = 0, ulong guildGuid = 0, ulong targetGuid = 0, ulong transportGuid = 0)
        {
            Guid = guid;
            GuildGuid = guildGuid;
            TargetGuid = targetGuid;
            TransportGuid = transportGuid;

            this.writer = writer;
            BitPosition = 8;
        }

        public void Write<T>(T bit)
        {
            --BitPosition;

            if (Convert.ToBoolean(bit, CultureInfo.InvariantCulture))
                BitValue |= (byte)(1 << (BitPosition));

            if (BitPosition == 0)
            {
                BitPosition = 8;
                writer.WriteUInt8(BitValue);
                BitValue = 0;
            }
        }

        public void Write<T>(T bit, int count)
        {
            checked
            {
                for (int i = count - 1; i >= 0; --i)
                    Write<T>((T)Convert.ChangeType(((Convert.ToUInt32(bit) >> i) & 1), typeof(T)));
            }
        }

        public void WriteStringLength(string data, int count)
        {
            byte[] sBytes = UTF8Encoding.UTF8.GetBytes(data);
            var length = sBytes.Length;

            Write<int>(length, count);
        }

        public void WriteGuidMask(params byte[] order)
        {
            for (byte i = 0; i < order.Length; i++)
                Write(GuidBytes[order[i]]);
        }

        public void WriteGuildGuidMask(params byte[] order)
        {
            for (byte i = 0; i < order.Length; i++)
                Write(GuildGuidBytes[order[i]]);
        }

        public void WriteTargetGuidMask(params byte[] order)
        {
            for (byte i = 0; i < order.Length; i++)
                Write(TargetGuidBytes[order[i]]);
        }

        public void WriteTransportGuidMask(params byte[] order)
        {
            for (byte i = 0; i < order.Length; i++)
                Write(TransportGuidBytes[order[i]]);
        }

        public void WriteGuidBytes(params byte[] order)
        {
            for (byte i = 0; i < order.Length; i++)
                if (GuidBytes[order[i]] != 0)
                    writer.WriteUInt8((byte)(GuidBytes[order[i]] ^ 1));
        }

        public void WriteGuildGuidBytes(params byte[] order)
        {
            for (byte i = 0; i < order.Length; i++)
                if (GuildGuidBytes[order[i]] != 0)
                    writer.WriteUInt8((byte)(GuildGuidBytes[order[i]] ^ 1));
        }

        public void WriteTargetGuidBytes(params byte[] order)
        {
            for (byte i = 0; i < order.Length; i++)
                if (TargetGuidBytes[order[i]] != 0)
                    writer.WriteUInt8((byte)(TargetGuidBytes[order[i]] ^ 1));
        }

        public void WriteTransportGuidBytes(params byte[] order)
        {
            for (byte i = 0; i < order.Length; i++)
                if (TransportGuidBytes[order[i]] != 0)
                    writer.WriteUInt8((byte)(TransportGuidBytes[order[i]] ^ 1));
        }

        public void Flush()
        {
            if (BitPosition != 0)
            {
                writer.WriteUInt8(BitValue);
                BitValue = 0;
                BitPosition = 8;
            }
        }
    }
}
