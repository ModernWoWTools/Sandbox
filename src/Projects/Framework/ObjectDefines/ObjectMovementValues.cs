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

using Framework.Constants;
using Framework.Constants.Movement;

namespace Framework.ObjectDefines
{
    // Initialize default values for ObjectUpdate movement
    public class ObjectMovementValues
    {
        // Bits
        public bool HasAnimKits               = false;
        public bool HasUnknown                = false;
        public uint BitCounter                = 0;
        public bool Bit0                      = false;
        public bool HasUnknown2               = false;
        public bool IsVehicle                 = false;
        public bool Bit2                      = false;
        public bool HasUnknown3               = false;
        public bool HasStationaryPosition     = false;
        public bool HasGoTransportPosition    = false;
        public bool IsSelf                    = false;
        public bool IsAlive                   = false;
        public uint BitCounter2               = 0;
        public bool Bit3                      = false;
        public bool HasUnknown4               = false;
        public bool HasTarget                 = false;
        public bool Bit1                      = false;
        public bool HasRotation               = false;
        public bool IsTransport               = false;
        public bool HasMovementFlags          = false;
        public bool HasMovementFlags2         = false;
        public bool IsFallingOrJumping        = false;
        public bool HasJumpData               = false;
        public bool IsAreaTrigger             = false;
        public bool IsSceneObject             = false;

        // Data
        public MovementFlag MovementFlags     = 0;
        public MovementFlag2 MovementFlags2   = 0;
        public uint Time                      = 0;

        // Jumping & Falling
        public float JumpVelocity             = 0;
        public float Cos                      = 0;
        public float Sin                      = 0;
        public float CurrentSpeed             = 0;
        public uint FallTime                  = 0;

        public ObjectMovementValues() { }
        public ObjectMovementValues(UpdateFlag updateflags)
        {
            IsSelf                 = (updateflags & UpdateFlag.Self)                != 0;
            IsAlive                = (updateflags & UpdateFlag.Alive)               != 0;
            HasRotation            = (updateflags & UpdateFlag.Rotation)            != 0;
            HasStationaryPosition  = (updateflags & UpdateFlag.StationaryPosition)  != 0;
            HasTarget              = (updateflags & UpdateFlag.Target)              != 0;
            IsTransport            = (updateflags & UpdateFlag.Transport)           != 0;
            HasGoTransportPosition = (updateflags & UpdateFlag.GoTransportPosition) != 0;
            HasAnimKits            = (updateflags & UpdateFlag.AnimKits)            != 0;
            IsVehicle              = (updateflags & UpdateFlag.Vehicle)             != 0;
        }
    }
}
