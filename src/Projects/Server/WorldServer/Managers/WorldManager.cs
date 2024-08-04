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

using AuthServer.WorldServer.Game.Entities;
using AuthServer.WorldServer.Managers;
using Framework.Constants;
using Framework.Constants.Net;
using Framework.Misc;
using Framework.Network.Packets;
using Framework.ObjectDefines;
using System.Collections.Concurrent;
using System.Collections.Generic;
using AuthServer.Game.Entities;
using AuthServer.Network;
using System.Linq;
using System.Threading;
using System.IO;
using System.Reflection;
using System;
using System.Globalization;
using System.Text;
using Framework.Logging;
using AuthServer.WorldServer.Game.Packets.PacketHandler;
using Framework.Constants.Misc;
using AuthServer.Game.Packets.PacketHandler;
using Framework.Constants.Movement;

namespace AuthServer.Game.Managers
{
    public sealed class WorldManager : Singleton<WorldManager>
    {
        public List<Emote> EmoteList = new List<Emote>();
        public List<Character> CharaterList = new List<Character>();
        public List<Races> ChrRaces = new List<Races>();
        public ConcurrentDictionary<ulong, WorldClass> Sessions;
        public ConcurrentDictionary<ulong, WorldClass2> Sessions2;
        public WorldClass Session { get; set; }
        public Dictionary<uint, uint> SpellVisualIds = new Dictionary<uint, uint>();
        public Dictionary<uint, uint> SpellXVisualIds = new Dictionary<uint, uint>();

        public Dictionary<uint, uint> MountDisplays = new Dictionary<uint, uint>();
        public Dictionary<string, Tuple<uint, uint>> MountSpells = new Dictionary<string, Tuple<uint, uint>>();
        public Dictionary<uint, uint> DefaultPowerTypes = new Dictionary<uint, uint>();
        public Dictionary<uint, uint> DefaultChrSpec = new Dictionary<uint, uint>();
        public Dictionary<uint, List<uint>> ChrSpecs = new Dictionary<uint, List<uint>>();
        public List<(Character Character, ulong Guid, uint Emote)> PlayerSpawns = new List<(Character Character, ulong Guid, uint Emote)>();

        static readonly object taskObject = new object();
        public bool waitAdd;

        WorldManager()
        {
            Initialize();
        }

        [Obfuscation(Feature = "virtualization", Exclude = true)]
        void Initialize()
        {
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Arctium.WoW.Sandbox.Server.Resources.dbinfo.txt")))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Split(new char[] { ';' });
                    var fieldBytes = line[5].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var dbinfo = new global::DBInfo
                    {
                        Name = line[0].ToLower(),
                        TableHash = uint.Parse(line[1]),
                        HasIndex = bool.Parse(line[2]),
                        HasRefId = bool.Parse(line[3]),
                        IDPosition = int.Parse(line[4])
                    };

                    dbinfo.FieldTypes = new string[fieldBytes.Length];

                    for (var i = 0; i < fieldBytes.Length; i++)
                        dbinfo.FieldTypes[i] = fieldBytes[i];

                    DBInfo.Add(line[0].ToLower(), dbinfo);
                }
            }

            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Arctium.WoW.Sandbox.Server.Resources.SpellXSpellVisual.csv")))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Split(new char[] { ',' });

                    if (!SpellVisualIds.ContainsKey(uint.Parse(line[11].Replace("\"", ""))))
                        SpellVisualIds.Add(uint.Parse(line[11].Replace("\"", "")), uint.Parse(line[2].Replace("\"", "")));

