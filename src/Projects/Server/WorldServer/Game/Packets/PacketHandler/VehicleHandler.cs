using Framework.Network.Packets;
using AuthServer.Network;
using Framework.Constants.Net;

namespace AuthServer.WorldServer.Game.Packets.PacketHandler
{
    class VehicleHandler
    {
        [Opcode(ClientMessage.CastSpell, "21796")]
        public static void HandleCastSpell(ref PacketReader packet, WorldClass session)
        {
        }
    }
}
