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
using System.IO;
using System.Numerics;

namespace Framework.Misc
{
    [System.Reflection.Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    public static class Extensions
    {
        #region BinaryReader
        public static Dictionary<Type, Func<BinaryReader, object>> ReadValue = new Dictionary<Type, Func<BinaryReader, object>>()
        {
            { typeof(bool),   br => br.ReadBoolean() },
            { typeof(sbyte),  br => br.ReadSByte()   },
            { typeof(byte),   br => br.ReadByte()    },
            { typeof(char),   br => br.ReadChar()    },
            { typeof(short),  br => br.ReadInt16()   },
            { typeof(ushort), br => br.ReadUInt16()  },
            { typeof(int),    br => br.ReadInt32()   },
            { typeof(uint),   br => br.ReadUInt32()  },
            { typeof(float),  br => br.ReadSingle()  },
            { typeof(long),   br => br.ReadInt64()   },
            { typeof(ulong),  br => br.ReadUInt64()  },
            { typeof(double), br => br.ReadDouble()  },
        };

        public static BigInteger ToBigInteger<T>(this T value, bool isBigEndian = false)
        {
            var ret = BigInteger.Zero;

            switch (typeof(T).Name)
            {
                case "Byte[]":
                    var data = value as byte[];

                    if (isBigEndian)
                        Array.Reverse(data);

                    ret = new BigInteger(data.Combine(new byte[] { 0 }));
                    break;
                case "BigInteger":
                    ret = (BigInteger)Convert.ChangeType(value, typeof(BigInteger));
                    break;
                default:
                    throw new NotSupportedException(string.Format("'{0}' conversion to 'BigInteger' not supported.", typeof(T).Name));
            }

            return ret;
        }

        public static T Read<T>(this BinaryReader br)
        {
            var type = typeof(T).IsEnum ? typeof(T).GetEnumUnderlyingType() : typeof(T);

            return (T)ReadValue[type](br);
        }
        #endregion
        #region UInt32
        public static uint LeftRotate(this uint value, int shiftCount)
        {
            return (uint)((value << shiftCount) | (value >> (0x20 - shiftCount)));
        }
        #endregion
        #region String
        public static byte[] ToByteArray(this string s)
        {
            var data = new byte[s.Length / 2];

            for (int i = 0; i < s.Length; i += 2)
                data[i / 2] = Convert.ToByte(s.Substring(i, 2), 16);

            return data;
        }
        #endregion
        #region ByteArray
        public static long FindPattern(this byte[] data, byte[] pattern, long start, long baseOffset = 0)
        {
            long matches;

            for (long i = start; i < data.Length; i++)
            {
                if (pattern.Length > (data.Length - i))
                    return 0;

                for (matches = 0; matches < pattern.Length; matches++)
                {
                    if ( (data[i + matches] != (byte)pattern[matches]))
                        break;
                }

                if (matches == pattern.Length)
                    return baseOffset + i;
            }

            return 0;
        }

        public static long FindPattern(this byte[] data, byte[] pattern, long baseOffset = 0) => FindPattern(data, pattern, 0L, baseOffset);

        public static HashSet<long> FindPattern(this byte[] data, byte[] pattern, int maxMatches, long maxOffset)
        {
            var matchList = new HashSet<long>();

            long match = 0;

            do
            {
                match = data.FindPattern(pattern, match, 0);

                if (match != 0)
                {
                    if (!matchList.Contains(match))
                        matchList.Add(match);

                    match += pattern.Length;
                }

            } while ((matchList.Count < maxMatches || match < maxOffset) && match != 0);

            return matchList;
        }

        public static byte[] GenerateRandomKey(this byte[] s, int length)
        {
            var random = new Random((int)((uint)(Guid.NewGuid().GetHashCode() ^ 1 >> 89 << 2 ^ 42)).LeftRotate(13));
            var key = new byte[length];

            for (int i = 0; i < length; i++)
            {
                int randValue = -1;

                do
                {
                    randValue = (int)((uint)random.Next(0xFF)).LeftRotate(1) ^ i;
                } while (randValue > 0xFF && randValue <= 0);

                key[i] = (byte)randValue;
            }

            return key;
        }

        public static bool Compare(this byte[] b, byte[] b2)
        {
            for (int i = 0; i < b2.Length; i++)
                if (b[i] != b2[i])
                    return false;

            return true;
        }

        public static byte[] Combine(this byte[] data, byte[] data2)
        {
            var combined = new byte[data.Length + data2.Length];

            Buffer.BlockCopy(data, 0, combined, 0, data.Length);
            Buffer.BlockCopy(data2, 0, combined, data.Length, data2.Length);

            return combined;
        }

        public static string ToHexString(this byte[] data)
        {
            var hex = "";

            foreach (var b in data)
                hex += string.Format("{0:X2}", b);

            return hex.ToUpper();
        }

        public static BigInteger MakeBigInteger(this byte[] data, bool isBigEndian = false)
        {
            if (isBigEndian)
                Array.Reverse(data);

            return new BigInteger(data.Combine(new byte[] { 0 }));
        }
        #endregion
        #region Generic
        public static BigInteger AssignValue<T>(this T value, bool isBigEndian = false)
        {
            var ret = BigInteger.Zero;

            switch (typeof(T).Name)
            {
                case "Byte[]":
                    ret = (value as byte[]).MakeBigInteger(isBigEndian);
                    break;
                case "BigInteger":
                    ret = (BigInteger)Convert.ChangeType(value, typeof(BigInteger));
                    break;
                default:
                    throw new NotSupportedException(string.Format("'{0}' conversion to 'BigInteger' not supported.", typeof(T).Name));
            }

            return ret;
        }
        #endregion
    }
}
