if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ConsultationDoctors]') and xtype='U')
CREATE TABLE [dbo].[ConsultationDoctors] (
    [ConsultationDoctorID] [varchar](32) NOT NULL,
    [ConsultationID] [varchar](32),
    [DoctorID] [varchar](32),
    [DoctorName] [varchar](32),
    [PhotoUrl] [nvarchar](255),
    [DepartmentName] [nvarchar](64),
    [HospitalName] [nvarchar](64),
    [Title] [nvarchar](20),
    [Amount] [decimal](18, 2) NOT NULL,
    [Opinion] [nvarchar](4000),
    [Perscription] [nvarchar](4000),
    [IsAttending] [bit] NOT NULL,
    [CreateUserID] [varchar](8000),
    [CreateTime] [datetimeoffset](7) NOT NULL,
    [ModifyUserID] [nvarchar](4000),
    [ModifyTime] [datetimeoffset](7),
    [DeleteUserID] [nvarchar](4000),
    [DeleteTime] [datetimeoffset](7),
    [IsDeleted] [bit] NOT NULL,
    CONSTRAINT [PK_dbo.ConsultationDoctors] PRIMARY KEY ([ConsultationDoctorID])
)
go


if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ConsultationLogs]') and xtype='U')
CREATE TABLE [dbo].[ConsultationLogs] (
    [ConsultationLogID] [varchar](32) NOT NULL,
    [ConsultationID] [varchar](32),
    [OperationType] [int] NOT NULL,
    [ConsultationProgress] [int] NOT NULL,
    [OPDState] [int] NOT NULL,
    [Remark] [nvarchar](4000),
    [OrgID] [varchar](32),
    [OperationUserName] [nvarchar](64),
    [CreateUserID] [varchar](8000),
    [CreateTime] [datetimeoffset](7) NOT NULL,
    [ModifyUserID] [nvarchar](4000),
    [ModifyTime] [datetimeoffset](7),
    [DeleteUserID] [nvarchar](4000),
    [DeleteTime] [datetimeoffset](7),
    [IsDeleted] [bit] NOT NULL,
    CONSTRAINT [PK_dbo.ConsultationLogs] PRIMARY KEY ([ConsultationLogID])
)
go


if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ConversationFriends]') and xtype='U')
CREATE TABLE [dbo].[ConversationFriends] (
    [FriendID] [nvarchar](32) NOT NULL,
    [ConversationRoomID] [nvarchar](32) NOT NULL,
    [FromUserIdentifier] [int] NOT NULL,
    [ToUserIdentifier] [int] NOT NULL,
    [GroupName] [nvarchar](32) NOT NULL,
    [Remark] [nvarchar](4000),
    [AddWording] [nvarchar](max),
    [ToUserID] [nvarchar](max) NOT NULL,
    [FromUserID] [nvarchar](max) NOT NULL,
    [CreateUserID] [varchar](8000),
    [CreateTime] [datetimeoffset](7) NOT NULL,
    [ModifyUserID] [nvarchar](4000),
    [ModifyTime] [datetimeoffset](7),
    [DeleteUserID] [nvarchar](4000),
    [DeleteTime] [datetimeoffset](7),
    [IsDeleted] [bit] NOT NULL,
    CONSTRAINT [PK_dbo.ConversationFriends] PRIMARY KEY ([FriendID])
)
go


if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ConversationIMUids]') and xtype='U')
CREATE TABLE [dbo].[ConversationIMUids] (
    [Identifier] [int] NOT NULL IDENTITY,
    [UserID] [nvarchar](4000) NOT NULL,
    [Enable] [bit] NOT NULL,
    [CreateUserID] [varchar](8000),
    [CreateTime] [datetimeoffset](7) NOT NULL,
    [ModifyUserID] [nvarchar](4000),
    [ModifyTime] [datetimeoffset](7),
    [DeleteUserID] [nvarchar](4000),
    [DeleteTime] [datetimeoffset](7),
    [IsDeleted] [bit] NOT NULL,
    CONSTRAINT [PK_dbo.ConversationIMUids] PRIMARY KEY ([Identifier])
)
go


