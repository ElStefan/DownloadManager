using DownloadManager.Library.Jobs;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadManager.Library.Core
{
    public class SystemCore
    {
        private IScheduler _scheduler;

        #region Singleton

        private static readonly Lazy<SystemCore> _instance = new Lazy<SystemCore>(() => new SystemCore());

        private SystemCore()
        {
        }

        public static SystemCore Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        #endregion Singleton

        public bool Start()
        {
            this._scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var runTime = DateBuilder.EvenHourDateAfterNow();

            var linkValidatorJob = JobBuilder.Create<LinkValidatorJob>()
                .WithIdentity(nameof(LinkValidatorJob))
                .Build();

            var wikiUpdateJob = JobBuilder.Create<WikiUpdateJob>()
                .WithIdentity(nameof(WikiUpdateJob))
                .Build();

            var linkValidatorTrigger = TriggerBuilder.Create()
                .ForJob(nameof(LinkValidatorJob))
                .StartAt(runTime)
                .WithCronSchedule("0 0 23 * * ? *")
                .WithIdentity("LinkValidatorTrigger")
                .Build();

            var wikiUpdateTrigger = TriggerBuilder.Create()
                .ForJob(nameof(WikiUpdateJob))
                .StartAt(runTime)
                .WithCronSchedule("0 0 23 * * ? *")
                .WithIdentity("WikiUpdateTrigger")
                .Build();

            this._scheduler.ScheduleJob(wikiUpdateJob, wikiUpdateTrigger);
            this._scheduler.ScheduleJob(linkValidatorJob, linkValidatorTrigger);

            this._scheduler.Start();

            this._scheduler.TriggerJob(new JobKey(nameof(WikiUpdateJob)));
            this._scheduler.TriggerJob(new JobKey(nameof(LinkValidatorJob)));

            return true;
        }

        public bool Stop()
        {
            this._scheduler.Shutdown();
            return true;
        }
    }
}
