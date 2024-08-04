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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using AuthServer.Game.Entities;
using AuthServer.Network;
using AuthServer.WorldServer.Game.Chat.Commands;
using AuthServer.WorldServer.Game.Entities;
using AuthServer.WorldServer.Managers;
using Framework.Constants;
using Framework.Constants.Misc;
using Framework.Constants.Net;
using Framework.Logging;
using Framework.Misc;
using Framework.Network.Packets;
using Framework.ObjectDefines;

namespace AuthServer.Game.Packets.PacketHandler
{
    public class ObjectHandler : Manager
    {
        public static void HandleUpdateObjectCreate(ref WorldClass session, bool tele = false, bool comment = false)
        {
            WorldObject character = session.Character;

            (character as Character).InRangeObjects.Clear();
            PacketWriter updateObject = new PacketWriter(ServerMessage.ObjectUpdate);
            BitPack BitPack = new BitPack(updateObject);

            if (session.Character.Bags == null)
            {
                session.Character.Bags = new Dictionary<byte, Dictionary<byte, Item>>()
                {
                    { 19, new Dictionary<byte, Item>() },
                    { 20, new Dictionary<byte, Item>() },
                    { 21, new Dictionary<byte, Item>() },
                    { 22, new Dictionary<byte, Item>() },
                    { 255, new Dictionary<byte, Item>() }
                };
            }

            if (session.Character.Equipment == null)
                session.Character.Equipment = new Dictionary<byte, Item>();

            // Create all items in all bags!
            foreach (var bag in session.Character.Bags)
            {
                foreach (var kp in bag.Value)
                {
                    PlayerCommands.bagSlotsList[kp.Key] = true;
                    var itemGuid = new SmartGuid(kp.Value.Guid, kp.Value.Id, GuidType.Item);

                    HandleUpdateObjectCreateItem(itemGuid, kp.Value, ref session, session.Character);
                }
            }

            // Create the equipment including our bags.
            foreach (var kp in session.Character.Equipment)
            {
                var itemGuid = new SmartGuid(kp.Value.Guid, kp.Value.Id, GuidType.Item);

                HandleUpdateObjectCreateItem(itemGuid, kp.Value, ref session, session.Character);
            }

            var updatefielddata = WriteUpdateFields(session.Character, comment);
            var ssize = 0 + updatefielddata.Length;

            var hdata = BitConverter.GetBytes((uint)ssize - 1).ToHexString();// "9b820000"; // updatefield size

            hdata += "FF"; // object type
            hdata += updatefielddata.ToHexString();

            updateObject.WriteInt32(1); // + session.Character.Equipment.Count + session.Character.Bag.Count);
            updateObject.WriteUInt16((ushort)session.Character.Map);
            BitPack.Write(0);
            BitPack.Write(0);
            BitPack.Flush();
            updateObject.WriteInt32(0);

            updateObject.WriteUInt8((byte)UpdateType.CreateObject2);
            updateObject.WriteSmartGuid(character.Guid);
            updateObject.WriteUInt8((byte)7);
            //updateObject.WriteUInt32(0xE1);
            var updateFlags = UpdateFlag.Alive | UpdateFlag.Rotation | UpdateFlag.Self;
            WorldMgr.WriteUpdateObjectMovement(ref updateObject, ref character, updateFlags);
            updateObject.Write(hdata.ToByteArray());

            var size = (uint)updateObject.BaseStream.Length - 29;
            updateObject.WriteUInt32Pos(size, 25);

            session.Send(ref updateObject);

            return;
        }

