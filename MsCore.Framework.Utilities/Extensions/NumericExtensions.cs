using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsCore.Framework.Utilities.Extensions
{
    public static class NumericExtensions
    {
        /// <summary>
        /// Sayı çift mi?
        /// </summary>
        public static bool MsIsEven(this int value)
        {
            return value % 2 == 0;
        }


        /// <summary>
        /// Sayı tek mi?
        /// </summary>
        public static bool MsIsOdd(this int value)
        {
            return value % 2 != 0;
        }

        /// <summary>
        /// Ondalık değeri yüzde string formatına dönüştürür.
        /// Örnek: 0.25 → %25
        /// </summary>
        public static string MsToPercentage(this double value, int decimalPlaces = 2)
        {
            return (value * 100).ToString($"N{decimalPlaces}", CultureInfo.CurrentCulture) + " %";
        }

        /// <summary>
        /// Ondalık değeri para birimi formatına dönüştürür.
        /// Örnek: 1250.5 → 1.250,50 ₺
        /// </summary>
        public static string MsToCurrency(this decimal value, string currencySymbol = "₺")
        {
            return string.Format(CultureInfo.CurrentCulture, "{0:N}", value) + " " + currencySymbol;
        }

        public static string MsToCurrency(this decimal value, string currencySymbol = "₺", string culture = "")
        {
            var cultureInfo = string.IsNullOrEmpty(culture)
                ? CultureInfo.CurrentCulture
                : new CultureInfo(culture);

            var numberFormat = (NumberFormatInfo)cultureInfo.NumberFormat.Clone();
            numberFormat.CurrencySymbol = currencySymbol;

            return value.ToString("C", numberFormat);
        }

        public static string MsToCurrency(this double value, string currencySymbol = "₺")
        {
            return string.Format(CultureInfo.CurrentCulture, "{0:N}", value) + " " + currencySymbol;
        }

        public static string MsToCurrency(this double value, string currencySymbol = "₺", string culture = "")
        {
            var cultureInfo = string.IsNullOrEmpty(culture)
                ? CultureInfo.CurrentCulture
                : new CultureInfo(culture);

            var numberFormat = (NumberFormatInfo)cultureInfo.NumberFormat.Clone();
            numberFormat.CurrencySymbol = currencySymbol;

            return value.ToString("C", numberFormat);
        }

        public static string MsToCurrency(this float value, string currencySymbol = "₺")
        {
            return string.Format(CultureInfo.CurrentCulture, "{0:N}", value) + " " + currencySymbol;
        }

        public static string MsToCurrency(this float value, string currencySymbol = "₺", string culture = "")
        {
            var cultureInfo = string.IsNullOrEmpty(culture)
                ? CultureInfo.CurrentCulture
                : new CultureInfo(culture);

            var numberFormat = (NumberFormatInfo)cultureInfo.NumberFormat.Clone();
            numberFormat.CurrencySymbol = currencySymbol;

            return value.ToString("C", numberFormat);
        }

        /// <summary>
        /// Sayıyı binlik ayracı ve ondalık basamak ile string formatına dönüştürür.
        /// Örnek: 1234567.89 → "1.234.567,89"
        /// </summary>
        public static string MsToFormattedString(this decimal value, int decimalPlaces = 2)
        {
            return value.ToString($"N{decimalPlaces}", CultureInfo.CurrentCulture);
        }

        public static string MsToFormattedString(this int value, int decimalPlaces = 2)
        {
            return value.ToString($"N{decimalPlaces}", CultureInfo.CurrentCulture);
        }

        public static string MsToFormattedString(this double value, int decimalPlaces = 2)
        {
            return value.ToString($"N{decimalPlaces}", CultureInfo.CurrentCulture);
        }

        public static string MsToFormattedString(this float value, int decimalPlaces = 2)
        {
            return value.ToString($"N{decimalPlaces}", CultureInfo.CurrentCulture);
        }

        public static string MsToFormattedString(this long value, int decimalPlaces = 2)
        {
            return value.ToString($"N{decimalPlaces}", CultureInfo.CurrentCulture);
        }


        /// <summary>
        /// Sayıyı Türkçe yazıya çevirir.
        /// Örnek: 1250 → "Bin İki Yüz Elli"
        /// </summary>
        public static string MsToWords(this long number)
        {
            if (number == 0) return "Sıfır";

            string[] units = { "", "Bir", "İki", "Üç", "Dört", "Beş", "Altı", "Yedi", "Sekiz", "Dokuz" };
            string[] tens = { "", "On", "Yirmi", "Otuz", "Kırk", "Elli", "Altmış", "Yetmiş", "Seksen", "Doksan" };
            string[] thousands = { "", "Bin", "Milyon", "Milyar", "Trilyon" };

            string words = "";
            int thousandCounter = 0;

            while (number > 0)
            {
                var n = (int)(number % 1000);
                if (n != 0)
                {
                    string segment = "";

                    var hundreds = n / 100;
                    var tensUnit = n % 100;
                    var ten = tensUnit / 10;
                    var unit = tensUnit % 10;

                    if (hundreds != 0)
                    {
                        segment += (hundreds == 1 ? "Yüz" : units[hundreds] + " Yüz") + " ";
                    }

                    if (ten != 0)
                    {
                        segment += tens[ten] + " ";
                    }

                    if (unit != 0)
                    {
                        segment += units[unit] + " ";
                    }

                    if (thousandCounter != 0)
                    {
                        if (!(n == 1 && thousandCounter == 1)) // "Bir Bin" → "Bin"
                            segment += thousands[thousandCounter] + " ";
                        else
                            segment += "Bin ";
                    }

                    words = segment + words;
                }

                number /= 1000;
                thousandCounter++;
            }

            return words.Trim();
        }

        /// <summary>
        /// Sayıyı Türkçe yazıya çevirir.
        /// Örnek: 1250 → "Bin İki Yüz Elli"
        /// </summary>
        public static string MsToWords(this int number)
        {
            if (number == 0) return "Sıfır";

            string[] units = { "", "Bir", "İki", "Üç", "Dört", "Beş", "Altı", "Yedi", "Sekiz", "Dokuz" };
            string[] tens = { "", "On", "Yirmi", "Otuz", "Kırk", "Elli", "Altmış", "Yetmiş", "Seksen", "Doksan" };
            string[] thousands = { "", "Bin", "Milyon", "Milyar", "Trilyon" };

            string words = "";
            int thousandCounter = 0;

            while (number > 0)
            {
                var n = (number % 1000);
                if (n != 0)
                {
                    string segment = "";

                    var hundreds = n / 100;
                    var tensUnit = n % 100;
                    var ten = tensUnit / 10;
                    var unit = tensUnit % 10;

                    if (hundreds != 0)
                    {
                        segment += (hundreds == 1 ? "Yüz" : units[hundreds] + " Yüz") + " ";
                    }

                    if (ten != 0)
                    {
                        segment += tens[ten] + " ";
                    }

                    if (unit != 0)
                    {
                        segment += units[unit] + " ";
                    }

                    if (thousandCounter != 0)
                    {
                        if (!(n == 1 && thousandCounter == 1)) // "Bir Bin" → "Bin"
                            segment += thousands[thousandCounter] + " ";
                        else
                            segment += "Bin ";
                    }

                    words = segment + words;
                }

                number /= 1000;
                thousandCounter++;
            }

            return words.Trim();
        }
    }
}
