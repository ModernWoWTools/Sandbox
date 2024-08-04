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
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using Framework.Misc;

namespace Framework.Cryptography.BNet
{
    public sealed class SRP6a : IDisposable
    {
        public byte[] I { get; private set; }
        public byte[] S2 { get; private set; }
        public byte[] V { get; private set; }
        public byte[] B { get; private set; }
        public byte[] SessionKey { get; private set; }
        public byte[] WoWSessionKey { get; private set; }
        public byte[] ClientM { get; private set; }
        public byte[] ServerM { get; private set; }

        public readonly BigInteger gBN;
        public readonly BigInteger k;

        public readonly byte[] N;
        public readonly byte[] S;
        public readonly byte[] g;

        SHA256 sha256;
        BigInteger A;
        BigInteger BN;
        BigInteger v;
        BigInteger b;
        BigInteger s;

        public SRP6a(string salt, string accountName = "", string passwordVerifier = "")
        {
            sha256 = new SHA256Managed();

            I = sha256.ComputeHash(Encoding.UTF8.GetBytes(accountName));

            N = new byte[]
            {
                0xAB, 0x24, 0x43, 0x63, 0xA9, 0xC2, 0xA6, 0xC3, 0x3B, 0x37, 0xE4, 0x61, 0x84, 0x25, 0x9F, 0x8B,
                0x3F, 0xCB, 0x8A, 0x85, 0x27, 0xFC, 0x3D, 0x87, 0xBE, 0xA0, 0x54, 0xD2, 0x38, 0x5D, 0x12, 0xB7,
                0x61, 0x44, 0x2E, 0x83, 0xFA, 0xC2, 0x21, 0xD9, 0x10, 0x9F, 0xC1, 0x9F, 0xEA, 0x50, 0xE3, 0x09,
                0xA6, 0xE5, 0x5E, 0x23, 0xA7, 0x77, 0xEB, 0x00, 0xC7, 0xBA, 0xBF, 0xF8, 0x55, 0x8A, 0x0E, 0x80,
                0x2B, 0x14, 0x1A, 0xA2, 0xD4, 0x43, 0xA9, 0xD4, 0xAF, 0xAD, 0xB5, 0xE1, 0xF5, 0xAC, 0xA6, 0x13,
                0x1C, 0x69, 0x78, 0x64, 0x0B, 0x7B, 0xAF, 0x9C, 0xC5, 0x50, 0x31, 0x8A, 0x23, 0x08, 0x01, 0xA1,
                0xF5, 0xFE, 0x31, 0x32, 0x7F, 0xE2, 0x05, 0x82, 0xD6, 0x0B, 0xED, 0x4D, 0x55, 0x32, 0x41, 0x94,
                0x29, 0x6F, 0x55, 0x7D, 0xE3, 0x0F, 0x77, 0x19, 0xE5, 0x6C, 0x30, 0xEB, 0xDE, 0xF6, 0xA7, 0x86
            };

            S = salt.ToByteArray();

            g = new byte[] { 2 };

            BN = MakeBigInteger(N);
            gBN = MakeBigInteger(g);
            k = MakeBigInteger(sha256.ComputeHash(N.Combine(g)));
            v = MakeBigInteger(passwordVerifier.ToByteArray());
        }

        public void CalculateX(string accountName, string password, bool calcB)
        {
            I = sha256.ComputeHash(Encoding.UTF8.GetBytes(accountName));

            var p = sha256.ComputeHash(Encoding.UTF8.GetBytes(I.ToHexString() + ":" + password.ToUpper()));
            var x = MakeBigInteger(sha256.ComputeHash(S.Combine(p)));

            CalculateV(x, calcB);
        }

        void CalculateV(BigInteger x, bool calcB)
        {
            v = BigInteger.ModPow(gBN, x, BN);
            V = v.ToByteArray();

            if (calcB)
                CalculateB();
        }

        public void CalculateB()
        {
            var randBytes = new byte[0x80];

            var random = RNGCryptoServiceProvider.Create();
            random.GetBytes(randBytes);

            S2 = randBytes;

            b = MakeBigInteger(randBytes);
            B = GetBytes(((k * v + BigInteger.ModPow(gBN, b, BN)) % BN).ToByteArray(), 0x80);
        }

        public void CalculateU(byte[] a)
        {
            A = MakeBigInteger(a);

            CalculateS(MakeBigInteger(sha256.ComputeHash(a.Combine(B))));
        }

        void CalculateS(BigInteger u)
        {
            s = BigInteger.ModPow(((A * BigInteger.ModPow(v, u, BN)) % BN), b, BN);

            CalculateSessionKey();
        }

        public void CalculateSessionKey()
        {
            var sBytes = GetBytes(s.ToByteArray(), 0x80);

            var part1 = new byte[sBytes.Length / 2];
            var part2 = new byte[sBytes.Length / 2];

            for (int i = 0; i < part1.Length; i++)
            {
                part1[i] = sBytes[i * 2];
                part2[i] = sBytes[i * 2 + 1];
            }

            part1 = sha256.ComputeHash(part1);
            part2 = sha256.ComputeHash(part2);

            SessionKey = new byte[part1.Length + part2.Length];

            for (int i = 0; i < part1.Length; i++)
            {
                SessionKey[i * 2] = part1[i];
                SessionKey[i * 2 + 1] = part2[i];
            }

            CalculateWoWSessionKey(SessionKey);
        }

        public void CalculateWoWSessionKey(byte[] arr)
        {
            var sBytes = arr;

            var part1 = new byte[sBytes.Length / 2];
            var part2 = new byte[sBytes.Length / 2];

            for (int i = 0; i < part1.Length; i++)
            {
                part1[i] = sBytes[i * 2];
                part2[i] = sBytes[i * 2 + 1];
            }

            part1 = new SHA1Managed().ComputeHash(part1);
            part2 = new SHA1Managed().ComputeHash(part2);

            WoWSessionKey = new byte[part1.Length + part2.Length];

            for (int i = 0; i < part1.Length; i++)
            {
                WoWSessionKey[i * 2] = part1[i];
                WoWSessionKey[i * 2 + 1] = part2[i];
            }
        }

        public void CalculateClientM(byte[] a)
        {
            var NHash = sha256.ComputeHash(N);
            var gHash = sha256.ComputeHash(g);

            for (int i = 0; i < NHash.Length; i++)
                NHash[i] ^= (byte)gHash[i];

            // Concat all variables for M1 hash
            var hash = NHash.Combine(sha256.ComputeHash(Encoding.UTF8.GetBytes(I.ToHexString()))).Combine(S).Combine(a).Combine(B).Combine(SessionKey);

            ClientM = sha256.ComputeHash(hash);
        }

        public void CalculateServerM(byte[] m1)
        {
            ServerM = sha256.ComputeHash(GetBytes(A.ToByteArray(), 0x80).Combine(m1).Combine(SessionKey));
        }

        public byte[] GetBytes(byte[] data, int count = 0x40)
        {
            if (data.Length <= count)
                return data;

            var bytes = new byte[count];

            Buffer.BlockCopy(data, 0, bytes, 0, 0x40);

            return bytes;
        }

        public BigInteger MakeBigInteger(byte[] data)
        {
            return new BigInteger(data.Combine(new byte[] { 0 }));
        }

        public void Dispose()
        {
            SessionKey = null;
            ServerM = null;
        }
    }
}
