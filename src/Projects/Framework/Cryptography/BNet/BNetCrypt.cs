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
using System.Security.Cryptography;

namespace Framework.Cryptography.BNet
{
    public sealed class BNetCrypt : IDisposable
    {
        public bool IsInitialized { get; set; }

        static readonly byte[] ServerEncryptionKey = { 0x68, 0xE0, 0xC7, 0x2E, 0xDD, 0xD6, 0xD2, 0xF3, 0x1E, 0x5A, 0xB1, 0x55, 0xB1, 0x8B, 0x63, 0x1E };
        static readonly byte[] ServerDecryptionKey = { 0xDE, 0xA9, 0x65, 0xAE, 0x54, 0x3A, 0x1E, 0x93, 0x9E, 0x69, 0x0C, 0xAA, 0x68, 0xDE, 0x78, 0x39 };

        SARC4 SARC4Encrypt, SARC4Decrypt;

        public BNetCrypt(byte[] sessionKey)
        {
            IsInitialized = false;

            if (IsInitialized)
                throw new InvalidOperationException("PacketCrypt already initialized!");

            SARC4Encrypt = new SARC4();
            SARC4Decrypt = new SARC4();

            var DecryptSHA256 = new HMACSHA256(sessionKey);
            var EncryptSHA256 = new HMACSHA256(sessionKey);

            SARC4Encrypt.PrepareKey(EncryptSHA256.ComputeHash(ServerEncryptionKey));
            SARC4Decrypt.PrepareKey(DecryptSHA256.ComputeHash(ServerDecryptionKey));

            IsInitialized = true;
        }

        public void Encrypt(byte[] data, int count)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("PacketCrypt not initialized!");

            SARC4Encrypt.ProcessBuffer(data, count);
        }

        public void Decrypt(byte[] data, int count)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("PacketCrypt not initialized!");

            SARC4Decrypt.ProcessBuffer(data, count);
        }

        public void Dispose()
        {
            IsInitialized = false;
        }
    }
}
