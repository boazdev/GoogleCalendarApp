using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Google.Apis.Auth.OAuth2;
using GoogleCalendarApp.Services;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using System.Diagnostics;
using Newtonsoft.Json;
using GoogleCalendarApp.Models.Responses;
using System.Web.Mvc;
using GoogleCalendarApp.Models.Dto;
namespace GoogleCalendarApp.Pages
{
    /// <summary>
    /// Code-behind for the Calendar page. Handles Google OAuth authentication,
    /// token exchange, event retrieval, and session management.
    /// </summary>
    public partial class Calendar : System.Web.UI.Page
    {
        /// <summary>
        /// Handles the Page Load event. If a Google OAuth code is present,
        /// exchanges it for an access token and stores it in session.
        /// Otherwise, attempts to fetch calendar events using the existing session token.
        /// </summary>
        /// <param name="sender">The source of the page load event.</param>
        /// <param name="e">Event data associated with the page load.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            string code = Request.QueryString["code"];

            if (!IsPostBack)
            {
                txtStartDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

                if (!string.IsNullOrEmpty(code))
                {
                    string clientId = GoogleApiConfig.Instance.ClientId;
                    string clientSecret = GoogleApiConfig.Instance.ClientSecret;
                    string redirectUri = GoogleApiConfig.Instance.RedirectUri;
                    var token = ExchangeCodeForToken(code, clientId, clientSecret, redirectUri);
                    Session["access_token"] = token.AccessToken;
                    Response.Redirect("~/Pages/Calendar.aspx");
                    return;
                }

                if (Session["access_token"] != null)
                {
                    LoadCalendar(DateTime.Now, "month");
                }
                else
                {
                    Response.Redirect("~/Pages/Login.aspx");
                }
            }
            else if (Session["access_token"] == null)
            {
                    Response.Redirect("~/Pages/Login.aspx");
            }
        }
        /// <summary>
        /// Exchanges an authorization code for an access token by calling the Google OAuth2 token endpoint.
        /// </summary>
        /// <param name="code">The authorization code returned by Google after user consent.</param>
        /// <param name="clientId">The application's client ID.</param>
        /// <param name="clientSecret">The application's client secret.</param>
        /// <param name="redirectUri">The redirect URI registered with the Google API Console.</param>
        /// <returns>A <see cref="TokenResponse"/> object containing the access token and related information.</returns>
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

        /// <summary>
        /// Retrieves upcoming calendar events for the authenticated user based on the specified date and view mode.
        /// </summary>
        /// <param name="accessToken">The OAuth2 access token used to authorize API requests.</param>
        /// <param name="startDate">The start date for retrieving events.</param>
        /// <param name="viewMode">The view mode, either "week" or "month", which determines the time range.</param>
        /// <returns>A list of <see cref="Google.Apis.Calendar.v3.Data.Event"/> items.</returns>

        private IList<Google.Apis.Calendar.v3.Data.Event> GetUpcomingEvents(string accessToken, DateTime startDate, string viewMode)
        {
            try
            {
                var credential = GoogleCredential.FromAccessToken(accessToken);

                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "GoogleCalendarApp"
                });

                var request = service.Events.List("primary");
                request.TimeMinDateTimeOffset = new DateTimeOffset(startDate);
                request.ShowDeleted = false;
                request.SingleEvents = true;

                if (viewMode == "week")
                    request.TimeMaxDateTimeOffset = new DateTimeOffset(startDate.AddDays(7));
                else if (viewMode == "month")
                    request.TimeMaxDateTimeOffset = new DateTimeOffset(startDate.AddMonths(1));

                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
                return request.Execute().Items;
            }
            catch (Exception e)
            {
                Response.Redirect("~/Pages/Login.aspx");
                return new List<Google.Apis.Calendar.v3.Data.Event>();
            }
        }

        /// <summary>
        /// Handles the Filter button click event. Fetches and displays calendar events
        /// based on the selected start date and view mode.
        /// </summary>
        /// <param name="sender">The source of the event (Filter button).</param>
        /// <param name="e">Event data associated with the button click.</param>
        protected void btnFilter_Click(object sender, EventArgs e)
        {
            if (Session["access_token"] == null)
            {
                Response.Redirect("~/Pages/Login.aspx");
                return;
            }

            DateTime.TryParse(txtStartDate.Text, out var startDate);
            string viewMode = ddlViewMode.SelectedValue;
            LoadCalendar(startDate, viewMode);
        }

        /// <summary>
        /// Transforms a list of Google Calendar events into a simplified list of DTOs
        /// containing formatted start time and summary.
        /// </summary>
        /// <param name="events">The list of Google Calendar events to transform.</param>
        /// <returns>
        /// A list of <see cref="CalendarEventDto"/> containing human-readable start times and summaries.
        /// </returns>
        private List<CalendarEventDto> MapEventsToDto(IEnumerable<Google.Apis.Calendar.v3.Data.Event> events)
        {
            return events.Select(ev => new CalendarEventDto
            {
                Start = ev.Start.DateTimeDateTimeOffset.HasValue
                    ? ev.Start.DateTimeDateTimeOffset.Value.ToString("g")
                    : ev.Start.Date,
                Summary = ev.Summary
            }).ToList();
        }

        private void LoadCalendar(DateTime startDate, string viewMode)
        {
            var events = GetUpcomingEvents(Session["access_token"].ToString(), startDate, viewMode);
            var data = MapEventsToDto(events);
            gvEvents.DataSource = data;
            gvEvents.DataBind();
        }

        /// <summary>
        /// Handles the Logout button click event. Clears the user session and abandons it.
        /// </summary>
        /// <param name="sender">The source of the event (Logout button).</param>
        /// <param name="e">Event data associated with the button click.</param>
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            Response.Redirect("~/Pages/Login.aspx");
        }
    }
}