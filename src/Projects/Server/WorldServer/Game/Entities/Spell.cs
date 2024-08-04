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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AuthServer.Game.WorldEntities
{
    [Serializable()]
    [DataContract]
    public enum PlayerSpellState
    {
        Unchanged = 0,
        Changed   = 1,
        New       = 2,
        Removed   = 3,
        Temporary = 4
    }

    [Serializable()]
    [DataContract]
    public class PlayerSpell
    {
        [DataMember]
        public uint SpellId;
        [DataMember]
        public PlayerSpellState State;
        [DataMember]
        public bool Active;
        [DataMember]
        public bool Dependent;
        [DataMember]
        public bool Disabled;
    }

    public static class SpellMethods
    {
        public static PlayerSpell FindPlayerSpell(this List<PlayerSpell> SpellList, uint spellId)
        {
            return SpellList.Find(p => p.SpellId == spellId);
        }
    }
}
