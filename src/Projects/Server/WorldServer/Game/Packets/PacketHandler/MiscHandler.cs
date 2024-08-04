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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AuthServer.Game.PacketHandler;
using AuthServer.Network;
using AuthServer.WorldServer.Managers;
using Framework.Constants;
using Framework.Constants.Misc;
using Framework.Constants.Net;
using Framework.Logging;
using Framework.Network.Packets;

namespace AuthServer.Game.Packets.PacketHandler
{
    public class MiscHandler : Manager
    {
        public static void HandleMessageOfTheDay(ref WorldClass session)
        {
            PacketWriter motd = new PacketWriter(ServerMessage.MOTD);
            BitPack BitPack = new BitPack(motd);

            List<string> motds = new List<string>();

            motds.Add("Hey!");
            motds.Add("For support visit https://arctium.io");
            motds.Add("INFO: worgen morph ids: 37915/37914 (use a HUMAN not a worgen)");
            motds.Add("Have fun with the Sandbox ;)");

            BitPack.Write<uint>(4, 4);
            BitPack.Flush();
            motds.ForEach(m =>
            {
                BitPack.Write(Encoding.UTF8.GetBytes(m).Length, 7);
                BitPack.Flush();
                //BitPack.Flush();

                motd.WriteString(m);
            });

            //BitPack.Flush();

            //motds.ForEach(m => motd.WriteString(m));

#if PUBLIC
            session.Send(ref motd);
#endif
        }

        [Opcode(ClientMessage.Ping, "17930")]
        public static void HandlePong(ref PacketReader packet, WorldClass session)
        {
            uint sequence = packet.Read<uint>();
            uint latency = packet.Read<uint>();

            PacketWriter pong = new PacketWriter(ServerMessage.Pong);

            pong.WriteUInt32(sequence);

            //session.Send(ref pong);
        }

        //[Opcode(ClientMessage.CalendarNum, "17930")]
        public static void HandleCalendarNum(ref PacketReader packet, WorldClass session)
        {
            PacketWriter pong = new PacketWriter(ServerMessage.CalendarNum);

            pong.WriteUInt32(1);

            session.Send(ref pong);
        }

        [Opcode(ClientMessage.CommentatorEnable, "")]
        public static void HandleCommentatorEnable(ref PacketReader packet, WorldClass session)
        {
            var mode = packet.ReadUInt32();

            ObjectHandler.Update<uint>(PlayerFields.PlayerFlags, mode == 1 ? 0x00400000 | 0x00080000 : 0u, session.Character);
        }

        // [Opcode2(ClientMessage.CalendarNum, "17930")]
        public static void HandleCalendarNum2(ref PacketReader packet, WorldClass2 session)
        {
            PacketWriter pong = new PacketWriter(ServerMessage.CalendarNum);

            pong.WriteUInt32(1);

            session.Send(ref pong);
        }

        [Opcode(ClientMessage.LogDisconnect, "17930")]
        public static void HandleDisconnectReason(ref PacketReader packet, WorldClass session)
        {
            var pChar = session.Character;
            var disconnectReason = packet.Read<uint>();

            if (pChar != null)
                WorldMgr.DeleteSession(pChar.Guid);

            CharacterHandler.chatRunning = false;

            Log.Message(LogType.Debug, "Account with Id {0} disconnected. Reason: {1}", session.Account != null ? session.Account.Id : -1, disconnectReason);

            Sandbox.AuthSession?.Dispose();
            session.Dispose();
        }

        [Opcode2(ClientMessage.LogDisconnect, "17930")]
        public static void HandleDisconnectReason(ref PacketReader packet, WorldClass2 session)
        {
            var pChar = session.Character;
            var disconnectReason = packet.Read<uint>();

            if (pChar != null)
                WorldMgr.DeleteSession(pChar.Guid);

            CharacterHandler.chatRunning = false;

            //Log.Message(LogType.Debug, "Account with Id {0} disconnected. Reason: {1}", session.Account != null ? session.Account.Id : -1, disconnectReason);

            session.Dispose();
        }

        public static void HandleCacheVersion(ref WorldClass session)
        {
            PacketWriter cacheVersion = new PacketWriter(ServerMessage.CacheVersion);

            cacheVersion.WriteUInt32(0);

            session.Send(ref cacheVersion);
        }

        [Opcode(ClientMessage.LoadingScreenNotify, "17930")]
        public static void HandleLoadingScreenNotify(ref PacketReader packet, WorldClass session)
        {
            //BitUnpack BitUnpack = new BitUnpack(packet);

            //uint mapId = packet.Read<uint>();
            //bool loadingScreenState = BitUnpack.GetBit();

            //Log.Message(LogType.Debug, "Loading screen for map '{0}' is {1}.", mapId, loadingScreenState ? "enabled" : "disabled");

            /* AuthenticationHandler.HandleConnectTo(session, 3724, 1, null);

             var suspendComs = new PacketWriter(ServerMessage.SuspendComms);

             suspendComs.WriteUInt32(0x14);

             session.Send(ref suspendComs);*/
            // Redirect the connection to the worldserver
            // AuthenticationHandler.HandleConnectTo(session, 8101, 0, null);

            //             var suspendComs = new PacketWriter(ServerMessage.SuspendComms);
            // 
            //             suspendComs.WriteUInt32(0x14);
            // 
            //             session.Send(ref suspendComs);
        }

