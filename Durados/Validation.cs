using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

namespace Durados
{
    public partial class Validation
    {
        public Validation()
        {
            RequiredMessage = "Required Field";
            NoMatchMessage = "No Match";
            HasMin = HasMax = false;
        }

        private Field field;

        internal void SetField(Field field) //init by Field
        {
            this.field = field;
        }

        public int Min { get { if (field != null) return field.Min; return 0; } }  //Min Value|Chars

        public int Max { get { if (field != null) return field.Max; return 0; } }  //Max Value|Chars

        public string Format { get { if (field != null) return field.Format; return string.Empty; } }

        //[Durados.Config.Attributes.ColumnProperty()]
        public string RequiredMessage { get; set; }

        //[Durados.Config.Attributes.ColumnProperty()]
        public string NoMatchMessage { get; set; }

        public string OutOfRangeMessage
        {
            get
            {
                if (HasRange())
                {
                    string vType = GetValidationType();

                    if (vType == "integer" || vType == "real")
                        return "{0}-{1}";
                    else if (Min != 0 && Max != 0)
                        return "Limited between {0} and {1}";
                }

                if (HasMin && Min != 0)
                {
                    return "Limited to {0}";
                }
                else if (HasMax && Max != 0)
                {
                    return "Limited upto {0}";
                }

                return string.Empty;
            }
        }


        //private string outOfRangeMessage;
        public bool HasMin { get; set; }
        public bool HasMax { get; set; }

        public bool HasRange() { return HasMin && HasMax && (Min != 0 || Max != 0); }

        public bool HasLimit() { return (HasMin && Min != 0) || (HasMax && Max != 0); }

        public string GetOutOfRangeMessage(string msg)
        {
            try
            {
                if (HasRange())
                {
                    string vType = GetValidationType();

                    if (vType == "integer" || vType == "real")
                        return string.Format(msg, Min, Max);
                    else if (Min != 0 && Max != 0)
                        return string.Format(msg, Min, Max);
                }

                if (HasMin && Min != 0)
                {
                    return string.Format(msg, Min);
                }
                else if (Max != 0)
                {
                    return string.Format(msg, Max);
                }
            }
            catch (Exception)
            {
                //TODO - Log or throw exception
            }

            return string.Format("Out of Range ({0}-{1})", Min, Max);
        }

        public int GetMinForJson()
        {
            if (HasMin)
                return Min;
            else
                return 0;
        }

        public int GetMaxForJson()
        {
            if (HasMax)
                return Max;
            else
                return 0;
        }


        //private string invalidFormatMessage;

        public string GetInvalidFormatMessage()
        {
            if (field == null || field.FieldType != FieldType.Column)
                return string.Empty;


            switch (field.GetColumnFieldType())
            {
                case ColumnFieldType.DateTime:
                    return field.Format;
                case ColumnFieldType.Integer:
                    return "Numeric";
                case ColumnFieldType.Real:
                    return "Numeric";
                default:
                    string vType = GetValidationType();
                    if (!string.IsNullOrEmpty(Format))
                        return "Invalid Format: " + Format;
                    else if (vType != string.Empty && vType != "none")
                        return "Invalid Format: " + System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(vType.Replace('_', ' '));
                    return string.Empty;
            }
        }

        public virtual string GetValidationType()
        {
            if (field != null)
            {
                ColumnFieldType FieldType = field.GetColumnFieldType();

                if (FieldType == ColumnFieldType.Integer || field.DisplayFormat == DisplayFormat.GeneralNumeric)
                {
                    return "integer";
                }
                else if (FieldType == ColumnFieldType.Real)
                {
                    return "real";
                }
                else if (FieldType == ColumnFieldType.DateTime)
                {
                    return "date";
                }
                else if (FieldType == ColumnFieldType.String)
                {
                    if (field.SpecialColumn == SpecialColumn.Email)
                        return "email";
                    else if (field.SpecialColumn == SpecialColumn.SSN)
                        return "social_security_number";
                    else if (field.SpecialColumn == SpecialColumn.Phone)
                        return "phone_number";
                    else if (field.SpecialColumn == SpecialColumn.Currency)
                        return "currency";
                    else if (field.SpecialColumn == SpecialColumn.Custom)
                        return "custom";
                }
            }
            return "none";
        }

    }

    public enum DateType
    {
        Date,
        DateTime,
        Time
    }

    public class DateFormatsMapper
    {
        private static List<DatesFormatStrings> legalDateFormats = new List<DatesFormatStrings>();
        private static List<DatesFormatStrings> legalTimeFormats = new List<DatesFormatStrings>();

        private static void InitLegalDateFormats()
        {
            legalDateFormats.Add(new DatesFormatStrings("mm/dd/yy", "MM/dd/yyyy", "MM/dd/yyyy"));
            legalDateFormats.Add(new DatesFormatStrings("dd/mm/yy", "dd/MM/yyyy", "dd/MM/yyyy"));
            legalDateFormats.Add(new DatesFormatStrings("mm-dd-yy", "MM-dd-yy", "MM-dd-yy"));
            legalDateFormats.Add(new DatesFormatStrings("dd-mm-yy", "dd-MM-yy", "dd-MM-yy"));
            legalDateFormats.Add(new DatesFormatStrings("M dd, yy", "MMM dd, yyyy", "MMM dd, yyyy"));
        }

