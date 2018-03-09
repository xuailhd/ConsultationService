#-----------------------------------------------------------
#	�Զ�����ű�
#	���ߣ����� geniusmign@qq.com
#	���ڣ�2017��10��26��
#-----------------------------------------------------------

#���ص����������Ŀ¼
cd ../../

#��ȡ��ǰ�������Ŀ¼λ��
$workdir= Get-Location

#��ȡ����ͷǰ׺ (����='d',����='t',Ԥ����='p',����='')
$hostHeaderPrefix=$args[0]
$target=$args[1]

#��ȡ����Ŀ¼�µ������ļ���Ȼ���Զ�����վ��
Get-ChildItem  $workdir/dist | where {$_.name -like "*.com"}  | ForEach-Object -Process{

	#����վ�㣨ʹ��Ĭ��Ӧ�ó���أ��˿�ʹ��80��ʹ��Ŀ¼������Ϊ����ͷ��
	New-Website -Name $_ -HostHeader $hostHeaderPrefix$_  -Port 80 -ApplicationPool 'ASP.NET v4.0' -PhysicalPath "$workdir/dist/$target/$_"  -force

	#����վ��
	Start-Website -Name $_
}

pause