using HealthCloud.Consultation.Common.Config;
using HealthCloud.Consultation.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Services
{
    public class SysConfigService
    {
        /// <summary>
        /// 从数据库获取某个配置项
        /// </summary>
        /// <typeparam name="TResult">配置项类型</typeparam>
        /// <returns></returns>
        public static bool Set<TConfigSection>(TConfigSection config)
        {
            return SysConfigRepository.Set(config);
        }


        /// <summary>
        /// 解析配置
        /// </summary>
        /// <typeparam name="TConfigSection">配置项类型</typeparam>
        /// <param name="CreateCacheFun">如果缓存不存在则通过此委托创建</param>
        /// <returns></returns>
        public static TConfigSection Get<TConfigSection>(bool cached = true, string PropNameSurfix = "")
        {
            return SysConfigRepository.Get<TConfigSection>(cached, PropNameSurfix);
        }
    }
}
