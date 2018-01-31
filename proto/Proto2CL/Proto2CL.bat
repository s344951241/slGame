@echo off

::Proto�ļ�·��
set SOURCE_PATH=.\Proto

::Protogen����·��
set PROTOGEN_PATH=..\ProtoGen\protogen.exe
::C#�ļ�����·��
set TARGET_PATH=.\Cs

::Protoc����·��
set PROTOC_PATH=..\ProtoGen\protoc.exe

::pb�ļ�·��
set TARGETPB_PATH=.\Lua

::ɾ��֮ǰ�������ļ�
del %TARGET_PATH%\*.* /f /s /q
echo -------------------------------------------------------------

for /f "delims=" %%i in ('dir /b "%SOURCE_PATH%\*.proto"') do (
    
    echo ת����%%i to %%~ni.cs
    %PROTOGEN_PATH% -i:%SOURCE_PATH%\%%i -o:%TARGET_PATH%\%%~ni.cs -ns:%%~ni
    echo ת����%%i to %%~ni.pb
    %PROTOC_PATH%  --descriptor_set_out=%TARGETPB_PATH%\%%~ni.pb %SOURCE_PATH%\%%i
)

echo ת�����

::���Ƶ�����
xcopy %TARGET_PATH%\*.cs ..\..\Assets\Scripts\net\proto\ /Y
xcopy %TARGETPB_PATH%\*.pb ..\..\Assets\Resources\GameAssets\Lua\Proto\ /Y

python RunProto.py

pause