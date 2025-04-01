©
KC:\HeroSystem\walletService\WalletService.Api\Controllers\BaseController.cs
	namespace 	
WalletService
 
. 
Api 
. 
Controllers '
;' (
public 
class 
BaseController 
: 
ControllerBase ,
{ 
	protected		 
static		 
ServicesResponse		 %
Success		& -
(		- .
object		. 4
data		5 9
)		9 :
=>

 

new

 
(

 
)

 
{ 	
Success 
= 
true 
, 
Code 
= 
( 
int 
) 
HttpStatusCode )
.) *
OK* ,
,, -
Data 
= 
data 
} 	
;	 

	protected 
static 
ServicesResponse %
Fail& *
(* +
string+ 1
message2 9
)9 :
=> 

new 
( 
) 
{ 	
Success 
= 
false 
, 
Code 
= 
( 
int 
) 
HttpStatusCode )
.) *

BadRequest* 4
,4 5
Message 
= 
message 
} 	
;	 

} ìP
NC:\HeroSystem\walletService\WalletService.Api\Controllers\CoinPayController.cs
	namespace 	
WalletService
 
. 
Api 
. 
Controllers '
;' (
[ 
ApiController 
] 
[ 

ApiVersion 
( 
$str 
) 
] 
[		 
Route		 
(		 
$str		 *
)		* +
]		+ ,
public

 
class

 
CoinPayController

 
:

  
BaseController

! /
{ 
private 
readonly 
ICoinPayService $
_coinPayService% 4
;4 5
private 
readonly 
ILogger 
< 
CoinPayController .
>. /
_logger0 7
;7 8
public 

CoinPayController 
( 
ICoinPayService ,
coinPayService- ;
,; <
ILogger= D
<D E
CoinPayControllerE V
>V W
loggerX ^
)^ _
{ 
_coinPayService 
= 
coinPayService (
;( )
_logger 
= 
logger 
; 
} 
[ 
HttpPost 
( 
$str !
)! "
]" #
public 

async 
Task 
< 
IActionResult #
># $
CreateTransaction% 6
(6 7
[7 8
FromBody8 @
]@ A$
CreateTransactionRequestB Z
request[ b
)b c
{ 
var 
result 
= 
await 
_coinPayService *
.* +
CreateTransaction+ <
(< =
request= D
)D E
;E F
return 
result 
? 
. 
Data 
is 
null #
?$ %
Ok& (
(( )
Fail) -
(- .
$str. 5
)5 6
)6 7
:8 9
Ok: <
(< =
result= C
)C D
;D E
} 
[ 
HttpPost 
( 
$str 
) 
] 
public 

async 
Task 
< 
IActionResult #
># $
CreateChannel% 2
(2 3
[3 4
FromBody4 <
]< =$
CreateTransactionRequest> V
requestW ^
)^ _
{   
var!! 
result!! 
=!! 
await!! 
_coinPayService!! *
.!!* +
CreateChannel!!+ 8
(!!8 9
request!!9 @
)!!@ A
;!!A B
return## 
result## 
?## 
.## 
Data## 
is## 
null## #
?##$ %
Ok##& (
(##( )
Fail##) -
(##- .
$str##. Q
)##Q R
)##R S
:##T U
Ok##V X
(##X Y
Success##Y `
(##` a
result##a g
)##g h
)##h i
;##i j
}$$ 
[&& 
HttpGet&& 
(&& 
$str&& &
)&&& '
]&&' (
public'' 

async'' 
Task'' 
<'' 
IActionResult'' #
>''# $"
GetNetworkByIdCurrency''% ;
(''; <
[''< =
	FromQuery''= F
]''F G
int''G J

idCurrency''K U
)''U V
{(( 
var)) 
result)) 
=)) 
await)) 
_coinPayService)) *
.))* +#
GetNetworksByIdCurrency))+ B
())B C

idCurrency))C M
)))M N
;))N O
return++ 
result++ 
?++ 
.++ 
Data++ 
is++ 
null++ #
?++$ %
Ok++& (
(++( )
Fail++) -
(++- .
$str++. V
)++V W
)++W X
:++Y Z
Ok++[ ]
(++] ^
result++^ d
)++d e
;++e f
},, 
[.. 
HttpPost.. 
(.. 
$str.. 
).. 
].. 
public// 

async// 
Task// 
<// 
IActionResult// #
>//# $
CreateAddress//% 2
(//2 3
[//3 4
FromBody//4 <
]//< =
CreateAddresRequest//> Q
request//R Y
)//Y Z
{00 
var11 
result11 
=11 
await11 
_coinPayService11 *
.11* +
CreateAddress11+ 8
(118 9
request119 @
)11@ A
;11A B
return33 
result33 
?33 
.33 
Data33 
is33 
null33 #
?33$ %
Ok33& (
(33( )
Fail33) -
(33- .
$str33. Q
)33Q R
)33R S
:33T U
Ok33V X
(33X Y
result33Y _
)33_ `
;33` a
}44 
[66 
HttpGet66 
(66 
$str66 !
)66! "
]66" #
public77 

async77 
Task77 
<77 
IActionResult77 #
>77# $
GetTransactionById77% 7
(777 8
int778 ;
idTransaction77< I
)77I J
{88 
var99 
result99 
=99 
await99 
_coinPayService99 *
.99* +
GetTransactionById99+ =
(99= >
idTransaction99> K
)99K L
;99L M
return;; 
result;; 
?;; 
.;; 
Data;; 
is;; 
null;; #
?;;$ %
Ok;;& (
(;;( )
Fail;;) -
(;;- .
$str;;. F
);;F G
);;G H
:;;I J
Ok;;K M
(;;M N
Success;;N U
(;;U V
result;;V \
);;\ ]
);;] ^
;;;^ _
}<< 
[>> 
HttpPost>> 
(>> 
$str>> 
)>> 
]>> 
public?? 

