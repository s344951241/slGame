@echo off

::Proto文件路径
set SOURCE_PATH=.\Proto

::Protogen工具路径
set PROTOGEN_PATH=..\ProtoGen\protogen.exe
::C#文件生成路径
set TARGET_PATH=.\Cs

::Protoc工具路径
set PROTOC_PATH=..\ProtoGen\protoc.exe

::pb文件路径
set TARGETPB_PATH=.\Lua

::删除之前创建的文件
del %TARGET_PATH%\*.* /f /s /q
echo -------------------------------------------------------------

for /f "delims=" %%i in ('dir /b "%SOURCE_PATH%\*.proto"') do (
    
    echo 转换：%%i to %%~ni.cs
    %PROTOGEN_PATH% -i:%SOURCE_PATH%\%%i -o:%TARGET_PATH%\%%~ni.cs -ns:%%~ni
    echo 转换：%%i to %%~ni.pb
    %PROTOC_PATH%  --descriptor_set_out=%TARGETPB_PATH%\%%~ni.pb %SOURCE_PATH%\%%i
)

echo 转换完成

::复制到工程
xcopy %TARGET_PATH%\*.cs ..\..\Assets\Scripts\net\proto\ /Y
xcopy %TARGETPB_PATH%\*.pb ..\..\Assets\Resources\GameAssets\Lua\Proto\ /Y

python RunProto.py

pause