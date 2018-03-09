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
using HealthCloud.Consultation.Dto.Response;
using HealthCloud.Consultation.ICaches.Keys;
using HealthCloud.Consultation.Enums;
using HealthCloud.Common.Cache;
using HealthCloud.Consultation.IServices;
using HealthCloud.Consultation.Services.QQCloudy;
using HealthCloud.Consultation.Dto.IM;
using HealthCloud.Common.EventBus;
using HealthCloud.Consultation.Dto.EventBus;
using HealthCloud.Common.Log;
using HealthCloud.Consultation.Repositories.EF;
using HealthCloud.Consultation.Models;
using HealthCloud.Common.Extensions;

namespace HealthCloud.Consultation.Services
{
    public class ConversationRoomService
    {
        private readonly ConversationRoomRepository conversationRoomRepository;
        private readonly ConversationIMUidRepository conversationIMUidRepository;
        private readonly IIMHepler imService;
        public ConversationRoomService()
        {
            conversationRoomRepository = new ConversationRoomRepository();
            conversationIMUidRepository = new ConversationIMUidRepository();
            imService = new IMHelper();
        }


        /// <summary>
        /// 获取多个用户的信息
        /// </summary>
        /// <param name="Identifiers"></param>
        /// <returns></returns>
        public List<ResponseConversationRoomMemberDTO> GetChannelUsersInfo(string ChannelID, params int[] Identifiers)
        {
            var result = conversationRoomRepository.GetChannelUsersInfo(ChannelID);

            if (Identifiers.Length > 0)
            {
                return result.Where(a => Identifiers.Contains(a.identifier)).ToList();
            }
            else
            {
                return result;
            }
        }


