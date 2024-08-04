using AuthServer.Network;
using Framework.Network.Packets;
using Framework.Constants.Net;

namespace AuthServer.WorldServer.Game.Packets.PacketHandler
{
    public class StoreHandler
    {
        public static void HandleEnableStore(ref WorldClass session)
        {
            var featureSystemStatusGlueScreen = new PacketWriter(ServerMessage.FeatureSystemStatusGlueScreen);

            var bitPack = new BitPack(featureSystemStatusGlueScreen);

            var IsDisabledByParentalControls = true; // Store
            var IsEnabled = true;  // Store
            var IsAvailable = true;  // PurchaseAPI

            bitPack.Write(false);
            bitPack.Write(IsEnabled);
            bitPack.Write(IsDisabledByParentalControls);
            bitPack.Write(IsAvailable);

            bitPack.Flush();

            session.Send(ref featureSystemStatusGlueScreen);

            var distributionList = new PacketWriter(ServerMessage.DistributionList);
            bitPack = new BitPack(distributionList);

            bitPack.Write(0, 19);
            bitPack.Flush();

            distributionList.WriteUInt32(0);

            session.Send(ref distributionList);

        }
    }
}
