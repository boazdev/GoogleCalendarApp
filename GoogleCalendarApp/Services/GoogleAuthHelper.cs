using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoogleCalendarApp.Services
{
    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Util.Store;
    using System.IO;
    using System.Threading;
    using System.Web;

    public static class GoogleAuthHelper
    {
        public static UserCredential GetUserCredential()
        {
            string[] scopes = { Google.Apis.Calendar.v3.CalendarService.Scope.CalendarReadonly };
            string credPath = HttpContext.Current.Server.MapPath("~/Credentials/token.json");

            using (var stream = new FileStream(
                HttpContext.Current.Server.MapPath("~/Credentials/client_secret.json"),
                FileMode.Open, FileAccess.Read))
            {
                return GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }
        }
    }
}