using AuthServer.Network;
using Framework.Constants.Net;
using Framework.Network.Packets;

namespace AuthServer.WorldServer.Game.Packets.PacketHandler
{
    class GarrisonHandler
    {
        public static void HandleGarrisonArchitectShow(ref WorldClass session)
        {
            var garrShow = new PacketWriter(ServerMessage.GarrisonArchitectShow);
            BitPack BitPack = new BitPack(garrShow, session.Character.Guid);

            BitPack.WriteGuidMask(7, 5, 3, 0, 4, 6, 2, 1);
            BitPack.Flush();

            BitPack.WriteGuidBytes(0, 1, 7, 6, 5, 4, 2, 3);

            session.Send(ref garrShow);
        }

        [Opcode(ClientMessage.GetGarrisonInfo, "17930")]
        public static void HandleGetGarrisonInfo(ref PacketReader packet, WorldClass session)
        {
            HandleGetGarrisonInfoResult(ref session);
        }

        [Opcode(ClientMessage.UpgradeGarrison, "17930")]
        public static void HandleUpgradeGarrison(ref PacketReader packet, WorldClass session)
        {
            var garUp = new PacketWriter(ServerMessage.GarrisonUpgradeResult);
            BitPack BitPack = new BitPack(garUp, session.Character.Guid);

            BitPack.Write(1);
            BitPack.Flush();

            garUp.Write(2);
            garUp.Write(2);

            session.Send(ref garUp);
        }


        public static void HandleGetGarrisonInfoResult(ref WorldClass session, bool sendFollower = false)
        {
            var garrInfo = new PacketWriter(ServerMessage.GetGarrisonInfoResult);
            BitPack BitPack = new BitPack(garrInfo, session.Character.Guid);

            BitPack.Write<int>(0, 19);
            BitPack.Write<int>(2, 19);

            BitPack.Write<int>(0, 22);
            BitPack.Write<int>(0, 19);
            BitPack.Write<int>(2, 18);
            BitPack.Write<int>(2, 22);
            BitPack.Write<int>(3, 22);
            BitPack.Flush();

            //if (sendFollower)
            {
                garrInfo.Write(6);
                garrInfo.Write(0);
                garrInfo.Write(2);
                garrInfo.Write(2);
                garrInfo.Write(200);
                garrInfo.Write(78009);
                garrInfo.Write(100);
                garrInfo.Write(6UL);
                garrInfo.Write(0);
                garrInfo.Write(100);

                garrInfo.Write(2);
                garrInfo.Write(1);
                garrInfo.Write(5);
                garrInfo.Write(90);//level
                garrInfo.Write(6);

                garrInfo.Write(4);
                garrInfo.Write(7);
            }
            {
                garrInfo.Write(18);
                garrInfo.Write(1);
                garrInfo.Write(14);
                garrInfo.Write(15);
                garrInfo.Write(450);
                garrInfo.Write(79376);
                garrInfo.Write(100);
                garrInfo.Write(18UL);
                garrInfo.Write(0);
                garrInfo.Write(100);

                garrInfo.Write(7);
                garrInfo.Write(1);
                garrInfo.Write(5);
                garrInfo.Write(100);//level
                garrInfo.Write(10);

                garrInfo.Write(30);
                garrInfo.Write(32);
                garrInfo.Write(40);
            }


            // mission
            garrInfo.Write(9999999);
            garrInfo.Write(2UL);
            garrInfo.Write(0);
            garrInfo.Write(9999999);
            garrInfo.Write(9999999);
            garrInfo.Write(2);
            garrInfo.Write(0);
            garrInfo.Write(9999999);

            garrInfo.Write(9999999);
            garrInfo.Write(7UL);
            garrInfo.Write(0);
            garrInfo.Write(9999999);
            garrInfo.Write(9999999);
            garrInfo.Write(7);
            garrInfo.Write(0);
            garrInfo.Write(9999999);

            garrInfo.Write(session.Character.Faction);
            garrInfo.Write(8);
            garrInfo.Write(3);

            session.Send(ref garrInfo);
        }

        public static void HandleGarrisonOpenMissionNPC(ref WorldClass session)
        {
            var garrMisson = new PacketWriter(ServerMessage.GarrisonOpenMissionNPC);
            BitPack BitPack = new BitPack(garrMisson, session.Character.Guid);


            BitPack.WriteGuidMask(2, 0, 7, 5, 6, 3, 4, 1);
            BitPack.Flush();

            BitPack.WriteGuidBytes(2, 1, 6, 5, 3, 4, 7, 0);

            session.Send(ref garrMisson);
        }

        public static void HandleOpenShipmentNPCFromGossip(ref WorldClass session)
        {
            var garrShip = new PacketWriter(ServerMessage.OpenShipmentNPCFromGossip);
            BitPack BitPack = new BitPack(garrShip, session.Character.Guid);


            BitPack.WriteGuidMask(6, 0, 3, 1, 4, 7, 6, 2);
            BitPack.Flush();

            BitPack.WriteGuidBytes(4, 2, 6, 1, 5, 7, 0, 3);
            garrShip.Write(1);
            session.Send(ref garrShip);
        }


        public static void HandleGarrisonAddMissionResult(ref WorldClass session)
        {
            var garraddMiss = new PacketWriter(ServerMessage.GarrisonAddMissionResult);

            garraddMiss.Write(0); // MissionState
            garraddMiss.Write(2); // MissionRecID
            garraddMiss.Write(2); // DbID
            garraddMiss.Write(1); // Result
            garraddMiss.Write(9999999); // OfferTime
            garraddMiss.Write(0); // StartTime
            garraddMiss.Write(9999999); // OfferDuration
            garraddMiss.Write(9999999); // TravelDuration
            garraddMiss.Write(9999999); // MissionDuration

            session.Send(ref garraddMiss);
        }

    }
}
