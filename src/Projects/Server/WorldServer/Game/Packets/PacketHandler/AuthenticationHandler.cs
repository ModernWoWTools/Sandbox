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
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using AuthServer.Network;
using AuthServer.WorldServer.Managers;

using Framework.Constants.Net;
using Framework.Cryptography.WoW;
using Framework.Database.Auth.Entities;
using Framework.Misc;
using Framework.Network.Packets;

namespace AuthServer.Game.Packets.PacketHandler
{
    public class AuthenticationHandler : Manager
    {
        //[Opcode(ClientMessage.TransferInitiate, "18322")]
        public static void HandleAuthChallenge(ref PacketReader packet, WorldClass session)
        {
            session.initiated = true;
            WorldMgr.Sessions.Clear();
            WorldMgr.Sessions2.Clear();

            PacketWriter authChallenge = new PacketWriter(ServerMessage.AuthChallenge, false);

            //authChallenge.WriteUInt32(0x97F68328);

            for (int i = 0; i < 8; i++)
                authChallenge.WriteUInt32(0);

            Globals.ServerSalt = new byte[0].GenerateRandomKey(16);

            authChallenge.WriteBytes(Globals.ServerSalt);

            authChallenge.WriteUInt8(1);

            session.Send(ref authChallenge);

        }

        static byte[] clientSeed, serverSeed;

        //[Opcode2(ClientMessage.TransferInitiate, "18322")]
        public static void HandleAuthChallenge(ref PacketReader packet, WorldClass2 session)
        {
            session.initiated = true;
            PacketWriter authChallenge = new PacketWriter(ServerMessage.AuthChallenge, false);

            //authChallenge.WriteUInt32(0x97F68328);

            authChallenge.Write(clientSeed = new byte[16]);
            authChallenge.Write(serverSeed = new byte[16]);

            authChallenge.Write(new byte[16]);

            authChallenge.WriteUInt8(1);

            session.Send(ref authChallenge);


        }
        static byte[] sessionKey = new byte[40];

        static byte[] arr1 = { 0x58, 0xCB, 0xCF, 0x40, 0xFE, 0x2E, 0xCE, 0xA6, 0x5A, 0x90, 0xB8, 0x01, 0x68, 0x6C, 0x28, 0x0B };
        static byte[] arr2 = { 0x16, 0xAD, 0x0C, 0xD4, 0x46, 0xF9, 0x4F, 0xB2, 0xEF, 0x7D, 0xEA, 0x2A, 0x17, 0x66, 0x4D, 0x2F };


