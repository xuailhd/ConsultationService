using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Novacode;
using Aspose.Words;
using System.Reflection;
using System.Collections;
using HealthCloud.Consultation.Dto.Common;
using HealthCloud.Consultation.Dto.Response;
using HealthCloud.Consultation.Dto.Request;
using HealthCloud.Consultation.Repositories;
using HealthCloud.Consultation.Enums;
using HealthCloud.Common.EventBus;
using HealthCloud.Consultation.Dto.EventBus;
using HealthCloud.Consultation.Repositories.EF;
using HealthCloud.Consultation.Models;
using HealthCloud.Consultation.Dto.Notice;
using HealthCloud.Common;
using HealthCloud.Common.Log;
using HealthCloud.Common.Extensions;
using HealthCloud.Consultation.IServices;
using HealthCloud.Consultation.Services.QQCloudy;

namespace HealthCloud.Consultation.Services
{
    /// <summary>
    /// 远程会诊
    /// </summary>
    public class RemoteConsultationService
    {
        private readonly RemoteConsultationRepository remoteConsultationRepository;
        private readonly ConversationRoomRepository conversationRoomRepository;
        private readonly IIMHepler imService;
        public RemoteConsultationService()
        {
            remoteConsultationRepository = new RemoteConsultationRepository();
            conversationRoomRepository = new ConversationRoomRepository();
            imService = new IMHelper();
        }

        /// <summary>
        /// 会诊单列表
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public PagedList<ResponseConsultationInfoDTO> GetList(RequestConsultationSearchDTO condition)
        {
            return remoteConsultationRepository.GetList(condition);
        }

        /// <summary>
        /// 会诊详情
        /// </summary>
        /// <returns></returns>
        public ResponseConsultationInfoDTO GetEntity(string consultationID, string CurrentOperatorUserID)
        {
            return remoteConsultationRepository.GetEntity(consultationID);
        }

        /// <summary>
        /// 添加或修改会诊单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public EnumApiStatus Modify(RequestConsultationInfoUpdateDTO model)
        {

            #region 基本验证
            model.Doctors = model.Doctors ?? new List<RequestConsultationDoctorDTO>();
            //把主诊医生排到最前
            model.Doctors = model.Doctors.OrderByDescending(i => i.IsAttending).ToList();
            //过滤重复的医生
            var doctorIds = model.Doctors.Select(i => i.DoctorID).Distinct().ToList();
            var newDoctors = new List<RequestConsultationDoctorDTO>();
            doctorIds.ForEach(doctorID =>
            {
                newDoctors.Add(model.Doctors.Where(i => i.DoctorID == doctorID).First());
            });
            model.Doctors = newDoctors;

            //只能有一个主诊医生
            if (model.Doctors.Count(i => i.IsAttending == true) > 1)
                return EnumApiStatus.OnlyAttendingDoctorOne;

            //会诊医生不能大于6个
            if (model.Doctors.Count > 6)
                return EnumApiStatus.DoctorCountGTSix;

            //会诊进度
            model.ConsultationProgress = GetConsultationProgress(model);

            #endregion
            ResponseConsultationLogDTO logDto = null;

            var ret = remoteConsultationRepository.Modify(model, ref logDto);

            if (ret == EnumApiStatus.BizOK)
            {
                using (MQChannel channel = new MQChannel())
                {
                    if (channel.Publish(new ConsultationOperationEvent()
                    {
                        ConsultationID = logDto.ConsultationID,
                        OperationType = logDto.OperationType,
                        OperationLog = logDto
                    }))
                    {
                        return EnumApiStatus.BizOK;
                    }
                    else
                    {
                        return EnumApiStatus.BizError;
                    }
                }
            }
            return ret;
        }

        
        public bool ModifyInspectResult(string fileID, string patientID, string studyUID)
        {
            return remoteConsultationRepository.ModifyInspectResult(fileID, patientID, studyUID);
        }

