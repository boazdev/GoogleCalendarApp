using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Calendar.v3.Data;
using GoogleCalendarApp.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using System.Net.Http;
using System.Diagnostics;
using Newtonsoft.Json;
using GoogleCalendarApp.Models.Responses;
namespace GoogleCalendarApp.Pages
{
    public partial class Calendar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            /*if (!IsPostBack)
            {
                var events = GoogleCalendarService.GetUpcomingEvents();
                var data = events.Select(ev => new
                {
                    Start = ev.Start.DateTimeDateTimeOffset.HasValue ? ev.Start.DateTimeDateTimeOffset.Value.ToString("g") : ev.Start.Date,
                    Summary = ev.Summary
                }).ToList();

                gvEvents.DataSource = data;
                gvEvents.DataBind();
            }*/
            string code = Request.QueryString["code"];
            if (!string.IsNullOrEmpty(code))
            {
                string clientId = GoogleApiConfig.Instance.ClientId;
                string clientSecret = GoogleApiConfig.Instance.ClientSecret;
                string redirectUri = GoogleApiConfig.Instance.RedirectUri;

                //var token = ExchangeCodeForToken(code, clientId, clientSecret, redirectUri);
                // Session["access_token"] = token.AccessToken;
                Debug.Print("Redirect Uri: " + redirectUri);
                Debug.Print("Code: " + code);
                var token = ExchangeCodeForToken(code, clientId, clientSecret, redirectUri);
                Debug.Print($"access token : {token.access_token}, refresh token: {token.refresh_token}");
                Session["access_token"] = token.access_token;
                var events = GetUpcomingEvents(token.access_token);
                var data = events.Select(ev => new
                {
                    Start = ev.Start.DateTimeDateTimeOffset.HasValue
                        ? ev.Start.DateTimeDateTimeOffset.Value.ToString("g")
                        : ev.Start.Date,
                    Summary = ev.Summary
                }).ToList();

                gvEvents.DataSource = data;
                gvEvents.DataBind();
                // Display `events` or bind to a repeater/grid
            }
            else if (Session["access_token"] != null)
            {
                var events = GetUpcomingEvents(Session["access_token"].ToString());
                var data = events.Select(ev => new
                {
                    Start = ev.Start.DateTimeDateTimeOffset.HasValue
                        ? ev.Start.DateTimeDateTimeOffset.Value.ToString("g")
                        : ev.Start.Date,
                    Summary = ev.Summary
                }).ToList();

                gvEvents.DataSource = data;
                gvEvents.DataBind();
                // Display `events` or bind to a repeater/grid
            }
            else
            {
                Response.Redirect("~/Login.aspx");
            }
        }
        public static TokenResponse ExchangeCodeForToken(string code, string clientId, string clientSecret, string redirectUri)
        {
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    { "code", code },
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
                    { "redirect_uri", redirectUri },
                    { "grant_type", "authorization_code" }
                };

                var content = new FormUrlEncodedContent(values);

                var response = client.PostAsync("https://oauth2.googleapis.com/token", content).Result;
                var responseString = response.Content.ReadAsStringAsync().Result;
                Debug.Print("Raw token response: " + responseString);
                return JsonConvert.DeserializeObject<TokenResponse>(responseString);
            }
        }

        private IList<Google.Apis.Calendar.v3.Data.Event> GetUpcomingEvents(string accessToken)
        {
            var credential = GoogleCredential.FromAccessToken(accessToken);

            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "GoogleCalendarApp"
            });

            var request = service.Events.List("primary");
            request.TimeMinDateTimeOffset = DateTimeOffset.UtcNow;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            var events = request.Execute().Items;
            return events;
        }
    }
}