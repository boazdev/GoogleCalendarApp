using GoogleCalendarApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GoogleCalendarApp.Pages
{
    /*
    Create an ASP.NET application to:

    Authenticate users with their Google accounts using the Google API for .NET.
    Retrieve event data from Google Calendar API (date, time, subject, etc.).
    Display events in a table or list format using HTML, C#, and ASP.NET Webforms.
    Provide documentation for all code, including explanations of functions, variables, and algorithms.
    */

    /*
     * https://www.youtube.com/watch?v=w6rzVKBsB3A&list=PLlaJNuOIC_9--9vwfGBR8XF2zymdqD-Sk
     */
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// Handles login redirect to Google OAuth
        /// </summary>
        /// <param name="sender">The button clicked</param>
        /// <param name="e">Event arguments</param>
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string clientId = GoogleApiConfig.Instance.ClientId;
            string redirectUri = GoogleApiConfig.Instance.RedirectUri;
            string authUrl = "https://accounts.google.com/o/oauth2/v2/auth" +
            "?scope=" + HttpUtility.UrlEncode("https://www.googleapis.com/auth/calendar https://www.googleapis.com/auth/calendar.events") +
            "&access_type=online" +
            "&include_granted_scopes=true" +
            "&response_type=code" +
            "&redirect_uri=" + HttpUtility.UrlEncode(redirectUri) +
            "&client_id=" + clientId +
            "&prompt=consent";
            Response.Redirect(authUrl);
        }
    }
}