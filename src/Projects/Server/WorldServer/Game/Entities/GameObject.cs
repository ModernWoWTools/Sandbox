using System;
using System.Collections.Generic;
using Framework.ObjectDefines;

namespace AuthServer.WorldServer.Game.Entities
{
    [Serializable()]
    [System.Reflection.Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = true)]
    public class GameObject
    {
        public int Map;
        public int Id;
        public int Type;
        public int Flags;
        public int DisplayInfoId;
        public Vector4 Pos = new Vector4();
        public Vector4 Rot = new Vector4();
        public string Name;
        public string IconName;
        public string CastBarCaption;
        public string Unk;
        public List<int> Data = new List<int>(34);
        public float Size;
        public List<int> QuestItemId = new List<int>(6);
        public int ExpansionRequired;
    }
}
