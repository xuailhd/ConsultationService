#-----------------------------------------------------------
#	�Զ�����ű�
#	���ߣ�����
#	���ڣ�2017��10��26��
#-----------------------------------------------------------

#���ص����������Ŀ¼
cd ../../

#��ȡ��ǰ�������Ŀ¼λ��
$workdir= Get-Location

New-Website -Name 'www.kmwlyy.com' -HostHeader 'www.kmwlyy.com'  -Port 80 -ApplicationPool 'ASP.NET v4.0' -PhysicalPath "$workdir\HealthCloud.MessageNotice.Business.Web"  -force


