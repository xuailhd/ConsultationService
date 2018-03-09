using HealthCloud.Consultation.Common.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Repositories
{
    public class SysConfigRepository
    {
        /// <summary>
        /// 从数据库获取某个配置项
        /// </summary>
        /// <typeparam name="TResult">配置项类型</typeparam>
        /// <returns></returns>
        public static bool Set<TConfigSection>(TConfigSection config)
        {
            IConfigSectionHandler cacheHandler = new CacheConfigSectionHandler();
            IConfigSectionHandler dbHandler = new DbConfigSectionHandler();

            if (dbHandler.SetSection<TConfigSection>(config))
            {
                return cacheHandler.SetSection<TConfigSection>(config);
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 解析配置
        /// </summary>
        /// <typeparam name="TConfigSection">配置项类型</typeparam>
        /// <param name="CreateCacheFun">如果缓存不存在则通过此委托创建</param>
        /// <returns></returns>
        public static TConfigSection Get<TConfigSection>(bool cached = true, string PropNameSurfix = "")
        {
            IConfigSectionHandler cacheHandler = new CacheConfigSectionHandler();
            IConfigSectionHandler dbHandler = new DbConfigSectionHandler();

            if (cached)
            {

                var entity = cacheHandler.GetSection<TConfigSection>(PropNameSurfix);

                if (entity == null)
                {
                    //从数据库获取配置
                    entity = dbHandler.GetSection<TConfigSection>(PropNameSurfix);

                    if (entity != null)
                    {
                        //设置缓存
                        cacheHandler.SetSection(entity, PropNameSurfix);
                    }
                }

                return entity;
            }
            else
            {

                var entity = dbHandler.GetSection<TConfigSection>(PropNameSurfix);
                if (entity != null)
                {
                    //设置缓存
                    cacheHandler.SetSection(entity, PropNameSurfix);
                }

                return entity;
            }
        }
    }
}
