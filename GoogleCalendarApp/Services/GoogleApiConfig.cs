using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace GoogleCalendarApp.Services
{
    /// <summary>
    /// Provides configuration values for Google API authentication using a singleton pattern.
    /// Loads client credentials from a JSON file and redirect URI from web.config.
    /// </summary>
    public class GoogleApiConfig
    {
        /// <summary>
        /// Lazy-initialized singleton instance of the <see cref="GoogleApiConfig"/> class.
        /// </summary>
        private static readonly Lazy<GoogleApiConfig> _instance = new Lazy<GoogleApiConfig>(() => new GoogleApiConfig());
        public string ClientId { get; private set; }
        public string ClientSecret { get; private set; }

        public string RedirectUri { get; private set; }

        /// <summary>
        /// Private constructor to prevent external instantiation.
        /// Loads the Google API credentials from a local JSON file and the redirect URI from config.
        /// </summary>
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

        /// <summary>
        /// Gets the singleton instance of the <see cref="GoogleApiConfig"/> class.
        /// </summary>
        public static GoogleApiConfig Instance => _instance.Value;
    }
}