        public static byte[] WriteUpdateFields(Character character, bool comment = false, bool self = true, uint emoteState = 0)
        {
            var writer = new PacketWriter();
            var emptyGuid = new SmartGuid();

            // ObjectFields
            writer.WriteInt32(0);
            writer.WriteUInt32(0);
            writer.WriteFloat(1);

            // UnitFields
            var race = WorldMgr.ChrRaces.Single(r => r.Id == character.Race);
            var displayId = character.Gender == 0 ? race.MaleDisplayId : race.FemaleDisplayId;


            if (race.Id == 22 || race.Id == 23)
                displayId = character.Gender == 0 ? 37915u : 37914;

            // UnitFields
            writer.WriteInt32((int)displayId); // DisplayID
            writer.WriteUInt32(0); // NpcFlags1
            writer.WriteUInt32(0); // NpcFlags2

            writer.WriteUInt32(0);    // StateSpellVisualID
            writer.WriteUInt32(1716); // StateAnimID
            writer.WriteUInt32(0);    // StateAnimKitID
            writer.WriteUInt32(0);    // StateWorldEffectIDs
            writer.WriteUInt32(0);    // StateWorldEffectsQuestObjectiveID
            writer.WriteUInt32(0);    // SpellOverrideNameID

            writer.WriteSmartGuid(emptyGuid); // Charm
            writer.WriteSmartGuid(emptyGuid); // Summon
            writer.WriteSmartGuid(emptyGuid); // Critter
            writer.WriteSmartGuid(emptyGuid); // CharmedBy
            writer.WriteSmartGuid(emptyGuid); // SummonedBy
            writer.WriteSmartGuid(emptyGuid); // CreatedBy
            writer.WriteSmartGuid(emptyGuid); // DemonCreator
            writer.WriteSmartGuid(emptyGuid); // LookAtControllerTarget
            writer.WriteSmartGuid(emptyGuid); // Target
            writer.WriteSmartGuid(emptyGuid); // BattlePetCompanionGUID
            writer.WriteUInt64(0);       // BattlePetDBID

            // ChannelData
            writer.WriteInt32(0);
            writer.WriteInt32(0);
            writer.WriteInt32(0);

            character.Faction = WorldMgr.ChrRaces.Single(r => r.Id == character.Race).Faction;

            writer.WriteUInt32(0); //SummonedByHomeRealm

            writer.WriteUInt8(character.Race);   // Race
            writer.WriteUInt8(character.Class);  // Class
            writer.WriteUInt8(0);           // PlayerClass
            writer.WriteUInt8(character.Gender); // Sex

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

            foreach (var dp in WorldMgr.DefaultPowerTypes)
            {
                uint index = 0;
                for (var i = 0; i < 19; i++)
                {
                    if (_powersByClass[dp.Key][i] != 19)
                        ++index;

                    _powersByClass[dp.Key][dp.Value] = index;
                }
            }

            writer.WriteUInt8((byte)powerTypes[character.Class]); // DisplayPower
            writer.WriteUInt32(0); // OverrideDisplayPowerID
            writer.WriteInt64(42); // Health

            for (var i = 0; i < 7; i++)
            {
                if (_powersByClass[character.Class][i] != 19)
                {
                    writer.WriteInt32(1); // Power
                    writer.WriteInt32(1); // MaxPower
                }
                else
                {
                    writer.WriteInt32(0); // Power
                    writer.WriteInt32(0); // MaxPower
                }
            }

            for (var i = 0; i < 7; i++)
            {
                writer.WriteFloat(0); // PowerRegenFlatModifier
                writer.WriteFloat(0); // PowerRegenInterruptedFlatModifier
            }

            writer.WriteInt64(42); // MaxHealth
            writer.WriteInt32(character.Level); // Level
            writer.WriteInt32(0); // EffectiveLevel
            writer.WriteInt32(0); // ContentTuningID
            writer.WriteInt32(0); // ScalingLevelMin
            writer.WriteInt32(0); // ScalingLevelMax
            writer.WriteInt32(0); // ScalingLevelDelta
            writer.WriteInt32(0); // ScalingFactionGroup
            writer.WriteInt32(0); // ScalingHealthItemLevelCurveID
            writer.WriteInt32(0); // ScalingDamageItemLevelCurveID
            writer.WriteInt32((int)character.Faction); // FactionTemplate

            // VirtualItems
            for (var i = 0; i < 3; i++)
            {
                writer.WriteInt32(0); // id
                writer.WriteInt32(0);
                writer.WriteUInt16(0); // modid
                writer.WriteUInt16(0);
            }

            writer.WriteUInt32(8);    // Flags
            writer.WriteUInt32(2048); // Flags2
            writer.WriteUInt32(32);   // Flags3
            writer.WriteUInt32(0);    // AuraState

            for (var i = 0; i < 2; i++)
            {
                writer.WriteUInt32(2000); // AttackRoundBaseTime
            }

            writer.WriteUInt32(0);     // RangedAttackRoundBaseTime
            writer.WriteFloat(0.389F); // BoundingRadius
            writer.WriteFloat(1.5F);   // CombatReach
            writer.WriteFloat(1);      // DisplayScale
            writer.WriteUInt32(0);     // CreatureFamily
            writer.WriteInt32(0);      // CreatureType
            writer.WriteUInt32(displayId);  // NativeDisplayId
            writer.WriteFloat(1);      // NativeXDisplayScale
            writer.WriteUInt32(character.MountId); // MountDisplayID
            writer.WriteUInt32(0); // CosmeticMountDisplayID
            writer.WriteFloat(1337); // MinDamage
            writer.WriteFloat(1337); // MaxDamage
            writer.WriteFloat(42);   // MinOffHandDamage
            writer.WriteFloat(42);   // MaxOffHandDamage

            writer.WriteUInt8(0); // StandState
            writer.WriteUInt8(1); // PetTalentPoints
            writer.WriteUInt8(0); // VisFlags
            writer.WriteUInt8(0); // AnimTier

            writer.WriteUInt32(0); // PetNumber
            writer.WriteUInt32(0); // PetNameTimestamp
            writer.WriteUInt32(0); // PetExperience
            writer.WriteUInt32(0); // PetNextLevelExperience

            writer.WriteFloat(1); // ModCastingSpeed
            writer.WriteFloat(0); // ModCastingSpeedNeg
            writer.WriteFloat(0); // ModSpellHaste
            writer.WriteFloat(0); // ModHaste
            writer.WriteFloat(0); // ModRangedHaste
            writer.WriteFloat(0); // ModHasteRegen
            writer.WriteFloat(1); // ModTimeRate

            writer.WriteInt32(0); // CreatedBySpell
            writer.WriteInt32(0); // EmoteState

            for (var i = 0; i < 4; i++)
            {
                writer.WriteInt32(0); // Stat
                writer.WriteInt32(0); // StatPosBuff
                writer.WriteInt32(0); // StatNegBuff
            }

            for (var i = 0; i < 7; i++)
            {
                writer.WriteInt32(1); //Resistance
            }

            for (var i = 0; i < 7; i++)
            {
                writer.WriteInt32(0); // BonusResistanceMod
                writer.WriteInt32(0); // PowerCostModifier
            }

            writer.WriteInt32(0); // BaseMana
            writer.WriteInt32(0); // BaseHealth

            writer.WriteUInt8(0); // SheatheState
            writer.WriteUInt8(0); // PvpFlags
            writer.WriteUInt8(0); // PetFlags
            writer.WriteUInt8(0); // ShapeshiftForm

            writer.WriteInt32(1); // AttackPower
            writer.WriteInt32(0); // AttackPowerModPos
            writer.WriteInt32(0); // AttackPowerModNeg
            writer.WriteFloat(0); // AttackPowerModNeg

            writer.WriteInt32(0); // RangedAttackPower
            writer.WriteInt32(0); // RangedAttackPowerModPos
            writer.WriteInt32(0); // RangedAttackPowerModNeg
            writer.WriteFloat(0); // RangedAttackPowerMultiplier

            writer.WriteInt32(0); // MainHandWeaponAttackPower
            writer.WriteInt32(0); // OffHandWeaponAttackPower
            writer.WriteInt32(0); // RangedWeaponAttackPower
            writer.WriteInt32(0); // SetAttackSpeedAura

            writer.WriteFloat(0); // Lifesteal
            writer.WriteFloat(0); // MinRangedDamage
            writer.WriteFloat(0); // MaxRangedDamage
            writer.WriteFloat(0); // ManaCostMultiplier
            writer.WriteFloat(1); // MaxHealthModifier

            writer.WriteFloat(1); // HoverHeight
            writer.WriteInt32(0); // MinItemLevelCutoff
            writer.WriteInt32(0); // MinItemLevel
            writer.WriteInt32(0); // MaxItemLevel
            writer.WriteInt32(0); // AzeriteItemLevel
            writer.WriteInt32(0); // WildBattlePetLevel
            writer.WriteUInt32(0); // BattlePetCompanionNameTimestamp
            writer.WriteInt32(0); // InteractSpellID
            writer.WriteInt32(0); // ScaleDuration
            writer.WriteInt32(0); // LooksLikeMountID
            writer.WriteInt32(0); // LooksLikeCreatureID
            writer.WriteInt32(-1); // LookAtControllerID

            writer.WriteInt32(0); // TaxiNodesID
            writer.WriteSmartGuid(emptyGuid); // GuildGUID

            writer.WriteUInt32(0); // PassiveSpells
            writer.WriteUInt32(0); // WorldEffects
            writer.WriteUInt32(0); // ChannelObjects

            writer.WriteUInt32(0); // SilencedSchoolMask

            writer.WriteSmartGuid(emptyGuid); // NameplateAttachToGUID

            //! PlayerFields
            writer.WriteSmartGuid(emptyGuid); // DuelArbiter
            writer.WriteSmartGuid(emptyGuid); // WowAccount
            writer.WriteSmartGuid(emptyGuid); // LootTargetGUID

            writer.WriteInt32(0); // 9.1.5 counter
            writer.WriteUInt32(comment ? 0x80000 | 0x400000 | 0x8000000 | 0x80000000 : 0u); // PlayerFlags
            writer.WriteUInt32(512); // PlayerFlagsEx
            writer.WriteUInt32(0); // GuildRankID
            writer.WriteUInt32(0); // GuildDeleteDate
            writer.WriteUInt32(0); // GuildLevel

            writer.WriteInt32(character.CharacterCustomizationChoices?.Count ?? 0); // Customizations

            writer.WriteUInt8(0); // PartyType
            writer.WriteUInt8(character.Gender); // NativeSex
            writer.WriteUInt8(0); // Inebriation
            writer.WriteUInt8(0); // PvpTitle
            writer.WriteUInt8(0); // ArenaFaction

            writer.WriteUInt32(0); // DuelTeam
            writer.WriteUInt32(0); // GuildTimeStamp

            // QuestLog
            for (var i = 0; i < 125; i++)
            {
                writer.WriteInt32(0);
                writer.WriteUInt32(0);
                writer.WriteUInt32(0);
                writer.WriteUInt32(0);
                writer.WriteUInt32(0);

                for (var j = 0; j < 24; j++)
                {
                    writer.WriteUInt16(0);
                }
            }

            writer.WriteUInt32(0); // QuestSessionQuestLog

            var written = false;

            // VisibleItems
            for (var i = 0; i < 19; i++)
            {
                written = false;

                foreach (var kp in character.Equipment)
                {
                    if (kp.Key == i)
                    {
                        writer.WriteInt32(kp.Value.Id);
                        writer.WriteInt32(0);
                        writer.WriteUInt16((ushort)kp.Value.ModId);
                        writer.WriteUInt16(0);

                        written = true;
                        break;
                    }
                }

                if (!written)
                {
                    writer.WriteInt32(0);
                    writer.WriteInt32(0);
                    writer.WriteUInt16(0);
                    writer.WriteUInt16(0);
                }
            }

            if (character.PrimarySpec == 0)
                character.PrimarySpec = WorldMgr.DefaultChrSpec[character.Class];

            writer.WriteInt32(-1); // PlayerTitle
            writer.WriteInt32(0); // FakeInebriation
            writer.WriteUInt32(1); // VirtualPlayerRealm
            writer.WriteUInt32(character.PrimarySpec); // CurrentSpecID
            writer.WriteInt32(0); // TaxiMountAnimKitID

            for (var i = 0; i < 6; i++)
            {
                writer.WriteFloat(42); // AvgItemLevel
            }

            writer.WriteUInt8(0); // CurrentBattlePetBreedQuality
            writer.WriteInt32(0); // HonorLevel
            writer.WriteUInt32(0); // ArenaCooldowns
            writer.WriteInt32(0); // Unk
            writer.WriteInt32(0); // Unk

            // CtrOptions
            writer.WriteInt32(0);
            writer.WriteInt32(0);
            writer.WriteInt32(0);

            writer.WriteInt32(0); // CovenantID
            writer.WriteInt32(0); // SoulbindID

            // Customizations
            character.CharacterCustomizationChoices?.ForEach((choice) =>
            {
                writer.WriteUInt32(choice.OptionId);
                writer.WriteUInt32(choice.Value);
            });

            // These are 2 bits. Both written as false and flushed. 8.2.5 PTR
            writer.WriteUInt8(0);

            writer.WriteFloat(0);
            writer.WriteInt32(0); // DungeonScore
            writer.WriteInt32(0); // DungeonScore

            //! ActivePlayerFields
            if (self)
            {
                var invSlots = new SmartGuid[199];

                // Only the first default bag is set through InvSlots.
                foreach (var kp in character.Bags[255])
                {
                    PlayerCommands.bagSlotsList[kp.Key] = true;

                    var itemGuid = new SmartGuid(kp.Value.Guid, kp.Value.Id, GuidType.Item);

                    invSlots[(kp.Key)] = itemGuid;
                }

                foreach (var kp in character.Equipment)
                {
                    if (kp.Key > 199)
                    {
                        Log.Message(LogType.Debug, "Given slot is bigger than 199");
                        continue;
                    }

                    var itemGuid = new SmartGuid(kp.Value.Guid, kp.Value.Id, GuidType.Item);

                    invSlots[(kp.Key)] = itemGuid;
                }

                // ActivePlayerFields
                for (var i = 0; i < invSlots.Length; i++)
                {
                    writer.WriteSmartGuid(invSlots[i] ?? emptyGuid);
                }

                writer.WriteSmartGuid(emptyGuid); // FarsightObject
                writer.WriteSmartGuid(emptyGuid); // SummonedBattlePetGUID
                writer.WriteUInt32(0); // KnownTitles
                writer.WriteUInt64(1); // Coinage
                writer.WriteInt32(200); // XP
                writer.WriteInt32(400); // NextLevelXP
                writer.WriteInt32(0); // TrialXP

                // Skills
                for (int i = 0; i < 256; i++)
                {
                    if (i < character.Skills.Count)
                    {
                        writer.WriteUInt16((ushort)character.Skills[i].Id);
                        writer.WriteUInt16(4);
                        writer.WriteUInt16(300);
                        writer.WriteUInt16(300);
                        writer.WriteUInt16(0);
                        writer.WriteInt16(0);
                        writer.WriteUInt16(0);
                    }
                    else
                    {
                        writer.WriteUInt16(0);
                        writer.WriteUInt16(0);
                        writer.WriteUInt16(0);
                        writer.WriteUInt16(0);
                        writer.WriteUInt16(0);
                        writer.WriteInt16(0);
                        writer.WriteUInt16(0);
                    }
                }

                writer.WriteInt32(0); // CharacterPoints
                writer.WriteInt32(21); // MaxTalentTiers
                writer.WriteUInt32(0); // TrackCreatureMask

                writer.WriteFloat(7F); // Float MainhandExpertise
                writer.WriteFloat(7F); // Float OffhandExpertise
                writer.WriteFloat(0); // Float RangedExpertise
                writer.WriteFloat(0); // Float CombatRatingExpertise
                writer.WriteFloat(0); // Float BlockPercentage
                writer.WriteFloat(0); // Float DodgePercentage
                writer.WriteFloat(0); // Float DodgePercentageFromAttribute
                writer.WriteFloat(0); // Float ParryPercentage
                writer.WriteFloat(0); // Float ParryPercentageFromAttribute
                writer.WriteFloat(0); // Float CritPercentage
                writer.WriteFloat(0); // Float RangedCritPercentage
                writer.WriteFloat(0); // Float OffhandCritPercentage
                writer.WriteFloat(0); // Float SpellCritPercentage
                writer.WriteInt32(0); // Int32 ShieldBlock
                writer.WriteFloat(0); // Float ShieldBlockCritPercentage
                writer.WriteFloat(42F); // Float Mastery
                writer.WriteFloat(7F); // Float Speed
                writer.WriteFloat(0); // Float Avoidance
                writer.WriteFloat(0); // Float Sturdiness
                writer.WriteInt32(0); // Int32 Versatility
                writer.WriteFloat(0); // Float VersatilityBonus
                writer.WriteFloat(0); // Float PvpPowerDamage
                writer.WriteFloat(0); // Float PvpPowerHealing

                // ExploredZones
                for (var i = 0; i < 240; i++)
                {
                    writer.WriteUInt64(ulong.MaxValue);
                }

                // RestInfo
                for (var i = 0; i < 2; i++)
                {
                    writer.WriteUInt32(0);
                    writer.WriteUInt8(2);
                }

                // ModDamageDone
                for (var i = 0; i < 7; i++)
                {
                    writer.WriteInt32(1);
                    writer.WriteInt32(0);
                    writer.WriteFloat(1);
                    writer.WriteFloat(0);
                }

                writer.WriteInt32(0); // ModHealingDonePos
                writer.WriteFloat(1); // ModHealingPercent
                writer.WriteFloat(1); // ModPeriodicHealingDonePercent

                for (var i = 0; i < 3; i++)
                {
                    writer.WriteFloat(1);
                    writer.WriteFloat(1);
                }

                writer.WriteFloat(1); // ModSpellPowerPercent
                writer.WriteFloat(1); // ModResiliencePercent
                writer.WriteFloat(1); // OverrideSpellPowerByAPPercent
                writer.WriteFloat(1); // OverrideAPBySpellPowerPercent

                writer.WriteInt32(0); // ModTargetResistance
                writer.WriteInt32(0); // ModTargetPhysicalResistance
                writer.WriteInt32(4104); // LocalFlags

                writer.WriteUInt8(0); // GrantableLevels
                writer.WriteUInt8(0); // MultiActionBars
                writer.WriteUInt8(0); // LifetimeMaxRank
                writer.WriteUInt8(0); // NumRespecs
                writer.WriteUInt32(0); // PvpMedals

                // Buyback
                for (var i = 0; i < 12; i++)
                {
                    writer.WriteUInt32(0);
                    writer.WriteUInt64(0);
                }

                writer.WriteUInt16(0); // TodayHonorableKills
                writer.WriteUInt16(0); // YesterdayHonorableKills
                writer.WriteInt32(0); // LifetimeHonorableKills
                writer.WriteInt32(-1); // WatchedFactionIndex

                // CombatRatings
                for (var i = 0; i < 32; i++)
                {
                    writer.WriteInt32(0);
                }

                writer.WriteInt32(character.Level); // MaxLevel
                writer.WriteInt32(0); // ScalingPlayerLevelDelta
                writer.WriteInt32(0); // MaxCreatureScalingLevel

                // NoReagentCostMask
                for (var i = 0; i < 4; i++)
                {
                    writer.WriteUInt32(0);
                }

                writer.WriteInt32(0); // PetSpellPower

                // ProfessionSkillLine
                for (var i = 0; i < 2; i++)
                {
                    writer.WriteInt32(0);
                }

                writer.WriteFloat(0); // UiHitModifier
                writer.WriteFloat(0); // UiSpellHitModifier
                writer.WriteInt32(0); // HomeRealmTimeOffset

                writer.WriteFloat(1); // ModPetHaste

                writer.WriteUInt8(0); // JailersTowerLevelMax
                writer.WriteUInt8(0); // JailersTowerLevel
                writer.WriteUInt8(0); // LocalRegenFlags
                writer.WriteUInt8(0); // AuraVision
                writer.WriteUInt8(28); // NumBackpackSlots

                writer.WriteInt32(0); // OverrideSpellsID
                writer.WriteUInt16(0); // LootSpecID
                writer.WriteUInt32(0); // OverrideZonePVPType

                writer.WriteSmartGuid(emptyGuid); // BnetAccount
                writer.WriteUInt64(0); // GuildClubMemberID

                // BagSlotFlags
                for (var i = 0; i < 4; i++)
                {
                    writer.WriteUInt32(0);
                }

                // BankBagSlotFlags
                for (var i = 0; i < 7; i++)
                {
                    writer.WriteUInt32(0);
                }

                // QuestCompleted
                for (var i = 0; i < 875; i++)
                {
                    writer.WriteUInt64(0);
                }

                writer.WriteInt32(0); // Honor
                writer.WriteInt32(0); // HonorNextLevel
                writer.WriteUInt8(0); // NumBankSlots

                writer.WriteInt32(0); // ResearchSites
                writer.WriteInt32(0); // ResearchSiteProgress
                writer.WriteInt32(0); // DailyQuestsCompleted
                writer.WriteInt32(0); // AvailableQuestLineXQuestIDs
                writer.WriteInt32(0); // Heirlooms
                writer.WriteUInt32(0); // HeirloomFlags
                writer.WriteUInt32(0); // Toys
                writer.WriteUInt32(0); // ToyFlags
                writer.WriteUInt32(0); // Transmog
                writer.WriteUInt32(0); // ConditionalTransmog
                writer.WriteUInt32(0); // SelfResSpells
                writer.WriteUInt32(0); // RuneforgePowers
                writer.WriteUInt32(0); // TransmogIllusions
                writer.WriteUInt32(0); // CharacterRestrictions
                writer.WriteUInt32(0); // SpellPctModByLabel
                writer.WriteUInt32(0); // SpellFlatModByLabel
                writer.WriteUInt32(0); // MawPowers
                writer.WriteUInt32(0); // MultiFloorExploration
                writer.WriteUInt32(0); // RecipeProgression
                writer.WriteUInt32(0); // ReplayedQuests
                writer.WriteUInt32(0); // DisabledSpells
                writer.WriteUInt32(0); // UiChromieTimeExpansionID
                writer.WriteUInt32(0); // TransportServerTime
                writer.WriteUInt32(0); // WeeklyRewardsPeriodSinceOrigin
                writer.WriteUInt32(0); // ?
                //writer.WriteUInt32(0); //? 
                writer.WriteInt16(0); // DEBUGSoulbindConduitRank

                // PvpInfo
                for (var i = 0; i < 6; i++)
                {
                    writer.WriteUInt32(0);
                    writer.WriteUInt32(0);
                    writer.WriteUInt32(0);
                    writer.WriteUInt32(0);
                    writer.WriteUInt32(0);
                    writer.WriteUInt32(0);
                    writer.WriteUInt32(0);
                    writer.WriteUInt32(0);
                    writer.WriteUInt32(0);
                    writer.WriteUInt32(0);
                    writer.WriteUInt32(0);

                    // Bit
                    writer.WriteUInt8(0);
                }

                // Bits
                writer.WriteUInt8(0);

                writer.WriteSmartGuid(emptyGuid);
                writer.WriteUInt32(0);

                writer.WriteUInt32(0);
                writer.WriteInt32(0);
            }

            return (writer.BaseStream as MemoryStream).ToArray();
        }

