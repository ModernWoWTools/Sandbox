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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using Arctium.WoW.Sandbox.Server.WorldServer.Game.Entities;

using AuthServer.Game.Entities;
using AuthServer.Game.Packets.PacketHandler;
using AuthServer.Network;
using AuthServer.WorldServer.Game.Chat.Commands;
using AuthServer.WorldServer.Game.Entities;
using AuthServer.WorldServer.Game.Packets.PacketHandler;
using AuthServer.WorldServer.Managers;

using Framework.Constants;
using Framework.Constants.Misc;
using Framework.Constants.Net;
using Framework.Logging;
using Framework.Misc;
using Framework.Network.Packets;
using Framework.ObjectDefines;
using Framework.Serialization;

namespace AuthServer.Game.PacketHandler
{
    public class CharacterHandler : Manager
    {
        [Opcode(ClientMessage.EnumCharacters, "18125")]
        //[Opcode(ClientMessage.EnumCharacters2, "18125")]
        public static void HandleEnumCharactersResult(ref PacketReader packet, WorldClass session)
        {

            // Set existing character from last world session to null
            session.Character = null;
            if (!File.Exists(Helper.DataDirectory() + "characters.json"))
                File.Create(Helper.DataDirectory() + "characters.json").Dispose();
            WorldMgr.CharaterList = Json.CreateObject<List<Character>>(File.ReadAllText(Helper.DataDirectory() + "characters.json"));

            if (WorldMgr.CharaterList == null)
                WorldMgr.CharaterList = new List<Character>();

            WorldMgr.CharaterList.Where(c => c.AccountId == session.Account.Id);

            // Updating Datalength
            for (var i = 0; i < WorldMgr.CharaterList.Count; i++)
                WorldMgr.CharaterList[i].DataLength = (int)ActivePlayerFields.End;

            File.WriteAllText(Helper.DataDirectory() + "characters.json", Json.CreateString(WorldMgr.CharaterList));

            // Re-read it
            WorldMgr.CharaterList = Json.CreateObject<List<Character>>(File.ReadAllText(Helper.DataDirectory() + "characters.json"));

            if (WorldMgr.CharaterList == null)
                WorldMgr.CharaterList = new List<Character>();

            PacketWriter enumCharacters = new PacketWriter(ServerMessage.EnumCharactersResult);
            BitPack BitPack = new BitPack(enumCharacters);

            BitPack.Write(1);
            BitPack.Write(0);
            BitPack.Write(1);
            BitPack.Write(1);
            BitPack.Write(0);
            BitPack.Write(0);
            BitPack.Write(1);

            BitPack.Flush();

            enumCharacters.WriteInt32(WorldMgr.CharaterList.Count);
            enumCharacters.WriteUInt32(0);
            enumCharacters.WriteInt32(WorldMgr.ChrRaces.Count);

            var appearanceUnlocks = new uint[]
            {
                15579
            };
            
            enumCharacters.WriteInt32(appearanceUnlocks.Length);

            for (var i = 0; i < appearanceUnlocks.Length; i++)
            {
                enumCharacters.WriteUInt32(appearanceUnlocks[i]);
                enumCharacters.WriteUInt32(1);
            }

            for (int i = 0; i < WorldMgr.CharaterList.Count; i++)
            {
                var loginCinematic = WorldMgr.CharaterList[i].LoginCinematic;
                var name = WorldMgr.CharaterList[i].Name;

                enumCharacters.WriteSmartGuid(WorldMgr.CharaterList[i].Guid);
                enumCharacters.WriteUInt64(0);
                enumCharacters.WriteUInt8(0);
                enumCharacters.WriteUInt8(WorldMgr.CharaterList[i].Race);
                enumCharacters.WriteUInt8(WorldMgr.CharaterList[i].Class);
                enumCharacters.WriteUInt8(WorldMgr.CharaterList[i].Gender);

                // ChrCustomizationChoiceCount
                enumCharacters.WriteInt32(WorldMgr.CharaterList[i].CharacterCustomizationChoices?.Count ?? 0);
                enumCharacters.WriteUInt8(WorldMgr.CharaterList[i].Level);

                enumCharacters.WriteUInt32(WorldMgr.CharaterList[i].Zone);
                enumCharacters.WriteUInt32(WorldMgr.CharaterList[i].Map);

                enumCharacters.WriteFloat(WorldMgr.CharaterList[i].Position.X);
                enumCharacters.WriteFloat(WorldMgr.CharaterList[i].Position.Y);
                enumCharacters.WriteFloat(WorldMgr.CharaterList[i].Position.Z);

                enumCharacters.WriteSmartGuid(WorldMgr.CharaterList[i].GuildGuid);

                enumCharacters.WriteUInt32(WorldMgr.CharaterList[i].CharacterFlags);
                enumCharacters.WriteUInt32(WorldMgr.CharaterList[i].CustomizeFlags);
                enumCharacters.WriteUInt32(0);
                enumCharacters.WriteUInt32(WorldMgr.CharaterList[i].PetDisplayInfo);
                enumCharacters.WriteUInt32(WorldMgr.CharaterList[i].PetLevel);
                enumCharacters.WriteUInt32(WorldMgr.CharaterList[i].PetFamily);

                // stuff
                enumCharacters.WriteUInt32(0);
                enumCharacters.WriteUInt32(0);

                var written = false;
                for (int j = 0; j < 23; j++)
                {
                    written = false;
                    foreach (var kp in WorldMgr.CharaterList[i].Equipment)
                    {
                        if (kp.Key == j)
                        {
                            if (j == 9)
                            {
                                enumCharacters.WriteUInt32(184373);
                                enumCharacters.WriteUInt32(0);
                                enumCharacters.WriteUInt32(0);
                                enumCharacters.WriteUInt8(0);
                                enumCharacters.WriteUInt8(0);
                            }
                            else
                            {
                                enumCharacters.WriteInt32(kp.Value.DisplayInfoId);//136
                                enumCharacters.WriteUInt32(0);//140
                                enumCharacters.WriteUInt32(0);

                                var inv = kp.Key;

                                if (kp.Key == 15)
                                    inv += 1;

                                enumCharacters.WriteUInt8(inv);//144
                                enumCharacters.WriteUInt8(0);
                            }
                            written = true;
                            break;
                        }
                    }


                    if (!written)
                    {
                        enumCharacters.WriteUInt32(0);
                        enumCharacters.WriteUInt32(0);
                        enumCharacters.WriteUInt32(0);
                        enumCharacters.WriteUInt8(0);
                        enumCharacters.WriteUInt8(0);
                    }
                }

                enumCharacters.WriteUInt64(0);
                enumCharacters.WriteUInt16(0);
                enumCharacters.WriteUInt32(0);
                enumCharacters.WriteUInt32(0);
                enumCharacters.WriteUInt32(WorldMgr.CharaterList[i].Runes);
                enumCharacters.WriteUInt32(0);
                enumCharacters.WriteUInt32(0);
                enumCharacters.WriteUInt32(0);

                WorldMgr.CharaterList[i].CharacterCustomizationChoices?.ForEach((choice) =>
                {
                    enumCharacters.WriteUInt32(choice.OptionId);
                    enumCharacters.WriteUInt32(choice.Value);
                });

                BitPack.Write(Encoding.UTF8.GetBytes(name).Length, 6);
                BitPack.Write(0);
                BitPack.Write(0);
                BitPack.Write(0, 5);
                BitPack.Flush();
                enumCharacters.WriteString(name);
            }

            foreach (var cr in WorldMgr.ChrRaces)
            {
                enumCharacters.WriteUInt32(cr.Id);

                BitPack.Write(1);
                BitPack.Write(1);
                BitPack.Write(1);

                BitPack.Flush();
            }

            WorldMgr.createTime = (uint)((DateTimeOffset)File.GetCreationTime(Helper.DataDirectory() + "characters.json")).ToUnixTimeSeconds();

            session.Send(ref enumCharacters);
        }

