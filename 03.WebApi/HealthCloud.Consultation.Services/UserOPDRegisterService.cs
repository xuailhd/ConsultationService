using HealthCloud.Common.EventBus;
using HealthCloud.Common.Extensions;
using HealthCloud.Common.Log;
using HealthCloud.Consultation.Common.Config;
using HealthCloud.Consultation.Dto.Common;
using HealthCloud.Consultation.Dto.EventBus;
using HealthCloud.Consultation.Dto.Request;
using HealthCloud.Consultation.Dto.Response;
using HealthCloud.Consultation.Enums;
using HealthCloud.Consultation.IServices;
using HealthCloud.Consultation.Models;
using HealthCloud.Consultation.Repositories;
using HealthCloud.Consultation.Repositories.EF;
using HealthCloud.Consultation.Services.QQCloudy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Services
{
    public class UserOPDRegisterService
    {
        private readonly UserOPDRegisterRepository userOPDRegisterRepository;
        private readonly ConversationRoomRepository conversationRoomRepository;
        private readonly IIMHepler imservice;

        public UserOPDRegisterService()
        {
            userOPDRegisterRepository = new UserOPDRegisterRepository();
            conversationRoomRepository = new ConversationRoomRepository();
            imservice = new IMHelper();
        }

        /// <summary>
        /// 获取预约列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PagedList<ResponseUserOPDRegisterDTO> GetPageList(RequestQueryOPDRegisterDTO request)
        {
            var today = DateTime.Now;

            using (var db = new DBEntities())
            {
                string queryPredicate = "IsDeleted = @0 AND (OPDType = @1 OR OPDType = @2)";
                List<object> paramValues = new List<object> { false, EnumDoctorServiceType.VidServiceType, EnumDoctorServiceType.AudServiceType };

                if (!string.IsNullOrEmpty(request.MemberID))
                {
                    if (!string.IsNullOrEmpty(request.IDNumber))
                    {
                        queryPredicate += " AND IDNumber = @3";
                        paramValues.Add(request.IDNumber);
                    }
                    else
                    {
                        queryPredicate += " AND MemberID = @3";
                        paramValues.Add(request.MemberID);
                    }
                }
                else
                {
                    queryPredicate += " AND UserID = @3";
                    paramValues.Add(request.UserID);
                }

                IQueryable<ResponseUserOPDRegisterDTO> query = null;
                // 过滤排班与处方明细，app不需要用到提高查询效率
                if (request.FilterRecipeAndSchedule.HasValue && request.FilterRecipeAndSchedule.Value)
                {
                    // 指定用户条件筛选，建议在UserOpdRegisters表Where条件进行
                    query = from opd in db.UserOPDRegisters.Where(queryPredicate, paramValues.ToArray())
                            join room in db.ConversationRooms.Where(a => a.IsDeleted == false) on opd.OPDRegisterID equals room.ServiceID into leftJoinRoom
                            from roomIfEmpty in leftJoinRoom.DefaultIfEmpty()
                            select new ResponseUserOPDRegisterDTO()
                            {
                                OPDRegisterID = opd.OPDRegisterID,//预约编号
                                OPDDate = opd.OPDDate,//排版日期
                                OPDType = opd.OPDType,//预约类型
                                RegDate = opd.RegDate,//预约时间                                                    
                                UserID = opd.UserID,//用户编号                             
                                Fee = opd.Fee,//费用
                                ConsultContent = opd.ConsultContent,
                                IDNumber = opd.IDNumber,
                                MemberID = opd.MemberID,
                                Age = opd.Age,
                                OPDState = opd.OPDState,
                                Room = new ResponseConversationRoomDTO()
                                {
                                    //就诊当天，没有就诊，用户已经支付
                                    ConversationRoomID = roomIfEmpty != null && (opd.OPDState == EnumOPDState.Completed ||
                                    (opd.OPDState != EnumOPDState.NoPay &&
                                    (opd.OPDDate.Year == today.Year &&
                                    opd.OPDDate.Month == today.Month &&
                                    opd.OPDDate.Day == today.Day))
                                    ) ? roomIfEmpty.ConversationRoomID : null,//就诊房间
                                    RoomState = roomIfEmpty != null ? roomIfEmpty.RoomState : EnumRoomState.NoTreatment,//预约状态
                                    Secret = roomIfEmpty != null ? roomIfEmpty.Secret : "",//房间密码
                                },
                                OrderNo = opd.OrderNo,
                                Member = new ResponseUserMemberDTO()
                                {
                                    UserID = opd.UserID,
                                    MemberID = opd.MemberID,//成员编号
                                    MemberName = opd.MemberName,
                                    Gender = opd.Gender,
                                    IDNumber = opd.IDNumber,
                                    Age = opd.Age
                                },
                                Doctor = new ResponseDoctorInfoDTO()
                                {

                                    DoctorID = opd.DoctorID,//医生编号
                                    DoctorName = opd.DoctorName,
                                }
                            };
                }
                else
                {
                    // 指定用户条件筛选，建议在UserOpdRegisters表Where条件进行
                    query = from opd in db.UserOPDRegisters.Where(queryPredicate, paramValues.ToArray())
                            join emr in db.UserMedicalRecords.Where(a => !a.IsDeleted) on opd.OPDRegisterID equals emr.OPDRegisterID into leftJoinEmr
                            from emrIfEmpty in leftJoinEmr.DefaultIfEmpty()
                            join room in db.ConversationRooms.Where(a => a.IsDeleted == false) on opd.OPDRegisterID equals room.ServiceID into leftJoinRoom
                            from roomIfEmpty in leftJoinRoom.DefaultIfEmpty()
                            select new ResponseUserOPDRegisterDTO
                            {
                                OPDRegisterID = opd.OPDRegisterID,//预约编号
                                OPDDate = opd.OPDDate,//排版日期
                                OPDType = opd.OPDType,//预约类型
                                RegDate = opd.RegDate,//预约时间                                                    
                                UserID = opd.UserID,//用户编号                             
                                Fee = opd.Fee,//费用
                                ConsultContent = opd.ConsultContent,
                                IDNumber = opd.IDNumber,
                                MemberID = opd.MemberID,
                                Age = opd.Age,
                                OPDState = opd.OPDState,
                                Room = new ResponseConversationRoomDTO()
                                {
                                    //就诊当天，没有就诊，用户已经支付
                                    ConversationRoomID = roomIfEmpty != null && (opd.OPDState == EnumOPDState.Completed ||
                                    (opd.OPDState != EnumOPDState.NoPay &&
                                    (opd.OPDDate.Year == today.Year &&
                                    opd.OPDDate.Month == today.Month &&
                                    opd.OPDDate.Day == today.Day))
                                    ) ? roomIfEmpty.ConversationRoomID : null,//就诊房间
                                    RoomState = roomIfEmpty != null ? roomIfEmpty.RoomState : EnumRoomState.NoTreatment,//预约状态
                                    Secret = roomIfEmpty != null ? roomIfEmpty.Secret : "",//房间密码
                                },
                                OrderNo = opd.OrderNo,
                                Member = new ResponseUserMemberDTO()
                                {
                                    UserID = opd.UserID,
                                    MemberID = opd.MemberID,//成员编号
                                    MemberName = opd.MemberName,
                                    Gender = opd.Gender,
                                    IDNumber = opd.IDNumber,
                                    Age = opd.Age
                                },
                                Doctor = new ResponseDoctorInfoDTO()
                                {

                                    DoctorID = opd.DoctorID,//医生编号
                                    DoctorName = opd.DoctorName,
                                },
                                IsExistMedicalRecord = emrIfEmpty != null
                            };
                }

                #region 处理搜索条件
                //查询关键字
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    query = query.Where(a => a.Member.MemberName.Contains(request.Keyword));
                }

                //开始日期
                if (request.BeginDate.HasValue)
                {
                    query = query.Where(a => a.OPDDate >= request.BeginDate);
                }

                //结束日期
                if (request.EndDate.HasValue)
                {
                    request.EndDate = request.EndDate.Value.AddDays(1);
                    query = query.Where(a => a.OPDDate < request.EndDate);
                }


                //类型
                if (request.OPDType.HasValue)
                {
                    query = query.Where(a => a.OPDType == request.OPDType.Value);
                }
                //状态
                if (request.OPDState.HasValue)
                {
                    query = query.Where(a => a.OPDState == request.OPDState.Value);
                }

                #endregion

                query = query.OrderByDescending(a => a.OPDBeginTime);

                var ret = query.ToPagedList(request.CurrentPage, request.PageSize);

                var ids = (from m in ret select m.OPDRegisterID).ToList();

                #region 已分诊的订单，预约医生信息改为分诊医生信息
                var triagedDoctor = (from triage in db.DoctorTriages.Where(i => i.TriageStatus == EnumTriageStatus.Triaged)
                                     join id in ids on triage.OPDRegisterID equals id
                                     select new ResponseUserOPDRegisterDTO
                                     {
                                         OPDRegisterID = triage.OPDRegisterID,
                                         Doctor = new ResponseDoctorInfoDTO
                                         {
                                             DoctorID = triage.TriageDoctorID,//医生编号
                                             DoctorName = triage.TriageDoctorName,
                                         }

                                     }).ToList();

                triagedDoctor.ForEach(triaged =>
                {
                    var opd = ret.Where(j => j.OPDRegisterID == triaged.OPDRegisterID).First();
                    opd.Doctor = triaged.Doctor;
                });

                #endregion

                return ret;
            }
        }

        /// <summary>
        /// 获取我咨询的记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PagedList<ResponseUserConsultDTO> GetConsultedPageList(RequestUserConsultsQueryDTO request)
        {
            if (request.EndDate.HasValue)
                request.EndDate = request.EndDate.Value.AddDays(1);


            using (DBEntities db = new DBEntities())
            {
                var query = from opd in db.UserOPDRegisters.Where(a => !a.IsDeleted) 
                            join triage in db.DoctorTriages on opd.OPDRegisterID equals triage.OPDRegisterID into leftTriageMid
                            from triageLeft in leftTriageMid.DefaultIfEmpty()
                            join room in db.ConversationRooms on opd.OPDRegisterID equals room.ServiceID //用户可以看到自己取消的订单，所以不应该加.Where(a => !a.IsDeleted)                 
                            join doctorUid in db.ConversationIMUids on opd.DoctorID equals doctorUid.UserID into leftJoinDocUid     //免费咨询 医生为空
                            from DocUidIfEmpty in leftJoinDocUid.DefaultIfEmpty()
                            join userUid in db.ConversationIMUids on opd.UserID equals userUid.UserID
                            join message in db.ConversationMessages.Where(a => a.IsDeleted == false).GroupBy(a => a.ConversationRoomID) 
                            on room.ConversationRoomID equals message.Key into leftJoinMessage
                            from messageIfEmpty in leftJoinMessage.DefaultIfEmpty()
                            join emr in db.UserMedicalRecords.Where(a => !a.IsDeleted) on opd.OPDRegisterID equals emr.OPDRegisterID into leftJoinEmr
                            from emrIfEmpty in leftJoinEmr.DefaultIfEmpty()
                            orderby opd.OPDBeginTime descending,opd.OPDState
                            select new ResponseUserConsultDTO()
                            {
                                Doctor = new ResponseDoctorInfoDTO
                                {
                                    DoctorID = opd.DoctorID,
                                    DoctorName = opd.DoctorName,
                                },
                                Room = new ResponseConversationRoomDTO()
                                {
                                    ServiceType = room.ServiceType,
                                    RoomState = room.RoomState,
                                    ConversationRoomID = room.ConversationRoomID,
                                    Secret = room.Secret,
                                    Duration = room.Duration,
                                    TotalTime = room.TotalTime,
                                    Enable = room.Enable,
                                    Priority = room.Priority,
                                },
                                UserMember = new ResponseUserMemberDTO()
                                {
                                    MemberID = opd.MemberID,
                                    MemberName = opd.MemberName,
                                    Gender = opd.Gender,
                                    IDNumber = opd.IDNumber,
                                },
                                UserID = opd.UserID,
                                InquiryType = opd.InquiryType,
                                ConsultContent = opd.ConsultContent,
                                ConsultTime = opd.RegDate,
                                AnswerTime = opd.AnswerTime,
                                FinishTime = opd.FinishTime,
                                OPDRegisterID = opd.OPDRegisterID,
                                IsExistMedicalRecord = emrIfEmpty != null,
                                TriageStatus = triageLeft == null ? EnumTriageStatus.Triaged : triageLeft.TriageStatus,
                                OPDState = opd.OPDState,
                                //取最后一条聊天记录
                                Messages = messageIfEmpty.OrderByDescending(a => new { a.MessageTime, a.ConversationMessageID }).Take(1)
                                .Select(a => new ResponseConversationMsgDTO
                                {
                                    MessageContent = a.MessageContent,
                                    MessageState = a.MessageState,
                                    MessageTime = a.MessageTime,
                                    MessageType = a.MessageType
                                }
                                ).ToList()
                            };

                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    query = query.Where(i => i.ConsultContent.Contains(request.Keyword));
                }

                if (request.OPDState.HasValue)
                {
                    query = query.Where(i => i.OPDState == request.OPDState.Value);
                }
                if (request.BeginDate.HasValue)
                {
                    query = query.Where(i => i.ConsultTime >= request.BeginDate);
                }
                if (request.EndDate.HasValue)
                {
                    query = query.Where(i => i.ConsultTime <= request.EndDate);
                }
                if (!string.IsNullOrEmpty(request.MemberName))
                {
                    query = query.Where(i => i.UserMember.MemberName.Contains(request.MemberName));
                }

                //传了MemberID，查memberid 的身份号的所有记录
                if (!string.IsNullOrEmpty(request.MemberID))
                {
                    if (!string.IsNullOrEmpty(request.IDNumber))
                    {
                        query = query.Where(t => t.UserMember.IDNumber == request.IDNumber);
                    }
                    else
                    {
                        query = query.Where(t => t.UserMember.MemberID == request.MemberID);
                    }
                }
                else
                {
                    query = query.Where(t => t.UserID == request.UserID);
                }

                if (request.IsPayed == true)
                {
                    query = query.Where(i => i.OPDState != EnumOPDState.NoPay);
                }

                var ret = query.ToPagedList(request.CurrentPage, request.PageSize);

                if (ret != null)
                {
                    var ids = (from record in ret select record.OPDRegisterID).ToList();

                    #region 已分诊的订单，预约医生信息改为分诊医生信息
                    var triagedDoctor = (from triage in db.DoctorTriages.Where(i => i.TriageStatus == EnumTriageStatus.Triaged)
                                         join id in ids on triage.OPDRegisterID equals id
                                         select new ResponseUserConsultDTO
                                         {
                                             OPDRegisterID = triage.OPDRegisterID,
                                             Doctor = new  ResponseDoctorInfoDTO
                                             {
                                                 DoctorID = triage.TriageDoctorID,//医生编号
                                                 DoctorName = triage.TriageDoctorName,
                                             }

                                         }).ToList();

                    triagedDoctor.ForEach(triaged =>
                    {
                        var opd = ret.Where(j => j.OPDRegisterID == triaged.OPDRegisterID).First();
                        opd.Doctor = triaged.Doctor;
                    });

                    #endregion
                }
                return ret;
            }
        }

        /// <summary>
        /// 获取咨询我的记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PagedList<ResponseUserConsultDTO> GetConsultMePageList(RequestUserConsultsQueryDTO request)
        {
            if (request.EndDate.HasValue)
                request.EndDate = request.EndDate.Value.AddDays(1);

            using (DBEntities db = new DBEntities())
            {
                var query = from opd in db.UserOPDRegisters.Where(a => a.DoctorID == request.CurrentOperatorDoctorID)
                            join triage in db.DoctorTriages.Where(i => i.TriageStatus == EnumTriageStatus.Triaged) on opd.OPDRegisterID equals triage.OPDRegisterID
                            join room in db.ConversationRooms.Where(a => !a.IsDeleted) on opd.OPDRegisterID equals room.ServiceID
                            join uid in db.ConversationIMUids.Where(a => !a.IsDeleted) on opd.UserID equals uid.UserID
                            //需要查询出已经删除的订单，未付款的订单不需要在医生端显示
                            join message in db.ConversationMessages.Where(a => !a.IsDeleted).GroupBy(a => a.ConversationRoomID) on room.ConversationRoomID equals message.Key into leftJoinMessage
                            from messageIfEmpty in leftJoinMessage.DefaultIfEmpty()
                            join userfile in db.UserFiles.Where(a => !a.IsDeleted).GroupBy(a => a.OutID) on opd.OPDRegisterID equals userfile.Key into leftJoinUserFiles
                            from userfileIfEmpty in leftJoinUserFiles.DefaultIfEmpty()
                            select new ResponseUserConsultDTO()
                            {
                                #region DTO操作
                                Doctor = new  ResponseDoctorInfoDTO
                                {
                                    DoctorID = opd.DoctorID,
                                    DoctorName = opd.DoctorName,
                                },
                                Room = new ResponseConversationRoomDTO()
                                {
                                    ChargingState = room.ChargingState,
                                    ServiceType = room.ServiceType,
                                    RoomState = room.RoomState,
                                    ConversationRoomID = room.ConversationRoomID,
                                    Secret = room.Secret,
                                    Duration = room.Duration,
                                    TotalTime = room.TotalTime,
                                    Enable = room.Enable,
                                    Priority = room.Priority
                                },
                                UserMember = new ResponseUserMemberDTO()
                                {
                                    MemberID = opd.MemberID,
                                    MemberName = opd.MemberName,
                                    Gender = opd.Gender,
                                },
                                //取最后一条聊天记录
                                Messages = messageIfEmpty.OrderByDescending(a => new { a.MessageTime, a.ConversationMessageID }).Take(1)
                                .Select(a => new ResponseConversationMsgDTO
                                {

                                    MessageContent = a.MessageContent,
                                    MessageState = a.MessageState,
                                    MessageTime = a.MessageTime,
                                    MessageType = a.MessageType

                                }).ToList(),
                                UserFiles = userfileIfEmpty.OrderBy(a => a.CreateTime).Take(3).Select(a => new ResponseUserFileDTO
                                {
                                    FileUrl = a.FileUrl,

                                }).ToList(),
                                InquiryType = opd.InquiryType,
                                ConsultContent = opd.ConsultContent,
                                OPDState = opd.OPDState,
                                ConsultTime = opd.RegDate,
                                FinishTime = opd.FinishTime,
                                OPDRegisterID = opd.OPDRegisterID,
                                AnswerTime = opd.AnswerTime,
                                PayTime = opd.PayTime,
                                #endregion
                            };

                if (request.OrderType.HasValue)
                {
                    switch (request.OrderType.Value)
                    {
                        case EnumRecordOrderType.OPDDate:
                            query = query.OrderByDescending(x => x.ConsultTime);
                            break;
                        case EnumRecordOrderType.OrderTime:
                            query = query.OrderByDescending(x => x.PayTime);
                            break;
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.Room.Priority).ThenBy(x => x.OPDState).ThenByDescending(x => x.ConsultTime);
                }


                if (request.BeginDate.HasValue)
                {
                    query = query.Where(a => a.ConsultTime >= request.BeginDate);
                }

                if (request.EndDate.HasValue)
                {
                    query = query.Where(a => a.ConsultTime < request.EndDate);
                }

                if (request.InquiryType.HasValue)
                {
                    query = query.Where(a => a.InquiryType == request.InquiryType);
                }

                if (request.OPDState.HasValue)
                {
                    query = query.Where(a => a.OPDState == request.OPDState && a.OPDState != EnumOPDState.NoPay);

                }

                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    query = query.Where(a => a.UserMember.MemberName.Contains(request.Keyword));
                }

                var ret = query.ToPagedList(request.CurrentPage, request.PageSize);
                return ret;
            }
        }

        /// <summary>
        /// 查询医生的咨询记录
        /// </summary>
        /// <param name="SelectType">-1：免费，义诊；0：免费；1：义诊；2：付费，套餐，会员，家庭医生</param>
        /// <param name="PageIndex">页码</param>
        /// <param name="PageSize">分页大小</param>
        /// <returns></returns>
        public PagedList<ResponseUserConsultDTO> ConsultMeRecord(
            string DoctorID,
          int SelectType,
          int PageIndex = 1,
          int PageSize = 10
          )
        {
            using (DBEntities db = new DBEntities())
            {
                var query = from opd in db.UserOPDRegisters
                            //需要查询出已经删除的订单，未付款的订单不需要在医生端显示
                            join room in db.ConversationRooms on opd.OPDRegisterID equals room.ServiceID
                            where opd.IsDeleted == false && opd.DoctorID == DoctorID
                            orderby
                            room.Priority ascending,
                            opd.OPDState ascending,
                            opd.OPDDate descending
                            select new ResponseUserConsultDTO()
                            {
                                OPDRegisterID = opd.OPDRegisterID,
                                MemberID = opd.MemberID,
                                MemberName = opd.MemberName,
                                ConsultContent = opd.ConsultContent,
                                OPDState = opd.OPDState,
                                ConsultTime = opd.OPDDate,
                                Price = opd.Fee,
                                Room = new ResponseConversationRoomDTO
                                {
                                    ServiceType = room.ServiceType,
                                    RoomState = room.RoomState,
                                    ConversationRoomID = room.ConversationRoomID,
                                    Secret = room.Secret,
                                    Duration = room.Duration,
                                    TotalTime = room.TotalTime,
                                    Enable = room.Enable
                                }
                            };

                switch (SelectType)
                {
                    case -1: query = query.Where(i => i.CostType == EnumCostType.Free || i.CostType == EnumCostType.FreeClinic); break;
                    case 0: query = query.Where(i => i.CostType == EnumCostType.Free); break;
                    case 1: query = query.Where(i => i.CostType == EnumCostType.FreeClinic); break;
                    case 2:
                        query = query.Where(i =>
                                            i.CostType == EnumCostType.FamilyDoctor
                                            || i.CostType == EnumCostType.MemberPackage
                                            || i.CostType == EnumCostType.OrgDiscount
                                            || i.CostType == EnumCostType.Pay); break;
                }

                var ret = query.ToPagedList(PageIndex, PageSize);
                return ret;
            }
        }

        /// <summary>
        /// 获取机构预约记录
        /// </summary>
        /// <param name="request"></param>
        /// <param name="isTreatAccount"></param>
        /// <returns></returns>
        public PagedList<ResponseUserOPDRegisterDTO> GetOrgOPDRegister(RequestQueryOPDRegisterDTO request)
        {
            var today = DateTime.Now;

            using (var db = new DBEntities())
            {
                var query = from opd in db.UserOPDRegisters
                            join triage in db.DoctorTriages.Where(i => i.TriageStatus == EnumTriageStatus.Triaged) on opd.OPDRegisterID equals triage.OPDRegisterID
                            join room in db.ConversationRooms.Where(a => a.IsDeleted == false) on opd.OPDRegisterID equals room.ServiceID into leftJoinRoom
                            from roomIfEmpty in leftJoinRoom.DefaultIfEmpty()
                            where !opd.IsDeleted && opd.OrgnazitionID == request.OrgnazitionID 
                            && (!request.isTreatAccount || (roomIfEmpty != null || opd.ScheduleID != ""))
                            select new ResponseUserOPDRegisterDTO()
                            {
                                OPDRegisterID = opd.OPDRegisterID,//预约编号
                                OPDDate = opd.OPDDate,//排版日期
                                OPDType = opd.OPDType,//预约类型
                                RegDate = opd.RegDate,//预约时间                                                    
                                UserID = opd.UserID,//用户编号                             
                                Fee = opd.Fee,//费用
                                ConsultContent = opd.ConsultContent,
                                Age = opd.Age,
                                OPDState = opd.OPDState,
                                Room = new ResponseConversationRoomDTO()
                                {
                                    //就诊当天，没有就诊，用户已经支付
                                    ConversationRoomID = roomIfEmpty.ConversationRoomID,
                                    RoomState = roomIfEmpty != null ? roomIfEmpty.RoomState : EnumRoomState.NoTreatment,//预约状态
                                    Secret = roomIfEmpty != null ? roomIfEmpty.Secret : "",//房间密码
                                },
                                OrderNo = opd.OrderNo,
                                Member = new ResponseUserMemberDTO()
                                {
                                    UserID = opd.UserID,
                                    MemberID = opd.MemberID,//成员编号
                                    MemberName = opd.MemberName,
                                    Gender = opd.Gender,
                                    Age = opd.Age,
                                },
                                Doctor = new ResponseDoctorInfoDTO()
                                {

                                    DoctorID = opd.DoctorID,//医生编号
                                    DoctorName = opd.DoctorName,
                                },
                                OfflineRecipe = roomIfEmpty == null,
                            };

                #region 处理搜索条件
                //查询关键字
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    query = query.Where(a => a.Member.MemberName.Contains(request.Keyword));
                }

                //开始日期
                if (request.BeginDate.HasValue)
                {
                    query = query.Where(a => a.OPDDate >= request.BeginDate);
                }

                //结束日期
                if (request.EndDate.HasValue)
                {
                    request.EndDate = request.EndDate.Value.AddDays(1);
                    query = query.Where(a => a.OPDDate < request.EndDate);
                }


                //类型
                if (request.OPDType.HasValue)
                {
                    query = query.Where(a => a.OPDType == request.OPDType.Value);
                }
                //状态
                if (request.OPDState.HasValue)
                {
                    query = query.Where(a => a.OPDState == request.OPDState.Value);
                }
                #endregion

                query = query.OrderByDescending(a => a.OPDBeginTime);

                var ret = query.ToPagedList(request.CurrentPage, request.PageSize);
                return ret;
            }
        }

        /// <summary>
        /// 获取医生的语音/视频看诊
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public PagedList<ResponseOPDRegisterAudVidDTO> GetDoctorAudVid(string DoctorID, int PageIndex = 1, int PageSize = int.MaxValue)
        {
            using (var db = new DBEntities())
            {
                var query = from opd in db.UserOPDRegisters
                            join triage in db.DoctorTriages.Where(i => i.TriageStatus == EnumTriageStatus.Triaged) on opd.OPDRegisterID equals triage.OPDRegisterID
                            join room in db.ConversationRooms.Where(a => a.IsDeleted == false) on opd.OPDRegisterID equals room.ServiceID into leftJoinRoom
                            from roomIfEmpty in leftJoinRoom.DefaultIfEmpty()
                            where opd.IsDeleted == false
                                  //&& opd.DoctorID == DoctorID
                                  && triage.TriageDoctorID == DoctorID
                                  && (opd.OPDType == EnumDoctorServiceType.AudServiceType || opd.OPDType == EnumDoctorServiceType.VidServiceType)
                            orderby opd.RegDate descending, opd.OPDRegisterID descending
                            select new ResponseOPDRegisterAudVidDTO()
                            {
                                OPDRegisterID = opd.OPDRegisterID,//预约编号
                                MemberID = opd.MemberID,
                                MemberName = opd.MemberName,
                                Birthday = opd.Birthday,
                                Gender = opd.Gender,
                                ChannelID = roomIfEmpty.ConversationRoomID,
                                OPDType = opd.OPDType,//预约类型
                                RegDate = opd.RegDate,//预约时间  
                                OPDDate = opd.OPDDate,
                                ConsultContent = opd.ConsultContent,
                                OPDState = opd.OPDState,
                                Price = opd.Fee,
                            };

                return query.ToPagedList(PageIndex, PageSize);
            }
        }

        /// <summary>
        /// 获取服务详情（患者查看）
        /// </summary>
        /// <param name="OPDRegisterID"></param>
        /// <returns></returns>
        public ResponseUserOPDRegisterDTO GetServiceDetail(string OPDRegisterID)
        {
            using (var db = new DBEntities())
            {
                var model = userOPDRegisterRepository.Single(OPDRegisterID, db);
                if (model != null)
                {
                    var isConsultation = model.OPDType == EnumDoctorServiceType.Consultation ? true : false;

                    #region  病历，诊断
                    model.UserMedicalRecord = (from m in db.UserMedicalRecords
                                               where m.OPDRegisterID == OPDRegisterID && (isConsultation == true) && m.IsDeleted == false
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
                                                   UserMedicalRecordID = m.UserMedicalRecordID
                                               }).FirstOrDefault();
                    #endregion

                    if (isConsultation)
                    {
                        #region 订单和房间信息
                        var consultation = (from consul in db.RemoteConsultations
                                            join room in db.ConversationRooms on new { ID = consul.ConsultationID, IsDeleted = false } equals new { ID = room.ServiceID, IsDeleted = room.IsDeleted } into roomLeftMid
                                            from roomLeft in roomLeftMid.DefaultIfEmpty()
                                            where consul.OPDRegisterID == OPDRegisterID && consul.IsDeleted == false
                                            select new
                                            {
                                                Room = roomLeft == null ? null : new ResponseConversationRoomDTO
                                                {
                                                    ConversationRoomID = roomLeft.ConversationRoomID,
                                                    RoomState = roomLeft.RoomState,
                                                    Secret = roomLeft.Secret
                                                }
                                            }).FirstOrDefault();

                        if (consultation != null)
                        {
                            model.Room = consultation.Room;
                        }
                        #endregion

                        if (model.UserMedicalRecord != null)
                        {
                            //主治医生治疗建议
                            model.UserMedicalRecord.Advised = (from consltation in db.RemoteConsultations.Where(i => i.OPDRegisterID == model.OPDRegisterID && i.IsDeleted == false)
                                                               join attendingDoctor in db.ConsultationDoctors on new { ID = consltation.ConsultationID, IsAttending = true } 
                                                               equals new { ID = attendingDoctor.ConsultationID, IsAttending = attendingDoctor.IsAttending }
                                                               select attendingDoctor.Perscription).FirstOrDefault();
                            #region 病例文件(附件)

                            model.AttachFiles = (from file in db.UserFiles
                                                 where file.OutID == model.UserMedicalRecord.UserMedicalRecordID && file.IsDeleted == false && file.FileType == 0
                                                 select new ResponseUserFileDTO
                                                 {
                                                     FileID = file.FileID,
                                                     FileName = file.FileName,
                                                     FileType = file.FileType,
                                                     FileUrl = file.FileUrl,
                                                     OutID = file.OutID,
                                                     Remark = file.Remark
                                                 }).ToList();

                            if (model.AttachFiles == null)
                            {
                                model.AttachFiles = new List<ResponseUserFileDTO>();
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        model.Room = (from m in db.ConversationRooms
                                      where m.ServiceID == model.OPDRegisterID && m.IsDeleted == false
                                      select new ResponseConversationRoomDTO
                                      {
                                          ConversationRoomID = m.ConversationRoomID,
                                          RoomState = m.RoomState,
                                          Secret = m.Secret
                                      }).FirstOrDefault();

                        #region 图文咨询以及音视频咨询增加问诊申请时的附件上传

                        IQueryable<UserFile> query = null;

                        query = from m in db.UserFiles where m.OutID == model.OPDRegisterID select m;
    

                        if (query != null)
                        {
                            model.AttachFiles = query.Where(i => i.FileType == 0).Select(m => new ResponseUserFileDTO
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

                    }

                }
                return model;
            }
        }

        public bool ExistsWithSubmitRequest(RequestCheckTodaySubmitedDTO request, out ResponseRepeatReturnDTO order)
        {
            return userOPDRegisterRepository.ExistsWithSubmitRequest(request, out order);
        }

        /// <summary>
        /// 创建诊室房间
        /// </summary>
        /// <param name="request"></param>
        /// <param name="exceptDoctors"></param>
        /// <returns></returns>
        public ApiResult CreateConsultingRoom(RequestUserOPDRegisterSubmitDTO request, List<string> exceptDoctors = null)
        {
            try
            {
                request.InquiryType = 1;

                var result = Submit(request);

                if (result.ActionStatus == "Success" || result.ActionStatus == "Repeat")
                {
                    if (CreateIMRoom(result.OPDRegisterID))
                    {
                        var room = conversationRoomRepository.GetChannelInfo(result.ChannelID);

                        if (room != null && !room.Enable)
                        {
                            throw new Exception("诊室未初始化");
                        }
                        else
                        {
                            using (MQChannel mqChannel = new MQChannel())
                            {
                                mqChannel.Publish(new Dto.EventBus.UserNoticeEvent()
                                {
                                    NoticeType = EnumNoticeSecondType.DoctorReportInterpretation,
                                    ServiceID = result.OPDRegisterID
                                });
                            }

                            var url = string.Format("/Manage/User#/User/UserOnlineVideo/ChannelID-{0}", room.ConversationRoomID);
                            var obj = new { MemberID = result.MemberID, ChannelID = room.ConversationRoomID, URL = url };
                            return obj.ToApiResultForObject();
                        }
                    }
                }
                return EnumApiStatus.BizError.ToApiResultForApiStatus();
            }
            catch (Exception ex)
            {
                return EnumApiStatus.BizError.ToApiResultForApiStatus(ex.Message);
            }
        }

        /// <summary>
        /// 提交预约
        /// </summary>
        /// <param name="request"></param>
        /// <param name="CheckExists">是否检查重复预约</param>
        /// <returns></returns>
        public ResponseUserOPDRegisterSubmitDTO Submit(RequestUserOPDRegisterSubmitDTO request,bool CheckExists = true)
        {
            request.OPDDate = DateTime.Now;
            request.OPDBeginTime = DateTime.Now.ToString("HH:mm");
            request.OPDEndTime = DateTime.Now.AddMinutes(30).ToString("HH:mm");

            var db = userOPDRegisterRepository.CreateDb();

            #region 校验失败：重复预约
            var existsOrder = new ResponseRepeatReturnDTO();

            if (CheckExists && userOPDRegisterRepository.ExistsWithSubmitRequest(
                request.Map<RequestUserOPDRegisterSubmitDTO, RequestCheckTodaySubmitedDTO>(), out existsOrder, db))
            {
                //一键呼叫时，且订单已经被医生领取，如果订单能够被取消掉则先取消订单再预约
                if (string.IsNullOrEmpty(request.DoctorID) &&
                    !string.IsNullOrEmpty(existsOrder.DoctorID) &&
                    existsOrder.Cancelable)
                {
                    //取消掉原订单
                    if (!userOPDRegisterRepository.CancelOpdRegister(existsOrder.OrderOutID))
                    {
                        return new ResponseUserOPDRegisterSubmitDTO
                        {
                            ErrorInfo = "取消未完成的订单失败，请重试",
                            ActionStatus = "Fail",
                        };
                    }
                }
                else
                {
                    return new ResponseUserOPDRegisterSubmitDTO
                    {
                        ErrorInfo = "当天已有未完成的预约，不能重复预约",
                        OPDRegisterID = existsOrder.OrderOutID,
                        ChannelID = existsOrder.ChannelID,
                        ActionStatus = "Repeat"
                    };

                }
            }
            #endregion

            userOPDRegisterRepository.AddOpdRegister(request, db);

            userOPDRegisterRepository.AddConversationRoom(request, db);

            if (userOPDRegisterRepository.SubmitChange(db))
            {
                return new ResponseUserOPDRegisterSubmitDTO
                {
                    ErrorInfo = "预约成功",
                    ActionStatus = "Success",
                    OPDRegisterID = request.OPDRegisterID,
                    ChannelID = request.ChannelID
                };
            }

            return new ResponseUserOPDRegisterSubmitDTO
            {
                ErrorInfo = "预约失败",
                ActionStatus = "Fail",
            };
        }


        /// <summary>
        /// 新增图文咨询
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseUserOPDRegisterSubmitDTO ConsultSubmit(RequestUserOPDRegisterSubmitDTO request, bool checkExists = true)
        {
            List<RequestUserFileDTO> Files = request.Files;

            var OPDBeginTime = DateTime.Now.ToString("HH:ss");
            var OPDEndTime = DateTime.Now.AddMinutes(30).ToString("HH:ss");
            var OPDDate = DateTime.Now;
            var IsUseTaskPool = false;

            var result = new ResponseUserOPDRegisterSubmitDTO
            {
                ErrorInfo = "预约失败",
                ActionStatus = "Fail",
                OPDRegisterID = "",
            };

            #region 没有指定当前操作用户

            if (string.IsNullOrEmpty(request.UserID))
            {
                throw new Exception("UserID 不存在！");
            }

            #endregion

            #region 校验失败：没有设置就诊人
            if (string.IsNullOrEmpty(request.MemberID))
            {
                result.ErrorInfo = "没有设置就诊人";
                result.ActionStatus = "Fail";
                return result;
            }
            #endregion          


            #region 校验失败：已经预约了

            var existsOrder = new ResponseRepeatReturnDTO();
            if (checkExists && ExistsWithSubmitRequest(request.Map<RequestUserOPDRegisterSubmitDTO, RequestCheckTodaySubmitedDTO>(), out existsOrder))
            {
                #region 预约失败
                result.ErrorInfo = "当天已有未完成的预约，不能重复预约";
                result.ActionStatus = "Repeat";
                result.OPDRegisterID = existsOrder.OrderOutID;
                result.OrderNO = existsOrder.OrderNo;
                result.OPDState = existsOrder.OPDState;
                result.ChannelID = existsOrder.ChannelID;
                #endregion

                return result;
            }
            #endregion

            result.MemberID = request.MemberID;


            // 没指定医生的情况下，按套餐来定服务价格
            if (string.IsNullOrWhiteSpace(request.DoctorID))
            {
                IsUseTaskPool = true;
            }

            #region 咨询医生分组
            if (!string.IsNullOrEmpty(request.DoctorGroupID))
            {
                IsUseTaskPool = true;
            }
            #endregion

            using (var db = new DBEntities())
            {
                #region 新增预约记录
                var opdModel = new UserOPDRegister()
                {
                    OPDRegisterID = Guid.NewGuid().ToString("N"),
                    IsUseTaskPool = IsUseTaskPool,
                    CreateTime = DateTime.Now,
                    RegDate = DateTime.Now,
                    CostType = request.CostType,
                    OPDBeginTime = OPDBeginTime,
                    OPDEndTime = OPDEndTime,
                    Fee = request.ServicePrice,//服务价格
                    DoctorID = string.IsNullOrEmpty(request.DoctorID) ? "" : request.DoctorID,//医生编号    
                    OPDState = EnumOPDState.NoPay,
                    OPDDate = DateTime.Now,
                    UserID = string.IsNullOrEmpty(request.UserID) ? "" : request.UserID,
                    ConsultContent = string.IsNullOrEmpty(request.ConsultContent) ? "" : request.ConsultContent,
                    ConsultDisease = string.IsNullOrEmpty(request.ConsultDisease) ? "" : request.ConsultDisease,
                    ScheduleID = "",
                    MemberID = string.IsNullOrEmpty(request.MemberID) ? "" : request.MemberID,
                    OPDType = EnumDoctorServiceType.PicServiceType,
                    OrgnazitionID = request.OrgnazitionID,
                    DoctorGroupID = string.IsNullOrEmpty(request.DoctorGroupID) ? "" : request.DoctorGroupID,
                    MemberName = request.MemberName,
                    Gender = request.Gender,
                    Marriage = request.Marriage,
                    Age = request.Age,
                    IDNumber = string.IsNullOrEmpty(request.IDNumber) ? "" : request.IDNumber,
                    IDType = request.IDType,
                    Mobile = request.Mobile,
                    Address = request.Address,
                    Birthday = request.Birthday,
                    InquiryType = request.InquiryType,
                };

                db.UserOPDRegisters.Add(opdModel);

                #region 是否进入导诊系统分诊，否则分诊状态设为已分诊
                var isToGuidance = request.IsToGuidance;
                db.DoctorTriages.Add(new DoctorTriage
                {
                    OPDRegisterID = opdModel.OPDRegisterID,
                    TriageDoctorID = isToGuidance ? "" : opdModel.DoctorID,
                    TriageStatus = isToGuidance ? EnumTriageStatus.None : EnumTriageStatus.Triaged,
                    IsToGuidance = isToGuidance
                });
                #endregion

                #endregion

                db.UserOPDRegisters.Add(opdModel);

                #region 添加图片
                if (Files != null)
                {
                    foreach (var file in Files)
                    {
                        db.UserFiles.Add(new UserFile()
                        {
                            FileID = Guid.NewGuid().ToString("N"),
                            FileName = file.FileUrl,
                            FileUrl = file.FileUrl,
                            FileType = 0,
                            AccessKey = "",
                            ResourceID = "",
                            Remark = file.Remark,
                            OutID = opdModel.OPDRegisterID,
                            UserID = request.UserID
                        });
                    }
                }
                #endregion

                #region 创建房间
                var room = new ConversationRoom();
                room.EndTime = DateTime.Parse(OPDDate.ToString("yyyy-MM-dd ") + OPDEndTime);
                room.BeginTime = DateTime.Parse(OPDDate.ToString("yyyy-MM-dd ") + OPDBeginTime);
                room.TotalTime = 0;
                room.RoomState = EnumRoomState.NoTreatment;//状态
                room.ConversationRoomID = Guid.NewGuid().ToString("N");
                room.ServiceID = opdModel.OPDRegisterID;
                room.Secret = Guid.NewGuid().ToString("N");//房间密码
                room.ServiceType = EnumDoctorServiceType.PicServiceType;//图文咨询
                room.TriageID = long.MaxValue;
                room.Priority = request.UserLevel;
                room.RoomType = EnumRoomType.Group;
                db.ConversationRooms.Add(room);
                #endregion

                if (db.SaveChanges() > 0)
                {
                    #region 预约成功         
                    result.OPDState = opdModel.OPDState;
                    result.ErrorInfo = "预约成功";
                    result.ActionStatus = "Success";
                    result.OPDRegisterID = opdModel.OPDRegisterID;
                    result.ChannelID = room.ConversationRoomID;
                    #endregion
                }
                else
                {
                    #region 预约失败
                    result.ErrorInfo = "预约失败";
                    result.ActionStatus = "Fail";
                    #endregion
                }
                return result;
            }
        }


        /// <summary>
        /// 转诊
        /// </summary>
        /// <param name="opdRegisterID"></param>
        /// <param name="toOPDType"></param>
        /// <returns></returns>
        public ResponseUserOPDRegisterSubmitDTO TransferTreatment(string opdRegisterID, EnumDoctorServiceType toOPDType)
        {
            ResponseUserOPDRegisterSubmitDTO result = new ResponseUserOPDRegisterSubmitDTO
            {
                ErrorInfo = "预约失败",
                ActionStatus = "Fail"
            };

            // 如果没有预约记录
            ResponseUserOPDRegisterDTO opdRegister = this.Single(opdRegisterID);
            if (opdRegister == null)
            {
                result.ErrorInfo = "未找到预约记录";
                return result;
            }

            // 已取消订单无法转诊
            if (opdRegister.OPDState ==  EnumOPDState.Canceled)
            {
                result.ErrorInfo = "预约已取消";
                return result;
            }

            if (opdRegister.OPDType == toOPDType)
            {
                result.ErrorInfo = "转诊服务类型不可以与原服务类型相同";
                return result;
            }

            var room = conversationRoomRepository.GetChannelInfo(opdRegister.Room.ConversationRoomID);
            if (room.RoomState != EnumRoomState.Waiting && room.RoomState != EnumRoomState.WaitAgain)
            {
                result.ErrorInfo = "当前状态不可转诊";
                return result;
            }

            // 转诊过程设置房间状态为不可修改
            if (opdRegister.Room != null && !conversationRoomRepository.CloseRoom(opdRegister.Room.ConversationRoomID, true))
            {
                LogHelper.DefaultLogger.Error("设置房间为关闭状态失败，房间号：" + opdRegister.Room.ConversationRoomID);
                return result;
            }

            try
            {
                // 义诊申请的问诊进行转诊 -- 不支持
                if (opdRegister.CostType ==  EnumCostType.FreeClinic)
                {
                    result.ActionStatus = "UnSupport";
                    result.ErrorInfo = "当前预约不可转诊";
                }
                // 付费服务、机构折扣、家庭医生申请的问诊进行转诊 -- 目前暂不支持，以后完善
                else if (opdRegister.CostType == EnumCostType.Pay || opdRegister.CostType == EnumCostType.FamilyDoctor ||
                    opdRegister.CostType == EnumCostType.OrgDiscount)
                {
                    result.ActionStatus = "UnSupport";
                    result.ErrorInfo = "当前预约不可转诊";
                }
                // 套餐申请的问诊进行转诊
                else if (opdRegister.CostType == EnumCostType.MemberPackage)
                {
                    result = MemberPackageTransferTreatment(opdRegister, opdRegister.CostType, toOPDType);
                }

                return result;
            }
            catch (Exception e)
            {
                LogHelper.DefaultLogger.Error(e);
                return result;
            }
            finally
            {
                if (opdRegister.Room != null && !conversationRoomRepository.CloseRoom(opdRegister.Room.ConversationRoomID, false))
                    LogHelper.DefaultLogger.Error("设置房间为非关闭状态失败，房间号：" + opdRegister.Room.ConversationRoomID);
            }
        }

        /// <summary>
        /// 使用套餐申请的问诊，进行转诊
        /// </summary>
        /// <param name="opdRegister"></param>
        private ResponseUserOPDRegisterSubmitDTO MemberPackageTransferTreatment(ResponseUserOPDRegisterDTO opdRegister, EnumCostType privilege, EnumDoctorServiceType toOPDType)
        {
            ResponseUserOPDRegisterSubmitDTO result = new ResponseUserOPDRegisterSubmitDTO
            {
                ErrorInfo = "预约失败",
                ActionStatus = "Fail"
            };

            

            // 根据服务类型重新预约
            result = SubmitByOPDType(new RequestUserOPDRegisterSubmitDTO
            {
                DoctorID = opdRegister.DoctorID,
                OrgnazitionID = opdRegister.OrgnazitionID,
                OPDType = toOPDType,
                MemberID = opdRegister.MemberID,
                UserID = opdRegister.UserID,
                UserLevel = opdRegister.Room.Priority,
                 CostType = privilege,
                ConsultContent = opdRegister.ConsultContent,
                ConsultDisease = opdRegister.ConsultDisease,
                DoctorGroupID = opdRegister.DoctorGroupID,
            });

            // 如果预约申请成功，取消原服务的订单
            if (result.ActionStatus == "Success")
            {
                if (!Cancel(opdRegister.OPDRegisterID))
                    LogHelper.DefaultLogger.Error("取消预约失败，预约号：" + opdRegister.OPDRegisterID);
            }

            return result;
        }

        private ResponseUserOPDRegisterSubmitDTO SubmitByOPDType(RequestUserOPDRegisterSubmitDTO request, bool consultFree = false)
        {
            // 如果转音视频问诊
            if (request.OPDType == EnumDoctorServiceType.AudServiceType || request.OPDType == EnumDoctorServiceType.VidServiceType)
            {
                return this.Submit(request);
            }
            // 如果转图文咨询
            else if (request.OPDType == EnumDoctorServiceType.PicServiceType)
            {
                ConversationRoomService roomService = new ConversationRoomService();
                return this.ConsultSubmit(request, false);
            }

            return new ResponseUserOPDRegisterSubmitDTO
            {
                ErrorInfo = "预约失败",
                ActionStatus = "Fail"
            };
        }


        /// <summary>
        /// 创建聊天的房间
        /// </summary>
        /// <param name="OPDRegisterID"></param>
        /// <returns></returns>
        public bool CreateIMRoom(string OPDRegisterID)
        {

            using (DBEntities db = new DBEntities())
            {
                var model = (from opd in db.UserOPDRegisters.Where(a => a.OPDRegisterID == OPDRegisterID)
                             join triage in db.DoctorTriages.Where(i => i.TriageStatus == EnumTriageStatus.Triaged) on opd.OPDRegisterID equals triage.OPDRegisterID into triageLeftMid
                             from triageLeft in triageLeftMid.DefaultIfEmpty()
                             select new
                             {
                                 OPDType = opd.OPDType,
                                 DoctorID = triageLeft == null ? "" : triageLeft.TriageDoctorID,//opd.DoctorID,
                                 opd.DoctorName,
                                 opd.DoctorPhotoUrl,
                                 UserID = opd.UserID,
                                 MemberID = opd.MemberID,
                                 opd.MemberName,
                                 opd.PhotoUrl,
                             }).FirstOrDefault();


                if (model != null)
                {

                    #region 创建IM群组
                    ConversationIMUidService imUidService = new ConversationIMUidService();
                    var roomService = new ConversationRoomService();

                    //房间信息
                    var room = roomService.GetChannelInfo(OPDRegisterID);

                    if (room != null)
                    {
                        var GroupName = model.OPDType.GetEnumDescript();
                        var Introduction = "";
                        var Notification = "";
                        var groupMembers = new List<int>();
                        var channelMembers = new List<RequestChannelMemberDTO>();

                        //患者信息
                        var userIdentifier = imUidService.GetUserIMUid(model.UserID);
                        if (userIdentifier > 0)
                        {
                            groupMembers.Add(userIdentifier);
                            channelMembers.Add(new RequestChannelMemberDTO()
                            {
                                Identifier = userIdentifier,
                                UserType = EnumUserType.User,
                                UserID = model.UserID,
                                UserMemberID = model.MemberID,
                                PhotoUrl = model.PhotoUrl,
                                UserCNName = model.MemberName,
                                UserENName = model.MemberName
                            });
                        }

                        //获取医生信息(如果走导诊系统，此时还未分配医生，分诊后才会把分诊医生加入到IM组)
                        if (!string.IsNullOrEmpty(model.DoctorID))
                        {
                            var doctorIdentifier = imUidService.GetUserIMUid(model.DoctorID);
                            if (doctorIdentifier > 0)
                            {
                                groupMembers.Add(doctorIdentifier);
                                channelMembers.Add(new RequestChannelMemberDTO()
                                {
                                    Identifier = doctorIdentifier,
                                    UserID = model.DoctorID,
                                    UserType = EnumUserType.Doctor,
                                    UserMemberID = model.MemberID,
                                    PhotoUrl = model.DoctorPhotoUrl,//DTO已经进行了路径转换，这里需要使用没有转换之前的数据
                                    UserENName = model.DoctorName,
                                    UserCNName = model.DoctorName
                                });
                            }
                        }

                        if (room.Enable)
                        {
                            if (imservice.AddGroupMember(room.ConversationRoomID, groupMembers))
                            {
                                return conversationRoomRepository.InsertChannelMembers(room.ConversationRoomID, channelMembers);
                            }
                        }
                        else
                        {
                            //创建裙子成功
                            if (imservice.CreateGroup(room.ConversationRoomID, GroupName, model.OPDType, groupMembers, Introduction, Notification))
                            {
                                using (MQChannel mqChannel = new MQChannel())
                                {
                                    if (mqChannel.Publish(new ChannelCreatedEvent()
                                    {
                                        ChannelID = room.ConversationRoomID,
                                        ServiceID = room.ServiceID,
                                        ServiceType = room.ServiceType

                                    }))
                                    {
                                        room.Enable = true;

                                        if (conversationRoomRepository.UpdateRoomEable(room.ConversationRoomID, room.Enable))
                                        {

                                            return conversationRoomRepository.InsertChannelMembers(room.ConversationRoomID, channelMembers);
                                        }
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                            }
                        }


                    }
                    else
                    {
                        return true;

                    }

                    #endregion

                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public ResponseDoctorTriageDTO GetDoctorTriageDetail(string OPDRegisterID)
        {
            return userOPDRegisterRepository.GetDoctorTriageDetail(OPDRegisterID);
        }

        public ResponseUserOPDRegisterDTO Single(string OPDRegisterID)
        {
            var db = userOPDRegisterRepository.CreateDb();

            var model = userOPDRegisterRepository.Single(OPDRegisterID, db);

            #region 房间信息
            if (model != null)
            {
                model.Room = conversationRoomRepository.GetChannelInfo(
                conversationRoomRepository.GetChannelIDByServiceID(model.OPDRegisterID, db), db);
            }
            #endregion

            userOPDRegisterRepository.Dispose(db);
            return model;
        }

        public bool Cancel(string OPDRegisterID)
        {
            using (var db = new DBEntities())
            {
                var dbmodel = db.UserOPDRegisters.Where(t => t.OPDRegisterID == OPDRegisterID && t.OPDState != EnumOPDState.Canceled).FirstOrDefault();

                if (dbmodel != null)
                {
                    dbmodel.OPDState = EnumOPDState.Canceled;
                    dbmodel.ModifyTime = DateTime.Now;
                    return db.SaveChanges() > 0;
                }
                return true;
            }
        }

        public bool UpdateReplied(string ServiceID)
        {
            using (var db = new DBEntities())
            {
                var dbmodel = db.UserOPDRegisters.Where(t => t.OPDRegisterID == ServiceID && t.OPDState!=  EnumOPDState.Replied).FirstOrDefault();

                if(dbmodel != null)
                {
                    dbmodel.OPDState = EnumOPDState.Replied;
                    dbmodel.AnswerTime = DateTime.Now;
                    dbmodel.ModifyTime = DateTime.Now;
                    return db.SaveChanges() > 0;
                }
                return true;
            }
        }

        public bool SendConsultContent(string ConversationRoomID, string OPDRegisterID)
        {
            var sysDerepService = new SysDereplicationService();
            var imUidService = new ConversationIMUidService();

            using (DBEntities db = new DBEntities())
            {
                var model = db.UserOPDRegisters.Where(a => a.OPDRegisterID == OPDRegisterID && !a.IsDeleted).FirstOrDefault();

                if (model != null)
                {
                    //用户唯一标识
                    var userIdentifier = imUidService.GetUserIMUid(model.UserID);

                    //发送图片消息
                    var ImageFiles = db.UserFiles.Where(a => a.OutID == OPDRegisterID).ToList();

                    using (MQChannel mqChannel = new MQChannel())
                    {
                        mqChannel.BeginTransaction();

                        foreach (var File in ImageFiles)
                        {
                            if (!mqChannel.Publish(new ChannelSendGroupMsgEvent<ResponseUserFileDTO>()
                            {
                                ChannelID = ConversationRoomID,
                                FromAccount = userIdentifier,
                                Msg = File.Map<UserFile, ResponseUserFileDTO>()

                            }))
                            {
                                return false;
                            }
                        }

                        if (!mqChannel.Publish(new ChannelSendGroupMsgEvent<string>()
                        {
                            ChannelID = ConversationRoomID,
                            FromAccount = userIdentifier,
                            Msg = model.ConsultContent

                        }))
                        {
                            return false;
                        }

                        mqChannel.Commit();

                        return true;
                    }
                }
            }

            return true;
        }

        public bool OPDComplete(string serviceID)
        {
            if (userOPDRegisterRepository.OPDComplete(serviceID))
            {
                throw new NotImplementedException("未实现发布订单完成消息");
            }
            return false;
        }

        public bool Delete(string opdRegisterID)
        {
            return userOPDRegisterRepository.Delete(opdRegisterID);
        }

        
    }
}
