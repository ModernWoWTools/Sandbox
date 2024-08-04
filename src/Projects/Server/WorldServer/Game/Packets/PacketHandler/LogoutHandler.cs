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
using System.Linq;

namespace AuthServer.Game.Packets.PacketHandler
{
    public class LogoutHandler : Manager
    {
        [Opcode2(ClientMessage.CliLogoutRequest, "17658")]
        public static void HandleLogoutRequest(ref PacketReader packet, WorldClass2 session)
        {
            var pChar = session.Character;

            WorldMgr.waitAdd = true;
           // pChar.InRangeObjects.Clear();

            ObjectMgr.SavePositionToDB(pChar);

            PacketWriter logoutComplete = new PacketWriter(ServerMessage.LogoutComplete);

            //logoutComplete.WriteSmartGuid(pChar.Guid);


           
            var sess1 = WorldMgr.GetSession(pChar.Guid);
            //var pkt = ObjectHandler.HandleDestroyObject(ref sess1, pChar.Guid);
            // Destroy object after logout
            // sess1.Send(ref logoutComplete);
            WorldMgr.WriteOutOfRangeObjects(SpawnMgr.GetOutOfRangeCreatures(pChar), WorldMgr.Sessions.ToList()[0].Value);

            WorldMgr.DeleteSession(pChar.Guid);
            //WorldMgr.DeleteSession2(pChar.Guid);

            session.Send(ref logoutComplete);

        }
    }
}
