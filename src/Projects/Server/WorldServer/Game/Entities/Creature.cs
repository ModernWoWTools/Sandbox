using System;
using System.Collections.Generic;

namespace AuthServer.WorldServer.Game.Entities
{
    [Serializable()]
    public class Creature
    {
        public int Id;
        public string Name;
        public string SubName;
        public string Title;
        public string IconName;
        public List<uint> Flag = new List<uint>(2);
        public int Type;
        public int Family;
        public int Rank;
        public List<int> QuestKillNpcId = new List<int>(2);
        public List<(int displayId, float, float)> DisplayInfoId = new List<(int displayId, float, float)>();
        public float Unknown;
        public float HealthModifier;
        public float PowerModifier;
        public bool RacialLeader;
        public List<int> QuestItemId = new List<int>();
        public int MovementInfoId;
        public int ExpansionRequired;
        public int Unknown2;
        public int Unknown3;
        public int Unknown4;
        public int Unknown5;
    }
}
