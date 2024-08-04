using System;
using System.Collections.Generic;
using AuthServer.Network;
using Framework.Network.Packets;
using Framework.Constants.Net;
using Framework.Constants;
using Framework.Constants.Objects;
using Framework.ObjectDefines;
using AuthServer.WorldServer.Game.Entities;
using AuthServer.WorldServer.Managers;
using Framework.Serialization;
using Framework.Misc;
using System.IO;
using AuthServer.Game.Entities;
using AuthServer.WorldServer.Game.Chat.Commands;
using System.Collections;
using AuthServer.Game.PacketHandler;
using AuthServer.Game.Packets.PacketHandler;

namespace AuthServer.WorldServer.Game.Packets.PacketHandler
{
    public class ItemHandler
    {
        [Opcode2(ClientMessage.UnEquipItem, "18125")]
        public static void HandleUnEquipItem(ref PacketReader packet, WorldClass2 session)
        {
            var data = packet.ReadBytes(5);
            var bagSlot = packet.Read<byte>(); // targetSlot - 23 + slotIndex in default bag
            var equipSlot = packet.Read<byte>(); // sourceSlot - 24

            Item removeditem2 = null;
            //if (session.Character.Bag.ContainsKey(bagSlot) && 
            //if (session.Character.Equipment.ContainsKey(equipSlot))
            {
                if (equipSlot >= 23 && bagSlot >= 23)
                {
                    var sourceitem = session.Character.Bags[255][equipSlot];
                    var destItemGuid = new SmartGuid();

                    session.Character.Bags[255].Remove(equipSlot);

                    if (PlayerCommands.bagSlotsList[bagSlot] && session.Character.Bags[255].TryGetValue(bagSlot, out var destitem))
                    {
                        session.Character.Bags[255].Remove(bagSlot);

                        session.Character.Bags[255].Add(equipSlot, destitem);
                        PlayerCommands.bagSlotsList[equipSlot] = true;

                        destItemGuid = new SmartGuid(destitem.Guid, destitem.Id, GuidType.Item);
                        //.Character.SetUpdateField<ulong>((int)ActivePlayerFields.InvSlots + ((equipSlot) * 4), itemGuid.Guid);
                        //session.Character.SetUpdateField<ulong>((int)ActivePlayerFields.InvSlots + ((equipSlot) * 4) + 2, itemGuid.HighGuid);
                    }
                    else
                    {
                        PlayerCommands.bagSlotsList[equipSlot] = false;

                        //session.Character.SetUpdateField<ulong>((int)ActivePlayerFields.InvSlots + ((equipSlot) * 4), 0ul);
                        //session.Character.SetUpdateField<ulong>((int)ActivePlayerFields.InvSlots + ((equipSlot) * 4) + 2, 0ul);

                    }

                    session.Character.Bags[255].Add(bagSlot, sourceitem);
                    PlayerCommands.bagSlotsList[bagSlot] = true;
                    var sourceItemGuid = new SmartGuid(sourceitem.Guid, sourceitem.Id, GuidType.Item);
                    //session.Character.SetUpdateField<ulong>((int)ActivePlayerFields.InvSlots + ((bagSlot) * 4), itemGuid2.Guid);
                    //session.Character.SetUpdateField<ulong>((int)ActivePlayerFields.InvSlots + ((bagSlot) * 4) + 2, itemGuid2.HighGuid);


                    var sess1 = Manager.WorldMgr.GetSession(session.Character.Guid);

                    sess1.Character = session.Character;

                    WorldObject character = session.Character;
                    PacketWriter updateObject = new PacketWriter(ServerMessage.ObjectUpdate);
                    BitPack BitPack = new BitPack(updateObject);

                    updateObject.WriteUInt32(1);
                    updateObject.WriteUInt16((ushort)character.Map);
                    updateObject.WriteUInt8(0);
                    updateObject.WriteInt32(0);

                    updateObject.WriteUInt8((byte)UpdateType.Values);
                    updateObject.WriteSmartGuid(session.Character.Guid);

                    var writer = new PacketWriter();
                    var bp = new BitPack(writer);
                    var bits = new BitArray((((int)ActivePlayerFields.End + 32 - 1) / 32) * 32, false);

                    // for (var j = 0; j < bits.Length; j += 32)
                    //     bits.Set(j, false);

                    bits.Set((int)ActivePlayerFields.EnableInvSlots, true);
                    bits.Set((int)ActivePlayerFields.InvSlots + (bagSlot), true);
                    bits.Set((int)ActivePlayerFields.InvSlots + (equipSlot), true);

                    // Always send all uint32 flags
                    writer.WriteUInt32(524287);
                    bp.Write(0, 17);

                    byte[] ret = new byte[(bits.Length) / 8 + 1];
                    bits.CopyTo(ret, 0);

                    var masks = new uint[ret.Length / 4];

                    for (var j = 0; j < masks.Length; j++)
                        masks[j] = BitConverter.ToUInt32(ret, j * 4);

                    for (var j = 0; j < masks.Length; j++)
                    {
                        //if (masks[j] != 0)
                        bp.Write(masks[j], 32);
                    }

                    bp.Flush();

                    var fieldWriter = new PacketWriter();
                    if (equipSlot < bagSlot)
                    {
                        // Empty GUID
                        fieldWriter.WriteSmartGuid(destItemGuid);
                        fieldWriter.WriteSmartGuid(sourceItemGuid);
                    }

                    if (bagSlot < equipSlot)
                    {
                        // Empty GUID
                        fieldWriter.WriteSmartGuid(sourceItemGuid);
                        fieldWriter.WriteSmartGuid(destItemGuid);
                    }

                    var fieldData = (fieldWriter.BaseStream as MemoryStream).ToArray();

                    updateObject.Write((writer.BaseStream as MemoryStream).ToArray().Length + 4 + fieldData.Length);
                    updateObject.WriteInt32(0x80);

                    updateObject.Write((writer.BaseStream as MemoryStream).ToArray());

                    updateObject.Write(fieldData);

                    var size = (uint)updateObject.BaseStream.Length - 29;
                    updateObject.WriteUInt32Pos(size, 25);

                    sess1.Send(ref updateObject);


                    return;
                }
                else if (session.Character.Equipment.ContainsKey(equipSlot))
                    removeditem2 = session.Character.Equipment[equipSlot];
                else
                {
                    AddItem(session, equipSlot, bagSlot);

                    return;
                }

                // Mark it as used.
                PlayerCommands.bagSlotsList[bagSlot] = true;

                //removedItem = session.Character.Bag [bagSlot];
                // remove if item exists for now...
                if (session.Character.Bags[255].ContainsKey(bagSlot))
                    session.Character.Bags[255].Remove(bagSlot);

                session.Character.Equipment.Remove(equipSlot);
                //if (!session.Character.Equipment.ContainsKey(equipSlot) && !session.Character.Bag.ContainsKey(bagSlot))
                {
                    //session.Character.Equipment.Add (equipSlot, removedItem);
                    session.Character.Bags[255].Add(bagSlot, removeditem2);

                    var bagItemGuid = new SmartGuid(removeditem2.Guid, removeditem2.Id, GuidType.Item);

                    //HandleItemPushResult(removedItem.Id, ref session);

                    //session.Character.SetUpdateField<ulong>((int)PlayerFields.InvSlots + (equipSlot * 4), removedItem.Guid);
                    //session.Character.SetUpdateField<ulong>((int)PlayerFields.InvSlots + (equipSlot * 4) + 2, 0x0C0F280000000000);


                    //session.Character.SetUpdateField<int>((int)PlayerFields.VisibleItems + (equipSlot * 3), removedItem.Id);


                    var sess1 = Manager.WorldMgr.GetSession(session.Character.Guid);

                    sess1.Character = session.Character;

                    WorldObject character = session.Character;
                    PacketWriter updateObject = new PacketWriter(ServerMessage.ObjectUpdate);
                    BitPack BitPack = new BitPack(updateObject);

                    updateObject.WriteUInt32(1);
                    updateObject.WriteUInt16((ushort)character.Map);
                    updateObject.WriteUInt8(0);
                    updateObject.WriteInt32(0);

                    updateObject.WriteUInt8((byte)UpdateType.Values);
                    updateObject.WriteSmartGuid(session.Character.Guid);

                    var writer = new PacketWriter();
                    var bp = new BitPack(writer);
                    var bits = new BitArray((((int)ActivePlayerFields.End + 32 - 1) / 32) * 32, false);

                    // for (var j = 0; j < bits.Length; j += 32)
                    //     bits.Set(j, false);

                    bits.Set((int)ActivePlayerFields.EnableInvSlots, true);
                    bits.Set((int)ActivePlayerFields.InvSlots + (bagSlot), true);
                    bits.Set((int)ActivePlayerFields.InvSlots + (equipSlot), true);

                    // Always send all uint32 flags
                    writer.WriteUInt32(524287);
                    bp.Write(0, 17);

                    byte[] ret = new byte[(bits.Length) / 8 + 1];
                    bits.CopyTo(ret, 0);

                    var masks = new uint[ret.Length / 4];

                    for (var j = 0; j < masks.Length; j++)
                        masks[j] = BitConverter.ToUInt32(ret, j * 4);

                    for (var j = 0; j < masks.Length; j++)
                    {
                        //if (masks[j] != 0)
                        bp.Write(masks[j], 32);
                    }

                    bp.Flush();

                    var fieldWriter = new PacketWriter();

                    fieldWriter.WriteSmartGuid(new SmartGuid());
                    fieldWriter.WriteSmartGuid(bagItemGuid);

                    var fieldData = (fieldWriter.BaseStream as MemoryStream).ToArray();

                    updateObject.Write((writer.BaseStream as MemoryStream).ToArray().Length + 4 + fieldData.Length);
                    updateObject.WriteInt32(0x80);

                    updateObject.Write((writer.BaseStream as MemoryStream).ToArray());

                    // Empty GUID
                    updateObject.Write(fieldData);

                    var size = (uint)updateObject.BaseStream.Length - 29;
                    updateObject.WriteUInt32Pos(size, 25);

                    sess1.Send(ref updateObject);

                    // Visible EQUIP
                    //session.Character.SetUpdateField<int>((int)PlayerFields.VisibleItems + (bagSlot * 2), 0);
                    //session.Character.SetUpdateField<ushort>((int)PlayerFields.VisibleItems + (bagSlot * 2) + 1, 0, 0);
                    //session.Character.SetUpdateField<ushort>((int)PlayerFields.VisibleItems + (bagSlot * 2) + 1, 0, 0);
                    //session.Character.SetUpdateField<ushort>((int)PlayerFields.VisibleItems + (bagSlot * 2) + 1, 0, 1);

                    character = session.Character;
                    updateObject = new PacketWriter(ServerMessage.ObjectUpdate);
                    BitPack = new BitPack(updateObject);

                    updateObject.WriteUInt32(1);
                    updateObject.WriteUInt16((ushort)character.Map);
                    updateObject.WriteUInt8(0);
                    updateObject.WriteInt32(0);

                    updateObject.WriteUInt8((byte)UpdateType.Values);
                    updateObject.WriteSmartGuid(session.Character.Guid);

                    writer = new PacketWriter();
                    bp = new BitPack(writer);
                    bits = new BitArray((((int)PlayerFields.End + 32 - 1) / 32) * 32, false);

                    // for (var j = 0; j < bits.Length; j += 32)
                    //     bits.Set(j, false);

                    bits.Set((int)PlayerFields.EnableVisibleItems, true);
                    bits.Set((int)PlayerFields.VisibleItems + (equipSlot), true);

                    // Always send all uint32 flags
                    bp.Write(63, 6);

                    ret = new byte[(bits.Length) / 8 + 1];
                    bits.CopyTo(ret, 0);

                    masks = new uint[ret.Length / 4];

                    for (var j = 0; j < masks.Length; j++)
                        masks[j] = BitConverter.ToUInt32(ret, j * 4);

                    for (var j = 0; j < masks.Length; j++)
                    {
                        //if (masks[j] != 0)
                        bp.Write(masks[j], 32);
                    }

                    bp.Flush();
                    
                    updateObject.Write((writer.BaseStream as MemoryStream).ToArray().Length + 4 + 10);
                    updateObject.WriteInt32(0x40);

                    updateObject.Write((writer.BaseStream as MemoryStream).ToArray());

                    BitPack.Write(0x1F, 5); //  all 3 values
                    BitPack.Flush();

                    updateObject.WriteInt32(0);
                    updateObject.WriteInt32(0);
                    updateObject.WriteInt16(0);
                    updateObject.WriteInt16(0);

                    size = (uint)updateObject.BaseStream.Length - 29;
                    updateObject.WriteUInt32Pos(size, 25);

                    sess1.Send(ref updateObject);
                }
            }

            Manager.ObjectMgr.SaveChar(session.Character);
        }

