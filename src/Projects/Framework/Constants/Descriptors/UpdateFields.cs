public enum ObjectFields
{
    EntryID = 1, // size 1, flags MIRROR_VIEWER_DEPENDENT
    DynamicFlags = 2, // size 1, flags MIRROR_VIEWER_DEPENDENT, MIRROR_URGENT
    Scale = 3, // size 1, flags MIRROR_ALL
    DisplayId = 0xFF,
    Guid = 0xFF, // size 4, flags MIRROR_ALL

    End = 4
}

public enum ItemFields
{
    Owner = ObjectFields.End + 0, // size 4, flags MIRROR_ALL
    ContainedIn = ObjectFields.End + 4, // size 4, flags MIRROR_ALL
    Creator = ObjectFields.End + 8, // size 4, flags MIRROR_ALL
    GiftCreator = ObjectFields.End + 12, // size 4, flags MIRROR_ALL
    StackCount = ObjectFields.End + 16, // size 1, flags MIRROR_OWNER
    Expiration = ObjectFields.End + 17, // size 1, flags MIRROR_OWNER
    SpellCharges = ObjectFields.End + 18, // size 5, flags MIRROR_OWNER
    DynamicFlags = ObjectFields.End + 23, // size 1, flags MIRROR_ALL
    Enchantment = ObjectFields.End + 24, // size 39, flags MIRROR_ALL
    PropertySeed = ObjectFields.End + 63, // size 1, flags MIRROR_ALL
    RandomPropertiesID = ObjectFields.End + 64, // size 1, flags MIRROR_ALL
    Durability = ObjectFields.End + 65, // size 1, flags MIRROR_OWNER
    MaxDurability = ObjectFields.End + 66, // size 1, flags MIRROR_OWNER
    CreatePlayedTime = ObjectFields.End + 67, // size 1, flags MIRROR_ALL
    ModifiersMask = ObjectFields.End + 68, // size 1, flags MIRROR_OWNER
    Context = ObjectFields.End + 69, // size 1, flags MIRROR_ALL
    ArtifactXP = ObjectFields.End + 70, // size 2, flags MIRROR_OWNER
    ItemAppearanceModID = ObjectFields.End + 72, // size 1, flags MIRROR_OWNER
    End = ObjectFields.End + 73
}

public enum ContainerFields
{
    Slots = ItemFields.End + 0, // size 144, flags MIRROR_ALL
    NumSlots = ItemFields.End + 144, // size 1, flags MIRROR_ALL
    End = ItemFields.End + 145
}

public enum CGAzeriteEmpoweredItemData
{
    Selections = ItemFields.End + 0, // size 4, flags MIRROR_ALL
    End = ItemFields.End + 4
}

public enum CGAzeriteItemData
{
    Xp = ItemFields.End + 0, // size 2, flags MIRROR_ALL
    Level = ItemFields.End + 2, // size 1, flags MIRROR_ALL
    AuraLevel = ItemFields.End + 3, // size 1, flags MIRROR_ALL
    KnowledgeLevel = ItemFields.End + 4, // size 1, flags MIRROR_OWNER
    DEBUGknowledgeWeek = ItemFields.End + 5, // size 1, flags MIRROR_OWNER
    End = ItemFields.End + 6
}

public enum UnitFields
{
    DynamicField0 = 1,
    DynamicField1 = 2,
    DynamicField2 = 3,
    DynamicField3 = 4,

    DisplayID = 5,
    StateSpellVisualID = 6,
    StateAnimID = 7,
    // 8
    // 9
    Charm = 10,
    Summon = 11,
    Critter = 12,
    CharmedBy = 13,
    SummonedBy = 14,
    CreatedBy = 15,
    DemonCreator = 16,
    LookAtControllerTarget = 17,
    Target = 18,
    BattlePetCompanionGUID = 19,
    BattlePetDBID = 20,
    // 21
    // 22
    // 23
    Race = 24,
    Class = 25,
    PlayerClass = 26,
    Sex = 27,
    DisplayPower = 28,
    // 29
    // 30
    // 31
    // 33 next 
    Health = 34,
    MaxHealth = 35,
    Level = 36,

