using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;

namespace Durados.Web.Mvc.UI
{
    public class Gantt
    {
        private Map Map
        {
            get
            {
                return Maps.Instance.GetMap();
            }
        }

        private ColumnField field;

        private int totalGanttDays;

        private DateTime NADate = new DateTime(1930,1,1); 
       
        public DateTime LowerBoundDate { get; set; }

        public DateTime UpperBoundDate { get; set; }

        public string FilterText { get; set; }

        public Gantt()
        {
        }

        public void Init(ColumnField field, object filter)
        {
            if (this.field != null) return;

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(Map.Database.DefaultCulture);

            this.field = field;

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(Map.Database.DefaultCulture);

            //Bounds dates LowerBoundDate
            string filters = filter == null? "" : filter.ToString();

            string lowerDate = string.Empty;
            string upperDate = string.Empty;


            if (filters.Contains("between") && filters.Contains("to"))
            {
                    string[] words = System.Text.RegularExpressions.Regex.Split(filters.Trim(), "to");

                    lowerDate = words[0].Substring(7).Trim();
                    upperDate = words[1].Trim();

            }

            if (lowerDate == string.Empty || lowerDate == string.Empty) {

                LowerBoundDate = new DateTime(DateTime.Today.Year, 1, 1);

                UpperBoundDate = new DateTime(DateTime.Today.Year, 12, 31);

            } else {

                LowerBoundDate = Durados.DateFormatsMapper.GetDateFromClient(lowerDate, field.Format);
                UpperBoundDate = Durados.DateFormatsMapper.GetDateFromClient(upperDate, field.Format);
                LowerBoundDate = new DateTime(LowerBoundDate.Year, LowerBoundDate.Month, 1);

                DateTime firstDay = new DateTime(UpperBoundDate.Year, UpperBoundDate.Month, 1);

                UpperBoundDate = firstDay.AddMonths(1).AddDays(-1);

            }

            this.FilterText =  "between " + LowerBoundDate.ToString(field.Format) + " to " + UpperBoundDate.ToString(field.Format);
 
            totalGanttDays = 1 + new TimeSpan(UpperBoundDate.Ticks).Days - new TimeSpan(LowerBoundDate.Ticks).Days;  
        }

        public string GetHeadersRow()
        {
            return string.Format(GetGanttContainer(), GetScale(true));
        }

        public string GetScale(bool isHeader)
        {
            string ganttScale = "<div class='scaleContainer'>";

            string scaleTpl = "<div style='width:{0}px'>{1}</div>";

            string content = isHeader? LowerBoundDate.ToString("MMM-yy") : "&nbsp;";

            ganttScale += string.Format(scaleTpl, GetMonthScaleWidth(LowerBoundDate), content);

            DateTime date = LowerBoundDate.AddMonths(1);

            while (date <= UpperBoundDate)
            {
                content = isHeader? date.ToString("MMM-yy") : "&nbsp;";

                ganttScale += string.Format(scaleTpl, GetMonthScaleWidth(date), content);

                date = date.AddMonths(1);
            }

            ganttScale += "</div>";

            return ganttScale;
        }

        private int GetMonthScaleWidth(DateTime date)
        {
            DateTime firstDay = new DateTime(date.Year, date.Month, 1);

            DateTime lastDay = firstDay.AddMonths(1).AddDays(-1);

             //float width = ((float)ts.Days / (float)this.totalGanttDays) * 100;

            return (1 + new TimeSpan(lastDay.Ticks).Days - new TimeSpan(firstDay.Ticks).Days) * 2;
        }

        private int GetMilestoneWidth(DateTime fromDate, DateTime toDate)
        {            
            if (LowerBoundDate > toDate || fromDate > UpperBoundDate) return 0;

            if (fromDate < LowerBoundDate) fromDate = LowerBoundDate;

            if (toDate > UpperBoundDate) toDate = UpperBoundDate;

            //TimeSpan ts = new TimeSpan(toDate.Ticks - fromDate.Ticks);

            //float width = ((float)ts.Days / (float)this.totalGanttDays) * 100;

            int days = new TimeSpan(toDate.Ticks).Days - new TimeSpan(fromDate.Ticks).Days;

            if (toDate >= UpperBoundDate) 
            {
                days+=1;
            }

            return days * 2;
        }

        //Milestones

        public virtual string GetMilestonesRow(DataRow row)
        {
            return string.Format(GetGanttContainer(), GetScale(false) + GetMilestones(row));
        }

        private string GetGanttContainer()
        {
            return "<div class='ganttRowContainer' style='width: "+ (totalGanttDays*2).ToString() + "px'>{0}</div>";
        }

