using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Diva2.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static Dictionary<int, string> days = new Dictionary<int, string>();

        public static string GetDayShrot(int key) {

            if (days.Count == 0) {
                days.Add(1, "po");
                days.Add(2, "út");
                days.Add(3, "st");
                days.Add(4, "čt");
                days.Add(5, "pá");
                days.Add(6, "so");
                days.Add(7, "ne");
            }

            return days[key];
            
        }

        
        public static int GetWeekNumber(this DateTime date, CultureInfo culture = null)
        {
            /*
            if(culture==null)
            {
                culture = CultureInfo.CurrentCulture;
            }
            int weekNum = culture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            /**/
            if (culture == null)
            {
                culture = CultureInfo.CurrentCulture;
            }
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(date);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                date = date.AddDays(3);
            }

            // Return the week of our adjusted day
            int weekNum = culture.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            return weekNum;
        }

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }


        public static DateTime FromUnix(int unix) {

            System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddSeconds(unix);
            return dateTime;            
        }

        
        public static int ToUnix(DateTime dt)
        {
            int unixTimestamp = (Int32)(dt.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            return unixTimestamp;
        } /**/

        public static int ToUnixWithoutMinutes(DateTime dt)
        {
            DateTime dtNew = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);

            int unixTimestamp = (Int32)(dtNew.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            return unixTimestamp;


        } /**/

        public static int ZbyvaDni(DateTime dt, DateTime dtn)
        {
            int dtu = ToUnix(dt);
            int dtun = ToUnix(dtn);
            int zbyva = (dtun - dtu ) / (60 * 60 * 24);
            return zbyva;
        }

        public static int ZbyvaDni(DateTime dt)
        {
            int now = ToUnix(DateTime.Now);
            int dtu = ToUnix(dt);
            int zbyva = (dtu - now) / (60 * 60 * 24);
            return zbyva;
        }

        public static int ZbyvaDni(int unix)
        {
            int now = ToUnix(DateTime.Now);
            int zbyva = (unix - now) / (60 * 60 * 24);
            return zbyva;
        }

        public static int CalculatePrice(int count, int priceDay, int priceHalf, DateTime from, DateTime to, out int hours)
        {

            int rest = 0;

            hours = (int)(to - from).TotalHours;
            decimal days = Math.Ceiling(((decimal)hours / (decimal)24));
            int hodFrom = from.Hour;

            if (hodFrom <= 12)
            { // pujceno dopoledne
                if (hours <= 5)
                { // cena za pulden
                    if (priceHalf == 0)
                    {
                        priceHalf = priceDay;
                    }
                    rest = (int)((decimal)count * (decimal)priceHalf);
                }
                else if (hours > 5 && hours <= 20)
                { // cena za den 
                    rest = (int)((decimal)count * (decimal)priceDay * (decimal)1);
                }
                else if (hours > 20)
                { // cena za den * pocet dni
                    rest = (int)((decimal)count * (decimal)priceDay * days);
                }
            }
            else
            { // pujceno odpoledne
                if (hours <= 20)
                { // cena za pulden
                    if (hodFrom > 14)
                    { // pujceno odpoledne
                      //rest = count * priceDay;
                        rest = (int)((decimal)count * (decimal)priceDay);
                    }
                    else
                    {
                        if (priceHalf == 0)
                        {
                            priceHalf = priceDay;
                        }
                        //rest = count * priceHalf;
                        rest = (int)((decimal)count * (decimal)priceHalf);
                    }
                }
                else if (hours > 20 && hours <= 27)
                { // cena za den 
                  //rest = count * priceDay * 1;
                    rest = (int)((decimal)count * (decimal)priceDay * (decimal)1);
                }
                else if (hours > 27)
                { // cena za den * pocet dni
                    //rest = count * priceDay * (int)Math.Ceiling(days);
                    rest = (int)((decimal)count * (decimal)priceDay * days);
                }
            }
            return rest;
        }
    }
}
