using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Arctium.WoW.Sandbox.Server.WorldServer.Game.Entities;
using Arctium.WoW.Sandbox.Server.WorldServer.Managers;

using AuthServer.Game.Chat;
using AuthServer.Game.Entities;
using AuthServer.Game.PacketHandler;
using AuthServer.Game.Packets.PacketHandler;
using AuthServer.Game.WorldEntities;
using AuthServer.Network;
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

namespace AuthServer.WorldServer.Game.Chat.Commands
{
    public class PlayerCommands
    {
#if !PUBLIC
        [ChatCommand("pepe", "Usage: !fly #state (Turns the fly mode 'on' or 'off')")]
#endif
        public static void Pepe(string[] args, WorldClass session)
        {
            var displayId = Command.Read<uint>(args, 1);
            var pChar = session.Character;

            if (pChar != null)
            {
                if (session.TargetGuid != 0)
                {
                    WorldObject spawn;

                    if (pChar.InRangeObjects.TryGetValue(session.TargetGuid, out spawn))
                    {
                        spawn.SetUpdateField((int)UnitFields.ChannelData + 1, Manager.WorldMgr.SpellVisualIds[(uint)displayId]);
                        spawn.SetUpdateField((int)UnitFields.StateSpellVisualID, Manager.WorldMgr.SpellVisualIds[(uint)displayId]);
                    }
                }
                else
                {
                    PacketWriter updateObject = new PacketWriter(ServerMessage.ObjectUpdate);
                    BitPack BitPack = new BitPack(updateObject);

                    updateObject.WriteUInt32(1);
                    updateObject.WriteUInt16((ushort)session.Character.Map);
                    updateObject.WriteUInt8(0);
                    updateObject.WriteInt32(0);

                    updateObject.WriteUInt8((byte)UpdateType.Values);
                    updateObject.WriteSmartGuid(session.Character.SGuid);

                    var writer = new PacketWriter();
                    var bp = new BitPack(writer);
                    var bits = new BitArray((((int)UnitFields.End + 32 - 1) / 32) * 32, false);

                    for (var i = 0; i < bits.Length; i += 32)
                        bits.Set(i, true);

                    bits.Set((int)UnitFields.EnableChannelData, true);
                    bits.Set((int)UnitFields.ChannelData + 1, true);
                    bits.Set((int)UnitFields.StateSpellVisualID, true);

                    // Always send all uint32 flags
                    bp.Write(63, 7);

                    byte[] ret = new byte[(bits.Length) / 8 + 1];
                    bits.CopyTo(ret, 0);

                    var masks = new uint[ret.Length / 4];

                    for (var i = 0; i < masks.Length; i++)
                        masks[i] = BitConverter.ToUInt32(ret, i * 4);

                    for (var i = 0; i < masks.Length; i++)
                    {
                        bp.Write(masks[i], 32);
                    }

                    bp.Flush();

                    updateObject.Write((writer.BaseStream as MemoryStream).ToArray().Length + 4 + 4);
                    updateObject.WriteUInt32(0x20);

                    updateObject.Write((writer.BaseStream as MemoryStream).ToArray());

                    updateObject.Write(Manager.WorldMgr.SpellVisualIds[(uint)242014]);
                    updateObject.Write(Manager.WorldMgr.SpellVisualIds[(uint)242014]);

                    var size = (uint)updateObject.BaseStream.Length - 29;
                    updateObject.WriteUInt32Pos(size, 25);

                    session.Send(ref updateObject);
                }
            }
        }

        [ChatCommand("addbag", "Usage: !addbag #id (space seperated)")]
        public static void AddBag(string[] args, WorldClass session)
        {
            // hexweave bag: 114821
            args = new[] { "!additem", "154696" };

            AddItem(args, session);
        }

        [ChatCommand("loadspawns", "")]
        public static void LoadInternalSpawns(string[] args, WorldClass session)
        {
            SpawnManager.GetInstance().LoadCreatureSpawns(true);
        }

        [ChatCommand("unloadspawns", "")]
        public static void UnloadInternalSpawns(string[] args, WorldClass session)
        {
            SpawnManager.GetInstance().RemoveInternalSpawns(session);
        }

        public static Dictionary<byte, bool> bagSlotsList = new Dictionary<byte, bool>()
        {
            { 23, false },
            { 24, false },
            { 25, false },
            { 26, false },
            { 27, false },
            { 28, false },
            { 29, false },
            { 30, false },
            { 31, false },
            { 32, false },
            { 33, false },
            { 34, false },
            { 35, false },
            { 36, false },
            { 37, false },
            { 38, false },

            // Authenticator
            { 39, false },
            { 40, false },
            { 41, false },
            { 42, false },

            // Let's use Vulpera bag sizes here.
            { 43, false },
            { 44, false },
            { 45, false },
            { 46, false },
            { 47, false },
            { 48, false },
            { 49, false },
            { 50, false },
        };

        static Dictionary<int, string> itemLinkDifficulty = new Dictionary<int, string>()
        {
            { -1, "normal" },
            { 1, "normal" }, // Should work for most items.
            { 3, "elite" },
            { 4, "lfr" },
            { 5, "heroic" },
            { 6, "mythic" }
        };

        [ChatCommand("additem", "Usage: !additem #id (space seperated)")]
        public static void AddItem(string[] args, WorldClass session)
        {
            var isItemLink = false;

            for (int i = 1; i < args.Length; i++)
            {
                // Skip fail entries caused by command splitting.
                if (isItemLink && !args[i].Contains("|Hitem"))
                    continue;

                var data = Command.Read<string>(args, i);
                var id = -1;
                var version = "normal";

                // Item Link!!!
                if (data.Contains("|Hitem"))
                {
                    isItemLink = true;

                    var linkSplit = args[i].Split(':');

                    if (linkSplit.Length < 17)
                    {
                        Log.Message(LogType.Error, "Error while parsing the additem command!");
                        return;
                    }

                    id = int.Parse(linkSplit[1]);
                    version = itemLinkDifficulty[int.Parse(linkSplit[13] == "" ? "-1" : linkSplit[13])];
                }
                else
                {
                    if (isItemLink)
                    {
                        Log.Message(LogType.Error, "Please DO NOT mix item ids and item links.");
                        return;
                    }

                    var split = args[i].Split(',');

                    id = int.Parse(split[0]);
                    version = "normal";

                    if (split.Length == 2)
                        version = split[1];

                    if (version == "elite")
                        version = "mythic";
                }

                var pChar = session.Character;

                if (pChar != null)
                {
                    Item item;
                    if (Manager.Equipments.AvailableItems.TryGetValue(id, out item))
                    {
#if PUBLIC
                        // No bags for public builds!
                        if (item.InventoryType == 18)
                            return;
#endif

                        var newItem = item.Clone();

                        Tuple<int, int> itemInfo;

                        if (item.DisplayInfoIds.Count > 0)
                        {
                            if (!item.DisplayInfoIds.TryGetValue(version, out itemInfo))
                                itemInfo = item.DisplayInfoIds.First().Value;

                            newItem.DisplayInfoId = itemInfo.Item1;
                            newItem.ModId = itemInfo.Item2;
                        }
                        else
                        {
                            newItem.DisplayInfoId = 0;
                            newItem.ModId = 0;
                        }

                        /// OLD
                        var orderedEquip = session.Character.Equipment.OrderBy(kp => kp.Value.Guid);
                        var lastEquipGuid = orderedEquip.Any() ? orderedEquip.Last().Value.Guid : (ulong)new Random(Environment.TickCount).Next(10000);

                        var orderedbag = session.Character.Bags[255].OrderBy(kp => kp.Value.Guid);
                        var lastBagGuid = orderedbag.Any() ? orderedbag.Last().Value.Guid : (ulong)new Random(Environment.TickCount).Next(10000);

                        var guid = lastEquipGuid + lastBagGuid;

                        //guid |= 0x4000000000000000;
                        var itemGuid = new SmartGuid(guid, newItem.Id, GuidType.Item);

                        newItem.Guid = itemGuid.Guid;

                        var freeSlot = bagSlotsList.OrderBy(bs => bs.Value).First();

                        if (!session.Character.Bags[255].ContainsKey((byte)(freeSlot.Key)))
                        {
                            bagSlotsList[freeSlot.Key] = true;

                            session.Character.Bags[255].Add((byte)(freeSlot.Key), newItem);

                            ObjectHandler.HandleUpdateObjectCreateItem(itemGuid, newItem, ref session, session.Character);
                            WorldObject character = session.Character;
                            PacketWriter updateObject = new PacketWriter(ServerMessage.ObjectUpdate);
                            BitPack BitPack = new BitPack(updateObject);

                            updateObject.WriteUInt32(1);
                            updateObject.WriteUInt16((ushort)character.Map);
                            updateObject.WriteUInt8(0);
                            updateObject.WriteInt32(0);

                            updateObject.WriteUInt8((byte)UpdateType.Values);
                            updateObject.WriteSmartGuid(session.Character.Guid);

                            var writer = new PacketWriter();
                            var bp = new BitPack(writer);
                            var bits = new BitArray((((int)ActivePlayerFields.End + 32 - 1) / 32) * 32, false);

                            // for (var j = 0; j < bits.Length; j += 32)
                            //     bits.Set(j, false);

                            bits.Set((int)ActivePlayerFields.EnableInvSlots, true);
                            bits.Set((int)ActivePlayerFields.InvSlots + ((freeSlot.Key)), true);
                            bits.Set((int)ActivePlayerFields.InvSlots + ((freeSlot.Key + 1)), true);

                            // Always send all uint32 flags
                            writer.WriteUInt32(524287);
                            bp.Write(0, 17);

                            byte[] ret = new byte[(bits.Length) / 8 + 1];
                            bits.CopyTo(ret, 0);

                            var masks = new uint[ret.Length / 4];

                            for (var j = 0; j < masks.Length; j++)
                                masks[j] = BitConverter.ToUInt32(ret, j * 4);

                            for (var j = 0; j < masks.Length; j++)
                            {
                                //if (masks[j] != 0)
                                bp.Write(masks[j], 32);
                            }

                            bp.Flush();

                            var fieldWriter = new PacketWriter();

                            // GUID
                            fieldWriter.WriteSmartGuid(itemGuid);
                            fieldWriter.WriteSmartGuid(new SmartGuid());

                            var fieldData = (fieldWriter.BaseStream as MemoryStream).ToArray();

                            updateObject.Write((writer.BaseStream as MemoryStream).ToArray().Length + 4 + fieldData.Length);
                            updateObject.WriteInt32(0x80);

                            updateObject.Write((writer.BaseStream as MemoryStream).ToArray());
                            updateObject.Write(fieldData);


                            var size = (uint)updateObject.BaseStream.Length - 29;
                            updateObject.WriteUInt32Pos(size, 25);

                            session.Send(ref updateObject);

                        }
                    }
                }
            }

        }