        [Opcode2(ClientMessage.EquipItem, "18125")]
        public static void HandleEquipItem(ref PacketReader packet, WorldClass2 session)
        {
            var val = packet.ReadUInt32();
            var bagSlot = packet.Read<byte>();
            AddItem(session, bagSlot);

            Manager.ObjectMgr.SaveChar(session.Character);
        }

        [Opcode2(ClientMessage.SwapItem, "18125")]
        public static void HandleSwapItem(ref PacketReader packet, WorldClass2 session)
        {
            var bitunpack = new BitUnpack(packet);
            var ctr = bitunpack.GetBits<byte>(2);
            var invitem = new List<(byte ContainerSlot, byte slot)>();

            // First: ContainerSlot = Bag (22), slot = place in bag (0)
            // Second: ContainerSlot = No bag (255), slot = equipSlot (6)
            for (var i = 0; i < ctr; i++)
                invitem.Add((packet.ReadByte(), packet.ReadByte()));

            var targetBag = packet.ReadByte(); // 255 if equip/default bag
            var sourceBag = packet.ReadByte(); // 255 if equip/default bag
            var targetSlot = packet.ReadByte(); // slot index of the target bag
            var sourceSlot = packet.ReadByte(); // slot index of the source/default bag

            if (targetBag == 255)
            {

            }
            else
            {
                byte equipSlot = 0;
                byte bagSlot = 0;
                Item removeditem2 = null;
                //if (session.Character.Bag.ContainsKey(bagSlot) && 
                //if (session.Character.Equipment.ContainsKey(equipSlot))
                {
                    if (equipSlot >= 23 && bagSlot >= 23)
                    {
                        throw new NotImplementedException("Nope Nope Nope");
                    }
                    else if (session.Character.Equipment.ContainsKey(equipSlot))
                        removeditem2 = session.Character.Equipment[equipSlot];
                    else
                    {
                        AddItem(session, equipSlot, bagSlot);

                        return;
                    }

                    //removedItem = session.Character.Bag [bagSlot];
                    // remove if item exists for now...
                    Item equipItem = null;
                    if (session.Character.Bags[255].ContainsKey(bagSlot))
                    {
                        equipItem = session.Character.Bags[255][bagSlot];
                        session.Character.Bags[255].Remove(bagSlot);
                    }

                    session.Character.Equipment.Remove(equipSlot);
                    //if (!session.Character.Equipment.ContainsKey(equipSlot) && !session.Character.Bag.ContainsKey(bagSlot))
                    {
                        session.Character.Bags[255].Add(bagSlot, removeditem2);

                        var bagItemGuid = new SmartGuid(removeditem2.Guid, removeditem2.Id, GuidType.Item);
                        var equipItemGuid = new SmartGuid(equipItem.Guid, removeditem2.Id, GuidType.Item);
                        var sess1 = Manager.WorldMgr.GetSession(session.Character.Guid);

                        sess1.Character = session.Character;

                        WorldObject character = session.Character;
                        PacketWriter updateObject = new PacketWriter(ServerMessage.ObjectUpdate);
                        BitPack BitPack = new BitPack(updateObject);

                        updateObject.WriteUInt32(1);
                        updateObject.WriteUInt16((ushort)character.Map);
                        updateObject.WriteUInt8(0);
                        updateObject.WriteInt32(0);

                        updateObject.WriteUInt8((byte)UpdateType.Values);
                        updateObject.WriteSmartGuid(session.Character.Guid);

                        var writer = new PacketWriter();
                        var bp = new BitPack(writer);
                        var bits = new BitArray((((int)ActivePlayerFields.End + 32 - 1) / 32) * 32, false);

                        // for (var j = 0; j < bits.Length; j += 32)
                        //     bits.Set(j, false);

                        bits.Set((int)ActivePlayerFields.EnableInvSlots, true);
                        bits.Set((int)ActivePlayerFields.InvSlots + (bagSlot), true);
                        bits.Set((int)ActivePlayerFields.InvSlots + (equipSlot), true);

                        // Always send all uint32 flags
                        writer.WriteUInt32(524287);
                        bp.Write(0, 17);

                        byte[] ret = new byte[(bits.Length) / 8 + 1];
                        bits.CopyTo(ret, 0);

                        var masks = new uint[ret.Length / 4];

                        for (var j = 0; j < masks.Length; j++)
                            masks[j] = BitConverter.ToUInt32(ret, j * 4);

                        for (var j = 0; j < masks.Length; j++)
                        {
                            //if (masks[j] != 0)
                            bp.Write(masks[j], 32);
                        }

                        bp.Flush();

                        var fieldWriter = new PacketWriter();

                        fieldWriter.WriteSmartGuid(equipItemGuid);
                        fieldWriter.WriteSmartGuid(bagItemGuid);

                        var fieldData = (fieldWriter.BaseStream as MemoryStream).ToArray();

                        updateObject.Write((writer.BaseStream as MemoryStream).ToArray().Length + 4 + fieldData.Length);
                        updateObject.WriteInt32(0x80);

                        updateObject.Write((writer.BaseStream as MemoryStream).ToArray());

                        // Empty GUID
                        updateObject.Write(fieldData);

                        var size = (uint)updateObject.BaseStream.Length - 29;
                        updateObject.WriteUInt32Pos(size, 25);

                        sess1.Send(ref updateObject);

                        character = session.Character;
                        updateObject = new PacketWriter(ServerMessage.ObjectUpdate);
                        BitPack = new BitPack(updateObject);

                        updateObject.WriteUInt32(1);
                        updateObject.WriteUInt16((ushort)character.Map);
                        updateObject.WriteUInt8(0);
                        updateObject.WriteInt32(0);

                        updateObject.WriteUInt8((byte)UpdateType.Values);
                        updateObject.WriteSmartGuid(session.Character.Guid);

                        writer = new PacketWriter();
                        bp = new BitPack(writer);
                        bits = new BitArray((((int)PlayerFields.End + 32 - 1) / 32) * 32, false);

                        // for (var j = 0; j < bits.Length; j += 32)
                        //     bits.Set(j, false);

                        bits.Set((int)PlayerFields.EnableVisibleItems, true);
                        bits.Set((int)PlayerFields.VisibleItems + (equipSlot), true);

                        // Always send all uint32 flags
                        bp.Write(63, 6);

                        ret = new byte[(bits.Length) / 8 + 1];
                        bits.CopyTo(ret, 0);

                        masks = new uint[ret.Length / 4];

                        for (var j = 0; j < masks.Length; j++)
                            masks[j] = BitConverter.ToUInt32(ret, j * 4);

                        for (var j = 0; j < masks.Length; j++)
                        {
                            //if (masks[j] != 0)
                            bp.Write(masks[j], 32);
                        }

                        bp.Flush();

                        updateObject.Write((writer.BaseStream as MemoryStream).ToArray().Length + 4 + 10);
                        updateObject.WriteInt32(0x40);

                        updateObject.Write((writer.BaseStream as MemoryStream).ToArray());

                        BitPack.Write(0x1F, 5); //  all 4 values
                        BitPack.Flush();

                        updateObject.WriteInt32(equipItem.Id);
                        updateObject.WriteInt32(0);
                        updateObject.WriteInt16((short)equipItem.ModId);
                        updateObject.WriteInt16(0);

                        size = (uint)updateObject.BaseStream.Length - 29;
                        updateObject.WriteUInt32Pos(size, 25);

                        sess1.Send(ref updateObject);
                    }
                }
            }
        }