        [Opcode(ClientMessage.ViolenceLevel, "17930")]
        public static void HandleViolenceLevel(ref PacketReader packet, WorldClass session)
        {
            byte violenceLevel = packet.Read<byte>();

            Log.Message(LogType.Debug, "Violence level from account '{0} (Id: {1})' is {2}.", session.Account.Name, session.Account.Id, (ViolenceLevel)violenceLevel);
        }

        [Opcode(ClientMessage.ActivePlayer, "17930")]
        public static void HandleActivePlayer(ref PacketReader packet, WorldClass session)
        {
            byte active = packet.Read<byte>();    // Always 0

            Log.Message(LogType.Debug, "Player {0} (Guid: {1}) is active.", session.Character.Name, session.Character.Guid);
        }

        [Opcode2(ClientMessage.CliSetSelection, "17930")]
        public static void HandleSetSelection(ref PacketReader packet, WorldClass2 session)
        {
            var fullGuid = packet.ReadGuid();

            var sess = WorldMgr.GetSession(session.Character.Guid);
            if (session.Character != null)
            {

                if (sess != null)
                    sess.TargetGuid = fullGuid.Guid;

                if (fullGuid.Guid == 0)
                    Log.Message(LogType.Debug, "Character (Guid: {0}) removed current selection.", session.Character.Guid);
                else
                {
                    Log.Message(LogType.Debug, "Character (Guid: {0}) selected 'Guid: {1}'.", session.Character.Guid, fullGuid.Guid);

                    //GarrisonHandler.HandleGetGarrisonInfoResult(ref session);
                    //GarrisonHandler.HandleGarrisonAddMissionResult(ref session);
                    //GarrisonHandler.HandleGarrisonArchitectShow(ref session);
                    //GarrisonHandler.HandleGarrisonAddMissionResult(ref session);
                }
            }

            /*var clearTarget = new PacketWriter(ServerMessage.ClearTarget);

            clearTarget.WriteSmartGuid(fullGuid);

            sess.Send(ref clearTarget);
*/
        }

        [Opcode(ClientMessage.SetActionButton, "17930")]
        public static void HandleSetActionButton(ref PacketReader packet, WorldClass session)
        {
            var pChar = session.Character;

            var actionId = packet.ReadUInt64();
            var slotId = packet.Read<byte>();

            if (actionId == 0)
            {
                var action = pChar.ActionButtons.Where(button => button.SlotId == slotId && button.SpecGroup == pChar.ActiveSpecGroup).Select(button => button).First();

                ActionMgr.RemoveActionButton(pChar, action, true);
                Log.Message(LogType.Debug, "Character (Guid: {0}) removed action button {1} from slot {2}.", pChar.Guid, actionId, slotId);

                return;
            }

            var newAction = new ActionButton
            {
                Action = actionId,
                SlotId = slotId,
                SpecGroup = pChar.ActiveSpecGroup
            };

            ActionMgr.AddActionButton(pChar, newAction, true);
            Log.Message(LogType.Debug, "Character (Guid: {0}) added action button {1} to slot {2}.", pChar.Guid, actionId, slotId);
        }

        public static void HandleUpdateActionButtons(ref WorldClass session)
        {
            var pChar = session.Character;

            PacketWriter updateActionButtons = new PacketWriter(ServerMessage.UpdateActionButtons);

            const int buttonCount = 132;
            var buttons = new byte[buttonCount][];
            var actions = ActionMgr.GetActionButtons(pChar, pChar.ActiveSpecGroup);

            for (int i = 0; i < buttonCount; i++)
                if (actions.Any(action => action.SlotId == i))
                    buttons[i] = BitConverter.GetBytes((ulong)actions.Where(action => action.SlotId == i).Select(action => action.Action).First());
                else
                    buttons[i] = new byte[8];

            for (int j = 0; j < buttonCount; j++)
            {
                updateActionButtons.WriteBytes(buttons[j]);
            }

            // 0 - Initial packet on Login (no verification) / 1 - Verify spells on switch (Spec change) / 2 - Clear Action Buttons (Spec change)
            updateActionButtons.WriteInt8(0);

            session.Send(ref updateActionButtons);
        }

        [Opcode(ClientMessage.DBQueryBulk, "34003")]
        public static void HandleDBQueryBulk(ref PacketReader packet, WorldClass session)
        {
            if (ReplayManager.GetInstance().Playing)
            {
                var bitUnpack = new BitUnpack(packet);
                var tableHash = packet.ReadUInt32();

                Log.Message(LogType.Info, $"Got hotfix request for table {tableHash:X}");

                var count = bitUnpack.GetBits<int>(13);

                for (var i = 0; i < count; i++)
                {
                    var id = packet.ReadUInt32();

#if DEBUG
                    
#endif
                }
            }

            //Hotfix.SendHotfixMessage(session);
        }

        //[Opcode(ClientMessage.HotfixRequest, "34003")]
        public static void HandleHotfixRequest(ref PacketReader packet, WorldClass session)
        {
            Log.Message(LogType.Info, $"Got hotfix request...");

            var currentBuild = packet.ReadUInt32();
            var internalBuild = packet.ReadUInt32();
            var hotfixCount = packet.ReadUInt32();

            for (var i = 0; i < hotfixCount; i++)
            {
                var tableHash = packet.ReadUInt32();
                var recordId = packet.ReadUInt32();
                var hotfixId = packet.ReadUInt32();

                //Hotfix.SendHotfixReply(session, tableHash, recordId);
                Hotfix.SendHotfixMessage(session);
            }
        }
    }
}
