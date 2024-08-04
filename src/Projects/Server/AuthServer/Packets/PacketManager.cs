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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AuthServer.AuthServer.Attributes;
using AuthServer.Network;
using Bgs.Protocol;
using Bgs.Protocol.Connection.V1;
using Framework.Constants.Misc;
using Framework.Constants.Net;
using Framework.Logging;
using Google.Protobuf;
using static AuthServer.AuthServer.Attributes.BnetServiceBase;

namespace AuthServer.Packets
{
    class PacketManager
    {
        static Dictionary<BnetServiceHash, Dictionary<uint, Tuple<MethodInfo, Type>>> BnetHandlers = new Dictionary<BnetServiceHash, Dictionary<uint, Tuple<MethodInfo, Type>>>();

        public static void Initialize()
        {
            IMessage msg = new ConnectRequest();

            var assembly = Assembly.GetExecutingAssembly();

            foreach (var type in assembly.GetTypes().Where(t => t.GetCustomAttribute<BnetServiceAttribute>() != null))
            {
                var methods = new Dictionary<uint, Tuple<MethodInfo, Type>>();

                foreach (var method in type.GetMethods().Where(m => m.GetCustomAttribute<BnetMethodAttribute>() != null))
                {
                    var methodAttributeInfo = method.GetCustomAttribute<BnetMethodAttribute>();

                    methods.Add(methodAttributeInfo.MethodId, Tuple.Create(method, method.GetParameters()[1].ParameterType));
                }

                var serviceAttributeInfo = type.GetCustomAttribute<BnetServiceAttribute>();

                BnetHandlers.Add(serviceAttributeInfo.Hash, methods);
            }

            //InvokeHandler
        }

        public static void InvokeHandler(AuthSession session, Header header, byte[] mesageData)
        {
            Log.Message(LogType.Debug, $"Got  method id for '{header.MethodId}' for '{(BnetServiceHash)header.ServiceHash}'");
            Dictionary<uint, Tuple<MethodInfo, Type>> data;

            if (BnetHandlers.TryGetValue((BnetServiceHash)header.ServiceHash, out data))
            {
                Tuple<MethodInfo, Type> method;

                if (data.TryGetValue(header.MethodId, out method))
                {
                    var handlerObj = Activator.CreateInstance(method.Item2) as IMessage;

                    handlerObj.MergeFrom(mesageData);

                    //Console.WriteLine("{0}: {1}", header.MethodId + "/" + (BnetServiceHash)header.ServiceHash, handlerObj.ToString());
                    //Console.WriteLine();
                    //Console.WriteLine();

                    try
                    {
                        method.Item1.Invoke(null, new object[] { session, handlerObj });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.InnerException.StackTrace);
                    }
                }
                else
                    Log.Message(LogType.Error, $"Got unhandled method id for '{header.MethodId}' for '{(BnetServiceHash)header.ServiceHash}'");
            }
            else
            {

                if (!Enum.IsDefined(typeof(BnetServiceHash), header.ServiceHash))
                    Console.WriteLine($"Got unknown Bnet service '{header.ToString()}'");
                else
                    Console.WriteLine($"Got unhandled Bnet service '{header.ToString()}'");
            }
        }
    }
}