        /// <summary>
        /// 修改会诊单的病历资料
        /// </summary>
        /// <returns></returns>
        public EnumApiStatus ModifyMedicalRecords(RequestMedicalRecordUpdateDTO model)
        {
            ResponseConsultationLogDTO logDto = null;
            var ret = remoteConsultationRepository.ModifyMedicalRecords(model, ref logDto);

            if (ret == EnumApiStatus.BizOK)
            {
                using (MQChannel channel = new MQChannel())
                {
                    channel.BeginTransaction();

                    if (channel.Publish(new ConsultationOperationEvent()
                    {
                        ConsultationID = logDto.ConsultationID,
                        OperationType = logDto.OperationType,
                        OperationLog = logDto
                    }))
                    {
                        return EnumApiStatus.BizOK;
                    }
                    else
                    {
                        return EnumApiStatus.BizError;
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// 设置会诊就绪(就绪后不能再编辑)
        /// </summary>
        /// <param name="consultationID"></param>
        /// <returns></returns>
        public EnumApiStatus SetArranged(RequestConsultationSetDTO request)
        {
            using (DBEntities db = new DBEntities())
            {
                var entity = db.RemoteConsultations.Where(i => i.ConsultationID == request.ConsultationID).FirstOrDefault();
                //会诊单不存在
                if (entity == null)
                    return EnumApiStatus.ConsultationNotExists;

                //已就绪，会诊中，已完成的单直接操作成功
                if (entity.ConsultationProgress == EnumConsultationProgress.Arranged ||
                    entity.ConsultationProgress == EnumConsultationProgress.InProgress ||
                    entity.ConsultationProgress == EnumConsultationProgress.Finished ||
                    entity.ConsultationProgress == EnumConsultationProgress.Invalid)
                    return EnumApiStatus.BizOK;

                //会诊目的和要求不能为空
                if (string.IsNullOrEmpty(entity.Purpose) || string.IsNullOrEmpty(entity.Requirement))
                    return EnumApiStatus.PurposeNotEmpty;

                //是否分配了主诊医生和会诊专家
                var consultationDoctors = db.ConsultationDoctors.Where(i => i.ConsultationID == entity.ConsultationID && i.IsDeleted == false).Select(i => i.IsAttending).ToList();
                consultationDoctors = consultationDoctors ?? new List<bool>();
                //还未分配主诊医生
                if (!consultationDoctors.Contains(true))
                    return EnumApiStatus.NoAssignedDoctor;
                //还未分配会诊专家
                if (!consultationDoctors.Contains(false))
                    return EnumApiStatus.NoAssignedSpecialty;

                //修改状态
                var oldAmount = entity.Amount;
                entity.ConsultationProgress = EnumConsultationProgress.Arranged;
                if (request.Amount.HasValue)
                    entity.Amount = request.Amount.Value;
                //默认会诊时间
                if (entity.StartTime == null)
                    entity.StartTime = DateTime.Now.AddMinutes(30);
                if (entity.FinishTime == null)
                    entity.FinishTime = entity.StartTime.Value.AddHours(1);
                //创建房间数据
                db.ConversationRooms.Add(GetRoomEntity(entity.ConsultationID));

                //操作日志
                var operationLog = new ResponseConsultationLogDTO()
                {
                    ConsultationLogID = Guid.NewGuid().ToString("N"),
                    CreateUserID = request.CurrentOperatorUserID,
                    OperationUserName = request.CurrentOperationUserName,
                    CreateTime = DateTime.Now,
                    OrgID = request.OrgId,
                    ConsultationID = entity.ConsultationID,
                    OperationType = EnumConsultationOperationType.Confirm,
                    OPDState = EnumOPDState.NoPay,
                    Remark = $"确认会诊单，发起付款，原价为{oldAmount}元，现价为{entity.Amount}元",
                    ConsultationProgress = entity.ConsultationProgress,
                };

                //创建订单

                var userID = db.UserOPDRegisters.Where(a => a.OPDRegisterID == entity.OPDRegisterID).Select(i => i.UserID).FirstOrDefault(); //成员用户ID
                using (MQChannel channel = new MQChannel())
                {
                    channel.BeginTransaction();

                    if (channel.Publish(new ConsultationOperationEvent()
                    {
                        ConsultationID = entity.ConsultationID,
                        OperationType = operationLog.OperationType,
                        OperationLog = operationLog
                    }))
                    {
                        if (db.SaveChanges() > 0)
                        {
                            channel.Commit();
                            // 推送订单通知
                            SendOrderNotice(db, request.ConsultationID, request.CurrentOperatorUserID);
                            return EnumApiStatus.BizOK;
                        }
                    }
                }

                return EnumApiStatus.BizError;
            }
        }

        /// <summary>
        /// 取消会诊单
        /// </summary>
        /// <param name="consultationID"></param>
        /// <returns></returns>
        public EnumApiStatus Cancel(RequestConsultationSetDTO request)
        {
            using (DBEntities db = new DBEntities())
            {
                var entity = db.RemoteConsultations.Where(i => i.ConsultationID == request.ConsultationID).FirstOrDefault();
                //会诊单不存在
                if (entity == null)
                    return EnumApiStatus.ConsultationNotExists;

                //会诊中，已完成，已失效的单直接操作成功
                if (entity.ConsultationProgress == EnumConsultationProgress.InProgress ||
                    entity.ConsultationProgress == EnumConsultationProgress.Finished ||
                    entity.ConsultationProgress == EnumConsultationProgress.Invalid)
                    return EnumApiStatus.BizOK;

                var oldConsultationProgress = entity.ConsultationProgress;
                entity.ConsultationProgress = EnumConsultationProgress.Invalid;

                //操作日志
                var operationLog = CreateOperationLog(entity.ConsultationID, EnumConsultationOperationType.Cancel, EnumOPDState.NoPay, 
                    entity.ConsultationProgress, "取消订单", request.CurrentOperatorUserID, request.CurrentOperationUserName, request.OrgId);

                using (MQChannel channel = new MQChannel())
                {
                    channel.BeginTransaction();

                    if (channel.Publish(new ConsultationOperationEvent()
                    {
                        ConsultationID = entity.ConsultationID,
                        OperationType = operationLog.OperationType,
                        OperationingConsultationProgress = oldConsultationProgress,
                        CurrentOperatorUserID = request.CurrentOperatorUserID,
                        OperationLog = operationLog
                    }))
                    {
                        if (db.SaveChanges() > 0)
                        {
                            channel.Commit();

                            return EnumApiStatus.BizOK;
                        }
                    }
                }

                return EnumApiStatus.BizError;
            }
        }

        /// <summary>
        /// 会诊单线下支付
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public EnumApiStatus OfflinePay(RequestConsultationOfflinePayDTO request)
        {
            if (request.Files == null || request.Files.Count == 0)
                return EnumApiStatus.NoPayedFile;


            using (DBEntities db = new DBEntities())
            {
                //订单已取消，不能支付

                var consultmodel = db.RemoteConsultations.Where(i => i.ConsultationID == request.ConsultationID).FirstOrDefault();

                if (consultmodel == null || consultmodel.ConsultationProgress == EnumConsultationProgress.Invalid)
                {
                    return EnumApiStatus.InvalidNoPay;
                }

                var opd = db.UserOPDRegisters.Where(t => t.OPDRegisterID == consultmodel.OPDRegisterID).FirstOrDefault();

                if (opd == null || opd.OPDState == EnumOPDState.NoPay)
                {
                    return EnumApiStatus.InvalidNoPay;
                }


                //已付款
                var isPayed = new SysDereplicationService().Exists(request.ConsultationID, "Orders", EnumDereplicationType.OfflinePayed);
                if (isPayed == true)
                    return EnumApiStatus.OfflinePayed;

                #region 添加文件
                request.Files.ForEach(i =>
                {
                    db.UserFiles.Add(new UserFile()
                    {
                        FileID = Guid.NewGuid().ToString("N"),
                        FileName = i.FileUrl,
                        FileUrl = i.FileUrl,
                        FileType = 0,
                        Remark = "OfflinePayFile",
                        OutID = request.ConsultationID,
                        UserID = request.CurrentOperatorUserID,
                        CreateUserID = request.CurrentOperatorUserID,
                        CreateTime = DateTime.Now
                    });
                });
                #endregion

                #region 记录该订单已支付
                db.SysDereplications.Add(new SysDereplication
                {
                    SysDereplicationID = Guid.NewGuid().ToString("N"),
                    TableName = "Orders",
                    OutID = request.ConsultationID,
                    DereplicationType = EnumDereplicationType.OfflinePayed,
                    CreateTime = DateTime.Now
                });
                #endregion

                opd.OPDState = EnumOPDState.NoReply;


                if (db.SaveChanges() > 0)
                {
                    return EnumApiStatus.BizOK;
                }
                else
                    return EnumApiStatus.BizOK;

            }
        }

        /// <summary>
        /// 线下退款操作
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public EnumApiStatus OfflineRefund(RequestConsultationOfflinePayDTO request)
        {

            if (request.Files == null || request.Files.Count == 0)
                return EnumApiStatus.NoPayedFile;

            using (DBEntities db = new DBEntities())
            {
                //已退款
                var isRefunded = new SysDereplicationService().Exists(request.ConsultationID, "Orders", EnumDereplicationType.OfflineRefund);
                if (isRefunded == true)
                    return EnumApiStatus.OfflineRefunded;

                #region 添加文件
                request.Files.ForEach(i =>
                {
                    db.UserFiles.Add(new UserFile()
                    {
                        FileID = Guid.NewGuid().ToString("N"),
                        FileName = i.FileUrl,
                        FileUrl = i.FileUrl,
                        FileType = 0,
                        Remark = "OfflineRefundFile",
                        OutID = request.ConsultationID,
                        UserID = request.CurrentOperatorUserID,
                        CreateUserID = request.CurrentOperatorUserID,
                        CreateTime = DateTime.Now
                    });
                });
                #endregion

                #region 记录该订单已支付
                db.SysDereplications.Add(new SysDereplication
                {
                    SysDereplicationID = Guid.NewGuid().ToString("N"),
                    TableName = "Orders",
                    OutID = request.ConsultationID,
                    DereplicationType = EnumDereplicationType.OfflineRefund,
                    CreateTime = DateTime.Now
                });
                #endregion

                //操作日志
                var operationLog = CreateOperationLog(request.ConsultationID, EnumConsultationOperationType.Refund,
                    EnumOPDState.NoPay, EnumConsultationProgress.Invalid, "订单退款", request.CurrentOperatorUserID
                    , request.CurrentOperatorUserName, request.OrgID);

                using (MQChannel channel = new MQChannel())
                {
                    channel.BeginTransaction();

                    if (channel.Publish(new ConsultationOperationEvent()
                    {
                        ConsultationID = request.ConsultationID,
                        OperationType = operationLog.OperationType,
                        CurrentOperatorUserID = request.CurrentOperatorUserID,
                        TradeNo = request.TradeNo,
                        OperationLog = operationLog
                    }))
                    {
                        if (db.SaveChanges() > 0)
                        {
                            channel.Commit();

                            return EnumApiStatus.BizOK;
                        }
                    }
                }

                return EnumApiStatus.BizError;
            }
        }


        /// <summary>
        ///  推送订单消息给主诊医生
        /// </summary>
        /// <param name="request"></param>
        private void SendOrderNotice(DBEntities db, string consultationID, string currentOperatorUserID)
        {
            try
            {
                var query = from consultationDoctor in db.ConsultationDoctors
                            where consultationDoctor.ConsultationID == consultationID &&
                            consultationDoctor.IsDeleted == false
                            select new { consultationDoctor.DoctorID, consultationDoctor.IsAttending, consultationDoctor.DoctorName };

                string sendUserID = query.Where(x => x.IsAttending).Select(x => x.DoctorID).FirstOrDefault();
                if (string.IsNullOrEmpty(sendUserID))
                    return;


                string doctorName = "";
                query.Where(x => !x.IsAttending).Select(x => x.DoctorName).ToList().ForEach(x => doctorName += x + "、");
                doctorName = doctorName.TrimEnd('、');


                string content = GetNoticeContentFromTemplate(EnumNoticeSecondType.RemoteConsultationOrderNotice,
                new
                {
                    DoctorName = doctorName,
                    Price = db.RemoteConsultations.Where(x => x.ConsultationID == consultationID).Select(x => x.Amount).FirstOrDefault().ToString()
                });

                NoticeService notice = new NoticeService();
                if (!notice.SendNotice(new RequestNoticeMessageDTO
                {
                    Title = "",
                    Summary = content,
                    Content = content,
                    NoticeSecondType = EnumNoticeSecondType.RemoteConsultationOrderNotice,

                },
                FromUserID: "",
                ToUserListType: EnumTargetUserCodeType.UserID,
                ToUserList: new List<string> { sendUserID }))
                {
                    //throw new Exception("推送远程会诊订单付款通知出现异常");
                }
            }
            catch (Exception e)
            {

                //throw new Exception("推送远程会诊订单付款通知出现异常：" + e.Message);

            }
        }

        /// <summary>
        /// 通过消息模板生成消息内容
        /// </summary>
        /// <param name="type"></param>
        /// <param name="templData"></param>
        /// <param name="currentOperatorUserID"></param>
        /// <returns></returns>
        private string GetNoticeContentFromTemplate(EnumNoticeSecondType type, object templData)
        {
            NoticeService noticeService = new NoticeService();
            var config = noticeService.GetMessageExtrasConfig(EnumTerminalType.Web, type);
            string content = config == null ? null : config.MsgTitle;
            if (string.IsNullOrEmpty(content))
                return null;

            foreach (PropertyInfo p in templData.GetType().GetProperties())
                content = content.Replace("$" + p.Name.Replace("@", ""), p.GetValue(templData) as string);

            return content;
        }

        /// <summary>
        /// 会诊建议和总结
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Comments(RequestConsultationCommentsDTO model)
        {
            using (DBEntities db = new DBEntities())
            {
                var entity = db.ConsultationDoctors.Where(i => i.ConsultationID == model.ConsultationID && i.DoctorID == model.ConsultationDoctorID && i.IsDeleted == false).FirstOrDefault();
                if (entity == null)
                    return false;

                var consultation = db.RemoteConsultations.Where(i => i.ConsultationID == model.ConsultationID && i.IsDeleted == false).FirstOrDefault();
                if (consultation.ConsultationProgress != EnumConsultationProgress.InProgress)
                    return false;

                //操作日志
                var operationLog = CreateOperationLog(consultation.ConsultationID, EnumConsultationOperationType.Comments, 
                    EnumOPDState.NoPay, consultation.ConsultationProgress, "专家提交会诊意见", 
                    model.CurrentOperatorUserID, model.CurrentOperatorUserName, model.OrgID);

                //主诊医生提交总结后，会诊单完成
                if (entity.IsAttending == true)
                {
                    //是否存在未填写总结的专家
                    var existsNoComment = db.ConsultationDoctors
                                          .Where(i => i.ConsultationID == model.ConsultationID &&
                                                      i.IsAttending == false &&
                                                      i.IsDeleted == false &&
                                                      (string.IsNullOrEmpty(i.Opinion) || string.IsNullOrEmpty(i.Perscription)))
                                          .Select(i => i.ConsultationDoctorID)
                                          .FirstOrDefault() != null ? true : false;
                    if (existsNoComment == true)
                        return false;

                    //提交病例
                    var userMedical = db.UserMedicalRecords.Where(i => i.OPDRegisterID == consultation.OPDRegisterID && i.IsDeleted == false).FirstOrDefault();

                    consultation.ConsultationProgress = EnumConsultationProgress.Finished;
                    consultation.FinishTimeReal = DateTime.Now;

                    //操作日志
                    operationLog.OperationType = EnumConsultationOperationType.Finished;
                    operationLog.ConsultationProgress = consultation.ConsultationProgress;
                    operationLog.Remark = "主诊医生提交会诊意见书，会诊结束";

                }
                else
                {
                    SendCommentNotice(db, model.ConsultationID, model.ConsultationDoctorID, model.CurrentOperatorUserID);
                }

                entity.Opinion = model.Opinion;
                entity.Perscription = model.Perscription;
                entity.ModifyUserID = model.CurrentOperatorUserID;
                entity.ModifyTime = DateTime.Now;

                using (MQChannel channel = new MQChannel())
                {
                    channel.BeginTransaction();

                    if (channel.Publish(new ConsultationOperationEvent()
                    {
                        ConsultationID = consultation.ConsultationID,
                        OperationType = operationLog.OperationType,
                        OperationLog = operationLog
                    }))
                    {
                        if (db.SaveChanges() > 0)
                        {
                            channel.Commit();
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// 推送整理会整意见给主诊医生
        /// </summary>
        /// <param name="db"></param>
        /// <param name="consultationID"></param>
        /// <param name="doctorID"></param>
        /// <param name="currentOperatorUserID"></param>
        private void SendCommentNotice(DBEntities db, string consultationID, string doctorID, string currentOperatorUserID)
        {
            try
            {
                var query = from consultation in db.RemoteConsultations
                            join opd in db.UserOPDRegisters on consultation.OPDRegisterID equals opd.OPDRegisterID
                            join consultationDoctor in db.ConsultationDoctors on consultation.ConsultationID equals consultationDoctor.ConsultationID
                            where consultationDoctor.ConsultationID == consultationID &&
                            consultation.IsDeleted == false && consultationDoctor.IsDeleted == false 
                            select new
                            {
                                DoctorID = consultationDoctor.DoctorID,
                                IsAttending = consultationDoctor.IsAttending,
                                DoctorName = consultationDoctor.DoctorName,
                                MemberName = opd.MemberName
                            };

                string sendUserID = query.Where(x => x.IsAttending).Select(x => x.DoctorID).FirstOrDefault();
                if (string.IsNullOrEmpty(sendUserID))
                    return;

                string content = GetNoticeContentFromTemplate(EnumNoticeSecondType.RemoteConsultationSummaryOpinionNotice,
                    new
                    {
                        DoctorName = query.Where(x => x.DoctorID == doctorID).Select(x => x.DoctorName).FirstOrDefault(),
                        PatientName = query.Where(x => x.DoctorID == doctorID).Select(x => x.MemberName).FirstOrDefault()
                    });

                if (string.IsNullOrEmpty(content))
                    return;

                NoticeService notice = new NoticeService();
                if (!notice.SendNotice(new RequestNoticeMessageDTO
                {
                    Title = "",
                    Summary = content,
                    Content = content,
                    NoticeSecondType = EnumNoticeSecondType.RemoteConsultationSummaryOpinionNotice,
                },
                FromUserID: "",
                ToUserListType: EnumTargetUserCodeType.UserID,
                ToUserList: new List<string> { sendUserID }))
                {
                    //throw new Exception("推送远程会诊整理会诊意见通知出现异常");
                }

            }
            catch (Exception e)
            {
                //throw new Exception("推送远程会诊整理会诊意见通知出现异常：" + e.Message);
            }
        }

        /// <summary>
        /// 会诊单，短信通知患者
        /// </summary>
        /// <param name="consultationID"></param>
        /// <param name="operationType"></param>
        /// <returns></returns>
        public bool SendSMSNotice(string consultationID, EnumConsultationOperationType operationType)
        {
            var consultation = GetEntity(consultationID, "");
            if (consultation == null)
                return true;

            //发送短信通知患者
            var config = SysConfigService.Get<Common.Config.Consultation>();

            //短信参数
            string servicePhone = "0755-22737150";
            string mobile = consultation.Member.Mobile;
            if (consultation.Member == null || string.IsNullOrEmpty(consultation.Member.Mobile)
                || !ToolHelper.CheckPhoneNumber(consultation.Member.Mobile))
            {
                LogHelper.DefaultLogger.Debug("手机号为空,取消发送");
                return true;
            }

            string consulNo = consultation.ConsultationNo;
            string memberName = consultation.Member.MemberName;
            string addresss = string.IsNullOrEmpty(consultation.Address) ? "无" : consultation.Address;
            string doctorNames = string.Join("、", consultation.Doctors.Where(i => i.IsAttending == false).Select(i => $"{i.HospitalName}，{i.DepartmentName}，{i.DoctorName}").ToList());
            string amount = "0";
            string consulTime = "";
            if (consultation.StartTime.HasValue && consultation.FinishTime.HasValue)
            {
                consulTime += consultation.StartTime.Value.ToString("yyyy年MM月dd日") + "(" + consultation.StartTime.Value.ToString("HH:mm") + "-" + consultation.FinishTime.Value.ToString("HH:mm") + ")";
            }

            int msgLogType = 6;
            string msgTemlateId, msgTemplateContent, msgTitle, msgContent, msgParms;
            switch (operationType)
            {
                case EnumConsultationOperationType.Create:
                    msgTemlateId = config.MsgConsulCreateNoticeTemlateId;
                    msgTemplateContent = config.MsgConsulCreateNoticeTemlateContent;
                    msgLogType = 6;
                    msgTitle = "会诊单创建通知";
                    msgContent = string.Format(msgTemplateContent, servicePhone);
                    msgParms = servicePhone;
                    break;

                case EnumConsultationOperationType.Confirm:
                    msgTemlateId = config.MsgConsulOnlinePayNoticeTemlateId;
                    msgTemplateContent = config.MsgConsulOnlinePayNoticeTemlateContent;
                    msgLogType = 7;
                    msgTitle = "会诊单线上付款通知";
                    amount = "￥" + consultation.Amount.ToString("0.00");
                    msgContent = string.Format(msgTemplateContent, consulNo, (doctorNames + "，" + consulTime), addresss, memberName, amount, servicePhone);
                    msgParms = $"{consulNo},{doctorNames + "，" + consulTime},{addresss},{memberName},{amount},{servicePhone}";
                    break;

                case EnumConsultationOperationType.Payed:
                    msgTemlateId = config.MsgConsulPaidNoticeTemlateId;
                    msgTemplateContent = config.MsgConsulPaidNoticeTemlateContent;
                    msgLogType = 8;
                    msgTitle = "会诊单支付成功通知";
                    msgContent = string.Format(msgTemplateContent, consulNo, (doctorNames + "，" + consulTime), addresss, memberName, servicePhone);
                    msgParms = $"{consulNo},{doctorNames + "，" + consulTime},{addresss},{memberName},{servicePhone}";
                    break;

                default:
                    return true;
            }

            var MsgParms = new List<string>();
            MsgParms.Add(msgParms);

            using (var mqChannel = new MQChannel())
            {
                mqChannel.Publish(new SMSSendEvent()
                {
                    Mobile = mobile,
                    TemplateID = msgTemlateId,
                    MsgType = msgLogType,
                    Content = msgContent,
                    MsgParms = MsgParms,
                    Title = msgTitle,
                });
            }
            return true;

        }

        /// <summary>
        /// 发送支付完成通知
        /// </summary>
        /// <param name="consultationID"></param>
        /// <returns></returns>
        public bool SendPaiedSMSNotice(string consultationID)
        {
            //当前会诊状态
            EnumConsultationProgress consultationProgress;
            using (DBEntities db = new DBEntities())
            {
                consultationProgress = db.RemoteConsultations.Where(i => i.ConsultationID == consultationID).Select(i => i.ConsultationProgress).FirstOrDefault();
            }

            //操作日志
            var operationLog = new ResponseConsultationLogDTO()
            {
                ConsultationLogID = Guid.NewGuid().ToString("N"),
                CreateUserID = "",
                CreateTime = DateTime.Now,
                OrgID = "",
                ConsultationID = consultationID,
                OperationType = EnumConsultationOperationType.Payed,
                OPDState = EnumOPDState.NoReply,
                ConsultationProgress = consultationProgress,
                Remark = "完成订单支付"
            };

            using (MQChannel channel = new MQChannel())
            {
                return channel.Publish(new ConsultationOperationEvent()
                {
                    ConsultationID = consultationID,
                    OperationType = operationLog.OperationType,
                    OperationLog = operationLog
                });
            }
        }

        /// <summary>
        /// 会诊开始
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public EnumApiStatus Start(string consultationID, string CurrentOperatorUserID, string CurrentOperatorUserName, string OrgId = "")
        {
            using (DBEntities db = new DBEntities())
            {
                var entity = db.RemoteConsultations.Where(i => i.ConsultationID == consultationID && i.IsDeleted == false).FirstOrDefault();
                if (entity == null)
                    return EnumApiStatus.ConsultationNotExists;

                //已处理
                if (entity.ConsultationProgress == EnumConsultationProgress.InProgress ||
                    entity.ConsultationProgress == EnumConsultationProgress.Finished ||
                    entity.ConsultationProgress == EnumConsultationProgress.Invalid)
                    return EnumApiStatus.BizOK;

                if (entity.ConsultationProgress != EnumConsultationProgress.Arranged)
                    return EnumApiStatus.ConsultationNotChangeProgress;

                var CurrentOperatorDoctorID = db.ConsultationDoctors.Where(i => i.DoctorID == CurrentOperatorUserID).Select(i => i.DoctorID).FirstOrDefault();
                //主诊医生ID
                var attendingDoctor = db.ConsultationDoctors.Where(i => i.ConsultationID == consultationID && i.IsAttending == true && i.IsDeleted == false).Select(i => i.DoctorID).FirstOrDefault();
                if (attendingDoctor != CurrentOperatorDoctorID)
                    return EnumApiStatus.ConsultationNotChangeProgress;

                entity.ConsultationProgress = EnumConsultationProgress.InProgress;
                entity.StartTimeReal = DateTime.Now;
                entity.ModifyTime = DateTime.Now;
                entity.ModifyUserID = CurrentOperatorUserID;

                //操作日志
                var operationLog = CreateOperationLog(entity.ConsultationID, EnumConsultationOperationType.Start, EnumOPDState.NoPay, entity.ConsultationProgress, "会诊开始", 
                    CurrentOperatorUserID, CurrentOperatorUserName, OrgId);

                using (MQChannel channel = new MQChannel())
                {
                    channel.BeginTransaction();

                    if (channel.Publish(new ConsultationOperationEvent()
                    {
                        ConsultationID = entity.ConsultationID,
                        OperationType = operationLog.OperationType,
                        OperationLog = operationLog
                    }))
                    {
                        if (db.SaveChanges() > 0)
                        {
                            channel.Commit();
                            return EnumApiStatus.BizOK;
                        }
                    }
                }

                return EnumApiStatus.BizError;
            }
        }

        /// <summary>
        /// 查询会诊日志
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public PagedList<ResponseConsultationLogDTO> GetOperationLogList(RequesConsultationLogSearchDTO request)
        {
            using (DBEntities db = new DBEntities())
            {
                var query = (from item in db.ConsultationLogs
                             where item.ConsultationID == request.ConsultationID && item.IsDeleted == false
                             select new ResponseConsultationLogDTO
                             {
                                 ConsultationProgress = item.ConsultationProgress,
                                 OperationType = item.OperationType,
                                 OPDState = item.OPDState,
                                 Remark = item.Remark,
                                 CreateTime = item.CreateTime,
                                 OperationUserName = item.OperationUserName
                             });

                query = query.OrderBy(i => i.CreateTime);
                return query.ToPagedList(request.CurrentPage, request.PageSize);
            }
        }

        /// <summary>
        /// 获取会诊状态
        /// </summary>
        /// <param name="consultationID"></param>
        /// <returns></returns>
        public EnumConsultationProgress GetProgress(string consultationID)
        {
            using (DBEntities db = new DBEntities())
            {
                var result = db.RemoteConsultations.Where(i => i.ConsultationID == consultationID && i.IsDeleted == false)
                    .Select(i => i.ConsultationProgress).FirstOrDefault();
                return result;
            }
        }

        /// <summary>
        /// 从数据判断会诊进度/状态
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private EnumConsultationProgress GetConsultationProgress(RequestConsultationInfoUpdateDTO model)
        {
            var haveSpecialist = false; //是否分配专家
            var haveAttending = false; //是否分配主诊医生
            if (model.Doctors != null)
            {
                model.Doctors.ForEach(i =>
                {
                    if (i.IsAttending == true)
                        haveAttending = true;
                    else
                        haveSpecialist = true;
                });
            }
            if (haveSpecialist == true)
                return EnumConsultationProgress.Specialist;
            if (haveAttending == true)
                return EnumConsultationProgress.Dispatch;

            return EnumConsultationProgress.Pending;

        }

        

        /// <summary>
        /// 房间实体
        /// </summary>
        /// <param name="ServiceID"></param>
        /// <returns></returns>
        private ConversationRoom GetRoomEntity(string ServiceID)
        {
            // 创建房间
            var room = new ConversationRoom();
            room.EndTime = DateTime.Now;
            room.BeginTime = DateTime.Now;
            room.TotalTime = 0;
            room.RoomState = EnumRoomState.NoTreatment;//状态
            room.ConversationRoomID = Guid.NewGuid().ToString("N");
            room.ServiceID = ServiceID;
            room.ServiceType = EnumDoctorServiceType.Consultation;
            room.RoomType = EnumRoomType.Group;
            room.TriageID = long.MaxValue;
            return room;
        }

        /// <summary>
        /// 创建群组
        /// </summary>
        /// <param name="ServiceID"></param>
        /// <returns></returns>
        public bool CreateIMRoom(string ServiceID)
        {
            try
            {
                using (var db = new DBEntities())
                {
                    var roomService = new ConversationRoomService();
                    List<int> identifiers = new List<int>();

                    #region 校验：会诊记录必须存在
                    //会诊数据
                    var entity = db.RemoteConsultations.FirstOrDefault(i => i.ConsultationID == ServiceID && i.IsDeleted == false);

                    if (entity == null)
                    {
                        return true;
                    }
                    #endregion

                    #region 校验：会诊必须有医生
                    var roomDoctors = db.ConsultationDoctors.Where(i => i.ConsultationID == ServiceID && i.IsDeleted == false).ToList();

                    if (roomDoctors.Count <= 0)
                    {
                        return false;
                    }
                    #endregion

                    #region 校验：是否为主诊医生
                    var attendingDoctor = roomDoctors.Where(a => a.IsAttending).FirstOrDefault();
                    if (attendingDoctor == null)
                    {
                        return false;
                    }
                    #endregion

                    #region 获取参加会诊所有医生的信息
                    var doctorInfos = (from doctor in roomDoctors
                                       join userUid in db.ConversationIMUids on doctor.DoctorID equals userUid.UserID
                                       select new
                                       {
                                           userUid.Identifier,
                                           DoctorName = doctor.DoctorName,
                                           DoctorID = doctor.DoctorID,
                                           PhotoUrl = doctor.PhotoUrl,
                                       }
                                   ).ToList().ToDictionary(a => a.DoctorID);
                    #endregion

                    #region 获取参加会诊的患者信息

                    var userInfos = (from opd in db.UserOPDRegisters
                                     join userUid in db.ConversationIMUids on opd.UserID equals userUid.UserID
                                     where opd.OPDRegisterID == entity.OPDRegisterID
                                     select new
                                     {
                                         userUid.Identifier,
                                         MemberID = opd.MemberID,
                                         MemberName = opd.MemberName,
                                         opd.PhotoUrl,
                                         UserID = opd.UserID
                                     }
                                 ).ToList();
                    #endregion

                    #region 校验：诊室是否村子，不存在则创建
                    //房间信息
                    var room = roomService.GetChannelInfo(ServiceID);

                    //房间不存在则创建
                    if (room == null)
                    {
                        // 创建房间
                        var roomEntity = new ConversationRoom();
                        roomEntity.EndTime = DateTime.Now;
                        roomEntity.BeginTime = DateTime.Now;
                        roomEntity.TotalTime = 0;
                        roomEntity.RoomState = EnumRoomState.NoTreatment;//状态
                        roomEntity.ConversationRoomID = Guid.NewGuid().ToString("N");
                        roomEntity.ServiceID = ServiceID;
                        roomEntity.ServiceType = EnumDoctorServiceType.Consultation;
                        roomEntity.RoomType = EnumRoomType.Group;
                        roomEntity.TriageID = long.MaxValue;
                        roomEntity.Enable = false;
                        db.ConversationRooms.Add(roomEntity);
                        if (db.SaveChanges() <= 0)
                        {
                            return false;
                        }
                        else
                        {
                            room = roomEntity.Map<ConversationRoom, ResponseConversationRoomDTO>();
                        }
                    }


                    #endregion


                    if (room != null)
                    {
                        identifiers.AddRange(doctorInfos.Select(a => a.Value.Identifier));
                        identifiers.AddRange(userInfos.Select(a => a.Identifier));

                        #region 创建IM群组
                        var GroupName = EnumDoctorServiceType.Consultation.GetEnumDescript();
                        var Introduction = "";
                        var Notification = "";
                        if (imService.CreateGroup(room.ConversationRoomID, GroupName, EnumDoctorServiceType.Consultation, identifiers, Introduction, Notification))
                        {
                            room.Enable = true;
                            if (conversationRoomRepository.UpdateRoomEable(room.ConversationRoomID, room.Enable))
                            {
                                var members = new List<RequestChannelMemberDTO>();

                                foreach (var doctorInfo in doctorInfos)
                                {
                                    members.Add(new RequestChannelMemberDTO()
                                    {
                                        UserType = EnumUserType.Doctor,
                                        UserID = doctorInfo.Value.DoctorID,
                                        Identifier = doctorInfo.Value.Identifier,
                                        UserMemberID = "",
                                        PhotoUrl = doctorInfo.Value.PhotoUrl,
                                        UserCNName = doctorInfo.Value.DoctorName,
                                        UserENName = doctorInfo.Value.DoctorName,
                                    });
                                }

                                foreach (var userInfo in userInfos)
                                {
                                    members.Add(new RequestChannelMemberDTO()
                                    {
                                        Identifier = userInfo.Identifier,
                                        UserType = EnumUserType.User,
                                        UserID = userInfo.UserID,
                                        UserMemberID = entity.MemberID,
                                        PhotoUrl = userInfo.PhotoUrl,
                                        UserCNName = userInfo.MemberName,
                                        UserENName = userInfo.MemberName,
                                    });
                                }

                                return conversationRoomRepository.ReplaceChannelMembers(room.ConversationRoomID, members);
                            }
                        }
                        #endregion
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.DefaultLogger.Error(ex.Message, ex);
                return false;
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
        public ResponseConsultationLogDTO CreateOperationLog(string consultationID
            , EnumConsultationOperationType consultationOperationType
            , EnumOPDState opdState
            , EnumConsultationProgress consultationProgress
            , string remark
            , string currentOperatorUserID
            , string operationUserName
            , string orgID
            )
        {
            //操作日志
            var operationLog = new ResponseConsultationLogDTO()
            {
                ConsultationLogID = Guid.NewGuid().ToString("N"),
                CreateUserID = currentOperatorUserID,
                OperationUserName = operationUserName,
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
        /// 记录操作日志
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool InsertOperationLog(ConsultationLog entity)
        {
            using (var db = new DBEntities())
            {
                db.ConsultationLogs.Add(entity);
                return db.SaveChanges() > 0 ? true : false;
            }
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
                                           item.DepartmentName,
                                           item.HospitalName
                                       }).ToList();
                        //现分配的医生
                        var attendingDoctorID = model.Doctors.Where(i => i.IsAttending).Select(i => i.DoctorID).FirstOrDefault();
                        var doctorList = (from doctor in model.Doctors
                                          select new
                                          {
                                              doctor.DoctorID,
                                              doctor.DoctorName,
                                              doctor.HospitalName,
                                              doctor.DepartmentName,
                                              doctor.Title,
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
                            message = "会诊已申请专家：" + string.Join("；", doctorList.Where(i => i.IsAttending == false).Select(i => i.DoctorName + "，" + i.HospitalName + "，" + i.DepartmentName + 
                            "，" + i.Title));
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
                                    message = "指派给医生服务人员：" + currAttendingDoctor.DoctorName + "，" + currAttendingDoctor.HospitalName + "，" + currAttendingDoctor.DepartmentName + 
                                        "，" + currAttendingDoctor.Title;
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

        /// <summary>
        /// 导出会诊报告单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MemoryStream GetWordFileStream(ResponseConsultationInfoDTO info, string tempPath)
        {
            DocX doc;
            try
            {
                doc = DocX.Load(tempPath);
                //给域赋值
                if (doc.Tables != null && doc.Tables.Count > 0)
                {
                    Table table = doc.Tables[0];
                    //第一行
                    var row1 = table.Rows[0];
                    row1.Cells[0].Paragraphs[0].RemoveText(0);
                    row1.Cells[0].Paragraphs[0].Append(info.Member.MemberName + "的会诊意见书").FontSize(24).Bold();
                    //第二行
                    var row2 = table.Rows[1];
                    row2.Cells[1].Paragraphs[0].Append(info.Member.MemberName).FontSize(12);
                    row2.Cells[3].Paragraphs[0].Append(info.Member.Gender.GetEnumDescript()).FontSize(12);
                    row2.Cells[5].Paragraphs[0].Append(info.Member.Age.ToString()).FontSize(12);
                    //第三行
                    var row3 = table.Rows[2];
                    row3.Cells[1].Paragraphs[0].Append(info.Member.IDNumber).FontSize(12);
                    row3.Cells[3].Paragraphs[0].Append(info.Member.Mobile).FontSize(12);
                    //第四行
                    var row4 = table.Rows[3];
                    //第五行
                    var row5 = table.Rows[4];
                    var attendingDoctor = info.Doctors.Where(j => j.IsAttending == true).FirstOrDefault();
                    attendingDoctor = attendingDoctor ?? new ResponseDoctorInfoDTO();
                    row5.Cells[1].Paragraphs[0].Append(attendingDoctor.DoctorName).FontSize(12);
                    row5.Cells[3].Paragraphs[0].Append(info.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")).FontSize(12);
                    //第六行
                    var row6 = table.Rows[5];
                    row6.Cells[1].Paragraphs[0].Append(info.MedicalRecord.ConsultationSubject).FontSize(12);
                    //第七行
                    var row7 = table.Rows[6];
                    row7.Cells[1].Paragraphs[0].Append(info.MedicalRecord.PreliminaryDiagnosis).FontSize(12);
                    //第八行
                    var row8 = table.Rows[7];
                    row8.Cells[1].Paragraphs[0].Append(info.Purpose).FontSize(12)
                                               .AppendLine(info.Requirement).FontSize(12);

                    //专家
                    var specialtyDoctor = info.Doctors.Where(j => j.IsAttending == false).ToList();
                    //第九行
                    var specialtyTitleRow = table.Rows[8];
                    var specialtyYJRow = table.Rows[9];
                    var specialtyFARow = table.Rows[10];
                    var specialtyQMRow = table.Rows[11];

                    var attendingTitleRow = table.Rows[12];
                    var attendingYJRow = table.Rows[13];
                    var attendingFARow = table.Rows[14];
                    var attendingQMRow = table.Rows[15];
                    int i = 0;
                    //会诊专家
                    for (i = 0; i < specialtyDoctor.Count; i++)
                    {
                        var doctor = specialtyDoctor[i];
                        table.InsertRow(specialtyTitleRow).Cells[0].Paragraphs[0].ReplaceText("XXX", doctor.DoctorName);
                        table.InsertRow(specialtyYJRow).Cells[1].Paragraphs[0].Append(doctor.Opinion).FontSize(12);
                        table.InsertRow(specialtyFARow).Cells[1].Paragraphs[0].Append(doctor.Perscription).FontSize(12);
                        var newRow = table.InsertRow(specialtyQMRow);
                        //医生签名图片
                        //byte[] byteData = null;
                        //if (!string.IsNullOrEmpty(doctor.SignatureUrl) &&
                        //    doctor.SignatureUrl.ToLower().StartsWith("http") &&
                        //    (byteData = KMEHosp.Common.Utility.WebAPIHelper.DownloadData(doctor.SignatureUrl)) != null)
                        //{
                        //    Novacode.Image img = doc.AddImage(new MemoryStream(byteData));
                        //    Picture pic = img.CreatePicture();
                        //    pic.Width = 65;
                        //    pic.Height = 40;
                        //    newRow.Cells[1].Paragraphs[0].AppendPicture(pic);
                        //}
                        //else
                        //{
                            newRow.Cells[1].Paragraphs[0].Append(doctor.DoctorName);
                        //}
                        //if (doctor.ModifyTime != null)
                        //{
                        //    newRow.Cells[3].Paragraphs[0].Append(doctor.ModifyTime.Value.ToString("yyyy-MM-dd"));
                        //}
                    }

                    //主诊医生
                    table.InsertRow(attendingTitleRow).Cells[0].Paragraphs[0].ReplaceText("XXX", attendingDoctor.DoctorName);
                    table.InsertRow(attendingYJRow).Cells[1].Paragraphs[0].Append(attendingDoctor.Opinion).FontSize(12);
                    table.InsertRow(attendingFARow).Cells[1].Paragraphs[0].Append(attendingDoctor.Perscription).FontSize(12);
                    var newRow2 = table.InsertRow(attendingQMRow);
                    //签名图片
                    //byte[] byteData2 = null;
                    //if (!string.IsNullOrEmpty(attendingDoctor.SignatureUrl) &&
                    //    attendingDoctor.SignatureUrl.ToLower().StartsWith("http") &&
                    //    (byteData2 = KMEHosp.Common.Utility.WebAPIHelper.DownloadData(attendingDoctor.SignatureUrl)) != null)
                    //{
                    //    Novacode.Image img = doc.AddImage(new MemoryStream(byteData2));
                    //    Picture pic = img.CreatePicture();
                    //    pic.Width = 65;
                    //    pic.Height = 40;
                    //    newRow2.Cells[1].Paragraphs[0].AppendPicture(pic);
                    //}
                    //else
                    //{
                        newRow2.Cells[1].Paragraphs[0].Append(attendingDoctor.DoctorName);
                    //}
                    if (attendingDoctor.ModifyTime != null)
                    {
                        newRow2.Cells[3].Paragraphs[0].Append(attendingDoctor.ModifyTime.Value.ToString("yyyy-MM-dd"));
                    }

                    //移除模板行
                    for (int j = 0; j < 8; j++)
                    {
                        table.RemoveRow(8);
                    }

                }

                var ms = new MemoryStream();

                doc.SaveAs(ms);
                return ms;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取PDF文件流
        /// 作者：郭明
        /// 日期：2017年8月14日
        /// </summary>
        /// <param name="wordPath"></param>
        /// <param name="pdfPath"></param>
        /// <returns></returns>
        public byte[] GetPDFFileStream(ResponseConsultationInfoDTO info, string tempPath)
        {
            var wordStream = GetWordFileStream(info, tempPath);
            Document doc = new Document(wordStream);
            var ms = new MemoryStream();
            doc.Save(ms, SaveFormat.Pdf);
            return ms.ToArray();
        }


    }

}
