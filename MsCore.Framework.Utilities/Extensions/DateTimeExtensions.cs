using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsCore.Framework.Utilities.Extensions
{
    using System;
    using System.Globalization;

    namespace MsCore.Framework.Utilities.Extensions
    {
        /// <summary>
        /// DateTime için tarih ve zaman işlemlerini kolaylaştıran yardımcı extension metodları.
        /// </summary>
        public static class DateTimeExtensions
        {
            /// <summary>
            /// DateTime değerini Unix Timestamp'e (saniye) dönüştürür.
            /// </summary>
            public static long MsToUnixTimeStamp(this DateTime date)
            {
                return new DateTimeOffset(date).ToUnixTimeSeconds();
            }

            /// <summary>
            /// Unix Timestamp değerinden DateTime'a dönüştürür.
            /// </summary>
            public static DateTime MsFromUnixTimeStamp(this long timestamp)
            {
                return DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
            }

            /// <summary>
            /// Tarihin hafta sonu olup olmadığını kontrol eder.
            /// </summary>
            public static bool MsIsWeekend(this DateTime date)
            {
                return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
            }

            /// <summary>
            /// Tarihin iş günü olup olmadığını kontrol eder.
            /// </summary>
            public static bool MsIsWorkingDay(this DateTime date)
            {
                return !date.MsIsWeekend();
            }

            /// <summary>
            /// Tarihin Türkçe gün adını getirir. Örnek: "Pazartesi"
            /// </summary>
            public static string MsGetTurkishDayName(this DateTime date)
            {
                return date.ToString("dddd", new CultureInfo("tr-TR"));
            }

            /// <summary>
            /// Tarihin Türkçe ay adını getirir. Örnek: "Temmuz"
            /// </summary>
            public static string MsGetTurkishMonthName(this DateTime date)
            {
                return date.ToString("MMMM", new CultureInfo("tr-TR"));
            }

            /// <summary>
            /// Tarihi Türkçe tarih formatında string'e dönüştürür.
            /// Örnek: 01 Ocak 2024
            /// </summary>
            public static string MsToTurkishDateString(this DateTime date)
            {
                return date.ToString("dd MMMM yyyy", new CultureInfo("tr-TR"));
            }

            /// <summary>
            /// Tarihi Türkçe kısa formatta döner.
            /// Örnek: "01.01.2024"
            /// </summary>
            public static string MsToTurkishShortDateString(this DateTime date)
            {
                return date.ToString("dd.MM.yyyy", new CultureInfo("tr-TR"));
            }

            /// <summary>
            /// Tarihi Türkçe tarih ve saat formatında döner.
            /// Örnek: "01 Ocak 2024 15:30"
            /// </summary>
            public static string MsToTurkishDateTimeString(this DateTime date)
            {
                return date.ToString("dd MMMM yyyy HH:mm", new CultureInfo("tr-TR"));
            }

            /// <summary>
            /// Tarihi Türkçe kısa tarih ve saat formatında döner.
            /// Örnek: "01.01.2024 15:30"
            /// </summary>
            public static string MsToTurkishShortDateTimeString(this DateTime date)
            {
                return date.ToString("dd.MM.yyyy HH:mm", new CultureInfo("tr-TR"));
            }

            /// <summary>
            /// Tarihi insan dostu bir formatta string'e dönüştürür.
            /// Örnek: "2 gün önce", "3 saat önce", "Şimdi"
            /// </summary>
            public static string MsToFriendlyTime(this DateTime date)
            {
                var ts = DateTime.Now - date;

                if (ts.TotalSeconds < 60)
                    return "Şimdi";

                if (ts.TotalMinutes < 60)
                    return $"{(int)ts.TotalMinutes} dakika önce";

                if (ts.TotalHours < 24)
                    return $"{(int)ts.TotalHours} saat önce";

                if (ts.TotalDays < 7)
                    return $"{(int)ts.TotalDays} gün önce";

                return date.ToString("dd.MM.yyyy");
            }

            /// <summary>
            /// Tarihi ISO 8601 formatında string'e dönüştürür. Örnek: "2024-07-01T12:00:00Z"
            /// </summary>
            /// <param name="date"></param>
            /// <returns></returns>
            public static string MsToIso8601String(this DateTime date)
            {
                return date.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            }

            /// <summary>
            /// Belirtilen tarihe iş günü ekler (Cumartesi ve Pazar hariç).
            /// </summary>
            public static DateTime MsAddWorkingDays(this DateTime date, int days)
            {
                if (days == 0) return date;

                int addedDays = 0;
                var newDate = date;

                while (addedDays < days)
                {
                    newDate = newDate.AddDays(1);

                    if (newDate.MsIsWorkingDay())
                        addedDays++;
                }

                return newDate;
            }

            /// <summary>
            /// Tarihten yaş hesaplar.
            /// </summary>
            public static int MsGetAge(this DateTime birthDate)
            {
                var today = DateTime.Today;
                var age = today.Year - birthDate.Year;
                if (birthDate > today.AddYears(-age)) age--;
                return age;
            }

            /// <summary>
            /// Tarihin gün başlangıcını döner (saat: 00:00:00).
            /// </summary>
            public static DateTime MsStartOfDay(this DateTime date)
            {
                return date.Date;
            }

            /// <summary>
            /// Tarihin gün sonunu döner (saat: 23:59:59.999).
            /// </summary>
            public static DateTime MsEndOfDay(this DateTime date)
            {
                return date.Date.AddDays(1).AddMilliseconds(-1);
            }

            /// <summary>
            /// İki tarih arasındaki toplam gün sayısını döner.
            /// </summary>
            public static int MsDaysBetween(this DateTime start, DateTime end)
            {
                return (end.Date - start.Date).Days;
            }

            /// <summary>
            /// Tarihin ayın ilk günü olup olmadığını kontrol eder.
            /// </summary>
            public static bool MsIsFirstDayOfMonth(this DateTime date)
            {
                return date.Day == 1;
            }

            /// <summary>
            /// Tarihin ayın son günü olup olmadığını kontrol eder.
            /// </summary>
            public static bool MsIsLastDayOfMonth(this DateTime date)
            {
                return date.Day == DateTime.DaysInMonth(date.Year, date.Month);
            }

            /// <summary>
            /// Tarihi SQL uyumlu formatta string'e dönüştürür. Örnek: "2024-07-01"
            /// </summary>
            public static string MsToSqlDate(this DateTime date)
            {
                return date.ToString("yyyy-MM-dd");
            }

            /// <summary>
            /// Tarihi SQL tarih saat formatında döner. Örnek: "2024-07-01 23:59:59"
            /// </summary>
            public static string MsToSqlDateTime(this DateTime date)
            {
                return date.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
    }

}
