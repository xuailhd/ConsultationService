using HealthCloud.Common.Cache;
using HealthCloud.Common.EventBus;
using HealthCloud.Common.Log;
using HealthCloud.Consultation.Enums;
using HealthCloud.Consultation.ICaches.Keys;
using HealthCloud.Consultation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.EventHandlers.ChannelNewMsgEvent
{
    /// <summary>
    /// 图文咨询，当医生回复时更新回复状态
    /// </summary>
    public class IfTextConsultUpdateConsultState : IEventHandler<Dto.EventBus.ChannelNewMsgEvent>
    {
        ConversationRoomService roomService = new ConversationRoomService();
        UserOPDRegisterService bllUserConsult = new UserOPDRegisterService();


        public bool Handle(Dto.EventBus.ChannelNewMsgEvent evt)
        {
            try
            {
                if (evt.OptPlatform != "RESTAPI" && evt.ServiceType == EnumDoctorServiceType.PicServiceType)
                {

                    //获取医生回复状态，已经回答的则忽略
                    var cacheKey_Channel_DoctorAnswerState = new StringCacheKey(StringCacheKeyType.Channel_DoctorAnswerState, evt.ChannelID.ToString());
                    var Channel_DoctorAnswerState = cacheKey_Channel_DoctorAnswerState.FromCache<bool?>();
                    if (Channel_DoctorAnswerState.HasValue && Channel_DoctorAnswerState.Value)
                    {
                        return true;
                    }

                    var room = roomService.GetChannelInfo(evt.ChannelID);

                    if (room != null && (
                        room.RoomState != EnumRoomState.AlreadyVisit &&
                        room.RoomState != EnumRoomState.InMedicalTreatment))
                    {
                        var userInfo = roomService.GetChannelUsersInfo(evt.ChannelID).Where(t =>
                        t.identifier == Convert.ToInt32(evt.FromAccount)).FirstOrDefault();

                        //医生回复了
                        if (userInfo != null && userInfo.UserType == EnumUserType.Doctor)
                        {

                            //修改咨询状态为已回复
                            bllUserConsult.UpdateReplied(evt.ServiceID);

                            var ExpectedState = room.RoomState;
                            if (roomService.CompareAndSetChannelState(
                                room.ConversationRoomID,
                                userInfo.UserID,
                                EnumRoomState.InMedicalTreatment,
                                room.DisableWebSdkInteroperability,
                                ref ExpectedState) != EnumApiStatus.BizOK)
                            {
                                return false;
                            }

                            //设置医生回复状态
                            true.ToCache(cacheKey_Channel_DoctorAnswerState,TimeSpan.FromHours(24));

                            using (MQChannel mqChannel = new MQChannel())
                            {
                                return mqChannel.Publish(new Dto.EventBus.UserNoticeEvent()
                                {                                 
                                    NoticeType = EnumNoticeSecondType.UserPicDoctorReplyNotice,
                                    ServiceID =evt.ServiceID
                                });
                            }

                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.DefaultLogger.Error(ex);
            }

            return false;

        }
    }
}
