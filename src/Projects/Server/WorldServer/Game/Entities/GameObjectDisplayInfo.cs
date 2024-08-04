namespace Arctium.WoW.Sandbox.Server.WorldServer.Game.Entities
{
    public class GameObjectDisplayInfo
    {
        public uint Id { get; set; }
        public float[] GetBox { get; set; } = new float[6];
        public uint FileId { get; set; }
        public ushort ObjectEffectPackageID { get; set; }
        public float OverrideLootEffectScale { get; set; }
        public float OverrideNameScale { get; set; }
    }
}