    // Second 32 Bits
    EffectiveLevel = 37,
    ContentTuningID = 38,
    ScalingLevelMin = 39,
    ScalingLevelMax = 40,
    ScalingLevelDelta = 41,
    ScalingFactionGroup = 42,
    ScalingHealthItemLevelCurveID = 43,
    ScalingDamageItemLevelCurveID = 44,
    FactionTemplate = 45,
    Flags = 46,
    Flags2 = 47,
    Flags3 = 48,
    AuraState = 49,
    RangedAttackRoundBaseTime = 50,
    BoundingRadius = 51,
    CombatReach = 52,
    DisplayScale = 53,
    //
    // 
    NativeDisplayID = 53,
    NativeXDisplayScale = 54,
    MountDisplayID = 55,
    // 56
    MinDamage = 58,
    MaxDamage = 59,
    MinOffHandDamage = 60,
    MaxOffHandDamage = 61,
    StandState = 62,
    PetTalentPoints = 63,
    VisFlags = 64,
    // 
    AnimTier = 66,
    PetNumber = 67,
    PetNameTimestamp = 68,

    // Third 32 Bits
    PetExperience = 69,
    PetNextLevelExperience = 70,
    ModCastingSpeed = 71,
    ModSpellHaste = 72,
    ModHaste = 73,
    ModRangedHaste = 74,
    ModHasteRegen = 75,
    ModTimeRate = 76,
    CreatedBySpell = 77,
    EmoteState = 78,
    BaseMana = 75,
    BaseHealth = 76,
    SheatheState = 77,
    PvpFlags = 78,
    PetFlags = 79,
    ShapeshiftForm = 80,
    AttackPower = 81,
    AttackPowerModPos = 82,
    AttackPowerModNeg = 83,
    AttackPowerMultiplier = 84,
    RangedAttackPower = 85,
    RangedAttackPowerModPos = 86,
    RangedAttackPowerModNeg = 87,
    RangedAttackPowerMultiplier = 88,
    MainHandWeaponAttackPower = 89,
    OffHandWeaponAttackPower = 90,
    RangedWeaponAttackPower = 91,
    SetAttackSpeedAura = 92,
    Lifesteal = 93,
    MinRangedDamage = 94,
    MaxRangedDamage = 95,

    // Fourth 32 Bits
    // 97
    MaxHealthModifier = 98,
    HoverHeight = 99,
    // 100
    // 101
    // 102
    // 103
    // 104
    // 105
    // 106
    // 107
    // 108
    // 109
    // 110
    LookAtControllerID = 114,
    // 112
    GuildGUID = 113,
    New830 = 114,

    // Fourth 32 Bits, Arrays
    EnableChannelData = 115,
    ChannelData = 116, // ChannelData[2] (UInt32)

    // [6]
    EnablePower = 117,
    Power = 118,
    MaxPower = 125,
    PowerRegenFlatModifier = 131,
    PowerRegenInterruptedFlatModifier = 137,

    // [3]
    EnableVirtualItems = 152,
    VirtualItems = 153, // SubStructure (4 bit mask, 0x1 = enable, Int32 (0x2), UInt16 (0x4), UInt16 (0x8))

    // [2]
    EnableAttackRoundBaseTime = 147,
    AttackRoundBaseTime = 148,

    // [4]
    EnableStats = 150,
    Stats = 151,
    StatPosBuff = 155,
    StatNegBuff = 159,

    // [7]
    EnableResistances = 163,
    Resistances = 164,
    BonusResistanceMods = 171,
    PowerCostModifier = 178,
    End = 192,


    //BattlePetDBID = ObjectFields.End + 40, // size 2, flags MIRROR_ALL
    SummonedByHomeRealm = ObjectFields.End + 44, // size 1, flags MIRROR_ALL
    OverrideDisplayPowerID = ObjectFields.End + 47, // size 1, flags MIRROR_ALL
    CastingSpeed = ObjectFields.End + 115, // size 1, flags MIRROR_ALL
    MinItemLevelCutoff = ObjectFields.End + 185, // size 1, flags MIRROR_ALL
    MinItemLevel = ObjectFields.End + 186, // size 1, flags MIRROR_ALL
    MaxItemLevel = ObjectFields.End + 187, // size 1, flags MIRROR_ALL
    WildBattlePetLevel = ObjectFields.End + 188, // size 1, flags MIRROR_ALL
    BattlePetCompanionNameTimestamp = ObjectFields.End + 189, // size 1, flags MIRROR_ALL
    InteractSpellID = ObjectFields.End + 190, // size 1, flags MIRROR_ALL
    //StateAnimID = ObjectFields.End + 192, // size 1, flags MIRROR_VIEWER_DEPENDENT, MIRROR_URGENT
    StateAnimKitID = ObjectFields.End + 193, // size 1, flags MIRROR_VIEWER_DEPENDENT, MIRROR_URGENT
    StateWorldEffectID = ObjectFields.End + 194, // size 4, flags MIRROR_VIEWER_DEPENDENT, MIRROR_URGENT
    ScaleDuration = ObjectFields.End + 198, // size 1, flags MIRROR_ALL
    LooksLikeMountID = ObjectFields.End + 199, // size 1, flags MIRROR_ALL
    LooksLikeCreatureID = ObjectFields.End + 200, // size 1, flags MIRROR_ALL
    //End = ObjectFields.End + 206
}