        [ChatCommand("bshop", "")]
        public static void BarberShop(string[] args, WorldClass session)
        {
            var pChar = session.Character;
            var pkt = new PacketWriter((ServerMessage)0x26BB);

            pkt.WriteUInt32(0);
            session.Send(ref pkt);
        }

        [ChatCommand("rplayer", "")]
        public static void ReloadPlayer(string[] args, WorldClass session)
        {
            var pChar = session.Character;

            CharacterHandler.SendAchievementData2(ref session);
            CharacterHandler.SendAchievementData3(ref session);

            var sess2 = Manager.WorldMgr.GetSession2(session.Character.Guid);
            MoveHandler.HandleTransferPending(ref sess2, pChar.Map);
            MoveHandler.HandleNewWorld(ref sess2, session.Character.Position, pChar.Map);

            Manager.ObjectMgr.SetPosition(ref pChar, session.Character.Position);
            Manager.ObjectMgr.SetMap(ref pChar, pChar.Map);

            ObjectHandler.HandleUpdateObjectCreate(ref session, false, false);

            if (session.IsFlying)
                MoveHandler.HandleMoveSetCanFly(ref sess2);

            MoveHandler.HandleMoveSetFlightSpeed(ref sess2, session.Flightspeed);
            MoveHandler.HandleMoveSetRunSpeed(ref sess2, session.RunSpeed);
        }


        [ChatCommand("scale", "Usage: !morph #size")]
        public static void Scale(string[] args, WorldClass session)
        {
            var scale = Command.Read<float>(args, 1);
            var pChar = session.Character;

            if (pChar != null)
            {
                if (session.TargetGuid != 0)
                {
                    WorldObject spawn;

                    if (pChar.InRangeObjects.TryGetValue(session.TargetGuid, out spawn))
                    {
                        WorldObject character = session.Character;
                        PacketWriter updateObject = new PacketWriter(ServerMessage.ObjectUpdate);
                        BitPack BitPack = new BitPack(updateObject);

                        updateObject.WriteUInt32(1);
                        updateObject.WriteUInt16((ushort)character.Map);
                        updateObject.WriteUInt8(0);
                        updateObject.WriteInt32(0);

                        updateObject.WriteUInt8((byte)UpdateType.Values);

                        if (spawn.SGuid == null)
                            updateObject.WriteSmartGuid(session.TargetGuid);
                        else
                            updateObject.WriteSmartGuid(spawn.SGuid);

                        var writer = new PacketWriter();
                        var bp = new BitPack(writer);
                        var bits = new BitArray((((int)ObjectFields.End + 32 - 1) / 32) * 32, false);

                        for (var i = 0; i < bits.Length; i += 32)
                            bits.Set(i, true);

                        bits.Set((int)ObjectFields.Scale, true);

                        // Always send all uint32 flags
                        bp.Write(9, 4);
                        bp.Flush();

                        updateObject.Write((writer.BaseStream as MemoryStream).ToArray().Length + 4 + 4);
                        updateObject.WriteUInt32(1);

                        updateObject.Write((writer.BaseStream as MemoryStream).ToArray());

                        updateObject.Write(scale);

                        var size = (uint)updateObject.BaseStream.Length - 29;
                        updateObject.WriteUInt32Pos(size, 25);

                        session.Send(ref updateObject);

                    }
                }
                else
                {
                    ObjectHandler.Update(ObjectFields.Scale, scale, pChar);
                }
            }
        }

        static uint sceneInstanceId = 0;

#if PUBLIC == false
        [ChatCommand("scenepkg", "sceneId")]
#endif
        public static void ScenePkg(string[] args, WorldClass session)
        {
            var sceneId = Command.Read<uint>(args, 1);
            var flags = 0x10;
            if (args.Length == 3)
                flags = Command.Read<int>(args, 2);

            var pChar = session.Character;

            var pkt = new PacketWriter(ServerMessage.PlayScene);

            pkt.WriteInt32(0); // SceneID
            pkt.WriteInt32(flags); // PlaybackFlags
            pkt.WriteUInt32(++sceneInstanceId); // SceneInstanceID
            pkt.WriteUInt32(sceneId); // SceneScriptPackageID

            pkt.WriteUInt16(0); // TransportGUID

            {
                pkt.WriteFloat(pChar.Position.X);
                pkt.WriteFloat(pChar.Position.Y);
                pkt.WriteFloat(pChar.Position.Z);
                pkt.WriteFloat(pChar.Position.O);
            }
            pkt.WriteUInt8(1);

            if (session != null)
            {
                session.Send(ref pkt);

                Console.WriteLine($"Started scene {sceneInstanceId}.");

                ChatMessageValues chatMessage = new ChatMessageValues(0, "");
                chatMessage.Message = $"Started scene {sceneInstanceId}.";
                ChatHandler.SendMessage(ref session, chatMessage);
            }
        }

#if !PUBLIC
        [ChatCommand("cscene", "")]
        public static void CancelScene(string[] args, WorldClass session)
        {
            var sceneId = 0u;

            if (args.Length == 2)
                sceneId = Command.Read<uint>(args, 1);
            else
                sceneId = sceneInstanceId;

            var pChar = session.Character;
            var pkt = new PacketWriter(ServerMessage.CancelScene);

            pkt.WriteUInt32(sceneId); // SceneScriptPackageID
            session.Send(ref pkt);

            Console.WriteLine($"Stopped scene {sceneId}.");

            ChatMessageValues chatMessage = new ChatMessageValues(0, "");
            chatMessage.Message = $"Stopped scene {sceneId}.";
            ChatHandler.SendMessage(ref session, chatMessage);
        }
        
        
        [ChatCommand("say", "")]
        public static void Say(string[] args, WorldClass session)
        {
            var type = Command.Read<string>(args, 1);
            var msg = Command.Read<string>(args, 2);

            for (var i = 3; i < args.Length; i++)
                msg += " " + Command.Read<string>(args, i);

            var time = 5000;// Command.Read<float>(args, 3);

            if (!Enum.TryParse<MessageType>($"ChatMessage{type}", out var msgType))
                return;

            ChatMessageValues chatMessage = new ChatMessageValues(msgType, msg, true, true, session.Character.Name);

            // Maaaaybe
            chatMessage.Language = 7;
            chatMessage.Time = time;

            ChatHandler.SendMessage(ref session, chatMessage);
        }
#endif

#if PUBLIC == false
        [ChatCommand("scene", "sceneId")]
#endif
        public static void Scene(string[] args, WorldClass session)
        {
            var sceneId = Command.Read<uint>(args, 1);
            var pChar = session.Character;
            var pkt = new PacketWriter(ServerMessage.PlayScene);

            pkt.WriteUInt32(sceneId); // SceneID
            pkt.WriteUInt32(0); // PlaybackFlags
            pkt.WriteUInt32(++sceneInstanceId); // SceneInstanceID
            pkt.WriteUInt32(0); // SceneScriptPackageID

            pkt.WriteUInt16(0); // TransportGUID

            pkt.WriteFloat(pChar.Position.X);
            pkt.WriteFloat(pChar.Position.Y);
            pkt.WriteFloat(pChar.Position.Z);
            pkt.WriteFloat(pChar.Position.O);

            session.Send(ref pkt);
        }

#if PUBLIC == false
        [ChatCommand("wstate", "wstateId val")]
#endif
        public static void WState(string[] args, WorldClass session)
        {
            var sceneId = Command.Read<uint>(args, 1);
            var val = Command.Read<uint>(args, 1);
            var pChar = session.Character;

            var pkt = new PacketWriter(ServerMessage.UpdateWorldState);

            pkt.WriteUInt32(sceneId); // id
            pkt.WriteUInt32(val); // flags
            pkt.WriteUInt8(0); // id

            session.Send(ref pkt);
        }