        static void AddItem(WorldClass2 session, byte bagSlot, byte targetSlot = 0xFF)
        {
            Item item;

            var sess1 = Manager.WorldMgr.GetSession(session.Character.Guid);

            if (session.Character.Bags[255].TryGetValue(bagSlot, out item))
            {
                var bagItemGuid = new SmartGuid();
                var equipItemGuid = new SmartGuid();
                var equipSlot = GetEquipSlot(item.InventoryType, sess1.Character);
                Item addedItem;

                if (targetSlot != 0xFF)
                    equipSlot = targetSlot;

                if (!session.Character.Equipment.ContainsKey(equipSlot))
                {
                    session.Character.Equipment.Add(equipSlot, item);

                    addedItem = item;

                    equipItemGuid = new SmartGuid(item.Guid, item.Id, GuidType.Item);
                    //if (session.Character.Bag.ContainsKey(bagSlot))
                    {
                        session.Character.Bags[255].Remove(bagSlot);
                    }
                }
                else
                {
                    //if (session.Character.Bag.ContainsKey(bagSlot) && 
                    //if (session.Character.Equipment.ContainsKey(equipSlot))
                    {
                        var removeditem2 = session.Character.Equipment[equipSlot];
                        //var itemGuid = new SmartGuid(removeditem2.Guid, 0, GuidType.Item);

                        var newItem = session.Character.Bags[255][bagSlot];
                        //var itemGuid2 = new SmartGuid(newItem.Guid, 0, GuidType.Item);

                        // Remove the items.
                        session.Character.Bags[255].Remove(bagSlot);
                        session.Character.Equipment.Remove(equipSlot);

                        // Add swapped items.
                        session.Character.Bags[255].Add(bagSlot, removeditem2);
                        bagItemGuid = new SmartGuid(removeditem2.Guid, removeditem2.Id, GuidType.Item);

                        session.Character.Equipment.Add(equipSlot, newItem);
                        equipItemGuid = new SmartGuid(newItem.Guid, newItem.Id, GuidType.Item);

                        addedItem = newItem;
                    }
                }

                WorldObject character = session.Character;
                PacketWriter updateObject = new PacketWriter(ServerMessage.ObjectUpdate);
                BitPack BitPack = new BitPack(updateObject);

                updateObject.WriteUInt32(1);
                updateObject.WriteUInt16((ushort)character.Map);
                updateObject.WriteUInt8(0);
                updateObject.WriteInt32(0);

                updateObject.WriteUInt8((byte)UpdateType.Values);
                updateObject.WriteSmartGuid(session.Character.Guid);

                var writer = new PacketWriter();
                var bp = new BitPack(writer);
                var bits = new BitArray((((int)ActivePlayerFields.End + 32 - 1) / 32) * 32, false);

                // for (var j = 0; j < bits.Length; j += 32)
                //     bits.Set(j, false);

                bits.Set((int)ActivePlayerFields.EnableInvSlots, true);
                bits.Set((int)ActivePlayerFields.InvSlots + (bagSlot), true);
                bits.Set((int)ActivePlayerFields.InvSlots + (equipSlot), true);

                // Always send all uint32 flags
                writer.WriteUInt32(524287);
                bp.Write(0, 17);

                byte[] ret = new byte[(bits.Length) / 8 + 1];
                bits.CopyTo(ret, 0);

                var masks = new uint[ret.Length / 4];

                for (var j = 0; j < masks.Length; j++)
                    masks[j] = BitConverter.ToUInt32(ret, j * 4);

                for (var j = 0; j < masks.Length; j++)
                {
                    //if (masks[j] != 0)
                    bp.Write(masks[j], 32);
                }

                bp.Flush();

                var fieldWriter = new PacketWriter();

                fieldWriter.WriteSmartGuid(equipItemGuid);
                fieldWriter.WriteSmartGuid(bagItemGuid);

                var fieldData = (fieldWriter.BaseStream as MemoryStream).ToArray();

                updateObject.Write((writer.BaseStream as MemoryStream).ToArray().Length + 4 + fieldData.Length);
                updateObject.WriteInt32(0x80);

                updateObject.Write((writer.BaseStream as MemoryStream).ToArray());

                // Empty GUID
                updateObject.Write(fieldData);

                var size = (uint)updateObject.BaseStream.Length - 29;
                updateObject.WriteUInt32Pos(size, 25);

                sess1.Send(ref updateObject);

                character = session.Character;
                updateObject = new PacketWriter(ServerMessage.ObjectUpdate);
                BitPack = new BitPack(updateObject);

                updateObject.WriteUInt32(1);
                updateObject.WriteUInt16((ushort)character.Map);
                updateObject.WriteUInt8(0);
                updateObject.WriteInt32(0);

                updateObject.WriteUInt8((byte)UpdateType.Values);
                updateObject.WriteSmartGuid(session.Character.Guid);

                writer = new PacketWriter();
                bp = new BitPack(writer);
                bits = new BitArray((((int)PlayerFields.End + 32 - 1) / 32) * 32, false);

                // for (var j = 0; j < bits.Length; j += 32)
                //     bits.Set(j, false);

                bits.Set((int)PlayerFields.EnableVisibleItems, true);
                bits.Set((int)PlayerFields.VisibleItems + (equipSlot), true);

                // Always send all uint32 flags
                bp.Write(63, 6);

                ret = new byte[(bits.Length) / 8 + 1];
                bits.CopyTo(ret, 0);

                masks = new uint[ret.Length / 4];

                for (var j = 0; j < masks.Length; j++)
                    masks[j] = BitConverter.ToUInt32(ret, j * 4);

                for (var j = 0; j < masks.Length; j++)
                {
                    //if (masks[j] != 0)
                    bp.Write(masks[j], 32);
                }

                bp.Flush();

                updateObject.Write((writer.BaseStream as MemoryStream).ToArray().Length + 4 + 10);
                updateObject.WriteInt32(0x40);

                updateObject.Write((writer.BaseStream as MemoryStream).ToArray());

                BitPack.Write(0x1F, 5); //  all 3 values
                BitPack.Flush();

                updateObject.WriteInt32(addedItem.Id);
                updateObject.WriteInt32(0);
                updateObject.WriteInt16((short)addedItem.ModId);
                updateObject.WriteInt16(0);

                size = (uint)updateObject.BaseStream.Length - 29;
                updateObject.WriteUInt32Pos(size, 25);

                sess1.Send(ref updateObject);
            }

            var index = 0;
            for (int i = 0; i < Manager.WorldMgr.CharaterList.Count; i++)
            {
                if (Manager.WorldMgr.CharaterList[i].Guid == session.Character.Guid)
                {
                    index = i;
                    break;
                }
            }

            Manager.WorldMgr.CharaterList[index] = session.Character;
            File.WriteAllText(Helper.DataDirectory() + "characters.json", Json.CreateString(Manager.WorldMgr.CharaterList));

            Manager.ObjectMgr.SaveChar(session.Character);
        }

