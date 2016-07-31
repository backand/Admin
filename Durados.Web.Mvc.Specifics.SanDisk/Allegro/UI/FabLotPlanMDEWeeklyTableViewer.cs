using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Data;
using System.Data.SqlClient;

using Durados.Web.Mvc.UI.Helpers;

namespace Durados.Web.Mvc.Specifics.SanDisk.Allegro.UI
{
    public class FabLotPlanMDEWeeklyTableViewer : FabLotPlanMDETableViewer
    {
        
        protected override DateTime? GetFirstPlanningMonth()
        {
            return base.GetFirstPlanningMonth().Value.StartOfWeekUS();
        }
        
        public override string GetDisplayName(Field field, DataRow row, string guid)
        {
            DateTime? firstPlanningMonth = GetFirstPlanningMonth();

            if (!firstPlanningMonth.HasValue)
                return field.DisplayName;

            int week = 0;
            bool success = int.TryParse(field.DisplayName.Remove(0, 1), out week);

            if (!success)
                return field.DisplayName;

            DateTime date = firstPlanningMonth.Value.Date.AddDays((week + 1) * DateHelper.DaysInWeek);


            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            CultureInfo provider = new System.Globalization.CultureInfo("en-US");

            return "WK" + (date.GetWeekNumberUS() - 1) + " " + date.ToString("MM/dd", provider);



        }




        
        
        protected override FabLotPlanLoader GetNewFabLotPlanLoader()
        {
            return new FabLotWeeklyPlanLoader();
        }

        protected override string GetPeriodPrefix()
        {
            return "w";
        }

        protected override int GetIndex(DateTime date, DateTime? firstPlanningMonth)
        {
            //DateTime lastDayOfTheMonth = GetLastDayOfTheMonth(date);
            return (new DateTime(date.Year, date.Month, 1)).AddMonths(2).StartOfWeekUS().Subtract(firstPlanningMonth.Value.StartOfWeekUS()).Days / 7 - 1;

            //return date.GetWeekNumberUS() - firstPlanningMonth.Value.GetWeekNumberUS();
        }


        protected override int GetFilterIndex(DateTime filterDate, DateTime? firstPlanningMonth)
        {
            return filterDate.StartOfWeekUS().Subtract(firstPlanningMonth.Value).Days / DateHelper.DaysInWeek;
        }


        private DateTime GetLastDayOfTheMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1).AddDays(-1);
        }

    }


    public class FabLotWeeklyPlanLoader : FabLotPlanLoader
    {
        const int numberOfWeeks = 12;



        //protected override SqlParameter GetFromParameter(FabLotPlanFilter filter)
        //{
        //    return new SqlParameter("FromDate", filter.Date.StartOfWeekUS());

        //}

        protected override SqlParameter GetToParameter(FabLotPlanFilter filter)
        {
            DateTime date = filter.Date.StartOfWeekUS();
            date = date.AddDays(DateHelper.DaysInWeek * numberOfWeeks);
            return new SqlParameter("ToDate", date);
        }

        protected override DateTime GetFirstPeriodDate(DateTime dateTime)
        {
            return dateTime.StartOfWeekUS();
        }
    }
}
