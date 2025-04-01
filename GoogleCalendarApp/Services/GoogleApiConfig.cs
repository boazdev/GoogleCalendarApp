using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace GoogleCalendarApp.Services
{
    public class GoogleApiConfig
    {
        private static readonly Lazy<GoogleApiConfig> _instance = new Lazy<GoogleApiConfig>(() => new GoogleApiConfig());

        public string ClientId { get; private set; }
        public string ClientSecret { get; private set; }

        public string RedirectUri { get; private set; }

        private GoogleApiConfig()
        {
            var path = HttpContext.Current.Server.MapPath("~/Credentials/client_secret.json");

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var secrets = GoogleClientSecrets.FromStream(stream).Secrets;
                ClientId = secrets.ClientId;
                ClientSecret = secrets.ClientSecret;
            }
            RedirectUri = ConfigurationManager.AppSettings["GoogleRedirectUri"];
        }

        public static GoogleApiConfig Instance => _instance.Value;
    }
}