if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ConversationMessages]') and xtype='U')
CREATE TABLE [dbo].[ConversationMessages] (
    [ConversationMessageID] [nvarchar](50) NOT NULL,
    [ConversationRoomID] [nvarchar](50) NOT NULL,
    [ServiceID] [nvarchar](4000) NOT NULL,
    [UserID] [nvarchar](4000) NOT NULL,
    [MessageType] [nvarchar](50) NOT NULL,
    [MessageContent] [text] NOT NULL,
    [MessageTime] [datetimeoffset](7) NOT NULL,
    [MessageState] [int] NOT NULL,
    [MessageSeq] [nvarchar](4000) NOT NULL,
    [MessageIndex] [int] NOT NULL,
    [CreateUserID] [varchar](8000),
    [CreateTime] [datetimeoffset](7) NOT NULL,
    [ModifyUserID] [nvarchar](4000),
    [ModifyTime] [datetimeoffset](7),
    [DeleteUserID] [nvarchar](4000),
    [DeleteTime] [datetimeoffset](7),
    [IsDeleted] [bit] NOT NULL,
    CONSTRAINT [PK_dbo.ConversationMessages] PRIMARY KEY ([ConversationMessageID])
)
go


if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ConversationRoomLogs]') and xtype='U')
CREATE TABLE [dbo].[ConversationRoomLogs] (
    [ConversationRoomLogID] [nvarchar](128) NOT NULL,
    [ConversationRoomID] [nvarchar](4000) NOT NULL,
    [OperationUserID] [nvarchar](4000) NOT NULL,
    [OperatorUserName] [nvarchar](4000) NOT NULL,
    [OperatorType] [nvarchar](4000) NOT NULL,
    [OperationTime] [datetimeoffset](7) NOT NULL,
    [OperationDesc] [nvarchar](200) NOT NULL,
    [OperationRemark] [nvarchar](200) NOT NULL,
    CONSTRAINT [PK_dbo.ConversationRoomLogs] PRIMARY KEY ([ConversationRoomLogID])
)
go


if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ConversationRooms]') and xtype='U')
CREATE TABLE [dbo].[ConversationRooms] (
    [ConversationRoomID] [nvarchar](128) NOT NULL,
    [ServiceID] [nvarchar](4000) NOT NULL,
    [ServiceType] [int] NOT NULL,
    [Secret] [nvarchar](4000) NOT NULL,
    [RoomState] [int] NOT NULL,
    [BeginTime] [datetimeoffset](7) NOT NULL,
    [EndTime] [datetimeoffset](7),
    [TotalTime] [int] NOT NULL,
    [Duration] [int] NOT NULL,
    [Enable] [bit] NOT NULL,
    [DisableWebSdkInteroperability] [bit] NOT NULL,
    [Close] [bit] NOT NULL,
    [RoomType] [int] NOT NULL,
    [TriageID] [bigint] NOT NULL,
    [Priority] [int] NOT NULL,
    [ChargingState] [int] NOT NULL,
    [ChargingSeq] [int] NOT NULL,
    [ChargingTime] [datetimeoffset](7) NOT NULL,
    [ChargingInterval] [int] NOT NULL,
    [CreateUserID] [varchar](8000),
    [CreateTime] [datetimeoffset](7) NOT NULL,
    [ModifyUserID] [nvarchar](4000),
    [ModifyTime] [datetimeoffset](7),
    [DeleteUserID] [nvarchar](4000),
    [DeleteTime] [datetimeoffset](7),
    [IsDeleted] [bit] NOT NULL,
    CONSTRAINT [PK_dbo.ConversationRooms] PRIMARY KEY ([ConversationRoomID])
)
go

if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ConversationRoomUids]') and xtype='U')
CREATE TABLE [dbo].[ConversationRoomUids] (
    [ConversationRoomID] [nvarchar](128) NOT NULL,
    [Identifier] [int] NOT NULL,
    [UserType] [int] NOT NULL,
    [UserMemberID] [varchar](32) NOT NULL,
    [UserID] [varchar](32) NOT NULL,
    [UserCNName] [varchar](64) NOT NULL,
    [UserENName] [varchar](64) NOT NULL,
    [Gender] [int] NOT NULL,
    [PhotoUrl] [varchar](200) NOT NULL,
    [CreateUserID] [varchar](8000),
    [CreateTime] [datetimeoffset](7) NOT NULL,
    [ModifyUserID] [nvarchar](4000),
    [ModifyTime] [datetimeoffset](7),
    [DeleteUserID] [nvarchar](4000),
    [DeleteTime] [datetimeoffset](7),
    [IsDeleted] [bit] NOT NULL,
    CONSTRAINT [PK_dbo.ConversationRoomUids] PRIMARY KEY ([ConversationRoomID], [Identifier])
)
go

