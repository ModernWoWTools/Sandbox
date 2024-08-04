using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arctium.WoW.Sandbox.Server.WorldServer.Game.Entities;
using AuthServer.Network;
using AuthServer.WorldServer.Managers;

using Framework.Constants.Misc;
using Framework.Constants.Net;
using Framework.Logging;
using Framework.Misc;
using Framework.Network.Packets;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Arctium.WoW.Sandbox.Server.WorldServer.Managers
{
    public class HotfixManager : Singleton<HotfixManager>
    {
        const string HotfixFolder = "hotfixes";
        const string HotfixFileExtension = ".json";

        public Dictionary<uint, Dictionary<uint, List<string>>> Hotfixes { get; }

        HotfixManager()
        {
            Hotfixes = new Dictionary<uint, Dictionary<uint, List<string>>>();

#if DEBUG
            if (!Directory.Exists("./hotfixes"))
                Directory.CreateDirectory("./hotfixes");
#endif
        }

        public async Task Load()
        {
#if DEBUG
            Log.Message(LogType.Info, $"Loading hotfixes...");

            foreach (var f in Directory.GetFiles($"{HotfixFolder}", $"*{HotfixFileExtension}"))
            {
                var hotfixName = Path.GetFileNameWithoutExtension(f);

                var fileContent = File.ReadAllText(f);
                dynamic hotfixObject = JsonConvert.DeserializeObject(fileContent);
                var dbInfo = Manager.WorldMgr.DBInfo[hotfixName];

                Hotfixes.Add(dbInfo.TableHash, new Dictionary<uint, List<string>>());

                foreach (JObject entry in hotfixObject)
                {
                    var values = new List<string>();
                    var properties = entry.Properties().Skip(1);

                    foreach (JProperty property in properties)
                    {
                        if (property.Value is JArray items)
                        {
                            foreach (var item in items)
                                values.Add(item.ToString());
                        }
                        else
                            values.Add(property.Value.ToString());
                    }

                    Hotfixes[dbInfo.TableHash].Add(uint.Parse(entry.Property("Id").Value.ToString()), values);
                }

                Log.Message(LogType.Info, $" - {hotfixName}: {Hotfixes[dbInfo.TableHash].Count}");
            }
#endif
        }

        public void Clear()
        {
            Hotfixes.Clear();
        }

        public void SendClearHotfixes(WorldClass session)
        {
#if DEBUG
            Log.Message(LogType.Info, $"Cleaning hotfix cache...");

            foreach (var table in Hotfixes)
            {
                var hotfixMessage = new PacketWriter(ServerMessage.HotfixMessage);

                hotfixMessage.WriteInt32(table.Value.Count);

                foreach (var hotfix in table.Value)
                {
                    hotfixMessage.Write(hotfix.Key + 838926338);
                    hotfixMessage.Write(hotfix.Key + 838926338);
                    hotfixMessage.Write(table.Key);
                    hotfixMessage.Write(hotfix.Key);
                    hotfixMessage.Write(0);
                    hotfixMessage.Write((byte)0); // allow
                }

                hotfixMessage.Write(0);

                session.Send(ref hotfixMessage);
            }

            Log.Message(LogType.Info, $"Done");
#endif
        }

        public void SendAvailableHotfixes(WorldClass session)
        {
#if DEBUG
            Log.Message(LogType.Info, $"Sending available hotfixes...");

            foreach (var table in Hotfixes)
            {
                var availableHotfixes = new PacketWriter(ServerMessage.AvailableHotfixes);

                availableHotfixes.WriteInt32(838926338); // TODO: Implement versions.
                availableHotfixes.WriteInt32(table.Value.Count);

                foreach (var hotfix in table.Value)
                {
                    availableHotfixes.WriteUInt32(table.Key);
                    availableHotfixes.WriteUInt32(hotfix.Key);
                    availableHotfixes.WriteUInt32(hotfix.Key + 838926338);
                }

                session.Send(ref availableHotfixes);
            }

            Log.Message(LogType.Info, $"Done");
#endif
        }

        public void SendHotfixReply(WorldClass session, uint tableHash, uint recordId)
        {
#if DEBUG
            if (Hotfixes.TryGetValue(tableHash, out var table) && table.TryGetValue(recordId, out var hotfix))
            {
                // Copy hotfix data.
                var fields = hotfix.ToList();
                var dbInfo = Manager.WorldMgr.DBInfo.SingleOrDefault(db => db.Value.TableHash == tableHash).Value;

                if (dbInfo != null)
                {
                    if (!dbInfo.HasIndex)
                        fields.Insert(dbInfo.IDPosition, recordId.ToString());

                    // Write the hotfix row data.
                    var hotfixRow = new BinaryWriter(new MemoryStream());

                    for (var i = 0; i < fields.Count; i++)
                    {
                        switch (dbInfo.FieldTypes[i].ToLower())
                        {
                            case "string":
                                var sBytes = Encoding.UTF8.GetBytes(fields[i]);

                                hotfixRow.Write(sBytes, 0, sBytes.Length);
                                hotfixRow.Write((byte)0);
                                break;
                            case "sbyte":
                                hotfixRow.Write(sbyte.Parse(fields[i]));
                                break;
                            case "byte":
                                hotfixRow.Write(byte.Parse(fields[i]));
                                break;
                            case "int16":
                                hotfixRow.Write(short.Parse(fields[i]));
                                break;
                            case "uint16":
                                hotfixRow.Write(ushort.Parse(fields[i]));
                                break;
                            case "int32":
                                hotfixRow.Write(int.Parse(fields[i]));
                                break;
                            case "uint32":
                                hotfixRow.Write(uint.Parse(fields[i]));
                                break;
                            case "single":
                                hotfixRow.Write(float.Parse(fields[i], NumberStyles.Any, CultureInfo.InvariantCulture));
                                break;
                            case "int64":
                                hotfixRow.Write(long.Parse(fields[i]));
                                break;
                            case "uint64":
                                hotfixRow.Write(ulong.Parse(fields[i]));
                                break;
                            case "ref":
                                hotfixRow.Write(uint.Parse(fields[i]));
                                break;
                            default:
                                Log.Message(LogType.Error, "Unknown field type for hotfixes.");
                                break;
                        }
                    }

                    var dbReply = new PacketWriter(ServerMessage.DBReply);

                    dbReply.Write(tableHash);
                    dbReply.Write(recordId);
                    dbReply.WriteUInt32(recordId + 838926338);
                    dbReply.Write((byte)0x80);

                    var hotfixRowData = (hotfixRow.BaseStream as MemoryStream).ToArray();

                    dbReply.Write(hotfixRowData.Length);
                    dbReply.Write(hotfixRowData);

                    session.Send(ref dbReply);
                }
            }
#endif
        }

        public void SendHotfixMessage(WorldClass session)
        {
            Log.Message(LogType.Info, $"Sending hotfixes...");

            Clear();
            Load().ConfigureAwait(false).GetAwaiter().GetResult();

            foreach (var table in Hotfixes)
            {
                var hotfixMessage = new PacketWriter(ServerMessage.HotfixMessage);

                hotfixMessage.WriteInt32(table.Value.Count);

                var hotfixData = new BinaryWriter(new MemoryStream());

                foreach (var hotfix in table.Value)
                {
                    // Copy hotfix data.
                    var fields = hotfix.Value.ToList();
                    var dbInfo = Manager.WorldMgr.DBInfo.SingleOrDefault(db => db.Value.TableHash == table.Key).Value;

                    if (dbInfo != null)
                    {
                        if (!dbInfo.HasIndex)
                            fields.Insert(dbInfo.IDPosition, hotfix.Key.ToString());

                        // Write the hotfix row data.
                        var hotfixRow = new BinaryWriter(new MemoryStream());

                        for (var i = 0; i < fields.Count; i++)
                        {
                            switch (dbInfo.FieldTypes[i].ToLower())
                            {
                                case "string":
                                    var sBytes = Encoding.UTF8.GetBytes(fields[i]);

                                    hotfixRow.Write(sBytes, 0, sBytes.Length);
                                    hotfixRow.Write((byte)0);
                                    break;
                                case "sbyte":
                                {
                                    if (sbyte.TryParse(fields[i], out var signedResult))
                                        hotfixRow.Write(signedResult);
                                    else if (byte.TryParse(fields[i], out var unsignedResult))
                                        hotfixRow.Write(unsignedResult);
                                    else
                                        throw new InvalidDataException("sbyte || byte");

                                    break;
                                }
                                case "byte":
                                {
                                    if (byte.TryParse(fields[i], out var unsignedResult))
                                        hotfixRow.Write(unsignedResult);
                                    else if (sbyte.TryParse(fields[i], out var signedResult))
                                        hotfixRow.Write(signedResult);
                                    else
                                        throw new InvalidDataException("sbyte || byte");

                                    break;
                                }
                                case "int16":
                                {
                                    if (short.TryParse(fields[i], out var signedResult))
                                        hotfixRow.Write(signedResult);
                                    else if (ushort.TryParse(fields[i], out var unsignedResult))
                                        hotfixRow.Write(unsignedResult);
                                    else
                                        throw new InvalidDataException("int16 || uint16");

                                    break;
                                }
                                case "uint16":
                                {
                                    if (ushort.TryParse(fields[i], out var unsignedResult))
                                        hotfixRow.Write(unsignedResult);
                                    else if (short.TryParse(fields[i], out var signedResult))
                                        hotfixRow.Write(signedResult);
                                    else
                                        throw new InvalidDataException("int16 || uint16");

                                    break;
                                }
                                case "int32":
                                {
                                    if (int.TryParse(fields[i], out var signedResult))
                                        hotfixRow.Write(signedResult);
                                    else if (uint.TryParse(fields[i], out var unsignedResult))
                                        hotfixRow.Write(unsignedResult);
                                    else
                                        throw new InvalidDataException("int32 || uint32");

                                    break;
                                }
                                case "uint32":
                                {
                                    if (uint.TryParse(fields[i], out var unsignedResult))
                                        hotfixRow.Write(unsignedResult);
                                    else if (int.TryParse(fields[i], out var signedResult))
                                        hotfixRow.Write(signedResult);
                                    else
                                        throw new InvalidDataException("int32 || uint32");

                                    break;
                                }
                                case "single":
                                    hotfixRow.Write(float.Parse(fields[i], NumberStyles.Any, CultureInfo.InvariantCulture));
                                    break;
                                case "int64":
                                    hotfixRow.Write(long.Parse(fields[i]));
                                    break;
                                case "uint64":
                                    hotfixRow.Write(ulong.Parse(fields[i]));
                                    break;
                                case "ref":
                                    hotfixRow.Write(uint.Parse(fields[i]));
                                    break;
                                default:
                                    Log.Message(LogType.Error, "Unknown field type for hotfixes.");
                                    break;
                            }
                        }

                        var hotfixRowData = (hotfixRow.BaseStream as MemoryStream).ToArray();
                        var bitPack = new BitPack(hotfixMessage);

                        hotfixMessage.Write(hotfix.Key + 838926338);
                        hotfixMessage.Write(hotfix.Key + 838926338);
                        hotfixMessage.Write(table.Key);
                        hotfixMessage.Write(hotfix.Key);
                        hotfixMessage.Write(hotfixRowData.Length);

                        // Allow the hotfix.
                        // 0 - Invalid, 1 - Valid, 2 - ???, 3 - Skip
                        bitPack.Write(1, 3);
                        bitPack.Flush();

                        // Write the hotfix data to the stream.
                        hotfixData.Write(hotfixRowData);
                    }
                }

                var hotfixDataArray = (hotfixData.BaseStream as MemoryStream).ToArray();

                hotfixMessage.Write(hotfixDataArray.Length);
                hotfixMessage.Write(hotfixDataArray);

                session.Send(ref hotfixMessage);
            }
        }

        public Dictionary<uint, GameObjectDisplayInfo> GameObjectFileHotfixes = new Dictionary<uint, GameObjectDisplayInfo>();

        public void CreateGameObjectDisplayEntry(WorldClass session)
        {
            // Just send all hotfixes.
            foreach (var go in GameObjectFileHotfixes)
            {
                var hotfixMessage = new PacketWriter(ServerMessage.HotfixMessage);

                hotfixMessage.WriteInt32(GameObjectFileHotfixes.Count);

                var hotfixData = new BinaryWriter(new MemoryStream());

                // Write the hotfix row data.
                var hotfixRow = new BinaryWriter(new MemoryStream());

                foreach (var gb in go.Value.GetBox)
                    hotfixRow.Write(gb);

                hotfixRow.Write(go.Value.FileId);
                hotfixRow.Write(go.Value.ObjectEffectPackageID);
                hotfixRow.Write(go.Value.OverrideLootEffectScale);
                hotfixRow.Write(go.Value.OverrideNameScale);

                var hotfixRowData = (hotfixRow.BaseStream as MemoryStream).ToArray();
                var bitPack = new BitPack(hotfixMessage);

                hotfixMessage.Write(-1);
                hotfixMessage.Write(-1);
                hotfixMessage.Write(1829768651);
                hotfixMessage.Write(go.Value.Id);
                hotfixMessage.Write(hotfixRowData.Length);

                // Allow the hotfix.
                // 0 - Invalid, 1 - Valid, 2 - ???, 3 - Skip
                bitPack.Write(1, 3);
                bitPack.Flush();

                // Write the hotfix data to the stream.
                hotfixData.Write(hotfixRowData);

                var hotfixDataArray = (hotfixData.BaseStream as MemoryStream).ToArray();

                hotfixMessage.Write(hotfixDataArray.Length);
                hotfixMessage.Write(hotfixDataArray);

                session.Send(ref hotfixMessage);
            }
        }
    }
}