        static byte[] sha2_3_grml = new byte[32];
        [Opcode(ClientMessage.AuthSession, "18125")]
        public static void HandleAuthResponse(ref PacketReader packet, WorldClass session)
        {
            /*var hmacsha1 = new HMACSHA1(Globals.SessionKey);
            var hmacsha2 = new HMACSHA1(Globals.SessionKey);
            var wowStr = Encoding.ASCII.GetBytes("WoW\0");
            var clientSalt2 = BitConverter.GetBytes(Globals.ClientSalt);
            var serverSalt = BitConverter.GetBytes(0x97F68328);

            hmacsha1.TransformBlock(wowStr, 0, wowStr.Length, wowStr, 0);
            hmacsha1.TransformBlock(clientSalt2, 0, clientSalt2.Length, clientSalt2, 0);
            hmacsha1.TransformFinalBlock(serverSalt, 0, serverSalt.Length);

            hmacsha2.TransformBlock(wowStr, 0, wowStr.Length, wowStr, 0);
            hmacsha2.TransformBlock(serverSalt, 0, serverSalt.Length, serverSalt, 0);
            hmacsha2.TransformFinalBlock(clientSalt2, 0, clientSalt2.Length);

            var firstHash = hmacsha1.Hash;
            var secondHash = hmacsha2.Hash;

            var kBytes = new byte[40];
            Array.Copy(firstHash, 0, kBytes, 0, firstHash.Length);
            Array.Copy(secondHash, 0, kBytes, firstHash.Length, secondHash.Length);

            session.Crypt.Initialize(sessionKey = kBytes);*/

            using (var crypt = new RsaCrypt())
            {
                /*var ecrypt2 = new PacketWriter(ServerMessage.EnableCrypt);
                var bitpack = new BitPack(ecrypt2);

                crypt.InitializeEncryption(RsaStore.D, RsaStore.P, RsaStore.Q, RsaStore.DP, RsaStore.DQ, RsaStore.InverseQ);
                crypt.InitializeDecryption(RsaStore.Exponent, RsaStore.Modulus);

                // We just need dummy data here.
                var encrypted = crypt.Encrypt(new byte[256].GenerateRandomKey(256));
                var eData = new byte[0x100];

                Array.Copy(encrypted, eData, 0x100);

                ecrypt2.Write(eData);

                // Don't enable crypt.
                bitpack.Write(false);
                bitpack.Flush();*/

                //session.Send(ref ecrypt2);
            }


            BitUnpack BitUnpack = new BitUnpack(packet);

            packet.Skip(23);

            Globals.ClientSalt = packet.ReadBytes(16);
            packet.ReadBytes(24);

            BitUnpack.GetBit();

            var ticket = packet.ReadBytes(packet.ReadUInt32());

            var hmacsha1 = new HMACSHA256(Globals.SessionKey);
            hmacsha1.TransformBlock(Globals.ServerSalt, 0, Globals.ServerSalt.Length, Globals.ServerSalt, 0);
            hmacsha1.TransformBlock(Globals.ClientSalt, 0, Globals.ClientSalt.Length, Globals.ClientSalt, 0);
            hmacsha1.TransformFinalBlock(arr1, 0, arr1.Length); // seed

            var hash1 = hmacsha1.Hash;
            var sha2 = new SHA256Managed();
            var sha2_2 = new SHA256Managed();
            var sha2_3 = new SHA256Managed();
            // ComputeHash?!?!
            sha2.TransformFinalBlock(hash1, 0, hash1.Length >> 1);

            // ComputeHash?!?!
            sha2_2.TransformFinalBlock(hash1, (hash1.Length >> 1), hash1.Length - (hash1.Length >> 1));


            sha2_3.TransformBlock(sha2.Hash, 0, 0x20, sha2.Hash, 0);
            sha2_3.TransformBlock(sha2_3_grml, 0, 0x20, sha2_3_grml, 0);
            sha2_3.TransformFinalBlock(sha2_2.Hash, 0, 0x20);

            sha2_3_grml = sha2_3.Hash;

            for (int i = 0, i2 = 0; i < 40; i++, i2++)
            {
                if (i == 32)
                {
                    sha2_3.Initialize();

                    sha2_3.TransformBlock(sha2.Hash, 0, 0x20, sha2.Hash, 0);
                    sha2_3.TransformBlock(sha2_3_grml, 0, 0x20, sha2_3_grml, 0);
                    sha2_3.TransformFinalBlock(sha2_2.Hash, 0, 0x20);

                    sha2_3_grml = sha2_3.Hash;

                    i2 = 0;
                }

                sessionKey[i] = sha2_3_grml[i2];
            }
            //session.Crypt.Initialize(sessionKey);

            var ecrypt = new PacketWriter(ServerMessage.EnableCrypt, false);
            var hmac = new HMACSHA256(new byte[16]);

            hmac.TransformBlock(new byte[1], 0, 1, null, 0);
            hmac.TransformFinalBlock(new byte[] { 0x90, 0x9C, 0xD0, 0x50, 0x5A, 0x2C, 0x14, 0xDD, 0x5C, 0x2C, 0xC0, 0x64, 0x14, 0xF3, 0xFE, 0xC9 }, 0, 16);

            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(new RSAParameters
                {
                    Modulus = new byte[] { 0xee, 0xb3, 0xdc, 0xd4, 0xd3, 0xc3, 0xb4, 0x54, 0x51, 0xce, 0x66, 0x5b, 0xcb, 0x32, 0xb8, 0xf0, 0xf7, 0x92, 0x53, 0xc6, 0x19, 0xf2, 0x0c, 0x85, 0x2f, 0x8a, 0x26, 0xa9, 0x7a, 0x45, 0x9f, 0x60, 0xc4, 0xeb, 0xcd, 0xea, 0x7f, 0x8d, 0x59, 0xd8, 0x57, 0xb2, 0x60, 0x7b, 0x09, 0x4c, 0x9b, 0x68, 0xb8, 0xc7, 0xed, 0xef, 0x1e, 0x80, 0x0d, 0xe6, 0x6b, 0x37, 0x5b, 0x53, 0x90, 0xeb, 0x18, 0x13, 0x0d, 0x7f, 0x43, 0x64, 0x83, 0xda, 0x98, 0xe6, 0xac, 0xc2, 0x30, 0xa2, 0x82, 0xa5, 0xc6, 0xcb, 0xc7, 0xfb, 0x86, 0x9f, 0x9f, 0xa9, 0x02, 0x6a, 0x03, 0x49, 0xc5, 0x38, 0xfb, 0xc0, 0xc8, 0x55, 0xcc, 0xc0, 0xce, 0x25, 0x91, 0xbe, 0x85, 0xcf, 0xd1, 0xd1, 0x37, 0xce, 0xcc, 0x83, 0xd2, 0xea, 0x30, 0x80, 0x07, 0x7b, 0x80, 0x9f, 0x9d, 0x44, 0x54, 0x22, 0x29, 0xbe, 0x86, 0xda, 0xdb, 0x48, 0xc5, 0xa9, 0xf9, 0x13, 0x36, 0x95, 0x23, 0x76, 0xf1, 0x0e, 0xdc, 0x84, 0x0d, 0x94, 0x02, 0x12, 0xa8, 0x97, 0xf3, 0x3b, 0x14, 0xee, 0xaa, 0x6f, 0x98, 0x05, 0x27, 0x4e, 0x1f, 0xa3, 0x60, 0xa5, 0xa9, 0xda, 0xd8, 0x17, 0xdf, 0x33, 0xcb, 0xe2, 0x13, 0x54, 0x8b, 0x18, 0xb0, 0xca, 0xb9, 0xbb, 0x88, 0x64, 0x06, 0xdf, 0x75, 0xa6, 0xd7, 0x61, 0x00, 0xbb, 0xb0, 0x5a, 0x0e, 0x7a, 0xd4, 0x77, 0x08, 0x4d, 0x15, 0xe2, 0x10, 0x83, 0xb0, 0x04, 0xaa, 0x9e, 0x8b, 0x77, 0xa9, 0x06, 0x89, 0x5d, 0x08, 0x5d, 0x0f, 0xb8, 0x2e, 0x6b, 0xc1, 0xcb, 0x64, 0xcf, 0x6e, 0x5c, 0xdb, 0x4f, 0x58, 0x65, 0x08, 0x51, 0xfb, 0x0d, 0x48, 0x1a, 0x6f, 0xb6, 0x3d, 0x1f, 0x0b, 0xdd, 0xfe, 0x1b, 0x1d, 0xf0, 0xbf, 0xb0, 0x27, 0x6b, 0xf5, 0x8e, 0xbc, 0xc7, 0x40, 0x01, 0xff, 0xa7, 0x0b, 0x80, 0xd6, 0x5f },
                    Exponent = new byte[] { 0x01, 0x00, 0x01 },
                    P = new byte[] { 0xf3, 0xb3, 0x59, 0x68, 0x6f, 0x5a, 0xf3, 0xf3, 0x28, 0x6f, 0xa1, 0xa0, 0x63, 0x80, 0x55, 0x2c, 0x72, 0x55, 0x39, 0x2c, 0xf3, 0x15, 0xd3, 0x72, 0x30, 0x0f, 0xb8, 0x2d, 0xf4, 0x9b, 0xb7, 0x38, 0x0e, 0x37, 0x64, 0x52, 0x67, 0x27, 0x83, 0xd0, 0x9a, 0x43, 0xa3, 0x0c, 0x17, 0xb2, 0xcc, 0x39, 0x5c, 0xec, 0x94, 0x51, 0xcb, 0x63, 0xd9, 0xc2, 0xcb, 0x76, 0x53, 0x02, 0xa4, 0x37, 0xdd, 0xce, 0x4e, 0x05, 0xfc, 0xf1, 0x1a, 0x92, 0x5a, 0x03, 0x25, 0x6a, 0x5a, 0xb2, 0x89, 0xf7, 0x96, 0x6b, 0xab, 0xd3, 0xfe, 0x4e, 0xab, 0x74, 0xfd, 0xdf, 0xe7, 0xe7, 0x35, 0x49, 0x78, 0x77, 0x75, 0x0e, 0xb3, 0x58, 0xdc, 0x27, 0x5c, 0x86, 0x43, 0xf0, 0x5f, 0xad, 0x3c, 0x91, 0x4d, 0xc1, 0x28, 0x67, 0x1f, 0x0c, 0xbb, 0xd9, 0x89, 0xe2, 0x2b, 0x6e, 0x56, 0x42, 0xae, 0x2d, 0xe1, 0xb9, 0xbd, 0x7d },
                    Q = new byte[] { 0xfa, 0xbf, 0xf0, 0x40, 0x12, 0x52, 0xea, 0x40, 0xf2, 0x40, 0xf8, 0xf5, 0x93, 0xf4, 0x8c, 0x0a, 0x55, 0x21, 0x5a, 0x1c, 0x80, 0x0f, 0x00, 0xe8, 0x77, 0x4d, 0xe1, 0x1d, 0x34, 0x07, 0x73, 0xd0, 0x65, 0x78, 0x9c, 0xa3, 0x5e, 0x65, 0x72, 0xfb, 0xfa, 0x56, 0x84, 0xde, 0xda, 0x10, 0xb8, 0x63, 0x80, 0xb6, 0xdf, 0xa3, 0xf1, 0xa6, 0xdd, 0xa2, 0x89, 0x8c, 0x2c, 0x52, 0xe2, 0xa0, 0x66, 0xa9, 0x42, 0xb0, 0x02, 0xf1, 0xa8, 0x49, 0x5b, 0xb1, 0xd4, 0x1a, 0x36, 0x66, 0x37, 0x1f, 0x17, 0xbb, 0x17, 0xf4, 0x15, 0xc1, 0x3a, 0x51, 0x53, 0x1b, 0xe6, 0xcf, 0x54, 0x26, 0x54, 0xa1, 0xa9, 0x2c, 0x4f, 0x25, 0xcd, 0x83, 0xb1, 0xac, 0x03, 0x57, 0xeb, 0x2a, 0x45, 0x96, 0x92, 0x04, 0x2e, 0x39, 0xe2, 0xb7, 0xa3, 0xfa, 0x7d, 0x21, 0x9a, 0x01, 0x97, 0xdd, 0xef, 0x12, 0x07, 0x13, 0x1a, 0x0b },
                    DP = new byte[] { 0xbf, 0x9b, 0x9c, 0x08, 0x88, 0xc5, 0x32, 0x59, 0x54, 0xc9, 0xb0, 0x82, 0xb2, 0xb9, 0x0c, 0x3e, 0xce, 0x06, 0x43, 0xd6, 0x1b, 0xaa, 0x65, 0x7d, 0xba, 0x5c, 0x21, 0xab, 0xc0, 0x4f, 0x4e, 0x57, 0x3c, 0x96, 0x40, 0xc2, 0xa9, 0x68, 0x60, 0x33, 0x7b, 0x97, 0x02, 0x73, 0x57, 0x8e, 0xb5, 0x13, 0xc5, 0x04, 0x1d, 0xf9, 0xd1, 0xb8, 0xc8, 0x56, 0x68, 0x32, 0xb8, 0x41, 0xb3, 0x6c, 0x59, 0x71, 0x03, 0xd7, 0xa1, 0x70, 0xd0, 0x3b, 0x91, 0x9a, 0x37, 0xa8, 0xcf, 0x76, 0xe1, 0x13, 0x51, 0xa9, 0x86, 0x88, 0x3f, 0xc3, 0x0a, 0xfc, 0x09, 0x40, 0x2a, 0xcd, 0x97, 0x10, 0x15, 0x2f, 0x97, 0x51, 0xdf, 0xa1, 0x56, 0x37, 0x7c, 0x7f, 0xf4, 0x9a, 0xe2, 0x1f, 0x0b, 0x28, 0x13, 0xc2, 0x73, 0x49, 0xe1, 0x17, 0xd9, 0xfa, 0xc8, 0x76, 0x93, 0x3f, 0x45, 0x83, 0x57, 0xff, 0xab, 0x22, 0xa6, 0xe1 },
                    DQ = new byte[] { 0xfa, 0xbc, 0x05, 0x44, 0x3c, 0x4e, 0x76, 0xf9, 0xbc, 0x84, 0x65, 0xae, 0x43, 0x65, 0x8c, 0xc7, 0x24, 0x3b, 0x36, 0x7d, 0x3b, 0x3c, 0x45, 0x7f, 0x2f, 0x15, 0x4c, 0x1e, 0x44, 0x91, 0x93, 0x28, 0x76, 0xde, 0xf5, 0x5a, 0x93, 0x20, 0xc4, 0xaa, 0xdc, 0xb7, 0xb9, 0x23, 0x92, 0x50, 0x97, 0xe0, 0xbf, 0xab, 0x9d, 0xd3, 0xb4, 0xe7, 0x53, 0xc6, 0x9c, 0xdd, 0xf2, 0xd0, 0x6a, 0xd7, 0x53, 0xf0, 0xba, 0xc3, 0x80, 0x1e, 0x1f, 0x56, 0x43, 0x7f, 0x7d, 0x85, 0x3c, 0x92, 0x57, 0x5a, 0x1b, 0x03, 0xb5, 0xbf, 0x18, 0x25, 0xe0, 0xe7, 0xc3, 0x40, 0x2a, 0x3b, 0x75, 0x7c, 0xe2, 0xb1, 0xc8, 0x95, 0x8b, 0x0b, 0x55, 0x06, 0xf4, 0x19, 0xfa, 0x39, 0xe6, 0xe8, 0x9d, 0x2f, 0x24, 0x11, 0x54, 0x1c, 0x03, 0xed, 0xf7, 0xe5, 0x99, 0x4c, 0x89, 0x50, 0x40, 0x88, 0xe6, 0xef, 0x52, 0xed, 0xb1, 0xe3 },
                    InverseQ = new byte[] { 0x52, 0x02, 0x1b, 0x3a, 0x96, 0xc8, 0x6d, 0xcf, 0xeb, 0xa3, 0x47, 0x77, 0xe6, 0x73, 0x84, 0x2a, 0x25, 0x51, 0xaa, 0x79, 0x7b, 0xc4, 0x04, 0xf0, 0x15, 0x9b, 0xac, 0x98, 0xa0, 0xa6, 0x11, 0x2f, 0x05, 0x6c, 0xfe, 0xb1, 0x6f, 0xdd, 0x62, 0xcd, 0x2c, 0xec, 0x30, 0x05, 0x0e, 0xa5, 0xcf, 0xb3, 0xfd, 0x46, 0xa6, 0x18, 0x07, 0x9c, 0x15, 0x95, 0xa4, 0x6f, 0x12, 0x16, 0xfd, 0xfa, 0x50, 0xaf, 0x7b, 0xfc, 0x55, 0x73, 0x13, 0x1e, 0x67, 0x91, 0xa4, 0x38, 0x13, 0xf1, 0x57, 0x28, 0x18, 0x57, 0xd9, 0x7f, 0xc0, 0xd7, 0x44, 0xdb, 0x93, 0x73, 0x38, 0x76, 0x2d, 0x28, 0x6d, 0xc6, 0x2b, 0x71, 0x84, 0x70, 0x50, 0xf0, 0x65, 0x4d, 0x8b, 0x3c, 0x7a, 0xb6, 0xd4, 0x4a, 0xe8, 0x50, 0x07, 0x6d, 0x45, 0xf6, 0xeb, 0x27, 0x82, 0x96, 0xdb, 0x27, 0x3c, 0x8a, 0x0b, 0x57, 0x2b, 0x14, 0xc1, 0x63 },
                    D = new byte[] { 0x22, 0x9a, 0xe6, 0xaf, 0xe0, 0x07, 0x66, 0x34, 0x37, 0x2b, 0xe2, 0x00, 0xfa, 0xc3, 0x5e, 0xb6, 0x68, 0x5d, 0xc9, 0x51, 0x55, 0xdf, 0x96, 0x5b, 0x14, 0x9a, 0x45, 0xa2, 0x9a, 0x3c, 0x4f, 0xaf, 0xba, 0xbc, 0xa8, 0xbc, 0x8f, 0x43, 0x51, 0xbc, 0x20, 0x72, 0x96, 0xb4, 0x1f, 0x94, 0x00, 0x8f, 0xbd, 0x02, 0x17, 0x07, 0x6c, 0x77, 0x8a, 0x0c, 0x56, 0x8c, 0xce, 0xeb, 0x9d, 0x7d, 0xc7, 0x9e, 0xb3, 0x7d, 0x38, 0xaa, 0xf0, 0xc6, 0x97, 0x16, 0x12, 0x03, 0x91, 0x03, 0x6e, 0x47, 0x54, 0x3b, 0xa4, 0xc1, 0x5d, 0x31, 0xf4, 0xf6, 0x8e, 0x88, 0x09, 0xf3, 0xfe, 0xe8, 0x94, 0xee, 0xcc, 0xdc, 0x4b, 0x73, 0xc4, 0x2f, 0x04, 0x23, 0x07, 0xc9, 0x2a, 0x14, 0xd7, 0xaf, 0x5e, 0x4c, 0xda, 0x1d, 0xe3, 0x6c, 0x1c, 0x29, 0x96, 0x6b, 0x0d, 0x64, 0xa3, 0x81, 0xd4, 0x65, 0x6f, 0xad, 0x78, 0xce, 0x9b, 0x52, 0xad, 0x39, 0x9e, 0x02, 0x4d, 0x33, 0x34, 0x5a, 0xb3, 0xda, 0x2d, 0x50, 0xd3, 0xf5, 0xac, 0x7c, 0xa7, 0x29, 0x23, 0x98, 0x5c, 0x35, 0xea, 0xf1, 0x8f, 0x8f, 0xf4, 0x79, 0x0e, 0x4c, 0xbd, 0x56, 0x96, 0x9b, 0xb5, 0xf6, 0x4e, 0xbb, 0xf0, 0x04, 0x5b, 0x6e, 0x7d, 0x5c, 0x31, 0x22, 0x42, 0x04, 0xeb, 0x07, 0x81, 0x20, 0xf9, 0x2e, 0x06, 0x26, 0x31, 0xea, 0x03, 0x33, 0xd9, 0x06, 0x63, 0x32, 0xff, 0x18, 0x65, 0x0c, 0xae, 0x28, 0x31, 0x77, 0x9f, 0xa9, 0x74, 0x9c, 0x7c, 0x3e, 0x30, 0xd1, 0x1c, 0x6e, 0xb8, 0x21, 0x6b, 0xea, 0x5c, 0x4b, 0x3d, 0x9c, 0xf4, 0x4b, 0x7e, 0x41, 0x2b, 0x59, 0x08, 0x5a, 0x62, 0x24, 0xba, 0xff, 0xbd, 0x79, 0x0b, 0x88, 0xe0, 0x7a, 0xf5, 0x0b, 0x25, 0x70, 0x72, 0x1e, 0x1f, 0x91, 0xfb, 0xeb, 0xa7, 0xce, 0x31, 0xf2, 0xdb, 0xc0, 0x16, 0x79 },

                });

                ecrypt.WriteBytes(rsa.SignHash(hmac.Hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1).Reverse().ToArray());
            }

