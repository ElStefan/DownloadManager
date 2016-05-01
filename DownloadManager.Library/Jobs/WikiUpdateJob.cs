using DownloadManager.Library.Helper;
using log4net;
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
        private static readonly ILog Log = LogManager.GetLogger(typeof(WikiUpdateJob));

        public void Execute(IJobExecutionContext context)
        {
            Log.Debug("Execute - Start");
            try
            {
                this.CheckVersions();
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
                    Log.DebugFormat("CheckVersions - No update found for '{0}' - Version: '{1}'", app.Name, app.WikiVersion);
                    continue;
                }
                Log.DebugFormat("CheckVersions - Update found for '{0}' - Change: '{1}' => '{2}'", app.Name, app.WikiVersion, version);
                app.WikiVersion = version;
                DatabaseHelper.Update(app, false);
            }
        }
    }
}