public enum PlayerFields
{
    PlayerFlags = 9,
    EnableVisibleItems = 161,
    VisibleItems = 162,

    DuelArbiter = UnitFields.End + 0, // size 4, flags MIRROR_ALL
    WowAccount = UnitFields.End + 4, // size 4, flags MIRROR_ALL
    LootTargetGUID = UnitFields.End + 8, // size 4, flags MIRROR_ALL
    PlayerFlagsEx = UnitFields.End + 13, // size 1, flags MIRROR_ALL
    GuildRankID = UnitFields.End + 14, // size 1, flags MIRROR_ALL
    GuildDeleteDate = UnitFields.End + 15, // size 1, flags MIRROR_ALL
    GuildLevel = UnitFields.End + 16, // size 1, flags MIRROR_ALL
    HairColorID = UnitFields.End + 17, // size 1, flags MIRROR_ALL
    CustomDisplayOption = UnitFields.End + 18, // size 1, flags MIRROR_ALL
    Inebriation = UnitFields.End + 19, // size 1, flags MIRROR_ALL
    ArenaFaction = UnitFields.End + 20, // size 1, flags MIRROR_ALL
    DuelTeam = UnitFields.End + 21, // size 1, flags MIRROR_ALL
    GuildTimeStamp = UnitFields.End + 22, // size 1, flags MIRROR_ALL
    QuestLog = UnitFields.End + 23, // size 800, flags MIRROR_PARTY
                                    // size 38, flags MIRROR_ALL
    PlayerTitle = UnitFields.End + 861 + 800, // size 1, flags MIRROR_ALL
    FakeInebriation = UnitFields.End + 862 + 800, // size 1, flags MIRROR_ALL
    VirtualPlayerRealm = UnitFields.End + 863 + 800, // size 1, flags MIRROR_ALL
    CurrentSpecID = UnitFields.End + 864 + 800, // size 1, flags MIRROR_ALL
    TaxiMountAnimKitID = UnitFields.End + 865 + 800, // size 1, flags MIRROR_ALL
    AvgItemLevel = UnitFields.End + 866 + 800, // size 4, flags MIRROR_ALL
    CurrentBattlePetBreedQuality = UnitFields.End + 870 + 800, // size 1, flags MIRROR_ALL
    HonorLevel = UnitFields.End + 871 + 800, // size 1, flags MIRROR_ALL
    End = 192
}

