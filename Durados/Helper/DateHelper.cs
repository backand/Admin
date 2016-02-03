using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

using System.Linq;
using Durados;
using System.IO;
using System.Security.Cryptography;


namespace System
{
    public static class DateHelper
    {
        public const int DaysInWeek = 7;

        public static int GetWeekNumberUS(this DateTime source)
        {
            CultureInfo cultureInfo = new CultureInfo("en-US");
            return cultureInfo.Calendar.GetWeekOfYear(source,
                                  CalendarWeekRule.FirstDay,
                                  DayOfWeek.Monday);
        }

        public static int GetWeekNumber(this DateTime source, CultureInfo cultureInfo, DayOfWeek startOfWeek)
        {
            return cultureInfo.Calendar.GetWeekOfYear(source,
                                  CalendarWeekRule.FirstDay,
                                  startOfWeek);
        }

        public static DateTime StartOfWeekUS(this DateTime source)
        {
            return StartOfWeek(source, DayOfWeek.Monday);
        }

        public static DateTime StartOfWeek(this DateTime source, DayOfWeek startOfWeek)
        {
            int diff = source.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return source.AddDays(-1 * diff).Date;
        }


    }
}
