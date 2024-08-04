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
using AuthServer.Game.Entities;
using AuthServer.Network;
using AuthServer.WorldServer.Managers;
using Framework.Constants.Movement;
using Framework.Constants.Net;
using Framework.Network.Packets;
using Framework.ObjectDefines;

namespace AuthServer.Game.Packets.PacketHandler
{
    public class MoveHandler : Manager
    {
        public static int moveCounter;

        [Opcode2(ClientMessage.Move1, "")]
        [Opcode2(ClientMessage.Move2, "")]
        [Opcode2(ClientMessage.Move3, "")]
        [Opcode2(ClientMessage.Move4, "")]
        [Opcode2(ClientMessage.Move5, "")]
        [Opcode2(ClientMessage.Move6, "")]
        [Opcode2(ClientMessage.Move7, "")]
        [Opcode2(ClientMessage.Move8, "")]
        [Opcode2(ClientMessage.Move9, "")]
        [Opcode2(ClientMessage.Move10, "")]
        [Opcode2(ClientMessage.Move11, "")]
        [Opcode2(ClientMessage.Move12, "")]
        [Opcode2(ClientMessage.Move13, "")]
        [Opcode2(ClientMessage.Move14, "")]
        [Opcode2(ClientMessage.Move15, "")]
        [Opcode2(ClientMessage.Move16, "")]
        [Opcode2(ClientMessage.Move30, "")]
        [Opcode2(ClientMessage.Move31, "")]
        [Opcode2(ClientMessage.Move32, "")]
        [Opcode2(ClientMessage.Move33, "")]
        [Opcode2(ClientMessage.Move34, "")]
        [Opcode2(ClientMessage.Move35, "")]
        [Opcode2(ClientMessage.Move36, "")]
        [Opcode2(ClientMessage.Move37, "")]
        [Opcode2(ClientMessage.Move38, "")]
        [Opcode2(ClientMessage.Move39, "")]
        [Opcode2(ClientMessage.Move40, "")]
        [Opcode2(ClientMessage.Move41, "")]
        [Opcode2(ClientMessage.Move42, "")]
        [Opcode2(ClientMessage.Move43, "")]
        [Opcode2(ClientMessage.Move44, "")]
        [Opcode2(ClientMessage.Move45, "")]
        [Opcode2(ClientMessage.Move46, "")]
        [Opcode2(ClientMessage.Move47, "")]
        [Opcode2(ClientMessage.Move48, "")]
        [Opcode2(ClientMessage.Move49, "")]
        [Opcode2(ClientMessage.Move50, "")]
        [Opcode2(ClientMessage.Move51, "")]
        public static void HandleMoveStartForward(ref PacketReader packet, WorldClass2 session)
        {
            ++moveCounter;
            /*if (packet.Opcode == ClientMessage.MoveFallLand)
            {
                var sess1 = WorldMgr.Sessions.SingleOrDefault().Value;
                EmoteHandler.SendEmote(399, sess1);
            }*/

            ObjectMovementValues movementValues = new ObjectMovementValues();
            BitUnpack BitUnpack = new BitUnpack(packet);

            var guid = packet.ReadSmartGuid();


            movementValues.MovementFlags = (MovementFlag)packet.ReadUInt32();
            movementValues.MovementFlags2 = (MovementFlag2)packet.ReadUInt32();
            packet.ReadUInt32();

            movementValues.Time = packet.ReadUInt32();

            lastTime = movementValues.Time;

            Vector4 vector = new Vector4()
            {
                X = packet.Read<float>(),
                Y = packet.Read<float>(),
                Z = packet.Read<float>(),
                O = packet.Read<float>()
            };

            var f1 = packet.Read<float>();
            var f2 = packet.Read<float>();

            var counter = packet.ReadUInt32();
            var index = packet.ReadUInt32(); // index

            for (int i = 0;
            i < counter;
            i++)
                packet.Read<uint>();

            movementValues.IsTransport = BitUnpack.GetBit();
            movementValues.IsFallingOrJumping = BitUnpack.GetBit();

            BitUnpack.GetBit();
            BitUnpack.GetBit();
            BitUnpack.GetBit();

            if (movementValues.IsFallingOrJumping)
            {
                movementValues.FallTime = packet.Read<uint>();
                movementValues.JumpVelocity = packet.Read<float>();

                movementValues.HasJumpData = BitUnpack.GetBit();

                if (movementValues.HasJumpData)
                {
                    movementValues.Sin = packet.Read<float>();
                    movementValues.Cos = packet.Read<float>();
                    movementValues.CurrentSpeed = packet.Read<float>();
                }
            }

            if (session != null)
            {
                Character pChar = session.Character;

                if (pChar != null)
                    ObjectMgr.SetPosition(ref pChar, vector, false);
            }

            //HandleMoveUpdate(guid, movementValues, vector, index);
        }


