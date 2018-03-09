#-----------------------------------------------------------
#	自动部署脚本
#	作者：郭明
#	日期：2017年10月26日
#-----------------------------------------------------------

#测试环境
.deploy.ps1 't' 'test'

New-Website -Name 'Consultation.kmwlyy.com' -HostHeader 'Consultation.kmwlyy.com'  -Port 80 -ApplicationPool 'ASP.NET v4.0' -PhysicalPath "$workdir\HealthCloud.Consultation.WebApi"  -force