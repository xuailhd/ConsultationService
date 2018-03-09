using HealthCloud.Common.Extensions;
using HealthCloud.Consultation.Dto.Common;
using HealthCloud.Consultation.Dto.EventBus;
using HealthCloud.Consultation.Dto.Request;
using HealthCloud.Consultation.Dto.Response;
using HealthCloud.Consultation.Enums;
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
    public class UserOPDRegisterRepository:BaseRepository
    {
        public bool ExistsWithSubmitRequest(RequestCheckTodaySubmitedDTO requst, out ResponseRepeatReturnDTO order, DBEntities db = null)
        {
            bool dbPrivateFlag = false;
            if (db == null)
            {
                db = CreateDb();
                dbPrivateFlag = true;
            }

            int Year = DateTime.Now.Year;
            int Month = DateTime.Now.Month;
            int Day = DateTime.Now.Day;

            ResponseRepeatReturnDTO opdModel = null;

            //未指定医生（医生领单的业务）
            if (string.IsNullOrEmpty(requst.DoctorID))
            {
                //有已经支付过，就诊未结束的就返回
                opdModel = (from opd in db.UserOPDRegisters.Where(a =>
                                 a.IsUseTaskPool
                                 && a.UserID == requst.UserID
                                 && a.MemberID == requst.MemberID
                                 && a.OPDType == requst.OPDType
                                 && (
                                     a.RegDate.Year == Year &&
                                     a.RegDate.Month == Month &&
                                     a.RegDate.Day == Day)
                                    && a.IsDeleted == false)
                            join room in db.ConversationRooms on opd.OPDRegisterID equals room.ServiceID
                            where (opd.OPDState != EnumOPDState.Completed && opd.OPDState != EnumOPDState.Canceled)
                            //排除已经关闭的订单
                            && !room.Close
                            orderby opd.PayTime descending
                            select new ResponseRepeatReturnDTO
                            {
                                OrderOutID = opd.OPDRegisterID,
                                OrderNo = opd.OrderNo,
                                OPDState = opd.OPDState,
                                ChannelID = room.ConversationRoomID,
                                DoctorID = opd.DoctorID,
                                Cancelable = (opd.OPDState == EnumOPDState.NoPay || opd.OPDState == EnumOPDState.NoReceive)
                            }).FirstOrDefault();
            }
            else
            {
                //如果有未完成或未取消的订单则返回
                opdModel = (from opd in db.UserOPDRegisters.Where(a =>
                                 !a.IsUseTaskPool
                                 && (a.DoctorID == requst.DoctorID)
                                 && a.UserID == requst.UserID
                                 && a.MemberID == requst.MemberID
                                 && a.OPDType == requst.OPDType
                                 && (
                                     a.RegDate.Year == Year &&
                                     a.RegDate.Month == Month &&
                                     a.RegDate.Day == Day)
                                    && a.IsDeleted == false)
                            join room in db.ConversationRooms on opd.OPDRegisterID equals room.ServiceID
                            where (opd.OPDState != EnumOPDState.Completed && opd.OPDState != EnumOPDState.Canceled)
                            //排除已经关闭的订单
                            && !room.Close
                            orderby opd.PayTime descending
                            select new ResponseRepeatReturnDTO
                            {
                                OrderOutID = opd.OPDRegisterID,
                                OrderNo = opd.OrderNo,
                                OPDState = opd.OPDState,
                                ChannelID = room.ConversationRoomID,
                                DoctorID = opd.DoctorID,
                                Cancelable = (opd.OPDState == EnumOPDState.NoPay || opd.OPDState == EnumOPDState.NoReceive
                                || opd.OPDState == EnumOPDState.NoReply)
                            }).FirstOrDefault();

            }

            if (dbPrivateFlag)
            {
                db.Dispose();
            }

            //已经预约了
            if (opdModel != null)
            {
                order = opdModel;
                return true;
            }
            else
            {
                order = null;
                return false;
            }
        }
    
        public bool CancelOpdRegister(string opdRegisterID, DBEntities db = null)
        {
            bool dbPrivateFlag = false;
            if (db == null)
            {
                db = CreateDb();
                dbPrivateFlag = true;
            }

            var opd = db.UserOPDRegisters.Where(t => t.OPDRegisterID == opdRegisterID).FirstOrDefault();


            var ret = true;
            if (opd != null)
            {
                opd.OPDState = EnumOPDState.Canceled;
            }
            else
            {
                ret = false;
            }

            if (dbPrivateFlag)
            {
                ret = db.SaveChanges()>0;
                db.Dispose();
            }

            return ret;
        }

        public ResponseUserOPDRegisterDTO Single(string OPDRegisterID, DBEntities db = null)
        {
            bool dbPrivateFlag = false;
            if (db == null)
            {
                db = CreateDb();
                dbPrivateFlag = true;
            }
            var dbmodel = db.UserOPDRegisters.Where(t => t.OPDRegisterID == OPDRegisterID).FirstOrDefault();

            ResponseUserOPDRegisterDTO model = null;

            if (dbmodel != null)
            {
                model = dbmodel.Map<UserOPDRegister, ResponseUserOPDRegisterDTO>();


                model.Room = (from m in db.ConversationRooms
                              where m.ServiceID == OPDRegisterID && m.IsDeleted == false
                              select m).FirstOrDefault().Map<ConversationRoom, ResponseConversationRoomDTO>();


                #region 用户病历
                model.UserMedicalRecord = (from m in db.UserMedicalRecords
                                           where m.OPDRegisterID == OPDRegisterID && m.IsDeleted == false
                                           orderby m.CreateTime descending
                                           select new ResponseUserMedicalRecordDTO()
                                           {

                                               Advised = m.Advised,
                                               AllergicHistory = m.AllergicHistory,
                                               ConsultationSubject = m.ConsultationSubject,
                                               Description = m.Description,
                                               FamilyMedicalHistory = m.FamilyMedicalHistory,
                                               PastMedicalHistory = m.PastMedicalHistory,
                                               PastOperatedHistory = m.PastOperatedHistory,
                                               PreliminaryDiagnosis = m.PreliminaryDiagnosis,
                                               PresentHistoryIllness = m.PresentHistoryIllness,
                                               Sympton = m.Sympton,
                                               UserMedicalRecordID = m.UserMedicalRecordID,

                                           }).FirstOrDefault();

                #endregion

                #region 附件
                IQueryable<UserFile> queryAttachFiles = null;

                queryAttachFiles = (from m in db.UserFiles
                                    where !m.IsDeleted && m.OutID == model.OPDRegisterID
                                    select m);


                if (queryAttachFiles != null)
                {
                    model.AttachFiles = queryAttachFiles.Where(i => i.FileType == 0).Select(m => new ResponseUserFileDTO
                    {
                        FileID = m.FileID,
                        FileName = m.FileName,
                        FileType = m.FileType,
                        FileUrl = m.FileUrl,
                        OutID = m.OutID,
                        Remark = m.Remark
                    }).ToList();
                }
                #endregion

                #region 医生信息
                if (!string.IsNullOrEmpty(model.DoctorID))
                {
                    model.Doctor = new ResponseDoctorInfoDTO()
                    {
                        DoctorName = dbmodel.DoctorName,
                    };

                }
                #endregion

                #region 患者信息
                if (!string.IsNullOrEmpty(model.MemberID))
                {
                    model.Member = new ResponseUserMemberDTO()
                    {
                        MemberID = model.MemberID,
                        MemberName = model.MemberName,
                        Gender = model.Gender,
                        Birthday = model.Birthday,
                        Mobile = model.Mobile
                    };
                }
                #endregion



                #region 分诊信息
                var triageModel = db.DoctorTriages.Where(i => i.OPDRegisterID == model.OPDRegisterID).Select(i => new ResponseDoctorTriageDTO
                {
                    OPDRegisterID = i.OPDRegisterID,
                    TriageDoctorID = i.TriageDoctorID,
                    TriageStatus = i.TriageStatus,
                    IsToGuidance = i.IsToGuidance
                }).FirstOrDefault();

                if (triageModel == null)
                {
                    triageModel = new ResponseDoctorTriageDTO
                    {
                        OPDRegisterID = model.OPDRegisterID,
                        TriageDoctorID = model.DoctorID,
                        TriageStatus = EnumTriageStatus.Triaged,
                        IsToGuidance = false
                    };
                }
                model.DoctorTriage = triageModel;
                #endregion

            }

            if (dbPrivateFlag)
            {
                db.Dispose();
            }

            return model;
        }

        public bool AddOpdRegister(RequestUserOPDRegisterSubmitDTO request, DBEntities db = null)
        {
            bool dbPrivateFlag = false;
            if (db == null)
            {
                db = CreateDb();
                dbPrivateFlag = true;
            }

            request.OPDRegisterID = Guid.NewGuid().ToString("N");

            #region 新增预约记录
            UserOPDRegister model = new UserOPDRegister()
            {
                IsUseTaskPool = request.IsUseTaskPool,
                CreateTime = DateTime.Now,
                ScheduleID = request.ScheduleID,
                DeleteTime = DateTime.Now,
                ModifyTime = DateTime.Now,
                IsDeleted = false,
                DoctorGroupID = string.IsNullOrEmpty(request.DoctorGroupID) ? "" : request.DoctorGroupID,
                CreateUserID = request.UserID,
                OrgnazitionID = request.OrgnazitionID,
                OPDType = request.OPDType,
                MemberID = request.MemberID,
                CostType  =request.CostType,
                RegDate = DateTime.Now,
                OPDBeginTime = request.OPDBeginTime,//服务预约时间段开始
                OPDEndTime = request.OPDEndTime,//服务预约时间端结束
                Fee = request.ServicePrice,//服务价格
                DoctorID = string.IsNullOrEmpty(request.DoctorID) ? "" : request.DoctorID,//医生编号
                DoctorName = string.IsNullOrEmpty(request.DoctorName) ? "" : request.DoctorName,
                PhotoUrl = string.IsNullOrEmpty(request.PhotoUrl) ? "": request.PhotoUrl,
                RenewFee = request.RenewFee,
                MedicalCardNumber = request.MedicalCardID,
                Flag = request.Flag,
                DoctorPhotoUrl = string.IsNullOrEmpty(request.DoctorPhotoUrl) ? "" : request.DoctorPhotoUrl,
                UserAccount = request.UserAccount,
                OPDDate = request.OPDDate,//预约日期
                OPDRegisterID = request.OPDRegisterID,
                UserID = request.UserID,
                ConsultContent = string.IsNullOrEmpty(request.ConsultContent) ? "" : request.ConsultContent,
                ConsultDisease = string.IsNullOrEmpty(request.ConsultDisease) ? "" : request.ConsultDisease,
                MemberName = request.MemberName,
                Gender = request.Gender,
                Marriage = request.Marriage,
                Age = request.Age,
                IDNumber = string.IsNullOrEmpty(request.IDNumber) ? "" : request.IDNumber,
                IDType = request.IDType,
                Mobile = request.Mobile,
                Address = request.Address,
                Birthday = request.Birthday
            };

            db.UserOPDRegisters.Add(model);
            #endregion

            var ret = true;

            if (dbPrivateFlag)
            {
                db.Dispose();
                ret = db.SaveChanges()>0;
            }
            return ret;
        }

        public bool AddConversationRoom(RequestUserOPDRegisterSubmitDTO request, DBEntities db = null)
        {
            bool dbPrivateFlag = false;
            if (db == null)
            {
                db = CreateDb();
                dbPrivateFlag = true;
            }

            #region 创建房间
            request.ChannelID = Guid.NewGuid().ToString("N");
            var room = new ConversationRoom()
            {
                EndTime = DateTime.Parse(request.OPDDate.ToString("yyyy-MM-dd ") + request.OPDEndTime),
                BeginTime = DateTime.Parse(request.OPDDate.ToString("yyyy-MM-dd ") + request.OPDBeginTime),
                TotalTime = 0,
                RoomState = EnumRoomState.NoTreatment,//状态
                ConversationRoomID = request.ChannelID,
                ServiceID = request.OPDRegisterID,
                ServiceType = request.OPDType,
                //如果预约类型是挂号那么房间类型就是线下看诊，否则是线上看诊
                RoomType = EnumRoomType.Group,
                TriageID = long.MaxValue,
                Priority = request.UserLevel,
                DisableWebSdkInteroperability = true,
            };
            
            db.ConversationRooms.Add(room);
            #endregion

            var ret = true;

            if (dbPrivateFlag)
            {
                db.Dispose();
                ret = db.SaveChanges() > 0;
            }
            return ret;
        }

        /// <summary>
        /// 看诊完成
        /// </summary>
        /// <param name="serviceID"></param>
        /// <returns></returns>
        public bool OPDComplete(string serviceID)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["KMEHospEntities"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    @"update UserOPDRegisters set OPDState=@DoctorID,ModifyUserID=@DoctorID,ModifyTime=@ModifyTime 
                                                where UserOPDRegisters.OPDRegisterID=@OPDRegisterID
                      update ConversationRooms set RoomState=@RoomState,ModifyTime=@ModifyTime  
                        where ConversationRooms.ServiceID=@OPDRegisterID  ", conn);

                cmd.Parameters.Add("@OPDState", SqlDbType.Int).Value = (int)EnumOPDState.Completed;
                cmd.Parameters.Add("@RoomState", SqlDbType.Int).Value = (int)EnumRoomState.AlreadyVisit;
                cmd.Parameters.Add("@ModifyTime", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@OPDRegisterID", SqlDbType.VarChar).Value = serviceID;

                var count = cmd.ExecuteNonQuery();
                return count > 0;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="opdRegisterID"></param>
        /// <returns></returns>
        public bool Delete(string opdRegisterID, DBEntities db = null)
        {
            bool dbPrivateFlag = false;
            if (db == null)
            {
                db = CreateDb();
                dbPrivateFlag = true;
            }

            var model = db.UserOPDRegisters.Where(t => t.OPDRegisterID == opdRegisterID).FirstOrDefault();

            if (model != null)
            {
                model.IsDeleted = true;
            }
            else
            {
                if (dbPrivateFlag)
                {
                    db.Dispose();
                }
                return true;
            }

            var ret = true;

            if (dbPrivateFlag)
            {
                db.Dispose();
                ret = db.SaveChanges() > 0;
            }
            return ret;

        }

        /// <summary>
        /// 获取制定咨询记录
        /// </summary>
        /// <param name="ServiceID"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public ResponseTaskDTO GetTask(string ServiceID, DBEntities db = null)
        {
            bool dbPrivateFlag = false;
            if (db == null)
            {
                db = CreateDb();
                dbPrivateFlag = true;
            }

            PagedList<List<ResponseTaskDTO>> response = new PagedList<List<ResponseTaskDTO>>();
            //查询预约患者，不显示已就诊的患者

            var query = from opd in db.UserOPDRegisters.Where(a => a.OPDRegisterID == ServiceID)
                                                join room in db.ConversationRooms on opd.OPDRegisterID equals room.ServiceID
                                                //join uid in db.ConversationIMUids on opd.UserID equals uid.UserID into leftJoinUid
                                                //from uidIfEmpty in leftJoinUid.DefaultIfEmpty()
                                                //join message in db.ConversationMessages.GroupBy(a => a.ConversationRoomID) on room.ChannelID equals message.Key into leftJoinMessage
                                                //from messageIfEmpty in leftJoinMessage.DefaultIfEmpty()
                                                select new ResponseTaskDTO
                                                {
                                                    RegDate = opd.RegDate,
                                                    OPDDate = opd.OPDDate,
                                                    ConsultContent = opd.ConsultContent,
                                                    Address = opd.Address,
                                                    Age = opd.Age,
                                                    Birthday = opd.Birthday,
                                                    DoctorID = opd.DoctorID,
                                                    DoctorName = opd.DoctorName,
                                                    Gender = opd.Gender,
                                                    IDNumber = opd.IDNumber,
                                                    IDType = opd.IDType,
                                                    Marriage = opd.Marriage,
                                                    MemberID = opd.MemberID,
                                                    MemberName = opd.MemberName,
                                                    Mobile = opd.Mobile,
                                                    OPDRegisterID = opd.OPDRegisterID,
                                                    RoomState = room.RoomState,
                                                    OPDType = opd.OPDType,
                                                    UserID = opd.UserID
                                                };

            var ret = query.FirstOrDefault();

            if (dbPrivateFlag)
            {
                db.Dispose();
            }

            return ret;
        }

        /// <summary>
        /// 获取咨询我的记录
        /// </summary>
        /// <param name="request"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public PagedList<ResponseTaskDTO> GetTaskList(RequestQueryTaskDTO request, DBEntities db = null)
        {
            PagedList<ResponseTaskDTO> response = new PagedList<ResponseTaskDTO>();
            bool dbPrivateFlag = false;
            if (db == null)
            {
                db = CreateDb();
                dbPrivateFlag = true;
            }
            //查询预约患者，不显示已就诊的患者

            //TODO：待优化成存储过程
            IQueryable<ResponseTaskDTO> query = null;

            query = from opd in db.UserOPDRegisters.Where(a => !a.IsDeleted).Where(a => a.DoctorID == request.DoctorID)
                    join room in db.ConversationRooms.Where(a => !a.IsDeleted) on opd.OPDRegisterID equals room.ServiceID
                    join uid in db.ConversationIMUids on opd.UserID equals uid.UserID into leftJoinUid
                    from uidIfEmpty in leftJoinUid.DefaultIfEmpty()
                    join message in db.ConversationMessages.GroupBy(a => a.ConversationRoomID) on room.ConversationRoomID equals message.Key into leftJoinMessage
                    from messageIfEmpty in leftJoinMessage.DefaultIfEmpty()
                        //orderby
                        //房间状态 -1 其它，0 未就诊，1 候诊中，2 就诊中，3 已就诊，4 呼叫中，5 离开中，6 断开连接
                        //房间状态排序：呼叫中、就诊中、候诊中、未就诊、（其他状态）
                        //((int)room.RoomState >= 0 && (int)room.RoomState <= 2 ? (int)room.RoomState : -1) descending,
                        //room.Priority ascending,
                        //room.TriageID ascending
                    select new ResponseTaskDTO
                    {
                        RegDate = opd.RegDate,
                        OPDDate = opd.OPDDate,
                        ConsultContent = opd.ConsultContent,
                        Address = opd.Address,
                        Age = opd.Age,
                        Birthday = opd.Birthday,
                        DoctorID = opd.DoctorID,
                        DoctorName = opd.DoctorName,
                        Gender = opd.Gender,
                        IDNumber = opd.IDNumber,
                        IDType = opd.IDType,
                        Marriage = opd.Marriage,
                        MemberID = opd.MemberID,
                        MemberName = opd.MemberName,
                        Mobile = opd.Mobile,
                        OPDRegisterID = opd.OPDRegisterID,
                        RoomState = room.RoomState,
                        OPDType = opd.OPDType,
                        UserID = opd.UserID,
                        MedicalCardNumber = opd.MedicalCardNumber,
                        PayTime = opd.PayTime,
                        Priority = room.Priority,
                        ChannelID = room.ConversationRoomID,
                        TriageID = room.TriageID
                    };
           
            if (request.OrderType.HasValue)
            {
                switch (request.OrderType.Value)
                {
                    case EnumRecordOrderType.OPDDate:
                        query = query.OrderByDescending(x => x.OPDDate);
                        break;
                    case EnumRecordOrderType.OrderTime:
                        query = query.OrderByDescending(x => x.PayTime);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(x => (int)x.RoomState >= 0 && (int)x.RoomState <= 2 ? (int)x.RoomState : -1).
                    ThenBy(x => x.Priority).ThenBy(x => x.TriageID);
            }

            //开始日期
            if (!string.IsNullOrEmpty(request.BeginDate))
            {
                DateTime beginTime = Convert.ToDateTime(request.BeginDate + " 00:00:00");
                query = query.Where(opd => opd.OPDDate >= beginTime);
            }

            if (!string.IsNullOrEmpty(request.EndDate))
            {
                DateTime endTime = Convert.ToDateTime(request.EndDate + " 23:59:59");
                query = query.Where(opd => opd.OPDDate <= endTime);
            }

            //房间状态
            if (request.RoomState != null && request.RoomState.Count > 0)
            {
                query = query.Where(opd => request.RoomState.Contains(opd.RoomState));
            }

            //服务类型
            if (request.OPDType != null && request.OPDType.Count > 0)
            {
                query = query.Where(opd => request.OPDType.Contains(opd.OPDType));
            }

            //会员编号
            if (!string.IsNullOrEmpty(request.MemberID))
            {
                query = query.Where(opd => opd.MemberID == request.MemberID);
            }

            //关键字
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                if (request.ResponseFilters.Count <= 0)
                {
                    query = query.Where(opd => opd.MemberName.Contains(request.Keyword) ||
                        opd.MedicalCardNumber.Contains(request.Keyword));
                }
                else
                {
                    query = query.Where(opd => opd.MemberName.Contains(request.Keyword));
                }
            }

            response = query.ToPagedList(request.CurrentPage,request.PageSize);

            // 合并是否已开处方的状态
            //if (request.IncludeDiagnoseStatus.HasValue && request.IncludeDiagnoseStatus.Value)
            //{
            //    List<string> medicalRecords = db.UserMedicalRecords.Where(x => opdRegisterIDs.Contains(x.OPDRegisterID)).Select(x => x.OPDRegisterID).ToList();
            //    response.Data.Where(x => medicalRecords.Contains(x.Room.ServiceID)).ToList().ForEach(x =>
            //    {
            //        x.IsDiagnosed = true;
            //    });
            //}

            // 合并已签名处方数
            //if (request.IncludeRecipeSignedCount.HasValue && request.IncludeRecipeSignedCount.Value)
            //{
            //    var recipeFiles = db.DoctorRecipeFiles.Where(x =>
            //        opdRegisterIDs.Contains(x.OPDRegisterID) && x.RecipeFileStatus == EnumRecipeFileStatus.Signed).GroupBy(x => x.OPDRegisterID).Select(x => new
            //        {
            //            OPDRegisterID = x.Key,
            //            RecipeSignedCount = x.Count()
            //        }).ToList();
            //    var ids = recipeFiles.Select(x => x.OPDRegisterID).ToList();

            //    response.Data = (from data in response.Data
            //                     join recipe in recipeFiles on data.Room.ServiceID equals recipe.OPDRegisterID into recipeLeftJoin
            //                     from recipeIfEmpty in recipeLeftJoin.DefaultIfEmpty()
            //                     select new
            //                     {
            //                         Data = data,
            //                         RecipeSignedCount = recipeIfEmpty == null ? 0 : recipeIfEmpty.RecipeSignedCount
            //                     }).Select(x =>
            //                     {
            //                         x.Data.RecipeSignedCount = x.RecipeSignedCount;
            //                         return x.Data;
            //                     }).ToList();

            //}

            if (dbPrivateFlag)
            {
                db.Dispose();
            }

            return response;

        }

        /// <summary>
        /// 领取问题 更新数据库，并发控制
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool AcceptTaskDB(DoctorAcceptEvent args)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["KMEHospEntities"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(@"update UserOPDRegisters set DoctorID=@DoctorID,ModifyUserID=@DoctorID,ModifyTime=@ModifyTime 
                                                where UserOPDRegisters.OPDRegisterID=@OPDRegisterID");

                cmd.Parameters.Add("@DoctorID", SqlDbType.VarChar).Value = args.DoctorID;
                cmd.Parameters.Add("@ModifyTime", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@OPDRegisterID", SqlDbType.VarChar).Value = args.ServiceID;

                int count = cmd.ExecuteNonQuery();
                return count>0;
            }
        }


        public List<string> GetDoctorGroupIdList(DBEntities db = null)
        {
            bool dbPrivateFlag = false;
            if (db == null)
            {
                db = CreateDb();
                dbPrivateFlag = true;
            }

            var query = from opd in db.UserOPDRegisters
                        where !opd.IsDeleted && (opd.OPDType == EnumDoctorServiceType.VidServiceType || opd.OPDType == EnumDoctorServiceType.AudServiceType)
                        && opd.IsUseTaskPool && (string.IsNullOrEmpty(opd.DoctorID) || opd.DoctorID == null) && opd.OPDState == EnumOPDState.NoReceive
                        group opd by opd.DoctorGroupID into gro
                        select gro.Key;


            var ret = query.ToList();

            if (dbPrivateFlag)
            {
                db.Dispose();
            }

            for(int i= ret.Count - 1; i >= 0; i--)
            {
                if(string.IsNullOrWhiteSpace(ret[i]))
                {
                    ret.RemoveAt(i);
                }
            }
            return ret;
        }


        /// <summary>
        /// 获取音视频待领取任务列表
        /// </summary>
        /// <param name="DoctorID"></param>
        /// <returns></returns>
        public List<ResponseTaskPriority> GetWaitVideoList(DBEntities db = null)
        {
            bool dbPrivateFlag = false;
            if (db == null)
            {
                db = CreateDb();
                dbPrivateFlag = true;
            }

            var list = (from opd in db.UserOPDRegisters.Where(a => !a.IsDeleted && string.IsNullOrEmpty(a.DoctorID) 
                        && (a.OPDType == EnumDoctorServiceType.VidServiceType || a.OPDType == EnumDoctorServiceType.AudServiceType) )
                        join room in db.ConversationRooms on opd.OPDRegisterID equals room.ServiceID
                        where opd.IsUseTaskPool && opd.OPDState == EnumOPDState.NoPay
                        orderby room.TriageID ascending, room.CreateTime ascending
                        select new ResponseTaskPriority()
                        {
                            Priority = room.Priority,
                            ServiceID = opd.OPDRegisterID,
                            DoctorGroupID = string.IsNullOrEmpty(opd.DoctorGroupID) ? "ALL" : opd.DoctorGroupID
                        }).ToList();

            if (dbPrivateFlag)
            {
                db.Dispose();
            }

            return list;

        }

        /// <summary>
        /// 获取咨询待领取任务列表
        /// </summary>
        /// <param name="DoctorID"></param>
        /// <returns></returns>
        public List<ResponseTaskPriority> GetWaitConsultList(DBEntities db = null)
        {
            bool dbPrivateFlag = false;
            if (db == null)
            {
                db = CreateDb();
                dbPrivateFlag = true;
            }

            var list = (from opd in db.UserOPDRegisters.Where(a => !a.IsDeleted && string.IsNullOrEmpty(a.DoctorID)
                        && a.OPDType == EnumDoctorServiceType.PicServiceType)
                        join room in db.ConversationRooms on opd.OPDRegisterID equals room.ServiceID
                        where opd.IsUseTaskPool && opd.OPDState == EnumOPDState.NoPay
                        orderby room.TriageID ascending, room.CreateTime ascending
                        select new ResponseTaskPriority()
                        {
                            Priority = room.Priority,
                            ServiceID = opd.OPDRegisterID,
                            DoctorGroupID = string.IsNullOrEmpty(opd.DoctorGroupID) ? "ALL" : opd.DoctorGroupID
                        }).ToList();

            if (dbPrivateFlag)
            {
                db.Dispose();
            }

            return list;

        }


        /// <summary>
        /// 获取视频咨询已经领取数量
        /// </summary>
        /// <param name="DoctorID"></param>
        /// <returns></returns>
        public List<string> GetVideoConsultFinishedList(string DoctorID, DBEntities db = null)
        {
            bool dbPrivateFlag = false;
            if (db == null)
            {
                db = CreateDb();
                dbPrivateFlag = true;
            }

            var Year = DateTime.Now.Year;
            var Month = DateTime.Now.Month;
            var Day = DateTime.Now.Day;


            var list = (from opd in db.UserOPDRegisters.Where(a => a.DoctorID == DoctorID &&
                         (a.OPDType == EnumDoctorServiceType.VidServiceType || a.OPDType == EnumDoctorServiceType.AudServiceType) &&
                         a.OPDDate.Year == Year &&
                         a.OPDDate.Month == Month &&
                         a.OPDDate.Day == Day)
                        join room in db.ConversationRooms on opd.OPDRegisterID equals room.ServiceID
                        where room.RoomState == EnumRoomState.AlreadyVisit && opd.IsUseTaskPool
                        orderby room.TriageID ascending, room.CreateTime ascending
                        select opd.OPDRegisterID).ToList();

            if (dbPrivateFlag)
            {
                db.Dispose();
            }

            return list;
        }

        /// <summary>
        /// 获取视频咨询已经领取数量
        /// </summary>
        /// <param name="DoctorID"></param>
        /// <returns></returns>
        public List<string> GetVideoConsultDoingList(string DoctorID, DBEntities db = null)
        {
            bool dbPrivateFlag = false;
            if (db == null)
            {
                db = CreateDb();
                dbPrivateFlag = true;
            }

            var Year = DateTime.Now.Year;
            var Month = DateTime.Now.Month;
            var Day = DateTime.Now.Day;

            var list = (from opd in db.UserOPDRegisters.Where(a => a.DoctorID == DoctorID &&
                         (a.OPDType == EnumDoctorServiceType.VidServiceType || a.OPDType == EnumDoctorServiceType.AudServiceType) &&
                         a.OPDDate.Year == Year &&
                         a.OPDDate.Month == Month &&
                         a.OPDDate.Day == Day)
                        join room in db.ConversationRooms on opd.OPDRegisterID equals room.ServiceID
                        where new List<EnumRoomState>() {
                                EnumRoomState.NoTreatment,
                                EnumRoomState.WaitAgain,
                                EnumRoomState.Waiting,
                                EnumRoomState.Calling,
                                EnumRoomState.InMedicalTreatment,
                                EnumRoomState.Disconnection}.Contains(room.RoomState)
                            && opd.IsUseTaskPool
                        orderby room.TriageID ascending, room.CreateTime ascending
                        select opd.OPDRegisterID).ToList();

            if (dbPrivateFlag)
            {
                db.Dispose();
            }

            return list;
        }


        /// <summary>
        /// 获取图文咨询已经领取数量
        /// <param name="DoctorID"></param>
        /// <returns></returns>
        public List<string> GetTextConsultFinishedList(string DoctorID, DBEntities db = null)
        {
            bool dbPrivateFlag = false;
            if (db == null)
            {
                db = CreateDb();
                dbPrivateFlag = true;
            }

            var Year = DateTime.Now.Year;
            var Month = DateTime.Now.Month;
            var Day = DateTime.Now.Day;


            var list = (from opd in db.UserOPDRegisters.Where(a => a.DoctorID == DoctorID &&
                         a.OPDType == EnumDoctorServiceType.PicServiceType &&
                         a.FinishTime.Value.Year == Year &&
                         a.FinishTime.Value.Month == Month &&
                         a.FinishTime.Value.Day == Day)
                        join room in db.ConversationRooms on opd.OPDRegisterID equals room.ServiceID
                        where room.RoomState == EnumRoomState.AlreadyVisit && opd.IsUseTaskPool
                        orderby room.TriageID ascending, room.CreateTime ascending
                        select opd.OPDRegisterID).ToList();

            if (dbPrivateFlag)
            {
                db.Dispose();
            }

            return list;
        }

        /// <summary>
        /// 获取图文咨询已经领取数量
        /// </summary>
        /// <param name="DoctorID"></param>
        /// <returns></returns>
        public List<string> GetTextConsultDoingList(string DoctorID, DBEntities db = null)
        {
            bool dbPrivateFlag = false;
            if (db == null)
            {
                db = CreateDb();
                dbPrivateFlag = true;
            }

            var Year = DateTime.Now.Year;
            var Month = DateTime.Now.Month;
            var Day = DateTime.Now.Day;


            var list = (from opd in db.UserOPDRegisters.Where(a => a.DoctorID == DoctorID &&
                           a.AcceptTime.Value.Year == Year &&
                           a.AcceptTime.Value.Month == Month &&
                           a.AcceptTime.Value.Day == Day)
                        join room in db.ConversationRooms on opd.OPDRegisterID equals room.ServiceID
                        where opd.IsUseTaskPool
                        orderby room.TriageID ascending, room.CreateTime ascending
                        select opd.OPDRegisterID).ToList();

            if (dbPrivateFlag)
            {
                db.Dispose();
            }

            return list;
        }


        public ResponseDoctorTriageDTO GetDoctorTriageDetail(string OPDRegisterID, DBEntities db = null)
        {
            bool dbPrivateFlag = false;
            if (db == null)
            {
                db = CreateDb();
                dbPrivateFlag = true;
            }

            var model = (from triage in db.DoctorTriages.Where(t => t.OPDRegisterID == OPDRegisterID && !t.IsDeleted)
                         select new ResponseDoctorTriageDTO()
                         {
                             OPDRegisterID = triage.OPDRegisterID,
                             IsToGuidance = triage.IsToGuidance,
                             TriageDoctorID = triage.TriageDoctorID,
                             TriageStatus = triage.TriageStatus
                         }).FirstOrDefault();

            if (dbPrivateFlag)
            {
                db.Dispose();
            }

            return model;
        }
    }
}
