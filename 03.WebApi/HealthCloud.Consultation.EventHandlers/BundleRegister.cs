using HealthCloud.Common.EventBus;
using HealthCloud.Consultation.Dto.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HealthCloud.Consultation.EventHandlers
{
    public static class BundleConfig
    {
        static MQChannel channel = new MQChannel();
        public static void Register()
        {
            channel.IfAckThen((eventId, queueName) =>
            {
                //try
                //{
                //    if (!string.IsNullOrEmpty(eventId))
                //    {
                //        eventService.UpdateFinishedState(eventId, queueName);
                //    }
                //}
                //catch (Exception ex)
                //{
                //    KMEHosp.Common.LogHelper.WriteError(ex);
                //}

            }).IfNackThen((eventId, queueName, ex, eventObj) =>
            {
                //var requeue = true;

                //try
                //{
                //    if (ex != null)
                //    {
                //        KMEHosp.Common.LogHelper.WriteError(ex);
                //    }

                //    if (!string.IsNullOrEmpty(eventId))
                //    {
                //        //记录重试次数(在阀值内则重新写队列)
                //        requeue = eventService.IncrementRetryCount(eventId, queueName);

                //    }
                //    else
                //    {
                //        //触发消息（DB赤持久化）
                //        if (eventService.TriggerEvent(eventObj))
                //        {
                //            //消息不需要重新写入队列
                //            requeue = false;
                //        }
                //    }
                //}
                //catch
                //{
                //    KMEHosp.Common.LogHelper.WriteError(ex);
                //}

                ////打印错误日志，然后重新写入队列
                return true;
            });


            #region  DoctorAcceptedQuestionEvent
            channel.Subscribe(new DoctorAcceptEvent.DefaultHandler());
            #endregion

            #region ChannelStateChangedEvent

            channel.Subscribe(new ChannelStateChangedEvent.DefaultHandler());

            channel.Subscribe(new ChannelStateChangedEvent.IfEnterChannelStartCharging());

            channel.Subscribe(new ChannelStateChangedEvent.IfEnterChannelStartRec());

            channel.Subscribe(new ChannelStateChangedEvent.IfLeaveChannelStopRec());

            #endregion

            #region ChanneCreateEvent

            channel.Subscribe(new ChanneCreateEvent.DefaultHandler());


            #endregion

            #region ChannelCreatedEvent
            channel.Subscribe(new ChannelCreatedEvent.IfVideoOrAudioConsultSendConsultContent());
            channel.Subscribe(new ChannelCreatedEvent.IfTextConsultCallDrKangAnswer());
            #endregion

            #region  ChannelNewMsgEvent
            channel.Subscribe(new ChannelNewMsgEvent.DefaultHandler());
            channel.Subscribe(new ChannelNewMsgEvent.IfTextConsultUpdateConsultState());
            channel.Subscribe(new ChannelNewMsgEvent.IfTextConsultCallDrKangAnswer());
            #endregion

            #region ChannelSendGroupMsgEvent
            channel.Subscribe(new ChannelSendGroupMsgEvent.IfCustomMsgSurvey());
            channel.Subscribe(new ChannelSendGroupMsgEvent.IfRoomDurationChangedGroupMsg());
            channel.Subscribe(new ChannelSendGroupMsgEvent.IfTextMsg());
            channel.Subscribe(new ChannelSendGroupMsgEvent.IfImageMsg());
            #endregion

            #region ChannelDurationChangeEvent
            channel.Subscribe(new ChannelDurationChangeEvent.Default());
            #endregion

            #region ChannelExpireEvent
            channel.Subscribe(new ChannelExpireEvent.SendNotice());
            #endregion

            #region ChannelChargingEvent

            channel.Subscribe(new ChannelChargingEvent.DefaultHandler());
            #endregion

            #region ChannelTriageChangedEvent
            channel.Subscribe(new ChannelTriageChangedEvent.DefaultHandler());
            #endregion

            #region ChannelC2CCreateEvent
            channel.Subscribe(new ChannelC2CCreateEvent.DefaultHandler());
            #endregion

            //新增的处理程序请依次注册，后续版本考虑自动加载处理
        }
    }
}