        [ChatCommand("morph", "Usage: !morph #displayId (Change the current displayId for your own character)")]
        public static void Morph(string[] args, WorldClass session)
        {
            var displayId = 0u;

            if (args.Length == 2)
                displayId = Command.Read<uint>(args, 1);

            var pChar = session.Character;

            if (ReplayManager.GetInstance().ReplayMode)
            {
                if (session.TargetGuid != 0)
                {
                    WorldObject character = session.Character;
                    PacketWriter updateObject = new PacketWriter(ServerMessage.ObjectUpdate);
                    BitPack BitPack = new BitPack(updateObject);

                    var mapId = Command.Read<ushort>(args, 2);

                    updateObject.WriteUInt32(1);
                    updateObject.WriteUInt16(mapId);
                    updateObject.WriteUInt8(0);
                    updateObject.WriteInt32(0);

                    updateObject.WriteUInt8((byte)UpdateType.Values);
                    updateObject.WriteSmartGuid(session.TargetGuid);

                    var writer = new PacketWriter();
                    var bp = new BitPack(writer);
                    var bits = new BitArray((((int)UnitFields.End + 32 - 1) / 32) * 32, false);

                    for (var i = 0; i < bits.Length; i += 32)
                        bits.Set(i, true);

                    //bits.Set(0, true);
                    bits.Set((int)UnitFields.NativeDisplayID, true);
                    bits.Set((int)UnitFields.DisplayID, true);

                    // Always send all uint32 flags
                    bp.Write(63, 7);

                    byte[] ret = new byte[(bits.Length) / 8 + 1];
                    bits.CopyTo(ret, 0);

                    var masks = new uint[ret.Length / 4];

                    for (var i = 0; i < masks.Length; i++)
                        masks[i] = BitConverter.ToUInt32(ret, i * 4);

                    for (var i = 0; i < masks.Length; i++)
                    {
                        bp.Write(masks[i], 32);
                    }

                    bp.Flush();

                    updateObject.Write((writer.BaseStream as MemoryStream).ToArray().Length + 4 + 8);
                    updateObject.WriteUInt32(0x20);

                    updateObject.Write((writer.BaseStream as MemoryStream).ToArray());

                    updateObject.Write(displayId);
                    updateObject.Write(displayId);

                    var size = (uint)updateObject.BaseStream.Length - 29;
                    updateObject.WriteUInt32Pos(size, 25);

                    session.Send(ref updateObject);
                }
            }
            else if (pChar != null)
            {
                if (session.TargetGuid != 0)
                {
                    WorldObject spawn;

                    if (pChar.InRangeObjects.TryGetValue(session.TargetGuid, out spawn))
                        ObjectHandler.Update(UnitFields.DisplayID, displayId, spawn);
                }
                else
                {
                    if (displayId == 0)
                    {
                        var race = Manager.WorldMgr.ChrRaces.Single(r => r.Id == pChar.Race);
                        displayId = pChar.Gender == 0 ? race.MaleDisplayId : race.FemaleDisplayId;
                    }

                    ObjectHandler.Update(UnitFields.DisplayID, displayId, pChar);
                    //ObjectHandler.Update(UnitFields.NativeDisplayID, displayId, pChar);
                }
            }
        }

        [ChatCommand("emote", "")]
        public static void Emote(string[] args, WorldClass session)
        {
            var emoteId = Command.Read<uint>(args, 1);
            var permanent = 0;

            if (args.Length == 3)
                permanent = Command.Read<int>(args, 2);

            var pChar = session.Character;

            if (pChar != null)
            {
                if (session.TargetGuid != 0)
                {
                    WorldObject spawn;

                    if (pChar.InRangeObjects.TryGetValue(session.TargetGuid, out spawn))
                    {
                        if (spawn is Character)
                        {
                            if (permanent == 1)
                            {
                                var playerSpawn = Manager.WorldMgr.PlayerSpawns.SingleOrDefault(c => c.Guid == session.TargetGuid);
                                var player = playerSpawn.Character;

                                WorldObject character = session.Character;
                                PacketWriter updateObject = new PacketWriter(ServerMessage.ObjectUpdate);
                                BitPack BitPack = new BitPack(updateObject);

                                updateObject.WriteUInt32(1);
                                updateObject.WriteUInt16((ushort)player.Map);
                                updateObject.WriteUInt8(0);
                                updateObject.WriteInt32(0);

                                updateObject.WriteUInt8((byte)UpdateType.Values);
                                updateObject.WriteSmartGuid(playerSpawn.Guid);

                                var writer = new PacketWriter();
                                var bp = new BitPack(writer);
                                var bits = new BitArray((((int)UnitFields.End + 32 - 1) / 32) * 32, false);

                                for (var i = 0; i < bits.Length; i += 32)
                                    bits.Set(i, true);

                                bits.Set((int)UnitFields.EmoteState, true);

                                // Always send all uint32 flags
                                bp.Write(63, 7);

                                byte[] ret = new byte[(bits.Length) / 8 + 1];
                                bits.CopyTo(ret, 0);

                                var masks = new uint[ret.Length / 4];

                                for (var i = 0; i < masks.Length; i++)
                                    masks[i] = BitConverter.ToUInt32(ret, i * 4);

                                for (var i = 0; i < masks.Length; i++)
                                {
                                    bp.Write(masks[i], 32);
                                }

                                bp.Flush();

                                updateObject.Write((writer.BaseStream as MemoryStream).ToArray().Length + 4 + 4);
                                updateObject.WriteUInt32(0x20);

                                updateObject.Write((writer.BaseStream as MemoryStream).ToArray());
                                updateObject.Write(emoteId);

                                player.EmoteId = emoteId;

                                var size = (uint)updateObject.BaseStream.Length - 29;
                                updateObject.WriteUInt32Pos(size, 25);

                                session.Send(ref updateObject);

                                Manager.SpawnMgr.UpdatePlayerSpawn(playerSpawn.Guid, player);
                                EmoteHandler.SendEmote(emoteId, session, spawn, session.TargetGuid);
                            }
                            else
                                EmoteHandler.SendEmote(emoteId, session, spawn, session.TargetGuid);
                        }
                        else
                        {
                            if (permanent == 1)
                            {
                                WorldObject character = session.Character;
                                PacketWriter updateObject = new PacketWriter(ServerMessage.ObjectUpdate);
                                BitPack BitPack = new BitPack(updateObject);

                                updateObject.WriteUInt32(1);
                                updateObject.WriteUInt16((ushort)spawn.Map);
                                updateObject.WriteUInt8(0);
                                updateObject.WriteInt32(0);

                                updateObject.WriteUInt8((byte)UpdateType.Values);
                                updateObject.WriteSmartGuid(spawn.SGuid);

                                var writer = new PacketWriter();
                                var bp = new BitPack(writer);
                                var bits = new BitArray((((int)UnitFields.End + 32 - 1) / 32) * 32, false);

                                for (var i = 0; i < bits.Length; i += 32)
                                    bits.Set(i, true);

                                bits.Set((int)UnitFields.EmoteState, true);

                                // Always send all uint32 flags
                                bp.Write(63, 7);
                                byte[] ret = new byte[(bits.Length) / 8 + 1];
                                bits.CopyTo(ret, 0);

                                var masks = new uint[ret.Length / 4];

                                for (var i = 0; i < masks.Length; i++)
                                    masks[i] = BitConverter.ToUInt32(ret, i * 4);

                                for (var i = 0; i < masks.Length; i++)
                                {
                                    bp.Write(masks[i], 32);
                                }

                                bp.Flush();

                                updateObject.Write((writer.BaseStream as MemoryStream).ToArray().Length + 4 + 4);
                                updateObject.WriteUInt32(0x20);

                                updateObject.Write((writer.BaseStream as MemoryStream).ToArray());
                                updateObject.Write(emoteId);

                                (spawn as CreatureSpawn).Emote = (int)emoteId;

                                var size = (uint)updateObject.BaseStream.Length - 29;
                                updateObject.WriteUInt32Pos(size, 25);

                                session.Send(ref updateObject);

                                Manager.SpawnMgr.UpdateSpawnEmote(session.TargetGuid, (int)emoteId);
                                EmoteHandler.SendEmote(emoteId, session, spawn, session.TargetGuid);
                            }
                            else
                                EmoteHandler.SendEmote(emoteId, session, spawn);
                        }
                    }
                }
                else
                    EmoteHandler.SendEmote(emoteId, session, null);
            }
        }

