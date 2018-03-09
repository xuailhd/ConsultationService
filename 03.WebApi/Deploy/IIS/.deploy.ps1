#-----------------------------------------------------------
#	自动部署脚本
#	作者：郭明 geniusmign@qq.com
#	日期：2017年10月26日
#-----------------------------------------------------------

#返回到解决方案根目录
cd ../../

#获取当前解决方案目录位置
$workdir= Get-Location

#获取主机头前缀 (开发='d',测试='t',预发布='p',生产='')
$hostHeaderPrefix=$args[0]
$target=$args[1]

#获取发布目录下的所有文件夹然后自动创建站点
Get-ChildItem  $workdir/dist | where {$_.name -like "*.com"}  | ForEach-Object -Process{

	#创建站点（使用默认应用程序池，端口使用80，使用目录名称作为主机头）
	New-Website -Name $_ -HostHeader $hostHeaderPrefix$_  -Port 80 -ApplicationPool 'ASP.NET v4.0' -PhysicalPath "$workdir/dist/$target/$_"  -force

	#启动站点
	Start-Website -Name $_
}

pause