        public static byte[] WriteUpdateFields(CreatureSpawn spawn)
        {
            var writer = new PacketWriter();
            var emptyGuid = new SmartGuid();

            // ObjectFields
            writer.WriteInt32(spawn.Id);
            writer.WriteUInt32(0);

            writer.WriteFloat(spawn.Scale);

            writer.WriteUInt32(spawn.ModelId); // DisplayID

            writer.WriteUInt32(0);
            writer.WriteUInt32(0);

            writer.WriteUInt32(0);
            writer.WriteUInt32((spawn.AnimState is 1716) ? 1716 : spawn.AnimState);
            writer.WriteUInt32(0);
            writer.WriteUInt32(0); // Counter16
            writer.WriteUInt32(0);

            writer.WriteUInt32(0);// 901

            writer.WriteSmartGuid(emptyGuid); // Charm
            writer.WriteSmartGuid(emptyGuid); // Summon
            writer.WriteSmartGuid(emptyGuid); // Critter
            writer.WriteSmartGuid(emptyGuid); // CharmedBy
            writer.WriteSmartGuid(emptyGuid); // SummonedBy
            writer.WriteSmartGuid(emptyGuid); // CreatedBy
            writer.WriteSmartGuid(emptyGuid); // DemonCreator
            writer.WriteSmartGuid(emptyGuid); // LookAtControllerTarget
            writer.WriteSmartGuid(emptyGuid); // Target
            writer.WriteSmartGuid(emptyGuid); // BattlePetCompanionGUID
            writer.WriteUInt64(0); // BattlePetDBID

            if (spawn.Guid == 452549342)
            {

                writer.WriteInt32(314020);
                writer.WriteInt32(281096);
                writer.WriteInt32(0);
            }
            else
            {
                writer.WriteInt32(0);
                writer.WriteInt32(0);
                writer.WriteInt32(0);
            }

            writer.WriteUInt32(0); // SummonedByHomeRealm

            writer.WriteUInt8(0); // Race
            writer.WriteUInt8(0); // Class
            writer.WriteUInt8(0); // PlayerClass
            writer.WriteUInt8(0); // Sex

            writer.WriteUInt8(0); // DisplayPower
            writer.WriteUInt32(0);
            writer.WriteInt64(spawn.Health); // Health

            for (var i = 0; i < 7; i++)
            {
                writer.WriteInt32(0); // Power
                writer.WriteInt32(0); // MaxPower
            }

            for (var i = 0; i < 7; i++)
            {
                writer.WriteFloat(0); // PowerRegenFlatModifier
                writer.WriteFloat(0); // PowerRegenInterruptedFlatModifier
            }

            writer.WriteInt64(spawn.MaxHealth); // MaxHealth
            writer.WriteUInt32(spawn.Level); // Level
            writer.WriteInt32(0); // EffectiveLevel
            writer.WriteInt32(0); // ContentTuningID
            writer.WriteInt32(0); // ScalingLevelMin
            writer.WriteInt32(0); // ScalingLevelMax
            writer.WriteInt32(0); // ScalingLevelDelta
            writer.WriteInt32(0); // ScalingFactionGroup
            writer.WriteInt32(0); // ScalingHealthItemLevelCurveID
            writer.WriteInt32(0); // ScalingDamageItemLevelCurveID
            writer.WriteUInt32(spawn.FactionTemplate); // FactionTemplate

            SpawnMgr.CreatureItems.TryGetValue(spawn.Guid, out var equip);
            var ct = 3;

            if (equip != null)
            {
                ct -= equip.Items.Count;

                foreach (var item in equip.Items)
                {

                    writer.WriteInt32(item.Id);
                    writer.WriteInt32(0);
                    writer.WriteUInt16((ushort)item.ModId);
                    writer.WriteUInt16(0);
                }
            }

            for (var i = 0; i < ct; i++)
            {
                writer.WriteInt32(0);
                writer.WriteInt32(0);
                writer.WriteUInt16(0);
                writer.WriteUInt16(0);
            }

            writer.WriteUInt32(spawn.UnitFlags); // Flags
            writer.WriteUInt32(2048); // Flags2
            writer.WriteUInt32(0); // Flags3
            if (spawn.Guid == 452549342)
                writer.WriteUInt32(13631488); // AuraState
            else
                writer.WriteUInt32(0); // AuraState

            for (var i = 0; i < 2; i++)
            {
                writer.WriteUInt32(2000); // AttackRoundBaseTime
            }

            writer.WriteUInt32(0); // RangedAttackRoundBaseTime
            writer.WriteFloat(0.389F); // BoundingRadius
            writer.WriteFloat(1.5F); // CombatReach
            writer.WriteFloat(1); // DisplayScale

            writer.WriteInt32(0); // 901
            writer.WriteInt32(0); // 901
            writer.WriteUInt32(spawn.ModelId); // NativeDisplayId

            writer.WriteFloat(1); // NativeXDisplayScale

            writer.WriteUInt32(spawn.MountDisplayId);
            writer.WriteInt32(0);

            writer.WriteFloat(1337); // MinDamage
            writer.WriteFloat(1337); // MaxDamage
            writer.WriteFloat(42); // MinOffHandDamage
            writer.WriteFloat(42); // MaxOffHandDamage

            /*writer.WriteUInt8(0); // StandState
            writer.WriteUInt8(1); // PetTalentPoints
            writer.WriteUInt8(0); // VisFlags
            writer.WriteUInt8(0); // AnimTier*/
            writer.WriteUInt32(0); // AnimTier

            writer.WriteUInt32(0); // PetNumber
            writer.WriteUInt32(0); // PetNameTimestamp
            writer.WriteUInt32(0); // PetExperience
            writer.WriteUInt32(0); // PetNextLevelExperience

            writer.WriteFloat(0); // ModCastingSpeed
            writer.WriteFloat(0); // ModSpellHaste
            writer.WriteFloat(0); // ModHaste
            writer.WriteFloat(0); // ModRangedHaste
            writer.WriteFloat(0); // ModHasteRegen
            writer.WriteFloat(0); // ModTimeRate
            writer.WriteFloat(0);

            writer.WriteInt32(0); // CreatedBySpell
            writer.WriteInt32(spawn.Emote); // EmoteState

            for (var i = 0; i < 4; i++)
            {
                writer.WriteInt32(0); // Stat
                writer.WriteInt32(0); // StatPosBuff
                writer.WriteInt32(0); // StatNegBuff
            }

            for (var i = 0; i < 7; i++)
            {
                writer.WriteInt32(1); //Resistance
            }

            for (var i = 0; i < 7; i++)
            {
                writer.WriteInt32(0); // BonusResistanceMod
                writer.WriteInt32(0); // PowerCostModifier
            }

            writer.WriteInt32(0); // BaseMana
            writer.WriteInt32(0); // BaseHealth

            writer.WriteUInt8(1); // SheatheState
            writer.WriteUInt8(1); // PvpFlags
            writer.WriteUInt8(1); // PetFlags
            writer.WriteUInt8(0); // ShapeshiftForm

            writer.WriteInt32(1); // AttackPower
            writer.WriteInt32(0); // AttackPowerModPos
            writer.WriteInt32(0); // AttackPowerModNeg
            writer.WriteFloat(0); // AttackPowerModNeg

            writer.WriteInt32(0); // RangedAttackPower
            writer.WriteInt32(0); // RangedAttackPowerModPos
            writer.WriteInt32(0); // RangedAttackPowerModNeg
            writer.WriteFloat(0); // RangedAttackPowerMultiplier

            writer.WriteInt32(0); // MainHandWeaponAttackPower
            writer.WriteInt32(0); // OffHandWeaponAttackPower
            writer.WriteInt32(0); // RangedWeaponAttackPower
            writer.WriteInt32(0); // SetAttackSpeedAura

            writer.WriteFloat(0); // Lifesteal
            writer.WriteFloat(0); // MinRangedDamage
            writer.WriteFloat(0); // MaxRangedDamage
            writer.WriteFloat(0); //
            writer.WriteFloat(1); // MaxHealthModifier
            writer.WriteFloat(spawn.HoverHeight); // HoverHeight

            writer.WriteInt32(0); //
            writer.WriteInt32(0); //
            writer.WriteInt32(0); //
            writer.WriteInt32(0);
            writer.WriteInt32(0);

            writer.WriteUInt32(0);

            writer.WriteInt32(0);
            writer.WriteInt32(0);
            writer.WriteInt32(0);
            writer.WriteInt32(0);
            writer.WriteInt32(0);
            //writer.WriteInt32(0);

            writer.WriteInt32(0); // LookAtControllerID
                                  // writer.WriteInt32(0);
            writer.WriteSmartGuid(emptyGuid); // GuildGUID

            writer.WriteUInt32(0);
            writer.WriteUInt32(0);
            writer.WriteUInt32(0);
            writer.WriteUInt32(0);
            writer.WriteSmartGuid(emptyGuid);

            return (writer.BaseStream as MemoryStream).ToArray();
        }

