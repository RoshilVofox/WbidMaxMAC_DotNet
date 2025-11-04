using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WBid.WBidiPad.PortableLibrary.Utility
{
   public static class Helper
    {
        public static DateTime ConvertToHerbFromUTC(DateTime utcTime)
        {
            utcTime = DateTime.SpecifyKind(utcTime, DateTimeKind.Utc);

            // Get US/Central TimeZone (Windows ID)
            TimeZoneInfo centralZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");

            // Convert to US/Central Time (handles DST automatically)
            DateTime centralTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, centralZone);

            return centralTime;
        }

        public static string ConvertRawHhmmtoHhColonMm(string hhmm)
       {
           string hh = hhmm.Substring(0, hhmm.Length - 2);
           string mm = hhmm.Substring(hhmm.Length - 2, 2);
           return hh + ":" + mm;
       }

        public static int ConvertMinutesToHhhmmInt(int minutes)
        {
            int hhh = (minutes / 60) * 100;
            int mm = minutes % 60;
            return hhh + mm;
        }

        public static string ConvertMinutesToHhhmm(int minutes)
       {
           string hhh = (minutes / 60).ToString();
           string mm = (minutes % 60).ToString();
           return hhh.PadLeft(2, '0') + ":" + mm.PadLeft(2, '0');
       }

       public static string ConvertMinutesToFormattedHour(int minutes)
       {
           string hhh = (minutes / 60).ToString("d2");
           string mm = (minutes % 60).ToString("d2");
           return hhh + ":" + mm;
       }
       public static int ConvertHhmmToMinutes(string hhmm)
       {

           hhmm = hhmm.PadLeft(5, '0');
           int hours = Convert.ToInt32(hhmm.Substring(0, 2));
           int minutes = Convert.ToInt32(hhmm.Substring(3, 2));
           return hours * 60 + minutes;
       }

        public static string CalcTimeFromMinutes(string tm)
        {
            string minutes = null;
            string hours = null;
            int mins = Convert.ToInt32(tm)
;
            int day = mins / (60 * 24);
            int days = mins % (60 * 24);
            int hrs = days / 60;
            int mns = days % 60;
            hrs = hrs + (day * 24);
            if (hrs < 10)
                hours = "0" + hrs.ToString();
            else
                hours = hrs.ToString();
            if (mns.ToString().Length == 1) minutes = "0" + mns.ToString();
            else minutes = mns.ToString();
            string displayTime = hours + ":" + minutes;
            return displayTime;
        }

        public static string CalcTimeFromMinutesFromMidnight(string tm)
       {
           string minutes = null;
           string hours = null;
           int mins = Convert.ToInt32(tm);
           int days = mins % (60 * 24);
           int hrs = days / 60;
           int mns = days % 60;
           if (hrs < 10) hours = "0" + hrs.ToString();
           else hours = hrs.ToString();
           if (mns.ToString().Length == 1) minutes = "0" + mns.ToString();
           else minutes = mns.ToString();
           string displayTime = hours + ":" + minutes;
           return displayTime;
       }

       public static string ConvertMinuteToHHMM(int minute)
       {
           string result = string.Empty;
           result = Convert.ToString(minute / 60).PadLeft(2, '0');
           result += ":";
           result += Convert.ToString(minute % 60).PadLeft(2, '0'); ;
           return result;

       }

       public static string TimeFromMinutes(string minutes)
       {
           string time = string.Empty;
            int hour = int.Parse(minutes) / 60;
           int minute = int.Parse(minutes) % 60;

           time = hour.ToString() + ":" + minute.ToString("d2");
           return time;
       }

       public static int ConvertHHMMtoMinute(string hhmm)
       {
           int minute = 0;
           if (hhmm != string.Empty)
           {
               string[] splitHhhmm = hhmm.Split(':');
               minute = int.Parse(splitHhhmm[0]) * 60 + int.Parse(splitHhhmm[1]);
           }
           return minute;
          
       }

       public static int ConvertformattedHhhmmToMinutes(string hhhmm)
       {
           hhhmm = hhhmm.Replace(":", "");
           hhhmm = hhhmm.PadLeft(5, '0');
           int hours = Convert.ToInt32(hhhmm.Substring(0, 3));
           int minutes = Convert.ToInt32(hhhmm.Substring(3, 2));
           return hours * 60 + minutes;


       }
       public static decimal ConvertHhhColonMmToFractionalHours(string hhmm)
       {
           string[] splitHhhmm;
           splitHhhmm = hhmm.Split(':');
           int hh = int.Parse(splitHhhmm[0]);
           int mm = int.Parse(splitHhhmm[1]);
           return hh + mm / 60m;
       }

        public static int ConvertToHerbFromDomTime(string domicile, DateTime dutPdDate, int domTime)
        {
            // TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time");
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            TimeSpan ts = new TimeSpan(12, 0, 0);

            // Insures that time dateTime is not 12:00AM but 12:00pm
            // Otherwise the day DST changes to DST would be wrong and the day it changes back to standard time would be wrong.

            dutPdDate = dutPdDate.Date + ts;
            bool isDst = timeZoneInfo.IsDaylightSavingTime(dutPdDate);
            // bool isDst = TimeZoneInfo.Local.IsDaylightSavingTime(date);

            switch (domicile)
            {
                case "ATL":
                    return domTime - 60;       // EST = herb - 60
                case "AUS":
                    return domTime;            // CST = herb 
                case "BWI":
                    return domTime - 60;       // EST = herb - 60
                case "BNA":
                    return domTime;            // CST = herb 
                case "DAL":
                    return domTime;            // CST = herb 
                case "DEN":
                    return domTime + 60;       // MST = herb + 60
                case "FLL":
                    return domTime - 60;       // EST = herb - 60
                case "HOU":
                    return domTime;            // CST = herb 
                case "LAS":
                    return domTime + 120;      // PST = herb + 120
                case "LAX":
                    return domTime + 120;      // PST = herb + 120
                case "MCO":
                    return domTime - 60;       // EST = herb - 60
                case "MDW":
                    return domTime;            // CST = herb 
                case "OAK":
                    return domTime + 120;      // PST = herb + 120
                case "PHX":
                    if (isDst)
                        return domTime + 120;  // PST = herb + 120
                    else
                        return domTime + 60;   // MST = herb + 60
                default:
                    return 1440;
            }
        }




    }
}
