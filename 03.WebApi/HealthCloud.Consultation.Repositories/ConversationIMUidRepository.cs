using HealthCloud.Common.Cache;
using HealthCloud.Consultation.Dto.Common;
using HealthCloud.Consultation.Dto.EventBus;
using HealthCloud.Consultation.Dto.Request;
using HealthCloud.Consultation.Dto.Response;
using HealthCloud.Consultation.Enums;
using HealthCloud.Consultation.ICaches.Keys;
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
    public class ConversationIMUidRepository:BaseRepository
    {

        /// <summary>
        /// 获取医生的通信唯一标识
        /// </summary>
        /// <param name="doctorIDList"></param>
        /// <returns></returns>
        public int GetUserIMUid(string userID)
        {
            var cacheKey = new StringCacheKey(StringCacheKeyType.MAP_GetIMUidByUserID, userID);

            int? uid = cacheKey.FromCache<int?>();
            if (uid == null || !uid.HasValue || uid.Value == 0)
            {
                uid = GetUserIMUids(userID).FirstOrDefault();
                uid.ToCache(cacheKey);
            }

            return uid.Value;

        }

        /// <summary>
        /// 获取医生的通信唯一标识
        /// </summary>
        /// <returns></returns>
        public List<int> GetUserIMUids(params string[] userIDList)
        {
            using (DBEntities db = new DBEntities())
            {
                #region 根据医生编号查询医生的所有用户标识
                var identifiers = (from uid in db.ConversationIMUids.Where(a => !a.IsDeleted)
                                   where userIDList.Contains(uid.UserID)
                                   select uid.Identifier).ToList();

                return identifiers;
                #endregion
            }
        }

        /// <summary>
        /// 通过IM唯一标识获取用户编号
        /// </summary>
        /// <param name="Identifiers"></param>
        /// <returns></returns>
        public List<string> GetUserIdByUids(params int[] Identifiers)
        {
            using (DBEntities db = new DBEntities())
            {

                return (from uid in db.ConversationIMUids.Where(a => !a.IsDeleted) 
                        join identifier in Identifiers on uid.Identifier equals identifier
                        select uid.UserID).ToList();

            }
        }

        /// <summary>
        /// 标识当前通信唯一标识有效
        /// </summary>
        /// <param name="Identifier"></param>
        /// <returns></returns>
        public bool EnableUid(int[] Identifiers, DBEntities db = null)
        {
            bool dbPrivateFlag = false;
            if (db == null)
            {
                db = CreateDb();
                dbPrivateFlag = true;
            }

            var list = db.ConversationIMUids.Where(a => Identifiers.Contains(a.Identifier) && !a.Enable).ToList();

            list.ForEach(t =>
            {
                t.Enable = true;
                t.ModifyTime = DateTime.Now;
            });

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
