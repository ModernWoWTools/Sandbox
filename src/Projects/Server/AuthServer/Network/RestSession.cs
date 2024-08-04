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
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using AuthServer.AuthServer.JsonObjects;
using Bgs.Protocol;
using Framework.Constants.Misc;
using Framework.Cryptography.BNet;
using Framework.Database.Auth.Entities;
using Framework.Logging;
using Framework.Misc;
using Framework.Serialization;
using Google.Protobuf;

namespace AuthServer.AuthServer.Network
{
    public class RestSession : IDisposable
    {
        public Account Account { get; set; }
        public List<Module> Modules { get; set; }
        public SRP6a SecureRemotePassword { get; set; }
        public BNetCrypt Crypt { get; set; }

        Socket client;
        NetworkStream nStream;
        SslStream sslStream;
        byte[] dataBuffer = new byte[0x800];
        // X509Store store = new X509Store(StoreName.CertificateAuthority);
        public RestSession(Socket socket)
        {
            client = socket;
        }

        public string GetCInfo()
        {
            var endPoint = client.RemoteEndPoint as IPEndPoint;
            return endPoint.Address + ":" + endPoint.Port;
        }

        public void Accept()
        {
            //var context = new ;
            HttpListener listener = new HttpListener();
            //listener.
            nStream = new NetworkStream(client);
            sslStream = new SslStream(nStream, false, App_CertificateValidation);

            var cert = new X509Certificate(Globals.CertData);
            //var chain = X509Chain.Create().Build(cert);
            SslServerAuthenticationOptions op = new SslServerAuthenticationOptions();

            op.ApplicationProtocols = new List<SslApplicationProtocol>();
            op.ApplicationProtocols.Add(SslApplicationProtocol.Http11);
            op.CertificateRevocationCheckMode = X509RevocationMode.NoCheck;
            op.ClientCertificateRequired = false;
            op.EnabledSslProtocols = SslProtocols.Tls12;
            op.RemoteCertificateValidationCallback = App_CertificateValidation;
            op.ServerCertificate = cert;
            sslStream.AuthenticateAsServer(op);


            //store.Open(OpenFlags.ReadWrite);
            //store.Add(new X509Certificate2(@"C:\Users\Fabian\Downloads\bnetserver.cert.pfx"));
            //store.Add(new X509Certificate2(@"C:\Users\Fabian\Downloads\bnetserver.cert.pfx"));
            // store.Add(caCert);
            //store.Close();

            try
            {
                //sslStream.AuthenticateAsServer(cert, false, (SslProtocols)0xFFFF, false);


                // have to do a loop... but this is the first app packet.
                // decrypt it for testing.
                var ctr = 0;
                var postCtr = 0;

                do
                {
                    ctr = sslStream.Read(dataBuffer, 0, dataBuffer.Length);

                    var temp = new byte[ctr];
                    Array.Copy(dataBuffer, temp, ctr);

                    if (temp == null || temp.Length == 0)
                    {
                        client.Dispose();
                        return;
                    }

                    var request = HttpRequest.Parse(temp);

                    if (request.Method == "GET")
                    {
                        Console.WriteLine("GET");
                        // Logon Form
                        var logonForm = new FormInputs();

                        logonForm.type = "LOGIN_FORM";

                        logonForm.inputs = new List<FormInput>();

                        // AccountForm
                        logonForm.inputs.Add(new FormInput
                        {
                            input_id = "account_name",
                            type = "text",
                            label = "Battle.net email",
                            max_length = 320
                        });

                        // PasswordForm
                        logonForm.inputs.Add(new FormInput
                        {
                            input_id = "password",
                            type = "password",
                            label = "Password",
                            max_length = 16
                        });

                        // SubmitForm
                        logonForm.inputs.Add(new FormInput
                        {
                            input_id = "log_in_submit",
                            type = "submit",
                            label = "Log in to Battle.net"
                        });

                        var content = Json.CreateString(logonForm);
                        Console.WriteLine(content);
                        
                        var header = new HttpHeader
                        {
                            ContentLength = content.Length,
                            Content = content
                        };

                        var response = HttpResponse.Create(HttpCode.OK, header);

                        //PacketLog.Write<AuthServerMessage>(0, response, client.RemoteEndPoint);

                        sslStream.Write(response);
                    }
                    else if (request.Method == "POST")
                    {
                        Console.WriteLine("POST");
                        switch (postCtr++)
                        {
                            case 0:
                                var logonForm = Json.CreateObject<LogonForm>(request.Content);

                                if (logonForm.inputs.SingleOrDefault(i => i.input_id == "account_name")?.value == "arctium@arctium" && logonForm.inputs.SingleOrDefault(i => i.input_id == "password")?.value == "arctium")
                                {
                                    var postLogonForm = new PostLogonForm
                                    {
                                        authentication_state = AuthenticationState.Done,
                                        login_ticket = "ARCTIUM-SANDBOX-1"
                                    };

                                    var content = Json.CreateString(postLogonForm);

                                    var header = new HttpHeader
                                    {
                                        ContentLength = content.Length,
                                        Content = content
                                    };

                                    var response = HttpResponse.Create(HttpCode.OK, header);

                                    //PacketLog.Write<AuthServerMessage>(0, response, client.RemoteEndPoint);
                                    sslStream.Write(response);
                                }
                                else
                                {
                                    var postLogonForm = new PostLogonForm
                                    {
                                        authentication_state = AuthenticationState.Done,
                                        login_ticket = "WRONG-ACCOUNT"
                                    };

                                    var content = Json.CreateString(postLogonForm);

                                    var header = new HttpHeader
                                    {
                                        ContentLength = content.Length,
                                        Content = content
                                    };

                                    var response = HttpResponse.Create(HttpCode.OK, header);

                                    //PacketLog.Write<AuthServerMessage>(0, response, client.RemoteEndPoint);

                                    sslStream.Write(response);
                                }


                                break;
                            default:
                                Console.WriteLine("Got unknown request!!!");
                                break;
                        }
                    }

                    /*    if (wut == 0)
                    {
                        // Sniffed data
                        var byteData = "485454502f312e3120323030204f4b0d0a446174653a204672692c203036204d617920323031362031393a34393a343220474d540d0a5365727665723a204170616368650d0a52657472792d41667465723a203630300d0a5365742d436f6f6b69653a20414452554d5f4254613d22523a307c673a63343565616466612d333339322d343735632d396662622d303364373863356331363866223b2056657273696f6e3d313b204d61782d4167653d33303b20457870697265733d4672692c2030362d4d61792d323031362031393a35303a313220474d543b20506174683d2f3b205365637572650d0a5365742d436f6f6b69653a20414452554d5f4254313d22523a307c693a3337303035223b2056657273696f6e3d313b204d61782d4167653d33303b20457870697265733d4672692c2030362d4d61792d323031362031393a35303a313220474d543b20506174683d2f3b205365637572650d0a5365742d436f6f6b69653a20414452554d5f4254313d22523a307c693a33373030357c653a3433223b2056657273696f6e3d313b204d61782d4167653d33303b20457870697265733d4672692c2030362d4d61792d323031362031393a35303a313220474d543b20506174683d2f3b205365637572650d0a5365742d436f6f6b69653a207765622e69643d45552d32613738633631322d663932322d346562642d383065642d6432656330373335636537333b20446f6d61696e3d626174746c652e6e65743b20457870697265733d5765642c2032342d4d61792d323038342032333a30333a343920474d543b20506174683d2f3b205365637572653b20487474704f6e6c790d0a436f6e74656e742d4c656e6774683a203430330d0a566172793a204163636570742d456e636f64696e670d0a436f6e74656e742d547970653a206170706c69636174696f6e2f6a736f6e3b636861727365743d5554462d380d0a0d0a";

                        byteData += "7b2274797065223a20224c4f47494e5f464f524d222c22696e70757473223a205b7b22696e7075745f6964223a20226163636f756e745f6e616d65222c2274797065223a202274657874222c226c6162656c223a2022426174746c652e6e657420656d61696c222c226d61785f6c656e677468223a203332307d2c7b22696e7075745f6964223a202270617373776f7264222c2274797065223a202270617373776f7264222c226c6162656c223a202250617373776f7264222c226d61785f6c656e677468223a2031367d2c7b22696e7075745f6964223a20226c6f675f696e5f7375626d6974222c2274797065223a20227375626d6974222c226c6162656c223a20224c6f6720696e20746f20426174746c652e6e6574227d5d2c227372705f75726c223a202268747470733a2f2f65752e626174746c652e6e65742f6c6f67696e2f737270222c227372705f6a73223a202268747470733a2f2f65752e626174746c652e6e65742f6c6f67696e2f7374617469632f6a732f6c6f67696e2f7372702d636c69656e742e6d696e2e6a73227d";
                        var pos = 0;
                        var bytes = new byte[byteData.Length / 2];
                        for (var i = pos; i < byteData.Length; i += 2)
                            bytes[i / 2] = Convert.ToByte(byteData.Substring(i, 2), 16);

                        sslStream.Write(bytes);
                    }
                    else if (wut == 1)
                    {
                        // Sniffed data
                        var byteData = "485454502f312e3120323030204f4b0d0a446174653a2053756e2c203038204d617920323031362031393a33373a333220474d540d0a5365727665723a204170616368650d0a52657472792d41667465723a203630300d0a436f6e74656e742d4c656e6774683a2038360d0a566172793a204163636570742d456e636f64696e670d0a436f6e74656e742d547970653a206170706c69636174696f6e2f6a736f6e3b636861727365743d7574662d380d0a0d0a";

                        byteData += "7b2261757468656e7469636174696f6e5f7374617465223a2022444f4e45222c226c6f67696e5f7469636b6574223a202245552d3330653164656465343963373838643461633265663730643236323933383738227d";
                        var pos = 0;
                        var bytes = new byte[byteData.Length / 2];
                        for (var i = pos; i < byteData.Length; i += 2)
                            bytes[i / 2] = Convert.ToByte(byteData.Substring(i, 2), 16);

                        sslStream.Write(bytes);
                    }

                    wut++;*/
                    //PacketLog.Write<AuthClientMessage>(0, temp, client.RemoteEndPoint);
                } while (ctr != 0);
            }
            catch (Exception)
            {
                //Console.WriteLine(ex.Message);
                client?.Close();
                client?.Dispose();
                Dispose();
            }
            //client.ReceiveAsync(socketEventargs);

        }


