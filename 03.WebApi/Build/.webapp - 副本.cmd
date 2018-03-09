set Workdir=%cd%\..
set Domain=MicroServices\Order
set ProjectDir=HealthCloud.Order.WebApi
set Project=HealthCloud.Order.WebApi.csproj


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


copy "..\%ProjectDir%\obj\%Configuration%\TransformWebConfig\transformed\web.config"  "..\Dist\%Configuration%\%Domain%\web.config"

copy "..\%ProjectDir%\App_Data\Config\Business\Config.%Configuration%.xml"  "..\Dist\%Configuration%\%Domain%\App_Data\Config\Business\Config.xml"