public enum ActivePlayerFields
{
    EnableInvSlots = 112,
    InvSlots = 113, // size 780, flags MIRROR_ALL
    FarsightObject = PlayerFields.End + 780, // size 4, flags MIRROR_ALL
    SummonedBattlePetGUID = PlayerFields.End + 784, // size 4, flags MIRROR_ALL
    KnownTitles = PlayerFields.End + 788, // size 12, flags MIRROR_ALL
    Coinage = PlayerFields.End + 800, // size 2, flags MIRROR_ALL
    XP = PlayerFields.End + 802, // size 1, flags MIRROR_ALL
    NextLevelXP = PlayerFields.End + 803, // size 1, flags MIRROR_ALL
    TrialXP = PlayerFields.End + 804, // size 1, flags MIRROR_ALL
    Skill = PlayerFields.End + 805, // size 896, flags MIRROR_ALL
    CharacterPoints = PlayerFields.End + 1701, // size 1, flags MIRROR_ALL
    MaxTalentTiers = PlayerFields.End + 1702, // size 1, flags MIRROR_ALL
    TrackCreatureMask = PlayerFields.End + 1703, // size 1, flags MIRROR_ALL
    TrackResourceMask = PlayerFields.End + 1706, // size 1, flags MIRROR_ALL
    MainhandExpertise = PlayerFields.End + 1707, // size 1, flags MIRROR_ALL
    OffhandExpertise = PlayerFields.End + 1708, // size 1, flags MIRROR_ALL
    RangedExpertise = PlayerFields.End + 1709, // size 1, flags MIRROR_ALL
    CombatRatingExpertise = PlayerFields.End + 1710, // size 1, flags MIRROR_ALL
    BlockPercentage = PlayerFields.End + 1710, // size 1, flags MIRROR_ALL
    DodgePercentage = PlayerFields.End + 1711, // size 1, flags MIRROR_ALL
    DodgePercentageFromAttribute = PlayerFields.End + 1712, // size 1, flags MIRROR_ALL
    ParryPercentage = PlayerFields.End + 1713, // size 1, flags MIRROR_ALL
    ParryPercentageFromAttribute = PlayerFields.End + 1714, // size 1, flags MIRROR_ALL
    CritPercentage = PlayerFields.End + 1715, // size 1, flags MIRROR_ALL
    RangedCritPercentage = PlayerFields.End + 1716, // size 1, flags MIRROR_ALL
    OffhandCritPercentage = PlayerFields.End + 1717, // size 1, flags MIRROR_ALL
    SpellCritPercentage = PlayerFields.End + 1718, // size 1, flags MIRROR_ALL
    ShieldBlock = PlayerFields.End + 1719, // size 1, flags MIRROR_ALL
    ShieldBlockCritPercentage = PlayerFields.End + 1720, // size 1, flags MIRROR_ALL
    Mastery = PlayerFields.End + 1721, // size 1, flags MIRROR_ALL
    Speed = PlayerFields.End + 1722, // size 1, flags MIRROR_ALL
    Avoidance = PlayerFields.End + 1723, // size 1, flags MIRROR_ALL
    Sturdiness = PlayerFields.End + 1724, // size 1, flags MIRROR_ALL
    Versatility = PlayerFields.End + 1725, // size 1, flags MIRROR_ALL
    VersatilityBonus = PlayerFields.End + 1726, // size 1, flags MIRROR_ALL
    PvpPowerDamage = PlayerFields.End + 1727, // size 1, flags MIRROR_ALL
    PvpPowerHealing = PlayerFields.End + 1728, // size 1, flags MIRROR_ALL
    ExploredZones = PlayerFields.End + 1729, // size 320, flags MIRROR_ALL
    RestInfo = PlayerFields.End + 2049, // size 4, flags MIRROR_ALL
    ModDamageDonePos = PlayerFields.End + 2053, // size 7, flags MIRROR_ALL
    ModDamageDoneNeg = PlayerFields.End + 2060, // size 7, flags MIRROR_ALL
    ModDamageDonePercent = PlayerFields.End + 2067, // size 7, flags MIRROR_ALL
    ModHealingDonePos = PlayerFields.End + 2074, // size 1, flags MIRROR_ALL
    ModHealingPercent = PlayerFields.End + 2075, // size 1, flags MIRROR_ALL
    ModHealingDonePercent = PlayerFields.End + 2076, // size 1, flags MIRROR_ALL
    ModPeriodicHealingDonePercent = PlayerFields.End + 2077, // size 1, flags MIRROR_ALL
    WeaponDmgMultipliers = PlayerFields.End + 2078, // size 3, flags MIRROR_ALL
    WeaponAtkSpeedMultipliers = PlayerFields.End + 2081, // size 3, flags MIRROR_ALL
    ModSpellPowerPercent = PlayerFields.End + 2084, // size 1, flags MIRROR_ALL
    ModResiliencePercent = PlayerFields.End + 2085, // size 1, flags MIRROR_ALL
    OverrideSpellPowerByAPPercent = PlayerFields.End + 2086, // size 1, flags MIRROR_ALL
    OverrideAPBySpellPowerPercent = PlayerFields.End + 2087, // size 1, flags MIRROR_ALL
    ModTargetResistance = PlayerFields.End + 2088, // size 1, flags MIRROR_ALL
    ModTargetPhysicalResistance = PlayerFields.End + 2089, // size 1, flags MIRROR_ALL
    LocalFlags = PlayerFields.End + 2090, // size 1, flags MIRROR_ALL
    NumRespecs = PlayerFields.End + 2091, // size 1, flags MIRROR_ALL
    PvpMedals = PlayerFields.End + 2092, // size 1, flags MIRROR_ALL
    BuybackPrice = PlayerFields.End + 2093, // size 12, flags MIRROR_ALL
    BuybackTimestamp = PlayerFields.End + 2105, // size 12, flags MIRROR_ALL
    YesterdayHonorableKills = PlayerFields.End + 2117, // size 1, flags MIRROR_ALL
    LifetimeHonorableKills = PlayerFields.End + 2118, // size 1, flags MIRROR_ALL
    WatchedFactionIndex = PlayerFields.End + 2119, // size 1, flags MIRROR_ALL
    CombatRatings = PlayerFields.End + 2120, // size 32, flags MIRROR_ALL
    PvpInfo = PlayerFields.End + 2152, // size 54, flags MIRROR_ALL
    MaxLevel = PlayerFields.End + 2206, // size 1, flags MIRROR_ALL
    ScalingPlayerLevelDelta = PlayerFields.End + 2207, // size 1, flags MIRROR_ALL
    MaxCreatureScalingLevel = PlayerFields.End + 2208, // size 1, flags MIRROR_ALL
    NoReagentCostMask = PlayerFields.End + 2209, // size 4, flags MIRROR_ALL
    PetSpellPower = PlayerFields.End + 2213, // size 1, flags MIRROR_ALL
    ProfessionSkillLine = PlayerFields.End + 2214, // size 2, flags MIRROR_ALL
    UiHitModifier = PlayerFields.End + 2216, // size 1, flags MIRROR_ALL
    UiSpellHitModifier = PlayerFields.End + 2217, // size 1, flags MIRROR_ALL
    HomeRealmTimeOffset = PlayerFields.End + 2218, // size 1, flags MIRROR_ALL
    ModPetHaste = PlayerFields.End + 2219, // size 1, flags MIRROR_ALL
    NumBackpackSlots = PlayerFields.End + 2220, // size 1, flags MIRROR_ALL
    OverrideSpellsID = PlayerFields.End + 2221, // size 1, flags MIRROR_ALL, MIRROR_URGENT_SELF_ONLY
    LfgBonusFactionID = PlayerFields.End + 2222, // size 1, flags MIRROR_ALL
    LootSpecID = PlayerFields.End + 2223, // size 1, flags MIRROR_ALL
    OverrideZonePVPType = PlayerFields.End + 2224, // size 1, flags MIRROR_ALL, MIRROR_URGENT_SELF_ONLY
    BagSlotFlags = PlayerFields.End + 2225, // size 4, flags MIRROR_ALL
    BankBagSlotFlags = PlayerFields.End + 2229, // size 7, flags MIRROR_ALL
    InsertItemsLeftToRight = PlayerFields.End + 2236, // size 1, flags MIRROR_ALL
    QuestCompleted = PlayerFields.End + 2237, // size 1750, flags MIRROR_ALL
    Honor = PlayerFields.End + 3987, // size 1, flags MIRROR_ALL
    HonorNextLevel = PlayerFields.End + 3988, // size 1, flags MIRROR_ALL
    PvpTierMaxFromWins = PlayerFields.End + 3989, // size 1, flags MIRROR_ALL
    PvpLastWeeksTierMaxFromWins = PlayerFields.End + 3990, // size 1, flags MIRROR_ALL
    End = 558 + 32 + 2 + 4 + 6 + 1 + 1 + 2
}

