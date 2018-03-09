using HealthCloud.Common;
using HealthCloud.Common.Cache;
using HealthCloud.Common.EventBus;
using HealthCloud.Common.Extensions;
using HealthCloud.Common.Json;
using HealthCloud.Common.Log;
using HealthCloud.Common.Utility;
using HealthCloud.Consultation.Common.Config;
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
    public class RemoteConsultationRepository : BaseRepository
    {
        /// <summary>
        /// 会诊单列表
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public PagedList<ResponseConsultationInfoDTO> GetList(RequestConsultationSearchDTO condition, DBEntities db = null)
        {
            bool dbPrivateFlag = false;
            if (db == null)
            {
                db = CreateDb();
                dbPrivateFlag = true;
            }
            var query = (from item in db.RemoteConsultations
                         join opd in db.UserOPDRegisters on item.OPDRegisterID equals opd.OPDRegisterID
                         join room in db.ConversationRooms on item.ConsultationID equals room.ServiceID into leftJoinRoom
                         from leftRoom in leftJoinRoom.DefaultIfEmpty()
                         where item.IsDeleted == false
                         select new ResponseConsultationInfoDTO
                         {
                             ConsultationID = item.ConsultationID,
                             ConsultationNo = item.ConsultationNo,
                             StartTime = item.StartTime,
                             FinishTime = item.FinishTime,
                             StartTimeReal = item.StartTimeReal,
                             FinishTimeReal = item.FinishTimeReal,
                             ConsultationProgress = item.ConsultationProgress,
                             ConsultationSource = item.ConsultationSource,
                             Amount = item.Amount,
                             Deposit = item.Deposit,
                             OPDRegisterID = item.OPDRegisterID,
                             CreateTime = item.CreateTime,
                             OPDState = opd.OPDState,
                             //UserID = membermap.UserID,
                             Member = new ResponseUserMemberDTO()
                             {
                                 MemberName = opd.MemberName,
                                 Gender = opd.Gender,
                                 Birthday = opd.Birthday,
                                 Mobile = opd.Mobile,
                                 IDType = opd.IDType,
                                 IDNumber = opd.IDNumber,
                                 MemberID = opd.MemberID
                             },
                             Room = new ResponseConversationRoomDTO
                             {
                                 ConversationRoomID = leftRoom.ConversationRoomID,
                                 RoomState = leftRoom != null ? leftRoom.RoomState : EnumRoomState.NoTreatment
                             },
                             Doctors = (from i in db.ConsultationDoctors
                                        where i.ConsultationID == item.ConsultationID
                                        select new ResponseDoctorInfoDTO
                                        {
                                            DoctorID = i.DoctorID,
                                            DoctorName = i.DoctorName,
                                            IsAttending = i.IsAttending
                                        }).ToList()

                         });

            //查询我是主诊医生的会诊单
            if (!string.IsNullOrEmpty(condition.CurrentOperatorDoctorID))
                query = query.Where(i => i.Doctors.Where(j => j.DoctorID == condition.CurrentOperatorDoctorID && (condition.IsSelectAll == true || j.IsAttending == true)).FirstOrDefault() != null);

            //查询专家的会诊单(仅查待会诊，会诊中，已完成的单)
            if (!string.IsNullOrEmpty(condition.SpecialtyDoctorID))
                query = query.Where(i => (i.ConsultationProgress == EnumConsultationProgress.Arranged ||
                                        i.ConsultationProgress == EnumConsultationProgress.InProgress ||
                                        i.ConsultationProgress == EnumConsultationProgress.Finished) &&
                                        i.Doctors.Where(j => j.DoctorID == condition.SpecialtyDoctorID && j.IsAttending == false).FirstOrDefault() != null);

            //查询患者的会诊单
            if (condition.MemberIds != null && condition.MemberIds.Count > 0)
            {
                query = query.Where(i => condition.MemberIds.Contains(i.Member.MemberID)
                                   && (i.ConsultationProgress == EnumConsultationProgress.Arranged ||
                                    i.ConsultationProgress == EnumConsultationProgress.InProgress ||
                                    i.ConsultationProgress == EnumConsultationProgress.Finished));
            }

            #region 查询条件
            if (!string.IsNullOrEmpty(condition.ConsultationNo))
                query = query.Where(i => i.ConsultationNo.Contains(condition.ConsultationNo));

            if (condition.ConsultationSource.HasValue)
                query = query.Where(i => i.ConsultationSource == condition.ConsultationSource.Value);

            if (condition.ConsultationProgresses != null && condition.ConsultationProgresses.Count > 0)
                query = query.Where(i => condition.ConsultationProgresses.Contains(i.ConsultationProgress));

            if (!string.IsNullOrEmpty(condition.Keyword))
                query = query.Where(i => i.Member.Mobile.Contains(condition.Keyword) || i.Doctors.Where(j => j.DoctorName.Contains(condition.Keyword) || i.Member.MemberName.Contains(condition.Keyword)).FirstOrDefault() != null);

            if (condition.BeginDate.HasValue)
                query = query.Where(i => i.StartTime >= condition.BeginDate || i.StartTimeReal >= condition.BeginDate);

            if (condition.EndDate.HasValue)
                query = query.Where(i => i.FinishTime <= condition.EndDate || i.FinishTimeReal <= condition.EndDate);

            if (condition.OPDState.HasValue)
                query = query.Where(i => i.OPDState == condition.OPDState);

            #region 导诊台条件查询

            if (!string.IsNullOrEmpty(condition.MemberID))
                query = query.Where(i => i.Member.MemberID == condition.MemberID);

            if (!string.IsNullOrEmpty(condition.MemberName))
                query = query.Where(i => i.Member.MemberName.Contains(condition.MemberName));

            if (!string.IsNullOrEmpty(condition.Mobile))
                query = query.Where(i => i.Member.Mobile.Contains(condition.Mobile));

            if (!string.IsNullOrEmpty(condition.SpecialtyDoctorName))
                query = query.Where(i => i.Doctors.Where(j => j.DoctorName.Contains(condition.SpecialtyDoctorName) && j.IsAttending == false).FirstOrDefault() != null);

            if (!string.IsNullOrEmpty(condition.DoctorName))
                query = query.Where(i => i.Doctors.Where(j => j.DoctorName.Contains(condition.DoctorName) && j.IsAttending == true).FirstOrDefault() != null);
            #endregion


            #endregion

            if (condition.OrderByStartTimeReal == true)
                query = query.OrderByDescending(i => i.StartTimeReal).ThenByDescending(i => i.CreateTime).ThenBy(i => i.ConsultationID);
            else
                query = query.OrderByDescending(i => i.CreateTime).ThenBy(i => i.ConsultationID);

            var result = query.ToPagedList(condition.CurrentPage, condition.PageSize);

            if (dbPrivateFlag)
            {
                db.Dispose();
            }

            return result;

        }

        public ResponseConsultationInfoDTO GetEntity(string consultationID, DBEntities db = null)
        {
            bool dbPrivateFlag = false;
            if (db == null)
            {
                db = CreateDb();
                dbPrivateFlag = true;
            }
            var query = (from item in db.RemoteConsultations
                         join opd in db.UserOPDRegisters on item.OPDRegisterID equals opd.OPDRegisterID
                         join room in db.ConversationRooms on item.ConsultationID equals room.ServiceID into leftJoinRoom
                         from leftRoom in leftJoinRoom.DefaultIfEmpty()
                         where item.ConsultationID == consultationID && item.IsDeleted == false
                         select new ResponseConsultationInfoDTO
                         {
                             ConsultationID = item.ConsultationID,
                             ConsultationNo = item.ConsultationNo,
                             StartTime = item.StartTime,
                             FinishTime = item.FinishTime,
                             StartTimeReal = item.StartTimeReal,
                             FinishTimeReal = item.FinishTimeReal,
                             ConsultationProgress = item.ConsultationProgress,
                             ConsultationSource = item.ConsultationSource,
                             Amount = item.Amount,
                             Deposit = item.Deposit,
                             Requirement = item.Requirement,
                             Purpose = item.Purpose,
                             OPDRegisterID = item.OPDRegisterID,
                             Address = item.Address,
                             CreateTime = item.CreateTime,
                             Member = new ResponseUserMemberDTO()
                             {
                                 UserID = opd.UserID,
                                 MemberID = opd.MemberID,
                                 MemberName = opd.MemberName,
                                 Gender = opd.Gender,
                                 Birthday = opd.Birthday,
                                 Mobile = opd.Mobile,
                                 IDType = opd.IDType,
                                 IDNumber = opd.IDNumber
                             },
                             Room = new ResponseConversationRoomDTO
                             {
                                 ConversationRoomID = leftRoom.ConversationRoomID,
                                 RoomState = leftRoom != null ? leftRoom.RoomState : EnumRoomState.NoTreatment
                             }
                         });

            var model = query.FirstOrDefault();
            if (model != null)
            {
                #region 会诊医生
                model.Doctors = (from i in db.ConsultationDoctors
                                 where i.ConsultationID == model.ConsultationID && i.IsDeleted == false
                                 select new ResponseDoctorInfoDTO
                                 {
                                     DoctorID = i.DoctorID,
                                     DoctorName = i.DoctorName,
                                 }).ToList();
                #endregion
            }

            if (dbPrivateFlag)
            {
                db.Dispose();
            }

            return model;
        }

        private UserOPDRegister CreateOpdRegister(RequestConsultationInfoUpdateDTO model, UserOPDRegister entity, RequestConsultationDoctorDTO attendingDoctor)
        {
            if (entity == null)
            {
                entity = new UserOPDRegister();
            }

            entity.UserID = model.CurrentOperatorUserID;
            entity.MemberID = model.MemberID;
            entity.OPDType = EnumDoctorServiceType.Consultation;
            entity.Fee = model.Amount;
            entity.CreateUserID = model.CurrentOperatorUserID;
            entity.OPDBeginTime = "";
            entity.OPDEndTime = "";
            entity.DoctorID = attendingDoctor == null ? "" : attendingDoctor.DoctorID;
            entity.DoctorName = attendingDoctor == null ? "" : attendingDoctor.DoctorName;
            entity.RegDate = model.StartTime == null ? DateTime.Now : model.StartTime.Value;
            entity.MemberName = model.MemberName;
            entity.Gender = model.Gender;
            entity.Marriage = model.Marriage;
            entity.Age = ToolHelper.GetAgeFromBirth(model.Birthday);
            entity.IDNumber = string.IsNullOrEmpty(model.IDNumber) ? "" : model.IDNumber;
            entity.IDType = model.IDType;
            entity.Mobile = model.Mobile;
            entity.Address = model.PatientAddress;
            entity.Birthday = model.Birthday;

            return entity;

        }

        public EnumApiStatus Modify(RequestConsultationInfoUpdateDTO model, ref ResponseConsultationLogDTO logDto, DBEntities db = null)
        {
            bool dbPrivateFlag = false;
            if (db == null)
            {
                db = CreateDb();
                dbPrivateFlag = true;
            }
            //主诊医生
            var attendingDoctor = model.Doctors.Where(i => i.IsAttending == true).FirstOrDefault();
            if (attendingDoctor == null)
            {
                //当前用户为主诊医生
                if (model.CurrUserAttending == true && !string.IsNullOrEmpty(model.ConsultationDoctorID))
                {
                    attendingDoctor = new RequestConsultationDoctorDTO { DoctorID = model.ConsultationDoctorID, IsAttending = true };
                    model.Doctors.Add(attendingDoctor);
                }
            }

            #region 病历
            UserMedicalRecord userMedical = null;
            if (model.MedicalRecord != null)
                userMedical = model.MedicalRecord.Map<RequestUserMedicalRecordDTO, UserMedicalRecord>();
            else
                userMedical = new UserMedicalRecord();
            userMedical.UserID = model.CurrentOperatorUserID;
            userMedical.MemberID = model.MemberID;
            userMedical.DoctorID = attendingDoctor == null ? model.Doctors.Select(i => i.DoctorID).First() : attendingDoctor.DoctorID;
            #endregion


            ConsultationLog operationLog;

            RemoteConsultation entity = null;
            if (!string.IsNullOrEmpty(model.ConsultationID))
            {
                #region 编辑(删除再添加)
                //订单不存在
                entity = db.RemoteConsultations.Where(i => i.ConsultationID == model.ConsultationID && i.IsDeleted == false).FirstOrDefault();
                if (entity == null)
                    return EnumApiStatus.ConsultationNotExists;

                //该状态不能修改
                if (entity.ConsultationProgress == EnumConsultationProgress.Arranged ||
                    entity.ConsultationProgress == EnumConsultationProgress.InProgress ||
                    entity.ConsultationProgress == EnumConsultationProgress.Finished ||
                    entity.ConsultationProgress == EnumConsultationProgress.Invalid)
                    return EnumApiStatus.CurrStatusNotModify;

                //已取消，已付款，已完成的订单不能再修改
                var opdRegister = db.UserOPDRegisters.Where(i => i.OPDRegisterID == entity.OPDRegisterID).FirstOrDefault();
                if (opdRegister != null && (opdRegister.OPDState != EnumOPDState.NoPay))
                    return EnumApiStatus.CurrOrderStatusNotModify;

                //修改会诊单
                entity.Requirement = model.Requirement;
                entity.Purpose = model.Purpose;
                entity.StartTime = model.StartTime;
                entity.FinishTime = model.FinishTime;
                entity.ConsultationProgress = model.ConsultationProgress;
                entity.Address = model.Address;
                db.Update(entity);

                //修改预约记录
                //opdRegister.OPDRegisterID = entity.OPDRegisterID;
                opdRegister = CreateOpdRegister(model, opdRegister, attendingDoctor);
                db.Update(opdRegister);

                //修改病例
                userMedical.UserMedicalRecordID = db.UserMedicalRecords.Where(i => i.OPDRegisterID == opdRegister.OPDRegisterID).Select(i => i.UserMedicalRecordID).First();
                userMedical.OPDRegisterID = opdRegister.OPDRegisterID;
                db.Update(userMedical);

                //删除医生
                var consultationDoctors = db.ConsultationDoctors.Where(i => i.ConsultationID == entity.ConsultationID);
                db.ConsultationDoctors.RemoveRange(consultationDoctors);

                //删除文件
                var userFiles = db.UserFiles.Where(i => i.OutID == userMedical.UserMedicalRecordID);
                db.UserFiles.RemoveRange(userFiles);

                //操作日志
                var operation = GetOperationType(entity, model);
                operationLog = CreateOperationLog(model.ConsultationID, operation.OperationType, EnumOPDState.NoPay, model.ConsultationProgress, operation.Message, model.CurrentOperatorUserID, model.OrgID);

                #endregion

            }
            else
            {
                // 添加会诊单
                entity = model.Map<RequestConsultationInfoUpdateDTO, RemoteConsultation>();
                entity.ConsultationID = Guid.NewGuid().ToString("N");
                entity.OPDRegisterID = entity.ConsultationID;
                entity.ConsultationNo = CreateConsultationNo();
                entity.CreateTime = DateTime.Now;
                entity.CreateUserID = model.CurrentOperatorUserID;
                db.RemoteConsultations.Add(entity);

                //添加预约记录
                var opdRegister = CreateOpdRegister(model, null, attendingDoctor);
                opdRegister.CreateTime = DateTime.Now;
                opdRegister.OPDDate = DateTime.Now;
                opdRegister.OrgnazitionID = model.OrgID;
                opdRegister.OPDRegisterID = entity.OPDRegisterID;

                db.UserOPDRegisters.Add(opdRegister);

                #region 是否进入导诊系统分诊，否则分诊状态设为不分诊
                db.DoctorTriages.Add(new DoctorTriage
                {
                    OPDRegisterID = opdRegister.OPDRegisterID,
                    TriageDoctorID = model.IsToGuidance ? "" : opdRegister.DoctorID,
                    TriageStatus = model.IsToGuidance ? EnumTriageStatus.None : EnumTriageStatus.Triaged,
                    IsToGuidance = model.IsToGuidance
                });
                #endregion

                //添加病历
                userMedical.UserMedicalRecordID = Guid.NewGuid().ToString("N");
                userMedical.OPDRegisterID = opdRegister.OPDRegisterID;
                db.UserMedicalRecords.Add(userMedical);

                //操作日志
                operationLog = CreateOperationLog(entity.ConsultationID, EnumConsultationOperationType.Create, EnumOPDState.NoPay, model.ConsultationProgress, "创建会诊订单", model.CurrentOperatorUserID, model.OrgID);

            }

            #region 添加医生
            if (model.Doctors != null && model.Doctors.Count > 0)
            {
                //会诊医生会诊服务价格
                decimal totalPrice = 0;
                model.Doctors.ForEach(item =>
                {
                    totalPrice += item.ServicePrice;
                    db.ConsultationDoctors.Add(new ConsultationDoctor
                    {
                        ConsultationDoctorID = Guid.NewGuid().ToString("N"),
                        ConsultationID = entity.ConsultationID,
                        DoctorID = item.DoctorID,
                        IsAttending = item.IsAttending,
                        Amount = item.ServicePrice
                    });
                });

                //平台服务费用
                totalPrice += model.PlatServicePrice;
                entity.Amount = totalPrice;
            }
            #endregion

            #region 添加文件
            if (model.Files != null)
            {
                model.Files.ForEach(i =>
                {
                    db.UserFiles.Add(new UserFile()
                    {
                        FileID = Guid.NewGuid().ToString("N"),
                        FileName = i.FileUrl,
                        FileUrl = i.FileUrl,
                        FileType = 0,
                        Remark = i.Remark,
                        OutID = userMedical.UserMedicalRecordID,
                        UserID = model.CurrentOperatorUserID
                    });
                });

            }
            #endregion

            #region 添加影像资料
            if (model.InspectResult != null)
            {
                //只保存新资料
                var inspectList = model.InspectResult.Where(i => string.IsNullOrEmpty(i.InspectResultID)).ToList();
                inspectList.ForEach(i =>
                {
                    var inspectEntity = i.Map<RequestUserInspectResultDTO, UserInspectResult>();
                    inspectEntity.InspectResultID = Guid.NewGuid().ToString("N");
                    db.UserInspectResults.Add(inspectEntity);
                });

                UploadInspect(inspectList);
            }
            #endregion

            logDto = operationLog.Map<ConsultationLog, ResponseConsultationLogDTO>();

            var ret = true;

            if (dbPrivateFlag)
            {
                ret = db.SaveChanges() > 0;
                db.Dispose();
            }

            if (ret)
            {
                return EnumApiStatus.BizOK;
            }
            else
            {
                return EnumApiStatus.BizError;
            }
        }

        public bool UploadInspect(List<RequestUserInspectResultDTO> inspects)
        {
            var config = SysConfigRepository.Get<Inspect>();
            var storeConfig = SysConfigRepository.Get<IMGStore>();
            var apiConfig = SysConfigRepository.Get<Common.Config.Consultation>();

            System.Collections.Generic.SortedList<string, string> list = new SortedList<string, string>();
            list.Add("appid", config.AppId);
            list.Add("appkey", config.AppKey);
            list.Add("noncestr", Guid.NewGuid().ToString("N"));
            string sign = WebAPIHelper.GetSign(list);

            list.Remove("appkey");
            list.Add("sign", sign);

            try
            {
                inspects.ForEach(x =>
                {
                    string param = JsonHelper.ToJson(new
                    {
                        fileId = x.InspectResultID,
                        filePath = storeConfig.UrlPrefix.TrimEnd('/') + "/" + x.FileUploadName.TrimStart('/'),
                        callBackUrl = apiConfig.ConsultationApiUrl.TrimEnd('/') + "/UploadInspectCallback"
                    });

                    string result = WebAPIHelper.HttpPost(config.UploadFileUrl, param, list);

                    var obj = JsonHelper.FromJson<dynamic>(result);
                    if (obj.error_code != 100)
                        LogHelper.DefaultLogger.Error(string.Format("影像上传接口：{0}，参数：{1}，调用异常：{2}",
                            config.UploadFileUrl, param, obj.message));
                });

                return true;
            }
            catch (Exception e)
            {
                LogHelper.DefaultLogger.Error(e.Message, e);
                return false;
            }
        }

        /// <summary>
        /// 生成会诊单号
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string CreateConsultationNo()
        {
            //前缀精确到分钟，如果缓存也不会对业务超出多大影响
            var Prefix = DateTime.Now.ToString("yyyyMMddHHmm");
            //获取种子
            var Seq = GlobalConsultationSeq(Prefix);
            //获取8为序号，不够的前面填充0
            var Suffix = Seq.ToString().PadLeft(4, '0');

            string consultationNo = string.Format("RC{0}{1}", Prefix, Suffix);
            return consultationNo;
        }

        private int GlobalConsultationSeq(string Suffix)
        {
            if (Manager.Instance != null)
            {
                var no = (int)(Manager.Instance.StringIncrement("SEQ:ConsultationNo:" + Suffix));
                Manager.Instance.ExpireEntryAt("SEQ:ConsultationNo:" + Suffix, DateTime.Now.AddMinutes(2) - DateTime.Now);
                return no;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 会诊操作日志
        /// </summary>
        /// <param name="consultationID"></param>
        /// <param name="consultationOperationType"></param>
        /// <param name="orderState"></param>
        /// <param name="consultationProgress"></param>
        /// <param name="remark"></param>
        /// <param name="currentOperatorUserID"></param>
        /// <param name="orgID"></param>
        /// <returns></returns>
        public ConsultationLog CreateOperationLog(string consultationID
            , EnumConsultationOperationType consultationOperationType
            , EnumOPDState opdState
            , EnumConsultationProgress consultationProgress
            , string remark
            , string currentOperatorUserID
            , string orgID)
        {
            //操作日志
            var operationLog = new ConsultationLog()
            {
                ConsultationLogID = Guid.NewGuid().ToString("N"),
                CreateUserID = currentOperatorUserID,
                CreateTime = DateTime.Now,
                OrgID = orgID,
                ConsultationID = consultationID,
                OperationType = consultationOperationType,
                OPDState = opdState,
                Remark = remark,
                ConsultationProgress = consultationProgress,
            };

            return operationLog;
        }

        /// <summary>
        /// 根据修改的数据获取操作类型
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public dynamic GetOperationType(RemoteConsultation entity, RequestConsultationInfoUpdateDTO model)
        {
            var operationType = EnumConsultationOperationType.Edit;
            var message = "修改会诊资料";

            try
            {
                if (model.Doctors != null && model.Doctors.Count > 0)
                {
                    using (DBEntities db = new DBEntities())
                    {
                        //原分配的医生
                        var doctors = (from item in db.ConsultationDoctors.Where(i => i.ConsultationID == entity.ConsultationID && i.IsDeleted == false)
                                       select new
                                       {
                                           item.IsAttending,
                                           item.DoctorID,
                                           item.DoctorName,
                                       }).ToList();
                        //现分配的医生
                        var attendingDoctorID = model.Doctors.Where(i => i.IsAttending).Select(i => i.DoctorID).FirstOrDefault();
                        var doctorList = (from doctor in model.Doctors
                                          select new
                                          {
                                              doctor.DoctorID,
                                              doctor.DoctorName,
                                              IsAttending = attendingDoctorID != null && attendingDoctorID == doctor.DoctorID ? true : false
                                          }).ToList();
                        var count = (from oldDoctor in doctors.Where(i => i.IsAttending == false)
                                     join currDoctor in model.Doctors.Where(i => i.IsAttending == false) on oldDoctor.DoctorID equals currDoctor.DoctorID
                                     select currDoctor).ToList().Count();
                        //会诊医生是否发生变化
                        if (doctors.Count(i => i.IsAttending == false) != model.Doctors.Count(i => i.IsAttending == false)
                            || doctors.Count(i => i.IsAttending == false) != count)
                        {
                            operationType = EnumConsultationOperationType.Specialist;
                            int j = 0;
                            message = "会诊已申请专家：" + string.Join("；", doctorList.Where(i => i.IsAttending == false).Select(i => i.DoctorName));// + "，" + i.HospitalName + "，" + i.DepartmentName + "，" + (int.TryParse(i.Title, out j) ? ((EnumDoctorTitle)j).GetEnumDescript() : i.Title)));
                            return new { OperationType = operationType, Message = message };
                        }
                        else
                        {
                            //主诊医生是否发生变化
                            var attendingDoctor = doctors.Where(i => i.IsAttending == true).FirstOrDefault();
                            var currAttendingDoctor = doctorList.Where(i => i.IsAttending == true).FirstOrDefault();
                            if ((attendingDoctor == null && currAttendingDoctor != null) ||
                                (attendingDoctor != null && currAttendingDoctor == null) ||
                                (attendingDoctor != null && currAttendingDoctor != null && attendingDoctor.DoctorID != currAttendingDoctor.DoctorID))
                            {
                                operationType = EnumConsultationOperationType.Dispatch;
                                int j = 0;
                                if (currAttendingDoctor == null && attendingDoctor != null)
                                    message = "撤回医生服务人员：" + attendingDoctor.DoctorName;
                                else
                                    message = "指派给医生服务人员：" + currAttendingDoctor.DoctorName;// + "，" + currAttendingDoctor.HospitalName + "，" + currAttendingDoctor.DepartmentName + "，" + (int.TryParse(currAttendingDoctor.Title, out j) ? ((EnumDoctorTitle)j).GetEnumDescript() : currAttendingDoctor.Title);
                                return new { OperationType = operationType, Message = message };
                            }
                        }
                    }
                }

            }
            catch (Exception ee)
            {

            }

            return new { OperationType = operationType, Message = message };

        }


        public bool ModifyInspectResult(string fileID, string patientID, string studyUID, DBEntities db = null)
        {
            bool dbPrivateFlag = false;
            if (db == null)
            {
                db = CreateDb();
                dbPrivateFlag = true;
            }

            var model = db.UserInspectResults.Where(x => x.InspectResultID == fileID).FirstOrDefault();
            model.CaseID = patientID;
            model.StuUID = studyUID;
            model.ModifyTime = DateTime.Now;

            var ret = true;
            if (dbPrivateFlag)
            {
                ret = db.SaveChanges() > 0;
                db.Dispose();
            }

            return true;
        }

        public EnumApiStatus ModifyMedicalRecords(RequestMedicalRecordUpdateDTO model, ref ResponseConsultationLogDTO logDto, DBEntities db = null)
        {
            bool dbPrivateFlag = false;
            if (db == null)
            {
                db = CreateDb();
                dbPrivateFlag = true;
            }

            //订单不存在
            var entity = db.RemoteConsultations.Where(i => i.ConsultationID == model.ConsultationID && i.IsDeleted == false).FirstOrDefault();
            if (entity == null)
                return EnumApiStatus.ConsultationNotExists;
            //该状态不能修改
            if (entity.ConsultationProgress == EnumConsultationProgress.Finished ||
                entity.ConsultationProgress == EnumConsultationProgress.Invalid)
                return EnumApiStatus.CurrStatusNotModify;

            //病历
            var medicalEntity = db.UserMedicalRecords.Where(i => i.OPDRegisterID == entity.OPDRegisterID).FirstOrDefault();
            if (medicalEntity == null)
                return EnumApiStatus.BizError;
            //修改病历
            var medical = model.MedicalRecord;
            medicalEntity.Advised = medical.Advised;
            medicalEntity.AllergicHistory = medical.AllergicHistory;
            medicalEntity.ConsultationSubject = medical.ConsultationSubject;
            medicalEntity.FamilyMedicalHistory = medical.FamilyMedicalHistory;
            medicalEntity.Description = medical.Description;
            medicalEntity.PastMedicalHistory = medical.PastMedicalHistory;
            medicalEntity.PastOperatedHistory = medical.PastOperatedHistory;
            medicalEntity.PresentHistoryIllness = medical.PresentHistoryIllness;
            medicalEntity.Sympton = medical.Sympton;
            medicalEntity.PreliminaryDiagnosis = medical.PreliminaryDiagnosis;
            medicalEntity.ModifyTime = DateTime.Now;
            medicalEntity.ModifyUserID = model.CurrentOperatorUserID;

            //修改病历文件
            var userFiles = db.UserFiles.Where(i => i.OutID == medicalEntity.UserMedicalRecordID);
            db.UserFiles.RemoveRange(userFiles);
            if (model.Files != null)
            {
                model.Files.ForEach(i =>
                {
                    db.UserFiles.Add(new UserFile()
                    {
                        FileID = Guid.NewGuid().ToString("N"),
                        FileName = i.FileUrl,
                        FileUrl = i.FileUrl,
                        FileType = 0,
                        Remark = i.Remark,
                        OutID = medicalEntity.UserMedicalRecordID,
                        UserID = medicalEntity.UserID
                    });
                });
            }

            //操作日志
            var operationLog = CreateOperationLog(model.ConsultationID, EnumConsultationOperationType.EditMedical, EnumOPDState.NoPay, entity.ConsultationProgress, "修改病历资料", model.CurrentOperatorUserID, model.OrgID);
            logDto = operationLog.Map<ConsultationLog, ResponseConsultationLogDTO>();
            var ret = true;
            if (dbPrivateFlag)
            {
                ret = db.SaveChanges() > 0;
                db.Dispose();
            }

            if (ret)
            {
                return EnumApiStatus.BizOK;
            }
            else
            {
                return EnumApiStatus.BizError;
            }
        }

    }
}
