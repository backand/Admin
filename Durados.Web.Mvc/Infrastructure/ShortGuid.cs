using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Web.Mvc.Infrastructure
{
    public static class ShortGuid
    {
        private static int current;
        private static DayOfWeek day;

        static ShortGuid()
        {
            current = 0;
            day = DateTime.Today.DayOfWeek;
        }

        public static string Next()
        {
            DayOfWeek dayOfWeek = DateTime.Today.DayOfWeek;

            if (day != dayOfWeek)
                current = 0;
            
            current++;
            day = dayOfWeek;
            

            return GetTodayLetter(dayOfWeek) + current.ToString();
        }

        private static string GetTodayLetter(DayOfWeek dayOfWeek)
        {
            
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return "a";
                case DayOfWeek.Monday:
                    return "b";
                case DayOfWeek.Tuesday:
                    return "c";
                case DayOfWeek.Wednesday:
                    return "d";
                case DayOfWeek.Thursday:
                    return "e";
                case DayOfWeek.Friday:
                    return "f";
                case DayOfWeek.Saturday:
                    return "g";

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