            // Crypt disabled.
            ecrypt.WriteUInt8(0);

            //session.Send(ref ecrypt);

            //return;

            // var hmacsha2 = new HMACSHA256(Globals.SessionKey);

            //var length = BitUnpack.GetBits<uint>(11);
            //var name = packet.ReadString(length);

            //packet.Skip(1);

            /*var packedAddonSize = packet.Read<uint>();
            var unpackedAddonSize = packet.Read<uint>();

            packet.Skip(2);

            var unpackedAddonData = new byte[unpackedAddonSize];


            if (packedAddonSize > 4)
             {
                 var packedAddonData = packet.ReadBytes(packedAddonSize - 4);
 
                 using (var deflate = new DeflateStream(new MemoryStream(packedAddonData), CompressionMode.Decompress))
                 {
                     var decompressed = new MemoryStream();
                     deflate.CopyTo(decompressed);
 
 
                     decompressed.Seek(0, SeekOrigin.Begin);
 
                     for (int i = 0; i < unpackedAddonSize; i++)
                         unpackedAddonData[i] = (byte)decompressed.ReadByte();
                 }
             }*/


            // uint nameLength = BitUnpack.GetBits<uint>(11);
            //string accountName = packet.ReadString(nameLength);

            //             SQLResult result = DB.Realms.Select("SELECT * FROM accounts WHERE name = ?", accountName);
            //             if (result.Count == 0)
            //                 session.clientSocket.Close();
            //             else
            //                 session.Account = new Account()
            //                  {
            //                      Id         = result.Read<int>(0, "id"),
            //                      Name       = result.Read<string>(0, "name"),
            //                      Password   = result.Read<string>(0, "password"),
            //                      SessionKey = result.Read<string>(0, "sessionkey"),
            //                      Expansion  = result.Read<byte>(0, "expansion"),
            //                      GMLevel    = result.Read<byte>(0, "gmlevel"),
            //                      IP         = result.Read<string>(0, "ip"),
            //                      Language   = result.Read<string>(0, "language")
            //                  };
            // 
            //             string K = session.Account.SessionKey;
            //             byte[] kBytes = new byte[K.Length / 2];
            // 
            //             for (int i = 0; i < K.Length; i += 2)
            //                 kBytes[i / 2] = Convert.ToByte(K.Substring(i, 2), 16);