        public static byte[] WriteUpdateFields(GameObjectSpawn spawn)
        {
            var writer = new PacketWriter();
            var emptyGuid = new SmartGuid();

            // ObjectFields
            writer.WriteInt32(spawn.Id);
            writer.WriteUInt32(0);
            writer.WriteFloat(spawn.GameObject.Size);

            writer.WriteInt32(spawn.GameObject.DisplayInfoId);
            writer.WriteUInt32(spawn.SpellVisualId);
            writer.WriteUInt32(0);
            writer.WriteUInt32(0);
            writer.WriteUInt32(0);
            writer.WriteUInt32(0);
            writer.WriteUInt32(0);

            writer.WriteSmartGuid(emptyGuid);
            writer.WriteSmartGuid(emptyGuid);
            writer.WriteInt32(spawn.GameObject.Flags);

            writer.WriteFloat(spawn.GameObject.Rot.X);
            writer.WriteFloat(spawn.GameObject.Rot.Y);
            writer.WriteFloat(spawn.GameObject.Rot.Z);
            writer.WriteFloat(spawn.GameObject.Rot.O);

            writer.WriteUInt32(spawn.FactionTemplate);
            writer.WriteUInt8(spawn.State);
            writer.WriteUInt8((byte)spawn.GameObject.Type);
            writer.WriteUInt8(0xFF);

            writer.WriteUInt32(0);
            writer.WriteUInt32(0);
            writer.WriteUInt32(0);
            writer.WriteUInt32(0);
            writer.WriteUInt32(0);
            writer.WriteUInt32(0);
            writer.WriteUInt32(0);
            writer.WriteUInt32(0);
            writer.WriteUInt32(0);

            return (writer.BaseStream as MemoryStream).ToArray();
        }

