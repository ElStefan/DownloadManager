using DownloadManager.Library.Helper;
using Serilog;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadManager.Library.Jobs
{
    public class WikiUpdateJob : IJob
    {
        private static readonly ILogger Log = Log.ForContext(typeof(WikiUpdateJob));

        public async Task Execute(IJobExecutionContext context)
        {
            Log.Debug("Execute - Start");
            try
            {
                await Task.Run(CheckVersions);
            }
            catch (Exception exception)
            {
                Log.Error("Execute", exception);
            }
            Log.Debug("Execute - Stop");
        }

        private void CheckVersions()
        {
            var apps = DatabaseHelper.GetAll();
            if(apps == null)
            {
                return;
            }
            foreach (var app in apps)
            {
                if (String.IsNullOrEmpty(app.WikiLink))
                {
                    continue;
                }
                var version = LinkHelper.GetNewestVersion(app.WikiLink);
                if (version.Equals(app.WikiVersion, StringComparison.OrdinalIgnoreCase))
                {
                    Log.Debug($"CheckVersions - No update found for '{app.Name}' - Version: '{app.WikiVersion}'");
                    continue;
                }
                Log.Debug($"CheckVersions - Update found for '{app.Name}' - Change: '{app.WikiVersion}' => '{version}'");
                app.WikiVersion = version;
                DatabaseHelper.Update(app, false);
            }
        }
    }
}