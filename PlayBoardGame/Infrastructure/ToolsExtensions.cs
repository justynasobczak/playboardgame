using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.TimeZones;

namespace PlayBoardGame.Infrastructure
{
    public static class ToolsExtensions
    {
        public static T Random<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            // note: creating a Random instance each call may not be correct for you,
            // consider a thread-safe static instance
            var r = new Random();
            var list = enumerable as IList<T> ?? enumerable.ToList();
            return list.Count == 0 ? default(T) : list[r.Next(0, list.Count)];
        }

        /*public static SelectListItem[] GetTimeZones()
        {
            var tzs = TimeZoneInfo.GetSystemTimeZones();
            return tzs.Select(tz => new SelectListItem
            {
                Text = tz.DisplayName,
                Value = tz.Id
            }).OrderBy(tz => tz.Value).ToArray();
        }*/

        public static List<KeyValuePair<string, string>> GetTimeZones()
        {
            /*var tzs = TimeZoneInfo.GetSystemTimeZones();
            var timeZones = new List<KeyValuePair<string, string>>();
            foreach (var item in tzs)
            {
                timeZones.Add(new KeyValuePair<string, string>(item.DisplayName, item.Id));
            }

            return timeZones;*/
            var timeZones2 = new List<KeyValuePair<string, string>>();
            var provider = DateTimeZoneProviders.Tzdb;
            foreach (var id in provider.Ids)
            {
                var zone = provider[id];
                timeZones2.Add(new KeyValuePair<string, string>(zone.Id, zone.Id));
                // Use the zone 
            }
            
            /*var tzs = TimeZoneInfo.GetSystemTimeZones();
            var timeZones = new List<KeyValuePair<string, string>>();
            foreach (var item in tzs)
            {
                timeZones.Add(new KeyValuePair<string, string>(item.DisplayName, item.Id));
            }*/

            return timeZones2;
        }

        public static TimeZoneInfo ConvertTimeZone(string userTimeZone, ILogger logger)
        {
            /*TimeZoneInfo timeZone;
            var tzs = TimeZoneInfo.GetSystemTimeZones();
            try
            {
                timeZone = TimeZoneInfo.FindSystemTimeZoneById(userTimeZone);
            }
            catch (TimeZoneNotFoundException)
            {
                logger.LogError("Unable to find the {0} zone in the registry.", userTimeZone);
                timeZone = tzs.First();
            }

            catch (InvalidTimeZoneException)
            {
                logger.LogError("Registry data on the {0} zone has been corrupted.", userTimeZone);
                timeZone = tzs.First();
            }

            return timeZone;*/
            TimeZoneInfo timeZone;
            var timeZones = GetTimeZones();
            var userTz = timeZones.FirstOrDefault(id => id.Key == userTimeZone).Key;
            try
            {
                timeZone = TimeZoneInfo.FindSystemTimeZoneById(userTz);
            }
            catch (TimeZoneNotFoundException)
            {
                logger.LogError("Unable to find the {0} zone in the registry.", userTimeZone);
                timeZone = TimeZoneInfo.GetSystemTimeZones().First();
            }

            catch (InvalidTimeZoneException)
            {
                logger.LogError("Registry data on the {0} zone has been corrupted.", userTimeZone);
                timeZone = TimeZoneInfo.GetSystemTimeZones().First();
            }
            

            return timeZone;
        }

        public static bool IsDateInFuture(DateTime dateUTC)
        {
            return DateTime.UtcNow < dateUTC;
        }

        public static bool IsStartDateBeforeEndDate(DateTime startDateUTC, DateTime endDateUTC)
        {
            return startDateUTC < endDateUTC;
        }
    }
}