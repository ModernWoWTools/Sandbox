using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AuthServer.WorldServer.Game.Entities;
using Framework.Logging;
using Framework.Misc;
using Framework.ObjectDefines;

namespace AuthServer.WorldServer.Managers
{
    // Id,SourceText,ItemId,Flags,SourceTypeEnum
    public class Toy
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
    }
    [System.Reflection.Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = true)]
    public class ItemManager : Singleton<ItemManager>
    {
        Dictionary<byte, Item> equipment;
        Dictionary<byte, Dictionary<byte, Item>> bags;
        public List<Toy> Toys { get; set; }

        ItemManager()
        {
            Toys = new List<Toy>();

#if !PUBLIC
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Arctium.WoW.Sandbox.Server.Resources.Toy.csv")))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Split(new string[] { "\",\"" }, StringSplitOptions.None);
                    var toy = new Toy
                    {
                        Id = int.Parse(line[0].Replace("\"", "")),
                        ItemId = int.Parse(line[0].Replace("\"", ""))
                    };

                    Toys.Add(toy);
                }

                Log.Message(Framework.Constants.Misc.LogType.Normal, $"Loaded {Toys.Count} toys.");
            }
#endif
            equipment = new Dictionary<byte, Item>();
            bags = new Dictionary<byte, Dictionary<byte, Item>>
            {
                { 19, new Dictionary<byte, Item>(30) },
                { 20, new Dictionary<byte, Item>(30) },
                { 21, new Dictionary<byte, Item>(30) },
                { 22, new Dictionary<byte, Item>(30) },
                { 255, new Dictionary<byte, Item>(20) }  // Default bag
            };

            // Fill additional bags with null values.
            for (byte i = 0; i < 30; i++)
            {
                bags[19].Add(i, null);
                bags[20].Add(i, null);
                bags[21].Add(i, null);
                bags[22].Add(i, null);
            }

            // Fill the default bag with null values.
            for (byte i = 0; i < 20; i++)
                bags[255].Add(i, null);
        }

        public void Equip(byte equipSlot, Item item)
        {

        }

        public void UnEquip(byte equipSlot)
        {

        }

        public (byte Bag, byte Slot, SmartGuid Guid) Add(Item item)
        {
            var freeBag = bags.FirstOrDefault(b => b.Value.Any(bs => bs.Value == null));
            var freeSlot = freeBag.Value.FirstOrDefault(bs => bs.Value == null);

            // Assign guid
            item.Guid = GetNewGuid();

            bags[freeBag.Key][freeSlot.Key] = item;

            // Return a new guis used for the update object packet.
            return (freeBag.Key, freeSlot.Key, new SmartGuid(item.Guid, item.Id, GuidType.Item));
        }

        public void Remove(byte bag, byte slot)
        {
            bags[bag][slot] = null;
        }

        ulong GetNewGuid()
        {
            var orderedEquip = equipment.OrderBy(kp => kp.Value.Guid);
            var lastEquipGuid = orderedEquip.Any() ? orderedEquip.Last().Value.Guid : (ulong)new Random(Environment.TickCount).Next(10000);

            var lastBagGuid = 0UL;

            foreach (var b in bags)
            {
                foreach (var bs in b.Value)
                {
                    if (lastBagGuid < bs.Value?.Guid)
                        lastBagGuid = bs.Value.Guid;
                }
            }

            lastBagGuid = lastBagGuid > 0 ? lastBagGuid : (ulong)new Random(Environment.TickCount).Next(10000);

            return lastEquipGuid + lastBagGuid;
        }
    }
}