        public static void HandleUpdateObjectCreateItem(SmartGuid guid, Item item2, ref WorldClass session, WorldObject character)
        {
            PacketWriter updateObject = new PacketWriter(ServerMessage.ObjectUpdate);
            BitPack BitPack = new BitPack(updateObject);
            var updatefielddata = WriteUpdateFields(guid, item2, character.Guid, character as Character);
            var ssize = 1 + updatefielddata.Length;

            var hdata = BitConverter.GetBytes((uint)ssize).ToHexString();

            hdata += "FF"; // object type
            hdata += updatefielddata.ToHexString();

            updateObject.WriteInt32(1); // + session.Character.Equipment.Count + session.Character.Bag.Count);
            updateObject.WriteUInt16((ushort)session.Character.Map);
            BitPack.Write(0);
            BitPack.Write(0);
            BitPack.Flush();
            updateObject.WriteInt32(0);

            updateObject.WriteUInt8((byte)UpdateType.CreateObject);
            updateObject.WriteSmartGuid(guid);

            var isContainer = item2.NumSlots != 0;

            updateObject.WriteUInt8(!isContainer ? (byte)ObjectType.Item : (byte)ObjectType.Container);

            BitPack.Write(0); // noBirthAnim
            BitPack.Write(0); // enablePortals
            BitPack.Write(0); // playHoverAnim
            BitPack.Write(0); // isSuppressingGreetings
            BitPack.Write(0); // move
            BitPack.Write(0); // passenger
            BitPack.Write(0); // stationary
            BitPack.Write(0); // combatVictim
            BitPack.Write(0); // serverTime
            BitPack.Write(0); // vehicle
            BitPack.Write(0); // animKit
            BitPack.Write(0); // rotation, wObject is GameObjectSpawn//512
            BitPack.Write(0); // areaTrigger
            BitPack.Write(0); // gameObject
            BitPack.Write(0); // thisIsYou
            BitPack.Write(0); // replaceActive
            BitPack.Write(0); // sceneObjCreate
            BitPack.Write(0); // scenePendingInstances

            BitPack.Flush();
            updateObject.WriteUInt32(0);
            updateObject.Write(hdata.ToByteArray());

            var size = (uint)updateObject.BaseStream.Length - 29;
            updateObject.WriteUInt32Pos(size, 25);

            session.Send(ref updateObject);
        }

