using HealthCloud.Consultation.Dto.IM;
using HealthCloud.Consultation.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.IServices
{
    public interface IIMHepler
    {
        bool SendGroupCustomMsg<TMsgType>(string GroupId, int From_Account, IRequestIMCustomMsg<TMsgType> Msg);

        bool CreateGroup(string GroupId,
            string GroupName,
            EnumDoctorServiceType ServiceType,
            List<int> Members,
            string Introduction = "",
            string Notification = "",
            string FaceUrl = "");

        bool AddGroupMember(string GroupId, List<int> Members);

        /// <summary>
        /// 发送图文
        /// </summary>
        /// <param name="GroupId"></param>
        /// <param name="From_Account"></param>
        /// <param name="FileID"></param>
        /// <param name="FileUrl"></param>
        /// <returns></returns>
        bool SendGroupImageMsg(string GroupId, int From_Account, string FileID, string FileUrl);

        /// <summary>
        /// 发送文件消息
        /// </summary>
        /// <param name="GroupId"></param>
        /// <param name="From_Account"></param>
        /// <param name="FileID"></param>
        /// <param name="FileSize"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        bool SendGroupFileMsg(string GroupId, int From_Account, string FileID, long FileSize, string FileName);

        /// <summary>
        /// 发送语音消息
        /// </summary>
        /// <param name="GroupId"></param>
        /// <param name="From_Account"></param>
        /// <param name="FileID"></param>
        /// <param name="FileSize"></param>
        /// <param name="Second"></param>
        /// <returns></returns>
        bool SendGroupAudioMsg(string GroupId, int From_Account, string FileID, long FileSize, int Second);

        /// <summary>
        /// 发送群组文本消息
        /// </summary>
        /// <param name="GroupId"></param>
        /// <param name="From_Account"></param>
        /// <param name="MsgContent"></param>
        /// <returns></returns>
        bool SendGroupTextMsg(string GroupId, int From_Account, string MsgContent);

        /// <summary>
        /// 添加好友
        /// </summary>
        /// <param name="From_Account"></param>
        /// <param name="Accounts"></param>
        /// <returns></returns>
        bool ApplyAddFriend(string From_Account, List<AddFriendAccount> Accounts);
    }
}
