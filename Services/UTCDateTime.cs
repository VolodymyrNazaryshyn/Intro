using System;

namespace Intro.Services
{
    public class UTCDateTime : IDateTime
    {
        public string Date()
        {
            var date = DateTime.Now.ToString("dd.MM.yyyy");
            return date;
        }
        public string Time()
        {
            var time = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now).ToString("hh:mm:ss");
            return time;
        }
    }
}
