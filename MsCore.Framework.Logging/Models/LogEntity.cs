using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsCore.Framework.Logging.Models
{
    public class LogEntity
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public LogTypeEnum? LogType { get; set; }
        public string? Error { get; set; }
        public string? Detail { get; set; }
        public string? HttpMethod { get; set; }
        public string? Path { get; set; }
        public string? User { get; set; }
        public string? RequestBody { get; set; }
        public string? ResponseBody { get; set; }
        public string? QueryString { get; set; }
        public long? ElapsedMs { get; set; }
    }
}
