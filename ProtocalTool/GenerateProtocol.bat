echo on
set Protoc=.\protoc.exe
set InputDirectory=.\
set OutputDirectory=.\..\Common\Google.Protocol\

@set /p ConfigName=name:
%Protoc% -I=%InputDirectory% --csharp_out=%OutputDirectory% %ConfigName%
pause