            session.Account = new Account
            {
                Id = 1,
                Email = "arctium@arctium",
                PasswordVerifier = "48CC8FA29A386EDDF16A6681E821D966A044FCA0CA8D8D984BE1DD4910A19B75130A4F01316844851DAA86F2AA6E43BBEDEED5F237CE98BE4722BFB9482E99609769171EADB85F8EBCCC0B84F4B3976ADA7FAF6A44AD5827354FB87CE358371DD2493CD79F0B6927A6FE218561218D7C7D5E20EB194434924E62FBB1B22EFD38",
                Salt = "5ECB46A13455B03D28FD3AD1866122AF689BF611F81F3C512A6DC447EC4F2E5F",
                IP = "",
                SessionKey = "",
                SecurityFlags = 0,
                OS = "Win",
                Expansion = 6,
                IsOnline = false
            };



            //session.Crypt.Initialize(kBytes);
            //uint realmId = 1;
            //             SQLResult realmClassResult = DB.Realms.Select("SELECT class, expansion FROM realm_classes WHERE realmId = ?", realmId);
            //             SQLResult realmRaceResult = DB.Realms.Select("SELECT race, expansion FROM realm_races WHERE realmId = ?", realmId);

            var defaultAllowedClasses = new byte[,] { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 2 },
                                                                  { 7, 0 }, { 8, 0 }, { 9, 0 }, { 10, 4 }, { 11, 6 }, { 12, 6 } };

