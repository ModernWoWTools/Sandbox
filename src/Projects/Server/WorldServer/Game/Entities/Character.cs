using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AuthServer.WorldServer.Game.Entities;
using AuthServer.WorldServer.Managers;
using AuthServer.Game.WorldEntities;
using Framework.ObjectDefines;
using System.Runtime.Serialization;
using Arctium.WoW.Sandbox.Server.WorldServer.Game.Entities;

namespace AuthServer.Game.Entities
{
    [Serializable()]
    [DataContract]
    public class Character : WorldObject
    {
        [DataMember]
        public uint AccountId;
        [DataMember]
        public string Name;
        [DataMember]
        public byte Race;
        [DataMember]
        public byte Class;
        [DataMember]
        public byte Gender;
        [DataMember]
        public byte Level;
        [DataMember]
        public uint Zone;
        [DataMember]
        public ulong GuildGuid;
        [DataMember]
        public uint PetDisplayInfo;
        [DataMember]
        public uint PetLevel;
        [DataMember]
        public uint PetFamily;
        [DataMember]
        public uint CharacterFlags;
        [DataMember]
        public uint CustomizeFlags;
        [DataMember]
        public bool LoginCinematic;
        [DataMember]
        public byte SpecGroupCount;
        [DataMember]
        public byte ActiveSpecGroup;
        [DataMember]
        public uint PrimarySpec;
        [DataMember]
        public uint SecondarySpec;
        [DataMember]
        public uint Runes;
        [DataMember]
        public uint EmoteId;
        [DataMember]
        public uint MountId;

        [DataMember]
        public List<Skill> Skills = new List<Skill>();
        [DataMember]
        public List<PlayerSpell> SpellList = new List<PlayerSpell>();
        [DataMember]
        public Dictionary<byte, Item> Equipment = new Dictionary<byte, Item>();
        [DataMember]
        public Dictionary<byte, Dictionary<byte, Item>> Bags = new Dictionary<byte, Dictionary<byte, Item>>()
        {
            { 19, new Dictionary<byte, Item>() },
            { 20, new Dictionary<byte, Item>() },
            { 21, new Dictionary<byte, Item>() },
            { 22, new Dictionary<byte, Item>() },
            { 255, new Dictionary<byte, Item>() }
        };
        // TODO: REMOVE!!!
        //public Dictionary<byte, Item> Bag = new Dictionary<byte, Item>();
        [DataMember]
        public List<ActionButton> ActionButtons = new List<ActionButton>();

        [DataMember]
        public List<CharacterCustomizationChoice> CharacterCustomizationChoices = new List<CharacterCustomizationChoice>();

        public Dictionary<ulong, WorldObject> InRangeObjects = new Dictionary<ulong, WorldObject>();

        public Character() { }
        public Character(ulong guid, int updateLength = (int)ActivePlayerFields.End) : base(updateLength)
        {
            foreach (var c in Manager.WorldMgr.CharaterList)
            {
                if (c.Guid == guid)
                {
                    Guid = c.Guid;
                    AccountId = 1;
                    Name = c.Name;
                    Race = c.Race;
                    Class = c.Class;
                    Gender = c.Gender;
                    Level = c.Level;
                    Zone = c.Zone;
                    GuildGuid = c.GuildGuid;
                    PetDisplayInfo = c.PetDisplayInfo;
                    PetLevel = c.PetLevel;
                    PetFamily = c.PetFamily;
                    CharacterFlags = c.CharacterFlags;
                    CustomizeFlags = c.CustomizeFlags;
                    LoginCinematic = c.LoginCinematic;
                    SpecGroupCount = c.SpecGroupCount;
                    ActiveSpecGroup = c.ActiveSpecGroup;
                    PrimarySpec = c.PrimarySpec;
                    SecondarySpec = c.SecondarySpec;
                    Position = c.Position;
                    Map = c.Map;
                    break;
                }
            }


        }