        [Opcode2(ClientMessage.DestroyItem, "18125")]
        public static void HandleDestroyItem(ref PacketReader packet, WorldClass2 session)
        {
            var unknown = packet.Read<uint>();
            var bagId = packet.Read<byte>();
            var bagSlot = packet.Read<byte>();
            var sess1 = Manager.WorldMgr.GetSession(session.Character.Guid);
            if (bagSlot >= 23)
            {
                Item removedItem;
                //if (session.Character.Bag.ContainsKey(bagSlot))
                {
                    removedItem = session.Character.Bags[255][bagSlot];
                    session.Character.Bags[255].Remove(bagSlot);

                    PlayerCommands.bagSlotsList[bagSlot] = false;

                    // ObjectHandler.HandleUpdateObjectValues(ref sess1);
                    WorldObject character = session.Character;
                    PacketWriter updateObject = new PacketWriter(ServerMessage.ObjectUpdate);
                    BitPack BitPack = new BitPack(updateObject);

                    updateObject.WriteUInt32(1);
                    updateObject.WriteUInt16((ushort)character.Map);
                    updateObject.WriteUInt8(0);
                    updateObject.WriteInt32(0);

                    updateObject.WriteUInt8((byte)UpdateType.Values);
                    updateObject.WriteSmartGuid(session.Character.Guid);

                    var writer = new PacketWriter();
                    var bp = new BitPack(writer);
                    var bits = new BitArray((((int)ActivePlayerFields.End + 32 - 1) / 32) * 32, false);

                    // for (var j = 0; j < bits.Length; j += 32)
                    //     bits.Set(j, false);

                    bits.Set((int)ActivePlayerFields.EnableInvSlots, true);
                    bits.Set((int)ActivePlayerFields.InvSlots + (bagSlot), true);

                    // Always send all uint32 flags
                    writer.WriteUInt32(524287);
                    bp.Write(0, 17);

                    byte[] ret = new byte[(bits.Length) / 8 + 1];
                    bits.CopyTo(ret, 0);

                    var masks = new uint[ret.Length / 4];

                    for (var j = 0; j < masks.Length; j++)
                        masks[j] = BitConverter.ToUInt32(ret, j * 4);

                    for (var j = 0; j < masks.Length; j++)
                    {
                        //if (masks[j] != 0)
                        bp.Write(masks[j], 32);
                    }

                    bp.Flush();

                    var fieldWriter = new PacketWriter();

                    fieldWriter.WriteSmartGuid(new SmartGuid());

                    var fieldData = (fieldWriter.BaseStream as MemoryStream).ToArray();

                    updateObject.Write((writer.BaseStream as MemoryStream).ToArray().Length + 4 + fieldData.Length);
                    updateObject.WriteInt32(0x80);

                    updateObject.Write((writer.BaseStream as MemoryStream).ToArray());

                    // Empty GUID
                    updateObject.Write(fieldData);

                    var size = (uint)updateObject.BaseStream.Length - 29;
                    updateObject.WriteUInt32Pos(size, 25);

                    sess1.Send(ref updateObject);
                    //ObjectHandler.HandleDestroyObject(ref sess1, removedItem.Guid, true);
                }
            }
            else
            {
                //bagSlot = GetEquipSlot(bagSlot);
                Item removedItem;
                //if (session.Character.Equipment.ContainsKey(bagSlot))
                {
                    removedItem = session.Character.Equipment[bagSlot];
                    session.Character.Equipment.Remove(bagSlot);

                    WorldObject character = session.Character;
                    PacketWriter updateObject = new PacketWriter(ServerMessage.ObjectUpdate);
                    BitPack BitPack = new BitPack(updateObject);

                    updateObject.WriteUInt32(1);
                    updateObject.WriteUInt16((ushort)character.Map);
                    updateObject.WriteUInt8(0);
                    updateObject.WriteInt32(0);

                    updateObject.WriteUInt8((byte)UpdateType.Values);
                    updateObject.WriteSmartGuid(session.Character.Guid);

                    var writer = new PacketWriter();
                    var bp = new BitPack(writer);
                    var bits = new BitArray((((int)ActivePlayerFields.End + 32 - 1) / 32) * 32, false);

                    // for (var j = 0; j < bits.Length; j += 32)
                    //     bits.Set(j, false);

                    bits.Set((int)ActivePlayerFields.EnableInvSlots, true);
                    bits.Set((int)ActivePlayerFields.InvSlots + (bagSlot), true);

                    // Always send all uint32 flags
                    writer.WriteUInt32(524287);
                    bp.Write(0, 17);

                    byte[] ret = new byte[(bits.Length) / 8 + 1];
                    bits.CopyTo(ret, 0);

                    var masks = new uint[ret.Length / 4];

                    for (var j = 0; j < masks.Length; j++)
                        masks[j] = BitConverter.ToUInt32(ret, j * 4);

                    for (var j = 0; j < masks.Length; j++)
                    {
                        //if (masks[j] != 0)
                        bp.Write(masks[j], 32);
                    }

                    bp.Flush();

                    var fieldWriter = new PacketWriter();

                    fieldWriter.WriteSmartGuid(new SmartGuid());

                    var fieldData = (fieldWriter.BaseStream as MemoryStream).ToArray();

                    updateObject.Write((writer.BaseStream as MemoryStream).ToArray().Length + 4 + fieldData.Length);
                    updateObject.WriteInt32(0x80);

                    updateObject.Write((writer.BaseStream as MemoryStream).ToArray());

                    // Empty GUID
                    updateObject.Write(fieldData);

                    var size = (uint)updateObject.BaseStream.Length - 29;
                    updateObject.WriteUInt32Pos(size, 25);

                    sess1.Send(ref updateObject);

                    // Visible EQUIP
                    //session.Character.SetUpdateField<int>((int)PlayerFields.VisibleItems + (bagSlot * 2), 0);
                    //session.Character.SetUpdateField<ushort>((int)PlayerFields.VisibleItems + (bagSlot * 2) + 1, 0, 0);
                    //session.Character.SetUpdateField<ushort>((int)PlayerFields.VisibleItems + (bagSlot * 2) + 1, 0, 0);
                    //session.Character.SetUpdateField<ushort>((int)PlayerFields.VisibleItems + (bagSlot * 2) + 1, 0, 1);

                    character = session.Character;
                    updateObject = new PacketWriter(ServerMessage.ObjectUpdate);
                    BitPack = new BitPack(updateObject);

                    updateObject.WriteUInt32(1);
                    updateObject.WriteUInt16((ushort)character.Map);
                    updateObject.WriteUInt8(0);
                    updateObject.WriteInt32(0);

                    updateObject.WriteUInt8((byte)UpdateType.Values);
                    updateObject.WriteSmartGuid(session.Character.Guid);

                    writer = new PacketWriter();
                    bp = new BitPack(writer);
                    bits = new BitArray((((int)PlayerFields.End + 32 - 1) / 32) * 32, false);

                    // for (var j = 0; j < bits.Length; j += 32)
                    //     bits.Set(j, false);

                    bits.Set((int)PlayerFields.EnableVisibleItems, true);
                    bits.Set((int)PlayerFields.VisibleItems + (bagSlot), true);

                    // Always send all uint32 flags
                    bp.Write(63, 6);

                    ret = new byte[(bits.Length) / 8 + 1];
                    bits.CopyTo(ret, 0);

                    masks = new uint[ret.Length / 4];

                    for (var j = 0; j < masks.Length; j++)
                        masks[j] = BitConverter.ToUInt32(ret, j * 4);

                    for (var j = 0; j < masks.Length; j++)
                    {
                        //if (masks[j] != 0)
                        bp.Write(masks[j], 32);
                    }


                    bp.Flush();

                    updateObject.Write((writer.BaseStream as MemoryStream).ToArray().Length + 4 + 10);
                    updateObject.WriteInt32(0x40);

                    updateObject.Write((writer.BaseStream as MemoryStream).ToArray());

                    BitPack.Write(0x1F, 5); //  all 3 values
                    BitPack.Flush();

                    updateObject.WriteInt32(0);
                    updateObject.WriteInt32(0);
                    updateObject.WriteInt16(0);
                    updateObject.WriteInt16(0);

                    size = (uint)updateObject.BaseStream.Length - 29;
                    updateObject.WriteUInt32Pos(size, 25);

                    sess1.Send(ref updateObject);



                    var pChar = session.Character;
                    var count = 0;

                    if (count > 0)
                    {
                        updateObject = new PacketWriter(ServerMessage.ObjectUpdate);
                        bp = new BitPack(updateObject);

                        updateObject.WriteUInt32(1);

                        updateObject.WriteUInt16((ushort)pChar.Map);
                        bp.Write(true);
                        bp.Flush();
                        updateObject.WriteInt32(0);

                        updateObject.WriteUInt16((ushort)pChar.Map);
                        updateObject.WriteUInt32((uint)count);
                        var itemGuid = new SmartGuid(removedItem.Guid, removedItem.Id, GuidType.Item);
                        updateObject.WriteSmartGuid(itemGuid);

                        //var size = (uint)updateObject.BaseStream.Length - 15;
                        //updateObject.WriteUInt32Pos(size, 11);

                        session.Send(ref updateObject);
                    }

                    //ObjectHandler.HandleDestroyObject(ref sess1,  true);
                }
            }

            sess1.Character = session.Character;
        }

