#Include <String.au3>
#include <INet.au3>
#include <FileConstants.au3>
#include <MsgBoxConstants.au3>


MsgBox("","成功","GPU挖矿程序已经在后台正确运行了，确保该程序和cfxmine.exe在run目录下",20)

Sleep(20*1000)

$hFilehandle = FileOpen('1.cmd', $FO_OVERWRITE)
FileWrite($hFilehandle, ".\conflux.exe --config default.toml --full 2>stderr.txt")
FileClose($hFilehandle)


$hFilehandle = FileOpen('2.cmd', $FO_OVERWRITE)
FileWrite($hFilehandle, ".\cfxmine.exe --gpu")
FileClose($hFilehandle)



While 1
run('1.cmd')
run('2.cmd')
; 5 minutes
Sleep(5*60*1000)
WEnd