        public void AddSpellSkills()
        {
            // Vulperaa, orcish
            if (Race == 35)
                SpellList.Add(new PlayerSpell
                {
                    SpellId = 312769,
                    State = PlayerSpellState.Unchanged,
                });


            SpellList.Add(new PlayerSpell
            {
                SpellId = 79738,
                State = PlayerSpellState.Unchanged,
            });

            SpellList.Add(new PlayerSpell
            {
                SpellId = 668,
                State = PlayerSpellState.Unchanged,
            });
            SpellList.Add(new PlayerSpell
            {
                SpellId = 669,
                State = PlayerSpellState.Unchanged,
            });

            //if (Race == 2)
            SpellList.Add(new PlayerSpell
            {
                SpellId = 79743,
                State = PlayerSpellState.Unchanged,
            });


            // CLoth Armor
            SpellList.Add(new PlayerSpell
            {
                SpellId = 9078,
                State = PlayerSpellState.Unchanged,
            });

            if (Class == 12)
                // SpellList.Add(new PlayerSpell { SpellId = 197130, State = PlayerSpellState.Unchanged });
                SpellList.Add(new PlayerSpell { SpellId = 131347, State = PlayerSpellState.Unchanged });

            // Winning Hand
            //SpellList.Add(new PlayerSpell { SpellId = 176890, State = PlayerSpellState.Unchanged });

            if (Race == 26 || Race == 25 || Race == 24)
            {
                Skills.Add(new Skill
                {
                    Id = 905,
                    SkillLevel = 300
                });


                SpellList.Add(new PlayerSpell { SpellId = 108127, State = PlayerSpellState.Unchanged });
                SpellList.Add(new PlayerSpell { SpellId = 131701, State = PlayerSpellState.Unchanged });
            }


            Skills.Add(new Skill
            {
                Id = 197,
                SkillLevel = 300
            });

            // language skills
            Skills.Add(new Skill
            {
                Id = 98,
                SkillLevel = 300
            });

            Skills.Add(new Skill
            {
                Id = 109,
                SkillLevel = 300
            });

            Skills.Add(new Skill
            {
                Id = 790,
                SkillLevel = 0
            });


            //weapon.armor
            Skills.Add(new Skill
            {
                Id = 44,
                SkillLevel = 300
            });

            Skills.Add(new Skill
            {
                Id = 45,
                SkillLevel = 300
            });

            Skills.Add(new Skill
            {
                Id = 415,
                SkillLevel = 300
            });
            Skills.Add(new Skill
            {
                Id = 226,
                SkillLevel = 300
            });
            Skills.Add(new Skill
            {
                Id = 173,
                SkillLevel = 300
            });
            Skills.Add(new Skill
            {
                Id = 473,
                SkillLevel = 300
            });
            Skills.Add(new Skill
            {
                Id = 414,
                SkillLevel = 300
            });
            Skills.Add(new Skill
            {
                Id = 54,
                SkillLevel = 300
            });
            Skills.Add(new Skill
            {
                Id = 413,
                SkillLevel = 300
            });
            Skills.Add(new Skill
            {
                Id = 293,
                SkillLevel = 300
            });
            Skills.Add(new Skill
            {
                Id = 229,
                SkillLevel = 300
            });
            Skills.Add(new Skill
            {
                Id = 433,
                SkillLevel = 300
            });
            Skills.Add(new Skill
            {
                Id = 136,
                SkillLevel = 300
            });
            Skills.Add(new Skill
            {
                Id = 43,
                SkillLevel = 300
            });
            Skills.Add(new Skill
            {
                Id = 172,
                SkillLevel = 300
            });
            Skills.Add(new Skill
            {
                Id = 160,
                SkillLevel = 300
            });
            Skills.Add(new Skill
            {
                Id = 55,
                SkillLevel = 300
            });
            Skills.Add(new Skill
            {
                Id = 228,
                SkillLevel = 300
            });

            if (Class == 12)
            {
                SpellList.Add(new PlayerSpell
                {
                    SpellId = 202782,
                    State = PlayerSpellState.Unchanged,
                });

                SpellList.Add(new PlayerSpell
                {
                    SpellId = 202783,
                    State = PlayerSpellState.Unchanged,
                });

                SpellList.Add(new PlayerSpell
                {
                    SpellId = 195304,
                    State = PlayerSpellState.Unchanged,
                });//196055
                SpellList.Add(new PlayerSpell
                {
                    SpellId = 196055,
                    State = PlayerSpellState.Unchanged,
                });
                Skills.Add(new Skill
                {
                    Id = 1848,
                    SkillLevel = 300
                });

                Skills.Add(new Skill
                {
                    Id = 2152,
                    SkillLevel = 300
                });
            }

            // Mount/Fly
            Skills.Add(new Skill
            {
                Id = 777,
                SkillLevel = 300
            });


            Skills.Add(new Skill
            {
                Id = 762,
                SkillLevel = 300
            });

            // Flying spells
            SpellList.Add(new PlayerSpell
            {
                SpellId = 33388,
                State = PlayerSpellState.Unchanged,
            });

            SpellList.Add(new PlayerSpell
            {
                SpellId = 33391,
                State = PlayerSpellState.Unchanged,
            });

            SpellList.Add(new PlayerSpell
            {
                SpellId = 34090,
                State = PlayerSpellState.Unchanged,
            });

            SpellList.Add(new PlayerSpell
            {
                SpellId = 34091,
                State = PlayerSpellState.Unchanged,
            });

            SpellList.Add(new PlayerSpell
            {
                SpellId = 90265,
                State = PlayerSpellState.Unchanged,
            });

            SpellList.Add(new PlayerSpell
            {
                SpellId = 90267,
                State = PlayerSpellState.Unchanged,
            });

            SpellList.Add(new PlayerSpell
            {
                SpellId = 54197,
                State = PlayerSpellState.Unchanged,
            });

            SpellList.Add(new PlayerSpell
            {
                SpellId = 115913,
                State = PlayerSpellState.Unchanged,
            });

            // titan grip, warrior
            if (Class == 1)
            {
                SpellList.Add(new PlayerSpell { SpellId = 46917, State = PlayerSpellState.Unchanged });
            }

            // prevent duplicates.
            Skills = Skills.GroupBy(x => x.Id).Select(x => x.First()).ToList();
            SpellList = SpellList.GroupBy(x => x.SpellId).Select(x => x.First()).ToList();
        }

