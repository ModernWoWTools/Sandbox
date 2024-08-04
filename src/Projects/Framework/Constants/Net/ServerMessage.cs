/*
 * Copyright (C) 2012-2014 Arctium Emulation <http://arctium.org>
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

namespace Framework.Constants.Net
{
    public enum ServerMessage : ushort
    {
        EnableCrypt = 0x3049,
        // move
        UnkMove = 0x2DDE,
        ConnectTo = 0x304D, // 20740
        SuspendComms = 0x304A,// 20740
        ResumeComms = 0x304B,//20740
        #region Legacy
        ObjectUpdate = 0x27BA, // 25902
        TutorialFlags = 0x27AC, // 25826
        StartCinematic = 0x0E29,
        ihavenoiea = 0x26FE,
        #endregion

        LevelUpInfo = 0x26DD, // 25902

        #region JAMClientConnection
        AuthChallenge = 0x3048,//20740
        Pong = 0x904D,//20740
        #endregion

        BattlenetStart = 0xC08,
        UnknownMount = 0x2C38,
        #region JAMClientMove
        MoveUpdate = 0x2DE0,
        MoveSetCanFly = 0x2E05,
        MoveUnsetCanFly = 0x2E06,
        MoveSetWalkSpeed = 0x2DF6,
        MoveSetRunSpeed = 0x2DF0,
        MoveSetSwimSpeed = 0x2DF2,
        MoveSetFlightSpeed = 0x2DF4,
        MoveTeleport = 0x2E04,
        MonsterMove = 0x2DD4,
        SetActiveMover = 0x2DD5,
        SetCollision = 0x2E11,
        #endregion

        #region JAMClientGossip
        GossipMessage = 0x0E52,
        #endregion

        #region JAMClientSpell
        SendKnownSpells = 0x2C2A,
        #endregion
        //0x2a95 questcache smsg
        #region JAMClientDispatch
        QueryCreatureResponse = 0x2914, // 25902
        EnumCharactersResult = 0x2583, // 24700
        LogoutComplete = 0x267C, // 25826
        TransferPending = 0x25C9,  // 25826
        NewWorld = 0x2598,  // 25826
        UnlearnedSpells = 0x2C4E,
        QueryGameObjectResponse = 0x2915, // 25902
        GenerateRandomCharacterNameResult = 0x2585, // 25826
        UITime = 0x275F, // 25902
        UpdateTalentData = 0x25D0, // 24700
        UpdateToyData = 0x25C5,
        LearnedSpells = 0x2C4D,
        PhaseChange = 0x2578,

        SpellStart = 0x2C3A,
        AuraUpdate = 0x2C22,
        SpellGo = 0x2C39,

        SpellChannelStart = 0x2C34,
        SpellChannelUpdate = 0x2C35,
        PlaySpellVisual = 0x2C45,
        PlaySpellVisualKit = 0x2C49,

        MirrorImageComponentedData = 0x2C14,
        MirrorImageCreatureData = 0x2C13,

        MOTD = 0x2BAF,
        Chat = 0x2BAD,
        ResetCompressionContext = 0x304F,
        Compression = 0x3052,

        AccountDataTimes = 0x26FE,//25902
        EnableDJump = 0x2DFC,
        AchievementEarned = 0x263A, // 25826
        AllAchievementData = 0x2570,
        LoginSetTimeSpeed = 0x2701, // 25902
        SetTimeZoneInformation = 0x266F, // 25902
        QueryNPCTextResponse = 0x2916,
        AddonInfo = 0x256D,// 20740
        CreateChar = 0x26F5, // 25902, 0x273E + 1
        DeleteChar = 0x26F6, // 25826
        AuthResponse = 0x256D,// 25826
        RealmQueryResponse = 0x2913, // 25902, 0x26E5 + 1
        UpdateActionButtons = 0x25D9, // 25902
        DestroyObject = 7436,
        CacheVersion = 0x291C, // 25826
        QueryPlayerNameResponse = 0x3002, // 25902, 0x2700 + 1
        DBReply = 0x290E, // 24700
        AvailableHotfixes = 0x290F, // 24700
        HotfixMessage = 0x2910,
        #endregion
        TokenDist = 0xFFFF,
        TokenDistGlue = 0xBADD,
        TokenTime = 0x2819,
        TokenBalance = 0x284C,
        Emote = 0x27B8, // 25826
        PlaySound = 0x275A,
        FeatureSystemStatusGlueScreen = 0x25BC,
        DistributionList = 0x775,
        ItemPushResult = 0x2618,
        TransferInitiate = 0x4F57,

        GarrisonAddMissionResult = 0x2925,
        GarrisonArchitectShow = 0x0A15,
        GarrisonUpgradeResult = 0x0E0B,
        GetGarrisonInfoResult = 0x0826,
        GarrisonOpenMissionNPC = 0x067E,
        OpenShipmentNPCFromGossip = 0x0D76,
        GetShipmentInfoResponse = 0x080D,
        OpenShipmentNPCResult = 0x0C15,

        InitWorldStates = 0x2738,
        UpdateWorldState = 0x2739,
        PlayScene = 0x262E, // 25826
        CancelScene = 0x262F, // 25826
        CalendarNum = 0x26C7,
        ClearTarget = 0x2942,
    }
}
