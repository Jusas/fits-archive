using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitsArchiveLib.Interfaces
{
    public interface ILogService : IDisposable
    {
        ILog InitializeLog(string logId, string logfilename);
        ILog InitializeLog(string logId, Stream stream);
        ILog GetLog(string logId);
        bool HasLog(string logId);
    }
}
