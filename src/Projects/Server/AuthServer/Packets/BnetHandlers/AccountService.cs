using AuthServer.AuthServer.Attributes;
using AuthServer.Network;
using Bgs.Protocol.Account.V1;
using Framework.Constants.Net;

namespace AuthServer.AuthServer.Packets.BnetHandlers
{
    [BnetService(BnetServiceHash.AccountService)]
    public class AccountService : BnetServiceBase
    {
        [BnetMethod(30)]
        public static void HandleGetAccountStateRequest(AuthSession session, GetAccountStateRequest getAccountStateRequest)
        {
            //Console.WriteLine($"GetAccountStateRequest: {getAccountStateRequest.ToString()}");

            var getAccountStateResponse = new GetAccountStateResponse();

            // Data from sniffs
            getAccountStateResponse.State = new AccountState
            {
                PrivacyInfo = new PrivacyInfo
                {
                    IsHiddenFromFriendFinder = true,
                    IsUsingRid = true
                }
            };

            getAccountStateResponse.Tags = new AccountFieldTags
            {
                PrivacyInfoTag = 0xD7CA834D
            };

            session.Send(getAccountStateResponse);
        }

        [BnetMethod(31)]
        public static void HandleGetGameAccountStateRequest(AuthSession session, GetGameAccountStateRequest getGameAccountStateRequest)
        {
            //Console.WriteLine($"GetGameAccountStateRequest: {getGameAccountStateRequest.ToString()}");

            var getGameAccountStateResponse = new GetGameAccountStateResponse();

            getGameAccountStateResponse.State = new GameAccountState
            {
                GameLevelInfo = new GameLevelInfo
                {
                    Program = 5730135,
                    Name = "WoW1", 
                },

                GameStatus = new GameStatus
                {
                    Program = 5730135,
                },
            };

            getGameAccountStateResponse.State.GameLevelInfo.Licenses.Add(new AccountLicense { Id = 250 });

            getGameAccountStateResponse.Tags = new GameAccountFieldTags
            {
                GameLevelInfoTag = 4140539163,
                GameStatusTag = 2562154393
            };

            session.Send(getGameAccountStateResponse);
        }
    }

}
