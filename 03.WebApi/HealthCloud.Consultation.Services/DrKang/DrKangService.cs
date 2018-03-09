using KMEHosp.Hystrix;
using HealthCloud.Consultation.Services.DrKang.Command;
using HealthCloud.Consultation.Services.DrKang.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Services.DrKang
{
    /// <summary>
    /// 康博士服务
    /// 作者：郭明
    /// 日期：2017年8月16日
    /// </summary>
    public class DrKangService
    {

        /// <summary>
        /// 设置基本信息
        /// 作者：郭明
        /// 日期：2017年8月16日
        /// </summary>
        /// <param name="name"></param>
        /// <param name="birthday"></param>
        /// <param name="des"></param>
        /// <param name="gender"></param>
        /// <param name="deviceID"></param>
        /// <returns></returns>
        public ResponseResultDataDTO setBaseMsg(string name,string birthday,string des,string gender,string deviceID)
        { 
            setBaseMsgCommand command = new setBaseMsgCommand(name, birthday, des, gender, deviceID);
            return command.Execute();
        }

        /// <summary>
        /// 导诊
        /// 作者：郭明
        /// 日期：2017年7月12日
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="userDeviceId"></param>
        public ResponseResultDataDTO drKangGuide(string keyword,string userDeviceId)
        {
            drKangGuideCommand command = new drKangGuideCommand(keyword, userDeviceId);
            return command.Execute();
        }

        /// <summary>
        /// 获取诊断小结
        /// 作者：郭明
        /// 日期：2017年7月12日
        /// </summary>
        public ResponseInterrogationRecordDTO getInterrogationRecord(string id)
        {
            getInterrogationRecordCommand command = new getInterrogationRecordCommand(id);
            return command.Execute();

        }

        /// <summary>
        /// 请求康博士服务
        /// 作者：郭明
        /// 日期：2017年7月12日
        /// </summary>
        public ResponseRequestDrKangServerDTO requestDrKangServer(EnumServiceType serviceType,string sessionID)
        {
            requestDrKangServerCommand command = new requestDrKangServerCommand(serviceType.ToString(), sessionID);
            return command.Execute();
        }

        /// <summary>
        /// 请求康博士服务
        /// 作者：郭明
        /// 日期：2017年7月12日
        /// </summary>
        public ResponseResultDataDTO pull(string sessionID)
        {
            pullCommand command = new pullCommand(sessionID);
            return command.Execute();
        }

        /// <summary>
        /// 请求康博士服务
        /// 作者：郭明
        /// 日期：2017年7月12日
        /// </summary>
        public ResponseResultDataDTO push(string sessionID,string msgId, string text)
        {
            pushCommand command = new pushCommand(sessionID, msgId, text);
            return command.Execute();
        }

        /// <summary>
        /// 获取问题记录
        /// 作者：郭明
        /// 日期：2017年7月12日
        /// </summary>
        public ResponseBaseQuestionRecordDataDTO getBaseQuestionRecord(string sessionID)
        {
            getBaseQuestionRecordCommand command = new getBaseQuestionRecordCommand(sessionID);
            return command.Execute();

        }

    }
}
