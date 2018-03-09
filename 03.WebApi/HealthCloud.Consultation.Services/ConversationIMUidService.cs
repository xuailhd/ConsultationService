using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using HealthCloud.Consultation.ICaches.Keys;
using HealthCloud.Consultation.Enums;
using HealthCloud.Common.Cache;
using HealthCloud.Consultation.Repositories;

namespace HealthCloud.Consultation.Services
{
    /// <summary>
    /// 房间相关业务逻辑
    /// </summary>
    public class ConversationIMUidService 
    {
        private readonly ConversationIMUidRepository conversationIMUidRepository;

        public ConversationIMUidService()
        {
            conversationIMUidRepository = new ConversationIMUidRepository();
        }

        
        /// <summary>
        /// 获取用户的通信唯一标识
        /// </summary>
        /// <param name="doctorIDList"></param>
        /// <returns></returns>
        public int GetUserIMUid(string UserID)
        {
            return conversationIMUidRepository.GetUserIMUid(UserID);
        }

        /// <summary>
        /// 通过IM唯一标识获取用户编号
        /// </summary>
        /// <param name="Identifiers"></param>
        /// <returns></returns>
        public List<string> GetUserIdByUids(params int[] Identifiers)
        {
            return conversationIMUidRepository.GetUserIdByUids(Identifiers);
        }

    }
}
