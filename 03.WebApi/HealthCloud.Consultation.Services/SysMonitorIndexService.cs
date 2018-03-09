using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Dynamic;
using System.Text.RegularExpressions;
using HealthCloud.Consultation.Repositories;
using HealthCloud.Consultation.Dto.Request;

namespace HealthCloud.Consultation.Services
{
    public class SysMonitorIndexService
    {
        private readonly SysMonitorIndexRepository sysMonitorIndexRepository;
        public SysMonitorIndexService()
        {
            sysMonitorIndexRepository = new SysMonitorIndexRepository();
        }

        /// <summary>
        /// 修改指标数据
        /// </summary>
        /// <param name="Category"></param>
        /// <param name="Name"></param>
        /// <param name="OutID"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public bool InsertAndUpdate(RequestSysMonitorIndexUpdateDTO request,bool NumberValuePlus=false)
        {
            return sysMonitorIndexRepository.InsertAndUpdate(request, NumberValuePlus);
        }

        /// <summary>
        /// 查询指标
        /// 作者：郭明
        /// 日期：2017年7月6日
        /// </summary>
        /// <param name="Category"></param>
        /// <returns></returns>
        //public System.Data.DataTable QueryTable(string Category,DateTime StartDate,DateTime EndDate)
        //{
        //    using (DBEntities db = new DBEntities())
        //    {
        //        var list = db.SysMonitorIndexs.Where(a => a.Category == Category && a.ModifyTime < EndDate && a.ModifyTime >= StartDate).ToList();

        //        System.Data.DataTable dt = new System.Data.DataTable();

        //        //按照外部记录编号进行分组
        //        var indexNameCategorys = list.GroupBy(a => a.Name);

        //        dt.Columns.Add("OutID");

        //        //创建表格列
        //        foreach (var category in indexNameCategorys)
        //        {
        //            dt.Columns.Add(category.Key);
        //        }


        //        //按照外部记录编号进行分组
        //        var recordGroups = list.GroupBy(a => a.OutID).ToList();

        //        foreach (var recordGroup in recordGroups)
        //        {
        //            var row = dt.NewRow();
        //            row["OutID"] = recordGroup.Key;

        //            foreach (var record in recordGroup)
        //            {
        //                row[record.Name] = record.Value;
        //            }

        //            dt.Rows.Add(row);
        //        }

        //        return dt;
        //    }
        //}
    }
}