async?? 
Task?? 
<?? 
IActionResult?? #
>??# $
Webhook??% ,
(??, -
)??- .
{@@ 
RequestAA 
.AA 
EnableBufferingAA 
(AA  
)AA  !
;AA! "
stringBB 
requestBodyBB 
;BB 
usingDD 
(DD 
varDD 
readerDD 
=DD 
newDD 
StreamReaderDD  ,
(DD, -
RequestDD- 4
.DD4 5
BodyDD5 9
,DD9 :
	leaveOpenDD; D
:DDD E
trueDDF J
)DDJ K
)DDK L
{EE 	
requestBodyFF 
=FF 
awaitFF 
readerFF  &
.FF& '
ReadToEndAsyncFF' 5
(FF5 6
)FF6 7
;FF7 8
}GG 	
RequestII 
.II 
BodyII 
.II 
PositionII 
=II 
$numII  !
;II! "
tryKK 
{LL 	
varMM 
resultMM 
=MM 
awaitMM 
_coinPayServiceMM .
.MM. /'
ReceiveCoinPayNotificationsMM/ J
(MMJ K
requestBodyMMK V
)MMV W
;MMW X
_loggerOO 
.OO 
LogInformationOO "
(OO" #
$strOO# @
,OO@ A
resultOOB H
)OOH I
;OOI J
returnQQ 
resultQQ 
isQQ 
falseQQ "
?QQ# $
OkQQ% '
(QQ' (
FailQQ( ,
(QQ, -
$strQQ- W
)QQW X
)QQX Y
:QQZ [
OkQQ\ ^
(QQ^ _
)QQ_ `
;QQ` a
}RR 	
catchSS 
(SS 
	ExceptionSS 
exSS 
)SS 
{TT 	
_loggerUU 
.UU 
LogErrorUU 
(UU 
exUU 
,UU  
$strUU! M
)UUM N
;UUN O
returnWW 

StatusCodeWW 
(WW 
$numWW !
,WW! "
$strWW# <
+WW= >
exWW? A
.WWA B
MessageWWB I
)WWI J
;WWJ K
}XX 	
}YY 
[[[ 
HttpPost[[ 
([[ 
$str[[ 
)[[ 
][[ 
public\\ 

async\\ 
Task\\ 
<\\ 
IActionResult\\ #
>\\# $
	SendFunds\\% .
(\\. /
[\\/ 0
FromBody\\0 8
]\\8 9
WithDrawalRequest\\: K
[\\K L
]\\L M
request\\N U
)\\U V
{]] 
var^^ 
result^^ 
=^^ 
await^^ 
_coinPayService^^ *
.^^* +
	SendFunds^^+ 4
(^^4 5
request^^5 <
)^^< =
;^^= >
return`` 
result`` 
is`` 
null`` 
?`` 
Ok``  "
(``" #
Fail``# '
(``' (
$str``( O
)``O P
)``P Q
:``R S
Ok``T V
(``V W
Success``W ^
(``^ _
result``_ e
)``e f
)``f g
;``g h
}aa 
[cc 
HttpGetcc 
(cc 
$strcc (
)cc( )
]cc) *
publicdd 

asyncdd 
Taskdd 
<dd 
IActionResultdd #
>dd# $%
GetTransactionByReferencedd% >
(dd> ?
[dd? @
	FromQuerydd@ I
]ddI J
stringddK Q
	referenceddR [
)dd[ \
{ee 
varff 
resultff 
=ff 
awaitff 
_coinPayServiceff *
.ff* +%
GetTransactionByReferenceff+ D
(ffD E
	referenceffE N
)ffN O
;ffO P
returnhh 
Okhh 
(hh 
Successhh 
(hh 
resulthh  
)hh  !
)hh! "
;hh" #
}ii 
}ll ªA
SC:\HeroSystem\walletService\WalletService.Api\Controllers\CoinPaymentsController.cs
	namespace 	
WalletService
 
. 
Api 
. 
Controllers '
;' (
[ 
ApiController 
] 
[		 

ApiVersion		 
(		 
$str		 
)		 
]		 
[

 
Route

 
(

 
$str

 .
)

. /
]

/ 0
public 
class "
CoinPaymentsController #
:$ %
BaseController& 4
{ 
private 
readonly 
IConPaymentService '
_conPaymentService( :
;: ;
public 
"
CoinPaymentsController !
(! "
IConPaymentService" 4
conPaymentService5 F
)F G
{ 
_conPaymentService 
= 
conPaymentService .
;. /
} 
[ 
HttpGet 
( 
$str 
) 
] 
public 

async 
Task 
< 
IActionResult #
># $

GetProfile% /
(/ 0
string0 6
pbntag7 =
)= >
{ 
var 
response 
= 
await 
_conPaymentService /
./ 0
GetPayByNameProfile0 C
(C D
pbntagD J
)J K
;K L
if 

( 
string 
. 
IsNullOrEmpty  
(  !
response! )
.) *
Error* /
)/ 0
||1 3
response4 <
.< =
Error= B
.B C
ToLowerC J
(J K
)K L
==M O
$strP T
)T U
{ 	
return 
Ok 
( 
response 
. 
Result %
)% &
;& '
} 	
return   
Ok   
(   
Fail   
(   
$str   *
)  * +
)  + ,
;  , -
}!! 
[## 
HttpGet## 
(## 
$str##  
)##  !
]##! "
public$$ 

async$$ 
Task$$ 
<$$ 
IActionResult$$ #
>$$# $
GetDepositAddress$$% 6
($$6 7
string$$7 =
currency$$> F
)$$F G
{%% 
var&& 
response&& 
=&& 
await&& 
_conPaymentService&& /
.&&/ 0
GetDepositAddress&&0 A
(&&A B
currency&&B J
)&&J K
;&&K L
if(( 

((( 
string(( 
.(( 
IsNullOrEmpty((  
(((  !
response((! )
!(() *
.((* +
Error((+ 0
)((0 1
||((2 4
response((5 =
.((= >
Error((> C
.((C D
ToLower((D K
(((K L
)((L M
==((N P
$str((Q U
)((U V
{)) 	
return** 
Ok** 
(** 
response** 
.** 
Result** %
)**% &
;**& '
}++ 	
return-- 
Ok-- 
(-- 
Fail-- 
(-- 
$str-- *
)--* +
)--+ ,
;--, -
}.. 
[00 
HttpGet00 
(00 
$str00 
)00 
]00  
public11 

async11 
Task11 
<11 
IActionResult11 #
>11# $
GetCoinBalances11% 4
(114 5
bool115 9
includeZeroBalances11: M
)11M N
{22 
var33 
response33 
=33 
await33 
_conPaymentService33 /
.33/ 0
GetCoinBalances330 ?
(33? @
includeZeroBalances33@ S
)33S T
;33T U
return55 
Ok55 
(55 
Success55 
(55 
response55 "
.55" #
Result55# )
)55) *
)55* +
;55+ ,
}66 
[88 
HttpPost88 
(88 
$str88 
)88 
]88 
public99 

async99 
Task99 
<99 
IActionResult99 #
>99# $
CreatePayment99% 2
(992 3
ConPaymentRequest993 D
request99E L
)99L M
{:: 
var;; 
response;; 
=;; 
await;; 
_conPaymentService;; /
.;;/ 0
CreatePayment;;0 =
(;;= >
request;;> E
);;E F
;;;F G
if== 

(== 
string== 
.== 
IsNullOrEmpty==  
(==  !
response==! )
!==) *
.==* +
Error==+ 0
)==0 1
||==2 4
response==5 =
.=== >
Error==> C
.==C D
ToLower==D K
(==K L
)==L M
====N P
$str==Q U
)==U V
{>> 	
return?? 
Ok?? 
(?? 
response?? 
.?? 
Result?? %
)??% &
;??& '
}@@ 	
returnBB 
OkBB 
(BB 
FailBB 
(BB 
$strBB ;
)BB; <
)BB< =
;BB= >
}CC 
[EE 
HttpGetEE 
(EE 
$strEE !
)EE! "
]EE" #
publicFF 

asyncFF 
TaskFF 
<FF 
IActionResultFF #
>FF# $
GetTransactionInfoFF% 7
(FF7 8
stringFF8 >
idTransactionFF? L
,FFL M
boolFFN R
fullInfoFFS [
)FF[ \
{GG 
varHH 
responseHH 
=HH 
awaitHH 
_conPaymentServiceHH /
.HH/ 0
GetTransactionInfoHH0 B
(HHB C
idTransactionHHC P
,HHP Q
fullInfoHHR Z
)HHZ [
;HH[ \
returnJJ 
OkJJ 
(JJ 
SuccessJJ 
(JJ 
responseJJ "
.JJ" #
ResultJJ# )
)JJ) *
)JJ* +
;JJ+ ,
}KK 
[MM 
HttpPostMM 
(MM 
$strMM 
)MM  
]MM  !
publicNN 

asyncNN 
TaskNN 
<NN 
IActionResultNN #
>NN# $
CoinPaymentsIpnNN% 4
(NN4 5
[NN5 6
FromFormNN6 >
]NN> ?

IpnRequestNN@ J

ipnRequestNNK U
)NNU V
{OO 
varPP 
responsePP 
=PP 
awaitPP 
_conPaymentServicePP /
.PP/ 0
ProcessIpnAsyncPP0 ?
(PP? @

ipnRequestPP@ J
,PPJ K
RequestPPL S
.PPS T
HeadersPPT [
)PP[ \
;PP\ ]
ifRR 

(RR 
responseRR 
==RR 
$strRR  
)RR  !
returnSS 
OkSS 
(SS 
responseSS 
)SS 
;SS  
elseTT 
returnUU 

BadRequestUU 
(UU 
responseUU &
)UU& '
;UU' (
}VV 
[XX 
HttpPostXX 
(XX 
$strXX $
)XX$ %
]XX% &
publicYY 

asyncYY 
TaskYY 
<YY 
IActionResultYY #
>YY# $ 
CreateMassWithdrawalYY% 9
(YY9 :
[YY: ;
FromBodyYY; C
]YYC D
WalletsRequestYYE S
[YYS T
]YYT U
requestsYYV ^
)YY^ _
{ZZ 
var[[ 
response[[ 
=[[ 
await[[ 
_conPaymentService[[ /
.[[/ 0 
CreateMassWithdrawal[[0 D
([[D E
requests[[E M
)[[M N
;[[N O
return\\ 
response\\ 
is\\ 
null\\ 
?\\  !
Ok\\" $
(\\$ %
Fail\\% )
(\\) *
$str\\* Y
)\\Y Z
)\\Z [
:\\\ ]
Ok\\^ `
(\\` a
response\\a i
)\\i j
;\\j k
}]] 
}^^ Äl
NC:\HeroSystem\walletService\WalletService.Api\Controllers\InvoiceController.cs
	namespace 	
WalletService
 
. 
Api 
. 
Controllers '
;' (
[

 
ApiController

 
]

 
[ 

ApiVersion 
( 
$str 
) 
] 
[ 
Route 
( 
$str *
)* +
]+ ,
public 
class 
InvoiceController 
:  
BaseController! /
{ 
private 
readonly 
IInvoiceService $
_invoiceService% 4
;4 5
public 

InvoiceController 
( 
IInvoiceService ,
invoiceService- ;
); <
{ 
_invoiceService 
= 
invoiceService (
;( )
} 
[ 
HttpGet 
( 
$str %
)% &
]& '
public 

async 
Task 
< 
IActionResult #
># $"
GetAllInvoicesByUserId% ;
(; <
[< =
	FromQuery= F
]F G
intH K
idL N
)N O
{ 
var 
result 
= 
await 
_invoiceService *
.* +"
GetAllInvoiceUserAsync+ A
(A B
idB D
)D E
;E F
return 
Ok 
( 
result 
) 
; 
} 
[ 
HttpGet 
( 
$str 
) 
] 
public 

async 
Task 
< 
IActionResult #
># $
GetAllInvoices% 3
(3 4
[4 5
	FromQuery5 >
]> ?
PaginationRequest@ Q
requestR Y
)Y Z
{ 
try   
{!! 	
var"" 
result"" 
="" 
await"" 
_invoiceService"" .
."". /
GetAllInvoices""/ =
(""= >
request""> E
)""E F
;""F G
return## 
Ok## 
(## 
Success## 
(## 
result## $
)##$ %
)##% &
;##& '
}$$ 	
catch%% 
(%% 
ArgumentException%%  
ex%%! #
)%%# $
{&& 	
return'' 

BadRequest'' 
('' 
new'' !
{''" #
message''$ +
='', -
ex''. 0
.''0 1
Message''1 8
}''9 :
)'': ;
;''; <
}(( 	
})) 
[++ 
HttpPost++ 
(++ 
$str++ -
)++- .
]++. /
public,, 

async,, 
Task,, 
<,, 
IActionResult,, #
>,,# $)
RevertCoinPaymentTransactions,,% B
(,,B C
),,C D
{-- 
var.. 
result.. 
=.. 
await.. 
_invoiceService.. *
...* +1
%RevertUnconfirmedOrUnpaidTransactions..+ P
(..P Q
)..Q R
;..R S
return00 
Ok00 
(00 
result00 
)00 
;00 
}11 
[33 
HttpGet33 
(33 
$str33 7
)337 8
]338 9
public44 

async44 
Task44 
<44 
IActionResult44 #
>44# $4
(GetAllInvoicesForTradingAcademyPurchases44% M
(44M N
)44N O
{55 
var66 
result66 
=66 
await66 
_invoiceService66 *
.66* +4
(GetAllInvoicesForTradingAcademyPurchases66+ S
(66S T
)66T U
;66U V
return88 
result88 
.88 
IsNullOrEmpty88 #
(88# $
)88$ %
?88& '
Ok88( *
(88* +
Fail88+ /
(88/ 0
$str880 J
)88J K
)88K L
:88M N
Ok88O Q
(88Q R
Success88R Y
(88Y Z
result88Z `
)88` a
)88a b
;88b c
}99 
[;; 
HttpPost;; 
(;; 
$str;; 1
);;1 2
];;2 3
public<< 

async<< 
Task<< 
<<< 
IActionResult<< #
><<# $-
!SendInvitationsForUpcomingCourses<<% F
(<<F G
[<<G H
	FromQuery<<H Q
]<<Q R
string<<S Y
link<<Z ^
,<<^ _
[<<` a
	FromQuery<<a j
]<<j k
string<<l r
code<<s w
)<<w x
{== 
var>> 
result>> 
=>> 
await>> 
_invoiceService>> *
.>>* +-
!SendInvitationsForUpcomingCourses>>+ L
(>>L M
link>>M Q
,>>Q R
code>>S W
)>>W X
;>>X Y
return@@ 
result@@ 
.@@ 
IsNullOrEmpty@@ #
(@@# $
)@@$ %
?@@& '
Ok@@( *
(@@* +
Fail@@+ /
(@@/ 0
$str@@0 J
)@@J K
)@@K L
:@@M N
Ok@@O Q
(@@Q R
Success@@R Y
(@@Y Z
result@@Z `
)@@` a
)@@a b
;@@b c
}AA 
[CC 
HttpGetCC 
(CC 
$strCC .
)CC. /
]CC/ 0
publicDD 

asyncDD 
TaskDD 
<DD 
IActionResultDD #
>DD# $+
GetAllInvoicesForModelOneAndTwoDD$ C
(DDC D
)DDD E
{EE 
varFF 
resultFF 
=FF 
awaitFF 
_invoiceServiceFF *
.FF* +(
GetAllInvoicesModelOneAndTwoFF+ G
(FFG H
)FFH I
;FFI J
returnHH 
resultHH 
.HH 
IsNullOrEmptyHH #
(HH# $
)HH$ %
?HH& '
OkHH( *
(HH* +
FailHH+ /
(HH/ 0
$strHH0 K
)HHK L
)HHL M
:HHN O
OkHHP R
(HHR S
SuccessHHS Z
(HHZ [
resultHH[ a
)HHa b
)HHb c
;HHc d
}II 
[KK 
HttpPostKK 
(KK 
$strKK 6
)KK6 7
]KK7 8
publicLL 

asyncLL 
TaskLL 
<LL 
IActionResultLL #
>LL# $2
&ProcessAndReturnBalancesForModels1A1B2LL% K
(LLK L
[LLL M
FromBodyLLM U
]LLU V+
ModelBalancesAndInvoicesRequestLLW v
requestLLw ~
)LL~ 
{MM 
varNN 
resultNN 
=NN 
awaitNN 
_invoiceServiceNN *
.NN* +2
&ProcessAndReturnBalancesForModels1A1B2NN+ Q
(NNQ R
requestNNR Y
)NNY Z
;NNZ [
returnPP 
resultPP 
isPP 
nullPP 
?PP 
OkPP  "
(PP" #
FailPP# '
(PP' (
$strPP( Q
)PPQ R
)PPR S
:PPT U
OkPPV X
(PPX Y
SuccessPPY `
(PP` a
resultPPa g
)PPg h
)PPh i
;PPi j
}QQ 
[SS 
HttpGetSS 
(SS 
$strSS 
)SS 
]SS 
publicTT 

asyncTT 
TaskTT 
<TT 
IActionResultTT #
>TT# $
CreateInvoiceTT% 2
(TT2 3
[TT3 4
	FromQueryTT4 =
]TT= >
intTT? B
	invoiceIdTTC L
)TTL M
{UU 
varVV 
resultVV 
=VV 
awaitVV 
_invoiceServiceVV *
.VV* +
CreateInvoiceVV+ 8
(VV8 9
	invoiceIdVV9 B
)VVB C
;VVC D
ifWW 

(WW 
resultWW 
.WW 
LengthWW 
==WW 
$numWW 
)WW 
{XX 	
returnYY 
NotFoundYY 
(YY 
$strYY ]
)YY] ^
;YY^ _
}ZZ 	
Response\\ 
.\\ 
ContentType\\ 
=\\ 
$str\\ 0
;\\0 1
Response]] 
.]] 
Headers]] 
.]] 
ContentDisposition]] +
=]], -
$"]]. 0
$str]]0 M
{]]M N
	invoiceId]]N W
}]]W X
$str]]X \
"]]\ ]
;]]] ^
return__ 
File__ 
(__ 
result__ 
,__ 
$str__ -
)__- .
;__. /
}`` 
[bb 
HttpGetbb 
(bb 
$strbb *
)bb* +
]bb+ ,
publiccc 

asynccc 
Taskcc 
<cc 
IActionResultcc #
>cc# $$
CreateInvoiceByReferencecc% =
(cc= >
[cc> ?
	FromQuerycc? H
]ccH I
stringccJ P
	referenceccQ Z
)ccZ [
{dd 
varee 
resultee 
=ee 
awaitee 
_invoiceServiceee *
.ee* +$
CreateInvoiceByReferenceee+ C
(eeC D
	referenceeeD M
)eeM N
;eeN O
ifff 

(ff 
resultff 
==ff 
nullff 
)ff 
returngg 
NotFoundgg 
(gg 
$strgg ]
)gg] ^
;gg^ _
Responseii 
.ii 
ContentTypeii 
=ii 
$strii 0
;ii0 1
Responsejj 
.jj 
Headersjj 
.jj 
ContentDispositionjj +
=jj, -
$"jj. 0
$strjj0 M
{jjM N
	referencejjN W
}jjW X
$strjjX \
"jj\ ]
;jj] ^
Responsekk 
.kk 
Headerskk 
.kk 
Addkk 
(kk 
$strkk )
,kk) *
resultkk+ 1
.kk1 2
BrandIdkk2 9
.kk9 :
ToStringkk: B
(kkB C
)kkC D
)kkD E
;kkE F
returnmm 
Filemm 
(mm 
resultmm 
.mm 

PdfContentmm %
??mm& (
throwmm) .
newmm/ 2%
InvalidOperationExceptionmm3 L
(mmL M
)mmM N
,mmN O
$strmmP a
)mma b
;mmb c
}nn 
[pp 
HttpPostpp 
(pp 
$strpp &
)pp& '
]pp' (
publicqq 

asyncqq 
Taskqq 
<qq 
IActionResultqq #
>qq# $"
HandleDebitTransactionqq% ;
(qq; <
[qq< =
FromBodyqq= E
]qqE F#
DebitTransactionRequestqqG ^
debitRequestqq_ k
)qqk l
{rr 
tryss 
{tt 	
varuu 
resultuu 
=uu 
awaituu 
_invoiceServiceuu .
.uu. /"
HandleDebitTransactionuu/ E
(uuE F
debitRequestuuF R
)uuR S
;uuS T
ifww 
(ww 
resultww 
==ww 
nullww 
)ww 
returnxx 

BadRequestxx !
(xx! "
$strxx" H
)xxH I
;xxI J
returnzz 
Okzz 
(zz 
Successzz 
(zz 
resultzz $
)zz$ %
)zz% &
;zz& '
}{{ 	
catch|| 
(|| 
	Exception|| 
ex|| 
)|| 
{}} 	
return~~ 

BadRequest~~ 
(~~ 
$"~~  
$str~~  B
{~~B C
ex~~C E
.~~E F
Message~~F M
}~~M N
"~~N O
)~~O P
;~~P Q
} 	
}
ÄÄ 
[
ÇÇ 
HttpGet
ÇÇ 
(
ÇÇ 
$str
ÇÇ 
)
ÇÇ 
]
ÇÇ 
public
ÉÉ 

async
ÉÉ 
Task
ÉÉ 
<
ÉÉ 
IActionResult
ÉÉ #
>
ÉÉ# $#
ExportInvoicesToExcel
ÉÉ% :
(
ÉÉ: ;
[
ÉÉ; <
	FromQuery
ÉÉ< E
]
ÉÉE F
DateTime
ÉÉG O
?
ÉÉO P
	startDate
ÉÉQ Z
=
ÉÉ[ \
null
ÉÉ] a
,
ÉÉa b
[
ÉÉc d
	FromQuery
ÉÉd m
]
ÉÉm n
DateTime
ÉÉo w
?
ÉÉw x
endDateÉÉy Ä
=ÉÉÅ Ç
nullÉÉÉ á
)ÉÉá à
{
ÑÑ 
try
ÖÖ 
{
ÜÜ 	
var
áá 
stream
áá 
=
áá 
await
áá 
_invoiceService
áá .
.
áá. /!
GenerateExcelReport
áá/ B
(
ááB C
	startDate
ááC L
,
ááL M
endDate
ááN U
)
ááU V
;
ááV W
return
àà 
File
àà 
(
àà 
stream
ââ 
,
ââ 
$str
ää S
,
ääS T
$"
ãã 
$str
ãã #
{
ãã# $
DateTime
ãã$ ,
.
ãã, -
Now
ãã- 0
:
ãã0 1
$str
ãã1 A
}
ããA B
$str
ããB G
"
ããG H
)
åå 
;
åå 
}
çç 	
catch
éé 
(
éé 
	Exception
éé 
ex
éé 
)
éé 
{
èè 	
return
êê 

BadRequest
êê 
(
êê 
new
êê !
{
êê" #
message
êê$ +
=
êê, -
ex
êê. 0
.
êê0 1
Message
êê1 8
}
êê9 :
)
êê: ;
;
êê; <
}
ëë 	
}
íí 
}ìì ﬂ
TC:\HeroSystem\walletService\WalletService.Api\Controllers\InvoiceDetailController.cs
	namespace 	
WalletService
 
. 
Api 
. 
Controllers '
;' (
[ 
ApiController 
] 
[ 

ApiVersion 
( 
$str 
) 
] 
[ 
Route 
( 
$str 0
)0 1
]1 2
public		 
class		 #
InvoiceDetailController		 $
:		% &
BaseController		' 5
{

 
private 
readonly !
IInvoiceDetailService *!
_invoiceDetailService+ @
;@ A
public 
#
InvoiceDetailController "
(" #!
IInvoiceDetailService# 8
invoiceService9 G
)G H
{ !
_invoiceDetailService 
= 
invoiceService  .
;. /
} 
[ 
HttpGet 
( 
$str $
)$ %
]% &
public 

async 
Task 
< 
IActionResult #
># $$
GetAllInvoiceDetailAsync% =
(= >
)> ?
{ 
var 
result 
= 
await !
_invoiceDetailService 0
.0 1$
GetAllInvoiceDetailAsync1 I
(I J
)J K
;K L
return 
Ok 
( 
result 
) 
; 
} 
} ú&
SC:\HeroSystem\walletService\WalletService.Api\Controllers\ModelProcessController.cs
	namespace 	
WalletService
 
. 
Api 
. 
Controllers '
;' (
[ 
ApiController 
] 
[ 

ApiVersion 
( 
$str 
) 
] 
[		 
Route		 
(		 
$str		 /
)		/ 0
]		0 1
public

 
class

 "
ModelProcessController

 #
:

$ %
BaseController

& 4
{ 
private 
readonly "
IProcessGradingService +"
_processGradingService2 H
;H I
private 
readonly (
IEcoPoolConfigurationService 1!
_configurationService2 G
;G H
public 
"
ModelProcessController !
(! ""
IProcessGradingService !
processGradingService% :
,: ;(
IEcoPoolConfigurationService $ 
configurationService% 9
)9 :
{ "
_processGradingService 
=  !
processGradingService! 6
;6 7!
_configurationService 
=   
configurationService! 5
;5 6
} 
[ 
HttpPost 
( 
$str %
)% &
]& '
public 

async 
Task 
< 
IActionResult #
># $
ExecuteFirstProcess% 8
(8 9
)9 :
{ 
await "
_processGradingService $
.$ %
ExecuteFirstProcess% 8
(8 9
)9 :
;: ;
return 
Ok 
( 
Success 
( 
$str 
) 
)  
;  !
} 
[   
HttpPost   
(   
$str   &
)  & '
]  ' (
public!! 

async!! 
Task!! 
<!! 
IActionResult!! #
>!!# $ 
ExecuteSecondProcess!!% 9
(!!9 :
)!!: ;
{"" 
await## "
_processGradingService## $
.##$ % 
ExecuteSecondProcess##% 9
(##9 :
)##: ;
;##; <
return%% 
Ok%% 
(%% 
Success%% 
(%% 
$str%% 
)%% 
)%%  
;%%  !
}&& 
[(( 
HttpPost(( 
((( 
$str(( &
)((& '
]((' (
public)) 

async)) 
Task)) 
<)) 
IActionResult)) #
>))# $ 
EcoPoolConfiguration))% 9
())9 :
[)): ;
FromBody)); C
]))C D'
EcoPoolConfigurationRequest))E `
request))a h
)))h i
{** 
await++ !
_configurationService++ #
.++# $.
"CreateOrUpdateEcoPoolConfiguration++$ F
(++F G
request++G N
)++N O
;++O P
return-- 
Ok-- 
(-- 
Success-- 
(-- 
$str-- 
)-- 
)--  
;--  !
}.. 
[00 
HttpGet00 
]00 
public11 

async11 
Task11 
<11 
IActionResult11 #
>11# $#
GetEcoPoolConfiguration11% <
(11< =
)11= >
{22 
var33 
configuration33 
=33 
await33 !!
_configurationService33" 7
.337 8*
GetEcoPoolDefaultConfiguration338 V
(33V W
)33W X
;33X Y
if55 

(55 
configuration55 
is55 
null55 !
)55! "
return66 
NotFound66 
(66 
)66 
;66 
return88 
Ok88 
(88 
configuration88 
)88  
;88  !
}99 
[;; 
HttpGet;; 
(;; 
$str;; :
);;: ;
];;; <
public<< 

async<< 
Task<< 
<<< 
IActionResult<< #
><<# $!
GetProgressPercentage<<% :
(<<: ;
[<<; <
	FromRoute<<< E
]<<E F
int<<G J
configurationId<<K Z
)<<Z [
{== 
var>> 
result>> 
=>> 
await>> !
_configurationService>> 0
.>>0 1!
GetProgressPercentage>>1 F
(>>F G
configurationId>>G V
)>>V W
;>>W X
return@@ 
Ok@@ 
(@@ 
Success@@ 
(@@ 
result@@  
)@@  !
)@@! "
;@@" #
}AA 
}BB ÿ
OC:\HeroSystem\walletService\WalletService.Api\Controllers\PagaditoController.cs
	namespace 	
WalletService
 
. 
Api 
. 
Controllers '
;' (
[

 
ApiController

 
]

 
[ 

ApiVersion 
( 
$str 
) 
] 
[ 
Route 
( 
$str +
)+ ,
], -
public 
class 
PagaditoController 
:  !
BaseController" 0
{ 
private 
readonly 
IPagaditoService %
_pagaditoService& 6
;6 7
public 

PagaditoController 
( 
IPagaditoService .
pagaditoService/ >
)> ?
{ 
_pagaditoService 
= 
pagaditoService *
;* +
} 
[ 
HttpPost 
( 
$str "
)" #
]# $
public 

async 
Task 
< 
IActionResult #
># $
CreateTransaction% 6
(6 7
[7 8
FromBody8 @
]@ A,
 CreatePagaditoTransactionRequestB b
requestc j
)j k
{ 
var 
result 
= 
await 
_pagaditoService +
.+ ,
CreateTransaction, =
(= >
request> E
)E F
;F G
return 
result 
is 
null 
? 
Ok  "
(" #
Fail# '
(' (
$str( /
)/ 0
)0 1
:2 3
Ok4 6
(6 7
Success7 >
(> ?
result? E
)E F
)F G
;G H
} 
[ 
HttpPost 
( 
$str 
) 
] 
public 

async 
Task 
< 
IActionResult #
># $
HandleWebhook% 2
(2 3
)3 4
{ 
var   
headers   
=   
Request   
.   
Headers   %
;  % &
Request"" 
."" 
EnableBuffering"" 
(""  
)""  !
;""! "
string## 
requestBody## 
;## 
using$$ 
($$ 
var$$ 
reader$$ 
=$$ 
new$$ 
StreamReader$$  ,
($$, -
Request$$- 4
.$$4 5
Body$$5 9
,$$9 :
	leaveOpen$$; D
:$$D E
true$$F J
)$$J K
)$$K L
{%% 	
requestBody&& 
=&& 
await&& 
reader&&  &
.&&& '
ReadToEndAsync&&' 5
(&&5 6
)&&6 7
;&&7 8
}'' 	
Request(( 
.(( 
Body(( 
.(( 
Position(( 
=(( 
$num((  !
;((! "
var** 
isSignatureValid** 
=** 
await** $
_pagaditoService**% 5
.**5 6
VerifySignature**6 E
(**E F
headers**F M
,**M N
requestBody**O Z
)**Z [
;**[ \
return,, 
isSignatureValid,, 
?,,  !
Ok,," $
(,,$ %
),,% &
:,,' (

BadRequest,,) 3
(,,3 4
$str,,4 v
),,v w
;,,w x
}-- 
}.. ¡
YC:\HeroSystem\walletService\WalletService.Api\Controllers\PaymentTransactionController.cs
	namespace 	
WalletService
 
. 
Api 
. 
Controllers '
;' (
[ 
ApiController 
] 
[ 

ApiVersion 
( 
$str 
) 
] 
[		 
Route		 
(		 
$str		 5
)		5 6
]		6 7
public

 
class

 (
PaymentTransactionController

 )
:

* +
BaseController

, :
{ 
private 
readonly &
IPaymentTransactionService /&
_paymentTransactionService0 J
;J K
public 
(
PaymentTransactionController '
(' (&
IPaymentTransactionService( B%
paymentTransactionServiceC \
)\ ]
{ &
_paymentTransactionService "
=# $%
paymentTransactionService% >
;> ?
} 
[ 
HttpPost 
] 
public 

async 
Task 
< 
IActionResult #
># $)
CreatePaymentTransactionAsync% B
(B C
[C D
FromBodyD L
]L M%
PaymentTransactionRequestN g
requesth o
)o p
{ 
var 
result 
= 
await &
_paymentTransactionService 5
.5 6)
CreatePaymentTransactionAsync6 S
(S T
requestT [
)[ \
;\ ]
return 
result 
is 
null 
? 
Ok  "
(" #
Fail# '
(' (
$str( H
)H I
)I J
:K L
OkM O
(O P
SuccessP W
(W X
resultX ^
)^ _
)_ `
;` a
} 
[ 
HttpGet 
( 
$str !
)! "
]" #
public 

async 
Task 
< 
IActionResult #
># $
GetAllWireTransfer% 7
(7 8
)8 9
{ 
var 
result 
= 
await &
_paymentTransactionService 5
.5 6
GetAllWireTransfer6 H
(H I
)I J
;J K
return   
Ok   
(   
Success   
(   
result    
)    !
)  ! "
;  " #
}!! 
[## 
HttpPost## 
(## 
$str## 
)## 
]##  
public$$ 

async$$ 
Task$$ 
<$$ 
IActionResult$$ #
>$$# $
ConfirmPayment$$% 3
($$3 4
[$$4 5
FromBody$$5 =
]$$= >,
 ConfirmPaymentTransactionRequest$$? _
request$$` g
)$$g h
{%% 
var&& 
result&& 
=&& 
await&& &
_paymentTransactionService&& 5
.&&5 6
ConfirmPayment&&6 D
(&&D E
request&&E L
)&&L M
;&&M N
return'' 
result'' 
?'' 
Ok'' 
('' 
Success'' "
(''" #
$str''# B
)''B C
)''C D
:''E F
Ok''G I
(''I J
Fail''J N
(''N O
$str''O q
)''q r
)''r s
;''s t
}(( 
},, •
UC:\HeroSystem\walletService\WalletService.Api\Controllers\ResultsEcoPoolController.cs
	namespace 	
WalletService
 
. 
Api 
. 
Controllers '
;' (
[ 
ApiController 
] 
[ 

ApiVersion 
( 
$str 
) 
] 
[ 
Route 
( 
$str 1
)1 2
]2 3
public		 
class		 $
ResultsEcoPoolController		 %
:		& '
BaseController		( 6
{

 
private 
readonly "
IResultsEcoPoolService +"
_resultsEcoPoolService, B
;B C
public 
$
ResultsEcoPoolController #
(# $"
IResultsEcoPoolService$ :!
resultsEcoPoolService; P
)P Q
{ "
_resultsEcoPoolService 
=  !
resultsEcoPoolService! 6
;6 7
} 
[ 
HttpGet 
( 
$str #
)# $
]$ %
public 

async 
Task 
< 
IActionResult #
># $ 
GetAllResultsEcoPool% 9
(9 :
): ;
{ 
var 
result 
= 
await "
_resultsEcoPoolService 1
.1 2%
GetAllResultsEcoPoolAsync2 K
(K L
)L M
;M N
return 
Ok 
( 
Success 
( 
result  
)  !
)! "
;" #
} 
} ©
UC:\HeroSystem\walletService\WalletService.Api\Controllers\UserStatisticsController.cs
	namespace 	
WalletService
 
. 
Api 
. 
Controllers '
;' (
[ 
ApiController 
] 
[		 

ApiVersion		 
(		 
$str		 
)		 
]		 
[

 
Route

 
(

 
$str

 /
)

/ 0
]

0 1
public 
class $
UserStatisticsController %
:& '
BaseController( 6
{ 
private 
readonly "
IUserStatisticsService +"
_userStatisticsService, B
;B C
public 
$
UserStatisticsController #
(# $"
IUserStatisticsService$ :!
userStatisticsService; P
)P Q
{ "
_userStatisticsService 
=  !
userStatisticsService! 6
;6 7
} 
[ 
HttpGet 
( 
$str 
) 
] 
public 

async 
Task 
< 
ServicesResponse &
>& '
GetUserStatistics( 9
(9 :
int: =
userId> D
)D E
{ 
var 
data 
= 
await "
_userStatisticsService /
./ 0"
GetUserStatisticsAsync0 F
(F G
userIdG M
)M N
;N O
return 
Success 
( 
data 
) 
; 
} 
} ˛≠
MC:\HeroSystem\walletService\WalletService.Api\Controllers\WalletController.cs
	namespace 	
WalletService
 
. 
Api 
. 
Controllers '
;' (
[		 
ApiController		 
]		 
[

 

ApiVersion

 
(

 
$str

 
)

 
]

 
[ 
Route 
( 
$str )
)) *
]* +
public 
class 
WalletController 
: 
BaseController  .
{ 
private 
readonly 
IWalletService #
_walletService$ 2
;2 3
public 

WalletController 
( 
IWalletService *
walletService+ 8
)8 9
{ 
_walletService 
= 
walletService &
;& '
} 
[ 
HttpGet 
( 
$str 
) 
] 
public 

async 
Task 
< 
IActionResult #
># $
GetAllWallets% 2
(2 3
)3 4
{ 
var 
result 
= 
await 
_walletService )
.) *
GetAllWallets* 7
(7 8
)8 9
;9 :
return 
Ok 
( 
Success 
( 
result  
)  !
)! "
;" #
} 
[ 
HttpGet 
( 
$str  
)  !
]! "
public   

async   
Task   
<   
IActionResult   #
>  # $
GetWalletsRequest  % 6
(  6 7
[  7 8
	FromQuery  8 A
]  A B
int  C F
userId  G M
)  M N
{!! 
var"" 
result"" 
="" 
await"" 
_walletService"" )
."") *
GetWalletsRequest""* ;
(""; <
userId""< B
)""B C
;""C D
return## 
Ok## 
(## 
Success## 
(## 
result##  
)##  !
)##! "
;##" #
}$$ 
[&& 
HttpGet&& 
(&& 
$str&& 
)&& 
]&& 
public'' 

async'' 
Task'' 
<'' 
IActionResult'' #
>''# $
GetWalletById''% 2
(''2 3
int''3 6
id''7 9
)''9 :
{(( 
var)) 
result)) 
=)) 
await)) 
_walletService)) )
.))) *
GetWalletById))* 7
())7 8
id))8 :
))): ;
;)); <
return** 
result** 
is** 
null** 
?** 
Ok**  "
(**" #
Fail**# '
(**' (
$str**( A
)**A B
)**B C
:**D E
Ok**F H
(**H I
Success**I P
(**P Q
result**Q W
)**W X
)**X Y
;**Y Z
}++ 
[-- 
HttpGet-- 
(-- 
$str-- .
)--. /
]--/ 0
public.. 

async.. 
Task.. 
<.. 
IActionResult.. #
>..# $"
GetWalletByAffiliateId..% ;
(..; <
int..< ?
id..@ B
)..B C
{// 
var00 
result00 
=00 
await00 
_walletService00 )
.00) *"
GetWalletByAffiliateId00* @
(00@ A
id00A C
)00C D
;00D E
return11 
result11 
.11 
IsNullOrEmpty11 #
(11# $
)11$ %
?11& '
Ok11( *
(11* +
Fail11+ /
(11/ 0
$str110 I
)11I J
)11J K
:11L M
Ok11N P
(11P Q
Success11Q X
(11X Y
result11Y _
)11_ `
)11` a
;11a b
}22 
[44 
HttpGet44 
(44 
$str44 )
)44) *
]44* +
public55 

async55 
Task55 
<55 
IActionResult55 #
>55# $
GetWalletByUserId55% 6
(556 7
int557 :
id55; =
)55= >
{66 
var77 
result77 
=77 
await77 
_walletService77 )
.77) *
GetWalletByUserId77* ;
(77; <
id77< >
)77> ?
;77? @
return88 
result88 
.88 
IsNullOrEmpty88 #
(88# $
)88$ %
?88& '
Ok88( *
(88* +
Fail88+ /
(88/ 0
$str880 I
)88I J
)88J K
:88L M
Ok88N P
(88P Q
Success88Q X
(88X Y
result88Y _
)88_ `
)88` a
;88a b
}99 
[;; 

HttpDelete;; 
(;; 
$str;; 
);; 
];; 
public<< 

async<< 
Task<< 
<<< 
IActionResult<< #
><<# $
DeleteWalletAsync<<% 6
(<<6 7
[<<7 8
	FromRoute<<8 A
]<<A B
int<<C F
id<<G I
)<<I J
{== 
var>> 
result>> 
=>> 
await>> 
_walletService>> )
.>>) *
DeleteWalletAsync>>* ;
(>>; <
id>>< >
)>>> ?
;>>? @
return@@ 
result@@ 
is@@ 
null@@ 
?@@ 

BadRequest@@  *
(@@* +
$str@@+ E
)@@E F
:@@G H
Ok@@I K
(@@K L
Success@@L S
(@@S T
result@@T Z
)@@Z [
)@@[ \
;@@\ ]
}AA 
[CC 
HttpGetCC 
(CC 
$strCC :
)CC: ;
]CC; <
publicDD 

asyncDD 
TaskDD 
<DD 
IActionResultDD #
>DD# $.
"GetBalanceInformationByAffiliateIdDD% G
(DDG H
intDDH K
idDDL N
)DDN O
{EE 
varFF 
resultFF 
=FF 
awaitFF 
_walletServiceFF )
.FF) *.
"GetBalanceInformationByAffiliateIdFF* L
(FFL M
idFFM O
)FFO P
;FFP Q
returnGG 
OkGG 
(GG 
SuccessGG 
(GG 
resultGG  
)GG  !
)GG! "
;GG" #
}HH 
[JJ 
HttpGetJJ 
(JJ 
$strJJ )
)JJ) *
]JJ* +
publicKK 

asyncKK 
TaskKK 
<KK 
IActionResultKK #
>KK# $&
GetBalanceInformationAdminKK% ?
(KK? @
)KK@ A
{LL 
varMM 
resultMM 
=MM 
awaitMM 
_walletServiceMM )
.MM) *&
GetBalanceInformationAdminMM* D
(MMD E
)MME F
;MMF G
returnNN 
OkNN 
(NN 
SuccessNN 
(NN 
resultNN  
)NN  !
)NN! "
;NN" #
}OO 
[RR 
HttpPostRR 
(RR 
$strRR  
)RR  !
]RR! "
publicSS 

asyncSS 
TaskSS 
<SS 
IActionResultSS #
>SS# $
PayWithMyBalanceSS% 5
(SS5 6
[SS6 7
FromBodySS7 ?
]SS? @
WalletRequestSSA N
requestSSO V
)SSV W
{TT 
varUU 
responseUU 
=UU 
awaitUU 
_walletServiceUU +
.UU+ ,
PayWithMyBalanceUU, <
(UU< =
requestUU= D
)UUD E
;UUE F
returnWW 
responseWW 
isWW 
falseWW  
?WW! "
OkWW# %
(WW% &
FailWW& *
(WW* +
$strWW+ O
)WWO P
)WWP Q
:WWR S
OkWWT V
(WWV W
SuccessWWW ^
(WW^ _
responseWW_ g
)WWg h
)WWh i
;WWi j
}XX 
[ZZ 
HttpPostZZ 
(ZZ 
$strZZ )
)ZZ) *
]ZZ* +
public[[ 

async[[ 
Task[[ 
<[[ 
IActionResult[[ #
>[[# $%
PayWithMyBalanceForOthers[[% >
([[> ?
[[[? @
FromBody[[@ H
][[H I
WalletRequest[[J W
request[[X _
)[[_ `
{\\ 
var]] 
response]] 
=]] 
await]] 
_walletService]] +
.]]+ ,%
PayWithMyBalanceForOthers]], E
(]]E F
request]]F M
)]]M N
;]]N O
return__ 
response__ 
is__ 
false__  
?__! "
Ok__# %
(__% &
Fail__& *
(__* +
$str__+ O
)__O P
)__P Q
:__R S
Ok__T V
(__V W
Success__W ^
(__^ _
response___ g
)__g h
)__h i
;__i j
}`` 
[bb 
HttpPostbb 
(bb 
$strbb &
)bb& '
]bb' (
publiccc 

asynccc 
Taskcc 
<cc 
IActionResultcc #
>cc# $"
PayWithMyBalanceModel2cc% ;
(cc; <
[cc< =
FromBodycc= E
]ccE F
WalletRequestccG T
requestccU \
)cc\ ]
{dd 
varee 
responseee 
=ee 
awaitee 
_walletServiceee +
.ee+ ,"
PayWithMyBalanceModel2ee, B
(eeB C
requesteeC J
)eeJ K
;eeK L
returngg 
responsegg 
isgg 
falsegg  
?gg! "
Okgg# %
(gg% &
Failgg& *
(gg* +
$strgg+ O
)ggO P
)ggP Q
:ggR S
OkggT V
(ggV W
SuccessggW ^
(gg^ _
responsegg_ g
)ggg h
)ggh i
;ggi j
}hh 
[jj 
HttpPostjj 
(jj 
$strjj *
)jj* +
]jj+ ,
publickk 

asynckk 
Taskkk 
<kk 
IActionResultkk #
>kk# $&
PayMembershipWithMyBalancekk% ?
(kk? @
[kk@ A
FromBodykkA I
]kkI J
WalletRequestkkK X
requestkkY `
)kk` a
{ll 
varmm 
responsemm 
=mm 
awaitmm 
_walletServicemm +
.mm+ ,&
PayMembershipWithMyBalancemm, F
(mmF G
requestmmG N
)mmN O
;mmO P
returnoo 
responseoo 
isoo 
falseoo  
?oo! "
Okoo# %
(oo% &
Failoo& *
(oo* +
$stroo+ O
)ooO P
)ooP Q
:ooR S
OkooT V
(ooV W
SuccessooW ^
(oo^ _
responseoo_ g
)oog h
)ooh i
;ooi j
}pp 
[rr 
HttpPostrr 
(rr 
$strrr %
)rr% &
]rr& '
publicss 

asyncss 
Taskss 
<ss 
IActionResultss #
>ss# $!
PayWithMyBalanceAdminss% :
(ss: ;
[ss; <
FromBodyss< D
]ssD E
WalletRequestssF S
requestssT [
)ss[ \
{tt 
varuu 
responseuu 
=uu 
awaituu 
_walletServiceuu +
.uu+ ,
AdminPaymentHandleruu, ?
(uu? @
requestuu@ G
)uuG H
;uuH I
returnww 
responseww 
isww 
falseww  
?ww! "
Okww# %
(ww% &
Failww& *
(ww* +
$strww+ O
)wwO P
)wwP Q
:wwR S
OkwwT V
(wwV W
SuccesswwW ^
(ww^ _
responseww_ g
)wwg h
)wwh i
;wwi j
}xx 
[zz 
HttpPutzz 
(zz 
$strzz 
)zz  
]zz  !
public{{ 

async{{ 
Task{{ 
<{{ 
IActionResult{{ #
>{{# $"
PayWithMyBalanceMobile{{% ;
({{; <
[{{< =
FromBody{{= E
]{{E F
WalletRequest{{G T
request{{U \
){{\ ]
{|| 
var}} 
response}} 
=}} 
await}} 
_walletService}} +
.}}+ ,
PayWithMyBalance}}, <
(}}< =
request}}= D
)}}D E
;}}E F
return 
response 
is 
false  
?! "
Ok# %
(% &
Fail& *
(* +
$str+ O
)O P
)P Q
:R S
OkT V
(V W
SuccessW ^
(^ _
response_ g
)g h
)h i
;i j
}
ÄÄ 
[
ÇÇ 
HttpPost
ÇÇ 
(
ÇÇ 
$str
ÇÇ /
)
ÇÇ/ 0
]
ÇÇ0 1
public
ÉÉ 

async
ÉÉ 
Task
ÉÉ 
<
ÉÉ 
IActionResult
ÉÉ #
>
ÉÉ# $-
TransferBalanceForNewAffiliates
ÉÉ% D
(
ÉÉD E
[
ÉÉE F
FromBody
ÉÉF N
]
ÉÉN O$
TransferBalanceRequest
ÉÉP f
request
ÉÉg n
)
ÉÉn o
{
ÑÑ 
var
ÖÖ 
response
ÖÖ 
=
ÖÖ 
await
ÖÖ 
_walletService
ÖÖ +
.
ÖÖ+ ,,
TransferBalanceForNewAffiliate
ÖÖ, J
(
ÖÖJ K
request
ÖÖK R
)
ÖÖR S
;
ÖÖS T
return
áá 
Ok
áá 
(
áá 
response
áá 
)
áá 
;
áá 
}
àà 
[
ää 
HttpPut
ää 
(
ää 
$str
ää 4
)
ää4 5
]
ää5 6
public
ãã 

async
ãã 
Task
ãã 
<
ãã 
IActionResult
ãã #
>
ãã# $3
%TransferBalanceForNewAffiliatesMobile
ãã% J
(
ããJ K
[
ããK L
FromBody
ããL T
]
ããT U$
TransferBalanceRequest
ããV l
request
ããm t
)
ããt u
{
åå 
var
çç 
response
çç 
=
çç 
await
çç 
_walletService
çç +
.
çç+ ,,
TransferBalanceForNewAffiliate
çç, J
(
ççJ K
request
ççK R
)
ççR S
;
ççS T
return
èè 
Ok
èè 
(
èè 
response
èè 
)
èè 
;
èè 
}
êê 
[
íí 
HttpPost
íí 
(
íí 
$str
íí 
)
íí  
]
íí  !
public
ìì 

async
ìì 
Task
ìì 
<
ìì 
IActionResult
ìì #
>
ìì# $
TransferBalance
ìì% 4
(
ìì4 5
[
ìì5 6
FromBody
ìì6 >
]
ìì> ?
string
ìì@ F
	encrypted
ììG P
)
ììP Q
{
îî 
var
ïï 
response
ïï 
=
ïï 
await
ïï 
_walletService
ïï +
.
ïï+ ,
TransferBalance
ïï, ;
(
ïï; <
	encrypted
ïï< E
)
ïïE F
;
ïïF G
return
óó 
Ok
óó 
(
óó 
response
óó 
)
óó 
;
óó 
}
òò 
[
õõ 
HttpPost
õõ 
(
õõ 
$str
õõ 4
)
õõ4 5
]
õõ5 6
public
úú 

async
úú 
Task
úú 
<
úú 
IActionResult
úú #
>
úú# $2
$RejectOrCancelRevertDebitTransaction
úú% I
(
úúI J
[
úúJ K
	FromQuery
úúK T
]
úúT U
int
úúV Y
option
úúZ `
,
úú` a
[
úúb c
FromBody
úúc k
]
úúk l
int
úúm p
id
úúq s
)
úús t
{
ùù 
var
ûû 
response
ûû 
=
ûû 
await
ûû 
_walletService
ûû +
.
ûû+ ,7
)HandleWalletRequestRevertTransactionAsync
ûû, U
(
ûûU V
option
ûûV \
,
ûû\ ]
id
ûû^ `
)
ûû` a
;
ûûa b
return
†† 
response
†† 
is
†† 
false
††  
?
††! "
Ok
††# %
(
††% &
Fail
††& *
(
††* +
$str
††+ Z
)
††Z [
)
††[ \
:
††] ^
Ok
††_ a
(
††a b
Success
††b i
(
††i j
response
††j r
)
††r s
)
††s t
;
††t u
}
°° 
[
££ 
HttpGet
££ 
(
££ 
$str
££ 3
)
££3 4
]
££4 5
public
§§ 

async
§§ 
Task
§§ 
<
§§ 
IActionResult
§§ #
>
§§# $)
GetPurchasesMadeInMyNetwork
§§% @
(
§§@ A
[
§§A B
	FromRoute
§§B K
]
§§K L
int
§§M P
id
§§Q S
)
§§S T
{
•• 
var
¶¶ 
result
¶¶ 
=
¶¶ 
await
¶¶ 
_walletService
¶¶ )
.
¶¶) *)
GetPurchasesMadeInMyNetwork
¶¶* E
(
¶¶E F
id
¶¶F H
)
¶¶H I
;
¶¶I J
if
®® 

(
®® 
result
®® 
==
®® 
null
®® 
)
®® 
{
©© 	
return
™™ 
Ok
™™ 
(
™™ 
Fail
™™ 
(
™™ 
$str
™™ >
)
™™> ?
)
™™? @
;
™™@ A
}
´´ 	
else
¨¨ 
{
≠≠ 	
var
ÆÆ 
response
ÆÆ 
=
ÆÆ 
new
ÆÆ 
{
ØØ 
result
∞∞ 
.
∞∞ 
Value
∞∞ 
.
∞∞ "
CurrentYearPurchases
∞∞ 1
,
∞∞1 2
result
±± 
.
±± 
Value
±± 
.
±± #
PreviousYearPurchases
±± 2
}
≤≤ 
;
≤≤ 
return
¥¥ 
Ok
¥¥ 
(
¥¥ 
Success
¥¥ 
(
¥¥ 
response
¥¥ &
)
¥¥& '
)
¥¥' (
;
¥¥( )
}
µµ 	
}
∂∂ 
[
∏∏ 
HttpGet
∏∏ 
(
∏∏ 
$str
∏∏ 2
)
∏∏2 3
]
∏∏3 4
public
ππ 

async
ππ 
Task
ππ 
<
ππ 
IActionResult
ππ #
>
ππ# $1
#GetAllAffiliatesWithPositiveBalance
ππ% H
(
ππH I
)
ππI J
{
∫∫ 
var
ªª 
result
ªª 
=
ªª 
await
ªª 
_walletService
ªª )
.
ªª) *1
#GetAllAffiliatesWithPositiveBalance
ªª* M
(
ªªM N
)
ªªN O
;
ªªO P
return
ºº 
Ok
ºº 
(
ºº 
Success
ºº 
(
ºº 
result
ºº  
)
ºº  !
)
ºº! "
;
ºº" #
}
ΩΩ 
[
øø 
HttpPost
øø 
(
øø 
$str
øø !
)
øø! "
]
øø" #
public
¿¿ 

async
¿¿ 
Task
¿¿ 
<
¿¿ 
IActionResult
¿¿ #
>
¿¿# $
CreateCreditAdmin
¿¿% 6
(
¿¿6 7+
CreditTransactionAdminRequest
¿¿7 T
request
¿¿U \
)
¿¿\ ]
{
¡¡ 
var
¬¬ 
response
¬¬ 
=
¬¬ 
await
¬¬ 
_walletService
¬¬ +
.
¬¬+ , 
CreateBalanceAdmin
¬¬, >
(
¬¬> ?
request
¬¬? F
)
¬¬F G
;
¬¬G H
return
ƒƒ 
response
ƒƒ 
is
ƒƒ 
false
ƒƒ !
?
ƒƒ" #
Ok
ƒƒ$ &
(
ƒƒ& '
Fail
ƒƒ' +
(
ƒƒ+ ,
$str
ƒƒ, P
)
ƒƒP Q
)
ƒƒQ R
:
ƒƒS T
Ok
ƒƒU W
(
ƒƒW X
Success
ƒƒX _
(
ƒƒ_ `
response
ƒƒ` h
)
ƒƒh i
)
ƒƒi j
;
ƒƒj k
}
≈≈ 
[
«« 
HttpPost
«« 
(
«« 
$str
«« '
)
««' (
]
««( )
public
»» 

async
»» 
Task
»» 
<
»» 
IActionResult
»» #
>
»»# $%
PayWithMyBalanceCourses
»»% <
(
»»< =
[
»»= >
FromBody
»»> F
]
»»F G
WalletRequest
»»H U
request
»»V ]
)
»»] ^
{
…… 
var
   
response
   
=
   
await
   
_walletService
   +
.
  + ,"
CoursePaymentHandler
  , @
(
  @ A
request
  A H
)
  H I
;
  I J
return
ÃÃ 
response
ÃÃ 
is
ÃÃ 
false
ÃÃ  
?
ÃÃ! "
Ok
ÃÃ# %
(
ÃÃ% &
Fail
ÃÃ& *
(
ÃÃ* +
$str
ÃÃ+ O
)
ÃÃO P
)
ÃÃP Q
:
ÃÃR S
Ok
ÃÃT V
(
ÃÃV W
Success
ÃÃW ^
(
ÃÃ^ _
response
ÃÃ_ g
)
ÃÃg h
)
ÃÃh i
;
ÃÃi j
}
ÕÕ 
[
œœ 
HttpPost
œœ 
(
œœ 
$str
œœ 
)
œœ 
]
œœ 
public
–– 

async
–– 
Task
–– 
<
–– 
IActionResult
–– #
>
––# $

RemoveKeys
––% /
(
––/ 0
[
––0 1
FromBody
––1 9
]
––9 :
DeleteKeysRequest
––; L
request
––M T
)
––T U
{
—— 
await
““ 
_walletService
““ 
.
““ 

RemoveKeys
““ '
(
““' (
request
““( /
)
““/ 0
;
““0 1
return
‘‘ 
Ok
‘‘ 
(
‘‘ 
Success
‘‘ 
(
‘‘ 
$str
‘‘ 
)
‘‘ 
)
‘‘  
;
‘‘  !
}
’’ 
}ÿÿ °,
TC:\HeroSystem\walletService\WalletService.Api\Controllers\WalletHistoryController.cs
	namespace 	
WalletService
 
. 
Api 
. 
Controllers '
;' (
[ 
ApiController 
] 
[		 

ApiVersion		 
(		 
$str		 
)		 
]		 
[

 
Route

 
(

 
$str

 0
)

0 1
]

1 2
public 
class #
WalletHistoryController $
:% &
BaseController' 5
{ 
private 
readonly !
IWalletHistoryService *!
_walletHistoryService+ @
;@ A
public 
#
WalletHistoryController "
(" #!
IWalletHistoryService# 8 
walletHistoryService9 M
)M N
{ !
_walletHistoryService 
=  
walletHistoryService  4
;4 5
} 
[ 
HttpGet 
( 
$str %
)% &
]& '
public 

async 
Task 
< 
IActionResult #
># $'
GetAllWalletsHistoriesAsync% @
(@ A
)A B
{ 
var 
result 
= 
await !
_walletHistoryService 0
.0 1'
GetAllWalletsHistoriesAsync1 L
(L M
)M N
;N O
return 
Ok 
( 
Success 
( 
result  
)  !
)! "
;" #
} 
[ 
HttpGet 
( 
$str 
) 
] 
public 

async 
Task 
< 
IActionResult #
># $'
GetWalletHistoriesByIdAsync% @
(@ A
intA D
idE G
)G H
{ 
var 
result 
= 
await !
_walletHistoryService 0
.0 1'
GetWalletHistoriesByIdAsync1 L
(L M
idM O
)O P
;P Q
return 
result 
is 
null 
? 
Ok  "
(" #
Fail# '
(' (
$str( I
)I J
)J K
:L M
OkN P
(P Q
SuccessQ X
(X Y
resultY _
)_ `
)` a
;a b
}   
["" 
HttpPost"" 
]"" 
public## 

async## 
Task## 
<## 
IActionResult## #
>### $&
CreateWalletHistoriesAsync##% ?
(##? @
[##@ A
FromBody##A I
]##I J 
WalletHistoryRequest##K _
request##` g
)##g h
{$$ 
var%% 
result%% 
=%% 
await%% !
_walletHistoryService%% 0
.%%0 1&
CreateWalletHistoriesAsync%%1 K
(%%K L
request%%L S
)%%S T
;%%T U
return'' 
result'' 
is'' 
null'' 
?'' 
Ok''  "
(''" #
Fail''# '
(''' (
$str''( K
)''K L
)''L M
:''N O
Ok''P R
(''R S
Success''S Z
(''Z [
result''[ a
)''a b
)''b c
;''c d
}(( 
[** 
HttpPut** 
(** 
$str** 
)** 
]** 
public++ 

async++ 
Task++ 
<++ 
IActionResult++ #
>++# $&
UpdateWalletHistoriesAsync++% ?
(++? @
[++@ A
	FromRoute++A J
]++J K
int++L O
id++P R
,++R S
[++T U
FromBody++U ]
]++] ^ 
WalletHistoryRequest++_ s
request++t {
)++{ |
{,, 
var-- 
result-- 
=-- 
await-- !
_walletHistoryService-- 0
.--0 1&
UpdateWalletHistoriesAsync--1 K
(--K L
id--L N
,--N O
request--P W
)--W X
;--X Y
return// 
result// 
is// 
null// 
?// 
Ok//  "
(//" #
Fail//# '
(//' (
$str//( K
)//K L
)//L M
://N O
Ok//P R
(//R S
Success//S Z
(//Z [
result//[ a
)//a b
)//b c
;//c d
}00 
[22 

HttpDelete22 
(22 
$str22 
)22 
]22 
public33 

async33 
Task33 
<33 
IActionResult33 #
>33# $&
DeleteWalletHistoriesAsync33% ?
(33? @
[33@ A
	FromRoute33A J
]33J K
int33L O
id33P R
)33R S
{44 
var55 
result55 
=55 
await55 !
_walletHistoryService55 0
.550 1&
DeleteWalletHistoriesAsync551 K
(55K L
id55L N
)55N O
;55O P
return77 
result77 
is77 
null77 
?77 
Ok77  "
(77" #
Fail77# '
(77' (
$str77( K
)77K L
)77L M
:77N O
Ok77P R
(77R S
Success77S Z
(77Z [
result77[ a
)77a b
)77b c
;77c d
}88 
};; ô%
TC:\HeroSystem\walletService\WalletService.Api\Controllers\WalletModel1AController.cs
	namespace 	
WalletService
 
. 
Api 
. 
Controllers '
;' (
[ 
ApiController 
] 
[ 

ApiVersion 
( 
$str 
) 
] 
[		 
Route		 
(		 
$str		 /
)		/ 0
]		0 1
public

 
class

 #
WalletModel1AController

 $
:

% &
BaseController

' 5
{ 
private 
readonly !
IWalletModel1AService *!
_walletModel1AService+ @
;@ A
public 
#
WalletModel1AController "
(" #!
IWalletModel1AService# 8 
walletModel1AService9 M
)M N
{ !
_walletModel1AService 
=  
walletModel1AService  4
;4 5
} 
[ 
HttpGet 
( 
$str ?
)? @
]@ A
public 

async 
Task 
< 
IActionResult #
># $.
"GetBalanceInformationByAffiliateId% G
(G H
intH K
affiliateIdL W
)W X
{ 
var 
response 
= 
await !
_walletModel1AService 2
.2 3.
"GetBalanceInformationByAffiliateId3 U
(U V
affiliateIdV a
)a b
;b c
return 
Ok 
( 
Success 
( 
response "
)" #
)# $
;$ %
} 
[ 
HttpPost 
( 
$str "
)" #
]# $
public 

async 
Task 
< 
IActionResult #
># $
PayWithMyBalance% 5
(5 6
[6 7
FromBody7 ?
]? @
WalletRequestA N
requestO V
)V W
{ 
var 
response 
= 
await !
_walletModel1AService 2
.2 3
PayWithMyBalance3 C
(C D
requestD K
)K L
;L M
return   
response   
is   
false    
?  ! "
Ok  # %
(  % &
Fail  & *
(  * +
$str  + O
)  O P
)  P Q
:  R S
Ok  T V
(  V W
Success  W ^
(  ^ _
response  _ g
)  g h
)  h i
;  i j
}!! 
[## 
HttpPost## 
(## 
$str## '
)##' (
]##( )
public$$ 

async$$ 
Task$$ 
<$$ 
IActionResult$$ #
>$$# $#
PayWithMyServiceBalance$$% <
($$< =
[$$= >
FromBody$$> F
]$$F G
WalletRequest$$H U
request$$V ]
)$$] ^
{%% 
var&& 
response&& 
=&& 
await&& !
_walletModel1AService&& 2
.&&2 3#
PayWithMyServiceBalance&&3 J
(&&J K
request&&K R
)&&R S
;&&S T
return(( 
response(( 
is(( 
false((  
?((! "
Ok((# %
(((% &
Fail((& *
(((* +
$str((+ O
)((O P
)((P Q
:((R S
Ok((T V
(((V W
Success((W ^
(((^ _
response((_ g
)((g h
)((h i
;((i j
})) 
[++ 
HttpPost++ 
(++ 
$str++ )
)++) *
]++* +
public,, 

async,, 
Task,, 
<,, 
IActionResult,, #
>,,# $%
CreateServiceBalanceAdmin,,% >
(,,> ?
[,,? @
FromBody,,@ H
],,H I)
CreditTransactionAdminRequest,,J g
request,,h o
),,o p
{-- 
var.. 
response.. 
=.. 
await.. !
_walletModel1AService.. 2
...2 3%
CreateServiceBalanceAdmin..3 L
(..L M
request..M T
)..T U
;..U V
return00 
response00 
is00 
false00  
?00! "
Ok00# %
(00% &
Fail00& *
(00* +
$str00+ O
)00O P
)00P Q
:00R S
Ok00T V
(00V W
Success00W ^
(00^ _
response00_ g
)00g h
)00h i
;00i j
}11 
}22 ô%
TC:\HeroSystem\walletService\WalletService.Api\Controllers\WalletModel1BController.cs
	namespace 	
WalletService
 
. 
Api 
. 
Controllers '
;' (
[ 
ApiController 
] 
[		 

ApiVersion		 
(		 
$str		 
)		 
]		 
[

 
Route

 
(

 
$str

 /
)

/ 0
]

0 1
public 
class #
WalletModel1BController $
:% &
BaseController' 5
{ 
private 
readonly !
IWalletModel1BService *!
_walletModel1BService+ @
;@ A
public 
#
WalletModel1BController "
(" #!
IWalletModel1BService# 8 
walletModel1BService9 M
)M N
{ !
_walletModel1BService 
=  
walletModel1BService  4
;4 5
} 
[ 
HttpGet 
( 
$str ?
)? @
]@ A
public 

async 
Task 
< 
IActionResult #
># $.
"GetBalanceInformationByAffiliateId% G
(G H
intH K
affiliateIdL W
)W X
{ 
var 
response 
= 
await !
_walletModel1BService 2
. .
"GetBalanceInformationByAffiliateId /
(/ 0
affiliateId0 ;
); <
;< =
return 
Ok 
( 
Success 
( 
response "
)" #
)# $
;$ %
} 
[ 
HttpPost 
( 
$str "
)" #
]# $
public 

async 
Task 
< 
IActionResult #
># $
PayWithMyBalance% 5
(5 6
[6 7
FromBody7 ?
]? @
WalletRequestA N
requestO V
)V W
{ 
var   
response   
=   
await   !
_walletModel1BService   2
.  2 3
PayWithMyBalance  3 C
(  C D
request  D K
)  K L
;  L M
return"" 
response"" 
is"" 
false""  
?""! "
Ok""# %
(""% &
Fail""& *
(""* +
$str""+ O
)""O P
)""P Q
:""R S
Ok""T V
(""V W
Success""W ^
(""^ _
response""_ g
)""g h
)""h i
;""i j
}## 
[%% 
HttpPost%% 
(%% 
$str%% '
)%%' (
]%%( )
public&& 

async&& 
Task&& 
<&& 
IActionResult&& #
>&&# $#
PayWithMyServiceBalance&&% <
(&&< =
[&&= >
FromBody&&> F
]&&F G
WalletRequest&&H U
request&&V ]
)&&] ^
{'' 
var(( 
response(( 
=(( 
await(( !
_walletModel1BService(( 2
.((2 3#
PayWithMyServiceBalance((3 J
(((J K
request((K R
)((R S
;((S T
return** 
response** 
is** 
false**  
?**! "
Ok**# %
(**% &
Fail**& *
(*** +
$str**+ O
)**O P
)**P Q
:**R S
Ok**T V
(**V W
Success**W ^
(**^ _
response**_ g
)**g h
)**h i
;**i j
}++ 
[-- 
HttpPost-- 
(-- 
$str-- )
)--) *
]--* +
public.. 

async.. 
Task.. 
<.. 
IActionResult.. #
>..# $%
CreateServiceBalanceAdmin..% >
(..> ?
[..? @
FromBody..@ H
]..H I)
CreditTransactionAdminRequest..J g
request..h o
)..o p
{// 
var00 
response00 
=00 
await00 !
_walletModel1BService00 2
.002 3%
CreateServiceBalanceAdmin003 L
(00L M
request00M T
)00T U
;00U V
return22 
response22 
is22 
false22  
?22! "
Ok22# %
(22% &
Fail22& *
(22* +
$str22+ O
)22O P
)22P Q
:22R S
Ok22T V
(22V W
Success22W ^
(22^ _
response22_ g
)22g h
)22h i
;22i j
}33 
}44 ¶!
SC:\HeroSystem\walletService\WalletService.Api\Controllers\WalletPeriodController.cs
	namespace 	
WalletService
 
. 
Api 
. 
Controllers '
;' (
[ 
ApiController 
] 
[ 

ApiVersion 
( 
$str 
) 
] 
[		 
Route		 
(		 
$str		 /
)		/ 0
]		0 1
public

 
class

 "
WalletPeriodController

 #
:

$ %
BaseController

& 4
{ 
private 
readonly  
IWalletPeriodService ) 
_walletPeriodService* >
;> ?
public 
"
WalletPeriodController !
(! " 
IWalletPeriodService" 6
walletPeriodService7 J
)J K
{  
_walletPeriodService 
= 
walletPeriodService 2
;2 3
} 
[ 
HttpGet 
( 
$str #
)# $
]$ %
public 

async 
Task 
< 
IActionResult #
># $ 
GetAllWalletsPeriods% 9
(9 :
): ;
{ 
var 
result 
= 
await  
_walletPeriodService /
./ 0 
GetAllWalletsPeriods0 D
(D E
)E F
;F G
return 
Ok 
( 
Success 
( 
result  
)  !
)! "
;" #
} 
[ 
HttpGet 
( 
$str 
) 
] 
public 

async 
Task 
< 
IActionResult #
># $
GetWalletPeriodById% 8
(8 9
int9 <
id= ?
)? @
{ 
var 
result 
= 
await  
_walletPeriodService /
./ 0
GetWalletPeriodById0 C
(C D
idD F
)F G
;G H
return 
result 
is 
null 
? 
Ok  "
(" #
Fail# '
(' (
$str( H
)H I
)I J
:K L
OkM O
(O P
SuccessP W
(W X
resultX ^
)^ _
)_ `
;` a
}   
["" 
HttpPost"" 
]"" 
public## 

async## 
Task## 
<## 
IActionResult## #
>### $#
CreateWalletPeriodAsync##% <
(##< =
[##= >
FromBody##> F
]##F G
WalletPeriodRequest##H [
[##[ \
]##\ ]
request##^ e
)##e f
{$$ 
await%%  
_walletPeriodService%% "
.%%" ##
CreateWalletPeriodAsync%%# :
(%%: ;
request%%; B
)%%B C
;%%C D
return'' 
Ok'' 
('' 
Success'' 
('' 
$str'' ?
)''? @
)''@ A
;''A B
}(( 
[++ 

HttpDelete++ 
(++ 
$str++ 
)++ 
]++ 
public,, 

async,, 
Task,, 
<,, 
IActionResult,, #
>,,# $#
DeleteWalletPeriodAsync,,% <
(,,< =
[,,= >
	FromRoute,,> G
],,G H
int,,I L
id,,M O
),,O P
{-- 
var.. 
result.. 
=.. 
await..  
_walletPeriodService.. /
.../ 0#
DeleteWalletPeriodAsync..0 G
(..G H
id..H J
)..J K
;..K L
return00 
result00 
is00 
null00 
?00 
Ok00  "
(00" #
Fail00# '
(00' (
$str00( J
)00J K
)00K L
:00M N
Ok00O Q
(00Q R
Success00R Y
(00Y Z
result00Z `
)00` a
)00a b
;00b c
}11 
}44 §C
TC:\HeroSystem\walletService\WalletService.Api\Controllers\WalletRequestController.cs
	namespace 	
WalletService
 
. 
Api 
. 
Controllers '
;' (
[ 
ApiController 
] 
[		 

ApiVersion		 
(		 
$str		 
)		 
]		 
[

 
Route

 
(

 
$str

 0
)

0 1
]

1 2
public 
class #
WalletRequestController $
:% &
BaseController' 5
{ 
private 
readonly !
IWalletRequestService *!
_walletRequestService+ @
;@ A
public 
#
WalletRequestController "
(" #!
IWalletRequestService# 8 
walletRequestService9 M
)M N
{ !
_walletRequestService 
=  
walletRequestService  4
;4 5
} 
[ 
HttpGet 
( 
$str $
)$ %
]% &
public 

async 
Task 
< 
IActionResult #
># $!
GetAllWalletsRequests% :
(: ;
); <
{ 
var 
result 
= 
await !
_walletRequestService 0
.0 1!
GetAllWalletsRequests1 F
(F G
)G H
;H I
return 
Ok 
( 
Success 
( 
result  
)  !
)! "
;" #
} 
[ 
HttpGet 
( 
$str 
) 
] 
public 

async 
Task 
< 
IActionResult #
># $,
 GetAllWalletRequestByAffiliateId% E
(E F
intF I
idJ L
)L M
{ 
var 
result 
= 
await !
_walletRequestService 0
.0 1 
GetWalletRequestById1 E
(E F
idF H
)H I
;I J
return 
result 
is 
null 
? 
Ok  "
(" #
Fail# '
(' (
$str( I
)I J
)J K
:L M
OkN P
(P Q
SuccessQ X
(X Y
resultY _
)_ `
)` a
;a b
}   
["" 
HttpPost"" 
]"" 
public## 

async## 
Task## 
<## 
IActionResult## #
>### $$
CreateWalletRequestAsync##% =
(##= >
[##> ?
FromBody##? G
]##G H 
WalletRequestRequest##I ]
request##^ e
)##e f
{$$ 
var%% 
result%% 
=%% 
await%% !
_walletRequestService%% 0
.%%0 1$
CreateWalletRequestAsync%%1 I
(%%I J
request%%J Q
)%%Q R
;%%R S
return'' 
result'' 
is'' 
null'' 
?'' 
Ok''  "
(''" #
Fail''# '
(''' (
$str''( K
)''K L
)''L M
:''N O
Ok''P R
(''R S
Success''S Z
(''Z [
result''[ a
)''a b
)''b c
;''c d
}(( 
[** 
HttpPost** 
(** 
$str** 
)** 
]** 
public++ 

async++ 
Task++ 
<++ 
IActionResult++ #
>++# $ 
ProcessOptionRequest++% 9
(++9 :
[++: ;
	FromQuery++; D
]++D E
int++F I
option++J P
,++P Q
[++R S
FromBody++S [
]++[ \
List++] a
<++a b
long++b f
>++f g
ids++h k
)++k l
{,, 
var-- 
result-- 
=-- 
await-- !
_walletRequestService-- 0
.--0 1
ProcessOption--1 >
(--> ?
option--? E
,--E F
ids--G J
)--J K
;--K L
return// 
result// 
is// 
null// 
?// 
Ok//  "
(//" #
Fail//# '
(//' (
$str//( K
)//K L
)//L M
://N O
Ok//P R
(//R S
Success//S Z
(//Z [
result//[ a
)//a b
)//b c
;//c d
}00 
[22 
HttpPost22 
(22 
$str22 )
)22) *
]22* +
public33 

async33 
Task33 
<33 
IActionResult33 #
>33# $%
CreateWalletRequestRevert33% >
(33> ?
[33? @
FromBody33@ H
]33H I*
WalletRequestRevertTransaction33J h
request33i p
)33p q
{44 
var55 
result55 
=55 
await55 !
_walletRequestService55 0
.550 1%
CreateWalletRequestRevert551 J
(55J K
request55K R
)55R S
;55S T
return77 
result77 
is77 
null77 
?77 
Ok77  "
(77" #
Fail77# '
(77' (
$str77( J
)77J K
)77K L
:77M N
Ok77O Q
(77Q R
Success77R Y
(77Y Z
result77Z `
)77` a
)77a b
;77b c
}88 
[:: 
HttpPut:: 
(:: 
$str:: (
)::( )
]::) *
public;; 

async;; 
Task;; 
<;; 
IActionResult;; #
>;;# $+
CreateWalletRequestRevertMobile;;% D
(;;D E
[;;E F
FromBody;;F N
];;N O*
WalletRequestRevertTransaction;;P n
request;;o v
);;v w
{<< 
var== 
result== 
=== 
await== !
_walletRequestService== 0
.==0 1%
CreateWalletRequestRevert==1 J
(==J K
request==K R
)==R S
;==S T
return?? 
result?? 
is?? 
null?? 
??? 
Ok??  "
(??" #
Fail??# '
(??' (
$str??( J
)??J K
)??K L
:??M N
Ok??O Q
(??Q R
Success??R Y
(??Y Z
result??Z `
)??` a
)??a b
;??b c
}AA 
[CC 
HttpGetCC 
(CC 
$strCC 3
)CC3 4
]CC4 5
publicDD 

asyncDD 
TaskDD 
<DD 
IActionResultDD #
>DD# $0
$GetAllWalletRequestRevertTransactionDD% I
(DDI J
)DDJ K
{EE 
varFF 
resultFF 
=FF 
awaitFF !
_walletRequestServiceFF 0
.FF0 10
$GetAllWalletRequestRevertTransactionFF1 U
(FFU V
)FFV W
;FFW X
returnHH 
resultHH 
isHH 
nullHH 
?HH 
OkHH  "
(HH" #
FailHH# '
(HH' (
$strHH( \
)HH\ ]
)HH] ^
:HH_ `
OkHHa c
(HHc d
SuccessHHd k
(HHk l
resultHHl r
)HHr s
)HHs t
;HHt u
}II 
[KK 
HttpPostKK 
(KK 
$strKK *
)KK* +
]KK+ ,
publicLL 

asyncLL 
TaskLL 
<LL 
IActionResultLL #
>LL# $&
AdministrativePaymentAsyncLL% ?
(LL? @
[LL@ A
FromBodyLLA I
]LLI J
WalletsRequestLLK Y
[LLY Z
]LLZ [
requestsLL\ d
)LLd e
{MM 
varNN 
resultNN 
=NN 
awaitNN !
_walletRequestServiceNN 0
.NN0 1&
AdministrativePaymentAsyncNN1 K
(NNK L
requestsNNL T
)NNT U
;NNU V
returnPP 
resultPP 
isPP 
falsePP 
?PP  
OkPP! #
(PP# $
FailPP$ (
(PP( )
$strPP) T
)PPT U
)PPU V
:PPW X
OkPPY [
(PP[ \
SuccessPP\ c
(PPc d
resultPPd j
)PPj k
)PPk l
;PPl m
}RR 
}UU †#
\C:\HeroSystem\walletService\WalletService.Api\Controllers\WalletRetentionConfigController.cs
	namespace 	
WalletService
 
. 
Api 
. 
Controllers '
;' (
[ 
ApiController 
] 
[		 

ApiVersion		 
(		 
$str		 
)		 
]		 
[

 
Route

 
(

 
$str

 8
)

8 9
]

9 :
public 
class +
WalletRetentionConfigController ,
:- .
BaseController/ =
{ 
private 
readonly )
IWalletRetentionConfigService 2)
_walletRetentionConfigService3 P
;P Q
public 
+
WalletRetentionConfigController *
(* +)
IWalletRetentionConfigService+ H(
walletRetentionConfigServiceI e
)e f
{ )
_walletRetentionConfigService %
=& '(
walletRetentionConfigService( D
;D E
} 
[ 
HttpGet 
( 
$str +
)+ ,
], -
public 

async 
Task 
< 
IActionResult #
># $(
GetAllWalletsRetentionConfig% A
(A B
)B C
{ 
var 
result 
= 
await )
_walletRetentionConfigService 8
.8 9(
GetAllWalletsRetentionConfig9 U
(U V
)V W
;W X
return 
Ok 
( 
Success 
( 
result  
)  !
)! "
;" #
} 
[ 
HttpGet 
( 
$str 
) 
] 
public 

async 
Task 
< 
IActionResult #
># $(
GetWalletRetentionConfigById% A
(A B
intB E
idF H
)H I
{ 
var 
result 
= 
await )
_walletRetentionConfigService 8
.8 9(
GetWalletRetentionConfigById9 U
(U V
idV X
)X Y
;Y Z
return   
result   
is   
null   
?   
Ok    "
(  " #
Fail  # '
(  ' (
$str  ( K
)  K L
)  L M
:  N O
Ok  P R
(  R S
Success  S Z
(  Z [
result  [ a
)  a b
)  b c
;  c d
}!! 
[## 
HttpPost## 
]## 
public$$ 

async$$ 
Task$$ 
<$$ 
IActionResult$$ #
>$$# $,
 CreateWalletRetentionConfigAsync$$% E
($$E F
[$$F G
FromBody$$G O
]$$O P(
WalletRetentionConfigRequest$$Q m
[$$m n
]$$n o
request$$p w
)$$w x
{%% 
var&& 
result&& 
=&& 
await&& )
_walletRetentionConfigService&& 8
.&&8 9,
 CreateWalletRetentionConfigAsync&&9 Y
(&&Y Z
request&&Z a
)&&a b
;&&b c
return(( 
Ok(( 
((( 
Success(( 
((( 
result((  
)((  !
)((! "
;((" #
})) 
[++ 

HttpDelete++ 
(++ 
$str++ 
)++ 
]++ 
public,, 

async,, 
Task,, 
<,, 
IActionResult,, #
>,,# $,
 DeleteWalletRetentionConfigAsync,,% E
(,,E F
[,,F G
	FromRoute,,G P
],,P Q
int,,R U
id,,V X
),,X Y
{-- 
var.. 
result.. 
=.. 
await.. )
_walletRetentionConfigService.. 8
...8 9,
 DeleteWalletRetentionConfigAsync..9 Y
(..Y Z
id..Z \
)..\ ]
;..] ^
return00 
result00 
is00 
null00 
?00 
Ok00  "
(00" #
Fail00# '
(00' (
$str00( M
)00M N
)00N O
:00P Q
Ok00R T
(00T U
Success00U \
(00\ ]
result00] c
)00c d
)00d e
;00e f
}11 
}44 ©+
QC:\HeroSystem\walletService\WalletService.Api\Controllers\WalletWaitController.cs
	namespace 	
WalletService
 
. 
Api 
. 
Controllers '
;' (
[ 
ApiController 
] 
[ 

ApiVersion 
( 
$str 
) 
] 
[		 
Route		 
(		 
$str		 -
)		- .
]		. /
public

 
class

  
WalletWaitController

 !
:

" #
BaseController

$ 2
{ 
private 
readonly 
IWalletWaitService '
_walletWaitService( :
;: ;
public 
 
WalletWaitController 
(  
IWalletWaitService  2
walletWaitService3 D
)D E
{ 
_walletWaitService 
= 
walletWaitService .
;. /
} 
[ 
HttpGet 
( 
$str !
)! "
]" #
public 

async 
Task 
< 
IActionResult #
># $
GetAllWalletsWaits% 7
(7 8
)8 9
{ 
var 
result 
= 
await 
_walletWaitService -
.- .
GetAllWalletsWaits. @
(@ A
)A B
;B C
return 
Ok 
( 
Success 
( 
result  
)  !
)! "
;" #
} 
[ 
HttpGet 
( 
$str 
) 
] 
public 

async 
Task 
< 
IActionResult #
># $
GetWalletById% 2
(2 3
int3 6
id7 9
)9 :
{ 
var 
result 
= 
await 
_walletWaitService -
.- .
GetWalletWaitById. ?
(? @
id@ B
)B C
;C D
return 
result 
is 
null 
? 
Ok  "
(" #
Fail# '
(' (
$str( F
)F G
)G H
:I J
OkK M
(M N
SuccessN U
(U V
resultV \
)\ ]
)] ^
;^ _
}   
[!! 
HttpPost!! 
]!! 
public"" 

async"" 
Task"" 
<"" 
IActionResult"" #
>""# $!
CreateWalletWaitAsync""% :
("": ;
[""; <
FromBody""< D
]""D E
WalletWaitRequest""F W
request""X _
)""_ `
{## 
var$$ 
result$$ 
=$$ 
await$$ 
_walletWaitService$$ -
.$$- .!
CreateWalletWaitAsync$$. C
($$C D
request$$D K
)$$K L
;$$L M
return&& 
result&& 
is&& 
null&& 
?&& 
Ok&&  "
(&&" #
Fail&&# '
(&&' (
$str&&( H
)&&H I
)&&I J
:&&K L
Ok&&M O
(&&O P
Success&&P W
(&&W X
result&&X ^
)&&^ _
)&&_ `
;&&` a
}'' 
[)) 
HttpPut)) 
()) 
$str)) 
))) 
])) 
public** 

async** 
Task** 
<** 
IActionResult** #
>**# $!
UpdateWalletWaitAsync**% :
(**: ;
[**; <
	FromRoute**< E
]**E F
int**G J
id**K M
,**M N
[**O P
FromBody**P X
]**X Y
WalletWaitRequest**Z k
request**l s
)**s t
{++ 
var,, 
result,, 
=,, 
await,, 
_walletWaitService,, -
.,,- .!
UpdateWalletWaitAsync,,. C
(,,C D
id,,D F
,,,F G
request,,H O
),,O P
;,,P Q
return.. 
result.. 
is.. 
null.. 
?.. 
Ok..  "
(.." #
Fail..# '
(..' (
$str..( H
)..H I
)..I J
:..K L
Ok..M O
(..O P
Success..P W
(..W X
result..X ^
)..^ _
).._ `
;..` a
}// 
[11 

HttpDelete11 
(11 
$str11 
)11 
]11 
public22 

async22 
Task22 
<22 
IActionResult22 #
>22# $!
DeleteWalletWaitAsync22% :
(22: ;
[22; <
	FromRoute22< E
]22E F
int22G J
id22K M
)22M N
{33 
var44 
result44 
=44 
await44 
_walletWaitService44 -
.44- .!
DeleteWalletWaitAsync44. C
(44C D
id44D F
)44F G
;44G H
return66 
result66 
is66 
null66 
?66 
Ok66  "
(66" #
Fail66# '
(66' (
$str66( H
)66H I
)66I J
:66K L
Ok66M O
(66O P
Success66P W
(66W X
result66X ^
)66^ _
)66_ `
;66` a
}77 
}::  ,
WC:\HeroSystem\walletService\WalletService.Api\Controllers\WalletWithdrawalController.cs
	namespace 	
WalletService
 
. 
Api 
. 
Controllers '
;' (
[ 
ApiController 
] 
[ 

ApiVersion 
( 
$str 
) 
] 
[		 
Route		 
(		 
$str		 3
)		3 4
]		4 5
public

 
class

 &
WalletWithdrawalController

 '
:

( )
BaseController

* 8
{ 
private 
readonly $
IWalletWithdrawalService -$
_walletWithdrawalService. F
;F G
public 
&
WalletWithdrawalController %
(% &$
IWalletWithdrawalService& >#
walletWithdrawalService? V
)V W
{ $
_walletWithdrawalService  
=! "#
walletWithdrawalService# :
;: ;
} 
[ 
HttpGet 
( 
$str '
)' (
]( )
public 

async 
Task 
< 
IActionResult #
># $$
GetAllWalletsWithdrawals% =
(= >
)> ?
{ 
var 
result 
= 
await $
_walletWithdrawalService 3
.3 4$
GetAllWalletsWithdrawals4 L
(L M
)M N
;N O
return 
Ok 
( 
Success 
( 
result  
)  !
)! "
;" #
} 
[ 
HttpGet 
( 
$str 
) 
] 
public 

async 
Task 
< 
IActionResult #
># $#
GetWalletWithdrawalById% <
(< =
int= @
idA C
)C D
{ 
var 
result 
= 
await $
_walletWithdrawalService 3
.3 4#
GetWalletWithdrawalById4 K
(K L
idL N
)N O
;O P
return   
result   
is   
null   
?   
Ok    "
(  " #
Fail  # '
(  ' (
$str  ( M
)  M N
)  N O
:  P Q
Ok  R T
(  T U
Success  U \
(  \ ]
result  ] c
)  c d
)  d e
;  e f
}"" 
[## 
HttpPost## 
]## 
public$$ 

async$$ 
Task$$ 
<$$ 
IActionResult$$ #
>$$# $'
CreateWalletWithdrawalAsync$$% @
($$@ A
[$$A B
FromBody$$B J
]$$J K#
WalletWithDrawalRequest$$L c
request$$d k
)$$k l
{%% 
var&& 
result&& 
=&& 
await&& $
_walletWithdrawalService&& 3
.&&3 4'
CreateWalletWithdrawalAsync&&4 O
(&&O P
request&&P W
)&&W X
;&&X Y
return)) 
result)) 
is)) 
null)) 
?)) 
Ok))  "
())" #
Fail))# '
())' (
$str))( O
)))O P
)))P Q
:))R S
Ok))T V
())V W
Success))W ^
())^ _
result))_ e
)))e f
)))f g
;))g h
}++ 
[-- 
HttpPut-- 
(-- 
$str-- 
)-- 
]-- 
public.. 

async.. 
Task.. 
<.. 
IActionResult.. #
>..# $'
UpdateWalletWithdrawalAsync..% @
(..@ A
[..A B
	FromRoute..B K
]..K L
int..M P
id..Q S
,..S T
[..U V
FromBody..V ^
]..^ _#
WalletWithDrawalRequest..` w
request..x 
)	.. Ä
{// 
var00 
result00 
=00 
await00 $
_walletWithdrawalService00 3
.003 4'
UpdateWalletWithdrawalAsync004 O
(00O P
id00P R
,00R S
request00T [
)00[ \
;00\ ]
return33 
result33 
is33 
null33 
?33 
Ok33  "
(33" #
Fail33# '
(33' (
$str33( O
)33O P
)33P Q
:33R S
Ok33T V
(33V W
Success33W ^
(33^ _
result33_ e
)33e f
)33f g
;33g h
}44 
[66 

HttpDelete66 
(66 
$str66 
)66 
]66 
public77 

async77 
Task77 
<77 
IActionResult77 #
>77# $'
DeleteWalletWithdrawalAsync77% @
(77@ A
[77A B
	FromRoute77B K
]77K L
int77M P
id77Q S
)77S T
{88 
var99 
result99 
=99 
await99 $
_walletWithdrawalService99 3
.993 4'
DeleteWalletWithdrawalAsync994 O
(99O P
id99P R
)99R S
;99S T
return;; 
result;; 
is;; 
null;; 
?;; 
Ok;;  "
(;;" #
Fail;;# '
(;;' (
$str;;( O
);;O P
);;P Q
:;;R S
Ok;;T V
(;;V W
Success;;W ^
(;;^ _
result;;_ e
);;e f
);;f g
;;;g h
}<< 
}?? Å
8C:\HeroSystem\walletService\WalletService.Api\Program.cs
var 
builder 
= 
WebApplication 
. 
CreateBuilder *
(* +
args+ /
)/ 0
;0 1
builder 
. 
Services 
. 
AddHealthChecks  
(  !
)! "
;" #
builder		 
.		 
Services		 
.		 $
IocAppInjectDependencies		 )
(		) *
)		* +
;		+ ,
QuestPDF 
. 	
Settings	 
. 
License 
= 
LicenseType '
.' (
	Community( 1
;1 2
builder 
. 
Services 
. #
AddEndpointsApiExplorer (
(( )
)) *
;* +
builder 
. 
Services 
. 
AddHostedService !
<! ".
"RedisCacheCleanupBackgroundService" D
>D E
(E F
)F G
;G H
var 
app 
= 	
builder
 
. 
Build 
( 
) 
; 
if 
( 
app 
. 
Environment 
. 
IsDevelopment !
(! "
)" #
)# $
app 
. %
UseDeveloperExceptionPage !
(! "
)" #
;# $
app 
. 
UseCors 
( 
) 
; 
app 
. 

UseSwagger 
( 
) 
; 

AppContext 

.
 
	SetSwitch 
( 
$str ;
,; <
true= A
)A B
;B C

AppContext 

.
 
	SetSwitch 
( 
$str @
,@ A
trueB F
)F G
;G H
app 
. 
UseSwaggerUI 
( 
s 
=> 
{ 
s 
. 
SwaggerEndpoint )
() *
$str* D
,D E
$strF Y
)Y Z
;Z [
}\ ]
)] ^
;^ _
app 
. 
UseHttpsRedirection 
( 
) 
; 
app 
. 
UseMiddleware 
< %
TokenValidationMiddleware +
>+ ,
(, -
)- .
;. /
app 
. 
UseMiddleware 
< 
ExceptionMiddleware %
>% &
(& '
)' (
;( )
app 
. 

UseRouting 
( 
) 
; 
app 
. 
UseAuthorization 
( 
) 
; 
app 
. 
MapHealthChecks 
( 
$str 
) 
; 
app   
.   
MapControllers   
(   
)   
;   
app!! 
.!! 
Run!! 
(!! 
)!! 	
;!!	 