        bool App_CertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {

            return true;
        }

        public void Send(byte[] rawdata) => sslStream.Write(rawdata);
        uint token = 0;
        public void Send(IMessage message)
        {
            try
            {
                var pData = message.ToByteArray();
                var header = new Header();

                // Server answer

                header.Token = 0;// token++;
                header.ServiceId = 0xFE;
                header.Size = (uint)pData.Length;


                var pHeader = header.ToByteArray();
                var dataToSend = new byte[2];

                dataToSend[0] = ((byte)((pHeader.Length >> 8)));
                dataToSend[1] = ((byte)((pHeader.Length & 0xff)));

                dataToSend = dataToSend.Concat(pHeader).Concat(pData).ToArray();

                if (dataToSend.Length != (2 + pHeader.Length + pData.Length))
                    Log.Message(LogType.Error, $"Wrong data length for '{message.ToString()}';");
                else
                    sslStream.Write(dataToSend);

                //PacketLog.Write<AuthServerMessage>(0, dataToSend, client.RemoteEndPoint);
            }
            catch (SocketException ex)
            {
                Log.Message(LogType.Error, "{0}", ex.Message);

                client.Close();
            }
        }

        public void Send(IMessage message, uint serviceHash, uint methodId)
        {
            try
            {
                var pData = message.ToByteArray();
                var header = new Header();

                // Server answer

                header.Token = token++;
                header.ServiceId = 0;
                header.ServiceHash = serviceHash;
                header.MethodId = methodId;
                header.Size = (uint)pData.Length;


                var pHeader = header.ToByteArray();
                var dataToSend = new byte[2];

                dataToSend[0] = ((byte)((pHeader.Length >> 8)));
                dataToSend[1] = ((byte)((pHeader.Length & 0xff)));

                dataToSend = dataToSend.Concat(pHeader).Concat(pData).ToArray();

                if (dataToSend.Length != (2 + pHeader.Length + pData.Length))
                    Log.Message(LogType.Error, $"Wrong data length for '{message.ToString()}';");
                else
                    sslStream.Write(dataToSend);

                //PacketLog.Write<AuthServerMessage>(0, dataToSend, client.RemoteEndPoint);
            }
            catch (SocketException ex)
            {
                Log.Message(LogType.Error, "{0}", ex.Message);

                client.Close();
            }
        }

        void SendCompleted(object sender, SocketAsyncEventArgs e)
        {
        }

        public void Dispose()
        {
            client = null;
            Account = null;
            SecureRemotePassword = null;
        }
    }
}