        [ChatCommand2("fly", "Usage: !fly #state (Turns the fly mode 'on' or 'off')")]
        public static void Fly(string[] args, WorldClass2 session)
        {
            var state = Command.Read<string>(args, 1);
            var message = state == "on" ? "Fly mode enabled." : "Fly mode disabled.";

            var sess1 = Manager.WorldMgr.GetSession(session.Character.Guid);
            ChatMessageValues chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, message);

            if (state == "on")
            {
                sess1.IsFlying = true;

                MoveHandler.HandleMoveSetCanFly(ref session);
                ChatHandler.SendMessage(ref sess1, chatMessage);
            }
            else if (state == "off")
            {
                sess1.IsFlying = false;

                MoveHandler.HandleMoveUnsetCanFly(ref session);
                ChatHandler.SendMessage(ref sess1, chatMessage);
            }
        }

        [ChatCommand("loc", "Usage: !loc #name (If name is used the location will be written to 'locations.txt'")]
        public static void Loc(string[] args, WorldClass session)
        {
            var sb = new StringBuilder();
            var pChar = session.Character;
            var locName = "Your Location";

            if (args.Length == 2)
                locName = Command.Read<string>(args, 1);

            var msg = $"Your Location: \r\nMap: {pChar.Map}\r\nX: |cffffffff|Hpos:|h{pChar.Position.X}|h|r Y: |cffffffff|Hpos:|h{pChar.Position.Y}|h|r Z: |cffffffff|Hpos:|h{pChar.Position.Z}|h|r O: |cffffffff|Hpos:|h{pChar.Position.O}|h|r";

            sb.AppendLine(string.Format("{5}: {0} {1} {2} {3} {4}", pChar.Position.X, pChar.Position.Y, pChar.Position.Z, pChar.Position.O, pChar.Map, locName));
            sb.AppendLine(string.Format("{5}: {0}, {1}, {2}, {3}, {4}", pChar.Position.X, pChar.Position.Y, pChar.Position.Z, pChar.Position.O, pChar.Map, locName));

            var sess1 = Manager.WorldMgr.GetSession(session.Character.Guid);
            var message = msg;

            ChatMessageValues chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, "");

            chatMessage.Message = message;

            ChatHandler.SendMessage(ref sess1, chatMessage);

            if (args.Length == 2)
                File.AppendAllText(Helper.DataDirectory() + "locations.txt", sb.ToString());
        }

        [ChatCommand2("move", "Usage: !tele [#x #y #z #o #map]")]
        public static void MovePosition(string[] args, WorldClass2 session)
        {
            var pChar = session.Character;
            var coord = Command.Read<string>(args, 1).ToLower();
            var value = Command.Read<float>(args, 2);

            var vector = new Vector4()
            {
                X = coord == "x" ? session.Character.Position.X + value : session.Character.Position.X,
                Y = coord == "y" ? session.Character.Position.Y + value : session.Character.Position.Y,
                Z = coord == "z" ? session.Character.Position.Z + value : session.Character.Position.Z,
            };

            MoveHandler.HandleMoveTeleport(ref session, vector);
            Manager.ObjectMgr.SetPosition(ref pChar, vector);
        }

        [ChatCommand2("tele", "Usage: !tele [#x #y #z #o #map]")]
        public static void Teleport(string[] args, WorldClass2 session)
        {
            var sess1 = Manager.WorldMgr.GetSession(session.Character.Guid);
            var pChar = session.Character;
            Vector4 vector = null;

            uint mapId = 0;

            if (args.Length > 2)
            {
                vector = new Vector4()
                {
                    X = Command.Read<float>(args, 1),
                    Y = Command.Read<float>(args, 2),
                    Z = Command.Read<float>(args, 3),
                };

                if (args.Length == 6)
                {
                    vector.O = Command.Read<float>(args, 4);

                    mapId = Command.Read<uint>(args, 5);
                }
                else
                {
                    mapId = Command.Read<uint>(args, 4);
                }
            }

            if (vector != null)
            {
                if (pChar.Map == mapId)
                {
                    MoveHandler.HandleMoveTeleport(ref session, vector);
                    Manager.ObjectMgr.SetPosition(ref pChar, vector);
                }
                else
                {
                    MoveHandler.HandleTransferPending(ref session, mapId);
                    MoveHandler.HandleNewWorld(ref session, vector, mapId);

                    Manager.ObjectMgr.SetPosition(ref pChar, vector);
                    Manager.ObjectMgr.SetMap(ref pChar, mapId);


                    var sss1 = Manager.WorldMgr.GetSession(session.Character.Guid);

                    ObjectHandler.HandleUpdateObjectCreate(ref sss1, false);

                    if (sess1.IsFlying)
                        MoveHandler.HandleMoveSetCanFly(ref session);

                    MoveHandler.HandleMoveSetFlightSpeed(ref session, sess1.Flightspeed);
                    MoveHandler.HandleMoveSetRunSpeed(ref session, sess1.RunSpeed);
                }
            }
        }

        [ChatCommand2("flightspeed", "Usage: !flightspeed #speed (Set the current flight speed)")]
        public static void FlightSpeed(string[] args, WorldClass2 session)
        {
            ChatMessageValues chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, "");
            var sess1 = Manager.WorldMgr.GetSession(session.Character.Guid);
            if (args.Length == 1)
                MoveHandler.HandleMoveSetFlightSpeed(ref session);
            else
            {
                var speed = Command.Read<float>(args, 1);

                if (speed <= 1000 && speed > 0)
                {
                    chatMessage.Message = "Flight speed set to " + speed + "!";

                    sess1.Flightspeed = speed;

                    MoveHandler.HandleMoveSetFlightSpeed(ref session, speed);
                    ChatHandler.SendMessage(ref sess1, chatMessage);
                }
                else
                {
                    chatMessage.Message = "Please enter a value between 0.0 and 50.0!";

                    ChatHandler.SendMessage(ref sess1, chatMessage);
                }

                return;
            }

            chatMessage.Message = "Flight speed set to default.";

            ChatHandler.SendMessage(ref sess1, chatMessage);
        }

        [ChatCommand2("swimspeed", "Usage: !swimspeed #speed (Set the current swim speed)")]
        public static void SwimSpeed(string[] args, WorldClass2 session)
        {
            ChatMessageValues chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, "");
            var sess1 = Manager.WorldMgr.GetSession(session.Character.Guid);
            if (args.Length == 1)
                MoveHandler.HandleMoveSetSwimSpeed(ref session);
            else
            {
                var speed = Command.Read<float>(args, 1);
                if (speed <= 1000 && speed > 0)
                {
                    chatMessage.Message = "Swim speed set to " + speed + "!";

                    MoveHandler.HandleMoveSetSwimSpeed(ref session, speed);
                    ChatHandler.SendMessage(ref sess1, chatMessage);
                }
                else
                {
                    chatMessage.Message = "Please enter a value between 0.0 and 50.0!";

                    ChatHandler.SendMessage(ref sess1, chatMessage);
                }

                return;
            }

            chatMessage.Message = "Swim speed set to default.";

            ChatHandler.SendMessage(ref sess1, chatMessage);
        }

        [ChatCommand2("runspeed", "Usage: !runspeed #speed (Set the current run speed)")]
        public static void RunSpeed(string[] args, WorldClass2 session)
        {
            ChatMessageValues chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, "");
            var sess1 = Manager.WorldMgr.GetSession(session.Character.Guid);
            if (args.Length == 1)
                MoveHandler.HandleMoveSetRunSpeed(ref session);
            else
            {
                var speed = Command.Read<float>(args, 1);
                if (speed <= 1000 && speed > 0)
                {
                    chatMessage.Message = "Run speed set to " + speed + "!";

                    sess1.RunSpeed = speed;

                    MoveHandler.HandleMoveSetRunSpeed(ref session, speed);
                    ChatHandler.SendMessage(ref sess1, chatMessage);
                }
                else
                {
                    chatMessage.Message = "Please enter a value between 0.0 and 50.0!";

                    ChatHandler.SendMessage(ref sess1, chatMessage);
                }

                return;
            }

            chatMessage.Message = "Run speed set to default.";

            ChatHandler.SendMessage(ref sess1, chatMessage);
        }

        [ChatCommand("mount", "Usage: !mount #displayId (Without 'displayId' to remove the mount)")]
        public static void Mount(string[] args, WorldClass session)
        {
            var sss1 = Manager.WorldMgr.GetSession(session.Character.Guid);
            var pChar = session.Character;
            if (args.Length == 1)
            {
                ObjectHandler.Update(UnitFields.MountDisplayID, 0, pChar);

                PacketWriter setRunSpeed = new PacketWriter(ServerMessage.MoveSetRunSpeed);

                setRunSpeed.WriteSmartGuid(session.Character.Guid);
                setRunSpeed.WriteUInt32(0);
                setRunSpeed.WriteFloat(7);

                session.Send(ref setRunSpeed);
            }
            else
            {

                ObjectHandler.Update(UnitFields.MountDisplayID, Command.Read<uint>(args, 1), pChar);

                PacketWriter setRunSpeed = new PacketWriter(ServerMessage.MoveSetRunSpeed);

                setRunSpeed.WriteSmartGuid(session.Character.Guid);
                setRunSpeed.WriteUInt32(0);
                setRunSpeed.WriteFloat(14);

                session.Send(ref setRunSpeed);
            }
        }

