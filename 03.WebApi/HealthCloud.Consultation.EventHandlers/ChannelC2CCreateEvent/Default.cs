using HealthCloud.Common.EventBus;
using HealthCloud.Common.Log;
using HealthCloud.Consultation.Dto.Request;
using HealthCloud.Consultation.IServices;
using HealthCloud.Consultation.Services;
using HealthCloud.Consultation.Services.QQCloudy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.EventHandlers.ChannelC2CCreateEvent
{

    public class DefaultHandler : IEventHandler<Dto.EventBus.ChannelC2CCreateEvent>
    {
        ConversationRoomService roomService = new ConversationRoomService();
        IIMHepler imService = new IMHelper();



        public bool Handle(Dto.EventBus.ChannelC2CCreateEvent evt)
        {
            try
            {
                if (evt == null)
                {
                    return true;
                }

                if (string.IsNullOrEmpty(evt.FromUserID))
                {
                    return true;
                }

                if (evt.AddFriendItem == null || evt.AddFriendItem.Count <= 0)
                {
                    return true;
                }


                //创建房间请求参数
                var createC2CChannelRequest = new RequestConversactionApplyAddFriendDTO()
                {
                    FromUserIdentifier = evt.FromUserIdentifier,
                    FromUserMemberID = evt.FromUserMemberID,
                    FromUserType = evt.FromUserType,
                    FromUserID = evt.FromUserID,
                    FromUserName = evt.FromUserName,
                    AddFriendItem = new List<AddFriendAccount>()
                };

                //添加群组请求参数
                var requestParamsCreateGroup = new List<int>() { evt.FromUserIdentifier };
                //添加好友请求参数
                var requestParamsApplyAddFriend = new List<Dto.IM.AddFriendAccount>();

                //循环好友项
                evt.AddFriendItem.ForEach(a =>
                {
                        //好友存在
                        createC2CChannelRequest.AddFriendItem.Add(new AddFriendAccount()
                    {
                        AddType = "Add_Type_Both",
                        AddWording = a.AddWording,
                        ForceAddFlags = 1,
                        GroupName = a.GroupName,
                        Remark = a.Remark,
                        ToUserID = a.ToUserID,
                        ToUserIdentifier = a.ToUserIdentifier,
                        ToUserMemberID = a.ToUserMemberID,
                        ToUserType = a.ToUserType,
                        ToUserName = a.ToUserName
                    });


                    requestParamsApplyAddFriend.Add(new Dto.IM.AddFriendAccount
                    {
                        AddWording = a.AddWording,
                        AddSource = "AddSource_Type_WEB",//需要前缀AddSource_Type_
                            GroupName = a.GroupName,
                        Remark = a.Remark,
                        To_Account = a.ToUserIdentifier.ToString()
                    });

                    requestParamsCreateGroup.Add(a.ToUserIdentifier);
                });

                //写入数据库
                var ChannelInfoList = roomService.ApplyAddFriend(createC2CChannelRequest);

                using (MQChannel mqChannel = new MQChannel())
                {
                    mqChannel.BeginTransaction();

                    foreach (var item in ChannelInfoList)
                    {
                        var room = item.Value;

                        //根据Uid 获取好友信息
                        var friend = requestParamsApplyAddFriend.Find(a => a.To_Account == item.Key.ToString());

                        //如果房间还未启用才调用请求否则忽略
                        if (!room.Enable)
                        {
                            //发送好友附言消息
                            if (!mqChannel.Publish(new Dto.EventBus.ChannelSendGroupMsgEvent<string>()
                            {
                                ChannelID = room.ConversationRoomID,
                                FromAccount = evt.FromUserIdentifier,
                                Msg = friend.AddWording
                            }))
                            {
                                return false;
                            }

                            //发布房间创建完成的领域消息
                            if (!mqChannel.Publish(new Dto.EventBus.ChannelCreatedEvent()
                            {
                                ChannelID = room.ConversationRoomID,
                                ServiceID = room.ServiceID,
                                ServiceType = room.ServiceType

                            }))
                            {
                                return false;
                            }

                            //新增好友
                            if (!imService.ApplyAddFriend(evt.FromUserIdentifier.ToString(), requestParamsApplyAddFriend))
                            {
                                return false;
                            }

                            //创建群组
                            if (!imService.CreateGroup(room.ConversationRoomID, room.ConversationRoomID, room.ServiceType, requestParamsCreateGroup, "", ""))
                            {
                                return false;
                            }

                            //设置房间已经启用
                            //room.Enable = true;

                            //更新房间状态
                            if (!roomService.UpdateRoomEable(room.ConversationRoomID, true))
                            {
                                return false;
                            }
                        }
                    }

                    mqChannel.Commit();
                }

                return true;

            }
            catch (Exception E)
            {
                LogHelper.DefaultLogger.Error(E);
            }

            return false;
        }
    }


}
