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

using Framework.Database.Auth.Entities;
using Framework.Misc;
using System.Collections.Generic;

namespace AuthServer.Managers
{
    class ComponentManager : Singleton<ComponentManager>
    {
        public readonly List<Component> ComponentList;

        ComponentManager()
        {
            ComponentList = new List<Component>();

            UpdateComponents();
        }

        void UpdateComponents()
        {
            ComponentList.Add(new Component
            {
                Program = "Bnet",
                Platform = "Win",
                Build = 37165
            });

            ComponentList.Add(new Component
            {
                Program = "Bnet",
                Platform = "Wn64",
                Build = 37165
            });

            ComponentList.Add(new Component
            {
                Program = "Bnet",
                Platform = "Mc64",
                Build = 37165
            });

            // WoW
            ComponentList.Add(new Component
            {
                Program = "WoW",
                Platform = "base",
                Build = 20365
            });

            ComponentList.Add(new Component
            {
                Program = "WoW",
                Platform = "enUS",
                Build = 20740
            });

            ComponentList.Add(new Component
            {
                Program = "WoW",
                Platform = "enGB",
                Build = 20740
            });

            ComponentList.Add(new Component
            {
                Program = "WoW",
                Platform = "ruRU",
                Build = 20740
            });

            ComponentList.Add(new Component
            {
                Program = "WoW",
                Platform = "deDE",
                Build = 20740
            });

            ComponentList.Add(new Component
            {
                Program = "WoW",
                Platform = "Win",
                Build = 20740
            });

            ComponentList.Add(new Component
            {
                Program = "WoW",
                Platform = "Wn64",
                Build = 20740
            });

            ComponentList.Add(new Component
            {
                Program = "WoWB",
                Platform = "Win",
                Build = 20740
            });

            ComponentList.Add(new Component
            {
                Program = "WoWB",
                Platform = "Wn64",
                Build = 20740
            });

            ComponentList.Add(new Component
            {
                Program = "WoWB",
                Platform = "Mc64",
                Build = 20740
            });

        }
    }
}
