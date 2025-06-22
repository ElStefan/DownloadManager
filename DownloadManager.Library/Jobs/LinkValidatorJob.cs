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
    public class LinkValidatorJob : IJob
    {

        private static readonly ILogger Log = Log.ForContext(typeof(LinkValidatorJob));

        public async Task Execute(IJobExecutionContext context)
        {
            Log.Debug("Execute - Start");
            try
            {
                await Task.Run(CheckDownloadLinks);
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
                    Log.Error($"CheckDownloadLinks - Downloadlink for '{app.Name}' is invalid: '{app.Link}'");
                    app.LinkValid = false;
                    DatabaseHelper.Update(app, true);
                    continue;
                }
                Log.Debug($"CheckDownloadLinks - Downloadlink for '{app.Name}' is valid");

                if (!app.LinkValid)
                {
                    Log.Debug($"CheckDownloadLinks - Updating validity for '{app.Name}', was invalid");
                    app.LinkValid = true;
                    DatabaseHelper.Update(app, true);
                }
            }
        }
    }
}