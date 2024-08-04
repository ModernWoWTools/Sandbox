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

using System.Collections.Generic;
using Framework.Network.Packets;
using AuthServer.Game.WorldEntities;
using AuthServer.Network;
using AuthServer.WorldServer.Managers;
using AuthServer.Game.Entities;
using Framework.Constants.Net;
using System;
using Framework.Constants;
using Framework.ObjectDefines;
using System.Threading.Tasks;
using System.Linq;
using Framework.Misc;
using AuthServer.Game.PacketHandler;
using AuthServer.WorldServer.Game.Entities;
using System.Collections;
using System.IO;

namespace AuthServer.Game.Packets.PacketHandler
{
    public class SpellHandler : Manager
    {
        public static void HandleSendKnownSpells(ref WorldClass session)
        {
            Character pChar = session.Character;

            // hax for spec swap
            pChar.SpellList.RemoveAll(p => p.SpellId == 200749);
            pChar.SpellList.Add(new PlayerSpell

            {
                SpellId = 200749,
                State = PlayerSpellState.Unchanged,
            });

            // No duplicates here.
            pChar.SpellList.RemoveAll(p => p.SpellId == 185394);
            pChar.SpellList.Add(new PlayerSpell { SpellId = 185394, State = PlayerSpellState.Unchanged });

            int count = pChar.SpellList.Count;


            PacketWriter writer = new PacketWriter(ServerMessage.SendKnownSpells);
            BitPack BitPack = new BitPack(writer);

            // hax for monks
            if (session.Character.Race == 24)
            {
                writer.Write("003E00000000000000C4000000C6000000CC000000510000000A0200006B000000E3000000C7000000CB000000C50000009C0400004E090000250D0000EA0B00005918000066180000671800004D1900004E190000CB190000621C0000631C0000BB1C0000C2200000A523000093540000945400000B5600001A59000067B30000FDEF00002E0B0100DC77010099BF010063C10100F0290100082A0100EE0402009B770200BE3903002122000075230000762300009C230000E63C000047A2010044A2010042A2010041A2010040A20100750202005FA60100B7E301004E170200854F0300827303006A8603002D100300501702008B7B020024DF02001DE80200".ToByteArray());
            }
            else
            {
                BitPack.Write(0);
                BitPack.Flush();

                // Remove Divine Steed mount spells.
                foreach (var kp in WorldMgr.MountSpells.ToList())
                    if (kp.Value.Item2 == 221883 || kp.Value.Item2 == 221886 || kp.Value.Item2 == 221887 || kp.Value.Item2 == 221885 ||
                        kp.Value.Item2 == 254471 || kp.Value.Item2 == 254472 || kp.Value.Item2 == 254473 || kp.Value.Item2 == 254474)
                        WorldMgr.MountSpells.Remove(kp.Key);

                writer.WriteInt32(count + WorldMgr.MountSpells.Count);
                writer.WriteInt32(0);

                // player spells
                pChar.SpellList.ForEach(spell => writer.WriteUInt32(spell.SpellId));

                // Learn mount spells
                foreach (var kp in WorldMgr.MountSpells)

                    writer.WriteUInt32(kp.Value.Item2);
            }

            session.Send(ref writer);
        }

        [Opcode2(ClientMessage.CancelAura, "17930")]
        public static void HandleCancelAura(ref PacketReader packet, WorldClass2 session)
        {
            var spellId = packet.Read<int>();
            byte slot = 1;
            
            var mountSpell = false;
            var mountId = 0u;

            foreach (var kp in WorldMgr.MountSpells)
            {
                if (kp.Value.Item2 == spellId)
                {
                    mountId = kp.Value.Item1;
                    mountSpell = true;
                    break;
                }
            }

            if (mountSpell)
            {
                ObjectHandler.Update(UnitFields.MountDisplayID, 0, session.Character);

                MoveHandler.HandleMoveSetRunSpeed(ref session, 7.0f);

                slot = 5;
            }

            AuraUpdate(WorldMgr.GetSession(session.Character.Guid), 0, new SmartGuid(1, (int)0, GuidType.Cast), slot);
        }

