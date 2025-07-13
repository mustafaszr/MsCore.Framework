using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace MsCore.Framework.Utilities.Helpers
{
    /// <summary>
    /// Dosya ve klasör işlemleri için genel yardımcı sınıf.
    /// Okuma, yazma, kopyalama, taşıma, sıkıştırma ve MIME tipi belirleme gibi işlemleri sağlar.
    /// </summary>
    public static class MsFileHelper
    {
        /// <summary>
        /// Belirtilen dosya yolundaki tüm içeriği okur ve string olarak döner.
        /// </summary>
        public static string MsReadAllText(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Dosya bulunamadı: {path}");

            return File.ReadAllText(path);
        }

        /// <summary>
        /// Belirtilen dosya yoluna verilen içeriği yazar. Dosya yoksa oluşturur, varsa üzerine yazar.
        /// </summary>
        public static void MsWriteAllText(string path, string content)
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(path, content);
        }

        /// <summary>
        /// Belirtilen dosyanın byte cinsinden dosya boyutunu döner.
        /// </summary>
        public static long MsGetFileSize(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"Dosya bulunamadı: {path}");

            return new FileInfo(path).Length;
        }

        /// <summary>
        /// Dosya uzantısına göre MIME tipini belirler. Bilinmiyorsa 'application/octet-stream' döner.
        /// </summary>
        public static string MsGetMimeType(string fileName)
        {
            var extension = Path.GetExtension(fileName)?.ToLowerInvariant();

            if (extension == null)
                return "application/octet-stream";

            return MimeTypes.TryGetValue(extension, out var mime) ? mime : "application/octet-stream";
        }

        private static readonly Dictionary<string, string> MimeTypes = new()
        {
            { ".txt", "text/plain" },
            { ".csv", "text/csv" },
            { ".pdf", "application/pdf" },
            { ".doc", "application/msword" },
            { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { ".xls", "application/vnd.ms-excel" },
            { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { ".png", "image/png" },
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".gif", "image/gif" },
            { ".zip", "application/zip" },
            { ".rar", "application/vnd.rar" },
            { ".7z", "application/x-7z-compressed" },
            { ".json", "application/json" },
            { ".xml", "application/xml" },
            { ".mp4", "video/mp4" },
            { ".mp3", "audio/mpeg" },
            { ".html", "text/html" },
            { ".htm", "text/html" }
        };

        /// <summary>
        /// Bir dosyayı GZip formatında sıkıştırır.
        /// </summary>
        public static void MsCompressFile(string sourcePath, string targetPath)
        {
            if (!File.Exists(sourcePath))
                throw new FileNotFoundException($"Kaynak dosya bulunamadı: {sourcePath}");

            using var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read);
            using var targetStream = new FileStream(targetPath, FileMode.Create);
            using var compressionStream = new GZipStream(targetStream, CompressionMode.Compress);

            sourceStream.CopyTo(compressionStream);
        }

        /// <summary>
        /// GZip ile sıkıştırılmış bir dosyayı belirtilen hedef dosyaya açar (decompress eder).
        /// </summary>
        public static void MsDecompressFile(string sourcePath, string targetPath)
        {
            if (!File.Exists(sourcePath))
                throw new FileNotFoundException($"Kaynak dosya bulunamadı: {sourcePath}");

            using var sourceStream = new FileStream(sourcePath, FileMode.Open);
            using var targetStream = new FileStream(targetPath, FileMode.Create);
            using var decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress);

            decompressionStream.CopyTo(targetStream);
        }

        /// <summary>
        /// Belirtilen dosyayı hedef konuma kopyalar. Hedef dosya varsa üzerine yazma seçeneği sunar.
        /// </summary>
        public static void MsCopyFile(string sourcePath, string targetPath, bool overwrite = true)
        {
            if (!File.Exists(sourcePath))
                throw new FileNotFoundException($"Kaynak dosya bulunamadı: {sourcePath}");

            var directory = Path.GetDirectoryName(targetPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            File.Copy(sourcePath, targetPath, overwrite);
        }

        /// <summary>
        /// Belirtilen dosyayı hedef konuma taşır. Taşıma sırasında hedef klasör yoksa oluşturur.
        /// </summary>
        public static void MsMoveFile(string sourcePath, string targetPath)
        {
            if (!File.Exists(sourcePath))
                throw new FileNotFoundException($"Kaynak dosya bulunamadı: {sourcePath}");

            var directory = Path.GetDirectoryName(targetPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            File.Move(sourcePath, targetPath);
        }

        /// <summary>
        /// Belirtilen dosyayı siler. Dosya yoksa işlem yapmaz, hata fırlatmaz.
        /// </summary>
        public static void MsDeleteFile(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        /// <summary>
        /// Belirtilen dosyanın var olup olmadığını kontrol eder. True/False döner.
        /// </summary>
        public static bool MsFileExists(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// Belirtilen klasörü ZIP dosyasına sıkıştırır. Hedef dosya varsa isteğe bağlı üzerine yazılabilir.
        /// </summary>
        public static void MsCompressDirectory(string sourceDirectory, string zipFilePath, bool overwrite = true)
        {
            if (!Directory.Exists(sourceDirectory))
                throw new DirectoryNotFoundException($"Klasör bulunamadı: {sourceDirectory}");

            if (File.Exists(zipFilePath) && overwrite)
                File.Delete(zipFilePath);

            var directory = Path.GetDirectoryName(zipFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            ZipFile.CreateFromDirectory(sourceDirectory, zipFilePath, CompressionLevel.Optimal, includeBaseDirectory: false);
        }

        /// <summary>
        /// ZIP dosyasını belirtilen klasöre açar (decompress eder). Hedef klasör varsa isteğe bağlı olarak silinir ve yeniden oluşturulur.
        /// </summary>
        public static void MsDecompressDirectory(string zipFilePath, string targetDirectory, bool overwrite = true)
        {
            if (!File.Exists(zipFilePath))
                throw new FileNotFoundException($"ZIP dosyası bulunamadı: {zipFilePath}");

            if (Directory.Exists(targetDirectory) && overwrite)
                Directory.Delete(targetDirectory, true);

            ZipFile.ExtractToDirectory(zipFilePath, targetDirectory);
        }
    }
}
