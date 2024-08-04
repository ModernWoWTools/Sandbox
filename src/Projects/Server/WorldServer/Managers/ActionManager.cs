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

using System.Collections.Generic;
using System.Linq;
using Framework.Misc;
using AuthServer.Game.Entities;
using System;
using AuthServer.WorldServer.Managers;
using System.Runtime.Serialization;

[Serializable()]
[DataContract]
public class ActionButton
{
    [DataMember]
    public ulong Action;
    [DataMember]
    public byte SlotId;
    [DataMember]
    public byte SpecGroup;
}

namespace AuthServer.Game.Managers
{
    public class ActionManager : Singleton<ActionManager>
    {
        ActionManager() { }

        public void LoadActionButtons(Character pChar)
        {

        }

        public List<ActionButton> GetActionButtons(Character pChar, byte specGroup)
        {
            return pChar.ActionButtons.Where(action => action.SpecGroup == specGroup).ToList();
        }

        public void AddActionButton(Character pChar, ActionButton actionButton, bool addToDb = false)
        {
            if (pChar == null || actionButton == null)
                return;

            pChar.ActionButtons.Add(actionButton);

            Manager.ObjectMgr.SaveChar(pChar);
        }

        public void RemoveActionButton(Character pChar, ActionButton actionButton, bool deleteFromDb = false)
        {
            if (pChar == null || actionButton == null)
                return;

            var deleted = pChar.ActionButtons.Remove(actionButton);

            Manager.ObjectMgr.SaveChar(pChar);
        }
    }
}
