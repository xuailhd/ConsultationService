using HealthCloud.Common.Cache;
using HealthCloud.Common.EventBus;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.TaskService
{
    [RunInstaller(true)]
    public partial class JobService : ServiceBase
    {
        IScheduler scheduler;

        public string serviceType { get; set; }

        public JobService(string serviceType)
        {
            this.serviceType = serviceType;
            InitializeComponent();

        }

        public void Start()
        {
            try
            {
                if (serviceType.ToLower() == "job")
                {
                    scheduler = StdSchedulerFactory.GetDefaultScheduler();
                    scheduler.Start();
                }
                else if (serviceType.ToLower() == "mq")
                {
                    HealthCloud.Consultation.EventHandlers.BundleConfig.Register();
                }

            }
            catch (Exception ex)
            {
                KMEHosp.Common.LogHelper.WriteError(ex);
            }
        }

        protected override void OnStart(string[] args)
        {
            Start();

        }

        protected override void OnStop()
        {
            KMEHosp.Common.LogHelper.WriteDebug("服务：开始 OnStop");
            if (scheduler != null)
            {
                scheduler.Shutdown(true);
            }

            KMEHosp.Common.LogHelper.WriteDebug("服务：结束 OnStop");
        }

    }
}