        public static int activeSpell = 0;

        [Opcode2(ClientMessage.CastSpell, "17930")]
        public static void HandleCastSpell(ref PacketReader packet, WorldClass2 session)
        {
            var sess = WorldMgr.GetSession(session.Character.Guid);

            var castId = packet.ReadGuid();
            var specID = packet.Read<uint>();
            var blubb2 = packet.Read<int>();
            var spellId = packet.Read<int>();

            // hax for spec switch
            if (spellId == 200749 && specID >= 62 && specID <= 581)
            {
                session.Character.PrimarySpec = specID;
                sess.Character.PrimarySpec = specID;

                CharacterHandler.HandleUpdateTalentData(ref sess);
            }

            CastSpell(sess, spellId, castId);

            var mountSpell = false;
            var mountId = 0u;

            foreach (var kp in WorldMgr.MountSpells)
            {
                if (kp.Value.Item2 == spellId)
                {
                    mountId = kp.Value.Item1;
                    mountSpell = true;
                    break;
                }
            }

            if (mountSpell)
            {
                if (activeSpell != 0)
                {

                    MoveHandler.HandleMoveSetRunSpeed(ref session, 7.0f);

                    activeSpell = 0;
                }
                else
                {
                    activeSpell = spellId;

                    MoveHandler.HandleMoveSetRunSpeed(ref session, 14.0f);
                }
            }
        }

        public static void AuraUpdate(WorldClass session, uint spellId, SmartGuid castId, byte slot = 5)
        {
            var auraUpdate = new PacketWriter(ServerMessage.AuraUpdate);
            var bitPack = new BitPack(auraUpdate);

            bitPack.Write(false);
            bitPack.Write(1, 9);
            bitPack.Flush();

            auraUpdate.WriteUInt8(slot);

            bitPack.Write(spellId != 0); // hasAura
            bitPack.Flush();

            if (spellId != 0)
            {
                auraUpdate.WriteSmartGuid(castId);
                auraUpdate.WriteUInt32(spellId);

                WorldMgr.SpellXVisualIds.TryGetValue(spellId, out var visualId);

                auraUpdate.WriteUInt32(visualId);
                auraUpdate.WriteUInt32(0);
                auraUpdate.WriteUInt16(0x1 | 0x2 | 0x8); // flags
                auraUpdate.WriteUInt32(1); // activeflags
                auraUpdate.WriteUInt16(0); // level
                auraUpdate.WriteUInt8(0);
                auraUpdate.WriteUInt32(0);

                bitPack.Write(false);
                bitPack.Write(false);
                bitPack.Write(false);
                bitPack.Write(false);
                bitPack.Write(0, 6); // points
                bitPack.Write(0, 6);
                bitPack.Write(false);
                bitPack.Flush();
            }

            auraUpdate.WriteSmartGuid(session.Character.Guid);

            session.Send(ref auraUpdate);
        }

        public static async void CastSpell(WorldClass session, int spellId, SmartGuid castId)
        {
            var mountSpell = false;
            var mountId = 0u;
            var castTime = 0u;

            foreach (var kp in WorldMgr.MountSpells)
            {
                if (kp.Value.Item2 == spellId)
                {
                    mountId = kp.Value.Item1;
                    mountSpell = true;
                    castTime = 1500;
                    break;
                }
            }

            var writer = new PacketWriter(ServerMessage.SpellStart);
            var BitPack = new BitPack(writer);
            castTime = spellId == 286031 ? 10000 : castTime;

            WriteCastData(ref writer, session, ref BitPack, spellId, castId, castTime);

            session.Send(ref writer);

            if (mountSpell)
            {
                await Task.Delay((int)castTime);

                AuraUpdate(session, (uint)spellId, new SmartGuid(1, (int)spellId, GuidType.Cast));
            }
            else
                AuraUpdate(session, (uint)spellId, castId, 1);

            // Add mount aura
            writer = new PacketWriter(ServerMessage.SpellGo);
            BitPack = new BitPack(writer);

            WriteCastData(ref writer, session, ref BitPack, spellId, castId);

            writer.WriteUInt8(0);
            session.Send(ref writer);

            // Update mount display.
            if (mountSpell)
                ObjectHandler.Update(UnitFields.MountDisplayID, WorldMgr.MountDisplays[mountId], session.Character);
        }

