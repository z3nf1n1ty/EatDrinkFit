// Project: EatDrinkFit.Web
// File: Helpers/TimezoneHelper.cs
// Origonially designed for ASP.NET Core 8.0

// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

// Copyright (C) 2024 Michael Peterson <14036481+z3nf1n1ty@users.noreply.github.com>

// SPDX-FileCopyrightText: 2024 Michael Peterson <14036481+z3nf1n1ty@users.noreply.github.com>
// SPDX-License-Identifier: Mozilla Public License 2.0
// FileContributor: Original contributer Michael Peterson 14036481+z3nf1n1ty@users.noreply.github.com
// FileContributor:

using TimeZoneConverter;

namespace EatDrinkFit.Web.Helpers
{
    public static class TimezoneHelper
    {
        public static string GetBrowserReportedTimezone(HttpRequest request)
        {
            string? userTimezone = request.Cookies["userTimezone"];

            if (userTimezone is null)
            {
                userTimezone = @"Etc/UTC";
            }

            return userTimezone;
        }

        /// <summary>
        /// Replace a provided default, aka min, DateTime object with the current local time specifed by the IANA timezone.
        /// </summary>
        /// <param name="source"></param>
        /// <returns>A DateTime ojbect with the current local time specifed by the IANA timezone.</returns>
        public static DateTime ProcessDefaultDateTime(DateTime source, string targetTimeZoneIANA)
        {
            // Should now work with both Windows and IANA timezones.

            var defaultDateTime = new DateTime();

            if (source == defaultDateTime)
            {
                //var tzi = TimeZoneInfo.FindSystemTimeZoneById(targetTimeZoneIANA);

                //source = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tzi);

                source = ConvertFromUTC_IANA(DateTime.UtcNow, targetTimeZoneIANA);
            }

            return source;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unclearDate"></param>
        /// <returns></returns>
        public static bool IsDaylightSavingsTime(DateTime unclearDate)
        {
            bool isDST = false;

            // Report time as DST if it is either ambiguous or DST.
            if (TimeZoneInfo.Local.IsAmbiguousTime(unclearDate) || TimeZoneInfo.Local.IsDaylightSavingTime(unclearDate))
            {
                isDST = true;
            }

            return isDST;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceTimeZoneIANA"></param>
        /// <returns></returns>
        public static DateTime ConvertToUTC_IANA(DateTime source, string sourceTimeZoneIANA)
        {
            // Should now work with both Windows and IANA timezones.

            // Get Windows aka .NET formatted timezone string.
            //string tzStr = TZConvert.IanaToWindows(sourceTimeZoneIANA);
            //System.TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(tzStr);

            System.TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(sourceTimeZoneIANA);

            DateTime result = System.TimeZoneInfo.ConvertTimeToUtc(source, tzi);
            //System.TimeZoneInfo.ConvertTime()

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targetTimeZoneIANA"></param>
        /// <returns></returns>
        public static DateTime ConvertFromUTC_IANA(DateTime source, string targetTimeZoneIANA)
        {
            // Should now work with both Windows and IANA timezones.

            // Get Windows aka .NET formatted timezone string.
            //string tzStr = TZConvert.IanaToWindows(targetTimeZoneIANA);
            //System.TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(tzStr);

            System.TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(targetTimeZoneIANA);

            DateTime result = System.TimeZoneInfo.ConvertTimeFromUtc(source, tzi);

            return result;
        }
    }
}
