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
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Framework.Logging.IO;

namespace Framework.Logging
{
    public class PacketLog
    {
        static LogWriter logger;

        public static void Initialize(string directory, string file)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            logger = new LogWriter(directory, file);
        }

        public static async void Write<T>(ushort value, byte[] data, EndPoint remote)
        {

            Func<Task> write = async delegate
            {
                var sb = new StringBuilder();
                var endPoint = remote as IPEndPoint;
                var clientInfo = endPoint.Address + ":" + endPoint.Port;

                sb.AppendLine(string.Format("Client: {0}", clientInfo));
                sb.AppendLine(string.Format("Time: {0}", DateTime.Now.ToString()));
                sb.AppendLine(string.Format("Type: {0}", typeof(T).Name));

                if (Enum.IsDefined(typeof(T), value))
                    sb.AppendLine(string.Format("Name: {0}", Enum.GetName(typeof(T), value)));

                sb.AppendLine(string.Format("Value: 0x{0:X} ({0})", value));
                sb.AppendLine(string.Format("Length: {0}", data.Length));

                sb.AppendLine("|----------------------------------------------------------------|");
                sb.AppendLine("| 00  01  02  03  04  05  06  07  08  09  0A  0B  0C  0D  0E  0F |");
                sb.AppendLine("|----------------------------------------------------------------|");
                sb.Append("|");

                var count = 0;
                var ctr = 0;

                foreach (var b in data)
                {
                    sb.Append(string.Format(" {0:X2} ", b));

                    if (count == 0xF)
                    {
                        sb.Append("|");
                        sb.Append("  " + Encoding.UTF8.GetString(data, ctr * 0x10, 0x10));
                        sb.AppendLine();
                        sb.Append("|");

                        count = 0;

                        ++ctr;
                    }
                    else
                        count++;
                };

                sb.AppendLine("");
                sb.AppendLine("|----------------------------------------------------------------|");
                sb.AppendLine("");

                await logger.Write(sb.ToString());
            };


            await write();

        }
    }
}
