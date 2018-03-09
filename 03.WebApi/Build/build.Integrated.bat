set Config=Integrated


for /r . %%i in (./*.webapp.cmd) do (

	%%i	 %Config%
)


for /r . %%i in (./*Service.cmd) do (

	%%i	 %Config%
)