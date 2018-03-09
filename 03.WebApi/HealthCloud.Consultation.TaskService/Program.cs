using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using CommandLine;
using CommandLine.Text;
using Logging = Common.Logging;
using HealthCloud.Common.JobHelper;
using System.Windows.Forms;
using System.ServiceProcess;

namespace HealthCloud.Consultation.TaskService
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                Logging.LogManager.Adapter = new Logging.Simple.ConsoleOutLoggerFactoryAdapter { Level = Logging.LogLevel.All };

                var options = new Options();

                if (CommandLine.Parser.Default.ParseArguments(args, options))
                {
                    Console.WriteLine($"console:{options.console}");
                    Console.WriteLine($"install:{options.install}");
                    Console.WriteLine($"unstall:{options.unstall}");
                    Console.WriteLine($"serviceName:{options.serviceName}");
                    Console.WriteLine($"serviceType:{options.serviceType}");
                    ProjectInstaller.ServiceName = options.serviceName;

                    //安装
                    if (options.install)
                    {

                        HealthCloud.Common.Log.LogHelper.DefaultLogger.Debug("安装服务:ServiceName:" + options.serviceName);
                        HealthCloud.Common.Log.LogHelper.DefaultLogger.Debug("安装服务:path:" + Application.ExecutablePath);

                        //安装
                        ServiceHelper.Install(
                           options.serviceName,                                // 服务名
                            options.serviceName,                             // 显示名称
                            $@"""{Application.ExecutablePath}"" -t {options.serviceType} -n {options.serviceName}",      // 映像路径，可带参数，若路径有空格，需给路径（不含参数）套上双引号
                            options.serviceName,                         // 服务描述
                            ServiceStartType.Auto,                 // 启动类型
                            Common.JobHelper.ServiceAccount.LocalService,           // 运行帐户，可选，默认是LocalSystem，即至尊帐户
                            null                              // 依赖服务，要填服务名称，没有则为null或空数组，可选
                            );


                        return;
                    }
                    //卸载
                    else if (options.unstall)
                    {

                        ProjectInstaller.ServiceName = options.serviceName;
                        HealthCloud.Common.Log.LogHelper.DefaultLogger.Debug("卸载服务 ServiceName:" + options.serviceName);
                        HealthCloud.Common.Log.LogHelper.DefaultLogger.Debug("卸载服务 path:" + Application.ExecutablePath);

                        ServiceHelper.Uninstall(options.serviceName);
                        return;
                    }
                    //控制台运行
                    else if (options.console)
                    {
                        if (options.serviceType.ToLower() == "mq")
                        {
                            new JobService(options.serviceType).Start();
                        }
                        else if (options.serviceType.ToLower() == "job")
                        {
                            new JobService(options.serviceType).Start();
                        }
                    }
                    //服务运行
                    else
                    {
                        ServiceBase.Run(new JobService(options.serviceType));
                    }
                }
            }
            catch (System.Exception E)
            {
                Console.WriteLine(E.Message);
                HealthCloud.Common.Log.LogHelper.DefaultLogger.Error(E.Message, E);
            }

            Console.ReadLine();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //
        }
    }
}
