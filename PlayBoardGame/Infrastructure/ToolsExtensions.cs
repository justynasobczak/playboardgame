using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

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
        
        public static SelectListItem[] GetTimeZones()
        {
            var tzs = TimeZoneInfo.GetSystemTimeZones();
            return tzs.Select(tz => new SelectListItem
            {
                Text = tz.DisplayName,
                Value = tz.Id
            }).OrderBy(tz => tz.Value).ToArray();
        }
        
        public static TimeZoneInfo ConvertTimeZone(string userTimeZone, ILogger logger)
        {
            TimeZoneInfo timeZone;
            try
            {
                timeZone = TimeZoneInfo.FindSystemTimeZoneById(userTimeZone);

            }
            catch (TimeZoneNotFoundException)
            {
                logger.LogError("Unable to find the {0} zone in the registry.", userTimeZone);
                timeZone = null;
            }
            
            catch (InvalidTimeZoneException)
            {
                logger.LogError("Registry data on the {0} zone has been corrupted.", userTimeZone);
                timeZone = null;
            }
            return timeZone;
        }
    }
}