        [Opcode2(ClientMessage.MoveSetActiveMover, "17930")]
        public static void HandleMoveSetActiveMover(ref PacketReader packet, WorldClass2 session)
        {
            Console.WriteLine("Setting active mover");

            var setActiveMover = new PacketWriter(ServerMessage.SetActiveMover);

            setActiveMover.WriteSmartGuid(session.Character.Guid);

            session.Send(ref setActiveMover);
        }

        public static uint lastTime;

        public static void HandleMoveUpdate(ulong guid, ObjectMovementValues movementValues, Vector4 vector, uint index)
        {
            PacketWriter moveUpdate = new PacketWriter(ServerMessage.MoveUpdate);
            BitPack BitPack = new BitPack(moveUpdate, guid);

            moveUpdate.WriteSmartGuid(guid);
            moveUpdate.WriteUInt32(movementValues.Time);
            moveUpdate.WriteFloat(vector.X);
            moveUpdate.WriteFloat(vector.Y);
            moveUpdate.WriteFloat(vector.Z);
            moveUpdate.WriteFloat(vector.O);
            moveUpdate.WriteFloat(0);
            moveUpdate.WriteFloat(0);
            moveUpdate.WriteUInt32(0);
            moveUpdate.WriteUInt32(0);

            BitPack.Write((uint)movementValues.MovementFlags, 30);
            BitPack.Write((uint)movementValues.MovementFlags2, 18);

            BitPack.Write(false);
            BitPack.Write(movementValues.IsFallingOrJumping);
            BitPack.Write(0);
            BitPack.Write(0);
            BitPack.Write(0);
            BitPack.Flush();


            if (movementValues.IsFallingOrJumping)
            {
                moveUpdate.WriteUInt32(movementValues.FallTime);
                moveUpdate.WriteFloat(movementValues.JumpVelocity);

                if (movementValues.HasJumpData)
                {
                    BitPack.Write(movementValues.HasJumpData);
                    BitPack.Flush();

                    moveUpdate.WriteFloat(movementValues.Sin);
                    moveUpdate.WriteFloat(movementValues.Cos);
                    moveUpdate.WriteFloat(movementValues.CurrentSpeed);
                }
            }

            var session = WorldMgr.GetSession(guid);
            if (session != null)
            {
                Character pChar = session.Character;

                ObjectMgr.SetPosition(ref pChar, vector, false);
                session.Send(ref moveUpdate);
            }
        }

        public static void HandleMoveSetWalkSpeed(ref WorldClass2 session, float speed = 2.5f)
        {
            PacketWriter setWalkSpeed = new PacketWriter(ServerMessage.MoveSetWalkSpeed);

            setWalkSpeed.WriteSmartGuid(session.Character.Guid);
            setWalkSpeed.WriteUInt32(0);
            setWalkSpeed.WriteFloat(speed);

            session.Send(ref setWalkSpeed);
        }

        public static void HandleMoveSetRunSpeed(ref WorldClass2 session, float speed = 7f)
        {
            PacketWriter setRunSpeed = new PacketWriter(ServerMessage.MoveSetRunSpeed);

            setRunSpeed.WriteSmartGuid(session.Character.Guid);
            setRunSpeed.WriteUInt32(0);
            setRunSpeed.WriteFloat(speed);

            session.Send(ref setRunSpeed);
        }

