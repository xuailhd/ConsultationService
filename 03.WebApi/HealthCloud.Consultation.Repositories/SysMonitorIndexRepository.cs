using HealthCloud.Consultation.Dto.Common;
using HealthCloud.Consultation.Dto.EventBus;
using HealthCloud.Consultation.Dto.Request;
using HealthCloud.Consultation.Dto.Response;
using HealthCloud.Consultation.Enums;
using HealthCloud.Consultation.Models;
using HealthCloud.Consultation.Repositories.EF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Repositories
{
    public class SysMonitorIndexRepository : BaseRepository
    {
        /// <summary>
        /// 修改指标数据
        /// </summary>
        /// <returns></returns>
        public bool InsertAndUpdate(RequestSysMonitorIndexUpdateDTO request, bool NumberValuePlus = false, DBEntities db = null)
        {
            bool dbPrivateFlag = false;
            if (db == null)
            {
                db = CreateDb();
                dbPrivateFlag = true;
            }


            foreach (var item in request.Values)
            {
                var model = db.SysMonitorIndexs.Where(a => a.Category == request.Category && a.Name == item.Key && a.OutID == request.OutID).FirstOrDefault();
                if (model == null)
                {
                    db.SysMonitorIndexs.Add(new SysMonitorIndex()
                    {
                        Category = request.Category,
                        Name = item.Key,
                        OutID = request.OutID,
                        Value = string.IsNullOrEmpty(item.Value) ? "-" : item.Value,
                        ModifyTime = DateTime.Now
                    });
                }
                else
                {
                    //数字值递增（保留原来的指标）
                    if (NumberValuePlus)
                    {
                        //原数据和现在的数据都是数字类型
                        if (HealthCloud.Common.ToolHelper.IsNumeric(model.Value) && HealthCloud.Common.ToolHelper.IsNumeric(item.Value))
                        {
                            model.Value = item.Value;
                        }
                        else
                        {
                            model.Value = item.Value;
                        }
                    }
                    else
                    {
                        model.Value = item.Value;
                    }

                    model.ModifyTime = DateTime.Now;
                }
            }

            var ret = true;

            if (dbPrivateFlag)
            {
                ret = db.SaveChanges() > 0;
                db.Dispose();
            }

            return ret;
        }
    }
}