        /// <summary>
        /// 获取多个用户的信息
        /// </summary>
        /// <param name="Identifiers"></param>
        /// <returns></returns>
        public List<ResponseConversationRoomMemberDTO> GetChannelUsersInfo(string ConversationRoomID)
        {
            return conversationRoomRepository.GetChannelUsersInfo(ConversationRoomID);
        }
        /// <summary>
        /// 获取房间信息
        /// </summary>
        /// <param name="ChannelID"></param>
        /// <returns></returns>
        public ResponseConversationRoomDTO GetChannelInfo(string ConversationRoomID)
        {
            if (!string.IsNullOrEmpty(ConversationRoomID))
            {
                return conversationRoomRepository.GetChannelInfo(ConversationRoomID);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取房间信息
        /// </summary>
        /// <param name="ServiceID"></param>
        /// <returns></returns>
        public ResponseConversationRoomDTO GetChannelInfoByServiceID(string ServiceID)
        {
            var ConversationRoomID = GetChannelIDByServiceID(ServiceID);
            if (!string.IsNullOrEmpty(ConversationRoomID))
                return conversationRoomRepository.GetChannelInfo(ConversationRoomID);
            else
                return null;
        }


        /// <summary>
        /// 根据服务编号获取频道编号
        /// </summary>
        /// <param name="ServiceID"></param>
        /// <returns></returns>
        public string GetChannelIDByServiceID(string ServiceID)
        {
            if (ServiceID != "")
            {
                return conversationRoomRepository.GetChannelIDByServiceID(ServiceID);
            }
            else
            {
                return null;
            }
        }

        public bool InsertChannelMembers(string ConversationRoomID, List<RequestChannelMemberDTO> members)
        {
            return conversationRoomRepository.InsertChannelMembers(ConversationRoomID, members);
        }

        /// <summary>
        /// 查询用户前面还有多少人在候诊
        /// </summary>
        /// <param name="DoctorID">医生</param>
        /// <param name="ChannelID">房间编号</param>
        /// <returns></returns>
        public int GetWaitingCount(string DoctorID, string ConversationRoomID)
        {
            DateTime today = DateTime.Now;
            int Year = today.Year;
            int Month = today.Month;
            int Day = today.Day;
            long TriageID = 0;

            if (!string.IsNullOrEmpty(ConversationRoomID))
            {
                var roomInfo = conversationRoomRepository.GetChannelInfo(ConversationRoomID);
                TriageID = roomInfo.TriageID;
            }

            return conversationRoomRepository.GetWaitingChannels(DoctorID, TriageID).Count;
        }


        /// <summary>
        /// 获取医生分组编号根据医生
        /// </summary>
        /// <param name="UserLevel"></param>
        /// <returns></returns>
        public List<string> GetDoctorGroupMemberByUserLevel(int UserLevel)
        {
            return conversationRoomRepository.GetDoctorGroupMemberByUserLevel(UserLevel);
        }


        /// <summary>
        /// 获取医生分组编号根据医生
        /// </summary>
        /// <param name="DoctorID"></param>
        /// <returns></returns>
        public List<string> GetDoctorGroupIdListByDoctorID(string DoctorID)
        {
            return conversationRoomRepository.GetDoctorGroupIdListByDoctorID(DoctorID);
        }


        /// <summary>
        /// 更新房间可用
        /// </summary>
        /// <param name="ConversationRoomID"></param>
        /// <param name="enable"></param>
        /// <returns></returns>
        public bool UpdateRoomEable(string ConversationRoomID, bool enable)
        {
            return conversationRoomRepository.UpdateRoomEable(ConversationRoomID, enable);
        }

        /// <summary>
        /// 更新房间计时序号
        /// </summary>
        /// <param name="channelID"></param>
        /// <param name="chargingSeq"></param>
        /// <param name="chargingTime"></param>
        /// <param name="chargingInterval"></param>
        /// <param name="roomState"></param>
        /// <returns></returns>
        public bool UpdateChannelChargeSeq(string ConversationRoomID, int chargingSeq, DateTime chargingTime, int chargingInterval, 
            EnumRoomState? roomState = null,DateTime? endTime = null)
        {
            return conversationRoomRepository.UpdateChannelChargeSeq(ConversationRoomID, chargingSeq, chargingTime, chargingInterval, roomState, endTime);
        }

        /// <summary>
        /// 修改分诊编号
        /// </summary>
        /// <param name="ConversationRoomID"></param>
        /// <param name="triageID"></param>
        /// <returns></returns>
        public bool UpdateTriageID(string ConversationRoomID, long triageID)
        {
            return conversationRoomRepository.UpdateTriageID(ConversationRoomID, triageID);
        }

        /// <summary>
        /// 添加好友
        /// 作者：郭明
        /// 日期：2017年6月23日
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Dictionary<int, ResponseConversationRoomDTO> ApplyAddFriend(RequestConversactionApplyAddFriendDTO request)
        {
            Dictionary<int, ResponseConversationRoomDTO> result = new Dictionary<int, ResponseConversationRoomDTO>();
            using (var db = new DBEntities())
            {
                request.AddFriendItem.ForEach(account =>
                {
                    var channelInfo = (from friend in db.ConversationFriends.Where(a => a.FromUserID == request.FromUserID && a.ToUserID == account.ToUserID)
                                       join room in db.ConversationRooms on friend.ConversationRoomID equals room.ConversationRoomID
                                       where room.RoomType == EnumRoomType.C2C
                                       select room).FirstOrDefault();

                    if (channelInfo == null)
                    {
                        var ConversationRoomID = Guid.NewGuid().ToString("N");

                        channelInfo = new ConversationRoom()
                        {
                            ServiceID = "",
                            Priority = 5,
                            ChargingInterval = 0,
                            ChargingSeq = 0,
                            ChargingState = EnumRoomChargingState.Stoped,
                            ChargingTime = DateTime.Now,
                            Duration = 0,
                            TotalTime = 0,
                            Enable = false,
                            ServiceType = EnumDoctorServiceType.PicServiceType,
                            Secret = "",
                            BeginTime = DateTime.Now,
                            EndTime = DateTime.Now,
                            ConversationRoomID = ConversationRoomID,
                            CreateTime = DateTime.Now,
                            IsDeleted = false,
                            RoomState = EnumRoomState.NoTreatment,
                            RoomType = EnumRoomType.C2C,
                            TriageID = long.MaxValue
                        };

                        db.ConversationRooms.Add(channelInfo);

                        db.ConversationRoomUids.Add(new ConversationRoomUid()
                        {
                            ConversationRoomID = ConversationRoomID,
                            Identifier = account.ToUserIdentifier,
                            UserType = account.ToUserType,
                            IsDeleted = false,
                            UserMemberID = account.ToUserMemberID,
                            CreateUserID = request.FromUserID,
                            UserID = account.ToUserID,
                            UserCNName = account.ToUserName,
                            UserENName = account.ToUserName,
                            PhotoUrl = "",
                        });

                        if (request.FromUserIdentifier != request.FromUserIdentifier)
                        {
                            db.ConversationRoomUids.Add(new ConversationRoomUid()
                            {
                                ConversationRoomID = ConversationRoomID,
                                Identifier = request.FromUserIdentifier,
                                UserType = request.FromUserType,
                                IsDeleted = false,
                                UserMemberID = request.FromUserMemberID,
                                CreateUserID = request.FromUserID,
                                UserID = request.FromUserID,
                                UserCNName = account.ToUserName,
                                UserENName = account.ToUserName,
                                PhotoUrl = "",
                            });
                        }

                        db.ConversationFriends.Add(new ConversationFriend()
                        {
                            ConversationRoomID = ConversationRoomID,
                            FromUserID = request.FromUserID,
                            FromUserIdentifier = request.FromUserIdentifier,
                            ToUserIdentifier = account.ToUserIdentifier,
                            ToUserID = account.ToUserID,
                            AddWording = account.AddWording,
                            CreateTime = DateTime.Now,
                            GroupName = account.GroupName,
                            Remark = account.Remark,
                            IsDeleted = false,

                            FriendID = Guid.NewGuid().ToString("N")
                        });

                        db.ConversationFriends.Add(new ConversationFriend()
                        {
                            ConversationRoomID = ConversationRoomID,
                            FromUserID = account.ToUserID,
                            FromUserIdentifier = account.ToUserIdentifier,
                            ToUserIdentifier = request.FromUserIdentifier,
                            ToUserID = request.FromUserID,
                            AddWording = account.AddWording,
                            CreateTime = DateTime.Now,
                            GroupName = account.GroupName,
                            Remark = account.Remark,
                            IsDeleted = false,

                            FriendID = Guid.NewGuid().ToString("N")
                        });

                        db.SaveChanges();

                        result.Add(account.ToUserIdentifier, channelInfo.Map<ConversationRoom, ResponseConversationRoomDTO>());
                    }
                    else
                    {
                        result.Add(account.ToUserIdentifier, channelInfo.Map<ConversationRoom, ResponseConversationRoomDTO>());
                    }
                });


            }

            return result;
        }

        /// <summary>
        /// 获取候诊统计
        /// 作者：郭明
        /// 日期：2017年7月11日
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ResponseGetWaitStatisticsDTO GetWaitingStatistics(RequestGetWaitStatisticsDTO request)
        {
            //任务优先级
            var TaskPriority = request.UserLevel;

            //处理一个任务需要消耗的时间（经验值）
            var TaskMinElapsedSeconds = 60 * 5;

            var result = new ResponseGetWaitStatisticsDTO()
            {
                ExpectedWaitTime = TaskMinElapsedSeconds
            };
            var grabOPDService = new SysGrabService<string>("UserOPDRegister");
            var today = DateTime.Now;
            int Year = today.Year;
            int Month = today.Month;
            int Day = today.Day;


            //查询指定医生需要等待的耗时
            if (!string.IsNullOrEmpty(request.DoctorID))
            {
                //通过数据库查询等待时间
                //TODO:需要从任务列表中获取，任务列表需要支持现在的排序规则
                var count = GetWaitingCount(request.DoctorID, request.ChannelID);
                result.ExpectedWaitTime = count * TaskMinElapsedSeconds;
            }
            else
            {
                //未领取的人数数量
                long unTakeTaskCount = 0;

                //获取当前医生所在的分组（可能多个）
                var doctorGroups = GetDoctorGroupIdListByDoctorID(request.DoctorID);

                //邱所有分组下，相同优先级的任务总数
                unTakeTaskCount = grabOPDService.TaskCount(TaskPriority, doctorGroups);

                var doctorTaskCountDict = new Dictionary<string, long>();

                //查询当前组的医生
                var doctorIdList = GetDoctorGroupMemberByUserLevel(request.UserLevel);

                //循环所有医生从任务列表中获取正在处理的订单数量
                doctorIdList.ForEach(a =>
                {
                    var doingTaskCount = grabOPDService.DoingTaskCount(a);
                    doctorTaskCountDict.Add(a, doingTaskCount);

                });

                //医生数量
                var doctorCount = doctorIdList.Count();

                //医生最少的人数数量（这就能够知道最闲的是谁）
                var doctorMinTaskCount = doctorCount > 0 ? doctorTaskCountDict.Min(a => a.Value) : 0;

                //当前组总任务数/当前组成员数量
                if (doctorCount > 0)
                {
                    result.ExpectedWaitTime = ((unTakeTaskCount / doctorCount) + doctorMinTaskCount) * TaskMinElapsedSeconds;
                }
                else
                {
                    result.ExpectedWaitTime = ((unTakeTaskCount) + doctorMinTaskCount) * TaskMinElapsedSeconds;
                }

            }

            return result;
        }

        /// <summary>
        /// 获取当前诊室的休诊状态
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public bool IsChannelDiagnoseOff(ResponseConversationRoomDTO room)
        {
            if (string.IsNullOrEmpty(room.DoctorID))
                return false;

            //获取医生配置
            List<ResponseConversationRoomDocConfDTO> doctorConfigs = conversationRoomRepository.GetDoctorConfigs(room.DoctorID);

            #region 判断医生当前休诊整体
            ConversationRoomDocDiagStateDTO state = new ConversationRoomDocDiagStateDTO { IsDiagnoseOff = false };

            // 获取休诊状态
            bool isDiagnoseOff = false;
            bool.TryParse(doctorConfigs.Where(x => x.ConfigType == EnumDoctorConfigType.DiagnoseOff)
                .Select(x => x.ConfigContent).FirstOrDefault(), out isDiagnoseOff);
            state.IsDiagnoseOff = isDiagnoseOff;

            if (!isDiagnoseOff)
                return false;

            // 获取休诊开始时间
            DateTime startTime = new DateTime();
            if (DateTime.TryParse(doctorConfigs.Where(x => x.ConfigType == EnumDoctorConfigType.DiagnoseOff_TimeStart)
                .Select(x => x.ConfigContent).FirstOrDefault(), out startTime))
                state.StartTime = startTime;

            // 获取休诊时长
            int duration = -1;
            if (int.TryParse(doctorConfigs.Where(x => x.ConfigType == EnumDoctorConfigType.DiagnoseOff_Duration)
                .Select(x => x.ConfigContent).FirstOrDefault(), out duration))
                state.Duration = duration;

            return state.IsDiagnoseOff && (state.EndTime.HasValue && DateTime.Now <= state.EndTime);
            #endregion

        }

        #region Command

        /// <summary>
        /// 发送候诊队列通知
        /// </summary>
        /// <param name="DoctorID">医生编号</param>
        /// <returns>发送通知的数量</returns>
        public int SendWaitingQueueChangeNotice(string DoctorID)
        {
            int result = -1;

            DateTime today = DateTime.Now;
            int Year = today.Year;
            int Month = today.Month;
            int Day = today.Day;


            var queue = conversationRoomRepository.GetWaitingChannels(DoctorID, 0);

            for (var i = 0; i < queue.Count; i++)
            {
                #region 通知其他候诊人员候诊人数有编号
                var DoctorUid = conversationIMUidRepository.GetUserIMUid(DoctorID);

                //发送实时消息
                if (imService.SendGroupCustomMsg(queue[i], DoctorUid, new RequestCustomMsgQueueChanged()
                {
                    Data = i,
                    Desc = "您前面有" + i + "位患者，请等待医生呼叫"
                }))
                {
                    result++;
                }
                #endregion
            }
            return result;
        }


        /// <summary>
        /// 批量将正在候诊状态的用户修改成离开
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public void Disconnection(string UserID)
        {
            var roomList = conversationRoomRepository.GetUserCurrentRooms(UserID);

            foreach (var room in roomList)
            {
                var ExpectedState = room.RoomState;

                CompareAndSetChannelState(room.ConversationRoomID, UserID, EnumRoomState.Disconnection, room.DisableWebSdkInteroperability, ref ExpectedState);
            }
        }


        /// <summary>
        /// 比较并设置房间状态
        /// </summary>
        /// <param name="ChannelID"></param>
        /// <param name="FromUserID"></param>
        /// <param name="State"></param>
        /// <param name="DisableWebSdkInteroperability"></param>
        /// <param name="ExpectedState"> 返回值</param>
        /// <returns></returns>
        public EnumApiStatus CompareAndSetChannelState(string ConversationRoomID, string FromUserID, EnumRoomState State, bool DisableWebSdkInteroperability, ref EnumRoomState ExpectedState)
        {

            try
            {
                var room = GetChannelInfo(ConversationRoomID);

                #region 校验：房间已标记为失效的，不可设置房间状态
                if (room.Close)
                {
                    return EnumApiStatus.BizChannelSetStateIfClose;
                }
                #endregion

                #region 校验：房间状态未启用
                if (!room.Enable)
                {
                    //频道未就绪（客户端重试）
                    return EnumApiStatus.BizChannelNotReady;
                }
                #endregion

                #region 校验：当前房间状态和预期的房间状态一致才允许设置

                #region 处理特殊状态情况 客户端没有WaitAgain 状态需要统一
                if (ExpectedState == EnumRoomState.Waiting && room.RoomState == EnumRoomState.WaitAgain)
                {
                    ExpectedState = room.RoomState;
                }
                #endregion

                if (room.RoomState != ExpectedState)
                {
                    //返回新的状态给客户端，客户端需要同步
                    ExpectedState = room.RoomState;

                    //前端用户只需要知道有候诊状态即可，不需要知道有重复候诊的状态
                    if (ExpectedState == EnumRoomState.WaitAgain)
                    {
                        ExpectedState = EnumRoomState.Waiting;
                    }

                    //当前状态不是预期状态
                    return EnumApiStatus.BizChannelRejectSetStateIfNotExpectedState;
                }
                #endregion

                #region 校验：只能在预约时间内进入诊室或提前30分钟进入诊室
                if ((State == EnumRoomState.WaitAgain || State == EnumRoomState.Waiting))
                {
                    if (DateTime.Now <= room.BeginTime.AddMinutes(-30))
                    {
                        return EnumApiStatus.BizChannelRejectConnectIfNoReservationTime;
                    }
                }
                #endregion

                #region 校验：医生在休诊状态下不可进入诊室
                if ((State == EnumRoomState.WaitAgain || State == EnumRoomState.Waiting))
                {
                    if (IsChannelDiagnoseOff(room))
                    {
                        return EnumApiStatus.BizChannelRejectConnectIfDiagnoseOff;
                    }
                }
                #endregion

                #region 房间状态切换的规则
                switch (room.RoomState)
                {
                    case EnumRoomState.AlreadyVisit:
                        {
                            //就诊已经结束不能在设置状态
                            return EnumApiStatus.BizOK;
                        }
                    case EnumRoomState.NoTreatment:
                        {
                            if (State == EnumRoomState.NoTreatment)
                            {
                                State = EnumRoomState.NoTreatment;
                            }
                            else if (State == EnumRoomState.InMedicalTreatment)
                            {
                                State = EnumRoomState.InMedicalTreatment;
                            }
                            else
                            {
                                State = EnumRoomState.Waiting;
                            }
                            break;
                        }
                    case EnumRoomState.Waiting:
                        {

                            //重试
                            if (State == EnumRoomState.Waiting || State == EnumRoomState.WaitAgain)
                            {
                                State = EnumRoomState.Waiting;
                            }
                            //医生呼叫
                            else if (State == EnumRoomState.Calling)
                            {
                                State = EnumRoomState.Calling;
                            }
                            //接听
                            else if (State == EnumRoomState.InMedicalTreatment)
                            {
                                State = EnumRoomState.InMedicalTreatment;
                            }
                            //候诊界面，用户点击离开或者异常断开都是未就诊
                            else
                            {
                                State = EnumRoomState.NoTreatment;
                            }
                            break;
                        }
                    case EnumRoomState.Calling:
                        {
                            //重试         
                            if (State == EnumRoomState.Calling)
                            {
                                State = EnumRoomState.Calling;
                            }
                            //接听
                            else if (State == EnumRoomState.InMedicalTreatment)
                            {
                                State = EnumRoomState.InMedicalTreatment;
                            }
                            //取消呼叫 或者拒绝
                            else if (State == EnumRoomState.Waiting || State == EnumRoomState.WaitAgain)
                            {
                                State = EnumRoomState.WaitAgain;
                            }
                            else
                            {
                                State = EnumRoomState.Disconnection;
                            }
                            break;
                        }
                    case EnumRoomState.InMedicalTreatment:
                        {
                            //重试
                            if (State == EnumRoomState.InMedicalTreatment)
                            {
                                State = EnumRoomState.InMedicalTreatment;
                            }
                            //医生挂断
                            else if (State == EnumRoomState.AlreadyVisit)
                            {
                                State = EnumRoomState.AlreadyVisit;
                            }
                            //重试候诊
                            else if (State == EnumRoomState.Waiting || State == EnumRoomState.WaitAgain)
                            {
                                State = EnumRoomState.WaitAgain;
                            }
                            //患者离开，一会再来
                            else
                            {
                                State = EnumRoomState.Disconnection;
                            }
                            break;
                        }
                    case EnumRoomState.Disconnection:
                        {
                            //医生挂断
                            if (State == EnumRoomState.AlreadyVisit)
                            {
                                State = EnumRoomState.AlreadyVisit;
                            }
                            //取消呼叫 或者拒绝
                            else if (State == EnumRoomState.Waiting || State == EnumRoomState.WaitAgain)
                            {
                                State = EnumRoomState.WaitAgain;
                            }
                            else
                            {
                                State = EnumRoomState.Disconnection;
                            }
                            break;
                        }
                    case EnumRoomState.WaitAgain:
                        {
                            //医生呼叫
                            if (State == EnumRoomState.Calling)
                            {
                                State = EnumRoomState.Calling;
                            }
                            //取消呼叫 或者拒绝
                            else if (State == EnumRoomState.Waiting || State == EnumRoomState.WaitAgain)
                            {
                                State = EnumRoomState.WaitAgain;
                            }
                            else
                            {
                                State = EnumRoomState.Disconnection;
                            }
                            break;
                        }
                }
                #endregion

                //默认高版本，当前禁用互通性（SDK直接可以互通的情况下）
                if (room.DisableWebSdkInteroperability)
                {
                    room.DisableWebSdkInteroperability = DisableWebSdkInteroperability;
                }

                room.RoomState = State;

                #region 推送状态变更消息，并修改状态
                using (MQChannel mqChannel = new MQChannel())
                {
                    mqChannel.BeginTransaction();
                    if (!mqChannel.Publish(new ChannelStateChangedEvent()
                    {
                        ChannelID = ConversationRoomID,
                        FromUserID = FromUserID,
                        State = State,
                        ExpectedState = ExpectedState,
                        DisableWebSdkInteroperability = room.DisableWebSdkInteroperability
                    }))
                    {
                        return EnumApiStatus.BizError;
                    }

                    if (conversationRoomRepository.UpdateChannelState(room.ConversationRoomID, room.RoomState))
                    {
                        mqChannel.Commit();
                        return EnumApiStatus.BizOK;
                    }
                    else
                    {
                        return EnumApiStatus.BizError;
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                LogHelper.DefaultLogger.Error(ex.Message, ex);
                //业务错误
                return EnumApiStatus.BizError;
            }
        }

        /// <summary>
        /// 更新房间状态
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public bool UpdateChannelState(ResponseConversationRoomDTO room)
        {
            return conversationRoomRepository.UpdateChannelState(room.ConversationRoomID, room.RoomState);
        }

        /// <summary>
        /// 获取最后的日志
        /// </summary>
        /// <param name="ConversationRoomID"></param>
        /// <param name="OperatorType"></param>
        /// <returns></returns>
        public ResponseConversationRoomLogDTO GetChannelLastLog(string ConversationRoomID, string OperatorType)
        {
            return conversationRoomRepository.GetChannelLastLog(ConversationRoomID, OperatorType);
        }

        /// <summary>
        /// 新增频道日志
        /// </summary>
        /// <param name="ConversationRoomID"></param>
        /// <param name="OperationUserID"></param>
        /// <param name="OperatorUserName"></param>
        /// <param name="OperatorType"></param>
        /// <param name="OperationDesc"></param>
        /// <param name="OperationRemark"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public bool InsertChannelLog(
            string ConversationRoomID,
            string OperationUserID,
            string OperatorUserName,
            string OperatorType,
            string OperationDesc = "",
            string OperationRemark = "")
        {
            return conversationRoomRepository.InsertChannelLog(ConversationRoomID, OperationUserID, OperatorUserName,
                OperatorType, OperationDesc, OperationRemark);
        }

        string Format(int duration)
        {
            TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(duration));
            string str = "";

            if (ts.Days > 0)
            {
                str += $"{ts.Days}天";
            }

            if (ts.Hours > 0)
            {
                str += $"{ts.Hours.ToString()}小时";
            }

            if (ts.Minutes > 0)
            {
                str += $"{ts.Minutes.ToString()}分钟";
            }

            if (ts.Seconds > 0)
            {
                str += $"{ts.Seconds}秒";
            }

            return str;
        }

        /// <summary>
        /// 重新开始计时
        /// </summary>
        /// <param name="ChannelID"></param>
        /// <returns></returns>
        public bool RestartCharging(string ConversationRoomID)
        {
            try
            {
                //获取房间信息
                var room = GetChannelInfo(ConversationRoomID);
                var doctorUser = GetChannelUsersInfo(ConversationRoomID).Find(a => a.UserType == EnumUserType.Doctor);

                #region 房间不存在或医生不存在则忽略
                if (room == null && doctorUser == null)
                {
                    return true;
                }
                #endregion

                #region 更新房间计费状态
                if (room.ChargingState != EnumRoomChargingState.Started)
                {
                    room.ChargingState = EnumRoomChargingState.Started;

                    if (!conversationRoomRepository.UpdateChannelChargeState(room.ConversationRoomID, room.ChargingState))
                    {
                        return false;
                    }
                }
                #endregion

                using (MQChannel mqChannel = new MQChannel())
                {
                    mqChannel.BeginTransaction();

                    #region 发布延时消息，15秒为一个周期。消费端收到消息后重新计算房间已通话时间。
                    if (!mqChannel.Publish<ChannelChargingEvent>(new ChannelChargingEvent()
                    {
                        ChannelID = ConversationRoomID,
                        Seq = room.ChargingSeq + 1,
                        ChargingTime = room.ChargingTime,
                        Interval = room.ChargingInterval
                    }))
                    {
                        return false;
                    }
                    #endregion

                    #region 发送服务时长变更消息
                    if (!mqChannel.Publish(new ChannelSendGroupMsgEvent<RequestCustomMsgRoomDurationChanged>()
                    {
                        Msg = new RequestCustomMsgRoomDurationChanged()
                        {
                            Data = new RequestConversationRoomStatusDTO()
                            {
                                ChannelID = room.ConversationRoomID,
                                State = room.RoomState,
                                ServiceID = room.ServiceID,
                                ServiceType = room.ServiceType,
                                DisableWebSdkInteroperability = room.DisableWebSdkInteroperability,
                                ChargingState = EnumRoomChargingState.Started,
                                Duration = room.Duration, //总时长
                                TotalTime = room.TotalTime// 消耗
                            },
                            Desc = $"服务计时已启动，总时长{Format(room.Duration)}, 剩余{Format(room.Duration - room.TotalTime)}"
                        },
                        ChannelID = room.ConversationRoomID,
                        FromAccount = doctorUser.identifier
                    }))
                    {

                        return false;
                    }
                    #endregion

                    mqChannel.Commit();

                    return true;
                }

            }
            catch (Exception ex)
            {
                LogHelper.DefaultLogger.Error(ex.Message, ex);
                return false;
            }
        }

        /// <summary>
        /// 暂停计时
        /// </summary>
        /// <param name="ChannelID"></param>
        /// <returns></returns>
        public bool PauseCharging(string ConversationRoomID)
        {
            try
            {
                //获取房间信息
                var room = GetChannelInfo(ConversationRoomID);

                //房间存在且状态不是暂停的则执行暂停操作（去重复）
                if (room != null && room.RoomState != EnumRoomState.AlreadyVisit && (room.ServiceType == EnumDoctorServiceType.AudServiceType || room.ServiceType == EnumDoctorServiceType.VidServiceType))
                {
                    var doctorUser = GetChannelUsersInfo(ConversationRoomID).Find(a => a.UserType == EnumUserType.Doctor);

                    if (doctorUser != null)
                    {
                        using (MQChannel channel = new MQChannel())
                        {
                            channel.BeginTransaction();

                            if (!channel.Publish(new ChannelSendGroupMsgEvent<RequestCustomMsgRoomDurationChanged>()
                            {
                                Msg = new RequestCustomMsgRoomDurationChanged()
                                {
                                    Data = new RequestConversationRoomStatusDTO()
                                    {
                                        ChannelID = room.ConversationRoomID,
                                        State = room.RoomState,
                                        ServiceID = room.ServiceID,
                                        ServiceType = room.ServiceType,
                                        DisableWebSdkInteroperability = room.DisableWebSdkInteroperability,
                                        ChargingState = EnumRoomChargingState.Paused,
                                        Duration = room.Duration, //总时长
                                        TotalTime = room.TotalTime// 消耗
                                    },
                                    Desc = $"服务计时已暂停，总时长{Format(room.Duration)}, 剩余{Format(room.Duration - room.TotalTime)}"
                                },
                                ChannelID = room.ConversationRoomID,
                                FromAccount = doctorUser.identifier
                            }))
                            {
                                return false;
                            }

                            room.ChargingState = EnumRoomChargingState.Paused;

                            if (conversationRoomRepository.UpdateChannelChargeState(room.ConversationRoomID, room.ChargingState))
                            {
                                channel.Commit();
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.DefaultLogger.Error(ex.Message, ex);
                return false;
            }

            return true;

        }

        /// <summary>
        /// 开始计时
        /// </summary>
        /// <param name="ChannelID"></param>
        /// <param name="Duration"></param>
        /// <param name="ServiceID"></param>
        /// <param name="OrderNo"></param>
        /// <returns></returns>
        public bool StartCharging(string ConversationRoomID, int Duration, string ServiceID, string OrderNo)
        {
            try
            {
                var room = GetChannelInfo(ConversationRoomID);

                #region 修改房间开始时间和已经消耗的时间
                //第一次进入诊室的时候
                if (room.TotalTime < 1)
                {
                    //room.BeginTime = DateTime.Now;
                    //room.TotalTime = 1;

                    if (!conversationRoomRepository.UpdateChannelChargeTime(ConversationRoomID, 1, DateTime.Now))
                    {
                        return false;
                    }
                }
                #endregion

                using (MQChannel mqChannel = new MQChannel())
                {
                    mqChannel.BeginTransaction();

                    //存在重复的请求，消费端需要去重复
                    if (!mqChannel.Publish(new ChannelDurationChangeEvent()
                    {
                        Duration = Duration, //套餐里面的服务时长单位是分钟需要转换成秒
                        ServiceID = ServiceID,
                        OrderNo = OrderNo,
                        NewUpgradeOrderNo = OrderNo
                    }))
                    {
                        return false;
                    }

                    //存在重复的请求，消费端需要去重复
                    //发布延时消息，15秒为一个周期。消费端收到消息后重新计算房间已通话时间。
                    if (!mqChannel.Publish(new ChannelChargingEvent()
                    {
                        ChannelID = ConversationRoomID,
                        Seq = 0,
                        ChargingTime = room.BeginTime,
                        Interval = 15
                    }))
                    {
                        return false;
                    }

                    room.ChargingState = EnumRoomChargingState.Started;

                    if (conversationRoomRepository.UpdateChannelChargeState(ConversationRoomID, EnumRoomChargingState.Started))
                    {
                        mqChannel.Commit();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.DefaultLogger.Error(ex.Message, ex);
                return false;
            }

        }

        /// <summary>
        /// 服务时间递增
        /// </summary>
        /// <param name="ServiceID">业务编号</param>
        /// <param name="Seconds">需要递增的服务时长，小于等于0意味着不限制服务时长（秒）</param>
        /// <returns></returns>
        public bool IncrementChannelDuration(string ConversationRoomID, string ServiceID, int Seconds, string OrderNo, string NewUpgradeOrderNo)
        {
            return conversationRoomRepository.IncrementChannelDuration(ConversationRoomID, ServiceID, Seconds, OrderNo, NewUpgradeOrderNo);
        }

        #endregion

    }
}



