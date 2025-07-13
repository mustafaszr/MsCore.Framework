using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MsCore.Framework.Logging.Interfaces;
using MsCore.Framework.Logging.Models;
using MsCore.Framework.Logging.Models.LoggingConfiguration;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace MsCore.Framework.Logging.Providers
{
    public class MsFileLogger : IMsFileLoggerService
    {
        private readonly FileLoggerOptions _options;

        public MsFileLogger(IOptions<FileLoggerOptions> options)
        {
            _options = options.Value;
            Directory.CreateDirectory(_options.DirectoryPath);
        }
        public async Task LogRequestAsync(LogEntityDto log) => await WriteLogAsync(log, LogTypeEnum.Request);

        public async Task LogResponseAsync(LogEntityDto log) => await WriteLogAsync(log, LogTypeEnum.Response);

        public async Task LogErrorAsync(LogEntityDto log) => await WriteLogAsync(log, LogTypeEnum.Error);

        public async Task LogInfoAsync(LogEntityDto log) => await WriteLogAsync(log, LogTypeEnum.Info);

        public async Task LogWarningAsync(LogEntityDto log) => await WriteLogAsync(log, LogTypeEnum.Warning);

        private async Task WriteLogAsync(LogEntityDto log, LogTypeEnum logType)
        {
            DateTime timestamp = DateTime.Now;

            string icon = logType switch
            {
                LogTypeEnum.Info => "ℹ️",
                LogTypeEnum.Warning => "⚠️",
                LogTypeEnum.Error => "🔥",
                _ => "✅"
            };

            StringBuilder sb = new();
            sb.AppendLine($"==================== [{icon} {logType}] ====================");
            sb.AppendLine($"CorrelationId: {log.CorrelationId ?? Guid.NewGuid()}");
            sb.AppendLine($"Date: {timestamp:dd-MM-yyyy HH:mm:ss}");
            sb.AppendLine($"LogType: {logType}");
            sb.AppendLine($"Error: {log.Error}");
            sb.AppendLine($"Detail: {log.Detail}");
            sb.AppendLine($"HttpMethod: {log.HttpMethod}");
            sb.AppendLine($"Path: {log.Path}");
            sb.AppendLine($"User: {log.User}");
            sb.AppendLine($"RequestBody: {(string.IsNullOrEmpty(log.RequestBody) ? log.RequestBody : MinifyJson(log.RequestBody))}");
            sb.AppendLine($"ResponseBody: {(string.IsNullOrEmpty(log.ResponseBody) ? log.ResponseBody : MinifyJson(log.ResponseBody))}");
            sb.AppendLine($"QueryString: {log.QueryString}");
            sb.AppendLine($"ElapsedMs: {log.ElapsedMs}");
            sb.AppendLine("=======================================================");

            string logLine = sb.ToString();

            string filePath = GetFilePath();

            await File.AppendAllTextAsync(filePath, logLine + Environment.NewLine);
        }

        private string GetFilePath()
        {
            string fileName = _options.FileName;
            string directory = _options.DirectoryPath;
            string filePath;

            switch (_options.RotationType)
            {
                case RotationType.Daily:
                    filePath = Path.Combine(directory, $"{fileName}_{DateTime.Now:ddMMyyyy}.log");
                    break;

                case RotationType.Weekly:
                    GregorianCalendar gc = new GregorianCalendar();
                    int weekNumber = gc.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                    filePath = Path.Combine(directory, $"{fileName}_Week{weekNumber}_{DateTime.Now.Year}.log");
                    break;

                case RotationType.SizeBased:
                    filePath = GetFilePathBySize();
                    break;

                default:
                    filePath = Path.Combine(directory, $"{fileName}.log");
                    break;
            }

            return filePath;
        }

        private string GetFilePathBySize()
        {
            string directory = _options.DirectoryPath;
            string fileName = _options.FileName;
            long maxBytes = _options.MaxFileSizeInMB * 1024 * 1024;

            int counter = 1;
            string filePath;

            do
            {
                filePath = Path.Combine(directory, $"{fileName}_{counter}.log");
                FileInfo fi = new FileInfo(filePath);

                if (!fi.Exists || fi.Length < maxBytes)
                    break;

                counter++;
            }
            while (true);

            return filePath;
        }

        private string MinifyJson(string jsonString)
        {
            try
            {
                using var document = JsonDocument.Parse(jsonString);
                return JsonSerializer.Serialize(document, new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = false // Arada boşluk bırakarak yazmayı devre dışı bırakır
                });
            }
            catch (Exception)
            {
                return jsonString;
            }
        }
    }
}