            // Default race/expansion data (sent in AuthResponse)
            var defaultAllowedRaces = new byte[,] { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 },
                                                                { 7, 0 }, { 8, 0 }, { 9, 3 }, { 10, 1 }, { 11, 1 }, { 22, 3 },
                                                                { 24, 4 }, { 25, 4 }, { 26, 4 } };

            var HasAccountData = true;
            var IsInQueue = false;

            PacketWriter authResponse = new PacketWriter(ServerMessage.AuthResponse);
            BitPack BitPack = new BitPack(authResponse);

            // uint32
            authResponse.WriteUInt32(0x00);// result

            BitPack.Write(HasAccountData);
            BitPack.Write(IsInQueue);

            BitPack.Flush();

            if (HasAccountData)
            {
                authResponse.WriteUInt32(1); // 0x20, virtualRealmAddress
                authResponse.WriteUInt32(0); // counter, realmInfo
                authResponse.WriteUInt32(0); // 0x34, timeRemain
                authResponse.WriteUInt8(8); // 0x40
                authResponse.WriteUInt8(8); // 0x41
                authResponse.WriteUInt32(0); // 0x44, timeSecondsUntilPCKick
                authResponse.WriteInt32(WorldMgr.ChrRaces.Count); // raceclass
                authResponse.WriteUInt32(1); // char templates
                authResponse.WriteUInt32(4); // 0x84, currencyID
                authResponse.WriteUnixTime();

                for (int i = 0; i < WorldMgr.ChrRaces.Count; i++)
                {
                    authResponse.WriteUInt8((byte)WorldMgr.ChrRaces[i].Id);

                    authResponse.WriteInt32(defaultAllowedClasses.Length / 2);

                    for (int j = 0; j < defaultAllowedClasses.Length / 2; j++)
                    {
                        authResponse.WriteUInt8(defaultAllowedClasses[j, 0]); // classid
                        authResponse.WriteUInt8(defaultAllowedClasses[j, 1]); // expid

                        authResponse.WriteUInt8(0); // isdisabled, enable all
                    }
                }
                /*for (int i = 0; i < defaultAllowedClasses.Length / 2; i++)
                {
                    authResponse.WriteUInt8(defaultAllowedClasses[i, 0]); // 0x40

                    authResponse.WriteUInt32(0);
                    //authResponse.WriteUInt8(defaultAllowedClasses[i, 1]); // 0x41
                }*/

                BitPack.Write(0);
                BitPack.Write(0);
                BitPack.Write(0);
                BitPack.Write(0);
                BitPack.Write(0);
                BitPack.Flush();

                // idk
                authResponse.WriteUInt32(0);
                authResponse.WriteUInt32(0);
                authResponse.WriteUInt32(0);
                BitPack.Write(0);
                BitPack.Write(0);
                BitPack.Write(0);
                BitPack.Flush();

                for (int i = 0; i < 1; i++)
                {
                    authResponse.WriteUInt32(50);
                    authResponse.WriteInt32(defaultAllowedClasses.Length);

                    // Alli = 3, Horde = 5 
                    for (int j = 0; j < defaultAllowedClasses.Length / 2; j++)
                    {
                        authResponse.WriteUInt8(defaultAllowedClasses[j, 0]);
                        authResponse.WriteUInt8(3);
                    }

                    for (int j = 0; j < defaultAllowedClasses.Length / 2; j++)
                    {
                        authResponse.WriteUInt8(defaultAllowedClasses[j, 0]);
                        authResponse.WriteUInt8(5);
                    }

                    BitPack.Write("Level 60 - Zereth Mortis".Length, 7);
                    BitPack.Write("Start in the new zone 'Zereth Mortis'".Length, 10);

                    BitPack.Flush();

                    authResponse.WriteString("Level 60 - Zereth Mortis");
                    authResponse.WriteString("Start in the new zone 'Zereth Mortis'");
                }
            }

            session.Send(ref authResponse);

            // Initialize available hotfixes.
            var pkt = new PacketWriter(ServerMessage.AvailableHotfixes);
            pkt.WriteUInt32(838926338);
            pkt.WriteUInt32(0);
            session.Send(ref pkt);

            //Hotfix.SendHotfixReply(session);

            //Hotfix.SendClearHotfixes(session);
            // Send hotfixes
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDFE5000000082CD65B0000000000".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDF7A000000082CD65B0000000000".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDFE9000000082CD65B0000000000".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDF49000000082CD65B0000000000".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDFEC000000082CD65B0000000000".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDF71000000082CD65B0000000000".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDFEE000000082CD65B0000000000".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDF7C000000082CD65B0000000000".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDFF1000000082CD65B0000000000".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDFE8000000082CD65B0000000000".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDF70000000082CD65B0000000000".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDF5F000000082CD65B0000000000".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDF6D000000082CD65B0000000000".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDFE6000000082CD65B0000000000".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDFEA000000082CD65B0000000000".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDF6F000000082CD65B0000000000".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDFEF000000082CD65B0000000000".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDF6E000000082CD65B0000000000".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDF6C000000082CD65B0000000000".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDF68000000082CD65B0000000000".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDF6B000000082CD65B0000000000".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDFE7000000082CD65B0000000000".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDFED000000082CD65B0000000000".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDFE1000000082CD65B0000000000".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDFF0000000082CD65B0000000000".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDF44000000082CD65B0000000000".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDF43000000082CD65B801000000076583BDACD5257A3F73D1598A2CA2D99".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDF47000000082CD65B8010000000972B6E74420EC519E6F9D97D594AA37C".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDF4B000000082CD65B8010000000CD0C0FFAAD9363EC14DD25ECDD2A5B62".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDF46000000082CD65B80100000009A89CC7E3ACB29CF14C60BC13B1E4616".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDF3D000000082CD65B80100000003832D7C42AAC9268F00BE7B6B48EC9AF".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDF48000000082CD65B8010000000AB55AE1BF0C7C519AFF028C15610A45B".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDF3B000000082CD65B8010000000C6C5F6C7F735D7D94C87267FA4994D45".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDF3F000000082CD65B8010000000D83BBCB46CC438B17A48E76C4F5654A3".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDF42000000082CD65B80100000004DD0DC82B101C80ABAC0A4D57E67F859".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDF3A000000082CD65B8010000000D1AC20FD14957FABC27196E9F6E7024A".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDF3C000000082CD65B801000000072A97A24A998E3A5500F3871F37628C0".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDF3E000000082CD65B8010000000C2501A72654B96F86350C5A927962F7A".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDF45000000082CD65B80100000008ACE8DB169E2F98AC36AD52C088E77C1".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDF4A000000082CD65B8010000000946D5659F2FAF327C0B7EC828B748ADB".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDF40000000082CD65B8010000000F0FDE1D29B274F6E7DBDB7FF815FE910".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9634); pkt.Write("CF532FDF41000000082CD65B8010000000857090D926BB28AEDA4BF028CACC4BA3".ToByteArray()); session.Send(ref pkt);
            //pkt = new PacketWriter((ServerMessage)9637); pkt.Write("0500000007284CB9D9DD00008CCA00007E010000800931C83AF2DA00008CCA000002000000801122990FF2DA00008CCA00007E00000080562D697E711201008FCA00001300000080562D697E701201008FCA0000130000008024020000000000000000000000D9DD000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000B06000000000000000FFFF000000000000000000000000000000000000FFFF0000000000000000000000000000000000000001FFFF00000000FF0000000000000000FFFF0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000008B8BFFFFFFFFFFFFFFFF507650204576656E743A20426174746C6520666F72204E617A6A617461720000F2DA00008B8BD9DD000000000000FFFF000000000000000000000000FF000000000000000000000078000000000000000000000000000000000000000000000014717100000000000000000000000000000000000000701201000201F3000000320000000F0300000000000000040100000000000000000000000000".ToByteArray()); session.Send(ref pkt);

            //Hotfix.SendAvailableHotfixes(session);
            //Hotfix.SendClearHotfixes(session);
            Hotfix.SendHotfixMessage(session);

            // New Mount 830, 6mo sub
            //pkt = new PacketWriter((ServerMessage)0x25A5); pkt.Write("CF532FDFF9000000082CD65B8010000000520421C1070D930C045516D231C9D442".ToByteArray()); session.Send(ref pkt);



            // Only send these if the replay mode is disabled.
           
            {

                //WorldMgr.SendHotfixes(session);

                TimeHandler.HandleSetTimezoneInformation(ref session);

                // Glue features
                var featureSystemStatusGlueScreen = new PacketWriter(ServerMessage.FeatureSystemStatusGlueScreen);

                /* var bitPack = new BitPack(featureSystemStatusGlueScreen);

                 var IsDisabledByParentalControls = false; // Store
                 var IsEnabled = false;  // Store
                 var IsAvailable = false;  // PurchaseAPI

                 bitPack.Write(IsEnabled);
                 bitPack.Write(IsAvailable);
                 bitPack.Write(IsDisabledByParentalControls);

                 bitPack.Write(false);
                 bitPack.Write(false);
                 bitPack.Write(true);
                 bitPack.Write(false);
                 bitPack.Write(false);
                 bitPack.Write(false);
                 bitPack.Write(false);
                 bitPack.Write(false);
                 bitPack.Write(false);
                 bitPack.Write(false);
                 bitPack.Write(false);
                 bitPack.Write(false);
                 bitPack.Write(false);
                 bitPack.Write(false);
                 bitPack.Write(false);
                 bitPack.Flush();

                 featureSystemStatusGlueScreen.WriteUInt32(300);
                 featureSystemStatusGlueScreen.WriteUInt32(30);
                 featureSystemStatusGlueScreen.WriteUInt64(0);
                 featureSystemStatusGlueScreen.WriteUInt32(100); // chars
                 featureSystemStatusGlueScreen.WriteUInt32(0);
                 featureSystemStatusGlueScreen.WriteUInt32(180);
                 featureSystemStatusGlueScreen.WriteUInt32(3);
                 featureSystemStatusGlueScreen.WriteUInt32(0);
                 featureSystemStatusGlueScreen.WriteUInt32(0);
                 featureSystemStatusGlueScreen.WriteUInt32(7);
                 featureSystemStatusGlueScreen.WriteUInt32(7);
                 featureSystemStatusGlueScreen.WriteUInt32(0);
                 featureSystemStatusGlueScreen.WriteUInt16(0);*/

                featureSystemStatusGlueScreen.WriteBytes(new byte[] {0x5C, 0x2F, 0xC0, 0x50, 0x0A, 0x00, 0x00, 0x00, 0x60, 0xEA, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x0B, 0x03, 0x00, 0x00, 0x2C, 0x01, 0x00, 0x00, 0x1E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x32, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0xB4, 0x00, 0x00,
    0x00, 0x06, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02,
    0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00});

                session.Send(ref featureSystemStatusGlueScreen);
            }

            // Open the 2nd connection.
            HandleConnectTo(session, (ushort)Sandbox.InstancePort, 1, null);

            session.Authed = true;
        }

        public static void HandleConnectTo(WorldClass session, ushort port = 3724, byte conn = 0, WorldClass2 session2 = null)
        {
            var connectTo = new PacketWriter(ServerMessage.ConnectTo);

            using (var crypt = new RsaCrypt())
            {
                crypt.InitializeEncryption(RsaStore.D, RsaStore.P, RsaStore.Q, RsaStore.DP, RsaStore.DQ, RsaStore.InverseQ);
                crypt.InitializeDecryption(RsaStore.Exponent, RsaStore.Modulus);

                var truePayloadOrder = new byte[payloadOrder.Length];

                for (var i = 0; i < payloadOrder.Length; i++)
                {
                    if (payloadOrder[i] > 16)
                        truePayloadOrder[i] = (byte)(payloadOrder[i] - 3);
                    else
                        truePayloadOrder[i] = (byte)payloadOrder[i];
                }

                var payloadData = new byte[0xFF];

                // 17 - 20, adler32, changes with compression seed...
                payloadData[0] = 0x43;
                payloadData[1] = 0xfd;
                payloadData[2] = 0xb8;
                payloadData[3] = 0x22;

                // 16, unk
                payloadData[4] = 0x01; // unk = 1 (sniff), addressType?

                // 0 - 15, Address
                payloadData[5] = 0x7F; // IPv4 Address = 127.0.0.1
                payloadData[6] = 0x00;
                payloadData[7] = 0x00;
                payloadData[8] = 0x01;
                payloadData[9] = 0x00; // placeholder IPv6
                payloadData[10] = 0x00;
                payloadData[11] = 0x00;
                payloadData[12] = 0x00;
                payloadData[13] = 0x00;
                payloadData[14] = 0x00;
                payloadData[15] = 0x00;
                payloadData[16] = 0x00;
                payloadData[17] = 0x00;
                payloadData[18] = 0x00;
                payloadData[19] = 0x00;
                payloadData[20] = 0x00;





                var portB = BitConverter.GetBytes(port);
                // 22 - 23, Port
                payloadData[21] = portB[0]; // Port = 3724
                payloadData[22] = portB[1];

                var msg = "An island of peace\nCorruption is brought ashore\nPandarens will rise\n\0\0\0"; // Add 3 00 as filler to reach 71 bytes

                // 24 - 94, message
                Array.Copy(Encoding.ASCII.GetBytes(msg), 0, payloadData, 23, 71);

                // 94 - 125, compression seed, let's use a static one to get a static adler32
                Array.Copy(new byte[] { 0xD6, 0xAC, 0x21, 0xE6, 0xB2, 0x7B, 0x06, 0x3D, 0xA9, 0x9C, 0x09, 0x4B, 0xC7, 0x30, 0x48, 0x34, 0xD4, 0xF0, 0x55, 0x3B, 0x1B, 0x1D, 0xC9, 0x5B, 0xFD, 0x3C, 0xB9, 0x30, 0x9D, 0xF5, 0x40, 0xC0 }, 0, payloadData, 93, 32);

                // 126 - 233, unknown, same for one account
                Array.Copy(new byte[108], 0, payloadData, 125, 108);

                // 21, unk
                payloadData[233] = 0x2A; // 0x2A

                /* var hmac = new HMACSHA256(RsaStore.WherePacketHmac);
                 var tempbuff = new byte[256];

                 hmac.TransformBlock(payloadData, 5, 16, tempbuff, 0);
                 hmac.TransformBlock(payloadData, 4, 1, tempbuff, 0);
                 hmac.TransformBlock(payloadData, 21, 2, tempbuff, 0);
                 hmac.TransformBlock(payloadData, 23, 71, tempbuff, 0);
                 hmac.TransformBlock(payloadData, 93, 32, tempbuff, 0);
                 hmac.TransformBlock(payloadData, 125, 108, tempbuff, 0);
                 hmac.TransformFinalBlock(payloadData, 233, 1);*/

                // 234 - 253, hmac, hm???? let's send a random val...
                Array.Copy(new byte[0].GenerateRandomKey(20), 0, payloadData, 234, 20);


                //var dataOrder = new byte[payloadData.Length];

                //for (var i = 0; i < payloadData.Length; i++)
                //    dataOrder[i] = payloadData[truePayloadOrder[i]];

                var encrypted = crypt.Encrypt(payloadData);

                var eData = new byte[0x100];

                Array.Copy(encrypted, eData, 0x100);

                connectTo.Write(eData);

                connectTo.WriteUInt8(1);
                connectTo.WriteUInt32(0x0100007F);
                connectTo.WriteUInt16(port);

                connectTo.WriteUInt32(0xE); // Serial
                connectTo.WriteUInt8(conn); // 0 = connection 1, 1 = connection 2
                connectTo.WriteUInt64(0xAB1DBDBB062E4B9D); // Key
            }

            if (session2 != null)
                session2.Send(ref connectTo);
            else
                session.Send(ref connectTo);
        }
        public static WorldClass2 session2;
        [Opcode2(ClientMessage.AuthContinuedSession, "18125")]
        public static void HandleAuthContinuedSession(ref PacketReader packet, WorldClass2 session)
        {
            session2 = session;


            var sess1 = WorldMgr.Sessions.SingleOrDefault();

            //packet.Read<ushort>();
            var dosResponse = packet.Read<ulong>();
            var key = packet.Read<ulong>();
            var digest = packet.ReadBytes(20);

            if (session2 != null)
            {
                //session2.Account = sess1.Value.Account;

                if (!WorldMgr.AddSession2(0, ref session2))
                {
                    //Log.Message(LogType.Error, "A Character with Guid: {0} is already logged in", guid);
                    return;
                }

                var email = Encoding.UTF8.GetBytes("arctium@arctium");
                var sKey = sessionKey;
                var val = BitConverter.GetBytes(0x97F68328u);


                var sha1 = new SHA1Managed();
                sha1.TransformBlock(email, 0, email.Length, email, 0);
                sha1.TransformBlock(sKey, 0, 40, sKey, 0);
                sha1.TransformFinalBlock(val, 0, 4);
                var finalVal = sha1.Hash;  // ok ==  received digest

                //session2.Crypt.Initialize(sessionKey, new byte[16], new byte[16]);


                //using (var crypt = new RsaCrypt())
                {
                    /* var ecrypt = new PacketWriter(ServerMessage.EnableCrypt);
                     var bitpack = new BitPack(ecrypt);

                     crypt.InitializeEncryption(RsaStore.D, RsaStore.P, RsaStore.Q, RsaStore.DP, RsaStore.DQ, RsaStore.InverseQ);
                     crypt.InitializeDecryption(RsaStore.Exponent, RsaStore.Modulus);

                     // We just need dummy data here.
                     var encrypted = crypt.Encrypt(new byte[256]);
                     var eData = new byte[0x100];

                     Array.Copy(encrypted, eData, 0x100);

                     ecrypt.Write(eData);

                     // Don't enable crypt.
                     bitpack.Write(false);
                     bitpack.Flush();
 */
                    //session2.Send(ref ecrypt);
                }


                var resumeComs = new PacketWriter(ServerMessage.ResumeComms);

                ReplayManager.GetInstance().Assign2(ref session2);

                session2.Send(ref resumeComs);
            }
            //AddonHandler.HandleAddonInfo(addondataaaa, null, session);
            //var suspendComs = new PacketWriter(ServerMessage.SuspendComms);

            //suspendComs.WriteUInt32(0x14);

            //session.Send(ref suspendComs);

            //var resumeComs = new PacketWriter(ServerMessage.ResumeComms);
            //resumeComs.WriteUInt32(0xE);
            //session.Send(ref resumeComs);
            //
        }

        [Opcode(ClientMessage.test, "18125")]
        public static void Handletest(ref PacketReader packet, WorldClass session)
        {
            var resumeComs = new PacketWriter(ServerMessage.ResumeComms);


            session.Send(ref resumeComs);

            WorldMgr.GetSession2(session.Character != null ? session.Character.Guid : 0)?.Send(ref resumeComs);
        }

        static int[] payloadOrder = { 90, 129, 38, 80, 60, 73, 33, 209, 163, 171, 106, 185, 61, 58, 1, 181, 255, 63, 155, 62, 82, 115, 59, 125, 57, 102, 203, 189, 130, 224, 12, 253, 239, 199, 124, 97, 167, 30, 152, 53, 243, 248, 132, 104, 210, 223, 28, 34, 200, 20, 21, 22, 23, 54, 187, 204, 141, 198, 245, 98, 91, 85, 120, 234, 242, 219, 237, 166, 100, 218, 159, 27, 8, 221, 206, 173, 67, 84, 231, 37, 170, 230, 35, 2, 86, 88, 220, 172, 153, 150, 180, 225, 47, 190, 236, 154, 113, 75, 211, 11, 116, 161, 5, 143, 101, 50, 118, 196, 133, 183, 235, 96, 215, 41, 109, 165, 55, 122, 31, 134, 9, 232, 112, 92, 139, 194, 188, 147, 32, 89, 103, 222, 195, 51, 40, 158, 49, 178, 157, 93, 70, 99, 79, 68, 69, 4, 217, 46, 238, 123, 137, 56, 74, 246, 48, 176, 140, 168, 76, 212, 7, 197, 119, 250, 241, 39, 252, 213, 111, 36, 95, 229, 216, 247, 191, 108, 177, 214, 226, 249, 202, 24, 126, 149, 174, 257, 207, 254, 144, 233, 240, 87, 175, 107, 162, 146, 94, 179, 14, 42, 128, 148, 72, 45, 71, 142, 186, 25, 164, 131, 156, 201, 81, 227, 29, 105, 15, 192, 6, 121, 251, 65, 52, 78, 228, 64, 151, 114, 193, 136, 77, 43, 110, 13, 205, 135, 83, 66, 244, 26, 138, 169, 256, 10, 117, 3, 127, 16, 184, 44, 208, 0, 182, 160, 145 };
    }
}