                    if (!SpellXVisualIds.ContainsKey(uint.Parse(line[11].Replace("\"", ""))))
                        SpellXVisualIds.Add(uint.Parse(line[11].Replace("\"", "")), uint.Parse(line[0].Replace("\"", "")));


                }
            }

            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Arctium.WoW.Sandbox.Server.Resources.Mount.csv")))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Split(new char[] { ';' });

                    if (line.Length < 3)
                        continue;

                    int intTest;

                    if (!int.TryParse(line[0].Replace("\"", ""), out intTest))
                        continue;

                    try
                    {
                        var mountId = uint.Parse(line[0].Replace("\"", ""));
                        var spellId = uint.Parse(line.Length == 12 ? line[5].Replace("\"", "") : line[7].Replace("\"", ""));
                        var mountName = line[1].Replace("\"", "").Replace(" ", "");

                        var nameCounts = 2;


                        while (MountSpells.ContainsKey(mountName))
                            mountName += nameCounts++.ToString();

                        MountSpells.Add(mountName, Tuple.Create(mountId, spellId));
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

            }

            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Arctium.WoW.Sandbox.Server.Resources.MountXDisplay.csv")))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Split(new char[] { ',' });

                    try
                    {
                        var mountId = uint.Parse(line[3].Replace("\"", ""));
                        var mountDisplayId = uint.Parse(line[1].Replace("\"", ""));

                        if (!MountDisplays.ContainsKey(mountId))
                            MountDisplays.Add(mountId, mountDisplayId);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                Log.Message(LogType.Debug, $"Loaded {MountSpells.Count} mounts.");
            }

            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Arctium.WoW.Sandbox.Server.Resources.ChrClassesXPowerTypes.csv")))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Split(new char[] { ';' });

                    try
                    {
                        var classId = uint.Parse(line[1].Replace("\"", ""));
                        var powerType = uint.Parse(line[2].Replace("\"", ""));

                        // Only Add powertypes for the default spec.
                        if (!DefaultPowerTypes.ContainsKey(classId))
                            DefaultPowerTypes.Add(classId, powerType);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }

            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Arctium.WoW.Sandbox.Server.Resources.ChrSpecialization.csv")))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine().Split(new string[] { "\";\"" }, StringSplitOptions.None);

                    try
                    {
                        var specId = uint.Parse(line[0].Replace("\"", ""));
                        var classId = uint.Parse(line[6].Replace("\"", ""));

                        if (classId > 0)
                        {

                            if (!ChrSpecs.ContainsKey(classId))
                            {
                                ChrSpecs.Add(classId, new List<uint>());
                                ChrSpecs[classId].Add(specId);
                            }
                            else
                                ChrSpecs[classId].Add(specId);
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }

            // Add default spec ids.
            DefaultChrSpec.Add(1, 71);
            DefaultChrSpec.Add(2, 70);
            DefaultChrSpec.Add(3, 253);
            DefaultChrSpec.Add(4, 259);
            DefaultChrSpec.Add(5, 256);
            DefaultChrSpec.Add(6, 251);
            DefaultChrSpec.Add(7, 262);
            DefaultChrSpec.Add(8, 62);
            DefaultChrSpec.Add(9, 265);
            DefaultChrSpec.Add(10, 268);
            DefaultChrSpec.Add(11, 102);
            DefaultChrSpec.Add(12, 577);

            Sessions = new ConcurrentDictionary<ulong, WorldClass>();
            Sessions2 = new ConcurrentDictionary<ulong, WorldClass2>();

            LoadEmotes();
            LoadRaces();
            StartRangeUpdateTimers();
        }

        public void LoadEmotes()
        {
            EmoteList.Add(new Emote { Id = 3, EmoteId = 14 });
            EmoteList.Add(new Emote { Id = 5, EmoteId = 21 });
            EmoteList.Add(new Emote { Id = 6, EmoteId = 24 });
            EmoteList.Add(new Emote { Id = 8, EmoteId = 20 });
            EmoteList.Add(new Emote { Id = 12, EmoteId = 24 });
            EmoteList.Add(new Emote { Id = 17, EmoteId = 2 });
            EmoteList.Add(new Emote { Id = 19, EmoteId = 3 });
            EmoteList.Add(new Emote { Id = 20, EmoteId = 11 });
            EmoteList.Add(new Emote { Id = 21, EmoteId = 4 });
            EmoteList.Add(new Emote { Id = 22, EmoteId = 19 });
            EmoteList.Add(new Emote { Id = 23, EmoteId = 11 });
            EmoteList.Add(new Emote { Id = 24, EmoteId = 21 });
            EmoteList.Add(new Emote { Id = 25, EmoteId = 6 });
            EmoteList.Add(new Emote { Id = 26, EmoteId = 4 });
            EmoteList.Add(new Emote { Id = 28, EmoteId = 431 });
            EmoteList.Add(new Emote { Id = 31, EmoteId = 18 });
            EmoteList.Add(new Emote { Id = 32, EmoteId = 6 });
            EmoteList.Add(new Emote { Id = 33, EmoteId = 2 });
            EmoteList.Add(new Emote { Id = 34, EmoteId = 10 });
            EmoteList.Add(new Emote { Id = 35, EmoteId = 7 });
            EmoteList.Add(new Emote { Id = 37, EmoteId = 7 });
            EmoteList.Add(new Emote { Id = 41, EmoteId = 23 });
            EmoteList.Add(new Emote { Id = 43, EmoteId = 5 });
            EmoteList.Add(new Emote { Id = 45, EmoteId = 11 });
            EmoteList.Add(new Emote { Id = 47, EmoteId = 11 });
            EmoteList.Add(new Emote { Id = 48, EmoteId = 3 });
            EmoteList.Add(new Emote { Id = 51, EmoteId = 20 });
            EmoteList.Add(new Emote { Id = 52, EmoteId = 11 });
            EmoteList.Add(new Emote { Id = 53, EmoteId = 3 });
            EmoteList.Add(new Emote { Id = 55, EmoteId = 3 });
            EmoteList.Add(new Emote { Id = 58, EmoteId = 17 });
            EmoteList.Add(new Emote { Id = 59, EmoteId = 68 });
            EmoteList.Add(new Emote { Id = 60, EmoteId = 11 });
            EmoteList.Add(new Emote { Id = 61, EmoteId = 12 });
            EmoteList.Add(new Emote { Id = 65, EmoteId = 18 });
            EmoteList.Add(new Emote { Id = 66, EmoteId = 274 });
            EmoteList.Add(new Emote { Id = 67, EmoteId = 273 });
            EmoteList.Add(new Emote { Id = 71, EmoteId = 20 });
            EmoteList.Add(new Emote { Id = 72, EmoteId = 25 });
            EmoteList.Add(new Emote { Id = 74, EmoteId = 16 });
            EmoteList.Add(new Emote { Id = 75, EmoteId = 15 });
            EmoteList.Add(new Emote { Id = 76, EmoteId = 11 });
            EmoteList.Add(new Emote { Id = 77, EmoteId = 14 });
            EmoteList.Add(new Emote { Id = 78, EmoteId = 66 });
            EmoteList.Add(new Emote { Id = 82, EmoteId = 22 });
            EmoteList.Add(new Emote { Id = 83, EmoteId = 6 });
            EmoteList.Add(new Emote { Id = 84, EmoteId = 24 });
            EmoteList.Add(new Emote { Id = 86, EmoteId = 13 });
            EmoteList.Add(new Emote { Id = 87, EmoteId = 12 });
            EmoteList.Add(new Emote { Id = 92, EmoteId = 20 });
            EmoteList.Add(new Emote { Id = 93, EmoteId = 1 });
            EmoteList.Add(new Emote { Id = 94, EmoteId = 5 });
            EmoteList.Add(new Emote { Id = 95, EmoteId = 6 });
            EmoteList.Add(new Emote { Id = 97, EmoteId = 1 });
            EmoteList.Add(new Emote { Id = 100, EmoteId = 4 });
            EmoteList.Add(new Emote { Id = 101, EmoteId = 3 });
            EmoteList.Add(new Emote { Id = 102, EmoteId = 3 });
            EmoteList.Add(new Emote { Id = 107, EmoteId = 6 });
            EmoteList.Add(new Emote { Id = 113, EmoteId = 14 });
            EmoteList.Add(new Emote { Id = 118, EmoteId = 6 });
            EmoteList.Add(new Emote { Id = 120, EmoteId = 6 });
            EmoteList.Add(new Emote { Id = 124, EmoteId = 6 });
            EmoteList.Add(new Emote { Id = 132, EmoteId = 479 });
            EmoteList.Add(new Emote { Id = 136, EmoteId = 19 });
            EmoteList.Add(new Emote { Id = 141, EmoteId = 26 });
            EmoteList.Add(new Emote { Id = 143, EmoteId = 18 });
            EmoteList.Add(new Emote { Id = 183, EmoteId = 14 });
            EmoteList.Add(new Emote { Id = 204, EmoteId = 15 });
            EmoteList.Add(new Emote { Id = 223, EmoteId = 430 });
            EmoteList.Add(new Emote { Id = 243, EmoteId = 21 });
            EmoteList.Add(new Emote { Id = 264, EmoteId = 275 });
            EmoteList.Add(new Emote { Id = 303, EmoteId = 22 });
            EmoteList.Add(new Emote { Id = 304, EmoteId = 25 });
            EmoteList.Add(new Emote { Id = 305, EmoteId = 25 });
            EmoteList.Add(new Emote { Id = 306, EmoteId = 22 });
            EmoteList.Add(new Emote { Id = 307, EmoteId = 15 });
            EmoteList.Add(new Emote { Id = 323, EmoteId = 1 });
            EmoteList.Add(new Emote { Id = 324, EmoteId = 1 });
            EmoteList.Add(new Emote { Id = 325, EmoteId = 1 });
            EmoteList.Add(new Emote { Id = 326, EmoteId = 1 });
            EmoteList.Add(new Emote { Id = 327, EmoteId = 25 });
            EmoteList.Add(new Emote { Id = 328, EmoteId = 24 });
            EmoteList.Add(new Emote { Id = 329, EmoteId = 1 });
            EmoteList.Add(new Emote { Id = 343, EmoteId = 21 });
            EmoteList.Add(new Emote { Id = 366, EmoteId = 377 });
            EmoteList.Add(new Emote { Id = 368, EmoteId = 25 });
            EmoteList.Add(new Emote { Id = 372, EmoteId = 274 });
            EmoteList.Add(new Emote { Id = 373, EmoteId = 274 });
            EmoteList.Add(new Emote { Id = 378, EmoteId = 7 });
            EmoteList.Add(new Emote { Id = 379, EmoteId = 18 });
            EmoteList.Add(new Emote { Id = 406, EmoteId = 20 });
            EmoteList.Add(new Emote { Id = 433, EmoteId = 1 });
            EmoteList.Add(new Emote { Id = 450, EmoteId = 25 });
            EmoteList.Add(new Emote { Id = 453, EmoteId = 1 });
            EmoteList.Add(new Emote { Id = 456, EmoteId = 483 });
            EmoteList.Add(new Emote { Id = 506, EmoteId = 60 });
        }

        public void LoadRaces()
        {
            ChrRaces.Add(new Races
            {
                Id = 1,
                MaleDisplayId = 57899,
                FemaleDisplayId = 56658,
                CinematicSequence = 81,
                Faction = 1
            });

            ChrRaces.Add(new Races
            {
                Id = 2,
                MaleDisplayId = 51894,
                FemaleDisplayId = 53762,
                CinematicSequence = 21,
                Faction = 2
            });

            ChrRaces.Add(new Races
            {
                Id = 3,
                MaleDisplayId = 49242,
                FemaleDisplayId = 53768,
                CinematicSequence = 41,
                Faction = 3
            });

            ChrRaces.Add(new Races
            {
                Id = 4,
                MaleDisplayId = 54918,
                FemaleDisplayId = 54439,
                CinematicSequence = 61,
                Faction = 4
            });

            ChrRaces.Add(new Races
            {
                Id = 5,
                MaleDisplayId = 54041,
                FemaleDisplayId = 56327,
                CinematicSequence = 2,
                Faction = 5
            });

            ChrRaces.Add(new Races
            {
                Id = 6,
                MaleDisplayId = 55077,
                FemaleDisplayId = 56316,
                CinematicSequence = 141,
                Faction = 6
            });

            ChrRaces.Add(new Races
            {
                Id = 7,
                MaleDisplayId = 51877,
                FemaleDisplayId = 53291,
                CinematicSequence = 101,
                Faction = 115
            });

            ChrRaces.Add(new Races
            {
                Id = 8,
                MaleDisplayId = 59071,
                FemaleDisplayId = 59223,
                CinematicSequence = 121,
                Faction = 116
            });

            ChrRaces.Add(new Races
            {
                Id = 9,
                MaleDisplayId = 6894,
                FemaleDisplayId = 6895,
                CinematicSequence = 172,
                Faction = 2204
            });

            ChrRaces.Add(new Races
            {
                Id = 10,
                MaleDisplayId = 62127,
                FemaleDisplayId = 62128,
                CinematicSequence = 162,
                Faction = 1610
            });

            ChrRaces.Add(new Races
            {
                Id = 11,
                MaleDisplayId = 57027,
                FemaleDisplayId = 58232,
                CinematicSequence = 163,
                Faction = 1629
            });

            ChrRaces.Add(new Races
            {
                Id = 22,
                MaleDisplayId = 29422,
                FemaleDisplayId = 29423,
                CinematicSequence = 170,
                Faction = 2203
            });

            ChrRaces.Add(new Races
            {
                Id = 24,
                MaleDisplayId = 38551,
                FemaleDisplayId = 38552,
                CinematicSequence = 259,
                Faction = 2395
            });

            ChrRaces.Add(new Races
            {
                Id = 25,
                MaleDisplayId = 38551,
                FemaleDisplayId = 38552,
                CinematicSequence = 259,
                Faction = 2401
            });

            ChrRaces.Add(new Races
            {
                Id = 26,
                MaleDisplayId = 38551,
                FemaleDisplayId = 38552,
                CinematicSequence = 259,
                Faction = 2402
            });

            // New
            ChrRaces.Add(new Races
            {
                Id = 27,
                MaleDisplayId = 75078,
                FemaleDisplayId = 75079,
                CinematicSequence = 0,
                Faction = 1610
            });
            ChrRaces.Add(new Races
            {
                Id = 28,
                MaleDisplayId = 75080,
                FemaleDisplayId = 75081,
                CinematicSequence = 0,
                Faction = 6
            });
            ChrRaces.Add(new Races
            {
                Id = 29,
                MaleDisplayId = 75082,
                FemaleDisplayId = 75083,
                CinematicSequence = 0,
                Faction = 4
            });
            ChrRaces.Add(new Races
            {
                Id = 30,
                MaleDisplayId = 75084,
                FemaleDisplayId = 75085,
                CinematicSequence = 0,
                Faction = 1629
            });

            ChrRaces.Add(new Races
            {
                Id = 31,
                MaleDisplayId = 79100,
                FemaleDisplayId = 79101,
                CinematicSequence = 0,
                Faction = 116
            });

            ChrRaces.Add(new Races
            {
                Id = 33,
                MaleDisplayId = 82317,
                FemaleDisplayId = 82318,
                CinematicSequence = 41,
                Faction = 1
            });
            ChrRaces.Add(new Races
            {
                Id = 32,
                MaleDisplayId = 80387,
                FemaleDisplayId = 80388,
                CinematicSequence = 0,
                Faction = 1
            });

            ChrRaces.Add(new Races
            {
                Id = 34,
                MaleDisplayId = 83910,
                FemaleDisplayId = 83911,
                CinematicSequence = 0,
                Faction = 3
            });

            ChrRaces.Add(new Races
            {
                Id = 35,
                MaleDisplayId = 83913,
                FemaleDisplayId = 83914,
                CinematicSequence = 172,
                Faction = 2204
            });

            ChrRaces.Add(new Races
            {
                Id = 36,
                MaleDisplayId = 84558,
                FemaleDisplayId = 84560,
                CinematicSequence = 0,
                Faction = 1
            });

            ChrRaces.Add(new Races
            {
                Id = 37,
                MaleDisplayId = 90786,
                FemaleDisplayId = 90787,
                CinematicSequence = 0,
                Faction = 1
            });

            // TODO: Add non player races.
        }

        public void StartRangeUpdateTimers()
        {
            var updateTask = new Thread(UpdateTask);

            updateTask.IsBackground = true;
            updateTask.Start();
        }

        void UpdateTask()
        {
            while (true)
            {
                lock (taskObject)
                {
                    Thread.Sleep(500);
                  
                    if (waitAdd)
                        continue;

                    foreach (var s in Sessions.ToList())
                    {
                        var session = s.Value;
                        var pChar = session.Character;

                        if (pChar != null)
                        {
                            WriteInRangeObjects(Manager.SpawnMgr.GetInRangeCreatures(pChar), session, ObjectType.Unit);
                            WriteInRangeObjects(Manager.SpawnMgr.GetInRangeObjects(pChar), session, ObjectType.GameObject);

#if DEBUG
                            WriteInRangeCharacer(GetInRangeCharacter(pChar), session);
#endif

                            WriteOutOfRangeObjects(Manager.SpawnMgr.GetOutOfRangeCreatures(pChar), session);
#if DEBUG
                            WriteOutOfRangePlayer(GetOutOfRangeCharacter(pChar), session);
#endif
                        }
                    };
                }
            }
        }

        public bool AddSession(ulong guid, ref WorldClass session)
        {
            return Sessions.TryAdd(guid, session);
        }

        public WorldClass DeleteSession(ulong guid)
        {
            WorldClass removedSession;
            Sessions.TryRemove(guid, out removedSession);

            return removedSession;
        }

        public WorldClass GetSession(string name)
        {
            foreach (var s in Sessions)
                if (s.Value.Character.Name == name)
                    return s.Value;

            return null;
        }

        public WorldClass GetSession(ulong guid)
        {
            WorldClass session;
            if (Sessions.TryGetValue(guid, out session))
                return session;

            return null;
        }

        public bool AddSession2(ulong guid, ref WorldClass2 session)
        {
            return Sessions2.TryAdd(guid, session);
        }

        public WorldClass2 DeleteSession2(ulong guid)
        {
            WorldClass2 removedSession;
            Sessions2.TryRemove(guid, out removedSession);

            return removedSession;
        }

        public WorldClass2 GetSession2(string name)
        {
            foreach (var s in Sessions2)
                if (s.Value.Character.Name == name)
                    return s.Value;

            return null;
        }

        public WorldClass2 GetSession2(ulong guid)
        {
            WorldClass2 session;
            if (Sessions2.TryGetValue(0, out session))
                return session;

            return null;
        }
        public WorldClass2 GetSession2()
        {
            return Sessions2.First().Value;
        }

        public void WriteCreateObject(ref PacketWriter updateObject, WorldObject obj, UpdateFlag updateFlags, ObjectType type, ulong otherGuid = 0, uint emoteState = 0)
        {
            byte[] updatefielddata = null;

            if (type == ObjectType.Unit)
                updatefielddata = ObjectHandler.WriteUpdateFields(obj as CreatureSpawn);

            if (type == ObjectType.GameObject)
                updatefielddata = ObjectHandler.WriteUpdateFields(obj as GameObjectSpawn);

            if (type == ObjectType.OtherPlayer)
                updatefielddata = ObjectHandler.WriteUpdateFields(obj as Character, false, otherGuid == 0, emoteState);

            var ssize = 0 + updatefielddata.Length;

            var hdata = BitConverter.GetBytes((uint)ssize).ToHexString();

            hdata += "FF"; // object type
            hdata += updatefielddata.ToHexString();

            updateObject.WriteUInt8((byte)UpdateType.CreateObject);

            if (type == ObjectType.Player || type == ObjectType.OtherPlayer)
                updateObject.WriteSmartGuid(otherGuid == 0 ? obj.Guid : otherGuid);
            else if (type == ObjectType.Unit || type == ObjectType.GameObject)
                updateObject.WriteSmartGuid(obj.SGuid);

            updateObject.WriteUInt8((byte)type);

            Manager.WorldMgr.WriteUpdateObjectMovement(ref updateObject, ref obj, updateFlags, type, otherGuid);
            updateObject.Write(hdata.ToByteArray());

        }

        public void WriteInRangeObjects(IEnumerable<WorldObject> objects, WorldClass session, ObjectType type)
        {
            var pChar = session.Character;
            var count = objects.Count();

            UpdateFlag updateFlags;

            if (type != ObjectType.GameObject)
                updateFlags = UpdateFlag.Alive;
            else
            {
                updateFlags = UpdateFlag.Rotation | UpdateFlag.StationaryPosition;
            }

            if (count > 0)
            {
                foreach (var o in objects)
                {
                    PacketWriter updateObject = new PacketWriter(ServerMessage.ObjectUpdate);
                    updateObject.WriteInt32(1);
                    updateObject.WriteUInt16((ushort)pChar.Map);
                    updateObject.WriteUInt8(0);
                    updateObject.WriteInt32(0);

                    {
                        WorldObject obj = o;

                        if (!pChar.InRangeObjects.ContainsKey(o.Guid))
                        {
                            WriteCreateObject(ref updateObject, obj, updateFlags, type);

                            if (pChar.Guid != o.Guid)
                                pChar.InRangeObjects.Add(obj.Guid, obj);
                        }
                    }

                    var size = (uint)updateObject.BaseStream.Length - 29;
                    updateObject.WriteUInt32Pos(size, 25);

                    session.Send(ref updateObject);

                    // Send emote data after create.
                    if (o is CreatureSpawn)
                    {
                        if ((o as CreatureSpawn).Emote != 0)
                            EmoteHandler.SendEmote((uint)(o as CreatureSpawn).Emote, session, o);
                    }
                }
            }
        }

        bool waitCheck;

        public void WriteInRangeCharacer(IEnumerable<(Character, ulong, uint)> objects, WorldClass session)
        {
            if (waitCheck)
                return;

            waitCheck = true;

            var pChar = session.Character;
            var count = objects.Count();
            var updateFlags = UpdateFlag.Alive;

            if (count > 0)
            {
                foreach (var o in objects)
                {
                    PacketWriter updateObject = new PacketWriter(ServerMessage.ObjectUpdate);
                    updateObject.WriteInt32(1);
                    updateObject.WriteUInt16((ushort)pChar.Map);
                    updateObject.WriteUInt8(0);
                    updateObject.WriteInt32(0);

                    {
                        WorldObject obj = o.Item1;

                        if (!pChar.InRangeObjects.ContainsKey(o.Item2))
                        {
                            WriteCreateObject(ref updateObject, obj, updateFlags, ObjectType.OtherPlayer, o.Item2, o.Item3);

                            if (pChar.Guid != o.Item2)
                                pChar.InRangeObjects.Add(o.Item2, obj);
                        }
                    }

                    var size = (uint)updateObject.BaseStream.Length - 29;
                    updateObject.WriteUInt32Pos(size, 25);

                    session.Send(ref updateObject);


                    // Send emote data after create.
                    if (o.Item1.EmoteId != 0)
                    {
                        EmoteHandler.SendEmote(o.Item1.EmoteId, session, o.Item1, o.Item2);
                    }
                }
            }

            waitCheck = false;
        }

        public void WriteOutOfRangeObjects(IEnumerable<WorldObject> objects, WorldClass session)
        {
            var pChar = session.Character;
            var count = objects.Count();

            if (count > 0)
            {
                foreach (var o in objects)
                {
                    PacketWriter updateObject = new PacketWriter(ServerMessage.ObjectUpdate);
                    BitPack bp = new BitPack(updateObject);

                    updateObject.WriteUInt32(0);

                    updateObject.WriteUInt16((ushort)pChar.Map);
                    bp.Write(true);
                    bp.Flush();

                    updateObject.WriteUInt16((ushort)pChar.Map);
                    updateObject.WriteUInt32((uint)count);

                    updateObject.WriteSmartGuid(o.SGuid);
                    updateObject.WriteInt32(0);

                    var spawn = Manager.SpawnMgr.FindSpawn(o.Guid);
                    Manager.SpawnMgr.RemoveSpawn(spawn, false);

                    pChar.InRangeObjects.Remove(o.Guid);

                    session.Send(ref updateObject);
                }
            }
        }

        public void WriteOutOfRangePlayer(IEnumerable<(Character, ulong)> objects, WorldClass session)
        {
            var pChar = session.Character;
            var count = objects.Count();

            if (count > 0)
            {
                waitCheck = true;

                foreach (var o in objects)
                {
                    PacketWriter updateObject = new PacketWriter(ServerMessage.ObjectUpdate);
                    BitPack bp = new BitPack(updateObject);

                    updateObject.WriteUInt32(0);

                    updateObject.WriteUInt16((ushort)pChar.Map);
                    bp.Write(true);
                    bp.Flush();

                    updateObject.WriteUInt16((ushort)pChar.Map);
                    updateObject.WriteUInt32((uint)count);

                    updateObject.WriteSmartGuid(o.Item2);
                    updateObject.WriteInt32(0);

                    PlayerSpawns.RemoveAll(c => c.Guid == o.Item2);

                    pChar.InRangeObjects.Remove(o.Item2);

                    session.Send(ref updateObject);
                }
            }

            waitCheck = false;
        }

        public IEnumerable<(Character, ulong, uint)> GetInRangeCharacter(WorldObject obj)
        {
            foreach (var c in PlayerSpawns.ToList())
                if (!obj.ToCharacter().InRangeObjects.ContainsKey(c.Guid))
                    if (obj.CheckDistance(c.Character))
                        yield return (c.Character, c.Guid, c.Emote);
        }

        public IEnumerable<(Character, ulong)> GetOutOfRangeCharacter(WorldObject obj)
        {
            foreach (var c in PlayerSpawns.ToList())
                if (obj.ToCharacter().InRangeObjects.ContainsKey(c.Guid))
                    if (!obj.CheckDistance(c.Character))
                        yield return (c.Character, c.Guid);
        }

        public void SendByDist(WorldObject obj, PacketWriter packet, float dist)
        {
            foreach (var s in Sessions)
                if (obj.CheckDistance(s.Value.Character, dist))
                    s.Value.Send(ref packet);
        }

        public void SendByDist2(WorldObject obj, PacketWriter packet, float dist)
        {
            foreach (var s in Sessions2)
                if (obj.CheckDistance(s.Value.Character, dist))
                    s.Value.Send(ref packet);
        }

        public enum AccountDataTypes
        {
            GlobalConfigCache = 0x00,
            PerCharacterConfigCache = 0x01,
            GlobalBindingsCache = 0x02,
            PerCharacterBindingsCache = 0x03,
            GlobalMacrosCache = 0x04,
            PerCharacterMacrosCache = 0x05,
            PerCharacterLayoutCache = 0x06,
            PerCharacterChatCache = 0x07,
        }

        public uint createTime = 0;

        public void WriteAccountDataTimes(AccountDataMasks mask, ref WorldClass session, bool meh = false)
        {
            PacketWriter accountDataTimes = new PacketWriter(ServerMessage.AccountDataTimes);

            accountDataTimes.WriteSmartGuid(session.Character.Guid);

            Log.Message(LogType.Debug, $"AccountData Guid: {session.Character.Guid}");

            accountDataTimes.WriteUnixTime();

            for (var i = 0; i < 8; i++)
            {
                switch (i)
                {
                    case 0:
                    case 4:
                        accountDataTimes.WriteUInt32(createTime);
                        break;
                    default:
                        accountDataTimes.WriteUInt32(0);
                        break;
                }
            }

            session.Send(ref accountDataTimes);
        }

        public void SendToInRangeCharacter(Character pChar, PacketWriter packet)
        {
            foreach (var c in Sessions.ToList())
            {
                WorldObject iChar;
                if (pChar.InRangeObjects.TryGetValue(c.Value.Character.Guid, out iChar))
                    c.Value.Send(ref packet);
            }
        }

        public void WriteUpdateObjectMovement(ref PacketWriter packet, ref WorldObject wObject, UpdateFlag updateFlags, ObjectType type = ObjectType.Player, ulong otherGuid = 0)
        {
            ObjectMovementValues values = new ObjectMovementValues(updateFlags);
            BitPack BitPack = new BitPack(packet);

            BitPack.Write(0);// noBirthAnim
            BitPack.Write(0);// enablePortals
            BitPack.Write(0);// playHoverAnim
            BitPack.Write(values.IsAlive);// move
            BitPack.Write(0);// passenger
            BitPack.Write(values.HasStationaryPosition);// stationary
            BitPack.Write(0);// combatVictim
            BitPack.Write(0);// serverTime
            BitPack.Write(0);// vehicle
            BitPack.Write(0);// animKit
            BitPack.Write(values.HasRotation);// rotation, wObject is GameObjectSpawn//512
            BitPack.Write(0);// areaTrigger
            BitPack.Write(0);// gameObject
            BitPack.Write(0);// replaceActive
            BitPack.Write(values.IsSelf);// thisIsYou
            BitPack.Write(0);// sceneObjCreate

            BitPack.Write(0);// scenePendingInstances
            BitPack.Write(0);

            BitPack.Flush();

            var hasSpline = false;
            var spline = new CreatureSpline();

            if (values.IsAlive)
            {
                if (type == ObjectType.Unit || type == ObjectType.GameObject)
                    packet.WriteSmartGuid(wObject.SGuid);
                else
                    packet.WriteSmartGuid(otherGuid == 0 ? wObject.Guid : otherGuid);

                packet.WriteUInt32((uint)spline.MovementFlags);
                packet.WriteUInt32((uint)values.MovementFlags2);
                packet.WriteUInt32(0);

                packet.WriteUInt32(0);
                packet.WriteFloat(wObject.Position.X);
                packet.WriteFloat(wObject.Position.Y);
                packet.WriteFloat(wObject.Position.Z);
                packet.WriteFloat(wObject.Position.O);
                packet.WriteFloat(0);
                packet.WriteFloat(0);
                packet.WriteUInt32(0);
                packet.WriteUInt32(0);

                if ((wObject as CreatureSpawn)?.Id == 114791)
                {
                    values.MovementFlags = (Framework.Constants.Movement.MovementFlag)512;
                    //values.MovementFlags |= Framework.Constants.Movement.MovementFlag.Flight;
                    //values.MovementFlags |= Framework.Constants.Movement.MovementFlag.CanFly;
                }

                if (wObject is CreatureSpawn)
                {
                    values.MovementFlags |= Framework.Constants.Movement.MovementFlag.Collision;

                    hasSpline = (wObject as CreatureSpawn).Spline != null;

                    if (hasSpline)
                        spline = (wObject as CreatureSpawn).Spline;
                }

                //BitPack.Write((uint)spline.MovementFlags, 30);
                //BitPack.Write((uint)values.MovementFlags2, 18);

                BitPack.Write(0);
                BitPack.Write(0);
                BitPack.Write(hasSpline);
                BitPack.Write(0);
                BitPack.Write(0);

                BitPack.Flush();

                //                 if (values.IsFallingOrJumping)//132
                //                 {
                //                     packet.WriteUInt32(values.FallTime);//108
                //                     packet.WriteFloat(values.JumpVelocity);//112
                // 
                //                     if (values.HasJumpData)//128
                //                     {
                //                         packet.WriteFloat(values.Sin);
                //                         packet.WriteFloat(values.Cos);
                //                         packet.WriteFloat(values.CurrentSpeed);
                //                     }
                //                 }

                packet.WriteFloat(MovementSpeed.WalkSpeed);
                packet.WriteFloat(MovementSpeed.RunSpeed);
                packet.WriteFloat(MovementSpeed.RunBackSpeed);
                packet.WriteFloat(MovementSpeed.SwimSpeed);
                packet.WriteFloat(MovementSpeed.SwimBackSpeed);
                packet.WriteFloat(MovementSpeed.FlySpeed);
                packet.WriteFloat(MovementSpeed.FlyBackSpeed);
                packet.WriteFloat(MovementSpeed.TurnSpeed);
                packet.WriteFloat(MovementSpeed.PitchSpeed);
                packet.WriteUInt32(0);
                packet.WriteFloat(1);

                BitPack.Write(hasSpline);
                BitPack.Flush();
            }

            if (hasSpline)
            {
                packet.WriteUInt32(spline.Id);
                packet.WriteFloat(spline.Destination.X);
                packet.WriteFloat(spline.Destination.Y);
                packet.WriteFloat(spline.Destination.Z);

                BitPack.Write(spline.Move != null);
                BitPack.Flush();

                // MovementSplineMove
                if (spline.Move != null)
                {
                    packet.WriteUInt32(spline.Move.Flags);
                    packet.WriteInt32(spline.Move.Elapsed);
                    packet.WriteUInt32(spline.Move.Duration);
                    packet.WriteFloat(spline.Move.DurationModifier);
                    packet.WriteFloat(spline.Move.NextDurationModifier);

                    BitPack.Write(spline.Move.Face, 2); // spline.Move
                    BitPack.Write(false);

                    BitPack.Write(spline.Move.Points.Length, 16);

                    BitPack.Write(false);
                    BitPack.Write(false);
                    BitPack.Write(false);
                    BitPack.Write(false);
                    BitPack.Flush();

                    foreach (var p in spline.Move.Points)
                    {
                        packet.WriteFloat(p.X);
                        packet.WriteFloat(p.Y);
                        packet.WriteFloat(p.Z);
                    }
                }
            }

            packet.WriteUInt32(0);

            if (values.HasStationaryPosition)
            {
                packet.WriteFloat(wObject.Position.X);
                packet.WriteFloat(wObject.Position.Y);
                packet.WriteFloat(wObject.Position.Z);
                packet.WriteFloat(wObject.Position.O);
            }

            // TODO: Fix for X & Y axis.
            if (values.HasRotation)
                packet.WriteInt64(Quaternion.GetCompressed(wObject.Position.O));
        }

        public void SendHotfixes(WorldClass session)
        {
            return;
#if !PUBLIC
            if (!Directory.Exists("./hotfixes"))
                Directory.CreateDirectory("./hotfixes");

            var hotfixFiles = Directory.GetFiles("./hotfixes/", "*.csv");
#else


            List<string> outfitLines = new List<string>();

            if (File.Exists("./CharStartOutfits.csv"))
            {
                using (StreamReader reader = new StreamReader("./CharStartOutfits.csv"))
                {
                    while (!reader.EndOfStream)
                    {
                        outfitLines.Add(reader.ReadLine());
                    }
                }
            }
            else
            {
                using (StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Arctium.WoW.Sandbox.Server.Resources.charstartoutfits.csv")))
                {
                    while (!reader.EndOfStream)
                    {
                        outfitLines.Add(reader.ReadLine());
                    }
                }
            }

            List<string> globalStringsLines = new List<string>();
            using (StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Arctium.WoW.Sandbox.Server.Resources.GlobalStrings.csv")))
            {
                while (!reader.EndOfStream)
                {
                    globalStringsLines.Add(reader.ReadLine());
                }
            }

            List<string> tactKeyLines = new List<string>();
            using (StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Arctium.WoW.Sandbox.Server.Resources.TactKey.csv")))
            {
                while (!reader.EndOfStream)
                {
                    tactKeyLines.Add(reader.ReadLine());
                }
            }


            var hotfixFiles = new Dictionary<string, string[]>
            {
               // { "AreaTable", areaTableLines.ToArray() },
                //{ "CharStartOutfit", outfitLines.ToArray() },
                //{ "GlobalStrings", globalStringsLines.ToArray() },
               //{ "TactKey", tactKeyLines.ToArray() },
               // { "TactKeyLookup", tactKeyLookupLines.ToArray() }
            }.ToArray();
#endif

            var hotfixes = new List<HotfixEntry>();

            foreach (var f in hotfixFiles)
            {
#if PUBLIC
                if (f.Value.Length == 0)
                    continue;
#endif
#if !PUBLIC
                var name = Path.GetFileNameWithoutExtension(f);
                var dbinfo = DBInfo[name.ToLower()];

#if LEYSTTV
                if (dbinfo.Name != "ItemDisplayInfo" || dbinfo.Name != "CharStartOutfit")
                    continue;
#endif

                //var lines = File.ReadAllText(f).Split(new[] { "\";" }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim('\r', '\n') + "\"").ToArray();
                var lines = File.ReadAllText(f).Split(new[] { "\";HFEND\n" }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim('\r', '\n') + "\"").ToArray();

                lines = lines.Where(l => !string.IsNullOrEmpty(l) && l != "\"").ToArray();

                var withoutHeader = lines[0].Split(new[] { '\n' }, 2);

                // Remove the header.
                if (withoutHeader.Length == 2)
                    lines[0] = withoutHeader[1];
#else

                var name = f.Key;
                var dbinfo = DBInfo[name.ToLower()];
                var lines = f.Value;
#endif

                var recSize = 0;
                //var fieldLengths = lines[0].Split(new[] { "," }, StringSplitOptions.None).Select(s => s.Replace("\"", "")).ToArray();

                for (var i = 0; i < dbinfo.FieldTypes.Length; i++)
                {
                    switch (dbinfo.FieldTypes[i].ToLower())
                    {
                        case "string":
                            break;
                        case "sbyte":
                            recSize += 1;
                            break;
                        case "byte":
                            recSize += 1;
                            break;
                        case "int16":
                            recSize += 2;
                            break;
                        case "uint16":
                            recSize += 2;
                            break;
                        case "int32":
                            recSize += 4;
                            break;
                        case "uint32":
                            recSize += 4;
                            break;
                        case "single":
                            recSize += 4;
                            break;
                        case "int64":
                            recSize += 8;
                            break;
                        case "uint64":
                            recSize += 8;
                            break;
                        case "ref":
                            recSize += 4;
                            break;
                        default:
                            Log.Message(LogType.Error, "Unknown field type for hotfixes.");
                            break;
                    }
                }

                var size = recSize;

                for (var j = 0; j < lines.Length; j++)
                {
                    size = recSize;
                    var pkt = new PacketWriter(ServerMessage.DBReply);
                    //pkt.Write(1);

                    var l = lines[j];
#if !PUBLIC
                    var fields = l.Split(new[] { "\";\"" }, StringSplitOptions.None).Select(s => s.Trim('"').Replace("\"\"", "\"")).ToArray();
#else
                    var fields = l.Split(new[] { "\",\"" }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim('"')).ToArray();
#endif
                    // CLientDB viewer always show ids at index 0
                    var id = uint.Parse(fields[0]);

                    // always skip the first field
                    fields = fields.Skip(1).ToArray();

                    // ok let's put the id at the correc position.
                    if (!dbinfo.HasIndex)
                    {
                        var fieldList = fields.ToList();

                        fieldList.Insert(dbinfo.IDPosition, id.ToString());

                        fields = fieldList.ToArray();
                    }

                    var writer = new BinaryWriter(new MemoryStream());

                    for (var i = 0; i < fields.Length; i++)
                    {
                        switch (dbinfo.FieldTypes[i].ToLower())
                        {
                            case "string":
                                var sBytes = Encoding.UTF8.GetBytes(fields[i]);

                                size += sBytes.Length + 1;

                                writer.Write(sBytes, 0, sBytes.Length);
                                writer.Write((byte)0);
                                break;
                            case "sbyte":
                                writer.Write(sbyte.Parse(fields[i]));
                                break;
                            case "byte":
                                writer.Write(byte.Parse(fields[i]));
                                break;
                            case "int16":
                                writer.Write(short.Parse(fields[i]));
                                break;
                            case "uint16":
                                writer.Write(ushort.Parse(fields[i]));
                                break;
                            case "int32":
                                writer.Write(int.Parse(fields[i]));
                                break;
                            case "uint32":
                                writer.Write(uint.Parse(fields[i]));
                                break;
                            case "single":
                                writer.Write(float.Parse(fields[i], NumberStyles.Any, CultureInfo.InvariantCulture));
                                break;
                            case "int64":
                                writer.Write(long.Parse(fields[i]));
                                break;
                            case "uint64":
                                writer.Write(ulong.Parse(fields[i]));
                                break;
                            case "ref":
                                writer.Write(uint.Parse(fields[i]));
                                break;
                            default:
                                Log.Message(LogType.Error, "Unknown field type for hotfixes.");
                                break;
                        }
                    }

                    pkt.Write(dbinfo.TableHash);
                    pkt.Write(id);
                    pkt.WriteUInt32(0x5BD62C08);
                    pkt.Write((byte)0x80); // allow

                    pkt.Write(size);
                    pkt.Write((writer.BaseStream as MemoryStream).ToArray());

                    session.Send(ref pkt);

                }
            }
        }

        public Dictionary<string, DBInfo> DBInfo = new Dictionary<string, DBInfo>();
    }
}

public class HotfixEntry
{
    public int Index { get; set; }
    public int Id { get; set; }
    public List<byte[]> Data { get; set; } = new List<byte[]>();
}

public class DBInfo
{
    public string Name { get; set; }
    public uint TableHash { get; set; }
    public bool HasIndex { get; set; }
    public string[] FieldTypes { get; set; }
    public bool HasRefId { get; set; }
    public int IDPosition { get; set; } = -1;
}
