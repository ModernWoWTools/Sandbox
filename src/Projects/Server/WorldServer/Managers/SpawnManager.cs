using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using AuthServer.Game.Entities;
using AuthServer.Network;
using AuthServer.WorldServer.Game.Entities;
using Framework.Constants.Misc;
using Framework.Constants.Net;
using Framework.Logging;
using Framework.Misc;
using Framework.Network.Packets;
using Framework.ObjectDefines;
using Framework.Serialization;
using Newtonsoft.Json;

namespace AuthServer.WorldServer.Managers
{
    [System.Reflection.Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = true)]
    public sealed class SpawnManager : Singleton<SpawnManager>
    {
        public Dictionary<ulong, CreatureSpawn> CreatureSpawns;
        public Dictionary<ulong, CreatureSpawn> CreatureSpawnsFile;
        public Dictionary<ulong, GameObjectSpawn> GameObjectSpawns;
        public Dictionary<ulong, CreatureSpawn> preMadeSpawns;
        public Dictionary<ulong, CreatureEquip> CreatureItems;
        public Dictionary<ulong, GameObjectSpawn> preMadeSpawns2;
        public Dictionary<ulong, CreatureSpline> Splines;
        Dictionary<ulong, List<Waypoint>> waypoints;
        List<ulong> InternalFiles = new List<ulong>();

        SpawnManager()
        {
            waypoints = new Dictionary<ulong, List<Waypoint>>();
            //CreatureSpawns = new Dictionary<ulong, CreatureSpawn>();

            //if (!File.Exists("./creaturepoints.txt"))
            //    File.Create("./creaturepoints.txt").Dispose();

            if (!File.Exists("./creaturespawns.txt"))
                File.Create("./creaturespawns.txt").Dispose();

            if (!File.Exists(Helper.DataDirectory() + "creatureequip.json"))
                File.Create(Helper.DataDirectory() + "creatureequip.json").Dispose();

            CreatureItems = Json.CreateObject<Dictionary<ulong, CreatureEquip>>(File.ReadAllText(Helper.DataDirectory() + "creatureequip.json"));

            if (CreatureItems == null)
                CreatureItems = new Dictionary<ulong, CreatureEquip>();

            if (!File.Exists("./gameobjectspawns.txt"))
                File.Create("./gameobjectspawns.txt").Dispose();

#if DEBUG
            if (!File.Exists("./playerspawns.txt"))
                File.Create("./playerspawns.txt").Dispose();
#endif
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Arctium.WoW.Sandbox.Server.Resources.Internal.creaturesplines.json")))
                Splines = JsonConvert.DeserializeObject<Dictionary<ulong, CreatureSpline>>(reader.ReadToEnd());

            if (Splines == null)
                Splines = new Dictionary<ulong, CreatureSpline>();

            Log.Message(LogType.Debug, $"Loaded {Splines.Count} splines.");

            Initialize();
        }

        public void Initialize()
        {
#if DEBUG
            LoadPlayerSpawns();
#endif
            LoadCreatureSpawns(false);
            //LoadCreatureSpawns();
            LoadGameobjectSpawns();
        }

        public void UpdateSpawnEmote(ulong guid, int emote)
        {
            CreatureSpawns[guid].Emote = emote;
            CreatureSpawnsFile[guid].Emote = emote;

            var sb = new StringBuilder();

            foreach (var cs in CreatureSpawnsFile)
            {
                if (!InternalFiles.Contains(cs.Key))
                    sb.AppendLine($"{cs.Value.Guid}, {cs.Value.Id}, {cs.Value.Map}, {cs.Value.Position.X}, {cs.Value.Position.Y}, {cs.Value.Position.Z}, {cs.Value.Position.O}, {cs.Value.Emote}," +
                    $" {cs.Value.AnimState}, {cs.Value.ModelId}, {cs.Value.Scale}, {cs.Value.EquipmentId}, {cs.Value.NpcFlags}, {cs.Value.UnitFlags}, {cs.Value.FactionTemplate}, {cs.Value.Level}, {cs.Value.MountDisplayId}, {cs.Value.HoverHeight}" +
                    $", {cs.Value.Health}, {cs.Value.MaxHealth}\r\n");
            }

            File.WriteAllText("./creaturespawns.txt", sb.ToString());
        }

        public void AddSpawn(CreatureSpawn spawn)
        {
            CreatureSpawns.Add(spawn.Guid, spawn);
            CreatureSpawnsFile.Add(spawn.Guid, spawn);

            File.AppendAllText("./creaturespawns.txt", $"{spawn.Guid}, {spawn.Id}, {spawn.Map}, {spawn.Position.X}, {spawn.Position.Y}, {spawn.Position.Z}, {spawn.Position.O}, {spawn.Emote}," +
                $" {spawn.AnimState}, {spawn.ModelId}, {spawn.Scale}, {spawn.EquipmentId}, {spawn.NpcFlags}, {spawn.UnitFlags}, {spawn.FactionTemplate}, {spawn.Level}, {spawn.MountDisplayId}, {spawn.HoverHeight}" +
                $", {spawn.Health}, {spawn.MaxHealth}\r\n");
        }

        public void AddSpawn(Character spawn, ulong guid, uint emote = 0)
        {
            Manager.WorldMgr.PlayerSpawns.Add((spawn, guid, 0));

            File.AppendAllText("./playerspawns.txt", $"{guid}, {spawn.Guid}, {spawn.Map}, {spawn.Position.X}, {spawn.Position.Y}, {spawn.Position.Z}, {spawn.Position.O}, {emote}, {0}\r\n");
        }

        public void AddWayPoint(Waypoint waypoint)
        {
            if (!waypoints.ContainsKey(waypoint.CreatureGuid))
                waypoints.Add(waypoint.CreatureGuid, new List<Waypoint>());

            waypoint.Index = GetLastWPIndex(waypoint.CreatureGuid);

            waypoints[waypoint.CreatureGuid].Add(waypoint);

            File.AppendAllText("./creaturepoints.txt", $"{waypoint.CreatureGuid}, {waypoint.Point.X}, {waypoint.Point.Y}, {waypoint.Point.Z}, {waypoint.Point.O}, {waypoint.Index}, {waypoint.WaitTime}\r\n");
        }

        public int GetLastWPIndex(ulong guid)
        {
            return waypoints[guid].OrderBy(wp => wp.Index).ToArray()[waypoints[guid].Count - 1].Index;
        }

        public void AddSpawn(GameObjectSpawn spawn)
        {
            GameObjectSpawns.Add(spawn.Guid, spawn);

            File.AppendAllText("./gameobjectspawns.txt",
                $"{spawn.Guid}, {spawn.Id}, {spawn.Map}, {spawn.Position.X}, {spawn.Position.Y}, {spawn.Position.Z}, {spawn.Position.O}, " +
                $"{spawn.GameObject.Rot.X}, {spawn.GameObject.Rot.Y}, {spawn.GameObject.Rot.Z}, {spawn.GameObject.Rot.O}, {spawn.AnimProgress}, {spawn.State}, {spawn.SpellVisualId}\n");
        }

        public void RemoveWaypoint(Waypoint wayPoint)
        {
            var sb = new StringBuilder();

            waypoints[wayPoint.CreatureGuid].RemoveAll(r => r.Index == wayPoint.Index);

            for (var i = 0; i < waypoints[wayPoint.CreatureGuid].Count; i++)
            {
                if (waypoints[wayPoint.CreatureGuid][i].Index > wayPoint.Index)
                    waypoints[wayPoint.CreatureGuid][i].Index -= 1;
            }

            foreach (var wp in waypoints)
            {
                foreach (var waypoint in wp.Value)
                    sb.AppendLine($"{waypoint.CreatureGuid}, {waypoint.Point.X}, {waypoint.Point.Y}, {waypoint.Point.Z}, {waypoint.Point.O}, {waypoint.Index}, {waypoint.WaitTime}");
            }

            File.WriteAllText("./creaturepoints.txt", sb.ToString());
        }

        public void RemoveSpawn(CreatureSpawn spawn, bool delete)
        {
            if (delete && !InternalFiles.Contains(spawn.Guid))
            {
                CreatureSpawnsFile.Remove(spawn.Guid);
                CreatureSpawns.Remove(spawn.Guid);
                var sb = new StringBuilder();

                foreach (var cs in CreatureSpawnsFile)
                    if (!InternalFiles.Contains(cs.Key))
                        sb.AppendLine($"{cs.Value.Guid}, {cs.Value.Id}, {cs.Value.Map}, {cs.Value.Position.X}, {cs.Value.Position.Y}, {cs.Value.Position.Z}, {cs.Value.Position.O}, {cs.Value.Emote}," +
                $" {cs.Value.AnimState}, {cs.Value.ModelId}, {cs.Value.Scale}, {cs.Value.EquipmentId}, {cs.Value.NpcFlags}, {cs.Value.UnitFlags}, {cs.Value.FactionTemplate}, {cs.Value.Level}, {cs.Value.MountDisplayId}, {cs.Value.HoverHeight}" +
                $", {cs.Value.Health}, {cs.Value.MaxHealth}\r\n");

                File.WriteAllText("./creaturespawns.txt", sb.ToString());
            }
        }

        public void RemoveInternalSpawns(WorldClass session)
        {
            foreach (var g in InternalFiles)
            {
                PacketWriter updateObject = new PacketWriter(ServerMessage.ObjectUpdate);
                BitPack bp = new BitPack(updateObject);

                updateObject.WriteUInt32(0);

                updateObject.WriteUInt16((ushort)session.Character.Map);
                bp.Write(true);
                bp.Flush();

                updateObject.WriteUInt16((ushort)session.Character.Map);
                updateObject.WriteUInt32((uint)1);

                updateObject.WriteSmartGuid(new SmartGuid(g, CreatureSpawns[g].Id, GuidType.Creature, (uint)session.Character.Map));
                updateObject.WriteInt32(0);

                session.Character.InRangeObjects.Remove(g);

                session.Send(ref updateObject);
            }

            foreach (var g in InternalFiles)
            {
                CreatureSpawnsFile.Remove(g);
                CreatureSpawns.Remove(g);
            }

            InternalFiles.Clear();
        }

        public (Character, uint) RemoveSpawn(ulong guid)
        {
            var ret = Manager.WorldMgr.PlayerSpawns.SingleOrDefault(c => c.Guid == guid);

            if (ret.Guid != 0)
            {
                Manager.WorldMgr.PlayerSpawns.RemoveAll(c => c.Guid == guid);

                var sb = new StringBuilder();

                foreach (var cs in Manager.WorldMgr.PlayerSpawns)
                    sb.AppendLine($"{cs.Guid}, {cs.Character.Guid}, {cs.Character.Map}, {cs.Character.Position.X}, {cs.Character.Position.Y}, {cs.Character.Position.Z}, {cs.Character.Position.O}, {cs.Emote}, {cs.Character.MountId}");

                File.WriteAllText("./playerspawns.txt", sb.ToString());
            }

            return (ret.Character, ret.Emote);
        }

        public void RemoveSpawn(GameObjectSpawn spawn)
        {
            GameObjectSpawns.Remove(spawn.Guid);

            var sb = new StringBuilder();

            foreach (var cs in GameObjectSpawns.Where(gs => gs.Key > lastPremadeGuid))
                sb.AppendLine($"{cs.Value.Guid}, {cs.Value.Id}, {cs.Value.Map}, {cs.Value.Position.X}, {cs.Value.Position.Y}, {cs.Value.Position.Z}, {cs.Value.Position.O}, " +
                $"{cs.Value.GameObject.Rot.X}, {cs.Value.GameObject.Rot.Y}, {cs.Value.GameObject.Rot.Z}, {cs.Value.GameObject.Rot.O}, {cs.Value.AnimProgress}, {cs.Value.State}, {cs.Value.SpellVisualId}");

            File.WriteAllText("./gameobjectspawns.txt", sb.ToString());
        }

        public void UpdatePlayerSpawn(ulong guid, Character character)
        {
            var oldData = Manager.WorldMgr.PlayerSpawns.Single(c => c.Guid == guid);

            Manager.WorldMgr.PlayerSpawns.RemoveAll(c => c.Guid == guid);
            Manager.WorldMgr.PlayerSpawns.Add((character, guid, character.EmoteId));

            var sb = new StringBuilder();

            foreach (var cs in Manager.WorldMgr.PlayerSpawns)
                sb.AppendLine($"{cs.Guid}, {cs.Character.Guid}, {cs.Character.Map}, {cs.Character.Position.X}, {cs.Character.Position.Y}, {cs.Character.Position.Z}, {cs.Character.Position.O}, {cs.Emote}, {cs.Character.MountId}");

            File.WriteAllText("./playerspawns.txt", sb.ToString());
        }

        public CreatureSpawn FindSpawn(ulong guid)
        {
            CreatureSpawn spawn;
            CreatureSpawns.TryGetValue(guid, out spawn);

            return spawn;
        }

        public GameObjectSpawn FindSpawn(long guid)
        {
            GameObjectSpawn spawn;
            GameObjectSpawns.TryGetValue((ulong)guid, out spawn);

            return spawn;
        }

        public IEnumerable<CreatureSpawn> GetInRangeCreatures(WorldObject obj)
        {
            IEnumerable<CreatureSpawn> list = new List<CreatureSpawn>();
            foreach (var c in new Dictionary<ulong, CreatureSpawn>(CreatureSpawns))
            {
                if (!obj?.ToCharacter().InRangeObjects.ContainsKey(c.Key) == true)
                {
                    if (obj.CheckDistance(c.Value))
                        yield return c.Value;
                }
            }
        }

        public IEnumerable<GameObjectSpawn> GetInRangeObjects(WorldObject obj)
        {
            IEnumerable<GameObjectSpawn> list = new List<GameObjectSpawn>();
            foreach (var c in GameObjectSpawns.ToList())
            {
                if (!obj?.ToCharacter().InRangeObjects.ContainsKey(c.Key) == true)
                    if (obj.CheckDistance(c.Value))
                        yield return c.Value;
            }
        }

        public IEnumerable<CreatureSpawn> GetInRangeCreatures2(WorldObject obj, bool meh)
        {
            List<CreatureSpawn> list = new List<CreatureSpawn>();
            foreach (var c in new Dictionary<ulong, CreatureSpawn>(CreatureSpawns))
                if (!obj.ToCharacter().InRangeObjects.ContainsKey(c.Key))
                    if (obj.CheckDistance(c.Value))
                        list.Add(c.Value);

            return list;
        }

        public IEnumerable<CreatureSpawn> GetOutOfRangeCreatures(WorldObject obj)
        {
            foreach (var c in new Dictionary<ulong, CreatureSpawn>(CreatureSpawns))
                if (obj.ToCharacter().InRangeObjects.ContainsKey(c.Key))
                    if (!obj.CheckDistance(c.Value))
                        yield return c.Value;
        }

        [Obfuscation(Feature = "virtualization", Exclude = true)]
        public void LoadCreatureSpawns(bool internalLoad = false)
        {
            if (CreatureSpawns == null)
                CreatureSpawns = new Dictionary<ulong, CreatureSpawn>();

            if (CreatureSpawnsFile == null)
                CreatureSpawnsFile = new Dictionary<ulong, CreatureSpawn>();

            bool updated = false;
            bool updated2 = false;
            bool updated3 = false;
            string[] line;

            var loadFile = internalLoad ? new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Arctium.WoW.Sandbox.Server.Resources.Internal.creaturespawns.txt")) : new StreamReader("./creaturespawns.txt");

            using (var reader = loadFile)
            {
                var cttr = 1;
                var guid = 1uL;

                while (!reader.EndOfStream)
                {
                    ++cttr;
                    line = reader.ReadLine().Split(new char[] { ',' });

                    if (line.Length < 6)
                        continue;

                    if (!updated && line.Length == 6)
                    {
                        Log.Message(LogType.Debug, "Found old creaturespawns format, Updating...");
                        updated = true;
                    }

                    if (!updated2 && line.Length == 7)
                    {
                        Log.Message(LogType.Debug, "Found old creaturespawns format, Updating...");
                        updated2 = true;
                    }

                    if (!updated3 && line.Length == 8)
                    {
                        Log.Message(LogType.Debug, "Found old creaturespawns format, Updating...");
                        updated3 = true;
                    }

                    var id = int.Parse(line[1]);
                    var map = 0;
                    var x = 0f;
                    var y = 0f;
                    var z = 0f;
                    var o = 0f;
                    var emote = 0;

                    if (line.Length == 6 || line.Length == 7)
                    {
                        id = int.Parse(line[0]);
                        map = int.Parse(line[1]);
                        x = float.Parse(line[2]);
                        y = float.Parse(line[3]);
                        z = float.Parse(line[4]);
                        o = float.Parse(line[5]);

                        guid++;
                    }


                    if (line.Length == 7)
                        emote = int.Parse(line[6]);

                    var creature = Manager.ObjectMgr.Creatures.ContainsKey(id) ? Manager.ObjectMgr.Creatures[id] : null;

                    if (line.Length == 8)
                    {
                        guid = ulong.Parse(line[0]);
                        id = int.Parse(line[1]);
                        map = int.Parse(line[2]);
                        x = float.Parse(line[3]);
                        y = float.Parse(line[4]);
                        z = float.Parse(line[5]);
                        o = float.Parse(line[6]);
                        emote = int.Parse(line[7]);

                        var tempLine = line.ToList();

                        tempLine.Add("0");
                        tempLine.Add($"{creature.DisplayInfoId[0].displayId}");
                        tempLine.Add("1");
                        tempLine.Add("0");
                        tempLine.Add("0");
                        tempLine.Add("0");
                        tempLine.Add("35");
                        tempLine.Add("1");
                        tempLine.Add("0");
                        tempLine.Add("1");
                        tempLine.Add("0");
                        tempLine.Add("0");

                        line = tempLine.ToArray();
                    }

                    if (line.Length == 20)
                    {
                        guid = ulong.Parse(line[0]);
                        id = int.Parse(line[1]);
                        map = int.Parse(line[2]);
                        x = float.Parse(line[3]);
                        y = float.Parse(line[4]);
                        z = float.Parse(line[5]);
                        o = float.Parse(line[6]);
                        emote = int.Parse(line[7]);
                    }


                    if (creature != null)
                    {
                        CreatureSpawn spawn = new CreatureSpawn()
                        {
                            SGuid = new SmartGuid(guid, id, GuidType.Creature, (uint)map),
                            Guid = (ulong)guid,
                            Id = id,
                            Creature = creature,
                            Position = new Vector4 { X = x, Y = y, Z = z, O = o },
                            Map = (uint)map,
                            Emote = emote
                        };

                        spawn.AnimState = uint.Parse(line[8]);
                        spawn.ModelId = uint.Parse(line[9]) == 0 ? (uint)creature.DisplayInfoId[0].displayId : uint.Parse(line[9]);
                        spawn.Scale = float.Parse(line[10]);
                        spawn.EquipmentId = int.Parse(line[11]);
                        spawn.NpcFlags = uint.Parse(line[12]);
                        spawn.UnitFlags = uint.Parse(line[13]);
                        spawn.FactionTemplate = uint.Parse(line[14]);
                        spawn.Level = uint.Parse(line[15]);
                        spawn.MountDisplayId = uint.Parse(line[16]);
                        spawn.HoverHeight = float.Parse(line[17]);
                        spawn.Health = long.Parse(line[18]);
                        spawn.MaxHealth = long.Parse(line[19]);

                        Splines.TryGetValue(spawn.Guid, out spawn.Spline);

                        if (!CreatureSpawns.ContainsKey((ulong)guid))
                            CreatureSpawns.Add((ulong)guid, spawn);

                        if (!CreatureSpawnsFile.ContainsKey((ulong)guid))
                            CreatureSpawnsFile.Add((ulong)guid, spawn);

                        if (internalLoad)
                            InternalFiles.Add((ulong)guid);
                    }
                }
            }

            if (updated)
            {
                var sb = new StringBuilder();

                foreach (var cs in CreatureSpawnsFile)
                    sb.AppendLine($"{cs.Value.Id}, {cs.Value.Map}, {cs.Value.Position.X}, {cs.Value.Position.Y}, {cs.Value.Position.Z}, {cs.Value.Position.O}, {cs.Value.Emote}");

                File.WriteAllText("./creaturespawns.txt", sb.ToString());

                CreatureSpawns = new Dictionary<ulong, CreatureSpawn>();
                CreatureSpawnsFile = new Dictionary<ulong, CreatureSpawn>();

                LoadCreatureSpawns();
            }

            if (updated2)
            {
                var sb = new StringBuilder();

                foreach (var cs in CreatureSpawnsFile)
                    sb.AppendLine($"{cs.Key}, {cs.Value.Id}, {cs.Value.Map}, {cs.Value.Position.X}, {cs.Value.Position.Y}, {cs.Value.Position.Z}, {cs.Value.Position.O}, {cs.Value.Emote}");

                File.WriteAllText("./creaturespawns.txt", sb.ToString());

                CreatureSpawns = new Dictionary<ulong, CreatureSpawn>();
                CreatureSpawnsFile = new Dictionary<ulong, CreatureSpawn>();

                LoadCreatureSpawns();
            }

            if (updated3)
            {
                var sb = new StringBuilder();

                foreach (var cs in CreatureSpawnsFile)
                    sb.AppendLine($"{cs.Key}, {cs.Value.Id}, {cs.Value.Map}, {cs.Value.Position.X}, {cs.Value.Position.Y}, {cs.Value.Position.Z}, {cs.Value.Position.O}, {cs.Value.Emote}," +
                $" {cs.Value.AnimState}, {cs.Value.ModelId}, {cs.Value.Scale}, {cs.Value.EquipmentId}, {cs.Value.NpcFlags}, {cs.Value.UnitFlags}, {cs.Value.FactionTemplate}, {cs.Value.Level}, {cs.Value.MountDisplayId}, {cs.Value.HoverHeight}" +
                $", {cs.Value.Health}, {cs.Value.MaxHealth}\r\n");

                File.WriteAllText("./creaturespawns.txt", sb.ToString());

                CreatureSpawns = new Dictionary<ulong, CreatureSpawn>();
                CreatureSpawnsFile = new Dictionary<ulong, CreatureSpawn>();

                LoadCreatureSpawns();
            }

            Log.Message(LogType.Debug, "Loaded {0} creature spawns.", CreatureSpawns.Count);
        }

        public void LoadCreatureWaypoints()
        {
            string[] line;

            using (var reader = new StreamReader("./creaturepoints.txt"))
            {
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine().Split(new char[] { ',' });
                    var guid = ulong.Parse(line[0]);
                    var x = float.Parse(line[1]);
                    var y = float.Parse(line[2]);
                    var z = float.Parse(line[3]);
                    var o = float.Parse(line[4]);
                    var index = int.Parse(line[5]);
                    var waitTime = int.Parse(line[6]);

                    var p = new Waypoint
                    {
                        CreatureGuid = guid,
                        Point = new Vector4
                        {
                            X = x,
                            Y = y,
                            Z = z,
                            O = o
                        },
                        Index = index,
                        WaitTime = waitTime
                    };

                    if (!waypoints.ContainsKey(guid))
                        waypoints.Add(guid, new List<Waypoint>());

                    waypoints[guid].Add(p);
                }
            }
        }

        public void LoadPlayerSpawns()
        {
            // First characterlist init.
            if (!File.Exists(Helper.DataDirectory() + "characters.json"))
                File.Create(Helper.DataDirectory() + "characters.json").Dispose();

            Manager.WorldMgr.CharaterList = Json.CreateObject<List<Character>>(File.ReadAllText(Helper.DataDirectory() + "characters.json"));

            if (Manager.WorldMgr.CharaterList == null)
                Manager.WorldMgr.CharaterList = new List<Character>();

            string[] line;

            using (var reader = new StreamReader("./playerspawns.txt"))
            {
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine().Split(new char[] { ',' });

                    var guid = ulong.Parse(line[0]);
                    var originalGuid = ulong.Parse(line[1]);
                    var map = uint.Parse(line[2]);
                    var x = float.Parse(line[3]);
                    var y = float.Parse(line[4]);
                    var z = float.Parse(line[5]);
                    var o = float.Parse(line[6]);
                    var emote = uint.Parse(line[7]);
                    var mountId = 0u;

                    if (line.Length == 9)
                        mountId = uint.Parse(line[8]);

                    // Serialize and deserialize it to create a copy of the object.
                    var tempPlayer = Json.CreateString(Manager.WorldMgr.CharaterList.SingleOrDefault(c => c.Guid == originalGuid));

                    if (tempPlayer == null)
                    {
                        Log.Message(LogType.Error, $"No character with guid '{originalGuid}' found. Skipping...");
                        continue;
                    }

                    var player = Json.CreateObject<Character>(tempPlayer);

                    if (player == null)
                    {
                        Log.Message(LogType.Error, $"No character with guid '{originalGuid}' found. Skipping...");
                        continue;
                    }

                    player.Map = map;
                    player.Position = new Vector4
                    {
                        X = x,
                        Y = y,
                        Z = z,
                        O = o
                    };

                    player.EmoteId = emote;
                    player.MountId = mountId;

                    if (!Manager.WorldMgr.PlayerSpawns.Any(c => c.Guid == guid))
                        Manager.WorldMgr.PlayerSpawns.Add((player, guid, emote));
                }
            }
        }

        ulong lastPremadeGuid;
        public void LoadGameobjectSpawns()
        {
            if (GameObjectSpawns == null)
                GameObjectSpawns = new Dictionary<ulong, GameObjectSpawn>();

            var ctr = 1;

            // Premade spawns;
            foreach (var obj in Manager.ObjectMgr.GameObjectSpawns)
            {
                ++ctr;
                GameObjectSpawn spawn = new GameObjectSpawn()
                {
                    SGuid = new SmartGuid((ulong)ctr, obj.Value.Id, GuidType.GameObject, (uint)obj.Value.Map),
                    Guid = (ulong)ctr,
                    Id = obj.Value.Id,
                    GameObject = obj.Value,
                    Position = new Vector4 { X = obj.Value.Pos.X, Y = obj.Value.Pos.Y, Z = obj.Value.Pos.Z, O = obj.Value.Pos.O },
                    Map = (uint)obj.Value.Map,
                    State = 0,
                    SpellVisualId = 0
                };

                if (spawn.Position.X != 0)
                    GameObjectSpawns.Add((ulong)ctr, spawn);
            }

            lastPremadeGuid = (ulong)ctr;

            bool updated = false;
            string[] line;
            using (var reader = new StreamReader("./gameobjectspawns.txt"))
            {
                var cttr = 70000;
                var guid = lastPremadeGuid;

                while (!reader.EndOfStream)
                {
                    ++cttr;
                    line = reader.ReadLine().Split(new char[] { ',' });

                    if (!updated && (line.Length == 12 || line.Length == 13))
                    {
                        Log.Message(LogType.Debug, "Found old gameobjectspawns format, Updating...");
                        updated = true;
                    }

                    var id = 0;
                    var map = 0;
                    var x = 0f;
                    var y = 0f;
                    var z = 0f;
                    var o = 0f;
                    var r0 = 0f;
                    var r1 = 0f;
                    var r2 = 0f;
                    var r3 = 0f;
                    var animState = 0;
                    var state = 0;
                    var spellVisualId = 0u;

                    if (line.Length == 12)
                    {
                        id = int.Parse(line[0]);
                        map = int.Parse(line[1]);
                        x = float.Parse(line[2]);
                        y = float.Parse(line[3]);
                        z = float.Parse(line[4]);
                        o = float.Parse(line[5]);
                        r0 = float.Parse(line[6]);
                        r1 = float.Parse(line[7]);
                        r2 = float.Parse(line[8]);
                        r3 = float.Parse(line[9]);
                        animState = int.Parse(line[10]);
                        state = int.Parse(line[11]);

                        guid++;
                    }

                    if (line.Length == 13)
                    {
                        guid = ulong.Parse(line[0]);
                        id = int.Parse(line[1]);
                        map = int.Parse(line[2]);
                        x = float.Parse(line[3]);
                        y = float.Parse(line[4]);
                        z = float.Parse(line[5]);
                        o = float.Parse(line[6]);
                        r0 = float.Parse(line[7]);
                        r1 = float.Parse(line[8]);
                        r2 = float.Parse(line[9]);
                        r3 = float.Parse(line[10]);
                        animState = int.Parse(line[11]);
                        state = int.Parse(line[12]);
                    }

                    if (line.Length == 14)
                    {
                        guid = ulong.Parse(line[0]);
                        id = int.Parse(line[1]);
                        map = int.Parse(line[2]);
                        x = float.Parse(line[3]);
                        y = float.Parse(line[4]);
                        z = float.Parse(line[5]);
                        o = float.Parse(line[6]);
                        r0 = float.Parse(line[7]);
                        r1 = float.Parse(line[8]);
                        r2 = float.Parse(line[9]);
                        r3 = float.Parse(line[10]);
                        animState = int.Parse(line[11]);
                        state = int.Parse(line[12]);
                        spellVisualId = uint.Parse(line[13]);
                    }

                    var gobject = Manager.ObjectMgr.GameObjects.ContainsKey(id) ? Manager.ObjectMgr.GameObjects[id] : null;

                    if (gobject != null)
                    {
                        if (GameObjectSpawns.ContainsKey(guid))
                            guid = (ulong)new Random(Environment.TickCount).Next((int)guid + 1, int.MaxValue);

                        gobject.Rot = new Vector4 { X = r0, Y = r1, Z = r2, O = r3 };
                        gobject.Pos = new Vector4 { X = x, Y = y, Z = z, O = o };
                        GameObjectSpawn spawn = new GameObjectSpawn()
                        {
                            SGuid = new SmartGuid((ulong)guid, gobject.Id, GuidType.GameObject, (uint)map),
                            Guid = (ulong)guid,
                            Id = id,
                            GameObject = gobject,
                            Position = new Vector4 { X = x, Y = y, Z = z, O = o },
                            AnimProgress = (byte)animState,
                            State = (byte)state,
                            Map = (uint)map,
                            SpellVisualId = spellVisualId
                        };




                        GameObjectSpawns.Add((ulong)guid, spawn);
                    }
                }
            }

            if (updated)
            {
                var sb = new StringBuilder();

                foreach (var cs in GameObjectSpawns.Where(gs => gs.Key > lastPremadeGuid))
                    sb.AppendLine($"{cs.Value.Guid}, {cs.Value.Id}, {cs.Value.Map}, {cs.Value.Position.X}, {cs.Value.Position.Y}, {cs.Value.Position.Z}, {cs.Value.Position.O}, " +
                                  $"{cs.Value.GameObject.Rot.X}, {cs.Value.GameObject.Rot.Y}, {cs.Value.GameObject.Rot.Z}, {cs.Value.GameObject.Rot.O}, {cs.Value.AnimProgress}, {cs.Value.State}, {cs.Value.SpellVisualId}");

                File.WriteAllText("./gameobjectspawns.txt", sb.ToString());

                GameObjectSpawns = new Dictionary<ulong, GameObjectSpawn>();

                LoadGameobjectSpawns();

                return;
            }

            Log.Message(LogType.Debug, "Loaded {0} gameobject spawns.", GameObjectSpawns.Count);
        }
    }
}