        [Opcode(ClientMessage.CreateCharacter, "18156")]
        public static void HandleCreateCharacter(ref PacketReader packet, WorldClass session)
        {
            BitUnpack BitUnpack = new BitUnpack(packet);

            var nameLength = BitUnpack.GetBits<uint>(6);
            var hasTemplate = BitUnpack.GetBit();

            var unk3 = BitUnpack.GetBit();
            var unk2 = BitUnpack.GetBit();

            var race = packet.ReadByte();
            var pClass = packet.ReadByte();
            var gender = packet.ReadByte();
            var customizationChoices = new List<CharacterCustomizationChoice>(packet.ReadInt32());
            var name = Character.NormalizeName(packet.ReadString(nameLength));
            var templateId = 0;

            if (hasTemplate)
                templateId = packet.ReadInt32();

            for (var i = 0; i < customizationChoices.Capacity; i++)
            {
                customizationChoices.Add(new CharacterCustomizationChoice
                {
                    OptionId = packet.ReadUInt32(),
                    Value = packet.ReadUInt32()
                });
            }

            var createChar = new PacketWriter(ServerMessage.CreateChar);

            if (WorldMgr.CharaterList.Count >= 100)
            {
                // Char limit.
                createChar.WriteUInt8(0x35);
                session.Send(ref createChar);
                return;
            }

            var result = WorldMgr.CharaterList.Any(c => c.Name == name);

            if (result)
            {
                // Name already in use
                createChar.WriteUInt8(0x32);
                session.Send(ref createChar);
                return;
            }

            // Bastion!
            var map = 2374;
            var zone = 12444;
            var posX = -3833.4006f;
            var posY = 1049.6012f;
            var posZ = 15.41108f;
            var posO = 4.6956673f;
            var level = 60;

            var isAlliance = race == 1 || race == 3 || race == 4 || race == 7 || race == 11 || race == 29 ||
                             race == 30 || race == 22 || race == 32 || race == 34 || race == 37;

            var isAlliedRace = race == 27 || race == 28 || race == 29 || race == 30 || race == 31 ||
                               race == 32 || race == 33 || race == 34 || race == 35 || race == 36 || race == 37;

            var isHeroClass = pClass == 6 || pClass == 12;

            if (!hasTemplate)
            {
                zone = 10639;
                map = 2175;
                posX = -462.4f;
                posY = -2619.8f;
                posZ = 0.4f;
                posO = 0.7923794f;
                level = 1;

                if (isAlliedRace)
                    level = 8;

                if (isHeroClass)
                    level = 10;
            }

            if (!isAlliance && !hasTemplate)
            {
                switch (race)
                {
                    case 1:
                        zone = 6170;
                        map = 0;
                        posX = -8914.57f;
                        posY = -133.909f;
                        posZ = 81.5378f;
                        posO = 5.10444f;
                        break;
                    case 2:
                        zone = 6170;
                        map = 1;
                        posX = -618.518f;
                        posY = -4251.67f;
                        posZ = 39.718f;
                        posO = 0f;
                        break;
                    case 3:
                        zone = 6176;
                        map = 0;
                        posX = -6230.42f;
                        posY = 330.232f;
                        posZ = 90.105f;
                        posO = 0.501087f;
                        break;
                    case 4:
                        zone = 6450;
                        map = 1;
                        posX = 10311.3f;
                        posY = 831.463f;
                        posZ = 1326.57f;
                        posO = 5.48033f;
                        break;
                    case 5:
                        zone = 6454;
                        map = 0;
                        posX = 1699.85f;
                        posY = 1706.56f;
                        posZ = 135.928f;
                        posO = 4.88839f;
                        break;
                    case 6:
                        zone = 6452;
                        map = 1;
                        posX = -2915.55f;
                        posY = -257.347f;
                        posZ = 59.2693f;
                        posO = 0.302378f;
                        break;
                    case 7:
                        zone = 1;
                        map = 0;
                        posX = -4983.42f;
                        posY = 877.7f;
                        posZ = 274.31f;
                        posO = 3.06393f;
                        break;
                    case 8:
                        zone = 6453;
                        map = 1;
                        posX = -1171.45f;
                        posY = -5263.65f;
                        posZ = 0.847728f;
                        posO = 5.78945f;
                        break;
                    case 9:
                        zone = 4737;
                        map = 648;
                        posX = -8423.81f;
                        posY = 1361.3f;
                        posZ = 104.671f;
                        posO = 1.55428f;
                        break;
                    case 10:
                        zone = 6455;
                        map = 530;
                        posX = 10349.6f;
                        posY = -6357.29f;
                        posZ = 33.4026f;
                        posO = 5.31605f;
                        break;
                    case 11:
                        zone = 6456;
                        map = 530;
                        posX = 3961.64f;
                        posY = -13931.2f;
                        posZ = 100.615f;
                        posO = 2.08364f;
                        break;
                    case 22:
                        zone = 4755;
                        map = 654;
                        posX = -1451.53f;
                        posY = 1403.35f;
                        posZ = 35.5561f;
                        posO = 0.333847f;
                        break;
                    case 24:
                        zone = 5736;
                        map = 860;
                        posX = 1466.09f;
                        posY = 3465.98f;
                        posZ = 181.86f;
                        posO = 2.87962f;
                        break;
                    default:
                        break;
                }

                if (race == 27)
                {
                    map = 1220;
                    posX = 286.9023f;
                    posY = 3353.4353f;
                    posZ = 145.4761f;
                    posO = 2.3605f;
                }
                else if (race == 28)
                {
                    map = 1220;
                    posX = 4075.55347f;
                    posY = 4384.899f;
                    posZ = 670.62525f;
                    posO = 0.2620099f;
                }
                else if (race == 29)
                {
                    map = 1622;
                    posX = 1382.72f;
                    posY = 2872.21f;
                    posZ = 57.15f;
                    posO = 0;
                }
                else if (race == 30)
                {
                    map = 1860;
                    posX = 458.87048f;
                    posY = 1449.9027f;
                    posZ = 757.5727f;
                    posO = 0.39674f;
                }
                else if (race == 31)
                {
                    map = 1642;
                    posX = -2237.45f;
                    posY = 806.3954f;
                    posZ = 13.75639f;
                    posO = 0.05105136f;
                }

                if (pClass == 6)
                {
                    zone = 4298;
                    map = 609;
                    posX = 2355.84f;
                    posY = -5664.77f;
                    posZ = 426.028f;
                    posO = 3.65997f;
                }

                if (pClass == 12)
                {
                    zone = 0;
                    map = 1481;
                    posX = 1180.54f;
                    posY = 3284.80f;
                    posZ = 70.3489f;
                    posO = 4.76132f;
                }
            }

            // Allow declined names for now
            var characterFlags = CharacterFlag.Decline;
            var guid = WorldMgr.CharaterList.Count > 0 ? WorldMgr.CharaterList[WorldMgr.CharaterList.Count - 1].Guid + 10 : 1;
            var pcharacter = new Character(guid)
            {
                Guid = guid,
                Name = name,
                AccountId = 1,
                Race = race,
                Class = pClass,
                Gender = gender,
                CharacterCustomizationChoices = customizationChoices,
                Zone = (uint)zone,
                Map = (uint)map,
                Position = new Vector4
                {
                    X = posX,
                    Y = posY,
                    Z = posZ,
                    O = posO
                },
                CharacterFlags = (uint)characterFlags,
                Level = (byte)level,
                LoginCinematic = true
            };

            pcharacter.AddSpellSkills();

            if (!hasTemplate)
            {
                var startItems = Equipments.CharStartOutfits.SingleOrDefault(cso => cso.RaceId == pcharacter.Race && cso.ClassId == pcharacter.Class &&
                                                                               cso.SexId == pcharacter.Gender)?.ItemId;

                if (startItems != null)
                {
                    for (var i = 0; i < startItems.Length; i++)
                    {
                        if (startItems[i] != 0)
                        {
                            var item = new Item();
                            if (Equipments.AvailableItems.ContainsKey((int)startItems[i]))
                            {
                                var sItem = Equipments.AvailableItems[(int)startItems[i]];
                                item.Id = sItem.Id;
                                item.DisplayInfoIds = sItem.DisplayInfoIds;
                                if (sItem.DisplayInfoIds.Count > 0)
                                    item.DisplayInfoId = sItem.DisplayInfoIds["normal"].Item1;
                                item.InventoryType = sItem.InventoryType;
                                item.Guid = (ulong)(pcharacter.Equipment.Count + pcharacter.Bags[255].Count) + 1;

                                if (item.InventoryType == 18)
                                {
                                    continue;
                                }

                                if (!pcharacter.Equipment.ContainsKey(ItemHandler.GetEquipSlot(item.InventoryType, pcharacter)))
                                    pcharacter.Equipment.Add(ItemHandler.GetEquipSlot(item.InventoryType, pcharacter), item);
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }
            }

#if !PUBLIC
            // Add four 32 slot bags!
            for (var i = 0; i < 4; i++)
            {
                var deepSeabag = Equipments.AvailableItems[154696];

                var item = new Item
                {
                    Id = deepSeabag.Id,
                    InventoryType = deepSeabag.InventoryType,
                    Guid = (ulong)(pcharacter.Equipment.Count + pcharacter.Bags[255].Count) + 1,
                    NumSlots = 32,
                    BagSlot = (byte)(19 + i)
                };

                pcharacter.Equipment.Add((byte)(19 + i), item);
            }
#endif

            WorldMgr.CharaterList.Add(pcharacter);

            File.WriteAllText(Helper.DataDirectory() + "characters.json", Json.CreateString(WorldMgr.CharaterList));

            // Success
            createChar.WriteUInt8(24);
            session.Send(ref createChar);
        }

        [Opcode(ClientMessage.CharDelete, "18156")]
        public static void HandleCharDelete(ref PacketReader packet, WorldClass session)
        {
            var guid = packet.ReadSmartGuid();
            var index = 0;

            for (int i = 0; i < WorldMgr.CharaterList.Count; i++)
            {
                if (WorldMgr.CharaterList[i].Guid == guid)
                {
                    index = i;
                    break;
                }
            }

            WorldMgr.CharaterList.RemoveAt(index);
            File.WriteAllText(Helper.DataDirectory() + "characters.json", Json.CreateString(WorldMgr.CharaterList));

            PacketWriter deleteChar = new PacketWriter(ServerMessage.DeleteChar);

            deleteChar.WriteUInt8(53);

            session.Send(ref deleteChar);
        }

        [Opcode(ClientMessage.GenerateRandomCharacterName, "18156")]
        public static void HandleGenerateRandomCharacterName(ref PacketReader packet, WorldClass session)
        {
            var race = packet.ReadByte();
            var gender = packet.ReadByte();

            List<string> names = Equipments.Namegens.Where(n => n.Race == race && n.Sex == gender).Select(n => n.Name).ToList();

            if (names.Count == 0)
                names = Equipments.Namegens.Select(n => n.Name).ToList();
            Random rand = new Random(Environment.TickCount);

            string NewName;

            do
            {
                NewName = names[rand.Next(names.Count)];

            }
            while (WorldMgr.CharaterList.Any(c => c.Name == NewName));

            PacketWriter generateRandomCharacterNameResult = new PacketWriter(ServerMessage.GenerateRandomCharacterNameResult);
            BitPack BitPack = new BitPack(generateRandomCharacterNameResult);

            BitPack.Write(1);
            BitPack.Write(NewName.Length, 6);

            BitPack.Flush();

            generateRandomCharacterNameResult.WriteString(NewName);
            session.Send(ref generateRandomCharacterNameResult);
        }
        static ulong lastSession = 0;

        [Opcode(ClientMessage.PlayerLogin, "18125")]
        public static void HandlePlayerLogin(ref PacketReader packet, WorldClass session)
        {
            {
                try
                {
                    var guid = packet.ReadSmartGuid();
                    packet.Skip(1); // clientSettings

                    //Log.Message(LogType.Debug, "Character with Guid: {0}, AccountId: {1} tried to enter the world.", guid, session.Account.Id);

                    for (int i = 0; i < WorldMgr.CharaterList.Count; i++)
                    {
                        if (WorldMgr.CharaterList[i].Guid == guid)
                        {
                            session.Character = WorldMgr.CharaterList[i];
                            break;
                        }
                    }
                    if (!WorldMgr.AddSession(guid, ref session))
                    {
                        Log.Message(LogType.Error, "A Character with Guid: {0} is already logged in", guid);
                        return;
                    }

                    if (session.Character == null)
                        session.Character = WorldMgr.CharaterList[0];

                    var sess2 = WorldMgr.GetSession2(lastSession);
                    if (sess2 == null)
                        sess2 = WorldMgr.GetSession2(guid);

                    if (sess2 != null)
                        sess2.Character = session.Character;

                    if (session.Character.InRangeObjects == null)
                        session.Character.InRangeObjects = new Dictionary<ulong, WorldObject>();

                    if (session.Character.DataLength == 0)
                        session.Character.DataLength = (int)ActivePlayerFields.End;

                    session.Character.MaskSize = (sess2.Character.DataLength) / 32;
                    session.Character.Mask = new BitArray(sess2.Character.DataLength, false);

                    WorldMgr.DeleteSession2(0);
                    WorldMgr.AddSession2(0, ref sess2);

                    WorldMgr.Sessions[guid] = session;
                    WorldMgr.waitAdd = false;

                    TutorialHandler.HandleTutorialFlags(ref session);

                    TimeHandler.HandleSetTimezoneInformation(ref session);
                    TimeHandler.HandleLoginSetTimeSpeed(ref session);

                    WorldMgr.WriteAccountDataTimes(AccountDataMasks.CharacterCacheMask, ref session);

                    HandleUpdateTalentData(ref session);

                    SpellHandler.HandleSendKnownSpells(ref session);
                    MiscHandler.HandleUpdateActionButtons(ref session);

                    if (session.Character.LoginCinematic)
                    {
                        if (sess2.Character.Race == 27)
                            PlayerCommands.Scene(new[] { "!scene", "2007" }, session);
                        else if (sess2.Character.Race == 28)
                            PlayerCommands.Scene(new[] { "!scene", "1984" }, session);
                        else if (sess2.Character.Race == 29)
                            PlayerCommands.Scene(new[] { "!scene", "2006" }, session);
                        else if (sess2.Character.Race == 30)
                            PlayerCommands.Scene(new[] { "!scene", "2005" }, session);

                        session.Character.LoginCinematic = false;

                        for (int i = 0; i < WorldMgr.CharaterList.Count; i++)
                        {
                            if (WorldMgr.CharaterList[i].Guid == session.Character.Guid)
                            {
                                WorldMgr.CharaterList[i].Position = session.Character.Position;
                                break;
                            }
                        }

                        while (true)
                        {
                            try
                            {
                                File.WriteAllText(Helper.DataDirectory() + "characters.json", Json.CreateString(WorldMgr.CharaterList));

                                Console.WriteLine("Characters saved.");
                                break;
                            }
                            catch (Exception ex)
                            {
                                Console.Write(ex);
                            }

                            Thread.Sleep(1);
                        }
                    }

                    ObjectHandler.HandleUpdateObjectCreate(ref session);
                    MiscHandler.HandleMessageOfTheDay(ref session);

                    SendAchievementData2(ref session);
                    SendAchievementData3(ref session);

                    if (session.Character.Class == 12)
                    {
                        var pkt = new PacketWriter(ServerMessage.EnableDJump);
                        pkt.WriteSmartGuid(session.Character.Guid);
                        pkt.WriteInt32(596542156);

                        session.Send(ref pkt);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine(ex.InnerException.Message);
                }
            }
        }

        public static bool chatRunning = true;

        public static void SendAchievementData2(ref WorldClass session, bool lvl110 = false)
        {
            var pkt = new PacketWriter(ServerMessage.AchievementEarned);
            BitPack BitPack = new BitPack(pkt);

            pkt.WriteSmartGuid(session.Character.Guid);
            pkt.WriteSmartGuid(session.Character.Guid);

            if (session.Character.Race == 27)
                pkt.WriteInt32(12244);
            else if (session.Character.Race == 28)
                pkt.WriteInt32(12245);
            else if (session.Character.Race == 29)
                pkt.WriteInt32(12242);
            else if (session.Character.Race == 30)
                pkt.WriteInt32(12243);
            else
                pkt.WriteInt32(3579);

            pkt.WritePackedTime();

            pkt.WriteInt32(1);
            pkt.WriteInt32(1);

            BitPack.Write(true);
            BitPack.Flush();

            session.Send(ref pkt);

            // 110
            if (session.Character.Level >= 110 && session.Character.Race >= 27)
            {
                pkt = new PacketWriter(ServerMessage.AchievementEarned);
                BitPack = new BitPack(pkt);

                pkt.WriteSmartGuid(session.Character.Guid);
                pkt.WriteSmartGuid(session.Character.Guid);

                if (session.Character.Race == 27)
                    pkt.WriteInt32(12413);
                else if (session.Character.Race == 28)
                    pkt.WriteInt32(12415);
                else if (session.Character.Race == 29)
                    pkt.WriteInt32(12291);
                else if (session.Character.Race == 30)
                    pkt.WriteInt32(12414);

                pkt.WritePackedTime();

                pkt.WriteInt32(1);
                pkt.WriteInt32(1);

                BitPack.Write(true);
                BitPack.Flush();

                session.Send(ref pkt);
            }
        }

        public static void SendAchievementData3(ref WorldClass session, bool lvl110 = false)
        {
            var pkt = new PacketWriter(ServerMessage.AllAchievementData);

            if (session.Character.Level >= 110 && session.Character.Race >= 27)
            {
                pkt.WriteInt32(2);
                pkt.WriteInt32(0);

                // earned
                if (session.Character.Race == 27)
                    pkt.WriteInt32(12244);
                else if (session.Character.Race == 28)
                    pkt.WriteInt32(12245);
                else if (session.Character.Race == 29)
                    pkt.WriteInt32(12242);
                else if (session.Character.Race == 30)
                    pkt.WriteInt32(12243);

                pkt.WritePackedTime();
                pkt.WriteSmartGuid(session.Character.Guid);

                pkt.WriteInt32(1);
                pkt.WriteInt32(1);

                // 110
                // earned
                if (session.Character.Race == 27)
                    pkt.WriteInt32(12413);
                else if (session.Character.Race == 28)
                    pkt.WriteInt32(12415);
                else if (session.Character.Race == 29)
                    pkt.WriteInt32(12291);
                else if (session.Character.Race == 30)
                    pkt.WriteInt32(12414);

                pkt.WritePackedTime();
                pkt.WriteSmartGuid(session.Character.Guid);

                pkt.WriteInt32(1);
                pkt.WriteInt32(1);
            }
            else
            {
                pkt.WriteInt32(1);
                pkt.WriteInt32(0);

                // earned
                if (session.Character.Race == 27)
                    pkt.WriteInt32(12244);
                else if (session.Character.Race == 28)
                    pkt.WriteInt32(12245);
                else if (session.Character.Race == 29)
                    pkt.WriteInt32(12242);
                else if (session.Character.Race == 30)
                    pkt.WriteInt32(12243);
                else
                    pkt.WriteInt32(3579);

                pkt.WritePackedTime();
                pkt.WriteSmartGuid(session.Character.Guid);

                pkt.WriteInt32(1);
                pkt.WriteInt32(1);
            }

            session.Send(ref pkt);
        }

        public static void HandleUpdateTalentData(ref WorldClass session)
        {
            if (session.Character.PrimarySpec < 62 || session.Character.PrimarySpec > 581)
                session.Character.PrimarySpec = WorldMgr.DefaultChrSpec[session.Character.Class];

            var pChar = session.Character;

            PacketWriter updateTalentData = new PacketWriter(ServerMessage.UpdateTalentData);
            BitPack BitPack = new BitPack(updateTalentData);

            updateTalentData.WriteUInt8(0);
            updateTalentData.WriteUInt32(session.Character.PrimarySpec);// pChar.SpecGroupCount);

            var counter = 1;// WorldMgr.ChrSpecs[pChar.Class].Count;

            updateTalentData.WriteInt32(counter); // counter

            for (int i = 0; i < counter; i++)
            {
                updateTalentData.WriteUInt32(session.Character.PrimarySpec);
                updateTalentData.WriteUInt32(0);
                updateTalentData.WriteUInt32(0);
            }

            session.Send(ref updateTalentData);
        }

        // TODO: Fix!
        public static void HandleUpdateToyData(ref WorldClass2 session)
        {
            var pChar = session.Character;

            PacketWriter updateToyData = new PacketWriter(ServerMessage.UpdateToyData);
            BitPack BitPack = new BitPack(updateToyData);

            updateToyData.WriteUInt8(0x80);
            updateToyData.WriteInt32(ItemManager.GetInstance().Toys.Count);
            updateToyData.WriteInt32(ItemManager.GetInstance().Toys.Count);

            foreach (var t in ItemManager.GetInstance().Toys)
                updateToyData.WriteInt32(t.ItemId);


            foreach (var t in ItemManager.GetInstance().Toys)
                BitPack.Write(false);

            BitPack.Flush();

            session.Send(ref updateToyData);
        }

        public static bool FileLocked(string FileName)
        {
            FileStream fs = null;

            try
            {
                // NOTE: This doesn't handle situations where file is opened for writing by another process but put into write shared mode, it will not throw an exception and won't show it as write locked
                fs = File.Open(FileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None); // If we can't open file for reading and writing then it's locked by another process for writing
            }
            catch (UnauthorizedAccessException) // https://msdn.microsoft.com/en-us/library/y973b725(v=vs.110).aspx
            {
                // This is because the file is Read-Only and we tried to open in ReadWrite mode, now try to open in Read only mode
                try
                {
                    fs = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.None);
                }
                catch (Exception)
                {
                    return true; // This file has been locked, we can't even open it to read
                }
            }
            catch (Exception)
            {
                return true; // This file has been locked
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            return false;
        }
    }
}
