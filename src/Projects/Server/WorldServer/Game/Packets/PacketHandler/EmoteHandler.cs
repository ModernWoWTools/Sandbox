using AuthServer.Network;
using Framework.Constants.Net;
using Framework.Network.Packets;
using System.Linq;
using AuthServer.WorldServer.Managers;
using AuthServer.WorldServer.Game.Entities;
using AuthServer.Game.Entities;

namespace AuthServer.WorldServer.Game.Packets.PacketHandler
{
    public class EmoteHandler
    {
        static uint currEmote = 0;

        [Opcode2(ClientMessage.MapReset, "18125")]
        public static void HandleEmoteReset(ref PacketReader packet, WorldClass2 session)
        {
            var session1 = Manager.WorldMgr.Sessions.First().Value;

            SendEmote(26, session1, null);
        }

        [Opcode2(ClientMessage.Emote, "18125")]
        public static void HandleEmote(ref PacketReader packet, WorldClass2 session)
        {
            var guid = packet.ReadSmartGuid();
            var emote = packet.ReadUInt32();
            var emoteSoundkit = packet.ReadUInt32();

            var emoteId = Manager.WorldMgr.EmoteList.SingleOrDefault(e => e.Id == emote);
            if (emoteId == null)
                return;

            var session1 = Manager.WorldMgr.Sessions.First().Value;

            if (currEmote == (uint)emoteId.EmoteId)
                currEmote = 0;
            else
                currEmote = (uint)emoteId.EmoteId;

            SendEmote(currEmote, session1, null);
        }

        public static void SendEmote(uint emoteId, WorldClass session, WorldObject obj, ulong guid = 0)
        {
            var emoteResponse = new PacketWriter(ServerMessage.Emote);

            //if (obj == null)
            //{
            //    emoteResponse.WriteSmartGuid(session.Character.Guid);
            //    emoteResponse.WriteUInt32(currEmote);
            //
            //    session.Send(ref emoteResponse);
            //
            //    session.Character.SetUpdateField<uint>((int)UnitFields.EmoteState, currEmote);
            //    
            //    //ObjectHandler.HandleUpdateObjectValues(ref session);
            //}
            //else
            {
                if (obj == null)
                    emoteResponse.WriteSmartGuid(session.Character.Guid);
                else if (obj is Character)
                    emoteResponse.WriteSmartGuid(guid);
                else
                    emoteResponse.WriteSmartGuid(obj.SGuid);

                emoteResponse.WriteUInt32(emoteId);
                emoteResponse.WriteUInt32(0);

                session.Send(ref emoteResponse);

                //obj.SetUpdateField<uint>((int)UnitFields.EmoteState, currEmote);

                //ObjectHandler.HandleUpdateObjectValues(ref session, obj);
            }
        }
    }
}