if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ConversationRoomUpgrades]') and xtype='U')
CREATE TABLE [dbo].[ConversationRoomUpgrades] (
    [ServiceID] [nvarchar](128) NOT NULL,
    [OrderNo] [nvarchar](128) NOT NULL,
    [NewUpgradeOrderNo] [nvarchar](128) NOT NULL,
    [Duration] [int] NOT NULL,
    [IsSendDurationMsg] [bit] NOT NULL,
    [IsSendExpireMsg] [bit] NOT NULL,
    [CreateUserID] [varchar](8000),
    [CreateTime] [datetimeoffset](7) NOT NULL,
    [ModifyUserID] [nvarchar](4000),
    [ModifyTime] [datetimeoffset](7),
    [DeleteUserID] [nvarchar](4000),
    [DeleteTime] [datetimeoffset](7),
    [IsDeleted] [bit] NOT NULL,
    CONSTRAINT [PK_dbo.ConversationRoomUpgrades] PRIMARY KEY ([ServiceID], [OrderNo], [NewUpgradeOrderNo])
)
go

if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DoctorConfigs]') and xtype='U')
CREATE TABLE [dbo].[DoctorConfigs] (
    [ConfigID] [varchar](32) NOT NULL,
    [DoctorID] [varchar](32) NOT NULL,
    [ConfigType] [int] NOT NULL,
    [ConfigContent] [nvarchar](200) NOT NULL,
    [CreateUserID] [varchar](8000),
    [CreateTime] [datetimeoffset](7) NOT NULL,
    [ModifyUserID] [nvarchar](4000),
    [ModifyTime] [datetimeoffset](7),
    [DeleteUserID] [nvarchar](4000),
    [DeleteTime] [datetimeoffset](7),
    [IsDeleted] [bit] NOT NULL,
    CONSTRAINT [PK_dbo.DoctorConfigs] PRIMARY KEY ([ConfigID])
)
go

if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DoctorGroupMembers]') and xtype='U')
CREATE TABLE [dbo].[DoctorGroupMembers] (
    [GroupMemberID] [varchar](32) NOT NULL,
    [DoctorGroupID] [varchar](32) NOT NULL,
    [DoctorID] [varchar](32) NOT NULL,
    [CreateUserID] [varchar](8000),
    [CreateTime] [datetimeoffset](7) NOT NULL,
    [ModifyUserID] [nvarchar](4000),
    [ModifyTime] [datetimeoffset](7),
    [DeleteUserID] [nvarchar](4000),
    [DeleteTime] [datetimeoffset](7),
    [IsDeleted] [bit] NOT NULL,
    CONSTRAINT [PK_dbo.DoctorGroupMembers] PRIMARY KEY ([GroupMemberID])
)
go

if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DoctorGroups]') and xtype='U')
CREATE TABLE [dbo].[DoctorGroups] (
    [DoctorGroupID] [varchar](32) NOT NULL,
    [GroupName] [varchar](100) NOT NULL,
    [CreateUserID] [varchar](8000),
    [CreateTime] [datetimeoffset](7) NOT NULL,
    [ModifyUserID] [nvarchar](4000),
    [ModifyTime] [datetimeoffset](7),
    [DeleteUserID] [nvarchar](4000),
    [DeleteTime] [datetimeoffset](7),
    [IsDeleted] [bit] NOT NULL,
    CONSTRAINT [PK_dbo.DoctorGroups] PRIMARY KEY ([DoctorGroupID])
)
go


if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DoctorGroupTaskRoutes]') and xtype='U')
CREATE TABLE [dbo].[DoctorGroupTaskRoutes] (
    [DoctorGroupID] [varchar](32) NOT NULL,
    [UserLevel] [int] NOT NULL,
    CONSTRAINT [PK_dbo.DoctorGroupTaskRoutes] PRIMARY KEY ([DoctorGroupID], [UserLevel])
)
go

