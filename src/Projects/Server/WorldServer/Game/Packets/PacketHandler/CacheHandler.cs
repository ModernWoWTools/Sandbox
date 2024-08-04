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

using System.IO;

using AuthServer.Game.Entities;
using AuthServer.Network;
using AuthServer.WorldServer.Managers;

using Framework.Constants.Misc;
using Framework.Constants.Net;
using Framework.Logging;
using Framework.Network.Packets;

namespace WorldServer.Game.Packets.PacketHandler
{
    public class CacheHandler : Manager
    {
        [Opcode2(ClientMessage.CliQueryCreature, "17930")]
        public static void HandleQueryCreature(ref PacketReader packet, WorldClass2 session)
        {
            var id = packet.Read<int>();

#if !PUBLIC
            if (ReplayManager.GetInstance().Playing && ReplayManager.GetInstance().CreatureCache.TryGetValue(id, out var p))
            {
                return;
            }
#endif
            PacketWriter queryCreatureResponse = new PacketWriter(ServerMessage.QueryCreatureResponse);
            BitPack BitPack = new BitPack(queryCreatureResponse);

            var creature = ObjectMgr.Creatures.ContainsKey(id) ? ObjectMgr.Creatures[id] : null;
            var hasData = (creature != null);

            queryCreatureResponse.WriteInt32(id);

            BitPack.Write(hasData);
            BitPack.Flush();
            if (hasData)
            {
                var stats = creature;

                BitPack.Write(stats.SubName.Length != 0 ? stats.SubName.Length + 1 : 0, 11);
                BitPack.Write(stats.Title.Length, 11);
                BitPack.Write(stats.IconName.Length + 1, 6);
                BitPack.Write(stats.RacialLeader);

                for (int i = 0; i < 8; i++)
                {
                    if (i == 0)
                        BitPack.Write(stats.Name.Length + 1, 11);
                    else
                        BitPack.Write(0, 11);
                }

                BitPack.Flush();

                queryCreatureResponse.WriteCString(stats.Name);

                foreach (var v in stats.Flag)
                    queryCreatureResponse.WriteUInt32(v);

                queryCreatureResponse.WriteInt32(stats.Type);
                queryCreatureResponse.WriteInt32(stats.Family);
                queryCreatureResponse.WriteInt32(stats.Rank);
                queryCreatureResponse.WriteInt32(stats.QuestItemId[0]); // proxyCreatureId1
                queryCreatureResponse.WriteInt32(stats.QuestItemId[1]); // proxyCreatureId2

                queryCreatureResponse.WriteInt32(stats.DisplayInfoId.Count);
                queryCreatureResponse.WriteFloat(stats.Unknown); // unk

                foreach (var di in stats.DisplayInfoId)
                {
                    queryCreatureResponse.WriteInt32(di.displayId);
                    queryCreatureResponse.WriteFloat(di.Item2);
                    queryCreatureResponse.WriteFloat(di.Item3);
                }

                queryCreatureResponse.WriteFloat(stats.HealthModifier);
                queryCreatureResponse.WriteFloat(stats.PowerModifier);
                queryCreatureResponse.WriteInt32(0); // QuestItemsCount
                queryCreatureResponse.WriteInt32(stats.MovementInfoId);
                queryCreatureResponse.WriteInt32(0);
                queryCreatureResponse.WriteInt32(stats.ExpansionRequired);
                queryCreatureResponse.WriteInt32(stats.Unknown3);
                queryCreatureResponse.WriteInt32(stats.Unknown4);
                queryCreatureResponse.WriteInt32(0);
                queryCreatureResponse.WriteInt32(0);
                queryCreatureResponse.WriteInt32(0);

                if (stats.SubName != "")
                    queryCreatureResponse.WriteCString(stats.SubName);

                if (stats.Title != "")
                    queryCreatureResponse.WriteCString(stats.Title);

                if (stats.IconName != "")
                    queryCreatureResponse.WriteCString(stats.IconName);

            }
            else
                Log.Message(LogType.Debug, "Creature (Id: {0}) not found.", id);

            session.Send(ref queryCreatureResponse);
        }

