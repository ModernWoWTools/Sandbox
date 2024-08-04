using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AuthServer.WorldServer.Game.Entities
{
    [Serializable()]
    [DataContract]
    public class Item
    {
        [DataMember]
        public ulong Guid { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public Dictionary<string, Tuple<int, int>> DisplayInfoIds { get; set; } = new Dictionary<string, Tuple<int, int>>();
        [DataMember]
        public int DisplayInfoId { get; set; }
        [DataMember]
        public byte InventoryType { get; set; }
        [DataMember]
        public int ModId { get; set; }
        [DataMember]
        public byte BagSlot { get; set; }
        [DataMember]
        public int NumSlots { get; set; }
        [DataMember]
        public int UsedSlots { get; set; }

        public Item Clone() => MemberwiseClone() as Item;
    }
}