public enum GameObjectFields
{
    CreatedBy = ObjectFields.End + 0, // size 4, flags MIRROR_ALL
    GuildGUID = ObjectFields.End + 4, // size 4, flags MIRROR_ALL
    DisplayID = ObjectFields.End + 8, // size 1, flags MIRROR_VIEWER_DEPENDENT, MIRROR_URGENT
    Flags = ObjectFields.End + 9, // size 1, flags MIRROR_ALL, MIRROR_URGENT
    ParentRotation = ObjectFields.End + 10, // size 4, flags MIRROR_ALL
    FactionTemplate = ObjectFields.End + 14, // size 1, flags MIRROR_ALL
    Level = ObjectFields.End + 15, // size 1, flags MIRROR_ALL
    PercentHealth = ObjectFields.End + 16, // size 1, flags MIRROR_ALL, MIRROR_URGENT
    SpellVisualID = ObjectFields.End + 17, // size 1, flags MIRROR_ALL, MIRROR_VIEWER_DEPENDENT, MIRROR_URGENT
    StateSpellVisualID = ObjectFields.End + 18, // size 1, flags MIRROR_VIEWER_DEPENDENT, MIRROR_URGENT
    SpawnTrackingStateAnimID = ObjectFields.End + 19, // size 1, flags MIRROR_VIEWER_DEPENDENT, MIRROR_URGENT
    SpawnTrackingStateAnimKitID = ObjectFields.End + 20, // size 1, flags MIRROR_VIEWER_DEPENDENT, MIRROR_URGENT
    StateWorldEffectID = ObjectFields.End + 21, // size 4, flags MIRROR_VIEWER_DEPENDENT, MIRROR_URGENT
    CustomParam = ObjectFields.End + 25, // size 1, flags MIRROR_ALL, MIRROR_URGENT
    End = ObjectFields.End + 26
}

