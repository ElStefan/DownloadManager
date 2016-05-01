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
    public class LinkValidatorJob : IJob
    {

        private static readonly ILog Log = LogManager.GetLogger(typeof(LinkValidatorJob));

        public void Execute(IJobExecutionContext context)
        {
            Log.Debug("Execute - Start");
            try
            {
                this.CheckDownloadLinks();
            }
            catch (Exception exception)
            {
                Log.Error("Execute", exception);
            }
            Log.Debug("Execute - Stop");
        }

        private void CheckDownloadLinks()
        {
            var apps = DatabaseHelper.GetAll();
            if(apps == null)
            {
                return;
            }
            foreach (var app in apps)
            {
                if (String.IsNullOrEmpty(app.Link))
                {
                    continue;
                }
                var isWorking = LinkHelper.ValidateApplication(app.Link);
                if (!isWorking)
                {
                    Log.ErrorFormat("CheckDownloadLinks - Downloadlink for '{0}' is invalid: '{1}'", app.Name, app.Link);
                    app.LinkValid = false;
                    DatabaseHelper.Update(app, true);
                    continue;
                }
                Log.DebugFormat("CheckDownloadLinks - Downloadlink for '{0}' is valid", app.Name);

                if (!app.LinkValid)
                {
                    Log.DebugFormat("CheckDownloadLinks - Updating validity for '{0}', was invalid", app.Name);
                    app.LinkValid = true;
                    DatabaseHelper.Update(app, true);
                }
            }
        }
    }
}