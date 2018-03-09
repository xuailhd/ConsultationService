using HealthCloud.Common.Cache;
using HealthCloud.Common.Extensions;
using HealthCloud.Common.Log;
using HealthCloud.Consultation.Dto.Common;
using HealthCloud.Consultation.Dto.EventBus;
using HealthCloud.Consultation.Dto.Request;
using HealthCloud.Consultation.Dto.Response;
using HealthCloud.Consultation.Enums;
using HealthCloud.Consultation.ICaches.Keys;
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
    public class ConversationRoomRepository : BaseRepository
    {
        /// <summary>
        /// 获取房间用户信息
        /// </summary>
        /// <param name="ConversationRoomID"></param>
        /// <returns></returns>
        public List<ResponseConversationRoomMemberDTO> GetChannelUsersInfo(string ConversationRoomID, DBEntities db = null)
        {
            var cacheKey = new StringCacheKey(StringCacheKeyType.Channel_Member, ConversationRoomID);
            var result = cacheKey.FromCache<List<ResponseConversationRoomMemberDTO>>();
            if (result == null)
            {
                bool dbPrivateFlag = false;
                if (db == null)
                {
                    db = CreateDb();
                    dbPrivateFlag = true;
                }

                result = (
                         from roomMember in db.ConversationRoomUids
                         join room in db.ConversationRooms on roomMember.ConversationRoomID equals room.ConversationRoomID
                         where room.ConversationRoomID == ConversationRoomID
                         select new ResponseConversationRoomMemberDTO
                         {
                             identifier = roomMember.Identifier,
                             UserCNName = roomMember.UserCNName,
                             UserENName = roomMember.UserENName,
                             PhotoUrl = roomMember.PhotoUrl,
                             UserType = roomMember.UserType,
                             UserID = roomMember.UserID,
                         }).ToList();

                if (dbPrivateFlag)
                {
                    db.Dispose();
                }

                result.ToCache(cacheKey, TimeSpan.FromHours(2));
            }
            return result;
        }

        /// <summary>
        /// 获取房间信息
        /// </summary>
        /// <param name="ConversationRoomID"></param>
        /// <returns></returns>
        public ResponseConversationRoomDTO GetChannelInfo(string ConversationRoomID, DBEntities db = null)
        {

            var cacheKey = new StringCacheKey(StringCacheKeyType.Channel, ConversationRoomID);
            ResponseConversationRoomDTO room = cacheKey.FromCache<ResponseConversationRoomDTO>();
            if (room == null)
            {
                bool dbPrivateFlag = false;
                if (db == null)
                {
                    db = CreateDb();
                    dbPrivateFlag = true;
                }

                var model = db.ConversationRooms.Where(a => a.ConversationRoomID == ConversationRoomID).FirstOrDefault();

                if (dbPrivateFlag)
                {
                    db.Dispose();
                }

                if (model != null)
                {
                    room = model.Map<ConversationRoom, ResponseConversationRoomDTO>();
                    room.ToCache(cacheKey, TimeSpan.FromHours(1));
                }
            }
            return room;
        }

        /// <summary>
        /// 根据服务编号获取频道编号
        /// </summary>
        /// <param name="ServiceID"></param>
        /// <returns></returns>
        public string GetChannelIDByServiceID(string ServiceID, DBEntities db = null)
        {
            var cacheKey_ConversationRoomID = new StringCacheKey(StringCacheKeyType.MAP_GetChannelIDByServiceID, ServiceID);
            string ConversationRoomID = cacheKey_ConversationRoomID.FromCache<string>();
            if (string.IsNullOrEmpty(ConversationRoomID))
            {
                bool dbPrivateFlag = false;
                if (db == null)
                {
                    db = CreateDb();
                    dbPrivateFlag = true;
                }
                
                var model = db.ConversationRooms.Where(a => a.ServiceID == ServiceID).Select(a => new { a.ConversationRoomID }).FirstOrDefault();

                if (dbPrivateFlag)
                {
                    db.Dispose();
                }

                if (model != null)
                {
                    ConversationRoomID = model.ConversationRoomID;
                    ConversationRoomID.ToCache(cacheKey_ConversationRoomID, TimeSpan.FromHours(1));
                }
                else
                {
                    return null;
                }
            }

            return ConversationRoomID;

        }

        /// <summary>
        /// 查询用户前面还有多少人在候诊chanelID
        /// </summary>
        /// <param name="DoctorID"></param>
        /// <param name="TriageID"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public List<string> GetWaitingChannels(string DoctorID, long TriageID, DBEntities db = null)
        {
            DateTime today = DateTime.Now;
            int Year = today.Year;
            int Month = today.Month;
            int Day = today.Day;

            bool dbPrivateFlag = false;
            if (db == null)
            {
                db = CreateDb();
                dbPrivateFlag = true;
            }

            var query = from room in db.ConversationRooms.Where(room => !room.IsDeleted
                                       && room.RoomState != EnumRoomState.AlreadyVisit
                                       && (room.TriageID != long.MaxValue && (room.TriageID < TriageID || TriageID == 0))
                                       && (room.ServiceType == EnumDoctorServiceType.AudServiceType || room.ServiceType == EnumDoctorServiceType.VidServiceType)
                                   )
                        join opd in db.UserOPDRegisters.Where(opd =>opd.OPDDate.Year == Year && opd.OPDDate.Month == Month 
                        && opd.OPDDate.Day == Day) on room.ServiceID equals opd.OPDRegisterID
                        where opd.OPDState == EnumOPDState.NoReceive
                        select room.ConversationRoomID;

            var ret = query.ToList();

            if (dbPrivateFlag)
            {
                db.Dispose();
            }
            return ret;
        }

        /// <summary>
        /// 获取会员等级对应的 医生成员
        /// </summary>
        /// <param name="UserLevel"></param>
        /// <returns></returns>
        public List<string> GetDoctorGroupMemberByUserLevel(int UserLevel, DBEntities db = null)
        {
            bool dbPrivateFlag = false;
            if (db == null)
            {
                db = CreateDb();
                dbPrivateFlag = true;
            }
            //查询当前组的医生
            var doctorIdList = (from doctor in db.DoctorGroupMembers
                                join doctorConfig in db.DoctorConfigs.Where(a => a.ConfigType == EnumDoctorConfigType.DiagnoseOff) on doctor.DoctorID equals doctorConfig.DoctorID into leftJoinDoctorConfig
                                from doctorConfigIfEmpty in leftJoinDoctorConfig.DefaultIfEmpty()
                                join doctorGroup in db.DoctorGroups on doctor.DoctorGroupID equals doctorGroup.DoctorGroupID
                                join route in db.DoctorGroupTaskRoute.Where(a => a.UserLevel == UserLevel) on doctorGroup.DoctorGroupID equals route.DoctorGroupID
                                where (doctorConfigIfEmpty == null || doctorConfigIfEmpty.ConfigContent == bool.FalseString)
                                select doctor.DoctorID).ToList();
            if (dbPrivateFlag)
            {
                db.Dispose();
            }

            return doctorIdList;

        }

        /// <summary>
        /// 获取医生分组编号根据医生
        /// </summary>
        /// <param name="DoctorID"></param>
        /// <returns></returns>
        public List<string> GetDoctorGroupIdListByDoctorID(string DoctorID, DBEntities db = null)
        {
            bool dbPrivateFlag = false;
            if (db == null)
            {
                db = CreateDb();
                dbPrivateFlag = true;
            }
            //查询当前组的医生
            var groups = (from doctor in db.DoctorGroupMembers
                          where doctor.DoctorID == DoctorID
                          select doctor.DoctorGroupID).ToList();

            if (dbPrivateFlag)
            {
                db.Dispose();
            }

            return groups;
        }

        /// <summary>
        /// 获取医生状态配置
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public List<ResponseConversationRoomDocConfDTO> GetDoctorConfigs(string DoctorID, DBEntities db = null)
        {
            #region 获取医生配置
            var CacheKey = new StringCacheKey(StringCacheKeyType.Doctor_Configs, DoctorID);
            var doctorConfigs = CacheKey.FromCache<List<ResponseConversationRoomDocConfDTO>>();

            if (doctorConfigs == null)
            {
                bool dbPrivateFlag = false;
                if (db == null)
                {
                    db = CreateDb();
                    dbPrivateFlag = true;
                }

                doctorConfigs = db.DoctorConfigs.Where(x => !x.IsDeleted && x.DoctorID == DoctorID).Select(x => new ResponseConversationRoomDocConfDTO
                {
                    DoctorID = DoctorID,
                    ConfigType = x.ConfigType,
                    ConfigContent = x.ConfigContent
                }).ToList();

                if (dbPrivateFlag)
                {
                    db.Dispose();
                }

                if (doctorConfigs != null)
                    doctorConfigs.ToCache(CacheKey);
            }
            return doctorConfigs;
            #endregion
        }


        /// <summary>
        /// 批量将正在候诊状态的用户修改成离开
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public List<ResponseConversationRoomDTO> GetUserCurrentRooms(string UserID, DBEntities db = null)
        {
            bool dbPrivateFlag = false;
            if (db == null)
            {
                db = CreateDb();
                dbPrivateFlag = true;
            }

            var roomList = (from room in db.ConversationRooms.Where(a =>
                    (a.RoomState != EnumRoomState.Disconnection &&
                    a.RoomState != EnumRoomState.NoTreatment &&
                    a.RoomState != EnumRoomState.AlreadyVisit)
                   && (a.ServiceType == EnumDoctorServiceType.AudServiceType ||
                   a.ServiceType == EnumDoctorServiceType.VidServiceType))
                            join opd in db.UserOPDRegisters.Where(a => a.UserID == UserID)
                            on room.ServiceID equals opd.OPDRegisterID
                            select new ResponseConversationRoomDTO
                            {
                                ConversationRoomID  = room.ConversationRoomID,
                                RoomState = room.RoomState,
                                DisableWebSdkInteroperability = room.DisableWebSdkInteroperability
                            }).ToList();

            if (dbPrivateFlag)
            {
                db.Dispose();
            }

            return roomList;
        }


        /// <summary>
        /// 替换频道成员
        /// </summary>
        /// <param name="ConversationRoomID"></param>
        /// <param name="members"></param>
        /// <returns></returns>
        public bool ReplaceChannelMembers(string ConversationRoomID, List<RequestChannelMemberDTO> members)
        {
            using (DBEntities db = new DBEntities())
            {
                var oldmembers = db.ConversationRoomUids.Where(a => a.ConversationRoomID == ConversationRoomID).ToList();

                foreach (var member in members)
                {
                    var roomUid = new ConversationRoomUid()
                    {
                        ConversationRoomID = ConversationRoomID,
                        UserType = member.UserType,
                        Identifier = member.Identifier,
                        UserMemberID = member.UserMemberID.IfNull(""),
                        UserCNName = member.UserCNName.IfNull(""),
                        UserENName = member.UserENName.IfNull(""),
                        PhotoUrl = member.PhotoUrl.IfNull(""),
                        UserID = member.UserID
                    };
                    db.ConversationRoomUids.Add(roomUid);
                }

                if (db.SaveChanges() > 0)
                {
                    var cacheKey = new StringCacheKey(StringCacheKeyType.Channel_Member, ConversationRoomID);
                    cacheKey.RemoveCache();

                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        /// <summary>
        /// 新增频道成员
        /// </summary>
        /// <param name="ConversationRoomID"></param>
        /// <param name="members"></param>
        /// <returns></returns>
        public bool InsertChannelMembers(string ConversationRoomID, List<RequestChannelMemberDTO> members)
        {
            using (DBEntities db = new DBEntities())
            {
                foreach (var member in members)
                {
                    if (!db.ConversationRoomUids.Where(a => a.ConversationRoomID == ConversationRoomID && a.Identifier == member.Identifier).Any())
                    {
                        var roomUid = new ConversationRoomUid()
                        {
                            ConversationRoomID = ConversationRoomID,
                            UserType = member.UserType,
                            Identifier = member.Identifier,
                            UserMemberID = member.UserMemberID.IfNull(""),
                            UserID = member.UserID,
                            UserCNName = member.UserCNName.IfNull(""),
                            UserENName = member.UserENName.IfNull(""),
                            PhotoUrl = member.PhotoUrl.IfNull(""),
                        };
                        db.ConversationRoomUids.Add(roomUid);
                    }
                }

                if (db.SaveChanges() >= 0)
                {
                    var cacheKey = new StringCacheKey(StringCacheKeyType.Channel_Member, ConversationRoomID);
                    cacheKey.RemoveCache();

                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        /// <summary>
        /// 设置频道状态
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public bool UpdateChannelState(string ConversationRoomID, EnumRoomState roomState)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["KMEHospEntities"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(@"update ConversationRooms set RoomState=@RoomState,ModifyTime=@ModifyTime 
                where ConversationRooms.ConversationRoomID=@ConversationRoomID");

                cmd.Parameters.Add("@ModifyTime", SqlDbType.DateTime).Value = DateTime.Now;
                //cmd.Parameters.Add("@TriageID", SqlDbType.BigInt).Value = room.TriageID;
                cmd.Parameters.Add("@RoomState", SqlDbType.Int).Value = (int)roomState;
                cmd.Parameters.Add("@ConversationRoomID", SqlDbType.Int).Value = ConversationRoomID;

                var ret = cmd.ExecuteNonQuery()>0;
                if (ret)
                {
                    var cacheKey = new StringCacheKey(StringCacheKeyType.Channel, ConversationRoomID);
                    cacheKey.RemoveCache();
                }

                return ret;
            }
        }

        /// <summary>
        /// 修改频道可用
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public bool UpdateRoomEable(string ConversationRoomID, bool enable)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["KMEHospEntities"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(@"update ConversationRooms set Enable=@Enable,ModifyTime=@ModifyTime 
                where ConversationRooms.ConversationRoomID=@ConversationRoomID");

                cmd.Parameters.Add("@ModifyTime", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@Enable", SqlDbType.Bit).Value = enable;
                cmd.Parameters.Add("@ConversationRoomID", SqlDbType.Int).Value = ConversationRoomID;

                var ret = cmd.ExecuteNonQuery() > 0;
                if (ret)
                {
                    var cacheKey = new StringCacheKey(StringCacheKeyType.Channel, ConversationRoomID);
                    cacheKey.RemoveCache();
                }

                return ret;
            }
        }

        /// <summary>
        /// 关闭频道
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public bool CloseRoom(string ConversationRoomID, bool close)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["KMEHospEntities"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(@"update ConversationRooms set Close=@Close,ModifyTime=@ModifyTime 
                where ConversationRooms.ConversationRoomID=@ConversationRoomID");

                cmd.Parameters.Add("@ModifyTime", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@Close", SqlDbType.Bit).Value = close;
                cmd.Parameters.Add("@ConversationRoomID", SqlDbType.Int).Value = ConversationRoomID;

                var ret = cmd.ExecuteNonQuery() > 0;
                if (ret)
                {
                    var cacheKey = new StringCacheKey(StringCacheKeyType.Channel, ConversationRoomID);
                    cacheKey.RemoveCache();
                }

                return ret;
            }
        }

        /// <summary>
        /// 设置频道计费状态
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public bool UpdateChannelChargeState(string ConversationRoomID, EnumRoomChargingState chargingState)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["KMEHospEntities"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(@"update ConversationRooms set ChargingState=@ChargingState,ModifyTime=@ModifyTime 
                    where ConversationRooms.ConversationRoomID=@ConversationRoomID");
                cmd.Parameters.Add("@ModifyTime", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@ChargingState", SqlDbType.Int).Value = (int)chargingState;
                cmd.Parameters.Add("@ConversationRoomID", SqlDbType.Int).Value = ConversationRoomID;

                var ret = cmd.ExecuteNonQuery() > 0;
                if (ret)
                {
                    var cacheKey = new StringCacheKey(StringCacheKeyType.Channel, ConversationRoomID.ToString());
                    cacheKey.RemoveCache();
                }

                return ret;
            }
        }

        /// <summary>
        /// 修改分诊编号
        /// </summary>
        /// <param name="ConversationRoomID"></param>
        /// <param name="triageID"></param>
        /// <returns></returns>
        public bool UpdateTriageID(string ConversationRoomID, long triageID)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["KMEHospEntities"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(@"update ConversationRooms set TriageID=@TriageID,ModifyTime=@ModifyTime 
                    where ConversationRooms.ConversationRoomID=@ConversationRoomID");
                cmd.Parameters.Add("@ModifyTime", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@TriageID", SqlDbType.BigInt).Value = triageID;
                cmd.Parameters.Add("@ConversationRoomID", SqlDbType.Int).Value = ConversationRoomID;

                var ret = cmd.ExecuteNonQuery() > 0;
                if (ret)
                {
                    var cacheKey = new StringCacheKey(StringCacheKeyType.Channel, ConversationRoomID);
                    cacheKey.RemoveCache();
                }

                return ret;
            }
        }

        public bool UpdateChannelChargeSeq(string conversationRoomID, int chargingSeq,DateTime chargingTime,int chargingInterval
            , EnumRoomState? roomState, DateTime? endTime)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["KMEHospEntities"].ConnectionString))
            {
                var sql = "" ;
                if (roomState.HasValue)
                {
                    sql = @"update ConversationRooms set ChargingTime=@ChargingTime,ChargingSeq=@ChargingSeq,
                    ChargingInterval=@ChargingInterval,RoomState=@RoomState,EndTime=@EndTime,ModifyTime=@ModifyTime 
                    where ConversationRooms.ConversationRoomID=@ConversationRoomID";
                }
                else
                {
                    sql = @"update ConversationRooms set ChargingTime=@ChargingTime,ChargingSeq=@ChargingSeq,
                    ChargingInterval=@ChargingInterval,ModifyTime=@ModifyTime 
                    where ConversationRooms.ConversationRoomID=@ConversationRoomID";
                }

                SqlCommand cmd = new SqlCommand(@"update ConversationRooms set ChargingTime=@ChargingTime,ChargingSeq=@ChargingSeq,
                    ChargingInterval=@ChargingInterval,RoomState=@RoomState,ModifyTime=@ModifyTime 
                    where ConversationRooms.ConversationRoomID=@ConversationRoomID");

                if (roomState.HasValue)
                {
                    cmd.Parameters.Add("@RoomState", SqlDbType.Int).Value = (int)roomState.Value;
                    cmd.Parameters.Add("@EndTime", SqlDbType.DateTime).Value = endTime.Value;
                }

                cmd.Parameters.Add("@ChargingTime", SqlDbType.DateTime).Value = chargingTime;
                cmd.Parameters.Add("@ChargingSeq", SqlDbType.Int).Value = chargingSeq;
                cmd.Parameters.Add("@ChargingInterval", SqlDbType.Int).Value = chargingInterval;
                cmd.Parameters.Add("@ModifyTime", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@ConversationRoomID", SqlDbType.Int).Value = conversationRoomID;

                var ret = cmd.ExecuteNonQuery() > 0;
                if (ret)
                {
                    var cacheKey = new StringCacheKey(StringCacheKeyType.Channel, conversationRoomID);
                    cacheKey.RemoveCache();
                }

                return ret;
            }
        }

        /// <summary>
        /// 设置频道计费时长
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        public bool UpdateChannelChargeTime(string ConversationRoomID,int TotalTime,DateTime BeginTime)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["KMEHospEntities"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(@"update ConversationRooms set BeginTime=@BeginTime,ModifyTime=@ModifyTime,
                TotalTime=@TotalTime
                where ConversationRooms.ConversationRoomID=@ConversationRoomID");

                cmd.Parameters.Add("@ModifyTime", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@TotalTime", SqlDbType.Int).Value = TotalTime;
                cmd.Parameters.Add("@BeginTime", SqlDbType.DateTime).Value = BeginTime;
                cmd.Parameters.Add("@ConversationRoomID", SqlDbType.VarChar).Value = ConversationRoomID;

                var ret = cmd.ExecuteNonQuery() > 0;
                if (ret)
                {
                    var cacheKey = new StringCacheKey(StringCacheKeyType.Channel, ConversationRoomID);
                    cacheKey.RemoveCache();
                }

                return ret;
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

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["KMEHospEntities"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(@"if(select top 1 1 from ConversationRoomUpgrades where ServiceID=@ServiceID and OrderNo=@OrderNo 
                    and NewUpgradeOrderNo=@NewUpgradeOrderNo) 
                    begin
                        update ConversationRooms set Duration=case when Duration<0 then 1 else Duration end + @Seconds
                        where ServiceID = @ServiceID
                        insert into ConversationRoomUpgrades(ServiceID,Duration,CreateTime,IsDeleted,OrderNo,NewUpgradeOrderNo)
                        values(@ServiceID,@Seconds,@CreateTime,0,@OrderNo,@NewUpgradeOrderNo)
                    end");

                cmd.Parameters.Add("@ServiceID", SqlDbType.VarChar).Value = ServiceID;
                cmd.Parameters.Add("@OrderNo", SqlDbType.VarChar).Value = OrderNo;
                cmd.Parameters.Add("@NewUpgradeOrderNo", SqlDbType.VarChar).Value = NewUpgradeOrderNo;
                cmd.Parameters.Add("@Seconds", SqlDbType.Int).Value = Seconds;
                cmd.Parameters.Add("@CreateTime", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@ModifyTime", SqlDbType.DateTime).Value = DateTime.Now;

                var ret = cmd.ExecuteNonQuery() > 0;
                if (ret)
                {
                    var cacheKey = new StringCacheKey(StringCacheKeyType.Channel, ConversationRoomID);
                    cacheKey.RemoveCache();
                }
                return ret;
            }
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
            string OperationRemark = "", DBEntities db = null)
        {
            bool dbPrivateFlag = false;
            if (db == null)
            {
                db = CreateDb();
                dbPrivateFlag = true;
            }

            db.ConversationRoomLogs.Add(new ConversationRoomLog()
            {
                ConversationRoomID = ConversationRoomID,
                ConversationRoomLogID = Guid.NewGuid().ToString("N"),
                OperationDesc = string.IsNullOrEmpty(OperationDesc) ? "" : OperationDesc,
                OperationRemark = string.IsNullOrEmpty(OperationRemark) ? "" : OperationRemark,
                OperationTime = DateTime.Now,
                OperatorType = OperatorType,
                OperationUserID = string.IsNullOrEmpty(OperationUserID) ? "" : OperationUserID,
                OperatorUserName = string.IsNullOrEmpty(OperatorUserName) ? "" : OperatorUserName
            });

            var ret = true;

            if (dbPrivateFlag)
            {
                ret = db.SaveChanges() > 0;
                db.Dispose();
            }

            return ret;
        }


        /// <summary>
        /// 获取最后的日志
        /// </summary>
        /// <param name="ConversationRoomID"></param>
        /// <param name="OperatorType"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public ResponseConversationRoomLogDTO GetChannelLastLog(string ConversationRoomID, string OperatorType, DBEntities db = null)
        {
            bool dbPrivateFlag = false;
            if (db == null)
            {
                db = CreateDb();
                dbPrivateFlag = true;
            }
            var log = db.ConversationRoomLogs.Where(a => a.ConversationRoomID == ConversationRoomID && a.OperatorType == OperatorType).OrderByDescending(a => a.OperationTime).FirstOrDefault();

            if (dbPrivateFlag)
            {
                db.Dispose();
            }

            return log.Map<ConversationRoomLog, ResponseConversationRoomLogDTO>();
        }
    }
}