#if PUBLIC == false
        [ChatCommand("smount", "Usage: !mount #displayId (Without 'displayId' to remove the mount)")]
#endif
        public static void SpellMount(string[] args, WorldClass session)
        {
            var sss1 = Manager.WorldMgr.GetSession(session.Character.Guid);


            var pChar = session.Character;
            if (args.Length == 1)
            {
                SpellHandler.AuraUpdate(sss1, 0, new SmartGuid(1, 0, GuidType.Cast));

                ObjectHandler.Update(UnitFields.MountDisplayID, 0, session.Character);

            }
            else
            {
                int displayId = Command.Read<int>(args, 1);
                var spell = 61451u;

                if (args.Length == 3)
                    spell = Command.Read<uint>(args, 2);

                var collisionHeight = new PacketWriter(ServerMessage.SetCollision);

                collisionHeight.WriteSmartGuid(session.Character.Guid);
                collisionHeight.WriteUInt32(0);
                collisionHeight.WriteFloat(2.7977f);
                collisionHeight.WriteFloat(1);
                collisionHeight.WriteUInt8(2);
                collisionHeight.WriteInt32(displayId);
                collisionHeight.WriteUInt32(5000000);

                session.Send(ref collisionHeight);

                SpellHandler.AuraUpdate(sss1, spell, new SmartGuid(1, (int)spell, GuidType.Cast));
                SpellHandler.AuraUpdate(sss1, 86458, new SmartGuid(1, (int)86458, GuidType.Cast), 40);

                ObjectHandler.Update(UnitFields.MountDisplayID, displayId, session.Character);
            }
        }

        [ChatCommand("anim", "Usage: !anim #animId (Without 'displayId' to remove the mount)")]
        public static void Anim(string[] args, WorldClass session)
        {
            var sss1 = Manager.WorldMgr.GetSession(session.Character.Guid);

            WorldObject pChar = session.Character;

            if (session.TargetGuid != 0)
            {
                WorldObject spawn;

                if (session.Character.InRangeObjects.TryGetValue(session.TargetGuid, out spawn))
                {
                    pChar = spawn;
                }
            }


            if (args.Length == 1)
            {
                ObjectHandler.Update(UnitFields.StateAnimID, 0, pChar);
            }
            else
            {
                var animId = Command.Read<int>(args, 1);

                ObjectHandler.Update(UnitFields.StateAnimID, animId, pChar);

            }
        }

        [ChatCommand("ctime")]
        public static void ChangeTime(string[] args, WorldClass session)
        {
            if (args.Length > 1)
            {
                var hours = Command.Read<int>(args, 1);
                var speed = 0.01666667f;

                if (args.Length > 2)
                    speed = Command.Read<float>(args, 1);

                PacketWriter loginSetTimeSpeed = new PacketWriter(ServerMessage.LoginSetTimeSpeed);

                loginSetTimeSpeed.WritePackedTime(hours);
                loginSetTimeSpeed.WritePackedTime(hours);
                loginSetTimeSpeed.WriteFloat(speed);
                loginSetTimeSpeed.WriteInt32(0);
                loginSetTimeSpeed.WriteInt32(0);

                session.Send(ref loginSetTimeSpeed);
            }
        }

        [ChatCommand("addspell")]
        public static void AddSpell(string[] args, WorldClass session)
        {
            var spelsl = new List<uint>();

            for (var i = 1; i < args.Length; i++)
            {
                var spellId = Command.Read<uint>(args, i);
                spelsl.Add(spellId);

                session.Character.SpellList.Add(new PlayerSpell
                {
                    SpellId = spellId,
                    State = PlayerSpellState.Unchanged
                });
            }

            Manager.ObjectMgr.SaveChar(session.Character);
            SpellHandler.HandleLearnedSpells(ref session, spelsl);
        }

        [ChatCommand2("commentator", "Usage: !anim #animId (Without 'displayId' to remove the mount)")]
        public static void commentator(string[] args, WorldClass2 session)
        {
            var onOrOff = Command.Read<string>(args, 1);
            var sss1 = Manager.WorldMgr.GetSession(session.Character.Guid);

            //ObjectHandler.Update<uint>(PlayerFields.PlayerFlags, onOrOff == "on" ? 0x00400000 | 0x00080000 : 0u, sss1.Character);

            var pChar = session.Character;
            var pChar1 = sss1.Character;

            var vector = new Vector4
            {
                X = pChar.Position.X,
                Y = pChar.Position.Y,
                Z = pChar.Position.Z,
                O = pChar.Position.O
            };

            MoveHandler.HandleTransferPending(ref session, pChar.Map);
            MoveHandler.HandleNewWorld(ref session, vector, pChar.Map);

            Manager.ObjectMgr.SetPosition(ref pChar, vector);
            Manager.ObjectMgr.SetMap(ref pChar, pChar.Map);

            ObjectHandler.HandleUpdateObjectCreate(ref sss1, false, onOrOff == "on");
        }

