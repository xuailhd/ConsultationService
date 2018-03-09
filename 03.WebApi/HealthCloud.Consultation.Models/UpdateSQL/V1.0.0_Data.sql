
delete SysConfigs where ConfigKey='Redis.AutoStart'
insert into SysConfigs(ConfigKey,ConfigValue) values('Redis.AutoStart','true')
delete SysConfigs where ConfigKey='Redis.DBNum'
insert into SysConfigs(ConfigKey,ConfigValue) values('Redis.DBNum','0')
delete SysConfigs where ConfigKey='Redis.KeyPrefix'
insert into SysConfigs(ConfigKey,ConfigValue,Remark) values('Redis.KeyPrefix','Consultation','»º´æKeyÇ°×º')
delete SysConfigs where ConfigKey='Redis.MaxReadPoolSize'
insert into SysConfigs(ConfigKey,ConfigValue) values('Redis.MaxReadPoolSize','200')
delete SysConfigs where ConfigKey='Redis.MaxWritePoolSize'
insert into SysConfigs(ConfigKey,ConfigValue) values('Redis.MaxWritePoolSize','200')
delete SysConfigs where ConfigKey='Redis.ReadServerList'
insert into SysConfigs(ConfigKey,ConfigValue) values('Redis.ReadServerList','10.2.29.205:6378')
delete SysConfigs where ConfigKey='Redis.WriteServerList'
insert into SysConfigs(ConfigKey,ConfigValue) values('Redis.WriteServerList','10.2.29.205:6378')

delete SysConfigs where ConfigKey='IMGStore.UrlPrefix'
insert into SysConfigs(ConfigKey,ConfigValue) values ('IMGStore.UrlPrefix','https://tstore.kmwlyy.com:8027/')

delete SysConfigs where ConfigKey='MQ.HostName'
insert into SysConfigs(ConfigKey,ConfigValue) values ('MQ.HostName','10.2.21.216')

delete SysConfigs where ConfigKey='MQ.Password'
insert into SysConfigs(ConfigKey,ConfigValue) values ('MQ.Password','123456')

delete SysConfigs where ConfigKey='MQ.Port'
insert into SysConfigs(ConfigKey,ConfigValue) values ('MQ.Port','5672')

delete SysConfigs where ConfigKey='MQ.UserName'
insert into SysConfigs(ConfigKey,ConfigValue) values ('MQ.UserName','KMEHosp')

delete SysConfigs where ConfigKey='MQ.VirtualHost'
insert into SysConfigs(ConfigKey,ConfigValue) values ('MQ.VirtualHost','/test')


