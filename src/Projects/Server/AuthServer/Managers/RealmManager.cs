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

using Framework.Constants.Misc;
using Framework.Database.Auth.Entities;
using Framework.Logging;
using Framework.Misc;
using System.Collections.Concurrent;

namespace AuthServer.Managers
{
    class RealmManager : Singleton<RealmManager>
    {
        public bool IsInitialized { get; private set; }
        public readonly ConcurrentDictionary<int, Realm> RealmList;

        RealmManager()
        {
            IsInitialized = false;

            RealmList = new ConcurrentDictionary<int, Realm>();

            var realm = new Realm
            {
                Id     = 1,
                Name   = "arctium.io",
                IP     = "127.0.0.1",
                Port   = 8100,
                Type   = 0,
                Status = 1,
                Flags  = 0,
            };

            if (RealmList.TryAdd(realm.Id, realm))
                Log.Message(LogType.Debug, "Added Realm (Id: {0}, Name: {1})", realm.Id, realm.Name);
        }
    }
}
