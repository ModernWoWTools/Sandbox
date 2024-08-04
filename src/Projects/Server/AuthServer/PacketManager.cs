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

using AuthServer.WorldServer.Managers;
using Framework.Constants.Misc;
using Framework.Constants.Net;
using Framework.Logging;
using Framework.Network.Packets;
using System;
using System.Collections.Generic;
using System.Reflection;
using AuthServer.Network;

namespace AuthServer.WorldServer.Game.Packets
{
    public class PacketManager : Manager
    {
        public static bool ReplayEnabled { get; set; }
        public static bool ReplayCharList { get; set; }
        public static bool StartReplayOnLogin { get; set; }

        static Dictionary<ClientMessage, HandlePacket> OpcodeHandlers = new Dictionary<ClientMessage, HandlePacket>();
        static Dictionary<ClientMessage, HandlePacket2> OpcodeHandlers2 = new Dictionary<ClientMessage, HandlePacket2>();

        delegate void HandlePacket(ref PacketReader packet, WorldClass session);
        delegate void HandlePacket2(ref PacketReader packet, WorldClass2 session);

        public static void DefineOpcodeHandler()
        {
            Assembly currentAsm = Assembly.GetExecutingAssembly();
            foreach (var type in currentAsm.GetTypes())
            {
                foreach (var methodInfo in type.GetMethods())
                {
                    foreach (var opcodeAttr in methodInfo.GetCustomAttributes<OpcodeAttribute>())
                        if (opcodeAttr != null)
                            OpcodeHandlers[opcodeAttr.Opcode] = (HandlePacket)Delegate.CreateDelegate(typeof(HandlePacket), methodInfo);
                }
            }
        }

        public static void DefineOpcodeHandler2()
        {
            Assembly currentAsm = Assembly.GetExecutingAssembly();
            foreach (var type in currentAsm.GetTypes())
            {
                foreach (var methodInfo in type.GetMethods())
                {
                    foreach (var opcodeAttr in methodInfo.GetCustomAttributes<Opcode2Attribute>())
                        if (opcodeAttr != null)
                            OpcodeHandlers2[opcodeAttr.Opcode] = (HandlePacket2)Delegate.CreateDelegate(typeof(HandlePacket2), methodInfo);
                }
            }
        }

        public static void InvokeHandler(ref PacketReader reader, WorldClass session)
        {
            if (ReplayEnabled)
            {
                Log.Message(LogType.Debug, "Opcode: {0} (0x{1:X}), Length: {2}", reader.Opcode, reader.Opcode, reader.Size);

                // Only handle defined packets while replaying a sniff.
                switch (reader.Opcode)
                {
                    case ClientMessage.EnumCharacters:
                    case ClientMessage.PlayerLogin:
                    case ClientMessage.ChatMessageSay:
                    case ClientMessage.QueryPlayerName:
                    case ClientMessage.QueryRealmName:
                    case ClientMessage.CliQueryGameObject:
                    case ClientMessage.CliQueryCreature:
                    case ClientMessage.DBQueryBulk:
                        break;
                    default:
                        return;
                }
            }

            if (session.Character != null)
            {
                ulong charGuid = session.Character.Guid;

                if (WorldMgr.Sessions.ContainsKey(charGuid))
                    WorldMgr.Sessions[charGuid] = session;
            }

            if (OpcodeHandlers.ContainsKey(reader.Opcode))
            {
                Log.Message(LogType.Packet, "Opcode: {0} (0x{1:X}), Length: {2}", reader.Opcode, reader.Opcode, reader.Size);
                OpcodeHandlers[reader.Opcode].Invoke(ref reader, session);
            }
            else
                Log.Message(LogType.Packet, "Unknown Opcode: {0} (0x{1:X}), Length: {2}", reader.Opcode, reader.Opcode, reader.Size);
        }

        public static void InvokeHandler(ref PacketReader reader, WorldClass2 session)
        {
            if (ReplayEnabled)
            {
                Log.Message(LogType.Debug, "Opcode2: {0} (0x{1:X}), Length: {2}", reader.Opcode, reader.Opcode, reader.Size);

                // Only handle defined packets while replaying a sniff.
                switch (reader.Opcode)
                {
                    case ClientMessage.AuthContinuedSession:
                    case ClientMessage.QueryPlayerName:
                    case ClientMessage.QueryRealmName:
                    case ClientMessage.CliQueryGameObject:
                    case ClientMessage.CliQueryCreature:
                    case ClientMessage.DBQueryBulk:
                        break;
                    default:
                        return;
                }
            }

            if (session.Character != null)
            {
                ulong charGuid = session.Character.Guid;

                if (WorldMgr.Sessions2.ContainsKey(0))
                    WorldMgr.Sessions2[0] = session;
            }

            if (OpcodeHandlers2.ContainsKey(reader.Opcode))
            {
                OpcodeHandlers2[reader.Opcode].Invoke(ref reader, session);
                Log.Message(LogType.Packet, "Opcode[2]: {0} (0x{1:X}), Length: {2}", reader.Opcode, reader.Opcode, reader.Size);
            }
            else
                Log.Message(LogType.Packet, "Unknown Opcode[2]: {0} (0x{1:X}), Length: {2}", reader.Opcode, reader.Opcode, reader.Size);
        }
    }
}