        public static byte GetEquipSlot(byte equipSlot, Character pChar)
        {
            switch (equipSlot)
            {
                case 11:
                    equipSlot = (int)EquipmentSlots.Finger1;
                    break;
                case 12:
                    equipSlot = (int)EquipmentSlots.Trinket1;
                    break;
                case 13:
                    equipSlot = (int)EquipmentSlots.MainHand;
                    if (pChar.Equipment.ContainsKey((int)EquipmentSlots.MainHand))
                    {
                        equipSlot = (int)EquipmentSlots.OffHand;
                    }

                    break;
                case 14:
                    equipSlot = (int)EquipmentSlots.OffHand;

                    break;
                case 15:
                    equipSlot = (int)EquipmentSlots.MainHand;
                    break;
                case 16:
                    equipSlot = (int)EquipmentSlots.Back;
                    break;
                case 17:
                    equipSlot = (int)EquipmentSlots.MainHand;

                    if (pChar.Class == 1 && pChar.Equipment.ContainsKey(equipSlot))
                        equipSlot = (int)EquipmentSlots.OffHand;
                    break;
                case 18: // bags
                    equipSlot = (int)EquipmentSlots.BagSlot0;

                    if (pChar.Equipment.ContainsKey(equipSlot))
                    {
                        equipSlot = (int)EquipmentSlots.BagSlot1;
                    }
                    else if (pChar.Equipment.ContainsKey((int)EquipmentSlots.BagSlot1))
                    {
                        equipSlot = (int)EquipmentSlots.BagSlot2;
                    }
                    else if (pChar.Equipment.ContainsKey((int)EquipmentSlots.BagSlot2))
                    {
                        equipSlot = (int)EquipmentSlots.BagSlot3;
                    }
                    else if (pChar.Equipment.ContainsKey((int)EquipmentSlots.BagSlot3))
                    {
                        equipSlot = (int)EquipmentSlots.BagSlot3;
                    }

                    break;
                case 19:
                    equipSlot = (int)EquipmentSlots.Tabard;
                    break;
                case 20:
                    equipSlot = (int)EquipmentSlots.Chest;
                    break;
                case 21:
                    equipSlot = (int)EquipmentSlots.MainHand;
                    break;
                case 22:
                case 23:
                    equipSlot = (int)EquipmentSlots.OffHand;
                    break;
                case 24: // removed?
                    return 24;
                case 25:
                case 26:
                case 27:
                case 28:
                    equipSlot = (int)EquipmentSlots.MainHand;
                    break;
                default:
                    equipSlot -= 1;
                    break;
            }

            return equipSlot;
        }
    }
}
