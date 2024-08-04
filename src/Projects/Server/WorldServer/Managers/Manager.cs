using Arctium.WoW.Sandbox.Server.WorldServer.Managers;

using AuthServer.Game.Managers;

namespace AuthServer.WorldServer.Managers
{
    public class Manager
    {
        public static ActionManager ActionMgr;
        public static ItemManager ItemMgr;
        public static ObjectManager ObjectMgr;
        public static SkillManager SkillMgr;
        public static SpellManager SpellMgr;
        public static SpawnManager SpawnMgr;
        public static WorldManager WorldMgr;
        public static EquipmentManager Equipments;
        public static HotfixManager Hotfix;

        public static void Initialize()
        {
            ActionMgr = ActionManager.GetInstance();
            ItemMgr = ItemManager.GetInstance();
            Equipments = EquipmentManager.GetInstance();
            SkillMgr = SkillManager.GetInstance();
            SpellMgr = SpellManager.GetInstance();
            ObjectMgr = ObjectManager.GetInstance();
            WorldMgr = WorldManager.GetInstance();
            SpawnMgr = SpawnManager.GetInstance();
            Hotfix = HotfixManager.GetInstance();
        }
    }
}