if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DoctorTriages]') and xtype='U')
CREATE TABLE [dbo].[DoctorTriages] (
    [OPDRegisterID] [varchar](32) NOT NULL,
    [TriageDoctorID] [varchar](32),
    [TriageDoctorName] [nvarchar](64),
    [TriageStatus] [int] NOT NULL,
    [IsToGuidance] [bit] NOT NULL,
    [CreateUserID] [varchar](8000),
    [CreateTime] [datetimeoffset](7) NOT NULL,
    [ModifyUserID] [nvarchar](4000),
    [ModifyTime] [datetimeoffset](7),
    [DeleteUserID] [nvarchar](4000),
    [DeleteTime] [datetimeoffset](7),
    [IsDeleted] [bit] NOT NULL,
    CONSTRAINT [PK_dbo.DoctorTriages] PRIMARY KEY ([OPDRegisterID])
)
go

if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SysMessageExtrasConfigs]') and xtype='U')
CREATE TABLE [dbo].[SysMessageExtrasConfigs] (
    [ExtrasConfigID] [varchar](32) NOT NULL,
    [TerminalType] [int] NOT NULL,
    [MsgType] [int] NOT NULL,
    [MsgTitle] [nvarchar](4000) NOT NULL,
    [PageUrl] [varchar](512) NOT NULL,
    [PageType] [varchar](20) NOT NULL,
    [PageTarget] [varchar](20),
    [CreateUserID] [varchar](8000),
    [CreateTime] [datetimeoffset](7) NOT NULL,
    [ModifyUserID] [nvarchar](4000),
    [ModifyTime] [datetimeoffset](7),
    [DeleteUserID] [nvarchar](4000),
    [DeleteTime] [datetimeoffset](7),
    [IsDeleted] [bit] NOT NULL,
    CONSTRAINT [PK_dbo.SysMessageExtrasConfigs] PRIMARY KEY ([ExtrasConfigID])
)
go

if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[RemoteConsultations]') and xtype='U')
CREATE TABLE [dbo].[RemoteConsultations] (
    [ConsultationID] [varchar](32) NOT NULL,
    [MemberID] [varchar](32) NOT NULL,
    [Requirement] [nvarchar](4000),
    [Purpose] [nvarchar](4000),
    [StartTime] [datetimeoffset](7),
    [FinishTime] [datetimeoffset](7),
    [StartTimeReal] [datetimeoffset](7),
    [FinishTimeReal] [datetimeoffset](7),
    [ConsultationNo] [varchar](32),
    [Deposit] [decimal](18, 2) NOT NULL,
    [Amount] [decimal](18, 2) NOT NULL,
    [ConsultationProgress] [int] NOT NULL,
    [ConsultationSource] [int] NOT NULL,
    [OPDRegisterID] [varchar](32),
    [Address] [nvarchar](100),
    [CreateUserID] [varchar](8000),
    [CreateTime] [datetimeoffset](7) NOT NULL,
    [ModifyUserID] [nvarchar](4000),
    [ModifyTime] [datetimeoffset](7),
    [DeleteUserID] [nvarchar](4000),
    [DeleteTime] [datetimeoffset](7),
    [IsDeleted] [bit] NOT NULL,
    CONSTRAINT [PK_dbo.RemoteConsultations] PRIMARY KEY ([ConsultationID])
)
go

if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SysConfigs]') and xtype='U')
CREATE TABLE [dbo].[SysConfigs] (
    [ConfigKey] [varchar](128) NOT NULL,
    [ConfigValue] [varchar](512) NOT NULL,
    [Remark] [nvarchar](512),
    CONSTRAINT [PK_dbo.SysConfigs] PRIMARY KEY ([ConfigKey])
)
go