        public virtual string GetMilestones(DataRow row)
        {
           //GetMilestoneWidth
           string msRow = string.Empty;

           string commDelem = field.GetValue(row);

           if (commDelem == null)
                throw new DuradosException("Gantt is undefined");

           string[] s = commDelem.Split(',');

           int count=(s.Length / 2);

           string ganttTpl = "<div class='{2}' style='width:{0}px;{1}' title='{3}'></div>";          

            try
            {
                //List<DateTime?> dates = GetDates(field, fields, dataRow); //Get From Row

                List<string> fields = GetFields(field,s);

                List<DateTime?> allDates = GetDates(s);// Get From Cell Data

                List<DateTime> dates = GetValidDates(s);

                List<Color> colors = GetColors(field, s.Length/2);
                
                msRow = "<div class='milestoneContainer'>";

                int position = -11;

                int width = 0;

                //bool hasPad = false;
                
                if (dates.Count > 0) {
                    width = GetMilestoneWidth(LowerBoundDate, dates[0]);
                    if (width > 0) {
                        //width -= 2; // for begining of the day!
                        msRow += string.Format(ganttTpl, width, "", "milestonePad","");
                        position += width;
                        //hasPad = true;
                    }
                }
                

                int allIndex = -1;
                
                string msImages = string.Empty; 

                for (int i=0; i < dates.Count; i++)
                {
                    allIndex++;

                    string bg = string.Empty;
                    if (!field.Milestone.Custom)
                        bg = GetMilestoneBgColor(GetMilestoneColor(colors, i));
                    
                    if (i+1 < dates.Count)
                    {
                        string title = string.Empty;

                        if (allDates[allIndex] == null) {
                            title += " - First milestone is missing ";
                            bg = "background: rgb(0,0,0)";
                        }

                        if (allDates[allIndex+1] == null) 
                        {
                            bg = "background: rgb(0,0,0)";                            
                            while (allIndex+1 < allDates.Count && allDates[allIndex+1] == null)
                            {
                                title += " - Missing milestone";
                                allIndex++;
                            }
                        }                        

                        width = GetMilestoneWidth(dates[i], dates[i+1]);

                        if (width <= 0) continue;

                        if (i==0) 
                        {
                            //if (!hasPad) width -= 2; // for begining of the day!

                            if (dates[i] >= LowerBoundDate)
                            {
                                msImages += string.Format("<img src='/Content/Images/milestone.png' alt='' title='{0}' class='milestoneImg' style='left: " + position.ToString() + "px' />", field.View.Fields[fields[allIndex]].DisplayName + " - " + dates[i].ToString(field.Format));
                            }
                        }

                        position += width;   

                        if (dates[i+1] <=  UpperBoundDate)
                            msImages += string.Format("<img src='/Content/Images/milestone.png' alt='' title='{0}' class='milestoneImg' style='left: " + position.ToString() + "px' />", field.View.Fields[fields[allIndex + 1]].DisplayName + " - " + dates[i + 1].ToString(field.Format));

                        title = "From " + dates[i].ToString(field.Format) + " To " + dates[i+1].ToString(field.Format) + title;


                        msRow += string.Format(ganttTpl, width, bg, "milestone"+i.ToString(), title);
                    }                    
                    
                }

                msRow += "</div>" + msImages;            

            }
            catch (Exception ex)
            {
                throw new DuradosException("Gantt error: " + ex.Message);
            }
            return msRow;
        }

        //private GetNextMilestoneDate(List<DateTime> dates, int index)
        //{

        //}
        private string GetMilestoneBgColor(Color color)
        {
                if (color == null)
                {
                    return "background: rgb(0,0,0)";
                }
                else
                {
                    return "background: rgb(" + color.R.ToString() + "," + color.G.ToString() + "," + color.B.ToString() + ")";
                }
        }

        private List<DateTime?> GetDates(ColumnField field, List<string> fields, DataRow dataRow)
        {
            if (field == null)
            {
                throw new MissingFieldException("Field does not exists!");
            }
            else
            {
                List<DateTime?> dates = new List<DateTime?>();
                foreach (string fieldName in fields)
                {

                    if (!field.View.Fields.ContainsKey(fieldName))
                    {
                        dates.Add(null);
                    }
                    else
                    {
                        Durados.Field dateField = field.View.Fields[fieldName];

                        string strDate = dateField.GetValue(dataRow);

                        DateTime date = new DateTime();

                        bool isValidDate = DateTime.TryParse(strDate, out date);

                        if (isValidDate && date > NADate)
                        {
                            dates.Add(date);
                        }
                        else
                        {
                            dates.Add(null);
                        }
                    }
                }
                return dates;
            }
           
        }


        private List<DateTime?> GetDates(string[] s)
        {
            List<DateTime?> dates = new List<DateTime?>();
            for (int i = 1; i < s.Length ; i = i + 2)
            {
                DateTime date = new DateTime(); ;

                bool isValidDate = DateTime.TryParse(s[i], out date);                
                
                if (isValidDate && date > NADate)
                {
                    dates.Add(date);
                }
                else
                {
                    dates.Add(null);
                }
            }
            return dates;
        }

        
        private List<DateTime> GetValidDates(string[] s)
        {
            List<DateTime> dates = new List<DateTime>();
            for (int i = 1; i < s.Length ; i = i + 2)
            {
                DateTime date = new DateTime();

                bool isValidDate = DateTime.TryParse(s[i], out date);

                if (isValidDate && date > NADate) {
                    dates.Add(date);
                }
            }
            return dates;
        }

        private List<string> GetFields(ColumnField field,string[] s)
        {
            List<string> fields = new List<string>();
            for (int i = 0; i < s.Length - 1; i = i + 2)//i++
            {
                if (field.View.Fields.ContainsKey(s[i].ToString()))
                {
                    fields.Add(s[i].ToString());
                }
                else
                {
                    fields.Add(string.Empty);
                }
            }
            return fields;
        }

        private  Color GetMilestoneColor(List<Color> colors, int i)
        {
            if (colors != null && i < colors.Count && colors[i] != null)
                return colors[i];
            else
                return Color.FromArgb(0, 0, 0);
        }
  
        private List<Color> GetColors(ColumnField field, int steps)
        {
            return field.Milestone.GetGradients(steps).ToList<Color>();
        }

        private static Color GetDefaultMilestoneColor()
        {
            return Color.Black;
        }
      
    }

}