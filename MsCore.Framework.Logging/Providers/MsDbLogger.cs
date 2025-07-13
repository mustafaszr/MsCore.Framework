using Microsoft.EntityFrameworkCore;
using MsCore.Framework.Logging.Interfaces;
using MsCore.Framework.Logging.Models;
using System.Text.Json;

namespace MsCore.Framework.Logging.Providers
{
    public class MsDbLogger<TContext> : IMsDbLoggerService where TContext : DbContext
    {
        private readonly TContext _context;

        public MsDbLogger(TContext context)
        {
            _context = context;
        }
        public async Task LogRequestAsync(LogEntityDto log) => await WriteLogAsync(log, LogTypeEnum.Request);

        public async Task LogResponseAsync(LogEntityDto log) => await WriteLogAsync(log, LogTypeEnum.Response);

        public async Task LogErrorAsync(LogEntityDto log) => await WriteLogAsync(log, LogTypeEnum.Error);

        public async Task LogInfoAsync(LogEntityDto log) => await WriteLogAsync(log, LogTypeEnum.Info);

        public async Task LogWarningAsync(LogEntityDto log) => await WriteLogAsync(log, LogTypeEnum.Warning);

        private async Task WriteLogAsync(LogEntityDto log, LogTypeEnum logType)
        {
            var entity = new LogEntity
            {
                CorrelationId = log.CorrelationId ?? Guid.NewGuid(),
                Timestamp = DateTime.Now,
                LogType = logType,
                Error = log.Error,
                Detail = log.Detail,
                HttpMethod = log.HttpMethod,
                Path = log.Path,
                User = log.User,
                RequestBody = string.IsNullOrEmpty(log.RequestBody) ? log.RequestBody : MinifyJson(log.RequestBody),
                ResponseBody = string.IsNullOrEmpty(log.ResponseBody) ? log.ResponseBody : MinifyJson(log.ResponseBody),
                QueryString = log.QueryString,
                ElapsedMs = log.ElapsedMs,
            };
            _context.Add(entity);
            await _context.SaveChangesAsync();
        }
        private string MinifyJson(string jsonString)
        {
            try
            {
                using var document = JsonDocument.Parse(jsonString);
                return JsonSerializer.Serialize(document, new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = false
                });
            }
            catch (JsonException)
            {
                return jsonString; // Parse edilemezse orijinal döndür
            }
        }
    }
}
