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

using Framework.Network.Packets;
using AuthServer.Network;
using AuthServer.WorldServer.Managers;
using Framework.Constants.Net;
using Framework.Logging;

namespace AuthServer.Game.Packets.PacketHandler
{
    public class TimeHandler : Manager
    {
        public static void HandleReadyForAccountDataTimes(ref PacketReader packet, WorldClass session)
        {
            Log.Message(Framework.Constants.Misc.LogType.Error, "ReadyForAccountDataTimes!!!");
            //WorldMgr.WriteAccountDataTimes(AccountDataMasks.GlobalCacheMask, ref session);
        }
        //[Opcode2(ClientMessage.ReadyForAccountDataTimes, "18156")]
        public static void HandleReadyForAccountDataTimes(ref PacketReader packet, WorldClass2 session)
        {
            Log.Message(Framework.Constants.Misc.LogType.Error, "ReadyForAccountDataTimes!!!");
            //WorldMgr.WriteAccountDataTimes(AccountDataMasks.GlobalCacheMask, ref session);
        }
        [Opcode(ClientMessage.UITimeRequest, "18156")]
        public static void HandleUITimeRequest(ref PacketReader packet, WorldClass session)
        {
            PacketWriter uiTime = new PacketWriter(ServerMessage.UITime);

            uiTime.WriteUnixTime();

            session.Send(ref uiTime);
        }

        public static void HandleLoginSetTimeSpeed(ref WorldClass session)
        {
            PacketWriter loginSetTimeSpeed = new PacketWriter(ServerMessage.LoginSetTimeSpeed);

            loginSetTimeSpeed.WritePackedTime();
            loginSetTimeSpeed.WritePackedTime();
            loginSetTimeSpeed.WriteFloat(0.01666667f);
            loginSetTimeSpeed.WriteInt32(0);
            loginSetTimeSpeed.WriteInt32(0);

            session.Send(ref loginSetTimeSpeed);
        }

        public static void HandleSetTimezoneInformation(ref WorldClass session)
        {
            PacketWriter setTimezoneInformation = new PacketWriter(ServerMessage.SetTimeZoneInformation);
            BitPack BitPack = new BitPack(setTimezoneInformation);

            var timeZone = "Etc/UTC";

            BitPack.Write(timeZone.Length, 7);
            BitPack.Write(timeZone.Length, 7);

            BitPack.Flush();

            setTimezoneInformation.WriteString(timeZone);
            setTimezoneInformation.WriteString(timeZone);

            session.Send(ref setTimezoneInformation);
        }
    }
}