        static byte[] WriteUpdateFields(SmartGuid guid, Item item2, ulong pGuid, Character pChar)
        {
            var writer = new PacketWriter();
            var emptyGuid = new SmartGuid();
            var guid2 = new SmartGuid { Type = GuidType.Player, CreationBits = pGuid, RealmId = 1 };

            // ObjectFields
            writer.WriteInt32(item2.Id);
            writer.WriteUInt32(0);
            writer.WriteFloat(1);

            // ItmeFields
            writer.WriteUInt32(0); // BonusListIDs

            writer.WriteSmartGuid(guid2); // Owner
            writer.WriteSmartGuid(guid2); // ContainedIn
            writer.WriteSmartGuid(emptyGuid); // Creator
            writer.WriteSmartGuid(emptyGuid); // GiftCreator
            writer.WriteUInt32(1); // StackCount
            writer.WriteUInt32(0); // Expiration

            // SpellCharges
            for (var i = 0; i < 5; i++)
            {
                writer.WriteInt32(0);
            }

            writer.WriteUInt32(196609); // DynamicFlags

            // Enchantments
            for (var i = 0; i < 13; i++)
            {
                writer.WriteInt32(0);
                writer.WriteUInt32(0);
                writer.WriteInt16(0);
                writer.WriteUInt16(0);
            }

            //writer.WriteInt32(0); // PropertySeed
            //writer.WriteInt32(0); // RandomPropertiesID


            writer.WriteUInt32(0); // Durability
            writer.WriteUInt32(0); // MaxDurability

            writer.WriteUInt32(0); // CreatePlayedTime
            writer.WriteUInt32(0); // ModifiersMask

            writer.WriteUInt64(0);
            writer.WriteUInt64(0);
            writer.WriteUInt8((byte)item2.ModId); // ItemAppearanceModID

            writer.WriteInt32(0);
            writer.WriteUInt32(0);
            writer.WriteUInt32(0);

            writer.WriteUInt8(0);

            // Container
            if (item2.NumSlots != 0)
            {
                var bag = pChar.Bags[item2.BagSlot];

                // Empty guids.
                for (byte i = 0; i < 36; i++)
                {
                    if (bag.TryGetValue(i, out var bagItem))
                        writer.WriteSmartGuid(new SmartGuid(bagItem.Guid, bagItem.Id, GuidType.Item));
                    else
                        writer.WriteSmartGuid(new SmartGuid());
                }

                // Back slot count.
                writer.WriteInt32(item2.NumSlots);
            }

            return (writer.BaseStream as MemoryStream).ToArray();
        }

