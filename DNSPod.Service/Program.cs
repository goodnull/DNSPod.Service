using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DNSPod.Service
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                bool isRuned;
                Mutex mutex = new Mutex(true, @"Global\DNSPod.Service", out isRuned);
                if (isRuned)
                {
                    DNSPodService service = new DNSPodService();
                    service.TestStartupAndStop(args);
                }
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new DNSPodService()
                };
                ServiceBase.Run(ServicesToRun);

            }

        }
    }
}
