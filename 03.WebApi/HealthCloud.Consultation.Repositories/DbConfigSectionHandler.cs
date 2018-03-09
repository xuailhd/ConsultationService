using HealthCloud.Common.Log;
using HealthCloud.Consultation.Common.Config;
using HealthCloud.Consultation.Repositories.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Repositories
{
    public class DbConfigSectionHandler : IConfigSectionHandler
    {
        public TConfigSection GetSection<TConfigSection>(string PropNameSurfix)
        {
            return GetConfigByDb<TConfigSection>(PropNameSurfix);
        }

        public bool SetSection<TConfigSection>(TConfigSection config, string Surfix)
        {
            return SetConfigToDb(config, Surfix);
        }

        /// <summary>
        /// 从数据库获取某个配置项
        /// 作者：郭明
        /// 日期：2016年7月29日
        /// </summary>
        /// <typeparam name="TResult">配置项类型</typeparam>
        /// <returns></returns>
        static TConfigSection GetConfigByDb<TConfigSection>(string PropNameSurfix)
        {
            Type type = typeof(TConfigSection);
            var config = Activator.CreateInstance(type);
            var configTypeName = type.Name;
            var props = type.GetProperties();

            //循环所有属性
            foreach (var p in props)
            {
                var value = GetSingleSectionValue(configTypeName, p.Name + PropNameSurfix);
                if (p.CanWrite)
                {
                    //一次读取数据库获取
                    try
                    {
                        p.SetValue(config, value, null);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.DefaultLogger.Debug(ex.Message);
                    };

                }
            }

            return (TConfigSection)config;
        }

        static bool SetConfigToDb<TConfigSection>(TConfigSection config, string Surfix)
        {
            Type type = typeof(TConfigSection);
            var configTypeName = type.Name;
            var props = type.GetProperties();
            //循环所有属性
            using (var db = new DBEntities())
            {
                foreach (var p in props)
                {
                    var configkey = getConfigKey(configTypeName, p.Name) + Surfix;
                    var model = db.SysConfigs.Where(t => t.ConfigKey == configkey).FirstOrDefault();
                    if (model != null)
                    {
                        model.ConfigValue = p.GetValue(config).ToString();
                    }
                }
                db.SaveChanges();
            }
            return true;
        }

        static string GetSingleSectionValue(string ConfigTypeName, string PropName)
        {
            using (var db = new DBEntities())
            {
                var configkey = getConfigKey(ConfigTypeName, PropName);
                var result = db.SysConfigs.Where(t => t.ConfigKey == configkey).FirstOrDefault();
                if (result != null)
                {
                    return result.ConfigValue;
                }
                else
                {
                    return "";
                }
            }
        }

        static string getConfigKey(string ConfigTypeName, string PropName)
        {
            return string.Format("{0}.{1}", ConfigTypeName, PropName);
        }
    }
}
