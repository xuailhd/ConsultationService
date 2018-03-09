using HealthCloud.Common.Cache;
using HealthCloud.Common.EventBus;
using HealthCloud.Common.Log;
using HealthCloud.Consultation.Dto.Common;
using HealthCloud.Consultation.Dto.EventBus;
using HealthCloud.Consultation.Dto.Exceptions;
using HealthCloud.Consultation.Dto.Request;
using HealthCloud.Consultation.Dto.Response;
using HealthCloud.Consultation.Enums;
using HealthCloud.Consultation.ICaches.Keys;
using HealthCloud.Consultation.Repositories;
using HealthCloud.Consultation.Repositories.EF;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthCloud.Consultation.Services
{

    public class DoctorTaskService
    {
        static SysGrabService<string> grabOPDService = new SysGrabService<string>("UserOPDRegister");
        static SysGrabService<string> grabConsultService = new SysGrabService<string>("UserConsult");

        static readonly UserOPDRegisterRepository userOPDRegisterRepository;


        static DoctorTaskService()
        {
            userOPDRegisterRepository = new UserOPDRegisterRepository();
        }


        /// <summary>
        /// 领取问题（盲领）
        /// </summary>
        /// <param name="AcceptDoctorID"></param>
        /// <returns></returns>
        public ApiResult AcceptTextConsult(string AcceptDoctorID,List<string> groupList)
        {
            if (string.IsNullOrEmpty(AcceptDoctorID))
            {
                return EnumApiStatus.BizError.ToApiResultForApiStatus();
            }

            //初始化已经领取列表
            SetTextConsultDoingList(AcceptDoctorID);
            SetTextConsultFinishedList(AcceptDoctorID);

            var UserConsultID = "";

            //获取医生分组
            //var groupList = doctorService.GetDoctorGroupIdListByDoctorID(AcceptDoctorID);

            var takeResult = grabConsultService.TakeTask(AcceptDoctorID, out UserConsultID, groupList);

            //领取了任务
            if (!string.IsNullOrEmpty(UserConsultID))
            {
                var task = userOPDRegisterRepository.GetTask(UserConsultID);

                if (task != null)
                {
                    using (MQChannel mqChannel = new MQChannel())
                    {
                        if (mqChannel.Publish(new DoctorAcceptEvent()
                        {
                            DoctorID = AcceptDoctorID,
                            ServiceID = task.OPDRegisterID,
                            ServiceType = task.OPDType,
                            ChannelID = task.ChannelID,
                            UserID = task.UserID,
                            UserMemberID = task.MemberID
                        }))
                        {
                            return task.ToApiResultForObject(takeResult);
                        }
                    }
                }
            }

            return takeResult.ToApiResultForApiStatus();
        }


        /// <summary>
        /// 领取问题（盲领）
        /// 作者：郭明
        /// 日期：2017年5月18日
        /// </summary>
        /// <param name="AcceptDoctorID"></param>
        /// <returns></returns>
        public ApiResult AcceptVideo(string DoctorID, List<string> groupList)
        {
            if (string.IsNullOrEmpty(DoctorID))
            {
                return EnumApiStatus.BizError.ToApiResultForApiStatus();
            }

            //设置识破正在处理的列表
            SetVideoConsultDoingList(DoctorID);
            //设置视频已经完成列表
            SetVideoConsultFinishedList(DoctorID);

            var OPDRegisterID = "";

            //获取医生分组
            var takeResult = grabOPDService.TakeTask(DoctorID, out OPDRegisterID, groupList);

            if (!string.IsNullOrEmpty(OPDRegisterID))
            {
                var task = userOPDRegisterRepository.GetTask(OPDRegisterID);

                if (task != null)
                {
                    using (MQChannel mqChannel = new MQChannel())
                    {
                        if (mqChannel.Publish<DoctorAcceptEvent>(new DoctorAcceptEvent()
                        {
                            DoctorID = DoctorID,
                            ServiceID = task.OPDRegisterID,
                            ServiceType = task.OPDType,
                            ChannelID = task.ChannelID,
                            UserID = task.UserID,
                            UserMemberID = task.MemberID
                        }))
                        {
                            return task.ToApiResultForObject(takeResult);
                        }
                    }
                }
            }

            return takeResult.ToApiResultForApiStatus();


        }

        /// <summary>
        /// 领取问题完成（盲领
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool AcceptVideoCompleted(DoctorAcceptEvent args)
        {
            if (userOPDRegisterRepository.AcceptTaskDB(args))
            {
                grabOPDService.StartTask(args.ServiceID, args.DoctorID);
            }
            else
            {
                grabOPDService.InvalidTask(args.ServiceID, args.DoctorID);
                throw new TaskConcurrentTakeException();
            }
            return true;
        }

        /// <summary>
        /// 领取完成
        /// 作者：郭明
        /// 日期：2017年5月18日
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool AcceptTextConsultCompletd(DoctorAcceptEvent args)
        {
            if (userOPDRegisterRepository.AcceptTaskDB(args))
            {
                grabConsultService.StartTask(args.ServiceID, args.DoctorID);
            }
            else
            {
                grabConsultService.InvalidTask(args.ServiceID, args.DoctorID);
                throw new TaskConcurrentTakeException();
            }
            return true;
        }

        /// <summary>
        /// 获取语音视频统计
        /// </summary>
        /// <param name="DoctorID"></param>
        public ResponseTaskStatisticsDTO GetAcceptStatistics(string DoctorID,List<string> groups)
        {
            var result = new ResponseTaskStatisticsDTO();

            result.VideoConsultTotalCount = grabOPDService.TaskSum(groups);
            result.VideoConsultAlreadyCount = grabOPDService.DoingTaskCount(DoctorID) + grabOPDService.FinishedTaskCount(DoctorID);
            result.TextConsultTotalCount = grabConsultService.TaskSum(groups);
            result.TextConsultAlreadyCount = grabConsultService.DoingTaskCount(DoctorID) + grabConsultService.FinishedTaskCount(DoctorID);
            return result;
        }

        /// <summary>
        /// 获取咨询我的记录
        /// <param name="doctorUid"></param>
        /// <param name="MemberName"></param>
        /// <param name="PageSize"></param>
        /// <param name="CurrentPage"></param>
        /// <param name="Status"></param>
        /// <param name="BeginDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="OPDType">预约类型（0-挂号、1-图文、2-语音、3-视频,-1-不区分类型）</param>
        /// <returns></returns>
        public PagedList<ResponseTaskDTO> GetTaskList(RequestQueryTaskDTO request)
        {
            return userOPDRegisterRepository.GetTaskList(request);
        }

        /// <summary>
        /// 获取未处理任务统计（未回复图文咨询以及候诊中音视频问诊数量）
        /// </summary>
        /// <param name="doctorID"></param>
        public ResponseUntreatedStatisticsDTO GetUntreatedStatistics(string doctorID)
        {
            ResponseUntreatedStatisticsDTO result = new ResponseUntreatedStatisticsDTO();

            using (var db = new DBEntities())
            {

                var query = from opd in db.UserOPDRegisters.Where(a => !a.IsDeleted)
                            join triage in db.DoctorTriages.Where(a => a.TriageDoctorID == doctorID && a.TriageStatus == EnumTriageStatus.Triaged) on opd.OPDRegisterID equals triage.OPDRegisterID
                            join room in db.ConversationRooms.Where(a => !a.IsDeleted) on opd.OPDRegisterID equals room.ServiceID
                            where opd.OPDState != EnumOPDState.Canceled && opd.OPDState!= EnumOPDState.NoReceive
                            select new
                            {
                                RegDate = opd.RegDate,
                                OPDDate = opd.OPDDate,
                                OPDState = opd.OPDState,
                                ServiceType = room.ServiceType,
                                RoomState = room.RoomState
                            };

                DateTime beginTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
                DateTime endTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd 23:59:59"));

                result.TextConsultUnRepliedCount = query.Where(x => x.ServiceType == EnumDoctorServiceType.PicServiceType &&
                    (x.RoomState != EnumRoomState.InMedicalTreatment && x.RoomState != EnumRoomState.AlreadyVisit)).Count();

                result.VideoConsultWaitingCount = query.Where(x => (x.ServiceType == EnumDoctorServiceType.AudServiceType || x.ServiceType == EnumDoctorServiceType.VidServiceType) &&
                    (x.RoomState == EnumRoomState.Waiting || x.RoomState == EnumRoomState.WaitAgain) &&
                    (x.OPDDate >= beginTime && x.OPDDate <= endTime)).Count();

                return result;
            }
        }

        /// <summary>
        /// 重设任务列表
        /// </summary>
        public void ResetTaskList()
        {

            //没有列表
            SetVideoConsultTaskList();
            //没有列表
            SetTextConsultTaskList();
        }

        #region Private
 
        static void SetTextConsultTaskList()
        {
            var cacheKey = new StringCacheKey(StringCacheKeyType.Sys_TaskListResetTime, "Video");

            //补库存需要小心前面任务没有处理的情况
            if (!cacheKey.FromCache<DateTime?>().HasValue)
            {
                var LockName = $"{nameof(SetTextConsultTaskList)}";
                var lockValue = Guid.NewGuid().ToString("N");

                if (LockName.Lock(lockValue,TimeSpan.FromSeconds(10)))
                {
                    try
                    {
                        //医生分组会比较多
                        var allDoctorGroups = userOPDRegisterRepository.GetDoctorGroupIdList();

                        //获取所有任务队列状态
                        var taskGroupQueueStatus = grabConsultService.TaskListEmpty(allDoctorGroups);

                        //查询出所有待处理的图文咨询记录编号和对应的优先级
                        var allList = userOPDRegisterRepository.GetWaitConsultList();

                        foreach (var status in taskGroupQueueStatus)
                        {
                            var taskGroupID = status.Key;
                            var taskQueueStatus = status.Value;

                            //如果其中一个队列是空的
                            if (taskQueueStatus.Any(a => !a.Value))
                            {
                                foreach (var taskQueue in taskQueueStatus)
                                {
                                    //当前任务列表没有初始化
                                    if (!taskQueue.Value)
                                    {
                                        //根据队列的优先级匹配相关的咨询记录
                                        var list = allList.Where(userConsult =>
                                        (userConsult.DoctorGroupID.ToUpper() == taskGroupID.ToUpper()) &&
                                        userConsult.Priority == taskQueue.Key).ToList();

                                        foreach (var userConsult in list)
                                        {
                                            grabConsultService.DispatchTask(userConsult.ServiceID, userConsult.Priority, userConsult.DoctorGroupID);
                                        }
                                    }
                                }
                            }
                        }
                        DateTime.Now.ToCache(cacheKey);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.DefaultLogger.Error(ex.Message, ex);
                    }
                    finally
                    {
                        LockName.UnLock(lockValue);
                    }
                }

            }

        }

        static void SetVideoConsultTaskList()
        {
            var cacheKey = new StringCacheKey(StringCacheKeyType.Sys_TaskListResetTime, "Video");

            //补库存需要小心前面任务没有处理的情况
            if (!cacheKey.FromCache<DateTime?>().HasValue)
            {
                var LockName = $"{nameof(SetVideoConsultTaskList)}";

                var lockValue = Guid.NewGuid().ToString("N");

                if (LockName.Lock(lockValue, TimeSpan.FromSeconds(10)))
                {
                    try
                    {
                        //医生分组会比较多
                        var allDoctorGroups = userOPDRegisterRepository.GetDoctorGroupIdList();

                        //获取所有医生分组的任务状态
                        var taskGroupQueueStatus = grabOPDService.TaskListEmpty(allDoctorGroups);

                        //查询出所有待处理的图文咨询记录编号和对应的优先级
                        var allList = userOPDRegisterRepository.GetWaitVideoList();

                        //循环处理医生分组的情况
                        foreach (var groupTaskStatus in taskGroupQueueStatus)
                        {
                            var taskGroupID = groupTaskStatus.Key;
                            var taskQueueStatus = groupTaskStatus.Value;

                            //如果其中一个队列是空的
                            if (taskQueueStatus.Any(a => !a.Value))
                            {
                                foreach (var taskQueue in taskQueueStatus)
                                {
                                    //当前任务列表没有初始化
                                    if (!taskQueue.Value)
                                    {
                                        //根据队列的优先级匹配相关的咨询记录
                                        var list = allList.Where(opd => (
                                        opd.DoctorGroupID.ToUpper() == taskGroupID.ToUpper()) && opd.Priority == taskQueue.Key).ToList();

                                        foreach (var opd in list)
                                        {
                                            grabOPDService.DispatchTask(opd.ServiceID, opd.Priority, opd.DoctorGroupID);
                                        }
                                    }
                                }
                            }

                        }

                        DateTime.Now.ToCache(cacheKey);

                    }
                    catch (Exception ex)
                    {
                        LogHelper.DefaultLogger.Error(ex.Message, ex);
                    }
                    finally
                    {
                        LockName.UnLock(lockValue);
                    }
                }

            }
        }

        /// <summary>
        /// 设置图文咨询已经领取列表
        /// 作者：郭明 
        /// 日期：2017年5月19日
        /// </summary>
        /// <param name="DoctorID"></param>
        void SetTextConsultDoingList(string DoctorID)
        {
            //初始化已经领取列表
            if (grabConsultService.DoingTaskListEmpty(DoctorID))
            {
                var LockName = $"{nameof(SetTextConsultDoingList)}:{DoctorID}";

                var lockValue = Guid.NewGuid().ToString("N");

                if (LockName.Lock(lockValue, TimeSpan.FromSeconds(10)))
                {
                    try
                    {

                        var list = userOPDRegisterRepository.GetTextConsultDoingList(DoctorID);

                        list.ForEach(item =>
                        {
                            grabConsultService.StartTask(item, DoctorID);
                        });
                    }
                    catch (Exception ex)
                    {
                        LogHelper.DefaultLogger.Error(ex.Message, ex);
                    }
                    finally
                    {
                        LockName.UnLock(lockValue);
                    }

                }
            }
        }

        /// <summary>
        /// 设置图文咨询已经领取列表
        /// 作者：郭明 
        /// 日期：2017年5月19日
        /// </summary>
        /// <param name="DoctorID"></param>
        void SetTextConsultFinishedList(string DoctorID)
        {

            //初始化已经领取列表
            if (grabConsultService.FinishedTaskListEmpty(DoctorID))
            {
                var LockName = $"{nameof(SetTextConsultFinishedList)}:{DoctorID}";

                var lockValue = Guid.NewGuid().ToString("N");

                if (LockName.Lock(lockValue, TimeSpan.FromSeconds(10)))
                {
                    try
                    {
                        var list = userOPDRegisterRepository.GetTextConsultFinishedList(DoctorID);
                        list.ForEach(item =>
                        {
                            grabConsultService.FinishTask(item, DoctorID);
                        });
                    }
                    catch (Exception ex)
                    {
                        LogHelper.DefaultLogger.Error(ex.Message, ex);
                    }
                    finally
                    {
                        LockName.UnLock(lockValue);
                    }
                }

            }

        }

        /// <summary>
        /// 设置识破咨询已经领取列表
        /// 作者：郭明
        /// 日期：2017年5月19日
        /// </summary>
        /// <param name="DoctorID"></param>
        void SetVideoConsultDoingList(string DoctorID)
        {
            //初始化已领取的列表
            if (grabOPDService.DoingTaskListEmpty(DoctorID))
            {
                var LockName = $"{nameof(SetVideoConsultDoingList)}:{DoctorID}";

                var lockValue = Guid.NewGuid().ToString("N");
                if (LockName.Lock(lockValue, TimeSpan.FromSeconds(10)))
                {
                    try
                    {
                        var list = userOPDRegisterRepository.GetVideoConsultDoingList(DoctorID);

                        list.ForEach(item =>
                        {
                            grabOPDService.StartTask(item, DoctorID);

                        });

                    }
                    catch (Exception ex)
                    {
                        LogHelper.DefaultLogger.Error(ex.Message, ex);
                    }
                    finally
                    {
                        LockName.UnLock(lockValue);
                    }

                }
            }

        }

        /// <summary>
        /// 设置识破咨询已经领取列表
        /// 作者：郭明
        /// 日期：2017年5月19日
        /// </summary>
        /// <param name="DoctorID"></param>
        void SetVideoConsultFinishedList(string DoctorID)
        {
            //初始化已领取的列表
            if (grabOPDService.FinishedTaskListEmpty(DoctorID))
            {
                var LockName = $"{nameof(SetVideoConsultFinishedList)}:{DoctorID}";

                var lockValue = Guid.NewGuid().ToString("N");

                if (LockName.Lock(lockValue, TimeSpan.FromSeconds(10)))
                {
                    try
                    {
                        var list = userOPDRegisterRepository.GetVideoConsultFinishedList(DoctorID);

                        list.ForEach(item =>
                        {
                            grabOPDService.FinishTask(item, DoctorID);

                        });
                    }
                    catch (Exception ex)
                    {
                        LogHelper.DefaultLogger.Error(ex.Message, ex);
                    }
                    finally
                    {
                        LockName.UnLock(lockValue);
                    }

                }
            }

        }


        
        #endregion
    }
}
