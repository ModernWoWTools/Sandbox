/*
 * Copyright (C) 2012-2014 Arctium <http://arctium.org>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using Framework.ObjectDefines;
using Framework.Misc;
using AuthServer.Game.Entities;
using AuthServer.WorldServer.Game.Entities;
using AuthServer.WorldServer.Managers;
using Framework.Serialization;
using System.IO;
using System.Reflection;
using Framework.Logging;
using Framework.Constants.Misc;
using System.Linq;
using System.IO.Compression;

namespace AuthServer.Game.Managers
{
    [System.Reflection.Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = true)]
    public sealed class ObjectManager : Singleton<ObjectManager>
    {
        Dictionary<ulong, WorldObject> objectList;
        public Dictionary<int, Creature> Creatures;
        public Dictionary<int, GameObject> GameObjects;
        public Dictionary<int, GameObject> GameObjectSpawns;

        ObjectManager()
        {
            objectList = new Dictionary<ulong, WorldObject>();
            Creatures = new Dictionary<int, Creature>();
            GameObjects = new Dictionary<int, GameObject>();
            GameObjectSpawns = new Dictionary<int, GameObject>();
            LoadCreatureStats();
            LoadGameobjectStats();
        }

        [Obfuscation(Feature = "virtualization", Exclude = true)]
        void LoadGameobjectStats()
        {
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Arctium.WoW.Sandbox.Server.Resources.gameobjects.txt")))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Split(new string[] { ",\"" }, StringSplitOptions.None).Select(s => s.Replace(@"""", "")).ToArray();
                    GameObject Stats = new GameObject();
                    {
                        Stats.Id = int.Parse(line[0].Replace(@"""", ""));
                        Stats.Name = line[1].Replace(@"""", "");
                        Stats.Pos.X = float.Parse(line[2].Replace(@"""", ""));
                        Stats.Pos.Y = float.Parse(line[3].Replace(@"""", ""));
                        Stats.Pos.Z = float.Parse(line[4].Replace(@"""", ""));
                        //Stats.Pos.O = float.Parse(line[8].Replace(@"""", ""));

                        Stats.Rot.X = float.Parse(line[5].Replace(@"""", ""));
                        Stats.Rot.Y = float.Parse(line[6].Replace(@"""", ""));
                        Stats.Rot.Z = float.Parse(line[7].Replace(@"""", ""));
                        Stats.Rot.O = float.Parse(line[8].Replace(@"""", ""));

                        Stats.Map = int.Parse(line[9].Replace(@"""", ""));
                        Stats.DisplayInfoId = int.Parse(line[10].Replace(@"""", ""));
                        Stats.Size = float.Parse(line[11].Replace(@"""", ""));
                        Stats.Type = int.Parse(line[12].Replace(@"""", ""));
                        Stats.Flags = int.Parse(line[13].Replace(@"""", ""));

                        Stats.IconName = "";
                        Stats.CastBarCaption = ""; //result.Read<string>(0, "CastBarCaption");

                        for (var i = 0; i < 34; i++)
                            Stats.Data.Add(0);

                        Stats.Data[0] = int.Parse(line[16].Replace(@"""", ""));
                        Stats.Data[1] = int.Parse(line[17].Replace(@"""", ""));
                        Stats.Data[2] = int.Parse(line[18].Replace(@"""", ""));
                        Stats.Data[3] = int.Parse(line[19].Replace(@"""", ""));
                        Stats.Data[4] = int.Parse(line[20].Replace(@"""", ""));
                        Stats.Data[5] = int.Parse(line[21].Replace(@"""", ""));
                        Stats.Data[6] = int.Parse(line[22].Replace(@"""", ""));
                        Stats.Data[7] = int.Parse(line[23].Replace(@"""", ""));


                        Stats.ExpansionRequired = 0; //result.Read<int>(0, "ExpansionRequired");

                        // for (int i = 0; i < Stats.Flag.Capacity; i++)
                        //Stats.Flag.Add(uint.Parse(line[4 + i].Replace(@"""", "")));
                    }
                    // for (int i = 0; i < Stats.DisplayInfoId.Capacity; i++)
                    //   Stats.DisplayInfoId.Add(int.Parse(line[10 + i].Replace(@"""", "")));
                    //if (Stats.Pos.X != 0)
                    GameObjectSpawns.Add(Stats.Id, Stats);

                    //AvailableDisplayIds.TryAdd(int.Parse(appearanceId), int.Parse(displayId));
                }
            }

            Log.Message(LogType.Debug, "Added {0} Gameobject spawns.", GameObjectSpawns.Count);

            // 2nd...
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Arctium.WoW.Sandbox.Server.Resources.gameobjectstats.csv")))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Split(new char[] { ';' });
                    GameObject Stats = new GameObject();
                    {
                        Stats.Id = int.Parse(line[0].Replace(@"""", ""));
                        Stats.Type = int.Parse(line[1].Replace(@"""", ""));
                        Stats.DisplayInfoId = int.Parse(line[2].Replace(@"""", ""));
                        if (line.Length == 42)
                        {
                            Stats.Name = line[3].Replace(@"""", "").TrimStart();
                            Stats.IconName = line[4].Replace(@"""", "");
                            Stats.CastBarCaption = line[5].Replace(@"""", ""); //result.Read<string>(0, "CastBarCaption");
                            Stats.Unk = line[6].Replace(@"""", "");

                            for (var i = 0; i < 34; i++)
                                Stats.Data.Add(int.Parse(line[7 + i].Replace(@"""", "")));

                            Stats.Size = float.Parse(line[41].Replace(@"""", ""));
                        }
                        else if (line.Length == 44)
                        {
                            Stats.Name = line[3].Replace(@"""", "").TrimStart();
                            Stats.IconName = line[4].Replace(@"""", "");
                            Stats.CastBarCaption = line[5].Replace(@"""", ""); //result.Read<string>(0, "CastBarCaption");
                            Stats.Unk = line[6].Replace(@"""", "");
                            Stats.Size = float.Parse(line[7].Replace(@"""", ""));

                            for (var i = 0; i < 34; i++)
                                Stats.Data.Add(int.Parse(line[8 + i].Replace(@"""", "")));
                        }


                        //Stats.Pos = null;
                        /*Stats.Map = int.Parse(line[18].Replace(@"""", ""));
                        Stats.Pos.X = float.Parse(line[1].Replace(@"""", ""));
                        Stats.Pos.Y = float.Parse(line[2].Replace(@"""", ""));
                        Stats.Pos.Z = float.Parse(line[3].Replace(@"""", ""));
                        Stats.Pos.O = float.Parse(line[8].Replace(@"""", ""));

                        Stats.Rot.X = float.Parse(line[4].Replace(@"""", ""));
                        Stats.Rot.Y = float.Parse(line[5].Replace(@"""", ""));
                        Stats.Rot.Z = float.Parse(line[6].Replace(@"""", ""));
                        Stats.Rot.O = float.Parse(line[7].Replace(@"""", ""));*/

                        Stats.ExpansionRequired = 0;// int.Parse(line[8 + Stats.Data.Capacity - 1].Replace(@"""", ""));


                        Stats.Flags = 0; //result.Read<int>(0, "Flags");


                        //Stats.Data[0] = int.Parse(line[20].Replace(@"""", ""));
                        //Stats.Data[1] = int.Parse(line[21].Replace(@"""", ""));
                        //Stats.Data[2] = int.Parse(line[22].Replace(@"""", ""));


                        // for (int i = 0; i < Stats.Flag.Capacity; i++)
                        //Stats.Flag.Add(uint.Parse(line[4 + i].Replace(@"""", "")));
                    }
                    // for (int i = 0; i < Stats.DisplayInfoId.Capacity; i++)
                    //   Stats.DisplayInfoId.Add(int.Parse(line[10 + i].Replace(@"""", "")));
                    //if (Stats.Pos.X != 0)
                    if (!GameObjects.ContainsKey(Stats.Id))
                        GameObjects.Add(Stats.Id, Stats);

                    //AvailableDisplayIds.TryAdd(int.Parse(appearanceId), int.Parse(displayId));
                }
            }
