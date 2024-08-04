using System;
using System.Linq;
using AuthServer.WorldServer.Managers;
using Framework.ObjectDefines;

namespace AuthServer.WorldServer.Game.Entities
{
    [Serializable()]
    public class GameObjectSpawn : WorldObject
    {
        public int Id;
        public uint FactionTemplate;
        public byte AnimProgress;
        public byte State;
        public uint SpellVisualId;
        public GameObject GameObject;

        public GameObjectSpawn(int updateLength = (int)GameObjectFields.End) : base(updateLength) { }

        public static ulong GetLastGuid()
        {
            return Manager.SpawnMgr.GameObjectSpawns.Keys.OrderByDescending(k => k).FirstOrDefault();
        }

        public void CreateFullGuid()
        {
            SGuid = new SmartGuid(Guid, Id, GuidType.GameObject, Map);
            Guid = SGuid.Guid;
        }

        public void CreateData(GameObject gameobject)
        {
            GameObject = gameobject;
        }

        public bool AddToDB()
        {
            return false;
        }

        public void AddToWorld()
        {
            CreateFullGuid();
            CreateData(GameObject);

            Manager.SpawnMgr.AddSpawn(this);
            /*CreateFullGuid();
            CreateData(GameObject);

            Globals.SpawnMgr.AddSpawn(this, ref GameObject);

            WorldObject obj = this;
            UpdateFlag updateFlags = UpdateFlag.Rotation | UpdateFlag.StationaryPosition;

            foreach (var v in Globals.WorldMgr.Sessions)
            {
                Character pChar = v.Value.Character;

                if (pChar.CheckDistance(this))
                {
                    PacketWriter updateObject = new PacketWriter(ServerMessage.ObjectUpdate);

                    updateObject.WriteUInt16((ushort)Map);
                    updateObject.WriteUInt32(1);

                    Manager.WorldMgr.WriteCreateObject(ref updateObject, obj, updateFlags, ObjectType.GameObject);

                    v.Value.Send(ref updateObject);
                }
            }*/
        }

        public override void SetUpdateFields()
        {
            // ObjectFields
            SetUpdateField<ulong>((int)ObjectFields.Guid, Guid);
            //SetUpdateField<uint>((int)ObjectFields.Guid + 2, 0x00000000);
            var high = new SmartGuid(Guid, Id, GuidType.GameObject, Map);

            SetUpdateField<ulong>((int)ObjectFields.Guid + 2, high.HighGuid);//   280F00);
            //SetUpdateField<ulong>((int)ObjectFields.Data, 0);
            //SetUpdateField<int>((int)ObjectFields.HeirTypeFlags, 0x81);
            SetUpdateField<int>((int)ObjectFields.EntryID, Id);
            SetUpdateField<float>((int)ObjectFields.Scale, GameObject.Size);

            // GameObjectFields
            SetUpdateField<ulong>((int)GameObjectFields.CreatedBy, 0);
            SetUpdateField<int>((int)GameObjectFields.DisplayID, GameObject.DisplayInfoId);
            SetUpdateField<int>((int)GameObjectFields.Flags, 49);// GameObject.Flags);
            SetUpdateField<float>((int)GameObjectFields.ParentRotation, GameObject.Rot.X);
            SetUpdateField<float>((int)GameObjectFields.ParentRotation + 1, GameObject.Rot.Y);
            SetUpdateField<float>((int)GameObjectFields.ParentRotation + 2, GameObject.Rot.Z);
            SetUpdateField<float>((int)GameObjectFields.ParentRotation + 3, GameObject.Rot.O);
            SetUpdateField<byte>((int)GameObjectFields.StateSpellVisualID, AnimProgress);
            SetUpdateField<byte>((int)GameObjectFields.StateSpellVisualID, 0, 1);
            SetUpdateField<byte>((int)GameObjectFields.StateSpellVisualID, 255, 2);
            SetUpdateField<byte>((int)GameObjectFields.StateSpellVisualID, 255, 3);
            SetUpdateField<uint>((int)GameObjectFields.FactionTemplate, FactionTemplate);
            SetUpdateField<int>((int)GameObjectFields.Level, 0);
            SetUpdateField<byte>((int)GameObjectFields.PercentHealth, State);
            SetUpdateField<byte>((int)GameObjectFields.PercentHealth, (byte)GameObject.Type, 1);
            SetUpdateField<byte>((int)GameObjectFields.PercentHealth, 0, 2);
            SetUpdateField<byte>((int)GameObjectFields.PercentHealth, 255, 3);
        }
    }
}
