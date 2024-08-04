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
using Framework.Logging;

namespace AuthServer.Configuration
{
    class AuthConfig
    {
        #region Config Options
        public static LogType LogLevel;
        public static string BindIP;
        public static int BindPort;
        #endregion

        public static void ReadConfig()
        {
#if !PUBLIC
            LogLevel =  LogType.Init | LogType.Normal | LogType.Error | LogType.Debug | LogType.Info;
            LogLevel =  (LogType)0xFF; //LogType.Init | LogType.Normal | LogType.Error | LogType.Debug | LogType.Info;
#else
            LogLevel = LogType.Init | LogType.Normal | LogType.Error | LogType.Debug | LogType.Info;
#endif
            BindIP = "0.0.0.0";
            BindPort = 8000;

            Log.Initialize(LogLevel, null);
        }
    }
}
