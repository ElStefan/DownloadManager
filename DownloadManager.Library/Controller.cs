using DownloadManager.Library.Core;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadManager.Library
{
    public static class Controller
    {

        private static readonly ILogger Log = Log.ForContext(typeof(Controller));
        public static bool Start()
        {
            Log.Debug("DownloadManager - Service starting");
            return SystemCore.Instance.Start();
        }
        public static bool Stop()
        {
            Log.Debug("DownloadManager - Stopping");
            return SystemCore.Instance.Stop();
        }
    }
}
