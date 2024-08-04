using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using AuthServer.AuthServer.Attributes;
using AuthServer.AuthServer.JsonObjects;
using AuthServer.Network;

using Bgs.Protocol;
using Bgs.Protocol.GameUtilities.V1;

using Framework.Constants.Net;
using Framework.Misc;
using Framework.Serialization;

using Google.Protobuf;

namespace AuthServer.AuthServer.Packets.BnetHandlers
{
    [BnetService(BnetServiceHash.GameUtilitiesService)]
    public class GameUtilitiesService : BnetServiceBase
    {
        [BnetMethod(1)]
        public static void HandleClientRequest(AuthSession session, ClientRequest clientRequest)
        {
            if (clientRequest.Attribute[0].Name == "Command_RealmListTicketRequest_v1_b9")
            {
                foreach (var a in clientRequest.Attribute)
                {
                    if (a.Name == "Param_ClientInfo")
                    {
                        var jsonVal = a.Value.BlobValue.ToStringUtf8().Replace("JSONRealmListTicketClientInformation:", "");
                        var realmListTicket = Json.CreateObject<RealmListTicketClientInformation>(jsonVal);

                        Globals.Secret = realmListTicket.info.secret.Select(x => Convert.ToByte(x)).ToArray();
                    }
                }

                var response = new ClientResponse();

                response.Attribute.Add(new Bgs.Protocol.Attribute
                {
                    Name = "Param_RealmListTicket",
                    Value = new Variant { BlobValue = ByteString.CopyFromUtf8("Nope") }
                });

                session.Send(response);
            }
            else if (clientRequest.Attribute[0].Name == "Command_LastCharPlayedRequest_v1_b9")
            {
                session.Send(new ClientResponse());
            }
            else if (clientRequest.Attribute[0].Name == "Command_RealmListRequest_v1_b9")
            {
                var realmlist = new RealmListUpdates
                {
                    updates = new List<RealmListUpdate>
                    {
                        new RealmListUpdate
                        {
                            wowRealmAddress = 1,
                            update = new Update
                            {
                                wowRealmAddress = 1,
                                cfgTimezonesID = 2,
                                populationState = 1,
                                cfgCategoriesID = 1,
                                version = new ClientVersion
                                {
                                    versionMajor = 9,
                                    versionMinor = 0,
                                    versionRevision = 1,
                                    versionBuild = (int)Globals.WoWBuild
                                },
                                cfgRealmsID = 1,
                                // Support latest build+ future ones
                                flags = session.ClientBuild < Globals.WoWBuild ? 5 | 1 : 4,
                                cfgConfigsID = 1,
                                cfgLanguagesID = 1,
                                #if PUBLIC
                                name = "Arctium"
#else
                                name = "Internal"
#endif
                            }
                        },
                    }
                };

                var response = new ClientResponse();

                response.Attribute.Add(new Bgs.Protocol.Attribute
                {
                    Name = "Param_RealmList",
                    Value = new Variant
                    {
                        BlobValue = ByteString.CopyFrom(GetCompressedData("JSONRealmListUpdates", Json.CreateString(realmlist)))
                    }
                });

                var realmCharacterCountList = new RealmCharacterCountList();

                response.Attribute.Add(new Bgs.Protocol.Attribute
                {
                    Name = "Param_CharacterCountList",
                    Value = new Variant
                    {
                        BlobValue = ByteString.CopyFrom(GetCompressedData("JSONRealmCharacterCountList", Json.CreateString(realmCharacterCountList)))
                    }
                });

                session.Send(response);
            }
            else if (clientRequest.Attribute[0].Name == "Command_RealmJoinRequest_v1_b9")
            {
                // Read
                // Param_RealmAddress, Selected RealmId, 1
                // Param_RealmListTicket, from Command_RealmListTicketRequest_v1_b9 answer
                // Param_BnetSessionKey, 64 bytes
                Globals.JoinSecret = new byte[0].GenerateRandomKey(32);

                SHA256 sha256 = new SHA256Managed();

                sha256.TransformBlock(Globals.Secret, 0, Globals.Secret.Length, Globals.Secret, 0);
                sha256.TransformFinalBlock(Globals.JoinSecret, 0, Globals.JoinSecret.Length);

                Globals.SessionKey = sha256.Hash;

                var response = new ClientResponse();

                // Param_RealmJoinTicket, server defined, goes to auth session packet
                response.Attribute.Add(new Bgs.Protocol.Attribute
                {
                    Name = "Param_RealmJoinTicket",
                    Value = new Variant
                    {
                        BlobValue = ByteString.CopyFromUtf8("arctium")
                    }
                });

                // Param_ServerAddresses, JSONRealmListServerIPAddresses, zlib
                var realmListServerIPAddresses = new RealmListServerIPAddresses();
                realmListServerIPAddresses.families = new List<Family>();

                // IPv4
                realmListServerIPAddresses.families.Add(new Family
                {
                    family = 1,
                    addresses = new List<JsonObjects.Address>()
                    {
                        new JsonObjects.Address
                        {
                            ip = "127.0.0.1",
                            port = Sandbox.RealmPort
                        }
                    }
                });

                realmListServerIPAddresses.families.Add(new Family
                {
                    family = 2,
                    addresses = new List<JsonObjects.Address>()
                });

                response.Attribute.Add(new Bgs.Protocol.Attribute
                {
                    Name = "Param_ServerAddresses",
                    Value = new Variant
                    {
                        BlobValue = ByteString.CopyFrom(GetCompressedData("JSONRealmListServerIPAddresses", Json.CreateString(realmListServerIPAddresses)))
                    }
                });

                // Param_JoinSecret, 32 bytes, random
                response.Attribute.Add(new Bgs.Protocol.Attribute
                {
                    Name = "Param_JoinSecret",
                    Value = new Variant
                    {
                        BlobValue = ByteString.CopyFrom(Globals.JoinSecret)
                    }
                });

                session.Send(response);
            }
        }

        public static byte[] GetCompressedData(string name, string data)
        {
            var rData = Encoding.UTF8.GetBytes(name + ":" + data + "\0");

            var nextData = Compress(rData, CompressionLevel.Fastest);
            var adlerVal = BitConverter.GetBytes(Adler32_Default(rData)).Reverse();

            return BitConverter.GetBytes(rData.Length).Concat(new byte[] { 0x78, 0x9C }).Concat(nextData).Concat(adlerVal).ToArray();
        }

        public static uint Adler32_Default(byte[] data)
        {
            var a = 1u;
            var b = 0u;

            for (var i = 0; i < data.Length; i++)
            {
                a = (a + data[i]) % 0xFFF1;
                b = (b + a) % 0xFFF1;
            }
            return (b << 16) + a;
        }

        [BnetMethod(10)]
        public static void HandleGetAllValuesForAttributeRequest(AuthSession session, GetAllValuesForAttributeRequest getAllValuesForAttributeRequest)
        {
            if (getAllValuesForAttributeRequest.AttributeKey == "Command_RealmListRequest_v1_b9")
            {
                var getAllValuesForAttributeResponse = new GetAllValuesForAttributeResponse();

                getAllValuesForAttributeResponse.AttributeValue.Add(new Variant
                {
                    StringValue = "0-0-0"
                });

                session.Send(getAllValuesForAttributeResponse);
            }
        }

        static byte[] Compress(byte[] data, CompressionLevel level)
        {
            using (var ms = new MemoryStream())
            {
                using (var ds = new DeflateStream(ms, level))
                {
                    ds.Write(data, 0, data.Length);
                    ds.Flush();
                }

               return ms.ToArray();
            }
        }
    }

}
