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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using AuthServer.AuthServer.Network;
using Framework.Constants.Misc;
using Framework.Logging;

namespace AuthServer.Network
{
    class Server : IDisposable
    {
        TcpListener listener;
        bool isRunning;

        public Server(string ip, int port, bool alt = false)
        {
            var bindIP = IPAddress.None;

            if (!IPAddress.TryParse(ip, out bindIP))
            {
                Log.Message(LogType.Normal, "AuthServer can't be started: Invalid IP-Address ({0})", ip);
                Console.ReadKey(true);

                Environment.Exit(0);
            }

            try
            {
                listener = new TcpListener(bindIP, port);
                listener.Start();

                if (isRunning = listener.Server.IsBound)
                {
                    if (!alt)
                        Task.Factory.StartNew(AcceptConnection);
                    else
                        Task.Factory.StartNew(AcceptRestConnection);
                }
            }
            catch (Exception ex)
            {
                Log.Message(LogType.Error, "{0}", ex.Message);
            }
        }

        async void AcceptConnection()
        {
            while (isRunning)
            {
                // Accept a new connection every 200ms.
                Thread.Sleep(200);

                if (listener.Pending())
                {
                    var clientSocket = await listener.AcceptSocketAsync();

                    if (clientSocket != null)
                    {
                        Sandbox.AuthSession = new AuthSession(clientSocket);

                        await Task.Factory.StartNew(Sandbox.AuthSession.Accept);
                    }
                }
            }
        }

        async void AcceptRestConnection()
        {
            while (isRunning)
            {
                // Accept a new connection every 200ms.
                Thread.Sleep(200);

                if (listener.Pending())
                {
                    var clientSocket = await listener.AcceptSocketAsync();

                    if (clientSocket != null)
                    {
                        var worker = new RestSession(clientSocket);

                        await Task.Factory.StartNew(worker.Accept);
                    }
                }
            }
        }

        public void Dispose()
        {
            listener = null;
            isRunning = false;
        }
    }
}
