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

namespace Framework.Constants
{
    public enum MessageType : byte
    {
        ChatMessageSystem              = 0x00,
        ChatMessageSay                 = 0x01,
        ChatMessageParty               = 0x02,
        ChatMessageRaid                = 0x03,
        ChatMessageGuild               = 0x04,
        ChatMessageOfficer             = 0x05,
        ChatMessageYell                = 0x06,
        ChatMessageWhisper             = 0x07,
        ChatMessageWhisperForeign      = 0x08,
        ChatMessageWhisperInform       = 0x09,
        ChatMessageEmote               = 0x0A,
        ChatMessageTextEmote           = 0x0B,
        ChatMessageMonsterSay          = 0x0C,
        ChatMessageMonsterParty        = 0x0D,
        ChatMessageMonsterYell         = 0x0E,
        ChatMessageMonsterWhisper      = 0x0F,
        ChatMessageMonsteeEmote        = 0x10,
        ChatMessageChannel             = 0x11,
        ChatMessageChannelJoin         = 0x12,
        ChatMessageChannelLeave        = 0x13,
        ChatMessageChannelList         = 0x14,
        ChatMessageChannelNotice       = 0x15,
        ChatMessageChannelNoticeUser   = 0x16,
        ChatMessageAfk                 = 0x17,
        ChatMessageDnd                 = 0x18,
        ChatMessageIgnored             = 0x19,
        ChatMessageSkill               = 0x1A,
        ChatMessageLoot                = 0x1B,
        ChatMessageMoney               = 0x1C,
        ChatMessageOpening             = 0x1D,
        ChatMessageTradeskills         = 0x1E,
        ChatMessagePetInfo             = 0x1F,
        ChatMessageCombatMiscInfo      = 0x20,
        ChatMessageCombatXpGain        = 0x21,
        ChatMessageCombatHonorGain     = 0x22,
        ChatMessageCombatFactionChange = 0x23,
        ChatMessageBgSystemNeutral     = 0x24,
        ChatMessageBgSystemAlliance    = 0x25,
        ChatMessageBgSystemHorde       = 0x26,
        ChatMessageRaidLeader          = 0x27,
        ChatMessageRaidWarning         = 0x28,
        ChatMessageRaidBossEmote       = 0x29,
        ChatMessageRaidBossWhisper     = 0x2A,
        ChatMessageFiltred             = 0x2B,
        ChatMessageBattleground        = 0x2C,
        ChatMessageBattlegroundLeader  = 0x2D,
        ChatMessageRestricted          = 0x2E,
        ChatMessageBattlenet           = 0x2F,
        ChatMessageAchievment          = 0x30,
        ChatMessageGuildAchievment     = 0x31,
        ChatMessageArenaPoints         = 0x32,
        ChatMessagePartyLeader         = 0x33
    }
}
