using AuthServer.AuthServer.Attributes;
using AuthServer.Network;
using Bgs.Protocol;
using Bgs.Protocol.Authentication.V1;
using Bgs.Protocol.Challenge.V1;
using Framework.Constants.Net;
using Framework.Misc;
using Google.Protobuf;

namespace AuthServer.AuthServer.Packets.BnetHandlers
{
    [BnetService(BnetServiceHash.AuthenticationServerService)]
    public class AuthenticationServerService : BnetServiceBase
    {
        public const string cred = "ARCTIUM-SANDBOX-1";
        [BnetMethod(1)]
        public static void HandleConnectRequest(AuthSession session, LogonRequest logonRequest)
        {
            System.Console.WriteLine($"LogonRequest: {logonRequest.ToString()}");
            session.ClientBuild = (uint)logonRequest.ApplicationVersion;
           // if (logonRequest.WebClientVerification == true)// && logonRequest.Email == "arctium@arctium")
            {
                //Console.WriteLine("Logon request with account 'arctium@arctium' and 'WebClientVerification' enabled.");

                var cur = new ChallengeExternalRequest();

                //cur.RequestToken = "asdasd";
                cur.PayloadType = "web_auth_url";
                cur.Payload = ByteString.CopyFromUtf8($"https://127.0.0.1:{Sandbox.RestPort}/login/?app=wow");

                session.Send(cur, 0xBBDA171Fu, 3);
            }

           
        }

        [BnetMethod(7)]
        public static void HandleVerifyWebCredentialsRequest(AuthSession session, VerifyWebCredentialsRequest verifyWebCredentialsRequest)
        {
            System.Console.WriteLine($"VerifyWebCredentialsRequest: {verifyWebCredentialsRequest.WebCredentials.ToStringUtf8()}");
            // login ticket from previous REST packet.
            if (verifyWebCredentialsRequest.WebCredentials.ToStringUtf8() == cred)
            {
                var logonResult = new LogonResult();
                //logonResult.Email = "arctium@arctium";
                logonResult.ErrorCode = 0x0; // ERROR_OK
                //logonResult.AvailableRegion.Add(0);

                logonResult.AccountId = new EntityId { High = 72057594037927936, Low = 1 };
                logonResult.GameAccountId.Add(new EntityId { High = 0x200000200576F57, Low = 1 });

                logonResult.SessionKey = ByteString.CopyFromUtf8(new byte[0].GenerateRandomKey(64).ToHexString());
                //Console.WriteLine($"VerifyWebCredentialsRequest: {verifyWebCredentialsRequest.ToString()}");

                session.Send(logonResult, 0x71240E35, 5);
            }
            else
            {
                var logonResult = new LogonResult();
                //logonResult.Email = "arctium@arctium";
                logonResult.ErrorCode = 0x3; // ERROR_

                session.Send(logonResult, 0x71240E35, 5);
                //session.Dispose();
            }
        }
    }

}
