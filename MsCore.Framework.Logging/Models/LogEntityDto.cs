using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsCore.Framework.Logging.Models
{
    public class LogEntityDto
    {
        public Guid? CorrelationId { get; set; }
        public string? Error { get; set; }
        public string? Detail { get; set; }
        public string? HttpMethod { get; set; }
        public string? Path { get; set; }
        public string? User { get; set; }
        public string? RequestBody { get; set; }
        public string? ResponseBody { get; set; }
        public string? QueryString { get; set; }
        public long? ElapsedMs { get; set; }
        public LogEntityDto(Guid? correlationId = null, string? error = null, string? detail = null, string? method = null, string? path = null, string? user = null, string? requestBody = null, string? responseBody = null, string? queryString = null, long? elapsedMs = null)
        {
            CorrelationId = correlationId == null ? Guid.NewGuid() : correlationId;
            Error = error;
            Detail = detail;
            HttpMethod = method;
            Path = path;
            User = user;
            RequestBody = requestBody;
            ResponseBody = responseBody;
            QueryString = queryString;
            ElapsedMs = elapsedMs;
        }
    }
}
