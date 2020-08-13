using System.IO;
using System.Text;
using System.Windows;
using Microsoft.Identity.Client;


namespace Ag_Analytics_Toolbar.Azure_Active_Directory_B2C
{
    class App
    {
        // https://aganalytics.b2clogin.com/
        //aganalytics.onmicrosoft.com
        //b2c_1_siupin
        //oauth2
        //client_id=a571b617-4e90-42ab-9f70-af77c1f11894
        //https%3A%2F%2Fanalytics.ag%2Fdeveloper%2Fsignin-oidc

        private static readonly string Tenant = "fabrikamb2c.onmicrosoft.com";
        private static readonly string AzureAdB2CHostname = "fabrikamb2c.b2clogin.com";
        private static readonly string ClientId = "841e1190-d73a-450c-9d68-f5cf16b78e81";
        private static readonly string RedirectUri = "https://fabrikamb2c.b2clogin.com/oauth2/nativeclient";
        public static string PolicySignUpSignIn = "b2c_1_susi";
        public static string PolicyEditProfile = "b2c_1_edit_profile";
        public static string PolicyResetPassword = "b2c_1_reset";

        public static string[] ApiScopes = { "https://fabrikamb2c.onmicrosoft.com/helloapi/demo.read" };
        public static string ApiEndpoint = "https://fabrikamb2chello.azurewebsites.net/hello";
        private static string AuthorityBase = $"https://{AzureAdB2CHostname}/tfp/{Tenant}/";
        public static string AuthoritySignUpSignIn = $"{AuthorityBase}{PolicySignUpSignIn}";
        public static string AuthorityEditProfile = $"{AuthorityBase}{PolicyEditProfile}";
        public static string AuthorityResetPassword = $"{AuthorityBase}{PolicyResetPassword}";

        public static IPublicClientApplication PublicClientApp { get; private set; }

        static App()
        {
            PublicClientApp = PublicClientApplicationBuilder.Create(ClientId)
                .WithB2CAuthority(AuthoritySignUpSignIn)
                .WithRedirectUri(RedirectUri)
                .WithLogging(Log, LogLevel.Info, false) // don't log PII details on a regular basis
                .Build();

            TokenCacheHelper.Bind(PublicClientApp.UserTokenCache);
        }
        private static void Log(LogLevel level, string message, bool containsPii)
        {
            string logs = ($"{level} {message}");
            StringBuilder sb = new StringBuilder();
            sb.Append(logs);
            File.AppendAllText(System.Reflection.Assembly.GetExecutingAssembly().Location + ".msalLogs.txt", sb.ToString());
            sb.Clear();
        }
    }
}
