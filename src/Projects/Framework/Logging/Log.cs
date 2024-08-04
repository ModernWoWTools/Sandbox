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

using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Framework.Constants.Misc;
using Framework.Logging.IO;

namespace Framework.Logging
{
    public class Log
    {
        static LogType logLevel;
        static BlockingCollection<Tuple<ConsoleColor, string>> logQueue = new BlockingCollection<Tuple<ConsoleColor, string>>();

        public static void SetLogLevel(LogType logLvl) => logLevel = logLvl;

        public static async void Initialize(LogType logLevel, LogWriter fileLogger = null)
        {
            Log.logLevel = logLevel;

            await Task.Delay(1).ContinueWith(async _ =>
            {
                while (true)
                {
                    var log = logQueue.Take();

                    if (log != null)
                    {
                        var msg = log.Item2;

                        if (fileLogger != null)
                            await fileLogger.Write(msg);

                        Console.ForegroundColor = log.Item1;
                        Console.WriteLine(msg);
                    }
                }
            });
        }

        static public void Message()
        {
            SetLogger(LogType.None, "");
        }

        static public void Message(LogType type, string text, params object[] args)
        {
            SetLogger(type, text, args);
        }

        static public void Wait()
        {
            Console.ReadKey(true);
        }

        static void SetLogger(LogType type, string text, params object[] args)
        {
            //Console.OutputEncoding = UTF8Encoding.UTF8;
            ConsoleColor foreGround;

            switch (type)
            {
                case LogType.Init:
                    foreGround = ConsoleColor.Cyan;
                    break;
                case LogType.Normal:
                    foreGround = ConsoleColor.Green;
                    text = text.Insert(0, "System: ");
                    break;
                case LogType.Error:
                    foreGround = ConsoleColor.Red;
                    text = text.Insert(0, "Error: ");
                    break;
                case LogType.Debug:
                    foreGround = ConsoleColor.DarkRed;
                    text = text.Insert(0, "Debug: ");
                    break;
                case LogType.Packet:
                case LogType.Info:
                    foreGround = ConsoleColor.Yellow;
                    break;
                case LogType.Database:
                    foreGround = ConsoleColor.DarkMagenta;
                    break;
                default:
                    foreGround = ConsoleColor.White;
                    break;
            }

            if ((logLevel & type) == type)
            {
                if (type.Equals(LogType.Init) || type.Equals(LogType.None))
                    logQueue.Add(Tuple.Create(foreGround, string.Format(text, args)));
                else
                    logQueue.Add(Tuple.Create(foreGround, string.Format("[" + DateTime.Now.ToLongTimeString() + "] " + text, args)));
            }
        }
    }
}