        [Opcode2(ClientMessage.CliQueryGameObject, "17930")]
        public static void HandleQueryGameObject(ref PacketReader packet, WorldClass2 session)
        {
            var id = packet.Read<int>();
            var guid = packet.ReadGuid();
#if !PUBLIC
            if (ReplayManager.GetInstance().Playing && ReplayManager.GetInstance().GameobjectCache.TryGetValue(id, out var p))
            {
                return;
            }
#endif
            PacketWriter queryGameObjectResponse = new PacketWriter(ServerMessage.QueryGameObjectResponse);
            BitPack BitPack = new BitPack(queryGameObjectResponse);

            var gObject = ObjectMgr.GameObjects.ContainsKey(id) ? ObjectMgr.GameObjects[id] : null;
            var hasData = (gObject != null);

            queryGameObjectResponse.WriteInt32(id);
            queryGameObjectResponse.WriteSmartGuid(guid);

            BitPack.Write(hasData);
            BitPack.Flush();

            var statData = new PacketWriter();

            if (hasData)
            {
                var stats = gObject;

                statData.WriteInt32((int)stats.Type);
                statData.WriteInt32(stats.DisplayInfoId);

                statData.WriteCString(stats.Name);

                for (int i = 0; i < 3; i++)
                    statData.WriteCString("");

                statData.WriteCString(stats.IconName);
                statData.WriteCString(stats.CastBarCaption);
                statData.WriteCString(stats.Unk);

                foreach (var v in stats.Data)
                    statData.WriteInt32(v);

                statData.WriteFloat(stats.Size);
                statData.WriteUInt8((byte)stats.QuestItemId.Count);

                foreach (var v in stats.QuestItemId)
                    statData.WriteInt32(v);

                statData.WriteInt32(stats.ExpansionRequired);
            }
            else
                Log.Message(LogType.Debug, "Gameobject (Id: {0}) not found.", id);

            var sdata = (statData.BaseStream as MemoryStream).ToArray();

            queryGameObjectResponse.WriteInt32(sdata.Length);
            queryGameObjectResponse.Write(sdata);


            session.Send(ref queryGameObjectResponse);
        }

        [Opcode(ClientMessage.QueryPlayerName, "17930")]
        public static void HandleQueryPlayerName(ref PacketReader packet, WorldClass session)
        {
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guid = packet.ReadSmartGuid();

#if !PUBLIC
            if (ReplayManager.GetInstance().Playing)
            {
                foreach (var p in ReplayManager.GetInstance().QueryPlayerNamePackets)
                {
                }
                return;
            }
#endif

            for (int i = 0; i < WorldMgr.CharaterList.Count; i++)
            {
                if (WorldMgr.CharaterList[i].Guid == guid)
                {
                    session.Character = WorldMgr.CharaterList[i];
                    break;
                }
            }
            var pSession = WorldMgr.GetSession(guid);

            // if (pSession != null)
            {
                var pChar = session.Character;

                if (pChar != null)
                {
                    PacketWriter queryPlayerNameResponse = new PacketWriter(ServerMessage.QueryPlayerNameResponse);
                    BitPack BitPack = new BitPack(queryPlayerNameResponse);

                    queryPlayerNameResponse.WriteUInt8(0); // result
                    queryPlayerNameResponse.WriteSmartGuid(guid);


                    BitPack.Write(0);
                    BitPack.Write(pChar.Name.Length, 6); // 48

                    for (int i = 0; i < 5; i++)
                        BitPack.Write(0, 7);

                    BitPack.Flush();

                    //for (int i = 0; i < 5; i++)
                    //    queryPlayerNameResponse.WriteString("");

                    queryPlayerNameResponse.WriteSmartGuid(guid);
                    queryPlayerNameResponse.WriteSmartGuid(guid); // 28
                    queryPlayerNameResponse.WriteSmartGuid(guid); // 28
                    queryPlayerNameResponse.WriteUInt64(0);// new
                    queryPlayerNameResponse.WriteUInt32(1);

                    queryPlayerNameResponse.WriteUInt8(pChar.Race); // 105, 113
                    queryPlayerNameResponse.WriteUInt8(pChar.Gender); // 104, 112
                    queryPlayerNameResponse.WriteUInt8(pChar.Class); // 106, 114
                    queryPlayerNameResponse.WriteUInt8(pChar.Level); // 107, 115
                    queryPlayerNameResponse.WriteUInt8(0); // 107, 115
                    //BitPack.Flush();
                    queryPlayerNameResponse.WriteString(pChar.Name);

                    session.Send(ref queryPlayerNameResponse);
                }
            }
        }

        [Opcode(ClientMessage.QueryRealmName, "17930")]
        public static void HandleQueryRealmName(ref PacketReader packet, WorldClass session)
        {
            Character pChar = session.Character;

            uint realmId = packet.Read<uint>();

#if !PUBLIC
            if (ReplayManager.GetInstance().Playing)
            {
                // fallback to "Arctium" for local/own realm
            }
#endif
            string realmName = "Arctium";

            PacketWriter realmQueryResponse = new PacketWriter(ServerMessage.RealmQueryResponse);
            BitPack BitPack = new BitPack(realmQueryResponse);

            realmQueryResponse.WriteUInt32(realmId);
            realmQueryResponse.WriteUInt8(0);

            BitPack.Write(1);
            BitPack.Write(0);
            BitPack.Write(realmName.Length, 9);
            BitPack.Write(realmName.Length, 9);

            BitPack.Flush();

            realmQueryResponse.WriteString(realmName);
            realmQueryResponse.WriteString(realmName);

            session.Send(ref realmQueryResponse);
        }
    }
}