#if PUBLIC == false
        [ChatCommand("phase")]
#endif
        public static void Phase(string[] args, WorldClass session)
        {
            var phaseIds = Command.Read<string>(args, 1).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var mapId = Command.Read<string>(args, 2).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var preloadMapid = Command.Read<string>(args, 3).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var mapAreaId = Command.Read<string>(args, 4).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            var pkt = new PacketWriter(ServerMessage.PhaseChange);

            if (phaseIds.Length == 1 && phaseIds[0] == "-1")
                phaseIds = new string[0];

            if (mapId.Length == 1 && mapId[0] == "-1")
                mapId = new string[0];

            if (preloadMapid.Length == 1 && preloadMapid[0] == "-1")
                preloadMapid = new string[0];

            if (mapAreaId.Length == 1 && mapAreaId[0] == "-1")
                mapAreaId = new string[0];

            pkt.WriteSmartGuid(session.Character.Guid);
            pkt.WriteInt32(phaseIds.Length > 1 ? 16 : 8);
            pkt.WriteInt32(phaseIds.Length); // phases count
            pkt.WriteSmartGuid(session.Character.Guid);

            // 1 phase change
            foreach (var fid in phaseIds)
            {
                pkt.WriteUInt16(1);
                pkt.WriteUInt16(ushort.Parse(fid));
            }
            // !phase 1 1903 9310
            pkt.WriteInt32(mapId.Length * 2);

            foreach (var fid in mapId)
            {
                pkt.WriteUInt16(ushort.Parse(fid));
            }

            pkt.WriteInt32(preloadMapid.Length * 2);

            foreach (var fid in preloadMapid)
            {
                pkt.WriteUInt16(ushort.Parse(fid));
            }

            pkt.WriteInt32(mapAreaId.Length * 2);

            foreach (var fid in mapAreaId)
            {
                pkt.WriteUInt16(ushort.Parse(fid));
            }

            var sess2 = Manager.WorldMgr.GetSession2(session.Character.Guid);

            sess2.Send(ref pkt);
        }

        [ChatCommand("level")]
        public static void level(string[] args, WorldClass session)
        {
            int level = Command.Read<int>(args, 1);
            if (level > 0 && level < 61)
            {
                var pkt = new PacketWriter(ServerMessage.LevelUpInfo);

                pkt.WriteInt32(level);
                pkt.WriteInt32(1);

                for (var i = 0; i < 6; i++)
                    pkt.WriteInt32(0);

                for (var i = 0; i < 4; i++)
                    pkt.WriteInt32(0);

                pkt.WriteInt32(1);
                pkt.WriteInt32(0);

                //session.Send(ref pkt);

                var pChar = session.Character;

                pChar.Level = (byte)level;

                Manager.ObjectMgr.SaveChar(pChar);

                CharacterHandler.SendAchievementData2(ref session);
                CharacterHandler.SendAchievementData3(ref session);

                var sess2 = Manager.WorldMgr.GetSession2(session.Character.Guid);
                MoveHandler.HandleTransferPending(ref sess2, pChar.Map);
                MoveHandler.HandleNewWorld(ref sess2, session.Character.Position, pChar.Map);

                Manager.ObjectMgr.SetPosition(ref pChar, session.Character.Position);
                Manager.ObjectMgr.SetMap(ref pChar, pChar.Map);

                ObjectHandler.HandleUpdateObjectCreate(ref session, false, false);

                if (session.IsFlying)
                    MoveHandler.HandleMoveSetCanFly(ref sess2);

                MoveHandler.HandleMoveSetFlightSpeed(ref sess2, session.Flightspeed);
                MoveHandler.HandleMoveSetRunSpeed(ref sess2, session.RunSpeed);
            }
        }

        [ChatCommand("addnpc")]
        public static void AddNpc(string[] args, WorldClass session)
        {
            Manager.WorldMgr.waitAdd = true;

            var pChar = session.Character;
            var creatureId = Command.Read<int>(args, 1);
            var creature = Manager.ObjectMgr.Creatures.ContainsKey(creatureId) ? Manager.ObjectMgr.Creatures[creatureId] : null;

            if (creature != null)
            {
                ChatMessageValues chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, "");

                CreatureSpawn spawn = new CreatureSpawn()
                {
                    Guid = CreatureSpawn.GetLastGuid() + 1,
                    Id = creatureId,
                    Creature = creature,
                    Position = pChar.Position,
                    Map = pChar.Map,
                    ModelId = (uint)creature.DisplayInfoId[0].displayId,
                    Scale = 1,
                    EquipmentId = 0,
                    NpcFlags = 0,
                    UnitFlags = 0,
                    FactionTemplate = 7,
                    Level = 1,
                    MountDisplayId = 0,
                    HoverHeight = 1,
                    AnimState = 1716
                };

                spawn.AddToWorld();

                chatMessage.Message = "Spawn successfully added.";

                ChatHandler.SendMessage(ref session, chatMessage);
            }

            Manager.WorldMgr.waitAdd = false;
        }

        [ChatCommand("addfile")]
        public static void AddFile(string[] args, WorldClass session)
        {
            Manager.WorldMgr.waitAdd = true;

            var fileType = Command.Read<string>(args, 1);
            var fileId = Command.Read<uint>(args, 2);

            // Gameobjects
            if (fileType == "wmo")
            {
                var creationType = "reuse";

                if (args.Length == 4)
                    creationType = Command.Read<string>(args, 3);

                // This is the default behavior.
                // Reuse an existing display id. Create a new one if no exists.
                if (creationType == "reuse")
                {
                    // Only create a hotfix if none exists for the current file id.
                    if (!Manager.Hotfix.GameObjectFileHotfixes.TryGetValue(fileId, out var hotfixes))
                    {
                        var gamoObjectDisplayInfo = new GameObjectDisplayInfo
                        {
                            // Start at display id 150001
                            Id = 150_000 + (uint)Manager.Hotfix.GameObjectFileHotfixes.Count + 1,
                            FileId = fileId
                        };

                        hotfixes = gamoObjectDisplayInfo;

                        Manager.Hotfix.GameObjectFileHotfixes[fileId] = hotfixes;
                    }

                    // Send hotfix.
                    Manager.Hotfix.CreateGameObjectDisplayEntry(session);

                    Thread.Sleep(2000);

                    // Spawn
                    AddWmo(fileId, session);
                }
                else if (creationType == "owrite")
                {

                }
                else if (creationType == "replace")
                {

                }
            }
            // Creatures & Items
            else if (fileType == "m2")
            {

            }

            Manager.WorldMgr.waitAdd = false;
        }

        [ChatCommand("clearequip")]
        public static void ClearEquip(string[] args, WorldClass session)
        {
            Manager.WorldMgr.waitAdd = true;

            var pChar = session.Character;
            var itemid = Command.Read<int>(args, 1);
            var cguid = (session.TargetGuid);

            if (cguid != 0)
            {
                WorldObject spawn;

                if (pChar.InRangeObjects.TryGetValue(session.TargetGuid, out spawn))
                {
                    Manager.SpawnMgr.CreatureItems.Remove(cguid);

                    var updateObject = new PacketWriter(ServerMessage.ObjectUpdate);

                    updateObject.WriteUInt32(1);
                    updateObject.WriteUInt16((ushort)spawn.Map);
                    updateObject.WriteUInt8(0);
                    updateObject.WriteInt32(0);

                    updateObject.WriteUInt8((byte)UpdateType.Values);

                    updateObject.WriteSmartGuid(spawn.SGuid);

                    var writer = new PacketWriter();
                    var bp = new BitPack(writer);
                    var maskSize = 153 - (153 % 32);
                    var bits = new BitArray(maskSize + 32, false);

                    // Set the uint32 enable bit based on the update bit position.
                    bits.Set(maskSize, true);
                    bits.Set((int)UnitFields.EnableVirtualItems, true);
                    bits.Set((int)UnitFields.VirtualItems, true);
                    bits.Set((int)UnitFields.VirtualItems + 1, true);
                    bits.Set((int)UnitFields.VirtualItems + 2, true);

                    // Copy the bit mask value into uint32 array.
                    var ret = new byte[(bits.Length) / 8 + 1];

                    bits.CopyTo(ret, 0);

                    var masks = new uint[ret.Length / 4];

                    for (var i = 0; i < masks.Length; i++)
                        masks[i] = BitConverter.ToUInt32(ret, i * 4);

                    // hax for now. Fix for ActivePlayerFields
                    if (7 > 32)
                        updateObject.WriteUInt32(0);// masks[maskIndex++]);

                    if (7 > 1)
                    {
                        bp.Write(1 << (masks.Length - 1), 7);

                        for (var i = 0; i < masks.Length; i++)
                        {
                            if (masks[i] != 0)
                                bp.Write(masks[i], 32);
                        }
                    }
                    else
                        bp.Write(masks[0], 7);

                    bp.Flush();

                    // Append the updatefield data to the update object packet.
                    updateObject.Write((writer.BaseStream as MemoryStream).ToArray().Length + 4 + 12 * 3);
                    updateObject.WriteInt32(0x20);

                    updateObject.Write((writer.BaseStream as MemoryStream).ToArray());

                    for (var i = 0; i < 3; i++)
                    {
                        updateObject.WriteInt8(0x1F);
                        updateObject.WriteInt32(0);
                        updateObject.WriteInt32(0);
                        updateObject.WriteUInt16(0);
                        updateObject.WriteUInt16(0);
                    }

                    var size = (uint)updateObject.BaseStream.Length - 29;
                    updateObject.WriteUInt32Pos(size, 25);

                    Manager.WorldMgr.Sessions.First().Value.Send(ref updateObject);
                }
            }
        }

        [ChatCommand("addequip")]
        public static void AddEquip(string[] args, WorldClass session)
        {
            Manager.WorldMgr.waitAdd = true;

            var pChar = session.Character;
            var itemid = Command.Read<int>(args, 1);
            var cguid = (session.TargetGuid);

            if (cguid != 0)
            {
                Manager.SpawnMgr.CreatureItems.TryGetValue(cguid, out var items);
                {
                    if (items == null)
                        items = new CreatureEquip { Guid = cguid, Items = new() };

                    if (Manager.Equipments.AvailableItems.TryGetValue(itemid, out var item))
                        items.Items.Add(item);

                    Manager.SpawnMgr.CreatureItems[cguid] = items;

                    if (session.TargetGuid != 0)
                    {
                        WorldObject spawn;

                        if (pChar.InRangeObjects.TryGetValue(session.TargetGuid, out spawn))
                        {
                            var updateObject = new PacketWriter(ServerMessage.ObjectUpdate);

                            updateObject.WriteUInt32(1);
                            updateObject.WriteUInt16((ushort)spawn.Map);
                            updateObject.WriteUInt8(0);
                            updateObject.WriteInt32(0);

                            updateObject.WriteUInt8((byte)UpdateType.Values);

                            updateObject.WriteSmartGuid(spawn.SGuid);

                            var writer = new PacketWriter();
                            var bp = new BitPack(writer);
                            var maskSize = 153 - (153 % 32);
                            var bits = new BitArray(maskSize + 32, false);

                            // Set the uint32 enable bit based on the update bit position.
                            bits.Set(maskSize, true);
                            bits.Set((int)UnitFields.EnableVirtualItems, true);
                            bits.Set((int)UnitFields.VirtualItems, true);
                            bits.Set((int)UnitFields.VirtualItems + 1, true);
                            bits.Set((int)UnitFields.VirtualItems + 2, true);

                            // Copy the bit mask value into uint32 array.
                            var ret = new byte[(bits.Length) / 8 + 1];

                            bits.CopyTo(ret, 0);

                            var masks = new uint[ret.Length / 4];

                            for (var i = 0; i < masks.Length; i++)
                                masks[i] = BitConverter.ToUInt32(ret, i * 4);

                            // hax for now. Fix for ActivePlayerFields
                            if (7 > 32)
                                updateObject.WriteUInt32(0);// masks[maskIndex++]);

                            if (7 > 1)
                            {
                                bp.Write(1 << (masks.Length - 1), 7);

                                for (var i = 0; i < masks.Length; i++)
                                {
                                    if (masks[i] != 0)
                                        bp.Write(masks[i], 32);
                                }
                            }
                            else
                                bp.Write(masks[0], 7);

                            bp.Flush();

                            // Append the updatefield data to the update object packet.
                            updateObject.Write((writer.BaseStream as MemoryStream).ToArray().Length + 4 + 12 * 3);
                            updateObject.WriteInt32(0x20);

                            updateObject.Write((writer.BaseStream as MemoryStream).ToArray());

                            foreach (var item2 in items.Items)
                            {
                                updateObject.WriteInt8(0x1F);
                                updateObject.WriteInt32(item2.Id);
                                updateObject.WriteInt32(0);
                                updateObject.WriteUInt16((ushort)item2.ModId);
                                updateObject.WriteUInt16(0);
                            }

                            for (var i = 0; i < 3 - items.Items.Count; i++)
                            {
                                updateObject.WriteInt8(0x1F);
                                updateObject.WriteInt32(0);
                                updateObject.WriteInt32(0);
                                updateObject.WriteUInt16(0);
                                updateObject.WriteUInt16(0);
                            }

                            var size = (uint)updateObject.BaseStream.Length - 29;
                            updateObject.WriteUInt32Pos(size, 25);

                            Manager.WorldMgr.Sessions.First().Value.Send(ref updateObject);
                        }
                    }

                    File.WriteAllText(Helper.DataDirectory() + "creatureequip.json", Json.CreateString(Manager.SpawnMgr.CreatureItems));
                }
            }
        }

