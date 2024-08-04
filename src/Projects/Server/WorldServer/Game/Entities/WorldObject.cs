using Framework.Network.Packets;
using Framework.ObjectDefines;
using System;
using System.Collections;
using AuthServer.Game.Entities;
using System.Runtime.Serialization;
using System.IO;

namespace AuthServer.WorldServer.Game.Entities
{
    [Serializable()]
    [DataContract]
    public class WorldObject
    {
        public SmartGuid SGuid;
        // General object data
        [DataMember]
        public ulong Guid;
        [DataMember]
        public Vector4 Position;
        [DataMember]
        public uint Map;

        public int MaskSize;
        [DataMember]
        public int DataLength;
        public BitArray Mask;
        public Hashtable UpdateData = new Hashtable();

        public WorldObject() { }

        public WorldObject(int dataLength)
        {
            DataLength = dataLength;
            MaskSize = (dataLength + 32 - 1) / 32;
            Mask = new BitArray(dataLength, false);
        }

        public bool CheckDistance(WorldObject obj, float dist = 100000)
        {
            if (Map == obj.Map)
            {
                float disX = (float)Math.Pow(Position.X - obj.Position.X, 2);
                float disY = (float)Math.Pow(Position.Y - obj.Position.Y, 2);
                float disZ = (float)Math.Pow(Position.Z - obj.Position.Z, 2);

                float distance = disX + disY + disZ;

                return distance <= dist;
            }

            return false;
        }

        public virtual void SetUpdateFields() { }

        public void SetItemUpdateFields(SmartGuid guid, int itemId = 0, int modId = 0)
        {
            SetUpdateField<ulong>((int)ObjectFields.Guid, guid.Guid);
            //SetUpdateField<uint>((int)ObjectFields.Guid + 2, 0x00000000);
            SetUpdateField<ulong>((int)ObjectFields.Guid + 2, guid.HighGuid);
            if (itemId == 158075)
            {
                //SetUpdateField<int>((int)ObjectFields.HeirTypeFlags, 0x13);
            }
            else if (itemId == 156829)
            {
                //SetUpdateField<int>((int)ObjectFields.HeirTypeFlags, 0xB);
            }
            else if (itemId == 114821)
            {
                //SetUpdateField<int>((int)ObjectFields.HeirTypeFlags, 0x7);
                SetUpdateField((int)ItemFields.DynamicFlags, 0x30001u);
            }
            //else
                //SetUpdateField<int>((int)ObjectFields.HeirTypeFlags, 0x3);


            SetUpdateField<int>((int)ObjectFields.EntryID, itemId);
            SetUpdateField<float>((int)ObjectFields.Scale, 1.0f);
            // SetUpdateField<ulong>((int)ObjectFields.Guid, Guid);
            //SetUpdateField<uint>((int)ObjectFields.Guid + 2, 0x00000000);
            //SetUpdateField<ulong>((int)ObjectFields.Guid + 2, 0x080F280000000000);

            var guid2 = new SmartGuid { Type = GuidType.Player, CreationBits = Guid, RealmId = 1 };
            SetUpdateField<ulong>((int)ItemFields.Owner, guid2.Guid);
            SetUpdateField<ulong>((int)ItemFields.Owner + 2, guid2.HighGuid);

            SetUpdateField<ulong>((int)ItemFields.ContainedIn, guid2.Guid);
            SetUpdateField<ulong>((int)ItemFields.ContainedIn + 2, guid2.HighGuid);
            SetUpdateField<uint>((int)ItemFields.StackCount, 1);
            SetUpdateField<uint>((int)ItemFields.DynamicFlags, 196609);

            for (var i = 0; i < 49; i++)
                SetUpdateField((int)ItemFields.Enchantment + i, 0u);

            if (modId != 0)
                SetUpdateField((int)ItemFields.ItemAppearanceModID, modId);

            if (itemId == 156829)
                SetAzeriteUpdateFields(guid);

            if (itemId == 158075)
                SetAzeriteUpdate2Fields();

            if (itemId == 114821)
            {
                //for (var i = 0; i < 144; i++)
                //    SetUpdateField((int)ContainerFields.Slots + i, 0u);

                //SetUpdateField((int)ContainerFields.NumSlots - 1, (uint)30);
                SetUpdateField((int)ContainerFields.NumSlots, (uint)30);

                // set test item
                
                //ObjectHandler.HandleUpdateObjectValues(ref session);
            }
        }

        public void SetAzeriteUpdateFields(SmartGuid guid)
        {
            /*SetUpdateField((int)CGAzeriteItemData.Xp, 500);
            SetUpdateField((int)CGAzeriteItemData.AuraLevel, 2);
            SetUpdateField((int)CGAzeriteItemData.KnowledgeLevel, 2);
            SetUpdateField((int)CGAzeriteItemData.Level, 2);
            SetUpdateField((int)CGAzeriteItemData.DEBUGknowledgeWeek, 2);*/
            SetUpdateField<ulong>((int)CGAzeriteEmpoweredItemData.Selections, 1337);
            //SetUpdateField<uint>((int)ObjectFields.Guid + 2, 0x00000000);
            SetUpdateField<ulong>((int)CGAzeriteEmpoweredItemData.Selections + 2, 1337);
            //SetUpdateField((int)CGAzeriteEmpoweredItemData.Selections, 1);
        }
        public void SetAzeriteUpdate2Fields()
        {
            SetUpdateField((int)CGAzeriteItemData.Xp, 5000);
            SetUpdateField((int)CGAzeriteItemData.AuraLevel, 10);
            SetUpdateField((int)CGAzeriteItemData.KnowledgeLevel, 10);
            SetUpdateField((int)CGAzeriteItemData.Level, 10);
            SetUpdateField((int)CGAzeriteItemData.DEBUGknowledgeWeek, 10);

            //SetUpdateField((int)CGAzeriteEmpoweredItemData.Selections, 2);
        }
        public void SetContainerFields(int numSlots)
        {
            SetUpdateField((int)ContainerFields.NumSlots, (uint)numSlots);
        }

        public void SetUpdateField<T>(int index, T value, byte offset = 0)
        {
        }


        public void WriteUpdateFields(ref PacketWriter packet)
        {
        }

        public void WriteDynamicUpdateFields(ref PacketWriter packet)
        {
        }

        public void RemoveFromWorld()
        {

        }

        public Character ToCharacter()
        {
            return this as Character;
        }
    }
}