        public static void WriteCastData(ref PacketWriter writer, WorldClass session, ref BitPack BitPack, int spellId, SmartGuid castId, uint castTime = 1500)
        {
            writer.WriteSmartGuid(session.Character.Guid);
            writer.WriteSmartGuid(session.Character.Guid);
            writer.WriteSmartGuid(castId);
            writer.WriteSmartGuid(castId);
            writer.WriteInt32(spellId);

            if (spellId == 185394)
                writer.WriteUInt32(83766); // spellxvisual
            else if (spellId == 264274)
                writer.WriteUInt32(264274);
            else
            {
                if (WorldMgr.SpellXVisualIds.TryGetValue((uint)spellId, out var visualXId))
                    writer.WriteUInt32(visualXId);
                else
                    writer.WriteUInt32(0);
            }

            if (spellId == 185394 || spellId == 264274)
                writer.WriteInt32(int.MaxValue); //  flags
            else
                writer.WriteUInt32(0);

            writer.WriteInt32(0);
            writer.WriteInt32(0);
            writer.WriteUInt32(castTime); // casttime

            // missile
            writer.WriteInt32(0);
            writer.WriteFloat(0);

            // ammo
            writer.WriteInt32(0);
            writer.WriteUInt8(0);

            writer.WriteUInt8(0);
            writer.WriteInt32(0);

            // immunities
            writer.WriteInt32(0);
            writer.WriteInt32(0);

            // predict
            writer.WriteInt32(0);
            writer.WriteUInt8(0);
            writer.WriteSmartGuid(0);

            //BitPack.Write(0, 22);
            BitPack.Write(0, 16);
            BitPack.Write(0, 16);
            BitPack.Write(0, 16);
            BitPack.Write(0, 9);

            BitPack.Write(false);
            BitPack.Write(0, 16);
            BitPack.Flush();

            BitPack.Write(0, 25);
            BitPack.Write(false);
            BitPack.Write(false);
            BitPack.Write(false);
            BitPack.Write(false);

            BitPack.Write(session.Character.Name.Length, 7);

            writer.WriteSmartGuid(session.TargetGuid == 0 ? session.Character.Guid : session.TargetGuid);
            writer.WriteSmartGuid(0);

            writer.WriteString(session.Character.Name);
        }

        public static void HandleLearnedSpells(ref WorldClass session, List<uint> newSpells)
        {
            PacketWriter writer = new PacketWriter(ServerMessage.LearnedSpells);
            BitPack BitPack = new BitPack(writer);

            writer.WriteInt32(newSpells.Count);
            writer.WriteInt32(newSpells.Count);

            for (int i = 0; i < newSpells.Count; i++)
                writer.WriteUInt32(newSpells[i]);

            for (int i = 0; i < newSpells.Count; i++)
                writer.WriteUInt32(newSpells[i]);

            BitPack.Write(0);
            BitPack.Flush();

            session.Send(ref writer);
        }

        public static void HandleUnlearnedSpells(ref WorldClass session, List<uint> oldSpells)
        {
            PacketWriter writer = new PacketWriter(ServerMessage.UnlearnedSpells);
            BitPack BitPack = new BitPack(writer);

            BitPack.Write<int>(oldSpells.Count, 22);

            BitPack.Flush();

            for (int i = 0; i < oldSpells.Count; i++)
                writer.WriteUInt32(oldSpells[i]);

            session.Send(ref writer);
        }
    }
}