#if DEBUG
        [ChatCommand("addplayer")]
        public static void AddPlayer(string[] args, WorldClass session)
        {
            var pChar = session.Character;
            var playername = Command.Read<string>(args, 1);

            // Serialize and deserialize it to create a copy of the object.
            var tempPlayer = Json.CreateString(Manager.WorldMgr.CharaterList.SingleOrDefault(c => c.Name.ToLower() == playername.ToLower()));
            var player = Json.CreateObject<Character>(tempPlayer);

            if (player != null)
            {
                ChatMessageValues chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, "");

                player.Position = pChar.Position;
                player.Map = pChar.Map;

                var guid = (ulong)new Random(Environment.TickCount).Next();
                player.Name = player.Name + guid;

                Manager.SpawnMgr.AddSpawn(player, (ulong)new Random(Environment.TickCount).Next());

                chatMessage.Message = "Player successfully added.";

                ChatHandler.SendMessage(ref session, chatMessage);
            }
        }

        [ChatCommand("delplayer")]
        public static void DelPlayer(string[] args, WorldClass session)
        {
            if (Manager.SpawnMgr.RemoveSpawn(session.TargetGuid).Item1 != null)
            {
                ChatMessageValues chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, "");

                PacketWriter updateObject = new PacketWriter(ServerMessage.ObjectUpdate);
                BitPack bp = new BitPack(updateObject);

                updateObject.WriteUInt32(0);
                updateObject.WriteUInt16((ushort)session.Character.Map);

                bp.Write(true);
                bp.Flush();

                updateObject.WriteUInt16((ushort)session.Character.Map);
                updateObject.WriteUInt32((uint)1);

                updateObject.WriteSmartGuid(session.TargetGuid);

                session.Character.InRangeObjects.Remove(session.TargetGuid);

                updateObject.WriteInt32(0);

                session.Send(ref updateObject);

                chatMessage.Message = "Player successfully deleted.";

                ChatHandler.SendMessage(ref session, chatMessage);
            }
        }

        [ChatCommand("moveplayer")]
        public static void MovePlayer(string[] args, WorldClass session)
        {
            var playerSpawn = Manager.WorldMgr.PlayerSpawns.SingleOrDefault(c => c.Guid == session.TargetGuid);
            var player = playerSpawn.Character;

            if (player != null)
            {
                player.Position = session.Character.Position;
                player.Map = session.Character.Map;

                PacketWriter moveUpdate = new PacketWriter(ServerMessage.MoveUpdate);
                BitPack BitPack = new BitPack(moveUpdate);

                moveUpdate.WriteSmartGuid(playerSpawn.Guid);
                moveUpdate.WriteUInt32(Helper.GetCurrentUnixTimestampMillis());
                moveUpdate.WriteFloat(player.Position.X);
                moveUpdate.WriteFloat(player.Position.Y);
                moveUpdate.WriteFloat(player.Position.Z);
                moveUpdate.WriteFloat(player.Position.O);
                moveUpdate.WriteFloat(0);
                moveUpdate.WriteFloat(0);
                moveUpdate.WriteUInt32(0);
                moveUpdate.WriteUInt32(0);

                BitPack.Write(0, 30);
                BitPack.Write(0, 18);

                BitPack.Write(false);
                BitPack.Write(false);
                BitPack.Write(0);
                BitPack.Write(0);
                BitPack.Write(0);
                BitPack.Flush();

                session.Send(ref moveUpdate);

                Manager.SpawnMgr.UpdatePlayerSpawn(playerSpawn.Guid, player);
            }
        }

        [ChatCommand("mountplayer")]
        public static void MountPlayer(string[] args, WorldClass session)
        {
            var playerSpawn = Manager.WorldMgr.PlayerSpawns.SingleOrDefault(c => c.Guid == session.TargetGuid);
            var player = playerSpawn.Character;

            if (player != null)
            {
                ChatMessageValues chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, "");

                WorldObject character = session.Character;
                PacketWriter updateObject = new PacketWriter(ServerMessage.ObjectUpdate);
                BitPack BitPack = new BitPack(updateObject);

                updateObject.WriteUInt32(1);
                updateObject.WriteUInt16((ushort)player.Map);
                updateObject.WriteUInt8(0);
                updateObject.WriteInt32(0);

                updateObject.WriteUInt8((byte)UpdateType.Values);
                updateObject.WriteSmartGuid(playerSpawn.Guid);

                var writer = new PacketWriter();
                var bp = new BitPack(writer);
                var bits = new BitArray((((int)UnitFields.End + 32 - 1) / 32) * 32, false);

                for (var i = 0; i < bits.Length; i += 32)
                    bits.Set(i, true);

                bits.Set((int)UnitFields.MountDisplayID, true);

                // Always send all uint32 flags
                bp.Write(63, 7);

                byte[] ret = new byte[(bits.Length) / 8 + 1];
                bits.CopyTo(ret, 0);

                var masks = new uint[ret.Length / 4];

                for (var i = 0; i < masks.Length; i++)
                    masks[i] = BitConverter.ToUInt32(ret, i * 4);

                for (var i = 0; i < masks.Length; i++)
                {
                    bp.Write(masks[i], 32);
                }

                bp.Flush();

                updateObject.Write((writer.BaseStream as MemoryStream).ToArray().Length + 4 + 4);
                updateObject.WriteUInt32(0x20);

                updateObject.Write((writer.BaseStream as MemoryStream).ToArray());

                if (args.Length == 1)
                {
                    updateObject.Write(0);

                    player.MountId = 0;
                }
                else
                {
                    var mountId = Command.Read<uint>(args, 1);

                    updateObject.Write(mountId);

                    player.MountId = mountId;
                }

                var size = (uint)updateObject.BaseStream.Length - 29;
                updateObject.WriteUInt32Pos(size, 25);

                session.Send(ref updateObject);

                Manager.SpawnMgr.UpdatePlayerSpawn(playerSpawn.Guid, player);
            }
        }