#if PUBLIC == false
            if (File.Exists("./gameobjectcache.csv"))
                using (var reader = new StreamReader(File.OpenRead("./gameobjectcache.csv")))
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine().Split(new char[] { ';' });
                        GameObject Stats = new GameObject();
                        {
                            Stats.Id = int.Parse(line[0].Replace(@"""", ""));
                            Stats.Type = int.Parse(line[1].Replace(@"""", ""));
                            Stats.DisplayInfoId = int.Parse(line[2].Replace(@"""", ""));
                            Stats.Name = line[3].Replace(@"""", "").TrimStart();
                            // 4
                            // 5
                            // 6
                            Stats.IconName = line[7].Replace(@"""", "");
                            Stats.CastBarCaption = line[8].Replace(@"""", ""); //result.Read<string>(0, "CastBarCaption");
                            Stats.Unk = line[9].Replace(@"""", "");

                            for (var i = 0; i < 34; i++)
                                Stats.Data.Add(int.Parse(line[10 + i].Replace(@"""", "")));

                            Stats.Size = float.Parse(line[44].Replace(@"""", ""));



                            Stats.ExpansionRequired = 0;// int.Parse(line[8 + Stats.Data.Capacity - 1].Replace(@"""", ""));


                            Stats.Flags = 0;//result.Read<int>(0, "Flags");


                            //Stats.Data[0] = int.Parse(line[20].Replace(@"""", ""));
                            //Stats.Data[1] = int.Parse(line[21].Replace(@"""", ""));
                            //Stats.Data[2] = int.Parse(line[22].Replace(@"""", ""));


                            // for (int i = 0; i < Stats.Flag.Capacity; i++)
                            //Stats.Flag.Add(uint.Parse(line[4 + i].Replace(@"""", "")));
                        }
                        // for (int i = 0; i < Stats.DisplayInfoId.Capacity; i++)
                        //   Stats.DisplayInfoId.Add(int.Parse(line[10 + i].Replace(@"""", "")));
                        if (GameObjects.ContainsKey(Stats.Id))
                            GameObjects[Stats.Id] = Stats;
                        else
                            GameObjects.Add(Stats.Id, Stats);

                        //AvailableDisplayIds.TryAdd(int.Parse(appearanceId), int.Parse(displayId));
                    }
#endif

            Log.Message(LogType.Debug, "Added {0} Gameobject stats.", GameObjects.Count);
        }

        [Obfuscation(Feature = "virtualization", Exclude = true)]
        void LoadCreatureStats()
        {
            using (var zipArchive = new ZipArchive(Assembly.GetExecutingAssembly().GetManifestResourceStream("Arctium.WoW.Sandbox.Server.Resources.creaturestats.zip"), ZipArchiveMode.Read))
            {
                var entry = zipArchive.Entries.First(n => n.FullName.Equals("creaturestats.txt"));

                using (var stream = entry.Open())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine().Split(new char[] { ';' });
                            Creature Stats = new Creature();
                            var pos = 0;

                            Stats.Id = int.Parse(line[pos++].Replace(@"""", ""));
                            Stats.RacialLeader = false;
                            Stats.Name = line[pos++].Replace(@"""", "");

                            // Skip other name fields.
                            pos += 7;

                            for (int i = 0; i < Stats.Flag.Capacity; i++)
                                Stats.Flag.Add((uint)uint.Parse(line[pos++].Replace(@"""", "")));

                            Stats.Type = int.Parse(line[pos++].Replace(@"""", ""));
                            Stats.Family = int.Parse(line[pos++].Replace(@"""", ""));
                            Stats.Rank = int.Parse(line[pos++].Replace(@"""", ""));

                            Stats.QuestItemId.Add(int.Parse(line[pos++].Replace(@"""", "")));
                            Stats.QuestItemId.Add(int.Parse(line[pos++].Replace(@"""", "")));

                            var displayCount = int.Parse(line[pos++].Replace(@"""", ""));
                            Stats.DisplayInfoId.Clear();
                            Stats.DisplayInfoId = new List<(int displayId, float, float)>(displayCount);

                            Stats.Unknown = float.Parse(line[pos++].Replace(@"""", ""));
                            Stats.HealthModifier = float.Parse(line[pos++].Replace(@"""", ""));
                            Stats.PowerModifier = float.Parse(line[pos++].Replace(@"""", ""));

                            // Skip quest item count.
                            pos++;

                            Stats.MovementInfoId = int.Parse(line[pos++].Replace(@"""", ""));

                            pos++;

                            Stats.ExpansionRequired = int.Parse(line[pos++].Replace(@"""", ""));

                            Stats.Unknown2 = int.Parse(line[pos++].Replace(@"""", ""));
                            Stats.Unknown3 = int.Parse(line[pos++].Replace(@"""", ""));
                            Stats.Unknown4 = int.Parse(line[pos++].Replace(@"""", ""));
                            Stats.Unknown5 = 0;//int.Parse(line[pos++].Replace(@"""", ""));
                                               //pos++;
                            pos++;

                            Stats.SubName = line[pos++].Replace(@"""", "");
                            Stats.Title = line[pos++].Replace(@"""", "");
                            Stats.IconName = line[pos++].Replace(@"""", "");


                            for (int i = 0; i < displayCount; i++)
                            {
                                if (pos >= line.Length - 4)
                                    break;

                                int.TryParse(line[pos++].Replace(@"""", ""), out var n1);
                                float.TryParse(line[pos++].Replace(@"""", ""), out var n2);
                                float.TryParse(line[pos++].Replace(@"""", ""), out var n3);

                                var displayData = (n1, n2, n3);
                                Stats.DisplayInfoId.Add(displayData);
                            }
                            if (!Creatures.ContainsKey(Stats.Id))
                                Creatures.Add(Stats.Id, Stats);
                        }
                    }
                }
            }

            Log.Message(LogType.Debug, "Added {0} creature stats.", Creatures.Count);
        }

        public WorldObject FindObject(ulong guid)
        {
            foreach (KeyValuePair<ulong, WorldObject> kvp in objectList)
                if (kvp.Key == guid)
                    return kvp.Value;

            return null;
        }

        public void SetPosition(ref Character pChar, Vector4 vector, bool dbUpdate = true)
        {
            pChar.Position = vector;

            Manager.WorldMgr.Sessions2[0].Character = pChar;

            SavePositionToDB(pChar);
        }

        public void SetMap(ref Character pChar, uint mapId, bool dbUpdate = true)
        {
            pChar.Map = mapId;

            Manager.WorldMgr.Sessions2[0].Character = pChar;

            SavePositionToDB(pChar);
        }

        public void SetZone(ref Character pChar, uint zoneId, bool dbUpdate = true)
        {
            pChar.Zone = zoneId;

            Manager.WorldMgr.Sessions2[0].Character = pChar;

            SaveZoneToDB(pChar);
        }

        public void SaveChar(Character pChar)
        {
            for (int i = 0; i < Manager.WorldMgr.CharaterList.Count; i++)
            {
                if (Manager.WorldMgr.CharaterList[i].Guid == pChar.Guid)
                {
                    Manager.WorldMgr.CharaterList[i] = pChar;
                    break;
                }
            }

            File.WriteAllText(Helper.DataDirectory() + "characters.json", Json.CreateString(Manager.WorldMgr.CharaterList));
        }

        public void SavePositionToDB(Character pChar)
        {
            for (int i = 0; i < Manager.WorldMgr.CharaterList.Count; i++)
            {
                if (Manager.WorldMgr.CharaterList[i].Guid == pChar.Guid)
                {
                    Manager.WorldMgr.CharaterList[i].Position = pChar.Position;
                    break;
                }
            }

            File.WriteAllText(Helper.DataDirectory() + "characters.json", Json.CreateString(Manager.WorldMgr.CharaterList));
        }

        public void SaveZoneToDB(Character pChar)
        {
            File.WriteAllText(Helper.DataDirectory() + "characters.json", Json.CreateString(Manager.WorldMgr.CharaterList));
        }
    }
}
