using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            }).ToArray();
        }
    }
}