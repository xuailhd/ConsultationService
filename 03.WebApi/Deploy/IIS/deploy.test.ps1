#-----------------------------------------------------------
#	�Զ�����ű�
#	���ߣ�����
#	���ڣ�2017��10��26��
#-----------------------------------------------------------

#���Ի���
.deploy.ps1 't' 'test'

New-Website -Name 'Consultation.kmwlyy.com' -HostHeader 'Consultation.kmwlyy.com'  -Port 80 -ApplicationPool 'ASP.NET v4.0' -PhysicalPath "$workdir\HealthCloud.Consultation.WebApi"  -force