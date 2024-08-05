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
    public enum ClientMessage : ushort
    {
        //EnableCrypt = 0x3769,
        EnableCrypt2 = 0x3767,
        #region ChatMessages
        ChatMessageSay = 0x37E7,
        ChatMessageYell = 0xFFFF,
        ChatMessageWhisper = 0xFFFF,
        #endregion

        #region UserRouterClient
        AuthContinuedSession = 0x3766,//20740
        test = 0x3764,// 20740
        AuthSession = 0x3765,// 20740
        Ping = 0x1897,
        LogDisconnect = 0x3769,//20740
        #endregion

        #region Legacy
        ActivePlayer = 0xFFFF,
        #endregion

        CancelAura = 0x31AF, // 24700
        CastSpell = 0x32AC, // 24700
        #region JAM
        ObjectUpdateFailed = 0xFFFF,
        ViolenceLevel = 0x3185,//20740
        GenerateRandomCharacterName = 0x35E7, // 24700
        EnumCharacters = 0x35E8,// 24700
        EnumCharacters2 = 0x36BC, // 20740
        PlayerLogin = 0x35EA,//24700
        LoadingScreenNotify = 0x35F8, // 24700
        SetActionButton = 0x3636,
        CreateCharacter = 0x3643, // 25753
        QueryPlayerName = 0x376F, // 24700
        QueryRealmName = 0x3689, // 24700
                                 //ReadyForAccountDataTimes = 0x3699,
        UITimeRequest = 0xBADD,//369C,//0x369F,
        CharDelete = 0x369A,// 23993 // 24700
        CliSetSpecialization = 0xFFFF,
        CliLearnTalents = 0xFFFF,
        CliQueryCreature = 0x327E, // 24700
        CliQueryGameObject = 0x327F, // 24700
        CliQueryNPCText = 0xFFFF,
        CliTalkToGossip = 0xFFFF,
        CliLogoutRequest = 0x34E4, // -2
        CliSetSelection = 0x353A, // 24700
        #endregion

        #region PlayerMove
        Move1 = 0x39E4,
        Move2 = 0x39E5,
        Move3 = 0x39E6,
        Move4 = 0x39E7,
        Move5 = 0x39E8,
        Move6 = 0x39E9,
        Move7 = 0x39EA,
        Move8 = 0x39EB,
        Move9 = 0x39EC,
        Move10 = 0x39ED,
        Move11 = 0x39EE,
        Move12 = 0x39EF,
        Move13 = 0x39F0,
        Move14 = 0x39F1,
        Move15 = 0x39F2,
        Move16 = 0x39F3,
        Move30 = 0x39FB,
        Move31 = 0x39FC,
        Move32 = 0x39FD,
        Move33 = 0x3A07,
        Move34 = 0x3A08,
        Move35 = 0x3A09,
        Move36 = 0x3A0A,
        Move37 = 0x3A0B,
        Move38 = 0x3A11,
        Move39 = 0x3A12,
        Move40 = 0x3A18,
        Move41 = 0x3A1A,
        Move42 = 0x3A1B,
        Move43 = 0x3A20,
        Move44 = 0x3A21,
        Move45 = 0x3A29,
        Move46 = 0x3A2A,
        Move47 = 0x3A2B,
        Move48 = 0x3A30,
        Move49 = 0x3A31,
        Move50 = 0x3A34,
        Move51 = 0x3A45,
        MoveSetActiveMover = 0x3A38,
        #endregion

        Emote = 0x348E,
        MapReset = 0x3553,
        EquipItem = 0x399A,
        DestroyItem = 0x32A2, // 25902
        SwapItem = 0x399C,
        UnEquipItem = 0x399D,

        TransferInitiate = 0xB1CE,

        GetGarrisonInfo = 0x0BFB,
        UpgradeGarrison = 0x0A9D,
        CalendarNum = 0x367C,
        DBQueryBulk = 0x35E4,
        HotfixRequest = 0x35E5,

        CommentatorEnable = 0x35F0
    }
}
