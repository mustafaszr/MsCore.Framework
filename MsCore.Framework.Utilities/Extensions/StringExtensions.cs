using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace MsCore.Framework.Utilities.Extensions
{
    /// <summary>
    /// String veri tipi için genel ve sık kullanılan yardımcı (extension) metotları içerir.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// String değerin null veya boş olup olmadığını kontrol eder.
        /// </summary>
        public static bool MsIsNullOrEmpty(this string? value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// String değerin null, boş veya sadece boşluklardan oluşup oluşmadığını kontrol eder.
        /// </summary>
        public static bool MsIsNullOrWhiteSpace(this string? value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// String ifadeyi baş harfleri büyük olacak şekilde başlık formatına dönüştürür.
        /// Örneğin: "merhaba dünya" → "Merhaba Dünya"
        /// </summary>
        public static string MsToTitleCase(this string input)
        {
            if (input.MsIsNullOrWhiteSpace()) return input;
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
        }

        /// <summary>
        /// String ifadeyi URL dostu bir slug formatına dönüştürür.
        /// Örneğin: "Merhaba Dünya!" → "merhaba-dunya"
        /// </summary>
        public static string MsToSlug(this string input)
        {
            if (input.MsIsNullOrWhiteSpace()) return string.Empty;

            var normalized = input.MsRemoveDiacritics().ToLowerInvariant();
            normalized = Regex.Replace(normalized, @"[^a-z0-9\s-]", "");
            normalized = Regex.Replace(normalized, @"\s+", " ").Trim();
            return normalized.Replace(" ", "-");
        }

        /// <summary>
        /// String içerisindeki Türkçe ve özel karakterleri İngilizce karşılıklarına dönüştürür.
        /// Örneğin: "Çalışma" → "Calisma"
        /// </summary>
        public static string MsRemoveDiacritics(this string input)
        {
            if (input.MsIsNullOrWhiteSpace()) return input;

            var normalizedString = input.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// String ifadeyi Base64 formatına dönüştürür.
        /// </summary>
        public static string MsToBase64(this string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Base64 formatındaki string ifadeyi orijinal haline decode eder.
        /// </summary>
        public static string MsFromBase64(this string input)
        {
            var bytes = Convert.FromBase64String(input);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// String ifadeyi belirtilen uzunlukta kısaltır.
        /// Uzunluk aşımı yoksa olduğu gibi döner.
        /// </summary>
        public static string MsTruncate(this string input, int length)
        {
            if (input.MsIsNullOrEmpty()) return input;
            return input.Length <= length ? input : input.Substring(0, length);
        }

        /// <summary>
        /// String ifadeyi belirtilen uzunlukta kısaltır ve sonuna "..." ekler.
        /// </summary>
        public static string MsTruncateWithEllipsis(this string input, int maxLength)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return input.Length <= maxLength ? input : input.Substring(0, maxLength) + "...";
        }

        /// <summary>
        /// String içerisindeki kelime sayısını döner.
        /// Örneğin: "Merhaba dünya" → 2
        /// </summary>
        public static int MsWordCount(this string input)
        {
            if (input.MsIsNullOrWhiteSpace()) return 0;
            return Regex.Matches(input, @"\b\w+\b").Count;
        }

        /// <summary>
        /// String içerisindeki HTML etiketlerini temizler.
        /// Örneğin: "&lt;div&gt;Test&lt;/div&gt;" → "Test"
        /// </summary>
        public static string MsRemoveHtmlTags(this string input)
        {
            if (input.MsIsNullOrWhiteSpace()) return input;
            return Regex.Replace(input, "<.*?>", string.Empty);
        }

        /// <summary>
        /// String ifadenin sayı (numeric) olup olmadığını kontrol eder.
        /// </summary>
        public static bool MsIsNumeric(this string input)
        {
            return double.TryParse(input, out _);
        }

        /// <summary>
        /// String ifadenin geçerli bir GUID olup olmadığını kontrol eder.
        /// </summary>
        public static bool MsIsGuid(this string input)
        {
            return Guid.TryParse(input, out _);
        }

        /// <summary>
        /// String ifadenin geçerli bir URL olup olmadığını kontrol eder.
        /// Sadece HTTP ve HTTPS URL'leri doğrular.
        /// </summary>
        public static bool MsIsValidUrl(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            return Uri.TryCreate(input, UriKind.Absolute, out Uri? uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        /// <summary>
        /// Türkçe karakterleri veritabanı ASCII uyumlu karakterlere dönüştürür.
        /// Örnek: Ş → Þ, İ → Ý
        /// </summary>
        public static string MsToAsciiForDatabase(this string? input)
        {
            if (string.IsNullOrEmpty(input)) return "";

            return input
                .Replace("Ş", "Þ")
                .Replace("İ", "Ý")
                .Replace("Ğ", "Ð")
                .Replace("ş", "þ")
                .Replace("ı", "ý")
                .Replace("ğ", "ð");
        }

        /// <summary>
        /// ASCII uyumlu karakterleri tekrar Türkçe karakterlere çevirir.
        /// Örnek: Þ → Ş, Ý → İ
        /// </summary>
        public static string MsToTurkishCharacters(this string? input)
        {
            if (string.IsNullOrEmpty(input)) return "";

            return input
                .Replace("Þ", "Ş")
                .Replace("Ý", "İ")
                .Replace("Ð", "Ğ")
                .Replace("þ", "ş")
                .Replace("ý", "ı")
                .Replace("ð", "ğ");
        }

        /// <summary>
        /// String içerisindeki değeri belirtilen karakter aralığında maskeleme işlemi yapar.
        /// Örnek: "1234567890".Mask(3, 4) → "123****890"
        /// </summary>
        /// <param name="input">Maskelenecek string</param>
        /// <param name="showFirst">Başta gösterilecek karakter sayısı</param>
        /// <param name="showLast">Sonda gösterilecek karakter sayısı</param>
        /// <param name="maskChar">Maskeleme karakteri. (Varsayılan '*')</param>
        /// <returns>Maskelenmiş string</returns>
        public static string MsMask(this string input, int showFirst = 2, int showLast = 2, char maskChar = '*')
        {
            if (string.IsNullOrEmpty(input)) return input;

            int inputLength = input.Length;

            if (showFirst + showLast >= inputLength)
                return new string(maskChar, inputLength); // Tamamen maskelenir

            var firstPart = input.Substring(0, showFirst);
            var lastPart = input.Substring(inputLength - showLast);
            var maskedPart = new string(maskChar, inputLength - showFirst - showLast);

            return firstPart + maskedPart + lastPart;
        }

        /// <summary>
        /// E-posta adresinin kullanıcı adını maskele.
        /// Örnek: "john.doe@gmail.com" → "jo***@gmail.com"
        /// </summary>
        public static string MsMaskEmail(this string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
                return email;

            var parts = email.Split('@');
            var userPart = parts[0].MsMask(2, 0);
            return userPart + "@" + parts[1];
        }

        /// <summary>
        /// String'in sadece orta kısmını maskele.
        /// Örnek: "1234567890" → "123****890"
        /// </summary>
        public static string MsMaskMiddle(this string input, int maskLength = 4, char maskChar = '*')
        {
            if (string.IsNullOrEmpty(input)) return input;
            if (input.Length <= maskLength) return new string(maskChar, input.Length);

            int start = (input.Length - maskLength) / 2;
            return input.Substring(0, start)
                   + new string(maskChar, maskLength)
                   + input.Substring(start + maskLength);
        }
    }
}
