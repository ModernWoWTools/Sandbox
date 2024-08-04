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

namespace Framework.Database.Auth.Entities
{
    public class Account
    {
        public int Id                  { get; set; }
        public string Name             { get; set; }
        public string Email            { get; set; }
        public string PasswordVerifier { get; set; }
        public string Salt             { get; set; }
        public string IP               { get; set; }
        public string SessionKey       { get; set; }
        public byte SecurityFlags      { get; set; }
        public string Language         { get; set; }
        public string OS               { get; set; }
        public byte Expansion          { get; set; }
        public bool IsOnline           { get; set; }
    }
}
