@set WORKSPACE=..
@set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
@set CONF_ROOT=.

@dotnet %LUBAN_DLL% ^
--conf %CONF_ROOT%/luban.conf ^
-t client ^
--timeZone "UTC" ^
--customTemplateDir %CONF_ROOT%\Templates ^
-c cs-bin ^
-d bin ^
-d json ^
-x outputCodeDir=%WORKSPACE%/Assets/Scripts/Hotfix/Common/Data/Gen ^
-x bin.outputDataDir=%WORKSPACE%/Assets/AssetsPackage/Data ^
-x json.outputDataDir=%WORKSPACE%/DataTables/Output/json ^
-x codeStyle=csharp-default ^
-x dataExporter=default ^
-x outputSaver=local 

@pause 