using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using GoogleCalendarApp.Services;

public class GoogleCalendarService
{
    public static IList<Google.Apis.Calendar.v3.Data.Event> GetUpcomingEvents()
    {
        UserCredential credential = GoogleAuthHelper.GetUserCredential();

        var service = new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "GoogleCalendarApp",
        });

        EventsResource.ListRequest request = service.Events.List("primary");
        //request.TimeMin = DateTime.Now;
        request.TimeMinDateTimeOffset = DateTimeOffset.Now;
        request.ShowDeleted = false;
        request.SingleEvents = true;
        request.MaxResults = 10;
        request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

        var events = request.Execute().Items;
        return events;
    }
}