set Workdir=%cd%\..
set Domain=MicroServices\Consultation
set ProjectDir=HealthCloud.Consultation.WebApi
set Project=HealthCloud.Consultation.WebApi.csproj


set Source=%Workdir%\%ProjectDir%
set OutPath=%Workdir%\Dist\%1\%Domain%
set Configuration=%1

"C:\Program Files (x86)\MSBuild\14.0\Bin\MsBuild.exe" 	^
	"%Source%\%Project%" 	^
	/toolsversion:14.0 ^
	/t:build ^
	/t:ResolveReferences;Compile 	^
	/t:_CopyWebApplication 	^
	/t:TransformWebConfig ^
	/p:VisualStudioVersion=14.0 	^
	/p:TargetFrameworkVersion=v4.5 	^
	/p:WebProjectOutputDir="%OutPath%" 	^
	/p:OutputPath="%OutPath%\bin" ^
	/p:Configuration=%Configuration%  


copy "..\%ProjectDir%\web.config"  "..\Dist\%Configuration%\%Domain%\web.config"