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
using Framework.Misc;

namespace Framework.Cryptography
{
    class RsaCrypt : IDisposable
    {
        BigInteger d, e, n, p, q, dp, dq, iq;
        bool isEncryptionInitialized;
        bool isDecryptionInitialized;

        public RsaCrypt()
        {
            Dispose();
        }

        public void InitializeEncryption(RsaData rsaData)
        {
            InitializeEncryption(rsaData.RsaParams.D, rsaData.RsaParams.P, rsaData.RsaParams.Q, rsaData.RsaParams.DP, rsaData.RsaParams.DQ, rsaData.RsaParams.InverseQ);
        }

        public void InitializeEncryption<T>(T d, T p, T q, T dp, T dq, T iq, bool isBigEndian = false)
        {
            this.d  = d.AssignValue(isBigEndian);
            this.p  = p.AssignValue(isBigEndian);
            this.q  = q.AssignValue(isBigEndian);
            this.dp = dp.AssignValue(isBigEndian);
            this.dq = dq.AssignValue(isBigEndian);
            this.iq = iq.AssignValue(isBigEndian);

            if (this.p.IsZero && this.q.IsZero)
                throw new InvalidOperationException("'0' isn't allowed for p or q");
            else
                isEncryptionInitialized = true;
        }

        public void InitializeDecryption(RsaData rsaData)
        {
            InitializeDecryption(rsaData.RsaParams.Exponent, rsaData.RsaParams.Modulus);
        }

        public void InitializeDecryption<T>(T e, T n, bool reverseBytes = false)
        {
            this.e = e.AssignValue(reverseBytes);
            this.n = n.AssignValue(reverseBytes);

            isDecryptionInitialized = true;
        }

        public byte[] Encrypt<T>(T data, bool isBigEndian = false)
        {
            if (!isEncryptionInitialized)
                throw new InvalidOperationException("Encryption not initialized");

            var bData = data.AssignValue(isBigEndian);

            var m1 = BigInteger.ModPow(bData % p, dp, p);
            var m2 = BigInteger.ModPow(bData % q, dq, q);

            var h = (iq * (m1 - m2)) % p;

            // Be sure to use the positive remainder
            if (h.Sign == -1)
                h = p + h;

            var m = m2 + h * q;

            return m.ToByteArray();
        }

        public byte[] Decrypt<T>(T data, bool isBigEndian = false)
        {
            if (!isDecryptionInitialized)
                throw new InvalidOperationException("Encryption not initialized");

            var c = data.AssignValue(isBigEndian);

            return BigInteger.ModPow(c, e, n).ToByteArray();
        }

        public void Dispose()
        {
            this.e  = 0;
            this.n  = 0;
            this.d  = 0;
            this.p  = 0;
            this.q  = 0;
            this.dp = 0;
            this.dq = 0;
            this.iq = 0;

            isEncryptionInitialized = false;
            isDecryptionInitialized = false;
        }
    }
}
