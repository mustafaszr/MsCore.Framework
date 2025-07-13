using MsCore.Framework.Logging.Interfaces;
using MsCore.Framework.Logging.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsCore.Framework.Logging.Providers
{
    public class MsCompositeLogger : IMsLoggerService
    {
        private readonly IEnumerable<IMsFileLoggerService> _fileLoggers;
        private readonly IEnumerable<IMsDbLoggerService> _dbLoggers;

        public MsCompositeLogger(IEnumerable<IMsFileLoggerService> fileLoggers, IEnumerable<IMsDbLoggerService> dbLoggers)
        {
            _fileLoggers = fileLoggers;
            _dbLoggers = dbLoggers;
        }

        public async Task LogErrorAsync(LogEntityDto log)
        {
            var tasks = _fileLoggers.Select(l => l.LogErrorAsync(log)).Concat(_dbLoggers.Select(l => l.LogErrorAsync(log)));
            await Task.WhenAll(tasks);
        }

        public async Task LogWarningAsync(LogEntityDto log)
        {
            var tasks = _fileLoggers.Select(l => l.LogWarningAsync(log)).Concat(_dbLoggers.Select(l => l.LogWarningAsync(log)));
            await Task.WhenAll(tasks);
        }

        public async Task LogInfoAsync(LogEntityDto log)
        {
            var tasks = _fileLoggers.Select(l => l.LogInfoAsync(log)).Concat(_dbLoggers.Select(l => l.LogInfoAsync(log)));
            await Task.WhenAll(tasks);
        }

        public async Task LogRequestAsync(LogEntityDto log)
        {
            var tasks = _fileLoggers.Select(l => l.LogRequestAsync(log)).Concat(_dbLoggers.Select(l => l.LogRequestAsync(log)));
            await Task.WhenAll(tasks);
        }

        public async Task LogResponseAsync(LogEntityDto log)
        {
            var tasks = _fileLoggers.Select(l => l.LogResponseAsync(log)).Concat(_dbLoggers.Select(l => l.LogResponseAsync(log)));
            await Task.WhenAll(tasks);
        }
    }
}
