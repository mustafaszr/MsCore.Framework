using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsCore.Framework.Logging.Models.LoggingConfiguration
{
    public class FileLoggerOptions
    {
        public string DirectoryPath { get; set; } = "Logs";
        public string FileName { get; set; } = "log";
        public long MaxFileSizeInMB { get; set; } = 5;
        public RotationType RotationType { get; set; } = RotationType.Daily;
    }
}
