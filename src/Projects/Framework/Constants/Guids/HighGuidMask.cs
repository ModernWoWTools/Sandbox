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

namespace Framework.Constants
{
    [Flags]
    public enum HighGuidMask
    {
        None          = 0x00,
        Object        = 0x01,
        Item          = 0x02,
        Container     = 0x04,
        Unit          = 0x08,
        Player        = 0x10,
        GameObject    = 0x20,
        DynamicObject = 0x40,
        Corpse        = 0x80,
        Guild         = 0x100
    }
}
