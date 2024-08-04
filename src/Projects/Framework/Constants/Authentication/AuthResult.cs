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

namespace Framework.Constants.Authentication
{
    public enum AuthResult : byte
    {
        GlobalSuccess       = 0x00,
        BadLoginInformation = 0x68,
        InvalidProgram      = 0x6D,
        InvalidPlatform     = 0x6E,
        InvalidLocale       = 0x6F,
        InvalidGameVersion  = 0x70,
        AlreadyLoggedIn     = 0xCD,
    }
}