if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SysDereplications]') and xtype='U')
CREATE TABLE [dbo].[SysDereplications] (
    [SysDereplicationID] [varchar](32) NOT NULL,
    [TableName] [varchar](64),
    [OutID] [varchar](32),
    [DereplicationType] [int] NOT NULL,
    [SuccessCount] [int] NOT NULL,
    [FailCount] [int] NOT NULL,
    [CreateUserID] [varchar](8000),
    [CreateTime] [datetimeoffset](7) NOT NULL,
    [ModifyUserID] [nvarchar](4000),
    [ModifyTime] [datetimeoffset](7),
    [DeleteUserID] [nvarchar](4000),
    [DeleteTime] [datetimeoffset](7),
    [IsDeleted] [bit] NOT NULL,
    CONSTRAINT [PK_dbo.SysDereplications] PRIMARY KEY ([SysDereplicationID])
)
go

if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[SysMonitorIndexes]') and xtype='U')
CREATE TABLE [dbo].[SysMonitorIndexes] (
    [Category] [varchar](50) NOT NULL,
    [Name] [varchar](50) NOT NULL,
    [OutID] [varchar](32) NOT NULL,
    [Value] [varchar](1000) NOT NULL,
    [ModifyTime] [datetimeoffset](7) NOT NULL,
    CONSTRAINT [PK_dbo.SysMonitorIndexes] PRIMARY KEY ([Category], [Name], [OutID])
)
go

if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[UserFiles]') and xtype='U')
CREATE TABLE [dbo].[UserFiles] (
    [FileID] [varchar](32) NOT NULL,
    [OutID] [varchar](32) NOT NULL,
    [FileName] [varchar](128) NOT NULL,
    [FileUrl] [varchar](128) NOT NULL,
    [FileType] [int] NOT NULL,
    [Remark] [nvarchar](512) NOT NULL,
    [UserID] [varchar](32) NOT NULL,
    [AccessKey] [varchar](50) NOT NULL,
    [ResourceID] [varchar](50) NOT NULL,
    [CreateUserID] [varchar](8000),
    [CreateTime] [datetimeoffset](7) NOT NULL,
    [ModifyUserID] [nvarchar](4000),
    [ModifyTime] [datetimeoffset](7),
    [DeleteUserID] [nvarchar](4000),
    [DeleteTime] [datetimeoffset](7),
    [IsDeleted] [bit] NOT NULL,
    CONSTRAINT [PK_dbo.UserFiles] PRIMARY KEY ([FileID])
)
go

if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[UserInspectResults]') and xtype='U')
CREATE TABLE [dbo].[UserInspectResults] (
    [InspectResultID] [varchar](32) NOT NULL,
    [MemberID] [varchar](32) NOT NULL,
    [InspectType] [nvarchar](128) NOT NULL,
    [InspectPoint] [nvarchar](64) NOT NULL,
    [InspectDate] [nvarchar](16) NOT NULL,
    [DoctorSuggest] [nvarchar](1024) NOT NULL,
    [FileUploadName] [nvarchar](256) NOT NULL,
    [FileName] [nvarchar](256) NOT NULL,
    [CaseID] [nvarchar](64) NOT NULL,
    [StudyID] [nvarchar](256),
    [StuUID] [nvarchar](256),
    [ImgMD5] [nvarchar](50),
    [CreateUserID] [varchar](8000),
    [CreateTime] [datetimeoffset](7) NOT NULL,
    [ModifyUserID] [nvarchar](4000),
    [ModifyTime] [datetimeoffset](7),
    [DeleteUserID] [nvarchar](4000),
    [DeleteTime] [datetimeoffset](7),
    [IsDeleted] [bit] NOT NULL,
    CONSTRAINT [PK_dbo.UserInspectResults] PRIMARY KEY ([InspectResultID])
)
go

