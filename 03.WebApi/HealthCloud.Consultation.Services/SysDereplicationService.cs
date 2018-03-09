﻿using HealthCloud.Consultation.Enums;
using HealthCloud.Consultation.Models;
using HealthCloud.Consultation.Repositories.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Services
{
    public class SysDereplicationService
    {
        /// <summary>
        /// 开
        /// </summary>
        /// <param name="OutID"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public bool Exists(string OutID, string TableName, EnumDereplicationType DereplicationType)
        {
            using (var db = new DBEntities())
            {
                var id = db.SysDereplications.Where(a => a.OutID == OutID && a.TableName == TableName && a.DereplicationType == DereplicationType).Select(i => i.SysDereplicationID).FirstOrDefault();
                return string.IsNullOrEmpty(id) ? false : true;
            }
        }

        /// <summary>
        /// 开
        /// </summary>
        /// <param name="OutID"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public string Begin(string OutID, string TableName, EnumDereplicationType DereplicationType)
        {
            using (var db = new DBEntities())
            {
                var derep = new SysDereplication()
                {
                    OutID = OutID,
                    TableName = TableName,
                    SysDereplicationID = Guid.NewGuid().ToString("N"),
                    SuccessCount = 0,
                    FailCount = 0,
                    DereplicationType = DereplicationType
                };

                db.SysDereplications.Add(derep);

                if (db.SaveChanges() > 0)
                {
                    return derep.SysDereplicationID;
                }
            }

            return "";
        }


        public bool Done(string SysDereplicationID, bool Success)
        {
            using (var db = new DBEntities())
            {
                SysDereplication derep = db.SysDereplications.Where(a => a.SysDereplicationID == SysDereplicationID).FirstOrDefault();

                if (derep != null)
                {
                    if (Success)
                        derep.SuccessCount++;
                    else
                        derep.FailCount++;

                    return db.SaveChanges() > 0;
                }
            }

            return false;
        }
    }
}