        public static void HandleMoveSetSwimSpeed(ref WorldClass2 session, float speed = 4.72222f)
        {
            PacketWriter setSwimSpeed = new PacketWriter(ServerMessage.MoveSetSwimSpeed);

            setSwimSpeed.WriteSmartGuid(session.Character.Guid);
            setSwimSpeed.WriteUInt32(0);
            setSwimSpeed.WriteFloat(speed);

            session.Send(ref setSwimSpeed);
        }

        public static void HandleMoveSetFlightSpeed(ref WorldClass2 session, float speed = 7f)
        {
            PacketWriter setFlightSpeed = new PacketWriter(ServerMessage.MoveSetFlightSpeed);

            setFlightSpeed.WriteSmartGuid(session.Character.Guid);
            setFlightSpeed.WriteUInt32(0);
            setFlightSpeed.WriteFloat(speed);

            session.Send(ref setFlightSpeed);
        }

        public static void HandleMoveSetCanFly(ref WorldClass2 session)
        {
            PacketWriter moveSetCanFly = new PacketWriter(ServerMessage.MoveSetCanFly);
            BitPack BitPack = new BitPack(moveSetCanFly, session.Character.Guid);

            moveSetCanFly.WriteSmartGuid(session.Character.Guid);
            moveSetCanFly.WriteUInt32(0);

            session.Send(ref moveSetCanFly);
        }

        public static void HandleMoveUnsetCanFly(ref WorldClass2 session)
        {
            PacketWriter unsetCanFly = new PacketWriter(ServerMessage.MoveUnsetCanFly);

            unsetCanFly.WriteSmartGuid(session.Character.Guid);
            unsetCanFly.WriteUInt32(0);

            session.Send(ref unsetCanFly);
        }

        public static void HandleMoveTeleport(ref WorldClass2 session, Vector4 vector)
        {
            bool isTransport = false;
            bool unknown = false;

            PacketWriter moveTeleport = new PacketWriter(ServerMessage.MoveTeleport);
            BitPack BitPack = new BitPack(moveTeleport);

            moveTeleport.WriteSmartGuid(session.Character.Guid);
            moveTeleport.WriteUInt32(0);

            moveTeleport.WriteFloat(vector.X);
            moveTeleport.WriteFloat(vector.Y);
            moveTeleport.WriteFloat(vector.Z);
            moveTeleport.WriteFloat(vector.O);
            moveTeleport.WriteUInt8(0);

            BitPack.Write(isTransport);
            BitPack.Write(unknown); // vehicle
            BitPack.Flush();

            if (isTransport)
                moveTeleport.WriteUInt64(0);

            if (unknown)
                moveTeleport.WriteUInt8(0);

            session.Send(ref moveTeleport);
        }

        public static void HandleTransferPending(ref WorldClass2 session, uint mapId)
        {
            var unknown = false;
            var isTransport = false;

            PacketWriter transferPending = new PacketWriter(ServerMessage.TransferPending);
            BitPack BitPack = new BitPack(transferPending);

            transferPending.WriteUInt32(mapId);
            transferPending.WriteFloat(session.Character.Position.X);
            transferPending.WriteFloat(session.Character.Position.Y);
            transferPending.WriteFloat(session.Character.Position.Z);
            BitPack.Write(isTransport);
            BitPack.Write(unknown);

            BitPack.Flush();

            session.Send(ref transferPending);
        }

        public static void HandleNewWorld(ref WorldClass2 session, Vector4 vector, uint mapId)
        {
            PacketWriter newWorld = new PacketWriter(ServerMessage.NewWorld);

            newWorld.WriteUInt32(mapId);
            newWorld.WriteFloat(vector.X);
            newWorld.WriteFloat(vector.Y);
            newWorld.WriteFloat(vector.Z);
            newWorld.WriteFloat(vector.O);
            newWorld.WriteUInt32(mapId);
            newWorld.WriteFloat(vector.X);
            newWorld.WriteFloat(vector.Y);
            newWorld.WriteFloat(vector.Z);

            session.Send(ref newWorld);
        }
    }
}
