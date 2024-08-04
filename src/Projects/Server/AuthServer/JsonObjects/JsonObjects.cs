using System.Collections.Generic;

namespace AuthServer.AuthServer.JsonObjects
{
    #region General definitions
    public class FormInputs
    {
        public string type { get; set; }

        public List<FormInput> inputs { get; set; }
    }

    public class FormInput
    {
        public string input_id { get; set; }

        public string type { get; set; }

        public string label { get; set; }

        public int max_length { get; set; }
    }

    public class FormInputValue
    {
        public string input_id { get; set; }

        public string value { get; set; }
    }
    #endregion

    #region Logon
    public class LogonForm
    {
        //public string this[string inputId] => inputs.SingleOrDefault(i => i.input_id == inputId)?.value;

        public string version { get; set; }

        public string program_id { get; set; }

        public string platform_id { get; set; }

        public List<FormInputValue> inputs { get; set; }
    }

    public static class AuthenticationState
    {
        public static string Done => "DONE";
        public static string Authenticator => "AUTHENTICATOR";
        public static string Legal => "LEGAL";
    }

    // Not finished
    public class PostLogonForm
    {
        public string authentication_state { get; set; }

        public string login_ticket { get; set; }
    }

    #endregion

    #region Realmlist
    public class RealmListTicketIdentity
    {
        public int gameAccountID { get; set; }

        public int gameAccountRegion { get; set; }
    }

    public class ClientVersion
    {
        public int versionMajor { get; set; }

        public int versionBuild { get; set; }

        public int versionMinor { get; set; }

        public int versionRevision { get; set; }
    }

    public class RealmListTicketInformation
    {
        public int platform { get; set; }

        public int currentTime { get; set; }

        public string buildVariant { get; set; }

        public string timeZone { get; set; }

        public int versionDataBuild { get; set; }

        public int audioLocale { get; set; }

        public ClientVersion version { get; set; }

        public List<int> secret { get; set; }

        public int type { get; set; }

        public int textLocale { get; set; }
    }

    public class RealmListTicketClientInformation
    {
        public RealmListTicketInformation info { get; set; }
    }

    public class Update
    {
        public int wowRealmAddress { get; set; }

        public int cfgTimezonesID { get; set; }

        public int populationState { get; set; }

        public int cfgCategoriesID { get; set; }

        public ClientVersion version { get; set; }

        public int cfgRealmsID { get; set; }

        public int flags { get; set; }

        public string name { get; set; }

        public int cfgConfigsID { get; set; }

        public int cfgLanguagesID { get; set; }
    }

    public class RealmListUpdate
    {
        public uint wowRealmAddress { get; set; }

        public Update update { get; set; }

        public bool deleting { get; set; }
    }

    public class RealmListUpdates
    {
        public IList<RealmListUpdate> updates { get; set; }
    }

    public class RealmEntry
    {
        public int wowRealmAddress { get; set; }

        public int cfgTimezonesID { get; set; }

        public int populationState { get; set; }

        public int cfgCategoriesID { get; set; }

        public ClientVersion version { get; set; }

        public int cfgRealmsID { get; set; }

        public int flags { get; set; }

        public string name { get; set; }

        public int cfgConfigsID { get; set; }

        public int cfgLanguagesID { get; set; }
    }

    public class RealmCharacterCountList
    {

        public IList<object> counts { get; set; }
    }

    public class Address
    {

        public string ip { get; set; }

        public int port { get; set; }
    }

    public class Family
    {

        public int family { get; set; }

        public IList<Address> addresses { get; set; }
    }

    public class RealmListServerIPAddresses
    {

        public IList<Family> families { get; set; }
    }


    #endregion
}
