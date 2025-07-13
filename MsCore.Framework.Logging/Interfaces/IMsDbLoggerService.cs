using MsCore.Framework.Logging.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsCore.Framework.Logging.Interfaces
{
    public interface IMsDbLoggerService
    {
        Task LogRequestAsync(LogEntityDto log);
        Task LogResponseAsync(LogEntityDto log);
        Task LogInfoAsync(LogEntityDto log);
        Task LogWarningAsync(LogEntityDto log);
        Task LogErrorAsync(LogEntityDto log);
    }
}
