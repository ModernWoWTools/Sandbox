using System;
using System.Collections.Generic;
using Framework.Misc;
using System.IO;
using AuthServer.WorldServer.Game.Entities;
using System.Collections.Concurrent;
using System.Reflection;
using Framework.Logging;

namespace AuthServer.WorldServer.Managers
{
    [System.Reflection.Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = true)]
    public class EquipmentManager : Singleton<EquipmentManager>
    {
        public ConcurrentDictionary<int, Item> AvailableItems = new ConcurrentDictionary<int, Item>();
        public ConcurrentDictionary<int, Item> ItemRefs = new ConcurrentDictionary<int, Item>();
        public ConcurrentDictionary<int, int> AvailableDisplayIds = new ConcurrentDictionary<int, int>();
        public ConcurrentDictionary<int, Dictionary<string, Tuple<int, int>>> AvailableDisplayIds2 = new ConcurrentDictionary<int, Dictionary<string, Tuple<int, int>>>();
        public List<CharStartOutfit> CharStartOutfits = new List<CharStartOutfit>();
        public List<Namegen> Namegens = new List<Namegen>();

        EquipmentManager()
        {
            LoadAvailableItmes();
        }

        string[] line;//, names;

        [Obfuscation(Feature = "virtualization", Exclude = true)]
        void LoadAvailableItmes()
        {
            //names = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Arctium.WoW.Sandbox.Server.Resources.charstartoutfits.txt")))
            {
                //names = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine().Split(new char[] { ',' });
                    
                    var csa = new CharStartOutfit();

                    csa.Id = uint.Parse(line[0]);
                    csa.ClassId = byte.Parse(line[1]); // 26
                    csa.SexId = byte.Parse(line[2]); // 27
                    csa.OutfitId = byte.Parse(line[3]); // 28
                    csa.PetDisplayId = uint.Parse(line[4]);
                    csa.PetFamilyId = byte.Parse(line[5]); // 29

                    for (var i = 1; i <= csa.ItemId.Length; i++)
                        csa.ItemId[i - 1] = uint.Parse(line[i + 5]);

                    csa.RaceId = byte.Parse(line[30]); // 30

                    CharStartOutfits.Add(csa);
                }
            }

            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Arctium.WoW.Sandbox.Server.Resources.itemappearance.txt")))
            {
                //names = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine().Split(new char[] { ',' });
                    var displayId = line[2].Replace(@"""", "");
                    var appearanceId = line[0].Replace(@"""", "");

                    AvailableDisplayIds.TryAdd(int.Parse(appearanceId), int.Parse(displayId));
                }
            }

            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Arctium.WoW.Sandbox.Server.Resources.ItemModifiedAppearance.txt")))
            {
                //names = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine().Split(new char[] {','});
                    var itemId = line[1].Replace(@"""", "");
                    if (int.Parse(line[3].Replace(@"""", "")) != 0)
                    {
                        if (!AvailableDisplayIds.TryGetValue(int.Parse(line[3].Replace(@"""", "")), out var displayId))
                            continue;

                        if (!AvailableDisplayIds2.ContainsKey(int.Parse(itemId)))
                        {
                            AvailableDisplayIds2.TryAdd(int.Parse(itemId), new Dictionary<string, Tuple<int, int>>());
                        }

                        var itemVersion = "normal";

                        switch (int.Parse(line[2].Replace(@"""", "")))
                        {
                            case 0:
                                itemVersion = "normal";
                                break;
                            case 1:
                                itemVersion = "heroic";
                                break;
                            case 3:
                                itemVersion = "mythic";
                                break;
                            case 4:
                                itemVersion = "lfr";
                                break;
                            default:
                                itemVersion = line[2].Replace(@"""", "");// throw new InvalidDataException("Invalid item version");
#if !PUBLIC
                                Log.Message(Framework.Constants.Misc.LogType.Debug, $"Item '{itemId}' unknown item version '{itemVersion}'.");
#endif
                                break;
                        }


                        AvailableDisplayIds2[int.Parse(itemId)].Add(itemVersion, Tuple.Create(displayId, int.Parse(line[2].Replace(@"""", ""))));
                    }
                }
            }

            var withDID = 0;
            var woDID = 0;
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Arctium.WoW.Sandbox.Server.Resources.Item.txt")))
            {
                //names = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine().Split(new char[] { ',' });
                    var itemId = line[0].Replace(@"""", "");
                    var slot = line[4].Replace(@"""", "");
                    var id = int.Parse(itemId);
					Dictionary<string, Tuple<int, int>> displayId;
                    if (AvailableDisplayIds2.TryGetValue(id, out displayId))
                    {
                        ++withDID;
                        AvailableItems.TryAdd(id, new Item {Id = id, DisplayInfoIds = AvailableDisplayIds2[id], InventoryType = byte.Parse(slot)});
                    }
                    else
                    {
                        ++woDID;
                        AvailableItems.TryAdd(id, new Item {Id = id, InventoryType = byte.Parse(slot)});
                    }
                }
            }

            /*using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Arctium.WoW.Sandbox.Server.Resources.itemreferences.txt")))
            {
                //names = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine().Split(new char[] { ';' });
                    var itemId = int.Parse(line[0].Replace(@"""", ""));
                    var refitemId = int.Parse(line[1].Replace(@"""", ""));
                    
                    int displayId;
                    if (AvailableDisplayIds2.TryGetValue(refitemId, out displayId))
                        AvailableItems.TryAdd(itemId, new Item { Id = itemId, DisplayInfoId = AvailableDisplayIds2[refitemId], InventoryType = AvailableItems[refitemId].InventoryType });
                    //else
                    //    AvailableItems.TryAdd(id, new Item { Id = id, DisplayInfoId = 0, InventoryType = byte.Parse(slot) });
                }
            }*/

            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Arctium.WoW.Sandbox.Server.Resources.namegens.txt")))
            {
                //names = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine().Split(new char[] { ',' });
                    var name = line[1].Replace(@"""", "");
                    var race = byte.Parse(line[2].Replace(@"""", ""));
                    var sex = byte.Parse(line[3].Replace(@"""", ""));

                    Namegens.Add(new Namegen {Name = name, Race = race, Sex = sex});
                    //else
                    //    AvailableItems.TryAdd(id, new Item { Id = id, DisplayInfoId = 0, InventoryType = byte.Parse(slot) });
                }
            }

            var ditem = AvailableItems[112454];

            Log.Message(Framework.Constants.Misc.LogType.Debug, "{0} items with display id loaded", withDID);
            Log.Message(Framework.Constants.Misc.LogType.Debug, "{0} items without loaded", woDID);
            //AvailableItems.Add(new Item { Id = 17, InventoryType = 4 });
        }

        public string GetEmbeddedResource(string ns, string res)
        {
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format("{0}.{1}", ns, res))))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