public enum CGDynamicObjectData
{
    Caster = ObjectFields.End + 0, // size 4, flags MIRROR_ALL
    Type = ObjectFields.End + 4, // size 1, flags MIRROR_ALL
    SpellXSpellVisualID = ObjectFields.End + 5, // size 1, flags MIRROR_ALL
    SpellID = ObjectFields.End + 6, // size 1, flags MIRROR_ALL
    Radius = ObjectFields.End + 7, // size 1, flags MIRROR_ALL
    CastTime = ObjectFields.End + 8, // size 1, flags MIRROR_ALL
    End = ObjectFields.End + 9
}

public enum CGCorpseData
{
    Owner = ObjectFields.End + 0, // size 4, flags MIRROR_ALL
    PartyGUID = ObjectFields.End + 4, // size 4, flags MIRROR_ALL
    GuildGUID = ObjectFields.End + 8, // size 4, flags MIRROR_ALL
    DisplayID = ObjectFields.End + 12, // size 1, flags MIRROR_ALL
    Items = ObjectFields.End + 13, // size 19, flags MIRROR_ALL
    SkinID = ObjectFields.End + 32, // size 1, flags MIRROR_ALL
    FacialHairStyleID = ObjectFields.End + 33, // size 1, flags MIRROR_ALL
    Flags = ObjectFields.End + 34, // size 1, flags MIRROR_ALL
    DynamicFlags = ObjectFields.End + 35, // size 1, flags MIRROR_VIEWER_DEPENDENT
    FactionTemplate = ObjectFields.End + 36, // size 1, flags MIRROR_ALL
    CustomDisplayOption = ObjectFields.End + 37, // size 1, flags MIRROR_ALL
    End = ObjectFields.End + 38
}

public enum CGAreaTriggerData
{
    OverrideScaleCurve = ObjectFields.End + 0, // size 7, flags MIRROR_ALL, MIRROR_URGENT
    ExtraScaleCurve = ObjectFields.End + 7, // size 7, flags MIRROR_ALL, MIRROR_URGENT
    Caster = ObjectFields.End + 14, // size 4, flags MIRROR_ALL
    Duration = ObjectFields.End + 18, // size 1, flags MIRROR_ALL
    TimeToTarget = ObjectFields.End + 19, // size 1, flags MIRROR_ALL, MIRROR_URGENT
    TimeToTargetScale = ObjectFields.End + 20, // size 1, flags MIRROR_ALL, MIRROR_URGENT
    TimeToTargetExtraScale = ObjectFields.End + 21, // size 1, flags MIRROR_ALL, MIRROR_URGENT
    SpellID = ObjectFields.End + 22, // size 1, flags MIRROR_ALL
    SpellForVisuals = ObjectFields.End + 23, // size 1, flags MIRROR_ALL
    SpellXSpellVisualID = ObjectFields.End + 24, // size 1, flags MIRROR_ALL
    BoundsRadius2D = ObjectFields.End + 25, // size 1, flags MIRROR_VIEWER_DEPENDENT, MIRROR_URGENT
    DecalPropertiesID = ObjectFields.End + 26, // size 1, flags MIRROR_ALL
    CreatingEffectGUID = ObjectFields.End + 27, // size 4, flags MIRROR_ALL
    End = ObjectFields.End + 31
}

public enum CGSceneObjectData
{
    ScriptPackageID = ObjectFields.End + 0, // size 1, flags MIRROR_ALL
    RndSeedVal = ObjectFields.End + 1, // size 1, flags MIRROR_ALL
    CreatedBy = ObjectFields.End + 2, // size 4, flags MIRROR_ALL
    SceneType = ObjectFields.End + 6, // size 1, flags MIRROR_ALL
    End = ObjectFields.End + 7
}

public enum CGConversationData
{
    LastLineEndTime = ObjectFields.End + 0, // size 1, flags MIRROR_VIEWER_DEPENDENT
    End = ObjectFields.End + 1
}