#endif

        public static void AddWmo(uint fileId, WorldClass session, uint displayId = 0)
        {
            Manager.WorldMgr.waitAdd = true;
            ChatMessageValues chatMessage = new ChatMessageValues(0, "");

            GameObjectSpawn spawn = new GameObjectSpawn()
            {
                Guid = GameObjectSpawn.GetLastGuid() + 1,
                Id = 0,
                GameObject = new GameObject { Id = 500000, Size = 1f, Type = 52, DisplayInfoId = (int)Manager.Hotfix.GameObjectFileHotfixes[fileId].Id },
                Position = session.Character.Position,
                Map = session.Character.Map
            };

            spawn.Position.O = session.Character.Position.O;

            spawn.AddToWorld();

            chatMessage.Message = $"GameobjectSpawn (Guid: {spawn.Guid}) successfully added.";

            ChatHandler.SendMessage(ref session, chatMessage);

            Manager.WorldMgr.waitAdd = false;
        }

        [ChatCommand("addobj")]
        public static void AddObj(string[] args, WorldClass session)
        {
            Manager.WorldMgr.waitAdd = true;

            var pChar = session.Character;
            var objId = Command.Read<int>(args, 1);
            var gameobject = Manager.ObjectMgr.GameObjects.ContainsKey(objId) ? Manager.ObjectMgr.GameObjects[objId] : null;

            if (gameobject != null)
            {
                ChatMessageValues chatMessage = new ChatMessageValues(0, "");

                GameObjectSpawn spawn = new GameObjectSpawn()
                {
                    Guid = GameObjectSpawn.GetLastGuid() + 1,
                    Id = objId,
                    GameObject = gameobject,
                    Position = pChar.Position,
                    Map = pChar.Map
                };

                spawn.Position.O = pChar.Position.O;

                spawn.AddToWorld();

                chatMessage.Message = $"GameobjectSpawn (Guid: {spawn.Guid}) successfully added.";

                ChatHandler.SendMessage(ref session, chatMessage);
            }

            Manager.WorldMgr.waitAdd = false;
        }

        [ChatCommand("delobj")]
        public static void DelObj(string[] args, WorldClass session)
        {
            Manager.WorldMgr.waitAdd = true;
            ChatMessageValues chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, "");

            var objId = Command.Read<long>(args, 1);
            var pChar = session.Character;
            var spawn = Manager.SpawnMgr.FindSpawn(objId);

            if (spawn != null)
            {
                Manager.SpawnMgr.RemoveSpawn(spawn);

                pChar.InRangeObjects.Remove(spawn.Guid);

                PacketWriter updateObject = new PacketWriter(ServerMessage.ObjectUpdate);
                BitPack bp = new BitPack(updateObject);

                updateObject.WriteUInt32(0);

                updateObject.WriteUInt16((ushort)pChar.Map);

                bp.Write(true);
                bp.Flush();

                updateObject.WriteUInt16((ushort)pChar.Map);
                updateObject.WriteUInt32((uint)1);

                updateObject.WriteSmartGuid(spawn.SGuid);
                updateObject.WriteInt32(0);

                session.Send(ref updateObject);

                chatMessage.Message = "Selected Spawn successfully removed.";

                ChatHandler.SendMessage(ref session, chatMessage);
            }
            else
            {
                chatMessage.Message = "Not a gameobject.";

                ChatHandler.SendMessage(ref session, chatMessage);
            }

            Manager.WorldMgr.waitAdd = false;
        }

        [ChatCommand("delnpc")]
        public static void DeleteNpc(string[] args, WorldClass session)
        {
            Manager.WorldMgr.waitAdd = true;
            ChatMessageValues chatMessage = new ChatMessageValues(MessageType.ChatMessageSystem, "");

            var pChar = session.Character;
            var spawn = Manager.SpawnMgr.FindSpawn(session.TargetGuid);

            if (spawn != null)
            {
                Manager.SpawnMgr.RemoveSpawn(spawn, true);

                PacketWriter updateObject = new PacketWriter(ServerMessage.ObjectUpdate);
                BitPack bp = new BitPack(updateObject);

                updateObject.WriteUInt32(0);
                updateObject.WriteUInt16((ushort)pChar.Map);

                bp.Write(true);
                bp.Flush();

                updateObject.WriteUInt16((ushort)pChar.Map);
                updateObject.WriteUInt32((uint)1);

                updateObject.WriteSmartGuid(spawn.SGuid);

                pChar.InRangeObjects.Remove(spawn.Guid);

                Manager.SpawnMgr.RemoveSpawn(spawn, true);

                updateObject.WriteInt32(0);

                session.Send(ref updateObject);

                chatMessage.Message = "Selected Spawn successfully removed.";

                ChatHandler.SendMessage(ref session, chatMessage);
            }
            else
            {
                chatMessage.Message = "Not a creature.";

                ChatHandler.SendMessage(ref session, chatMessage);
            }

            Manager.WorldMgr.waitAdd = false;
        }


        [ChatCommand("hotfix")]
        public static void Hotfix(string[] args, WorldClass session)
        {
            HotfixManager.GetInstance().SendHotfixMessage(session);
        }
    }
}
