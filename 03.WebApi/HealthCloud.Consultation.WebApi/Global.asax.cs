using HealthCloud.Common.Cache;
using HealthCloud.Common.Cache.Redis;
using HealthCloud.Common.EventBus;
using HealthCloud.Consultation.Common.Config;
using HealthCloud.Consultation.Dto.Common;
using HealthCloud.Consultation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace HealthCloud.Consultation.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            RegisterApplicationConfig();

            //监听刷新系统配置消息（分布式环境下不用重启则可能够系统配置）
            Manager.Instance.Subscribe<object>("Sys/Config/Refresh", (a) =>
            {
                RegisterApplicationConfig();

            });
        }

        public static void RegisterApplicationConfig()
        {

            //获取Redis缓存配置,首次不从缓存中获取
            var redisConfig = SysConfigService.Get<Redis>(false);

            //注册Redis处理程序，并指定默认Db
            Manager.Register(
                (dbNum) =>
                HealthCloud.Common.Cache.Redis.RedisCacheManage.Create(redisConfig, dbNum), int.Parse(redisConfig.DBNum));

            var imgConfig = SysConfigService.Get<IMGStore>(false);

            #region 注册图片配置
            ImageBaseDto.UrlPrefix = imgConfig.UrlPrefix;
            #endregion


            #region 注册消息队列配置
            var mqConfig = SysConfigService.Get<MQ>(false);
            HealthCloud.Common.EventBus.Configuration.RegisterConfig(mqConfig);

            #endregion


            HealthCloud.MicroService.ServiceSelfRegistration.Register(new MicroService.ServiceRegisterConfig()
            {
                SERVICE_REGISTRY_PORT = System.Configuration.ConfigurationManager.AppSettings["SERVICE_REGISTRY_PORT"],
                SERVICE_80_CHECK_HTTP = System.Configuration.ConfigurationManager.AppSettings["SERVICE_80_CHECK_HTTP"],
                SERVICE_80_CHECK_INTERVAL = System.Configuration.ConfigurationManager.AppSettings["SERVICE_80_CHECK_INTERVAL"],
                SERVICE_80_CHECK_TIMEOUT = System.Configuration.ConfigurationManager.AppSettings["SERVICE_80_CHECK_TIMEOUT"],
                SERVICE_ADDRESS = System.Configuration.ConfigurationManager.AppSettings["SERVICE_ADDRESS"],
                SERVICE_NAME = System.Configuration.ConfigurationManager.AppSettings["SERVICE_NAME"],
                SERVICE_PORT = System.Configuration.ConfigurationManager.AppSettings["SERVICE_PORT"],
                SERVICE_REGION = System.Configuration.ConfigurationManager.AppSettings["SERVICE_REGION"],
                SERVICE_REGISTRY_ADDRESS = System.Configuration.ConfigurationManager.AppSettings["SERVICE_REGISTRY_ADDRESS"],
                SERVICE_SELF_REGISTER = System.Configuration.ConfigurationManager.AppSettings["SERVICE_SELF_REGISTER"],
                SERVICE_TAGS = System.Configuration.ConfigurationManager.AppSettings["SERVICE_TAGS"],
            });
        }
    }
}
