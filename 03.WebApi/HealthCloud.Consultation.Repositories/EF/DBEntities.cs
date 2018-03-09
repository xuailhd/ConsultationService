using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Configuration;
using System.Data;
using EntityFramework.Extensions;
using HealthCloud.Consultation.Models;

namespace HealthCloud.Consultation.Repositories.EF
{
    public class DBEntities : DbContext
    {
        public DBEntities()
          : base("name=KMEHospEntities")
        {
            //Database.SetInitializer<DBEntities>(new MigrateDatabaseToLatestVersion<DBEntities, Configuration>());

            //关闭了实体验证
            this.Configuration.ValidateOnSaveEnabled = false;
            //关闭延迟加载
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;

            Database.SetInitializer<DBEntities>(null);


        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<SysConfig> SysConfigs { get; set; }

        /// <summary>
        /// 线下挂号记录表
        /// </summary>
        public DbSet<UserRegistration> UserRegistrations { get; set; }

        /// <summary>
        /// 预约记录表
        /// </summary>
        public DbSet<UserOPDRegister> UserOPDRegisters { get; set; }

        /// <summary>
        /// 看诊房间时长续费日志
        /// </summary>
        public DbSet<ConversationRoomUpgrade> ConversationRoomUpgrades { get; set; }

        /// <summary>
        /// 看诊房间操作日志
        /// </summary>
        public DbSet<ConversationRoomLog> ConversationRoomLogs { get; set; }

        /// <summary>
        /// 好友
        /// </summary>
        public DbSet<ConversationFriend> ConversationFriends { get; set; }

        /// <summary>
        /// 看诊房间表
        /// </summary>
        public DbSet<ConversationRoom> ConversationRooms { get; set; }

        /// <summary>
        /// 会话内容
        /// </summary>
        public DbSet<ConversationMessage> ConversationMessages { get; set; }

        /// <summary>
        /// 实时通讯用户唯一标识
        /// </summary>
        public DbSet<ConversationIMUid> ConversationIMUids { get; set; }

        /// <summary>
        /// 会话房间 成员表
        /// </summary>
        public DbSet<ConversationRoomUid> ConversationRoomUids { get; set; }

        /// <summary>
        /// 医生分组
        /// </summary>
        public DbSet<DoctorGroup> DoctorGroups { get; set; }

        /// <summary>
        /// 分组成员
        /// </summary>
        public DbSet<DoctorGroupMember> DoctorGroupMembers { get; set; }


        /// <summary>
        /// 医生分组领单路由
        /// </summary>
        public DbSet<DoctorGroupTaskRoute> DoctorGroupTaskRoute { get; set; }

        /// <summary>
        /// 分诊
        /// </summary>
        public DbSet<DoctorTriage> DoctorTriages { get; set; }

        /// <summary>
        /// 监控用户看诊指标
        /// </summary>
        public DbSet<SysMonitorIndex> SysMonitorIndexs { get; set; }

        /// <summary>
        /// 医生配置信息表
        /// </summary>
        public DbSet<DoctorConfig> DoctorConfigs { get; set; }

        /// <summary>
        /// 远程会诊
        /// </summary>
        public DbSet<RemoteConsultation> RemoteConsultations { get; set; }

        /// <summary>
        /// 会诊日志
        /// </summary>
        public DbSet<ConsultationLog> ConsultationLogs { get; set; }

        /// <summary>
        ///会诊医生
        /// </summary>
        public DbSet<ConsultationDoctor> ConsultationDoctors { get; set; }

        /// <summary>
        /// 用户文件
        /// </summary>
        public DbSet<UserFile> UserFiles { get; set; }

        /// <summary>
        /// 用户检查结果
        /// </summary>
        public DbSet<UserInspectResult> UserInspectResults { get; set; }

        /// <summary>
        /// 病历
        /// </summary>
        public DbSet<UserMedicalRecord> UserMedicalRecords { get; set; }

        /// <summary>
        /// 系统去重
        /// </summary>
        public DbSet<SysDereplication> SysDereplications { get; set; }

        /// <summary>
        /// 消息附加参数配制
        /// </summary>
        public DbSet<SysMessageExtrasConfig> MessageExtrasConfigs { get; set; }
    }
}