        bool IsHorde()
        {
            // 10, 9, 2, 26, 6, 8, 5
            return Race == 10 || Race == 9 || Race == 2 || Race == 26 || Race == 6 ||
                Race == 8 || Race == 5 || Race == 27 || Race == 28 || Race == 21 || Race == 35;
        }

        public uint Faction;

        public override void SetUpdateFields()
        {
            UpdateData = new Hashtable();
            Mask = new BitArray((int)ActivePlayerFields.End, false);
            var guid = new SmartGuid { Type = GuidType.Player, CreationBits = Guid, RealmId = 1 };
            // 0DB90000000000000000000000280F08
            //SetUpdateField<ulong>((int)ObjectFields.Guid, guid.Guid);
            //SetUpdateField<uint>((int)ObjectFields.Guid + 2, 0x00000000);
            //SetUpdateField<ulong>((int)ObjectFields.Guid + 2, guid.HighGuid);//   280F00);
            //SetUpdateField<int>((int)ObjectFields.HeirTypeFlags, 0x61 | 0x1 | 0x21);
            SetUpdateField<float>((int)ObjectFields.Scale, 1.0f);

            //SetUpdateField<int>((int)UnitFields.Health, 1);
            //SetUpdateField<int>((int)UnitFields.MaxHealth, 1);

            //SetUpdateField<int>((int)UnitFields.SandboxScalingID, 2402);
            SetUpdateField<int>((int)ActivePlayerFields.NumBackpackSlots, 0x00140000);
            Faction = Manager.WorldMgr.ChrRaces.Single(r => r.Id == Race).Faction;



            //SetUpdateField<int>((int)UnitFields.Level, Level);
            //SetUpdateField<int>((int)ActivePlayerFields.MaxLevel, 120);




            SetUpdateField<uint>((int)UnitFields.FactionTemplate, Faction);

            SetUpdateField<byte>((int)UnitFields.Sex, Race, 0);
            SetUpdateField<byte>((int)UnitFields.Sex, Class, 1);
            SetUpdateField<byte>((int)UnitFields.Sex, 0, 2);
            SetUpdateField<byte>((int)UnitFields.Sex, Gender, 3);

            var powerTypes = new Dictionary<int, int>
            {
                { 1, 1 },
                { 2, 0 },
                { 3, 2 },
                { 4, 3 },
                { 5, 0 },
                { 6, 6 },
                { 7, 0 },
                { 8, 0 },
                { 9, 0 },
                { 10, 3 },
                { 11, 0 },
                { 12, 17 },
            };

            uint[][] _powersByClass = new uint[13][];

            for (uint i = 0; i < (int)13; ++i)
            {
                _powersByClass[i] = new uint[(int)19];

                for (uint j = 0; j < (int)19; ++j)
                    _powersByClass[i][j] = (uint)19;
            }

            foreach (var dp in Manager.WorldMgr.DefaultPowerTypes)
            {
                uint index = 0;
                for (var i = 0; i < 19; i++)
                {
                    if (_powersByClass[dp.Key][i] != 19)
                        ++index;

                    _powersByClass[dp.Key][dp.Value] = index;
                }
            }

            SetUpdateField((int)UnitFields.DisplayPower, powerTypes[Class]);


            var maxPowers = 0;
            for (int i = 0; i < 19; i++)
            {
                if (maxPowers >= 6)
                    break;

                if (_powersByClass[Class][i] != 19)
                {
                    SetUpdateField((int)UnitFields.Power + (int)_powersByClass[Class][i], 1);
                    SetUpdateField((int)UnitFields.MaxPower + (int)_powersByClass[Class][i], 1);
                    maxPowers++;
                }
            }


            var race = Manager.WorldMgr.ChrRaces.Single(r => r.Id == Race);
            var displayId = Gender == 0 ? race.MaleDisplayId : race.FemaleDisplayId;

            if (race.Id == 22 || race.Id == 23)
                displayId = Gender == 0 ? 29422u : 29423;
            //SetUpdateField((int)ObjectFields.DisplayId, displayId);
            //SetUpdateField<uint>((int)UnitFields.DisplayID, displayId);
            //SetUpdateField<uint>((int)UnitFields.NativeDisplayID, displayId);
            //SetUpdateField<uint>((int)UnitFields.Flags, 0x8);
            //SetUpdateField<uint>((int)UnitFields.Flags2, 2048);

            //SetUpdateField<float>((int)UnitFields.BoundingRadius, 0.389F);
            //SetUpdateField<float>((int)UnitFields.CombatReach, 1.5F);
            // SetUpdateField<float>((int)UnitFields.ModCastingSpeed, 1);
            //SetUpdateField<float>((int)UnitFields.MaxHealthModifier, 1);
            //SetUpdateField<uint>((int)UnitFields.HoverHeight, 1);
            SetUpdateField<int>((int)ActivePlayerFields.RestInfo, (0 << 24 | 0 << 16 | 0 << 8 | 2));
            SetUpdateField<int>((int)PlayerFields.ArenaFaction, 0);
            //SetUpdateField<int>((int)PlayerFields.Inebriation, 65536);
            SetUpdateField<byte>((int)PlayerFields.Inebriation, 0, 0);
            SetUpdateField<byte>((int)PlayerFields.Inebriation, 0, 1);
            SetUpdateField<byte>((int)PlayerFields.Inebriation, (byte)Gender, 2);
            SetUpdateField<byte>((int)PlayerFields.Inebriation, 0, 3);

            SetUpdateField<int>((int)ActivePlayerFields.WatchedFactionIndex, -1);
            SetUpdateField<int>((int)ActivePlayerFields.XP, 0);
            SetUpdateField<int>((int)ActivePlayerFields.LocalFlags, 8);

            SetUpdateField<int>((int)ActivePlayerFields.NextLevelXP, 400);
            SetUpdateField<uint>((int)UnitFields.MountDisplayID, 0);

            SetUpdateField<int>((int)ActivePlayerFields.CharacterPoints, 0);

            //SetUpdateField<int>((int)ActivePlayerFields.Prestige, 50);
            //SetUpdateField<int>((int)ActivePlayerFields.HonorLevel, 50);
            SetUpdateField<int>((int)ActivePlayerFields.HonorNextLevel, 350);
            SetUpdateField((int)ActivePlayerFields.Mastery, 1f);
            SetUpdateField<int>((int)ActivePlayerFields.MaxTalentTiers, 21);
            //SetUpdateField<int>((int)UnitFields.AuraState + 0, 4194304);

            for (int i = 0; i < 320; i++)
                SetUpdateField<uint>((int)ActivePlayerFields.ExploredZones + i, uint.MaxValue);

            //SetUpdateField<int>((int)UnitFields.Power + 1, 100);
            //SetUpdateField<int>((int)UnitFields.Power + 2, 1000);

            //for (int i = 0; i < 128; i++)

            for (int i = 0; i < 128; i++)
            {
                if (i < Skills.Count)
                {
                    SetUpdateField<int>((int)ActivePlayerFields.Skill + i, (ushort)Skills[i].Id);
                    SetUpdateField<int>((int)ActivePlayerFields.Skill + i + 64, 4);
                    SetUpdateField<ushort>((int)ActivePlayerFields.Skill + i + 128, 300);
                    // SetUpdateField<ushort>((int)ActivePlayerFields.Skill + i + 192, 300);
                    SetUpdateField<int>((int)ActivePlayerFields.Skill + i + 256, 300);
                    SetUpdateField<int>((int)ActivePlayerFields.Skill + i + 320, 0);
                    SetUpdateField<int>((int)ActivePlayerFields.Skill + i + 384, 0);
                }
                else
                {

                }
            }

            if (Race == 24)
            {
                SetUpdateField<int>((int)ActivePlayerFields.Skill, (int)8978568);//1.258165E-38
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 1, (int)10485899);//1.469387E-38
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 2, (int)10748066);//1.506125E-38
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 3, (int)11206821);//1.57041E-38
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 4, (int)11337900);//1.588778E-38
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 5, (int)11993270);//1.680615E-38
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 6, (int)12189881);//1.708166E-38
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 7, (int)13238469);//1.855105E-38
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 8, (int)20644153);//3.434735E-38
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 9, (int)23331149);//4.187792E-38
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 10, (int)27066761);//5.767489E-38
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 11, (int)27197854);//5.84097E-38
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 12, (int)30998961);//7.971564E-38
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 13, (int)49480353);//3.570715E-37
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 14, (int)49939191);//3.67359E-37
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 15, (int)50660097);//3.908864E-37
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 16, (int)50987785);//4.055804E-37
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 17, (int)51905303);//4.467233E-37
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 18, (int)53084954);//4.996207E-37
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 19, (int)58917685);//7.700224E-37
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 20, (int)60556169);//9.169668E-37
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 21, (int)63898534);//1.21672E-36
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 22, (int)64029648);//1.228479E-36
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 23, (int)64160722);//1.240234E-36
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 24, (int)64291796);//1.251989E-36
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 25, (int)119931862);//1.248958E-34
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 26, (int)131073945);//3.130083E-34
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 89, (int)65536);//9.18355E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 124, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 125, (int)1);//1.401298E-45
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 128, (int)1);//1.401298E-45
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 129, (int)65536);//9.18355E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 130, (int)1);//1.401298E-45
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 132, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 133, (int)327680);//4.591775E-40
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 138, (int)65536);//9.18355E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 139, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 140, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 144, (int)1);//1.401298E-45
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 146, (int)65536);//9.18355E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 147, (int)327680);//4.591775E-40
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 148, (int)327980);//4.595979E-40
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 149, (int)1);//1.401298E-45
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 153, (int)65536);//9.18355E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 188, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 189, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 190, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 191, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 192, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 193, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 194, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 195, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 196, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 197, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 198, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 199, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 200, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 201, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 202, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 203, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 204, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 205, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 206, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 207, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 208, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 209, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 210, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 211, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 212, (int)65836);//9.225589E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 213, (int)34734081);//1.072639E-37
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 214, (int)34734610);//1.072698E-37
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 215, (int)34734610);//1.072698E-37
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 216, (int)3277330);//4.592517E-39
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 217, (int)65661);//9.201066E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 218, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 252, (int)327685);//4.591845E-40
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 253, (int)5);//7.006492E-45
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 255, (int)5);//7.006492E-45
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 256, (int)5);//7.006492E-45
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 257, (int)327680);//4.591775E-40
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 258, (int)5);//7.006492E-45
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 260, (int)327685);//4.591845E-40
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 261, (int)327680);//4.591775E-40
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 266, (int)65536);//9.18355E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 267, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 268, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 271, (int)5);//7.006492E-45
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 272, (int)65537);//9.18369E-41
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 274, (int)327680);//4.591775E-40
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 275, (int)327681);//4.591789E-40
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 276, (int)327980);//4.595979E-40
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 277, (int)5);//7.006492E-45
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 281, (int)4915200);//6.887662E-39
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 282, (int)327680);//4.591775E-40
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 390, (int)15);//2.101948E-44
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 405, (int)983040);//1.377532E-39
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 406, (int)983055);//1.377553E-39
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 407, (int)983055);//1.377553E-39
                SetUpdateField<int>((int)ActivePlayerFields.Skill + 408, (int)15);//2.101948E-44
            }

            SetUpdateField<uint>((int)PlayerFields.VirtualPlayerRealm, 1);

            /*foreach (var kp in Equipment)
            {
                SetUpdateField<ulong>((int)PlayerFields.InvSlots + (kp.Key * 4), 0);
                SetUpdateField<ulong>((int)PlayerFields.InvSlots + (kp.Key * 4) + 2, 0);
                SetUpdateField<ulong>((int)PlayerFields.InvSlots + (kp.Key * 4), kp.Value.Guid);
                SetUpdateField<ulong>((int)PlayerFields.InvSlots + (kp.Key * 4) + 2, 0x0C0F280000000000);

                SetUpdateField<int>((int)PlayerFields.VisibleItems + (kp.Key * 3), 0);
                SetUpdateField<int>((int)PlayerFields.VisibleItems + (kp.Key * 3), kp.Value.Id);
            }

            foreach (var kp in Bag)
            {
                SetUpdateField<ulong>((int)PlayerFields.InvSlots + (kp.Key * 4), kp.Value.Guid);
                SetUpdateField<ulong>((int)PlayerFields.InvSlots + (kp.Key * 4) + 2, 0x0C0F280000000000);
            }*/

            SetUpdateField<int>((int)ActivePlayerFields.ModDamageDonePercent, 2);
            SetUpdateField<int>((int)UnitFields.AttackRoundBaseTime, 2000);
            SetUpdateField<int>((int)UnitFields.RangedAttackRoundBaseTime, 2000);
            //SetUpdateField<int>((int)UnitFields.ModCastingSpeed, 42);

            SetUpdateField<float>((int)UnitFields.MinDamage, 1337);
            SetUpdateField<float>((int)UnitFields.MaxDamage, 1337);
            SetUpdateField<float>((int)UnitFields.MinOffHandDamage, 42);
            SetUpdateField<float>((int)UnitFields.MaxOffHandDamage, 42);
            SetUpdateField<float>((int)ActivePlayerFields.Speed, 7);
            SetUpdateField<int>((int)ActivePlayerFields.OffhandExpertise, 7);


            SetUpdateField<uint>((int)UnitFields.AnimTier, 65536);
            SetUpdateField<int>((int)UnitFields.Stats + 0, 1);
            SetUpdateField<int>((int)UnitFields.Stats + 1, 1);
            SetUpdateField<int>((int)UnitFields.Stats + 2, 1);
            SetUpdateField<int>((int)UnitFields.Stats + 3, 1);

            SetUpdateField<int>((int)UnitFields.StatPosBuff + 1, 0);
            SetUpdateField<int>((int)UnitFields.StatPosBuff + 2, 0);
            SetUpdateField<int>((int)UnitFields.Resistances, 1);
            SetUpdateField<int>((int)UnitFields.AttackPower, 1);
            SetUpdateField<float>((int)UnitFields.ScaleDuration, 100);
            SetUpdateField<int>((int)UnitFields.LookAtControllerID, -1);

            //SetUpdateField<byte>((int)PlayerFields.CustomDisplayOption, 0, 1);
            //SetUpdateField<byte>((int)PlayerFields.CustomDisplayOption, 0, 2);
            //SetUpdateField<byte>((int)PlayerFields.CustomDisplayOption, 0, 3);

            SetUpdateField<uint>((int)PlayerFields.PlayerTitle, 0);


            if (PrimarySpec == 0)
            {
                PrimarySpec = Manager.WorldMgr.DefaultChrSpec[Class];
            }
            SetUpdateField<uint>((int)PlayerFields.CurrentSpecID, PrimarySpec);
            SetUpdateField((int)ActivePlayerFields.NumRespecs, 0);


            SetUpdateField<float>((int)PlayerFields.AvgItemLevel, 1f);
            SetUpdateField<float>((int)PlayerFields.AvgItemLevel + 1, 1f);
            SetUpdateField<float>((int)PlayerFields.AvgItemLevel + 2, 1f);
            SetUpdateField<float>((int)PlayerFields.AvgItemLevel + 3, 1f);

            SetUpdateField<uint>((int)ActivePlayerFields.NumRespecs, 0);
            // 
            //             for (int i = 0; i < 172; i++)
            //                 SetUpdateField<uint>((int)PlayerFields.InvSlots + i, 0);


            // SetGuidValue(PLAYER_FIELD_INV_SLOT_HEAD + (slot * 2), pItem->GetObjectGuid());

            //SetUInt32Value(PLAYER_VISIBLE_ITEM_1_ENTRYID + (slot * 2), pItem->GetEntry());
        }

        public static string NormalizeName(string name)
        {
            return name[0].ToString().ToUpper() + name.Remove(0, 1).ToLower();
        }

    }
}
