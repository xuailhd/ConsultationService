#-----------------------------------------------------------
#	自动部署脚本
#	作者：郭明
#	日期：2017年10月26日
#-----------------------------------------------------------

#返回到解决方案根目录
cd ../../

#获取当前解决方案目录位置
$workdir= Get-Location

New-Website -Name 'www.kmwlyy.com' -HostHeader 'www.kmwlyy.com'  -Port 80 -ApplicationPool 'ASP.NET v4.0' -PhysicalPath "$workdir\HealthCloud.MessageNotice.Business.Web"  -force


