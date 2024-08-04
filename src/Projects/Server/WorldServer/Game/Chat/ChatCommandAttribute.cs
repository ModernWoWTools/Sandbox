﻿/*
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

namespace AuthServer.Game.Chat
{
    public class ChatCommandAttribute : Attribute
    {
        public string ChatCommand { get; set; }
        public string Description { get; set; }

        public ChatCommandAttribute(string chatCommand, string description = "")
        {
            ChatCommand = chatCommand;
            Description = description;
        }
    }

    public class ChatCommand2Attribute : Attribute
    {
        public string ChatCommand
        { get; set; }
        public string Description
        { get; set; }

        public ChatCommand2Attribute(string chatCommand, string description = "")
        {
            ChatCommand = chatCommand;
            Description = description;
        }
    }
}
