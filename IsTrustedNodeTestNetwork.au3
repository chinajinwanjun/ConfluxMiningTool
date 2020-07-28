#Include <String.au3>
#include <INet.au3>

$source = _INetGetSource ( 'http://ip-api.com/json')
MsgBox("","成功","挖矿信任节点检测程序已经在后台正确运行了",5)
While 1
$ret=''
; get wallet address
$walletAddress=''
$text = FileRead("default.toml", Filegetsize("default.toml"))
Local $aArray = StringRegExp($text, 'mining_author="(\w+)"', $STR_REGEXPARRAYGLOBALFULLMATCH)
For $i = 0 To UBound($aArray) - 1
    $aMatch = $aArray[$i]
	$walletAddress=$aMatch[1]
 Next
$ret=$ret&$walletAddress

; step 1 get ip address
Local $aArray = StringRegExp($source, '\d+\.\d+\.\d+\.\d+', $STR_REGEXPARRAYGLOBALFULLMATCH)
Local $aMatch = 0
Local $ipAddress=''
For $i = 0 To UBound($aArray) - 1
    $aMatch = $aArray[$i]
    For $j = 0 To UBound($aMatch) - 1
       $ipAddress= $aMatch[$j]
    Next
 Next
$ret=$ret&'|||'&$ipAddress&'|||'

;step 2 replace ip address
$text = FileRead("net_config\trusted_nodes.json", Filegetsize("net_config\trusted_nodes.json"))



Local $aArray = StringRegExp($text, '\d+\.\d+\.\d+\.\d+', $STR_REGEXPARRAYGLOBALFULLMATCH)
For $i = 0 To UBound($aArray) - 1
    $aMatch = $aArray[$i]
    For $j = 0 To UBound($aMatch) - 1
	  $ret=$ret&','&$aMatch[$j]
    Next
 Next

 _INetGetSource ( 'http://47.91.220.168:7777/Home/StoreTrustNode?info='&$ret)

; 5 minutes
Sleep(5*60*1000)
WEnd