
set Workdir="%cd%/../../"
set Source=%Workdir%/KMEHosp.UI.WinSvr.JobService
set OutPath=%Workdir%/Dist/%1/WinSrv
set Project=KMEHosp.UI.WinSvr.JobService.csproj

set Configuration=%1

"C:\Program Files (x86)\MSBuild\14.0\Bin\MsBuild.exe" 	^
	%Source%\%Project% 	^
	/toolsversion:14.0 ^
	/t:rebuild ^
	/t:ResolveReferences;Compile 	^
	/t:_CopyWebApplication 	^
	/p:VisualStudioVersion=14.0 	^
	/p:Configuration=Release;TargetFrameworkVersion=v4.6.1 	^
	/p:WebProjectOutputDir=%OutPath% 	^
	/p:OutputPath=%OutPath%\bin ^
	/t:TransformWebConfig ^
	/p:Configuration=%Configuration%  