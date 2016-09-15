using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.UI.Helpers.CronScheduler
{
    public class AwsScheduler : Scheduler
    {
        const char SPACE = ' ';
        const string ASTERISK = "*";
        const string QUESTIONMARK = "?";
        
        const int MINUTE = 0;
        const int HOUR = 1;
        const int DAY_OF_MONTH = 2;
        const int MONTH = 3;
        const int DAY_OF_WEEK = 4;
        const int PERIOD_LENGTH = 5;

        public override string Standardize(string schedule)
        {
            schedule = HandleWeekOrMonthBase(schedule);
            schedule = AddYear(schedule);
            schedule = AddCron(schedule);
            return schedule;
        }

        private string AddCron(string schedule)
        {
            return "cron(" + schedule + ")";
        }

        private string AddYear(string schedule)
        {
            List<string> periods = new List<string>(schedule.Split(SPACE));
            periods.Add(ASTERISK);
            return string.Join(SPACE.ToString(), periods);
        }

        private string HandleWeekOrMonthBase(string schedule)
        {
            string[] periods = schedule.Split(SPACE);

            if (periods.Length != PERIOD_LENGTH)
            {
                throw new DuradosException("Expecting 5 periods in schedule");
            }

            if (periods[DAY_OF_WEEK] == ASTERISK && periods[DAY_OF_MONTH] == ASTERISK)
            {
                periods[DAY_OF_WEEK] = QUESTIONMARK;
            }
            else if (periods[DAY_OF_WEEK] != ASTERISK && periods[DAY_OF_MONTH] != ASTERISK)
            {
                throw new DuradosException("Base your schedule either on the Days of the Months or the Days of the Week, not on both.");
            }
            else
            {
                if (periods[DAY_OF_WEEK] == ASTERISK)
                {
                    periods[DAY_OF_MONTH] = QUESTIONMARK;
                }
                else
                {
                    periods[DAY_OF_WEEK] = QUESTIONMARK;
                }
            }
            return string.Join(SPACE.ToString(), periods);
        }
    }
}
