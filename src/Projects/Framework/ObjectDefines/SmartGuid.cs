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

namespace Framework.ObjectDefines
{
    [Serializable()]
    public class SmartGuid
    {
		public ulong Guid { get; set; }
		public ulong HighGuid { get; set; }
        public byte[] Raw { get; set; }

        public SmartGuid() {  }
		public SmartGuid(ulong guid, int id, GuidType highType, ulong mapId = 0)
		{
            if (highType == GuidType.Creature)
		    {
		        Guid |= guid;
		        HighGuid |= (ulong) 1 << 42;
		        HighGuid |= mapId << 29;

		    }
            else
		         Guid = guid;

            HighGuid = ((ulong)highType << 0x3A) | (ulong)id << 6;
        }
        public GuidType Type
        {
            get { return (GuidType)(HighGuid >> 58); }
            set { HighGuid |= (ulong)value << 58; }
        }

        public GuidSubType SubType
        {
            get { return (GuidSubType)(Guid >> 56); }
            set { Guid |= (ulong)value << 56; }
        }

        public ushort RealmId
        {
            get { return (ushort)((HighGuid >> 42) & 0x1FFF); }
            set { HighGuid |= (ulong)value << 42; }
        }

        public ushort ServerId
        {
            get { return (ushort)((Guid >> 40) & 0xFFFFFF); }
            set { Guid |= (ulong)value << 40; }
        }

        public ushort MapId
        {
            get { return (ushort)((HighGuid >> 29) & 0x1FFF); }
            set { HighGuid |= (ulong)value << 29; }
        }

        public uint Id
        {
            get { return (uint)(HighGuid & 0xFFFFFF) >> 6; }
            set { HighGuid |= (ulong)value << 6; }
        }

        public ulong CreationBits
        {
            get { return Guid & 0xFFFFFFFFFF; }
            set { Guid |= value; }
        }
    }
}

public enum GuidSubType : byte
{
    None = 0
}

public enum GuidType : byte
{
    Null             = 0,
    Uniq             = 1,
    Player           = 2,
    Item             = 3,
    WorldTransaction = 4,
    StaticDoor       = 5,
    Transport        = 6,
    Conversation     = 7,
    Creature         = 8,
    Vehicle          = 9,
    Pet              = 10,
    GameObject       = 11,
    DynamicObject    = 11,
    AreaTrigger      = 12,
    Corpse           = 13,
    LootObject       = 14,
    SceneObject      = 15,
    Scenario         = 16,
    AIGroup          = 17,
    DynamicDoor      = 18,
    ClientActor      = 19,
    Vignette         = 20,
    CallForHelp      = 21,
    AIResource       = 22,
    AILock           = 23,
    AILockTicket     = 24,
    ChatChannel      = 25,
    Party            = 26,
    Guild            = 27,
    WowAccount       = 28,
    BNetAccount      = 29,
    GMTask           = 30,
    MobileSession    = 31,
    RaidGroup        = 32,
    Spell            = 33,
    Mail             = 34,
    WebObj           = 35,
    LFGObject        = 36,
    LFGList          = 37,
    UserRouter       = 38,
    PVPQueueGroup    = 39,
    UserClient       = 40,
    PetBattle        = 41,
    UniqueUserClient = 42,
    BattlePet        = 43,
    CommerceObj      = 44,
    ClientSession    = 45,
    Cast             = 47
}
