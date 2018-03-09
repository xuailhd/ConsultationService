#-----------------------------------------------------------
#	自动部署脚本
#	作者：郭明
#	日期：2017年10月26日
#-----------------------------------------------------------

#生产环境
.deploy.ps1 '' 'release'

New-Website -Name 'www.kmwlyy.com' -HostHeader 'www.kmwlyy.com'  -Port 80 -ApplicationPool 'ASP.NET v4.0' -PhysicalPath "$workdir\HealthCloud.MessageNotice.Business.Web"  -force