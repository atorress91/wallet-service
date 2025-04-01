ü%
KC:\HeroSystem\walletService\WalletService.Worker\Abstract\CronJobService.cs
	namespace 	
WalletService
 
. 
Worker 
. 
Abstract '
;' (
public 
abstract 
class 
CronJobService $
:% &
IHostedService' 5
,5 6
IDisposable7 B
{ 
private 
System 
. 
Timers "
." #
Timer# (
_timer) /
;/ 0
private 
readonly 
CronExpression #
_expression) 4
;4 5
private		 
readonly		 
TimeZoneInfo		 !
_timeZoneInfo		) 6
;		6 7
	protected 
CronJobService 
( 
string #
cronExpression$ 2
)2 3
{ 
_expression 
= 
CronExpression &
.& '
Parse' ,
(, -
cronExpression- ;
); <
;< =
_timeZoneInfo 
= 
TimeZoneInfo $
.$ %
Local% *
;* +
} 
	protected 
abstract 
Task 
Work  
(  !
CancellationToken! 2
cancellationToken3 D
)D E
;E F
public 

virtual 
async 
Task 

StartAsync (
(( )
CancellationToken) :
cancellationToken; L
)L M
{ 
await 
ScheduleJob 
( 
cancellationToken +
)+ ,
;, -
} 
private 
async 
Task 
ScheduleJob "
(" #
CancellationToken# 4
cancellationToken5 F
)F G
{ 
var 
next 
= 
_expression 
. 
GetNextOccurrence 0
(0 1
DateTimeOffset1 ?
.? @
Now@ C
,C D
_timeZoneInfoE R
)R S
;S T
if 

( 
next 
. 
HasValue 
) 
{ 	
var 
delay 
= 
next 
. 
Value "
-# $
DateTimeOffset% 3
.3 4
Now4 7
;7 8
if 
( 
delay 
. 
TotalMilliseconds '
<=( *
$num+ ,
), -
await. 3
ScheduleJob4 ?
(? @
cancellationToken@ Q
)Q R
;R S
_timer 
= 
new 
System 
.  
Timers  &
.& '
Timer' ,
(, -
delay- 2
.2 3
TotalMilliseconds3 D
)D E
;E F
_timer   
.   
Elapsed   
+=   
async   #
(  $ %
sender  % +
,  + ,
args  - 1
)  1 2
=>  3 5
{!! 
_timer"" 
."" 
Dispose"" 
("" 
)""  
;""  !
_timer## 
=## 
null## 
!## 
;## 
if%% 
(%% 
!%% 
cancellationToken%% &
.%%& '#
IsCancellationRequested%%' >
)%%> ?
await%%@ E
Work%%F J
(%%J K
cancellationToken%%K \
)%%\ ]
;%%] ^
if&& 
(&& 
!&& 
cancellationToken&& &
.&&& '#
IsCancellationRequested&&' >
)&&> ?
await&&@ E
ScheduleJob&&F Q
(&&Q R
cancellationToken&&R c
)&&c d
;&&d e
}'' 
;'' 
_timer(( 
.(( 
Start(( 
((( 
)(( 
;(( 
})) 	
await++ 
Task++ 
.++ 
CompletedTask++  
;++  !
},, 
public.. 

virtual.. 
async.. 
Task.. 
	StopAsync.. '
(..' (
CancellationToken..( 9
cancellationToken..: K
)..K L
{// 
_timer00 
.00 
Stop00 
(00 
)00 
;00 
await11 
Task11 
.11 
CompletedTask11  
;11  !
}22 
public44 

virtual44 
void44 
Dispose44 
(44  
)44  !
{55 
_timer66 
.66 
Dispose66 
(66 
)66 
;66 
}77 
}88 Ñ
;C:\HeroSystem\walletService\WalletService.Worker\Program.cs
var 
host 
=	 

Host 
.  
CreateDefaultBuilder $
($ %
args% )
)) *
. 
ConfigureServices 
( 
services 
=>  "
{ 
services 
. 
AddHostedService !
<! "
Worker" (
>( )
() *
)* +
;+ ,
services		 
.		 '
IocWorkerInjectDependencies		 ,
(		, -
)		- .
;		. /
}

 
)

 
. 
Build 

(
 
) 
; 
await 
host 

.
 
RunAsync 
( 
) 
; ‚
[C:\HeroSystem\walletService\WalletService.Worker\Workers\ProcessDeleteTransactionsWorker.cs
	namespace 	
WalletService
 
. 
Worker 
. 
Workers &
;& '
public 
class +
ProcessDeleteTransactionsWorker ,
:- .
CronJobService/ =
{		 
private

 
readonly

 
IServiceProvider

 %
_serviceProvider

> N
;

N O
private 
readonly 
Logger 
_logger #
;# $
public 
+
ProcessDeleteTransactionsWorker *
(* +
string 
cronExpression1 ?
,? @
IServiceProvider 
serviceProvider1 @
)@ A
:B C
baseD H
(H I
cronExpressionI W
)W X
{ 
_serviceProvider 
= 
serviceProvider *
;* +
_logger 
= 

LogManager %
.% &!
GetCurrentClassLogger& ;
(; <
)< =
;= >
} 
	protected 
override 
async 
Task !
Work" &
(& '
CancellationToken' 8
cancellationToken9 J
)J K
{ 
try 
{ 	
using 
var 
scope 
=% &
_serviceProvider' 7
.7 8
CreateScope8 C
(C D
)D E
;E F
var 
invoiceService $
=% &
scope' ,
., -
ServiceProvider- <
.< =
GetRequiredService= O
<O P
IInvoiceServiceP _
>_ `
(` a
)a b
;b c
_logger 
. 
Info 
( 
$str M
)M N
;N O
await 
invoiceService  
.  !1
%RevertUnconfirmedOrUnpaidTransactions! F
(F G
)G H
;H I
} 	
catch 
( 
	Exception 
e 
) 
{   	
_logger!! 
.!! 
Error!! 
(!! 
$"!! 
$str!! S
{!!S T
e!!T U
.!!U V
ToJsonString!!V b
(!!b c
)!!c d
}!!d e
"!!e f
)!!f g
;!!g h
}"" 	
}## 
}$$ ùi
BC:\HeroSystem\walletService\WalletService.Worker\Workers\Worker.cs
	namespace 	
WalletService
 
. 
Worker 
. 
Workers &
;& '
public 
class 
Worker 
: 
BackgroundService '
{		 
private

 
readonly

 
ILogger

 
<

 
Worker

 #
>

# $
_logger

% ,
;

, -
private 
readonly  
IServiceScopeFactory ) 
_serviceScopeFactory* >
;> ?
private 
Task 
? 
_executingTask  
;  !
private #
CancellationTokenSource #
_cts$ (
;( )
private 
readonly 
List 
< 
BaseKafkaConsumer +
>+ ,

_consumers- 7
=8 9
new: =
(= >
)> ?
;? @
private 
readonly $
ApplicationConfiguration -
_configuration. <
;< =
public 

Worker 
( 
ILogger 
< 
Worker  
>  !
logger" (
,( )
IOptions* 2
<2 3$
ApplicationConfiguration3 K
>K L
configurationM Z
,Z [ 
IServiceScopeFactory\ p 
serviceScopeFactory	q „
)
„ …
{ 
_logger 
= 
logger %
;% & 
_serviceScopeFactory 
= 
serviceScopeFactory 2
;2 3
_configuration 
= 
configuration ,
., -
Value- 2
;2 3
} 
public 

override 
Task 

StartAsync #
(# $
CancellationToken$ 5
cancellationToken6 G
)G H
{ 
_logger 
. 

LogWarning 
( 
$str R
)R S
;S T
_cts 
= #
CancellationTokenSource 0
.0 1#
CreateLinkedTokenSource1 H
(H I
cancellationTokenI Z
)Z [
;[ \
_executingTask 
= 
ExecuteAsync %
(% &
_cts& *
.* +
Token+ 0
)0 1
;1 2
return 
_executingTask 
. 
IsCompleted )
?* +
_executingTask, :
:; <
Task= A
.A B
CompletedTaskB O
;O P
} 
public!! 

override!! 
async!! 
Task!! 
	StopAsync!! (
(!!( )
CancellationToken!!) :
cancellationToken!!; L
)!!L M
{"" 
if## 

(## 
_executingTask## 
is## 
null## "
)##" #
return$$ 
;$$ 
_logger&& 
.&& 

LogWarning&& 
(&& 
$str&& <
)&&< =
;&&= >
_cts'' 
.'' 
Cancel'' 
('' 
)'' 
;'' 
foreach)) 
()) 
var)) 
item)) 
in)) 

_consumers)) '
)))' (
item** 
.** 
StopConsume** 
(** 
)** 
;** 
await,, 
Task,, 
.,, 
WhenAny,, 
(,, 
_executingTask,, )
,,,) *
Task,,+ /
.,,/ 0
Delay,,0 5
(,,5 6
Timeout,,6 =
.,,= >
Infinite,,> F
,,,F G
cancellationToken,,H Y
),,Y Z
),,Z [
;,,[ \
cancellationToken-- 
.-- (
ThrowIfCancellationRequested-- 6
(--6 7
)--7 8
;--8 9
_logger// 
.// 

LogWarning// 
(// 
$str// ;
)//; <
;//< =
}00 
	protected22 
override22 
Task22 
ExecuteAsync22 (
(22( )
CancellationToken22) :
stoppingToken22; H
)22H I
{33 
var44 !
consumerCountSettings44 !
=44" #
_configuration44$ 2
.442 3
ConsumersSetting443 C
;44C D
for66 
(66 
var66 
i66 
=66 
$num66 
;66 
i66 
<66 !
consumerCountSettings66 1
!661 2
.662 3
ConsumersCount663 A
;66A B
i66C D
++66D F
)66F G

_consumers77 
.77 
Add77 
(77 
new77 !
ProcessModel3Consumer77 4
(774 5
new775 8
ConsumerSettings779 I
{88 
Topics99 
=99  !
new99" %
[99% &
]99& '
{99( )
KafkaTopics99* 5
.995 6
ProcessModel3Topic996 H
}99I J
,99J K
GroupId:: 
=::  !
$"::" $
{::$ %
KafkaTopics::% 0
.::0 1
ProcessModel3Topic::1 C
}::C D
$str::D J
"::J K
,::K L
GroupInstanceId;; 
=;;  !
$";;" $
{;;$ %
KafkaTopics;;% 0
.;;0 1
ProcessModel3Topic;;1 C
};;C D
$str;;D T
{;;T U
i;;U V
};;V W
";;W X
}<< 
,<< 
_configuration<< 
,<< 
_logger<< &
,<<& ' 
_serviceScopeFactory<<( <
)<<< =
{== 
ConsumerIndex>> 
=>> 
i>>  !
}?? 
)?? 
;?? 
forAA 
(AA 
varAA 
iAA 
=AA 
$numAA 
;AA 
iAA 
<AA !
consumerCountSettingsAA 1
!AA1 2
.AA2 3
ConsumersCountAA3 A
;AAA B
iAAC D
++AAD F
)AAF G

_consumersBB 
.BB 
AddBB 
(BB 
newBB !
ProcessModel2ConsumerBB 4
(BB4 5
newBB5 8
ConsumerSettingsBB9 I
{CC 
TopicsDD 
=DD  !
newDD" %
[DD% &
]DD& '
{DD( )
KafkaTopicsDD* 5
.DD5 6
ProcessModel2TopicDD6 H
}DDI J
,DDJ K
GroupIdEE 
=EE  !
$"EE" $
{EE$ %
KafkaTopicsEE% 0
.EE0 1
ProcessModel2TopicEE1 C
}EEC D
$strEED J
"EEJ K
,EEK L
GroupInstanceIdFF 
=FF  !
$"FF" $
{FF$ %
KafkaTopicsFF% 0
.FF0 1
ProcessModel2TopicFF1 C
}FFC D
$strFFD T
{FFT U
iFFU V
}FFV W
"FFW X
}GG 
,GG 
_configurationGG 
,GG 
_loggerGG &
,GG& ' 
_serviceScopeFactoryGG( <
)GG< =
{HH 
ConsumerIndexII 
=II 
iII  !
}JJ 
)JJ 
;JJ 
forLL 
(LL 
varLL 
iLL 
=LL 
$numLL 
;LL 
iLL 
<LL !
consumerCountSettingsLL 1
!LL1 2
.LL2 3
ConsumersCountLL3 A
;LLA B
iLLC D
++LLD F
)LLF G

_consumersMM 
.MM 
AddMM 
(MM 
newMM "
ProcessModel1BConsumerMM 5
(MM5 6
newMM6 9
ConsumerSettingsMM: J
{NN 
TopicsOO 
=OO  !
newOO" %
[OO% &
]OO& '
{OO( )
KafkaTopicsOO* 5
.OO5 6
ProcessModel1BTopicOO6 I
}OOJ K
,OOK L
GroupIdPP 
=PP  !
$"PP" $
{PP$ %
KafkaTopicsPP% 0
.PP0 1
ProcessModel1BTopicPP1 D
}PPD E
$strPPE K
"PPK L
,PPL M
GroupInstanceIdQQ 
=QQ  !
$"QQ" $
{QQ$ %
KafkaTopicsQQ% 0
.QQ0 1
ProcessModel1BTopicQQ1 D
}QQD E
$strQQE U
{QQU V
iQQV W
}QQW X
"QQX Y
}RR 
,RR 
_configurationRR 
,RR 
_loggerRR &
,RR& ' 
_serviceScopeFactoryRR( <
)RR< =
{SS 
ConsumerIndexTT 
=TT 
iTT  !
}UU 
)UU 
;UU 
forWW 
(WW 
varWW 
iWW 
=WW 
$numWW 
;WW 
iWW 
<WW !
consumerCountSettingsWW 1
!WW1 2
.WW2 3
ConsumersCountWW3 A
;WWA B
iWWC D
++WWD F
)WWF G

_consumersXX 
.XX 
AddXX 
(XX 
newXX "
ProcessModel1AConsumerXX 5
(XX5 6
newXX6 9
ConsumerSettingsXX: J
{YY 
TopicsZZ 
=ZZ  !
newZZ" %
[ZZ% &
]ZZ& '
{ZZ( )
KafkaTopicsZZ* 5
.ZZ5 6
ProcessModel1ATopicZZ6 I
}ZZJ K
,ZZK L
GroupId[[ 
=[[  !
$"[[" $
{[[$ %
KafkaTopics[[% 0
.[[0 1
ProcessModel1ATopic[[1 D
}[[D E
$str[[E K
"[[K L
,[[L M
GroupInstanceId\\ 
=\\  !
$"\\" $
{\\$ %
KafkaTopics\\% 0
.\\0 1
ProcessModel1ATopic\\1 D
}\\D E
$str\\E U
{\\U V
i\\V W
}\\W X
"\\X Y
}]] 
,]] 
_configuration]] 
,]] 
_logger]] &
,]]& ' 
_serviceScopeFactory]]( <
)]]< =
{^^ 
ConsumerIndex__ 
=__ 
i__  !
}`` 
)`` 
;`` 
forbb 
(bb 
varbb 
ibb 
=bb 
$numbb 
;bb 
ibb 
<bb !
consumerCountSettingsbb 1
.bb1 2(
ConsumersProcessPaymentCountbb2 N
;bbN O
ibbP Q
++bbQ S
)bbS T

_consumerscc 
.cc 
Addcc 
(cc 
newcc -
!ProcessPaymentModel1A1B23Consumercc @
(cc@ A
newccA D
ConsumerSettingsccE U
{dd 
Topicsee 
=ee  !
newee" %
[ee% &
]ee& '
{ee( )
KafkaTopicsee* 5
.ee5 6,
 ProcessPaymentModelTwoThreeTopicee6 V
}eeW X
,eeX Y
GroupIdff 
=ff  !
$"ff" $
{ff$ %
KafkaTopicsff% 0
.ff0 1,
 ProcessPaymentModelTwoThreeTopicff1 Q
}ffQ R
$strffR X
"ffX Y
,ffY Z
GroupInstanceIdgg 
=gg  !
$"gg" $
{gg$ %
KafkaTopicsgg% 0
.gg0 1,
 ProcessPaymentModelTwoThreeTopicgg1 Q
}ggQ R
$strggR b
{ggb c
iggc d
}ggd e
"gge f
}hh 
,hh 
_configurationhh 
,hh 
_loggerhh &
,hh& ' 
_serviceScopeFactoryhh( <
)hh< =
{ii 
ConsumerIndexjj 
=jj 
ijj  !
}kk 
)kk 
;kk 
formm 
(mm 
varmm 
imm 
=mm 
$nummm 
;mm 
imm 
<mm !
consumerCountSettingsmm 1
.mm1 2(
ConsumersProcessPaymentCountmm2 N
;mmN O
immP Q
++mmQ S
)mmS T

_consumersnn 
.nn 
Addnn 
(nn 
newnn ,
 ProcessModelsFourFiveSixConsumernn ?
(nn? @
newnn@ C
ConsumerSettingsnnD T
{oo 
Topicspp 
=pp  !
newpp" %
[pp% &
]pp& '
{pp( )
KafkaTopicspp* 5
.pp5 6(
ProcessModelFourFiveSixTopicpp6 R
}ppS T
,ppT U
GroupIdqq 
=qq  !
$"qq" $
{qq$ %
KafkaTopicsqq% 0
.qq0 1(
ProcessModelFourFiveSixTopicqq1 M
}qqM N
$strqqN T
"qqT U
,qqU V
GroupInstanceIdrr 
=rr  !
$"rr" $
{rr$ %
KafkaTopicsrr% 0
.rr0 1(
ProcessModelFourFiveSixTopicrr1 M
}rrM N
$strrrN ^
{rr^ _
irr_ `
}rr` a
"rra b
}ss 
,ss 
_configurationss 
,ss 
_loggerss &
,ss& ' 
_serviceScopeFactoryss( <
)ss< =
{tt 
ConsumerIndexuu 
=uu 
iuu  !
}vv 
)vv 
;vv 
foreachxx 
(xx 
varxx 
consumerxx 
inxx  

_consumersxx! +
)xx+ ,
consumeryy 
.yy 
Consumeyy 
(yy 
)yy 
;yy 
_logger{{ 
.{{ 
LogInformation{{ 
({{ 
$str{{ 5
){{5 6
;{{6 7
return|| 
Task|| 
.|| 
CompletedTask|| !
;||! "
}}} 
}~~ 