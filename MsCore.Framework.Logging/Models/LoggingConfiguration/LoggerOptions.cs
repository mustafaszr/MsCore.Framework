using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsCore.Framework.Logging.Models.LoggingConfiguration
{
    /// <summary>
    /// MsCore Logger için yapılandırma ayarlarını sağlar.
    /// Hangi log sağlayıcılarının (dosya, veritabanı) aktif olacağı ve
    /// dosya loglama için klasör, dosya adı, boyut gibi seçenekleri içerir.
    /// </summary>
    public class LoggerOptions
    {
        /// <summary>
        /// Dosya tabanlı loglamayı aktif eder veya devre dışı bırakır.
        /// </summary>
        public bool UseFileLogger { get; set; }
        /// <summary>
        /// Veritabanı tabanlı loglamayı aktif eder veya devre dışı bırakır.
        /// </summary>
        public bool UseDatabaseLogger { get; set; }
        /// <summary>
        /// Log dosyalarının kaydedileceği klasör yoludur.
        /// Bu alan UseFileLogger true olduğunda zorunludur.
        /// Varsayılan değer 'Logs' olarak atanır.
        /// </summary>
        public string DirectoryPath { get; set; } = "Logs";
        /// <summary>
        /// Log dosyasının adıdır (uzantısız).
        /// Varsayılan değer 'log' olarak atanır.
        /// </summary>
        public string FileName { get; set; } = "log";
        /// <summary>
        /// Boyut bazlı log döngüsü için maksimum dosya boyutu (MB cinsinden).
        /// Varsayılan 5 MB'dır.
        /// </summary>
        public long MaxFileSizeInMB { get; set; } = 5;
        /// <summary>
        /// Log dosyalarının nasıl döngüye gireceğini belirler.
        /// Günlük, haftalık veya boyut bazlı seçenekler.
        /// </summary>
        public RotationType RotationType { get; set; } = RotationType.Daily;
    }
}