        private static void InitLegalTimeFormats()
        {
            legalTimeFormats.Add(new DatesFormatStrings("hh:mm:ss", "hh:mm:ss", "hh:mm:ss"));
            legalTimeFormats.Add(new DatesFormatStrings("hh:mm:ss tt", "hh:mm:ss tt", "hh:mm:ss tt"));
            legalTimeFormats.Add(new DatesFormatStrings("h:mm:ss", "h:mm:ss", "h:mm:ss"));
            legalTimeFormats.Add(new DatesFormatStrings("h:mm:ss tt", "h:mm:ss tt", "h:mm:ss tt"));
            legalTimeFormats.Add(new DatesFormatStrings("hh:mm", "hh:mm", "hh:mm"));
            legalTimeFormats.Add(new DatesFormatStrings("hh:mm tt", "hh:mm tt", "hh:mm tt"));
            legalTimeFormats.Add(new DatesFormatStrings("h:mm", "h:mm", "h:mm"));
            legalTimeFormats.Add(new DatesFormatStrings("h:mm tt", "h:mm tt", "h:mm tt"));
        }

        public static DateType GetDateType(string format)
        {
            DateType dateType = DateType.Date;
            bool formatIncludesDate = getLegalDateFormats().Any(_format => format.StartsWith(_format.Csharp));
            bool formatIncludesTime = getLegalTimeFormats().Any(_format => format.EndsWith(_format.Csharp));

            if (formatIncludesDate)
            {
                if (formatIncludesTime)
                {
                    dateType = DateType.DateTime;
                }
            }
            else if (formatIncludesTime)
            {
                dateType = DateType.Time;
            }
            
            return dateType;
        }

        public static List<DatesFormatStrings> getLegalTimeFormats()
        {
            if (legalTimeFormats.Count == 0)
            {
                InitLegalTimeFormats();
            }

            return legalTimeFormats;
        }

        public static List<DatesFormatStrings> getLegalDateFormats()
        {
            if (legalDateFormats.Count == 0)
            {
                InitLegalDateFormats();
            }

            return legalDateFormats;
        }

        private static IList<DatesFormatStrings> getLegalFormats(bool isTimeFormat)
        {
            return isTimeFormat ? getLegalDateFormats() : getLegalDateFormats();
        }

        public static string getDefaultCsharpFormat(bool isTimeFormat = false)
        {
            IList<DatesFormatStrings> formats = getLegalFormats(isTimeFormat);
           
            return formats[0].Csharp;
        }

        public static string getDefaultSpryFormat(bool isTimeFormat = false)
        {
            IList<DatesFormatStrings> formats = getLegalFormats(isTimeFormat);

            return formats[0].Spry;
        }

        public static string getDefaultJQueryFormat(bool isTimeFormat = false)
        {
            IList<DatesFormatStrings> formats = getLegalFormats(isTimeFormat);

            return formats[0].JQuery;
        }

        public static string getSpryDateFormat(string cs, bool isTimeFormat = false)
        {
            IList<DatesFormatStrings> formats = getLegalFormats(isTimeFormat);

            foreach (DatesFormatStrings format in formats)
            {
                if (format.Csharp == cs)
                {
                    return format.Spry;
                }
            }

            return getDefaultJQueryFormat(isTimeFormat);
        }

        public static string getJQueryDateFormat(string cs, bool isTimeFormat = false)
        {
            IList<DatesFormatStrings> formats = getLegalFormats(isTimeFormat);

            foreach (DatesFormatStrings format in formats)
            {
                if (format.Csharp == cs)
                {
                    return format.JQuery;
                }
            }

            return getDefaultSpryFormat(isTimeFormat);
        }

        public static DateTime GetDateFromClient(string date, string dateFormat)
        {

            System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
            try
            {
                return DateTime.ParseExact(date, dateFormat, provider);
            }
            catch
            {
                try
                {
                    return DateTime.Parse(date);
                }
                catch (Exception)
                {
                    try
                    {
                        return DateTime.ParseExact(date.Replace("GMT", "UTC"), "ddd MMM dd yyyy HH:mm:ss UTCzzzzz", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception exception)
                    {
                        throw new DuradosException("Please provide a valid date in the format: " + dateFormat, exception);
                    }
                }
            }
        }

        public static DateTime GetDateFromClient(string date, string dateFormat, string timeZoneKey)
        {
            //string date = "2009-02-25 16:13:00Z"; // Coordinated Universal Time string from DateTime.Now.ToUniversalTime().ToString("u");
            //DateTime localDateTime = DateTime.Parse(date); // Local .NET timeZone.
            DateTime localDateTime = GetDateFromClient(date, dateFormat); // Local .NET timeZone.
            DateTime utcDateTime = localDateTime.ToUniversalTime();

            // ID from "HKEY_LOCAL_MACHINE\Software\Microsoft\Windows NT\CurrentVersion\Time Zone"
            // See http://msdn.microsoft.com/en-us/library/system.timezoneinfo.id.aspx
            //string nzTimeZoneKey = "New Zealand Standard Time";
            //TimeZoneInfo nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById(nzTimeZoneKey);
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneKey);
            DateTime dateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZone);

            return dateTime;
        }

    }

    [DataContract]
    public class DatesFormatStrings
    {
        public DatesFormatStrings(string jquery, string spry, string csharp) 
        {
            this.JQuery = jquery;
            this.Spry = spry;
            this.Csharp = csharp;            
        }

        [DataMember]
        public string Spry { get; set; }

        [DataMember]
        public string JQuery { get; set; }

        [DataMember]
        public string Csharp { get; set; }

    }

    //[DataContract]
    //public static class DatesFormtsDictionary 
    //{
    //    [DataMember]
    //    public List<DatesFormatStrings> formats = DateFormatsMapper.getLegalDateFormats();
    //}



}