        public static void Update<T>(ObjectFields objectField, T newValue, WorldObject worldObject)
        {
            Update((int)objectField, newValue, worldObject, 0x1, 4, 1);
        }

        public static void Update<T>(UnitFields unitField, T newValue, WorldObject worldObject)
        {
            Update((int)unitField, newValue, worldObject, 0x20, 7, 7);
        }

        public static void Update<T>(PlayerFields playerField, T newValue, WorldObject worldObject)
        {
            Update((int)playerField, newValue, worldObject, 0x40, 6, 6);
        }

        public static void Update<T>(ActivePlayerFields activePlayerfield, T newValue, WorldObject worldObject)
        {
            Update((int)activePlayerfield, newValue, worldObject, 0x80, 17, 49);
        }

        public static void Update<T>(GameObjectFields gameobjectField, T newValue, WorldObject worldObject)
        {
            Update((int)gameobjectField, newValue, worldObject, 0x100, 21, 1);
        }

        static void Update<T>(int updateField, T newValue, WorldObject worldObject,
                              int typeMask, int bitCount, uint maskCount)
        {
            var updateObject = new PacketWriter(ServerMessage.ObjectUpdate);

            updateObject.WriteUInt32(1);
            updateObject.WriteUInt16((ushort)worldObject.Map);
            updateObject.WriteUInt8(0);
            updateObject.WriteInt32(0);

            updateObject.WriteUInt8((byte)UpdateType.Values);

            if (worldObject is not Character)
                updateObject.WriteSmartGuid(worldObject.SGuid);
            else
                updateObject.WriteSmartGuid(worldObject.Guid);

            var writer = new PacketWriter();
            var bp = new BitPack(writer);
            var maskSize = updateField - (updateField % 32);
            var bits = new BitArray(maskSize + 32, false);

            // Set the uint32 enable bit based on the update bit position.
            bits.Set(maskSize, true);
            bits.Set(updateField, true);

            // Copy the bit mask value into uint32 array.
            var ret = new byte[(bits.Length) / 8 + 1];

            bits.CopyTo(ret, 0);

            var masks = new uint[ret.Length / 4];

            for (var i = 0; i < masks.Length; i++)
                masks[i] = BitConverter.ToUInt32(ret, i * 4);

            // hax for now. Fix for ActivePlayerFields
            if (maskCount > 32)
                updateObject.WriteUInt32(0);// masks[maskIndex++]);

            if (maskCount > 1)
            {
                bp.Write(1 << (masks.Length - 1), bitCount);

                for (var i = 0; i < masks.Length; i++)
                {
                    if (masks[i] != 0)
                        bp.Write(masks[i], 32);
                }
            }
            else
                bp.Write(masks[0], bitCount);

            bp.Flush();

            // Append the updatefield data to the update object packet.
            updateObject.Write((writer.BaseStream as MemoryStream).ToArray().Length + 4 + 4);
            updateObject.WriteInt32(typeMask);

            updateObject.Write((writer.BaseStream as MemoryStream).ToArray());

            var fieldSize = Unsafe.SizeOf<T>();
            var fieldBuffer = new byte[fieldSize];

            Unsafe.WriteUnaligned(ref fieldBuffer[0], newValue);

            updateObject.Write(fieldBuffer);

            var size = (uint)updateObject.BaseStream.Length - 29;
            updateObject.WriteUInt32Pos(size, 25);

            WorldMgr.Sessions.First().Value.Send(ref updateObject);
        }
    }
}