if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[UserOPDRegisters]') and xtype='U')
CREATE TABLE [dbo].[UserOPDRegisters] (
    [OPDRegisterID] [nvarchar](128) NOT NULL,
    [UserID] [nvarchar](4000) NOT NULL,
    [UserAccount] [nvarchar](4000) NOT NULL,
    [DoctorID] [nvarchar](4000) NOT NULL,
    [DoctorName] [nvarchar](4000) NOT NULL,
    [DoctorPhotoUrl] [nvarchar](255),
    [DoctorGroupID] [nvarchar](4000) NOT NULL,
    [ScheduleID] [nvarchar](4000),
    [PayTime] [datetimeoffset](7) NOT NULL,
    [RegDate] [datetimeoffset](7) NOT NULL,
    [OPDDate] [datetimeoffset] (7)NOT NULL,
    [OPDBeginTime] [nvarchar](30) NOT NULL,
    [OPDEndTime] [nvarchar](30) NOT NULL,
    [ConsultContent] [nvarchar](400) NOT NULL,
    [ConsultDisease] [nvarchar](128),
    [OPDType] [int] NOT NULL,
    [OrderNo] [nvarchar](50) NOT NULL,
    [Fee] [decimal](18, 2) NOT NULL,
    [RenewFee] [decimal](18, 2) NOT NULL,
    [MemberID] [nvarchar](50) NOT NULL,
    [MemberName] [nvarchar](4000) NOT NULL,
    [Gender] [int] NOT NULL,
    [Marriage] [int] NOT NULL,
    [Age] [int] NOT NULL,
    [IDNumber] [nvarchar](4000) NOT NULL,
    [IDType] [int] NOT NULL,
    [Mobile] [nvarchar](20),
    [Address] [nvarchar](256),
    [PhotoUrl] [nvarchar](255),
    [Birthday] [nvarchar](10) NOT NULL,
    [MedicalCardNumber] [nvarchar](4000),
    [OrgnazitionID] [nvarchar](4000) NOT NULL,
    [IsUseTaskPool] [bit] NOT NULL,
    [Flag] [int] NOT NULL,
    [AcceptTime] [datetimeoffset](7),
    [AnswerTime] [datetimeoffset](7),
    [InquiryType] [int] NOT NULL,
    [FinishTime] [datetimeoffset](7),
    [OPDState] [int] NOT NULL,
    [CostType] [int] NOT NULL,
    [CreateUserID] [varchar](8000),
    [CreateTime] [datetimeoffset](7) NOT NULL,
    [ModifyUserID] [nvarchar](4000),
    [ModifyTime] [datetimeoffset](7),
    [DeleteUserID] [nvarchar](4000),
    [DeleteTime] [datetimeoffset](7),
    [IsDeleted] [bit] NOT NULL,
    CONSTRAINT [PK_dbo.UserOPDRegisters] PRIMARY KEY ([OPDRegisterID])
)
go

if not exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[UserRegistrations]') and xtype='U')
CREATE TABLE [dbo].[UserRegistrations] (
    [RegistrationID] [nvarchar](128) NOT NULL,
    [OutID] [nvarchar](4000) NOT NULL,
    [HospitalID] [nvarchar](4000) NOT NULL,
    [DepartmentID] [nvarchar](4000) NOT NULL,
    [DoctorID] [nvarchar](4000) NOT NULL,
    [PatientID] [nvarchar](4000) NOT NULL,
    [Name] [nvarchar](50) NOT NULL,
    [Sex] [int] NOT NULL,
    [Mobile] [nvarchar](4000),
    [Email] [nvarchar](200),
    [Birth] [nvarchar](10),
    [IDType] [int] NOT NULL,
    [PatientType] [nvarchar](50),
    [RegType] [int] NOT NULL,
    [MedicalType] [int] NOT NULL,
    [IDNumber] [nvarchar](4000),
    [TriageID] [nvarchar](4000) NOT NULL,
    [MedicalCardID] [nvarchar](4000) NOT NULL,
    [OPDDate] [nvarchar](4000) NOT NULL,
    [OPDBeginTime] [nvarchar](10) NOT NULL,
    [OPDEndTime] [nvarchar](10) NOT NULL,
    [OPDRegisterID] [nvarchar](50) NOT NULL,
    [CaseNo] [nvarchar](4000) NOT NULL,
    [OutDepartmentID] [nvarchar](4000) NOT NULL,
    [OutDoctorID] [nvarchar](4000) NOT NULL,
    [CreateUserID] [varchar](8000),
    [CreateTime] [datetimeoffset](7) NOT NULL,
    [ModifyUserID] [nvarchar](4000),
    [ModifyTime] [datetimeoffset](7),
    [DeleteUserID] [nvarchar](4000),
    [DeleteTime] [datetimeoffset](7),
    [IsDeleted] [bit] NOT NULL,
    CONSTRAINT [PK_dbo.UserRegistrations] PRIMARY KEY ([RegistrationID])
)
go