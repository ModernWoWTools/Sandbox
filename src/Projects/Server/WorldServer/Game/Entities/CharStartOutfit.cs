namespace AuthServer.WorldServer.Game.Entities
{
    public class CharStartOutfit
    {
        public uint Id { get; set; }
        public uint[] ItemId { get; set; } = new uint[24];
        public uint PetDisplayId { get; set; }
        public byte RaceId { get; set; }
        public byte ClassId { get; set; }
        public byte SexId { get; set; }
        public byte OutfitId { get; set; }
        public byte PetFamilyId { get; set; }
    }

    public class Namegen
    {
        public string Name { get; set; }
        public byte Race { get; set; }
        public  byte Sex { get; set; }
    }
}
