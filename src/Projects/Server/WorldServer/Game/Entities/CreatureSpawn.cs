using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using AuthServer.WorldServer.Managers;
using Framework.ObjectDefines;

namespace AuthServer.WorldServer.Game.Entities
{
    [Serializable()]
    [DataContract]
    public class Vector3
    {
        [DataMember]
        public float X { get; set; }
        [DataMember]
        public float Y { get; set; }
        [DataMember]
        public float Z { get; set; }
    }
    public class Waypoint
    {
        public ulong CreatureGuid { get; set; }
        public Framework.ObjectDefines.Vector4 Point { get; set; }
        public int Index { get; set; }
        public int WaitTime { get; set; }
    }
    public class SplineMove
    {
        public uint Flags { get; set; }
        public int Elapsed { get; set; }
        public uint Duration { get; set; }
        public float DurationModifier { get; set; }
        public float NextDurationModifier { get; set; }
        public byte Face { get; set; }
        public byte Mode { get; set; }
        public Vector3[] Points { get; set; }

    }
    public sealed class CreatureSpline
    {
        public ulong Guid { get; set; }
        public uint Id { get; set; }
        public uint MovementFlags { get; set; }
        public Vector3 Destination { get; set; }
        public SplineMove Move { get; set; }
    }

    [Serializable()]
    public class CreatureEquip
    {
        public ulong Guid { get; set; }
        public List<Item> Items { get; set; } // Max 3
    }

    [Serializable()]
    public class CreatureSpawn : WorldObject
    {
        public int Id;
        public Creature Creature;
        public int Emote;
        public ConcurrentBag<Waypoint> Waypoints;
        public CreatureSpline Spline;

        // New
        public uint AnimState;
        public uint ModelId;
        public float Scale = 1;
        public int EquipmentId;
        public uint NpcFlags;
        public uint UnitFlags;
        public uint FactionTemplate;
        public uint Level;
        public uint MountDisplayId;
        public float HoverHeight;
        public long Health = 1;
        public long MaxHealth = 1;

        public CreatureSpawn(int updateLength = (int)UnitFields.End) : base(updateLength)
        {
            Waypoints = new ConcurrentBag<Waypoint>();
        }

        public static ulong GetLastGuid()
        {
            return Manager.SpawnMgr.CreatureSpawns.Keys.OrderByDescending(k => k).FirstOrDefault();
        }

        public void CreateFullGuid()
        {
            SGuid = new SmartGuid(Guid, Id, GuidType.Creature, Map);
            Guid = SGuid.Guid;
        }

        public void CreateData(Creature creature)
        {
            Creature = creature;
        }

        public void AddToWorld()
        {
            CreateFullGuid();
            CreateData(Creature);

            Manager.SpawnMgr.AddSpawn(this);
        }

        public override void SetUpdateFields()
        {
        }
    }
}
