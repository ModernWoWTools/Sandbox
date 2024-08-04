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

using Framework.Constants;
using Framework.Network.Packets;
using Framework.ObjectDefines;
using AuthServer.Network;
using AuthServer.WorldServer.Managers;
using Framework.Constants.Net;
using AuthServer.Game.Chat;
using Framework.Logging;
using Framework.Constants.Misc;

namespace AuthServer.Game.Packets.PacketHandler
{
    public class ChatHandler : Manager
    {
        [Opcode(ClientMessage.ChatMessageSay, "17930")]
        public static void HandleChatMessageSay(ref PacketReader packet, WorldClass session)
        {
            BitUnpack BitUnpack = new BitUnpack(packet);

            var language = packet.Read<int>();

            var messageLength = BitUnpack.GetBits<uint>(10) >> 1;
            var message = packet.ReadString(messageLength);

            ChatMessageValues chatMessage = new ChatMessageValues(MessageType.ChatMessageSay, message, true, true, session.Character.Name);
            chatMessage.Language = (byte)language;

            Log.Message(LogType.Debug, $"Chat: {message}");

            if (ChatCommandParser.CheckForCommand(message))
            {
                ChatCommandParser.ExecuteChatHandler(message, session);
                ChatCommandParser.ExecuteChatHandler2(message, WorldMgr.GetSession2(session.Character.Guid));
            }
            else
                SendMessage(ref session, chatMessage);
        }

        [Opcode(ClientMessage.ChatMessageYell, "17930")]
        public static void HandleChatMessageYell(ref PacketReader packet, WorldClass session)
        {
            BitUnpack BitUnpack = new BitUnpack(packet);

            var language = packet.Read<int>();

            var messageLength = packet.ReadByte();
            var message = packet.ReadString(messageLength);

            ChatMessageValues chatMessage = new ChatMessageValues(MessageType.ChatMessageYell, message, true, true, session.Character.Name);
            chatMessage.Language = (byte)language;

            SendMessage(ref session, chatMessage);
        }

        [Opcode(ClientMessage.ChatMessageWhisper, "17930")]
        public static void HandleChatMessageWhisper(ref PacketReader packet, WorldClass session)
        {
            BitUnpack BitUnpack = new BitUnpack(packet);

            var language = packet.Read<int>();

            var messageLength = BitUnpack.GetBits<byte>(8);
            var nameLength = BitUnpack.GetBits<byte>(9);

            string receiverName = packet.ReadString(nameLength);
            string message = packet.ReadString(messageLength);

            WorldClass rSession = WorldMgr.GetSession(receiverName);

            if (rSession == null)
                return;

            ChatMessageValues chatMessage = new ChatMessageValues(MessageType.ChatMessageWhisperInform, message, false, true, session.Character.Name);
            SendMessage(ref session, chatMessage, rSession);

            chatMessage = new ChatMessageValues(MessageType.ChatMessageWhisper, message, false, true, session.Character.Name);
            SendMessage(ref rSession, chatMessage, session);
        }

        public static void SendMessage(ref WorldClass session, ChatMessageValues chatMessage, WorldClass pSession = null)
        {
            var pChar = session.Character;
            var guid = pChar.Guid;

            if (pSession != null)
                guid = pSession.Character.Guid;

            PacketWriter chat = new PacketWriter(ServerMessage.Chat);
            BitPack BitPack = new BitPack(chat);

            chat.WriteUInt8((byte)chatMessage.ChatType);
            chat.WriteUInt32(chatMessage.Language);

            if (chatMessage.ChatType == MessageType.ChatMessageRaidWarning ||
                chatMessage.ChatType == MessageType.ChatMessageSystem)
            {
                chat.WriteSmartGuid(0);
                chat.WriteSmartGuid(0);
                chat.WriteSmartGuid(0);
                chat.WriteSmartGuid(0);
            }
            else
            {
                chat.WriteSmartGuid(guid);
                chat.WriteSmartGuid(0);
                chat.WriteSmartGuid(guid);
                chat.WriteSmartGuid(0);
            }

            chat.WriteUInt32(0);
            chat.WriteUInt32(0);
            chat.WriteSmartGuid(0);
            chat.WriteUInt32(0);
            chat.WriteFloat(chatMessage.Time);

            BitPack.WriteStringLength(chatMessage.From, 11);
            BitPack.Write(0, 11);
            BitPack.Write(0, 5);
            BitPack.Write(0, 7);
            BitPack.WriteStringLength(chatMessage.Message, 12);
            BitPack.Write(0, 11);
            BitPack.Write(false);
            BitPack.Write(false);

            BitPack.Flush();

            chat.WriteString(chatMessage.From, false);
            chat.WriteString(chatMessage.Message, false);


            switch (chatMessage.ChatType)
            {
                case MessageType.ChatMessageSay:
                    WorldMgr.SendByDist(pChar, chat, 625);
                    break;

                case MessageType.ChatMessageYell:
                    WorldMgr.SendByDist(pChar, chat, 90000);
                    break;
                default:
                    session.Send(ref chat);
                    break;
            }
        }
    }
}
