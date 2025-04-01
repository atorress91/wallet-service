∆F
ZC:\HeroSystem\walletService\WalletService.Models\Configuration\ApplicationConfiguration.cs
	namespace 	
WalletService
 
. 
Models 
. 
Configuration ,
;, -
public 
class $
ApplicationConfiguration %
{ 
public 

ConnectionStrings 
? 
ConnectionStrings /
{0 1
get2 5
;5 6
set7 :
;: ;
}< =
public 

string 
? 
	ClientUrl 
{ 
get "
;" #
set$ '
;' (
}) *
public 

EndpointTokens 
? 
EndpointTokens )
{* +
get, /
;/ 0
set1 4
;4 5
}6 7
public 

	Endpoints 
? 
	Endpoints 
{  !
get" %
;% &
set' *
;* +
}, -
public

 

EmailCredentials

 
?

 
EmailCredentials

 -
{

. /
get

0 3
;

3 4
set

5 8
;

8 9
}

: ;
public 

ConsumersSetting 
? 
ConsumersSetting -
{. /
get0 3
;3 4
set5 8
;8 9
}: ;
public 

ConPayments 
? 
ConPayments #
{$ %
get& )
;) *
set+ .
;. /
}0 1
public 

SendingBlue 
? 
SendingBlue #
{$ %
get& )
;) *
set+ .
;. /
}0 1
public 

CoinPay 
? 
CoinPay 
{ 
get !
;! "
set# &
;& '
}( )
public 

Pagadito 
? 
Pagadito 
{ 
get  #
;# $
set% (
;( )
}* +
public 

	WebTokens 
? 
	WebTokens 
{  !
get" %
;% &
set' *
;* +
}, -
} 
public 
class 
ConnectionStrings 
{ 
public 

string 
?  
PostgreSqlConnection '
{( )
get* -
;- .
set/ 2
;2 3
}4 5
public 

string 
? 
RedisConnection "
{# $
get% (
;( )
set* -
;- .
}/ 0
} 
public 
class 
ConsumersSetting 
{ 
public   

int   
ConsumersCount   
{   
get    #
;  # $
set  % (
;  ( )
}  * +
public!! 

int!! (
ConsumersProcessPaymentCount!! +
{!!, -
get!!. 1
;!!1 2
set!!3 6
;!!6 7
}!!8 9
=!!: ;
$num!!< =
;!!= >
public"" 

string"" 

BrokerList"" 
{"" 
get"" "
;""" #
set""$ '
;""' (
}"") *
}## 
public%% 
class%% 
	Endpoints%% 
{&& 
public'' 

string'' 
?'' 
	WalletURL'' 
{'' 
get'' "
;''" #
set''$ '
;''' (
}'') *
public(( 

string(( 
?(( 
AccountServiceURL(( $
{((% &
get((' *
;((* +
set((, /
;((/ 0
}((1 2
public)) 

string)) 
?)) "
SystemConfigurationURL)) )
{))* +
get)), /
;))/ 0
set))1 4
;))4 5
}))6 7
public++ 

string++ 
?++ 
InventoryServiceURL++ &
{++' (
get++) ,
;++, -
set++. 1
;++1 2
}++3 4
public-- 

string-- 
?-- 

CoinPayURL-- 
{-- 
get--  #
;--# $
set--% (
;--( )
}--* +
}.. 
public00 
class00 
EndpointTokens00 
{11 
public22 

string22 
?22 
WalletToken22 
{22  
get22! $
;22$ %
set22& )
;22) *
}22+ ,
public33 

string33 
?33 
AccountServiceToken33 &
{33' (
get33) ,
;33, -
set33. 1
;331 2
}333 4
public44 

string44 
?44 +
SystemConfigurationServiceToken44 2
{443 4
get445 8
;448 9
set44: =
;44= >
}44? @
public55 

string55 
?55 !
InventoryServiceToken55 (
{55) *
get55+ .
;55. /
set550 3
;553 4
}555 6
}66 
public88 
class88 
EmailCredentials88 
{99 
public:: 

string:: 
?:: 
From:: 
{:: 
get:: 
;:: 
set:: "
;::" #
}::$ %
public;; 

string;; 
?;; 
Password;; 
{;; 
get;; !
;;;! "
set;;# &
;;;& '
};;( )
public<< 

string<< 
?<< 
Smtp<< 
{<< 
get<< 
;<< 
set<< "
;<<" #
}<<$ %
public== 

int== 
Port== 
{== 
get== 
;== 
set== 
;== 
}==  !
}>> 
public@@ 
class@@ 
ConPayments@@ 
{AA 
publicBB 

stringBB 
KeyBB 
{BB 
getBB 
;BB 
setBB  
;BB  !
}BB" #
publicCC 

stringCC 
SecretCC 
{CC 
getCC 
;CC 
setCC  #
;CC# $
}CC% &
publicEE 

stringEE 
	IpnSecretEE 
{EE 
getEE !
;EE! "
setEE# &
;EE& '
}EE( )
publicGG 

stringGG 

MerchantIdGG 
{GG 
getGG "
;GG" #
setGG$ '
;GG' (
}GG) *
publicII 

stringII 

DebugEmailII 
{II 
getII "
;II" #
setII$ '
;II' (
}II) *
}JJ 
publicLL 
classLL 
SendingBlueLL 
{MM 
publicNN 

stringNN 
?NN 
ApiKeyNN 
{NN 
getNN 
;NN  
setNN! $
;NN$ %
}NN& '
}OO 
publicQQ 
classQQ 
CoinPayQQ 
{RR 
publicSS 

stringSS 
?SS 
	SecretKeySS 
{SS 
getSS "
;SS" #
setSS$ '
;SS' (
}SS) *
publicTT 

stringTT 
?TT 
InitialTokenTT 
{TT  !
getTT" %
;TT% &
setTT' *
;TT* +
}TT, -
publicUU 

intUU 
UserIdUU 
{UU 
getUU 
;UU 
setUU  
;UU  !
}UU" #
}VV 
publicXX 
classXX 
PagaditoXX 
{YY 
publicZZ 

stringZZ 
?ZZ 
UrlZZ 
{ZZ 
getZZ 
;ZZ 
setZZ !
;ZZ! "
}ZZ# $
public[[ 

string[[ 
?[[ 
Uid[[ 
{[[ 
get[[ 
;[[ 
set[[ !
;[[! "
}[[# $
public\\ 

string\\ 
?\\ 
Wsk\\ 
{\\ 
get\\ 
;\\ 
set\\ !
;\\! "
}\\# $
}]] 
public__ 
class__ 
	WebTokens__ 
{`` 
publicaa 

stringaa 
?aa 
EcosystemTokenaa !
{aa" #
getaa$ '
;aa' (
setaa) ,
;aa, -
}aa. /
publicbb 

stringbb 
?bb 
RecyCoinTokenbb  
{bb! "
getbb# &
;bb& '
setbb( +
;bb+ ,
}bb- .
publiccc 

stringcc 
?cc 
HouseCoinTokencc !
{cc" #
getcc$ '
;cc' (
setcc) ,
;cc, -
}cc. /
publicee 

stringee 
?ee 
ExitoJuntosTokenee #
{ee$ %
getee& )
;ee) *
setee+ .
;ee. /
}ee0 1
}ff û
RC:\HeroSystem\walletService\WalletService.Models\Configuration\ConsumerSettings.cs
	namespace 	
WalletService
 
. 
Models 
. 
Configuration ,
;, -
public 
class 
ConsumerSettings 
{ 
public 

string 
[ 
] 
Topics 
{ 
get  
;  !
set" %
;% &
}' (
public 

string 
GroupId 
{ 
get 
;  
set! $
;$ %
}& '
public 

bool 
ReadLast 
{ 
get 
; 
set  #
;# $
}% &
=' (
false) .
;. /
public 

bool 
UniqueByMac 
{ 
get !
;! "
set# &
;& '
}( )
=* +
true, 0
;0 1
public		 

string		 
GroupInstanceId		 !
{		" #
get		$ '
;		' (
set		) ,
;		, -
}		. /
public

 

bool

 
EnableAutoCommit

  
{

! "
get

# &
;

& '
set

( +
;

+ ,
}

- .
=

/ 0
false

1 6
;

6 7
} ƒ
PC:\HeroSystem\walletService\WalletService.Models\Constants\ApmTransactionType.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Constants (
;( )
public 
class 
ApmTransactionType 
{ 
public 

const 
string 
Request 
=  !
$str" +
;+ ,
public 

const 
string 
KafkaMessage $
=% &
$str' 6
;6 7
public 

const 
string 
BackgroundJob %
=& '
$str( 8
;8 9
} ˘
GC:\HeroSystem\walletService\WalletService.Models\Constants\CacheKeys.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Constants (
;( )
public 
static 
class 
	CacheKeys 
{ 
public 

const 
string $
BalanceInformationModel2 0
=1 2
$str3 Y
;Y Z
public 

const 
string %
BalanceInformationModel1A 1
=2 3
$str4 [
;[ \
public 

const 
string %
BalanceInformationModel1B 1
=2 3
$str4 [
;[ \
public 

const 
string 
ModelInformation (
=) *
$str+ I
;I J
}		 ê	
KC:\HeroSystem\walletService\WalletService.Models\Constants\CoinPayRoutes.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Constants (
;( )
public 
static 
class 
CoinPayRoutes !
{ 
public 

const 
string 
GetTransactionRoute +
=, -
$str. b
;b c
public 

const 
string "
CreateTransactionRoute .
=/ 0
$str1 Z
;Z [
public 

const 
string 
SendFundsRoute &
=' (
$str) O
;O P
public 

const 
string 
CreateChannelRoute *
=+ ,
$str- W
;W X
public		 

const		 
string		 (
GetNetworksByIdCurrencyRoute		 4
=		5 6
$str		7 b
;		b c
public

 

const

 
string

 
CreateAddressRoute

 *
=

+ ,
$str

- Z
;

Z [
} ˝ñ
GC:\HeroSystem\walletService\WalletService.Models\Constants\Constants.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Constants (
;( )
public 
static 
class 
	Constants 
{ 
public 

const 
int !
OriginEcoPoolPurchase -
=> ?
$num@ A
;A B
public 

const 
string "
EcoPoolProductCategory .
=> ?
$str@ R
;R S
public 

const 
string *
EcoPoolProductCategoryForAdmin 6
=> ?
$str@ e
;e f
public 

const 
string "
SubjectConfirmPurchase .
=> ?
$str@ X
;X Y
public		 

const		 
string		 %
SubjectConfirmAffiliation		 1
=		> ?
$str		@ R
;		R S
public

 

const

 
string

  
RevertEcoPoolConcept

 ,
=

> ?
$str

@ ]
;

] ^
public 

const 
string "
AdminEcosystemUserName .
=> ?
$str@ P
;P Q
public 

const 
string 
RecycoinAdmin %
=> ?
$str@ O
;O P
public 

const 
string 
HouseCoinAdmin &
=> ?
$str@ P
;P Q
public 

const 
string 
ExitoJuntosAdmin (
=> ?
$str@ R
;R S
public 

const 
int 
AdminUserId #
=> ?
$num@ A
;A B
public 

const 
string 
AdminCredit #
=> ?
$str@ X
;X Y
public 

const 
string (
HandleDebitTransactionCourse 4
=> ?
$str@ k
;k l
public 

const 
string 
WalletBalance %
=> ?
$str@ T
;T U
public 

const 
string  
WalletModel1ABalance ,
=> ?
$str@ S
;S T
public 

const 
string  
WalletModel1BBalance ,
=> ?
$str@ S
;S T
public 

const 
string %
CoursesDebitTransactionSp 1
=> ?
$str@ b
;b c
public 

const 
string 
AdminPayment $
=> ?
$str@ P
;P Q
public 

const 
string 
ReverseBalance &
=> ?
$str@ P
;P Q
public 

const 
string 
ConPaymentAddress )
=> ?
$str@ d
;d e
public 

const 
string 
ConPaymentCurrency *
=> ?
$str@ L
;L M
public 

const 
string #
CoinPaymentsBnbCurrency /
=> ?
$str@ L
;L M
public 

const 
int 
MembershipBonus '
=> ?
$num@ A
;A B
public 

const 
string 
SubjectConfirmBonus +
=> ?
$str@ V
;V W
public 

const 
int 
DaysToPayQuantity )
=> ?
$num@ C
;C D
public 

const 
string 
CoinPayments $
=> ?
$str@ N
;N O
public 

const 
string 
CoinPay 
=> ?
$str@ I
;I J
public   

const   
string   
Pagadito    
=  > ?
$str  @ J
;  J K
public!! 

const!! 
int!! 
Batches!! 
=!!> ?
$num!!@ D
;!!D E
public"" 

const"" 
string"" 
WithdrawalBalance"" )
=""> ?
$str""@ Q
;""Q R
public## 

const## 
int## 

EmptyValue## "
=##D E
$num##F G
;##G H
public$$ 

const$$ 
string$$ 

Membership$$ "
=$$> ?
$str$$@ K
;$$K L
public%% 

const%% 
string%% !
TransferForMembership%% -
=%%> ?
$str%%@ j
;%%j k
public&& 

const&& 
string&&  
TransferToMembership&& ,
=&&> ?
$str&&@ k
;&&k l
public'' 

const'' 
int'' 
CoinPaymentTax'' &
=''> ?
$num''@ A
;''A B
public(( 

const(( 
string(( 
DebitTransactionSp(( *
=((> ?
$str((@ Y
;((Y Z
public)) 

const)) 
string)) %
DebitTransactionSpModel1A)) 1
=))> ?
$str))@ b
;))b c
public** 

const** 
string** %
DebitTransactionSpModel1B** 1
=**> ?
$str**@ b
;**b c
public++ 

const++ 
string++ %
DebitEcoPoolTransactionSp++ 1
=++> ?
$str++@ a
;++a b
public,, 

const,, 
string,, ,
 DebitEcoPoolTransactionSpModel1A,, 8
=,,> ?
$str,,@ j
;,,j k
public-- 

const-- 
string-- ,
 DebitEcoPoolTransactionSpModel1B-- 8
=--> ?
$str--@ j
;--j k
public.. 

const.. 
string.. #
AdminDebitTransactionSp.. /
=..> ?
$str..@ _
;.._ `
public// 

const// 
string// $
HandleDebitTransactionSp// 0
=//> ?
$str//@ h
;//h i
public00 

const00 
string00 
CreditTransactionSp00 +
=00> ?
$str00@ Z
;00Z [
public11 

const11 
string11 &
CreditTransactionSpModel1A11 2
=11> ?
$str11@ c
;11c d
public22 

const22 
string22 &
CreditTransactionSpModel1B22 2
=22> ?
$str22@ c
;22c d
public33 

const33 
string33 
Model2RequestSp33 '
=33> ?
$str33@ U
;33U V
public44 

const44 
string44 
Model1ARequestSp44 (
=44> ?
$str44@ U
;44U V
public55 

const55 
string55 
Model1BRequestSp55 (
=55> ?
$str55@ U
;55U V
public66 

const66 
string66 
Model3RequestSp66 '
=66> ?
$str66@ [
;66[ \
public77 

const77 
string77 "
RevertDebitTransaction77 .
=77> ?
$str77@ a
;77a b
public88 

const88 
string88 )
CoinPaymentRevertTransactions88 5
=88> ?
$str88@ g
;88g h
public99 

const99 
string99 (
HandleMembershipTransactions99 4
=99> ?
$str99@ c
;99c d
public:: 

const:: 
string:: '
MembershipDebitTransactions:: 3
=::> ?
$str::@ c
;::c d
public;; 

const;; 
string;; *
GetTotalPurchasesInMyNetworkSp;; 6
=;;> ?
$str;;@ p
;;;p q
public<< 

const<< 
string<<  
TypeTableAffiliateId<< ,
=<<> ?
$str<<@ Z
;<<Z [
public== 

const== 
string== %
BulkAdministrativeDebitSp== 1
===> ?
$str==@ b
;==b c
public>> 

const>> 
string>> )
CommissionModelTwoDescription>> 5
=>>> ?
$str>>@ f
;>>f g
public?? 

const?? 
string?? /
#CommissionModelTwoDescriptionNormal?? ;
=??> ?
$str??@ S
;??S T
public@@ 

const@@ 
string@@ +
CommissionModelThreeDescription@@ 7
=@@> ?
$str@@@ f
;@@f g
publicAA 

constAA 
stringAA .
"CommissionModel1ADescriptionNormalAA :
=AA= >
$strAA? S
;AAS T
publicBB 

constBB 
stringBB .
"CommissionModel1BDescriptionNormalBB :
=BB= >
$strBB? S
;BBS T
publicCC 

constCC 
stringCC (
CommissionModel1ADescriptionCC 4
=CC; <
$strCC= d
;CCd e
publicDD 

constDD 
stringDD (
CommissionModel1BDescriptionDD 4
=DD; <
$strDD= d
;DDd e
publicEE 

constEE 
stringEE 1
%CommissionModelThreeDescriptionNormalEE =
=EE> ?
$strEE@ S
;EES T
publicFF 

constFF 
stringFF  
CommissionMembershipFF ,
=FF> ?
$strFF@ [
;FF[ \
publicGG 

constGG 
stringGG  
ConceptBinaryPaymentGG ,
=GG> ?
$strGG@ i
;GGi j
publicHH 

constHH 
stringHH #
ConceptModelFivePaymentHH /
=HH> ?
$strHH@ i
;HHi j
publicII 

constII 
stringII "
ConceptModelSixPaymentII .
=II> ?
$strII@ i
;IIi j
publicJJ 

constJJ 
stringJJ *
ConceptCommissionBinaryPaymentJJ 6
=JJ> ?
$strJJ@ m
;JJm n
publicKK 

constKK 
stringKK -
!ConceptCommissionModelFivePaymentKK 9
=KK> ?
$strKK@ m
;KKm n
publicLL 

constLL 
stringLL ,
 ConceptCommissionModelSixPaymentLL 8
=LL> ?
$strLL@ m
;LLm n
publicMM 

constMM 
stringMM !
DefaultWithdrawalZoneMM -
=MM> ?
$strMM@ _
;MM_ `
publicNN 

constNN 
stringNN 
SendFundsConceptNN (
=NN> ?
$strNN@ R
;NNR S
publicOO 

constOO 
intOO 
UsdtIdCurrencyOO &
=OO> ?
$numOO@ B
;OOB C
publicPP 

constPP 
intPP 
UsdtIdNetworkPP %
=PP> ?
$numPP@ B
;PPB C
publicQQ 

constQQ 
intQQ 
BnbIdNetworkQQ $
=QQ> ?
$numQQ@ B
;QQB C
publicRR 

constRR 
intRR 
CoinPayIdWalletRR '
=RR> ?
$numRR@ E
;RRE F
publicSS 

constSS 
intSS 
ChildrenLimitModel4SS +
=SS> ?
$numSS@ A
;SSA B
publicTT 

constTT 
intTT 
ChildrenLimitModel5TT +
=TT> ?
$numTT@ A
;TTA B
publicUU 

constUU 
intUU 
LevelsLimitModel5UU )
=UU> ?
$numUU@ A
;UUA B
publicVV 

constVV 
intVV 
ChildrenLimitModel6VV +
=VV> ?
$numVV@ A
;VVA B
publicWW 

constWW 
intWW 
LevelsLimitModel6WW )
=WW> ?
$numWW@ B
;WWB C
publicXX 

constXX 
stringXX '
GetAllDetailsModelOneAndTwoXX 3
=XX> ?
$strXX@ a
;XXa b
publicYY 

constYY 
stringYY ,
 DistributeCommissionsPerPurchaseYY 8
=YY> ?
$strYY@ f
;YYf g
public[[ 

static[[ 
int[[ 
[[[ 
][[ 
CustomerModel4Scope[[ +
=[[, -
{[[. /
$num[[0 1
,[[1 2
$num[[3 4
,[[4 5
$num[[6 7
}[[7 8
;[[8 9
public\\ 

const\\ 
int\\ 
CustomerModel5Scope\\ (
=\\, -
$num\\. /
;\\/ 0
public]] 

static]] 
int]] 
[]] 
]]] 
CustomerModel6Scope]] +
=]], -
{]]. /
$num]]0 1
,]]1 2
$num]]2 3
,]]3 4
$num]]4 5
,]]5 6
$num]]6 7
,]]7 8
$num]]8 9
,]]9 :
$num]]: <
,]]< =
$num]]> @
}]]A B
;]]B C
public__ 

const__ 
int__ 
	Ecosystem__ 
=__4 5
$num__6 7
;__7 8
public`` 

const`` 
int`` 
RecyCoin`` 
=``4 5
$num``6 7
;``7 8
publicaa 

constaa 
intaa 
	HouseCoinaa 
=aa4 5
$numaa6 7
;aa7 8
publicbb 

constbb 
intbb  
RecyCoinPaymentGroupbb )
=bb4 5
$numbb6 8
;bb8 9
publiccc 

constcc 
stringcc 
EcosystemSenderNamecc +
=cc4 5
$strcc6 S
;ccS T
publicdd 

constdd 
stringdd 
RecyCoinSenderNamedd *
=dd4 5
$strdd6 @
;dd@ A
publicee 

constee 
stringee 
HouseCoinSenderNameee +
=ee4 5
$stree6 B
;eeB C
publicff 

constff 
stringff !
ExitoJuntosSenderNameff -
=ff4 5
$strff6 D
;ffD E
publicgg 

constgg 
stringgg '
SubjectInvitationForAcademygg 3
=gg4 5
$strgg6 V
;ggV W
publichh 

consthh 
stringhh '
PurchasingPoolRevertBalancehh 3
=hh4 5
$strhh6 \
;hh\ ]
publicii 

constii 
stringii 
WireTransferii $
=ii4 5
$strii6 N
;iiN O
publicjj 

constjj 
stringjj &
GetTradingAcademyDetailsSpjj 2
=jj4 5
$strjj6 W
;jjW X
publickk 

constkk 
stringkk 
BalanceRefundskk &
=kk4 5
$strkk6 K
;kkK L
publicll 

constll 
stringll !
ServiceBalanceModel1All -
=ll4 5
$strll6 T
;llT U
publicmm 

constmm 
stringmm !
ServiceBalanceModel1Bmm -
=mm4 5
$strmm6 T
;mmT U
publicoo 

constoo 
intoo 
ForMonthoo  
=oo8 9
$numoo: <
;oo< =
publicpp 

constpp 
intpp 
ForWeekpp 
=pp8 9
$numpp: <
;pp< =
publicrr 

staticrr 
stringrr 
[rr 
]rr 
PartitionKeysrr (
=rr) *
{ss 	
$strss
 
,ss 
$strss 
,ss 
$strss 
,ss 
$strss 
,ss 
$strss !
,ss! "
$strss# &
,ss& '
$strss( +
,ss+ ,
$strss- 0
,ss0 1
$strss2 5
,ss5 6
$strss7 :
,ss: ;
$strss< @
,ss@ A
$strssB F
,ssF G
$strssH L
,ssL M
$strssN R
,ssR S
$strssT X
,ssX Y
$strssZ ^
}ss_ `
;ss` a
publicuu 

constuu 
stringuu 3
'DebitEcoPoolTransactionServiceSpModel1Auu ?
=uuB C
$struuD n
;uun o
publicvv 

constvv 
stringvv 4
(CreditEcoPoolTransactionServiceSpModel1Avv @
=vvB C
$strvvD o
;vvo p
publicww 

constww 
stringww 4
(CreditEcoPoolTransactionServiceSpModel1Bww @
=wwB C
$strwwD o
;wwo p
publicxx 

constxx 
stringxx 3
'DebitEcoPoolTransactionServiceSpModel1Bxx ?
=xxB C
$strxxD n
;xxn o
publicyy 

constyy 
stringyy 2
&DebitEcoPoolTransactionServiceSpModel2yy >
=yyB C
$stryyD m
;yym n
publiczz 

constzz 
stringzz 
PagaditoConnectKeyzz *
=zzB C
$strzzD f
;zzf g
public{{ 

const{{ 
string{{ 
PagaditoExecuteKey{{ *
={{B C
$str{{D f
;{{f g
public|| 

const|| 
string||  
PagaditoGetStatusKey|| ,
=||B C
$str||D f
;||f g
public}} 

const}} 
string}} 
PagaditoCurrency}} (
=}}B C
$str}}D I
;}}I J
public~~ 

const~~ 
string~~  
PagaditoFormatReturn~~ ,
=~~B C
$str~~D J
;~~J K
public 

const 
string 
RegisteredStatus (
=B C
$strD P
;P Q
public
ÄÄ 

const
ÄÄ 
int
ÄÄ "
RegisteredStatusCode
ÄÄ ,
=
ÄÄB C
$num
ÄÄD E
;
ÄÄE F
public
ÅÅ 

const
ÅÅ 
string
ÅÅ 
ExpiredStatus
ÅÅ %
=
ÅÅB C
$str
ÅÅD M
;
ÅÅM N
public
ÇÇ 

const
ÇÇ 
int
ÇÇ 
ExpiredStatusCode
ÇÇ )
=
ÇÇB C
-
ÇÇD E
$num
ÇÇE F
;
ÇÇF G
public
ÉÉ 

const
ÉÉ 
string
ÉÉ 
CompletedStatus
ÉÉ '
=
ÉÉB C
$str
ÉÉD O
;
ÉÉO P
public
ÑÑ 

const
ÑÑ 
int
ÑÑ !
CompletedStatusCode
ÑÑ +
=
ÑÑB C
$num
ÑÑD G
;
ÑÑG H
public
ÖÖ 

const
ÖÖ 
int
ÖÖ !
TradingAcademyGroup
ÖÖ +
=
ÖÖB C
$num
ÖÖD E
;
ÖÖE F
public
ÜÜ 

const
ÜÜ 
int
ÜÜ 
SuccessStatusCode
ÜÜ )
=
ÜÜB C
$num
ÜÜD E
;
ÜÜE F
public
áá 

const
áá 
int
áá &
CoinPayPendingStatusCode
áá 0
=
ááB C
$num
ááD E
;
ááE F
public
àà 

const
àà 
int
àà &
CoinPaySuccessStatusCode
àà 0
=
ààB C
$num
ààD E
;
ààE F
public
ââ 

const
ââ 
int
ââ &
CoinPayExpiredStatusCode
ââ 0
=
ââB C
$num
ââD E
;
ââE F
}ää à
TC:\HeroSystem\walletService\WalletService.Models\DTO\AffiliateBtc\AffiliateBtcDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #
AffiliateBtc# /
;/ 0
public 
class 
AffiliateBtcDto 
{ 
[ 
JsonPropertyName 
( 
$str 
) 
] 
public #
int$ '
Id( *
{+ ,
get- 0
;0 1
set2 5
;5 6
}7 8
[		 
JsonPropertyName		 
(		 
$str		 #
)		# $
]		$ %
public		& ,
int		- 0
AffiliateId		1 <
{		= >
get		? B
;		B C
set		D G
;		G H
}		I J
[

 
JsonPropertyName

 
(

 
$str

 
)

  
]

  !
public

" (
string

) /
Address

0 7
{

8 9
get

: =
;

= >
set

? B
;

B C
}

D E
=

F G
String

H N
.

N O
Empty

O T
;

T U
[ 
JsonPropertyName 
( 
$str 
) 
]  
public! '
byte( ,
Status- 3
{4 5
get6 9
;9 :
set; >
;> ?
}@ A
[ 
JsonPropertyName 
( 
$str !
)! "
]" #
public$ *
int+ .
	NetworkId/ 8
{9 :
get; >
;> ?
set@ C
;C D
}E F
[ 
JsonPropertyName 
( 
$str !
)! "
]" #
public$ *
DateTime+ 3
	CreatedAt4 =
{> ?
get@ C
;C D
setE H
;H I
}J K
[ 
JsonPropertyName 
( 
$str !
)! "
]" #
public$ *
DateTime+ 3
?3 4
	UpdatedAt5 >
{? @
getA D
;D E
setF I
;I J
}K L
[ 
JsonPropertyName 
( 
$str !
)! "
]" #
public$ *
DateTime+ 3
?3 4
	DeletedAt5 >
{? @
getA D
;D E
setF I
;I J
}K L
} ”
bC:\HeroSystem\walletService\WalletService.Models\DTO\AffiliateInformation\UserBinaryInformation.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." # 
AffiliateInformation# 7
;7 8
public 
class !
UserBinaryInformation "
{ 
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public		 

decimal		 

LeftVolume		 
{		 
get		  #
;		# $
set		% (
;		( )
}		* +
public

 

decimal

 
RightVolume

 
{

  
get

! $
;

$ %
set

& )
;

) *
}

+ ,
} 
public 
class 
UserBinaryResponse 
{ 
public 

bool 
success 
{ 
get 
; 
set "
;" #
}$ %
public 

List 
< !
UserBinaryInformation %
>% &
data' +
{, -
get. 1
;1 2
set3 6
;6 7
}8 9
public 

string 
message 
{ 
get 
;  
set! $
;$ %
}& '
public 

int 
code 
{ 
get 
; 
set 
; 
}  !
} ˜
[C:\HeroSystem\walletService\WalletService.Models\DTO\AffiliateInformation\UserStatistics.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." # 
AffiliateInformation# 7
;7 8
public 
class 
UserStatistics 
{ 
public 

int 
AmountPoolModel1A  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

int 
AmountPoolModel1B  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

int 
AmountPoolModel2 
{  !
get" %
;% &
set' *
;* +
}, -
public 

int 
AmountPoolModel3 
{  !
get" %
;% &
set' *
;* +
}, -
public		 

decimal		 
?		 
VolumeLeftModel4		 $
{		% &
get		' *
;		* +
set		, /
;		/ 0
}		1 2
public

 

decimal

 
?

 
VolumeRightModel4

 %
{

& '
get

( +
;

+ ,
set

- 0
;

0 1
}

2 3
public 

decimal $
AmountChildrenLeftModel4 +
{, -
get. 1
;1 2
set3 6
;6 7
}8 9
public 

decimal %
AmountChildrenRightModel4 ,
{- .
get/ 2
;2 3
set4 7
;7 8
}9 :
public 

decimal  
AmountPassiveModel1A '
{( )
get* -
;- .
set/ 2
;2 3
}4 5
public 

decimal !
AmountResidualModel1A (
{) *
get+ .
;. /
set0 3
;3 4
}5 6
public 

decimal  
AmountPassiveModel1B '
{( )
get* -
;- .
set/ 2
;2 3
}4 5
public 

decimal !
AmountResidualModel1B (
{) *
get+ .
;. /
set0 3
;3 4
}5 6
public 

decimal 
AmountPassiveModel2 &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
public 

decimal  
AmountResidualModel2 '
{( )
get* -
;- .
set/ 2
;2 3
}4 5
public 

decimal "
AmountPercentageModel3 )
{* +
get, /
;/ 0
set1 4
;4 5
}6 7
public 

decimal  
AmountResidualModel3 '
{( )
get* -
;- .
set/ 2
;2 3
}4 5
public 

int %
AmountUsersDirectModel123 (
{) *
get+ .
;. /
set0 3
;3 4
}5 6
public 

int &
AmountUsersNetworkModel123 )
{* +
get, /
;/ 0
set1 4
;4 5
}6 7
public 

int #
AmountUsersDirectModel5 &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
public 

int $
AmountUsersNetworkModel5 '
{( )
get* -
;- .
set/ 2
;2 3
}4 5
public 

int #
AmountUsersDirectModel6 &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
public 

int $
AmountUsersNetworkModel6 '
{( )
get* -
;- .
set/ 2
;2 3
}4 5
} †
cC:\HeroSystem\walletService\WalletService.Models\DTO\BalanceInformationDto\BalanceInformationDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #!
BalanceInformationDto# 8
;8 9
public 
class !
BalanceInformationDto "
{ 
public 

decimal 
? 
ReverseBalance "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 

decimal 
? 
TotalAcquisitions %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
public		 

decimal		 
?		 
AvailableBalance		 $
{		% &
get		' *
;		* +
set		, /
;		/ 0
}		1 2
public 

decimal 
?  
TotalCommissionsPaid (
{) *
get+ .
;. /
set0 3
;3 4
}5 6
public 

decimal 
? 
ServiceBalance "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 

decimal 
BonusAmount 
{  
get! $
;$ %
set& )
;) *
}+ ,
} ˜
NC:\HeroSystem\walletService\WalletService.Models\DTO\CoinPayDto\CurrencyDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #

CoinPayDto# -
;- .
public 
class 
CurrencyDto 
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

string 
? 
Name 
{ 
get 
; 
set "
;" #
}$ %
public 

string 
? 
Description 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

string 
? 
Code 
{ 
get 
; 
set "
;" #
}$ %
public		 

bool		 
IsErc		 
{		 
get		 
;		 
set		  
;		  !
}		" #
}

 À
OC:\HeroSystem\walletService\WalletService.Models\DTO\CoinPayDto\SendFundsDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #

CoinPayDto# -
;- .
public 
class 
SendFundsDto 
{ 
public 

List 
< 
SendFundsResponse !
>! "
SuccessfulResponses# 6
{7 8
get9 <
;< =
set> A
;A B
}C D
=E F
newG J
ListK O
<O P
SendFundsResponseP a
>a b
(b c
)c d
;d e
public 

List 
< 
SendFundsResponse !
>! "
FailedResponses# 2
{3 4
get5 8
;8 9
set: =
;= >
}? @
=A B
newC F
ListG K
<K L
SendFundsResponseL ]
>] ^
(^ _
)_ `
;` a
}		 ï

QC:\HeroSystem\walletService\WalletService.Models\DTO\CoinPayDto\UserBalanceDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #

CoinPayDto# -
;- .
public 
class 
UserBalanceDto 
{ 
public 

double 
Balance 
{ 
get 
;  
set! $
;$ %
}& '
public 

int 
BlockedBalance 
{ 
get  #
;# $
set% (
;( )
}* +
public 

double 
CurrentBalance  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

CurrencyDto 
? 
Currency  
{! "
get# &
;& '
set( +
;+ ,
}- .
public		 

bool		 
IsActive		 
{		 
get		 
;		 
set		  #
;		# $
}		% &
public

 

bool

 
Locked

 
{

 
get

 
;

 
set

 !
;

! "
}

# $
} Æ
PC:\HeroSystem\walletService\WalletService.Models\DTO\CoinPayDto\WithdrawalDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #

CoinPayDto# -
;- .
public 
class 
WithdrawalDto 
{ 
public		 

ServicesResponse		 
?		 
Response		 %
{		& '
get		( +
;		+ ,
set		- 0
;		0 1
}		2 3
public

 

WithdrawalStatus

 
Status

 "
{

# $
get

% (
;

( )
set

* -
;

- .
}

/ 0
} £
gC:\HeroSystem\walletService\WalletService.Models\DTO\CoinPaymentTransactionDto\PaymentTransactionDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #%
CoinPaymentTransactionDto# <
;< =
public 
class !
PaymentTransactionDto "
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

string 
IdTransaction 
{  !
get" %
;% &
set' *
;* +
}, -
=. /
string0 6
.6 7
Empty7 <
;< =
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

decimal 
Amount 
{ 
get 
;  
set! $
;$ %
}& '
public		 

decimal		 
AmountReceived		 !
{		" #
get		$ '
;		' (
set		) ,
;		, -
}		. /
public

 

string

 
Products

 
{

 
get

  
;

  !
set

" %
;

% &
}

' (
=

( )
string

* 0
.

0 1
Empty

1 6
;

6 7
public 

int 
Status 
{ 
get 
; 
set  
;  !
}" #
public 

bool 
	Acredited 
{ 
get 
;  
set! $
;$ %
}& '
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
set% (
;( )
}* +
public 

DateTime 
	UpdatedAt 
{ 
get  #
;# $
set% (
;( )
}* +
public 

DateTime 
? 
	DeletedAt 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

string 
? 
PaymentMethod  
{! "
get# &
;& '
set( +
;+ ,
}- .
} ï
QC:\HeroSystem\walletService\WalletService.Models\DTO\EcoPoolDto\UserEcoPoolDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #

EcoPoolDto# -
;- .
public 
class 
UserEcoPoolDto 
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

string 
UserName 
{ 
get  
;  !
set" %
;% &
}' (
=) *
null+ /
!/ 0
;0 1
public 

string 
Email 
{ 
get 
; 
set "
;" #
}$ %
=& '
null( ,
!, -
;- .
public 

byte 
AffiliateMode 
{ 
get  #
;# $
set% (
;( )
}* +
public		 

string		 
?		 
Name		 
{		 
get		 
;		 
set		 "
;		" #
}		$ %
public

 

string

 
?

 
LastName

 
{

 
get

 !
;

! "
set

# &
;

& '
}

( )
public 

int 
Father 
{ 
get 
; 
set  
;  !
}" #
public 

int 
Sponsor 
{ 
get 
; 
set !
;! "
}# $
public 

int 
BinarySponsor 
{ 
get "
;" #
set$ '
;' (
}) *
public 

byte 
BinaryMatrixSide  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

int 
ExternalGradingId  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

bool 
IsForcedComplete  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

bool 
IsBinaryEvaluated !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 

int 
? 
ExternalProductId !
{" #
get$ '
;' (
set) ,
;, -
}. /
} ûF
MC:\HeroSystem\walletService\WalletService.Models\DTO\GradingDto\GradingDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #

GradingDto# -
;- .
public 
class 

GradingDto 
{ 
[ 
JsonPropertyName 
( 
$str 
) 
] 
public #
int$ '
Id( *
{+ ,
get- 0
;0 1
set2 5
;5 6
}7 8
[		 
JsonPropertyName		 
(		 
$str		 
)		 
]		 
public		 %
string		& ,
Name		- 1
{		2 3
get		4 7
;		7 8
set		9 <
;		< =
}		> ?
=		@ A
null		B F
!		F G
;		G H
[ 
JsonPropertyName 
( 
$str #
)# $
]$ %
public& ,
string- 3
?3 4
Description5 @
{A B
getC F
;F G
setH K
;K L
}M N
[ 
JsonPropertyName 
( 
$str #
)# $
]$ %
public& ,
int- 0

ScopeLevel1 ;
{< =
get> A
;A B
setC F
;F G
}H I
[ 
JsonPropertyName 
( 
$str #
)# $
]$ %
public& ,
bool- 1

IsInfinity2 <
{= >
get? B
;B C
setD G
;G H
}I J
[ 
JsonPropertyName 
( 
$str *
)* +
]+ ,
public 

decimal 
PersonalPurchases $
{% &
get' *
;* +
set, /
;/ 0
}1 2
[ 
JsonPropertyName 
( 
$str 0
)0 1
]1 2
public 

bool "
PersonalPurchasesExact &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
[ 
JsonPropertyName 
( 
$str )
)) *
]* +
public 

decimal 
PurchasesNetwork #
{$ %
get& )
;) *
set+ .
;. /
}0 1
[ 
JsonPropertyName 
( 
$str %
)% &
]& '
public( .
decimal/ 6
BinaryVolume7 C
{D E
getF I
;I J
setK N
;N O
}P Q
[ 
JsonPropertyName 
( 
$str %
)% &
]& '
public( .
int/ 2
VolumePoints3 ?
{@ A
getB E
;E F
setG J
;J K
}L M
[ 
JsonPropertyName 
( 
$str -
)- .
]. /
public 

int 
VolumePointsNetwork "
{# $
get% (
;( )
set* -
;- .
}/ 0
[!! 
JsonPropertyName!! 
(!! 
$str!! )
)!!) *
]!!* +
public"" 

int"" 
ChildrenLeftLeg"" 
{""  
get""! $
;""$ %
set""& )
;"") *
}""+ ,
[$$ 
JsonPropertyName$$ 
($$ 
$str$$ *
)$$* +
]$$+ ,
public%% 

int%% 
ChildrenRightLeg%% 
{%%  !
get%%" %
;%%% &
set%%' *
;%%* +
}%%, -
['' 
JsonPropertyName'' 
('' 
$str'' '
)''' (
]''( )
public''* 0
int''1 4
FrontByMatrix''5 B
{''C D
get''E H
;''H I
set''J M
;''M N
}''O P
[)) 
JsonPropertyName)) 
()) 
$str)) &
)))& '
]))' (
public))) /
int))0 3
FrontQualif1))4 @
{))A B
get))C F
;))F G
set))H K
;))K L
}))M N
[++ 
JsonPropertyName++ 
(++ 
$str++ %
)++% &
]++& '
public++( .
int++/ 2
FrontScore1++3 >
{++? @
get++A D
;++D E
set++F I
;++I J
}++K L
[-- 
JsonPropertyName-- 
(-- 
$str-- &
)--& '
]--' (
public--) /
int--0 3
FrontQualif2--4 @
{--A B
get--C F
;--F G
set--H K
;--K L
}--M N
[// 
JsonPropertyName// 
(// 
$str// %
)//% &
]//& '
public//( .
int/// 2
FrontScore2//3 >
{//? @
get//A D
;//D E
set//F I
;//I J
}//K L
[11 
JsonPropertyName11 
(11 
$str11 &
)11& '
]11' (
public11) /
int110 3
FrontQualif3114 @
{11A B
get11C F
;11F G
set11H K
;11K L
}11M N
[33 
JsonPropertyName33 
(33 
$str33 %
)33% &
]33& '
public33( .
int33/ 2
FrontScore3333 >
{33? @
get33A D
;33D E
set33F I
;33I J
}33K L
[55 
JsonPropertyName55 
(55 
$str55 +
)55+ ,
]55, -
public66 

bool66 
ExactFrontRatings66 !
{66" #
get66$ '
;66' (
set66) ,
;66, -
}66. /
[88 
JsonPropertyName88 
(88 
$str88 (
)88( )
]88) *
public88+ 1
int882 5
LeaderByMatrix886 D
{88E F
get88G J
;88J K
set88L O
;88O P
}88Q R
[:: 
JsonPropertyName:: 
(:: 
$str:: '
)::' (
]::( )
public::* 0
int::1 4
?::4 5
NetworkLeaders::6 D
{::E F
get::G J
;::J K
set::L O
;::O P
}::Q R
[<< 
JsonPropertyName<< 
(<< 
$str<< 1
)<<1 2
]<<2 3
public== 

int== 
?== #
NetworkLeadersQualifier== '
{==( )
get==* -
;==- .
set==/ 2
;==2 3
}==4 5
[?? 
JsonPropertyName?? 
(?? 
$str??  
)??  !
]??! "
public??# )
int??* -
???- .
Products??/ 7
{??8 9
get??: =
;??= >
set??? B
;??B C
}??D E
[AA 
JsonPropertyNameAA 
(AA 
$strAA $
)AA$ %
]AA% &
publicAA' -
intAA. 1
?AA1 2
AffiliationsAA3 ?
{AA@ A
getAAB E
;AAE F
setAAG J
;AAJ K
}AAL M
[CC 
JsonPropertyNameCC 
(CC 
$strCC !
)CC! "
]CC" #
publicCC$ *
boolCC+ /
HaveBothCC0 8
{CC9 :
getCC; >
;CC> ?
setCC@ C
;CCC D
}CCE F
[EE 
JsonPropertyNameEE 
(EE 
$strEE (
)EE( )
]EE) *
publicEE+ 1
intEE2 5
ActivateUserByEE6 D
{EEE F
getEEG J
;EEJ K
setEEL O
;EEO P
}EEQ R
[GG 
JsonPropertyNameGG 
(GG 
$strGG 
)GG 
]GG  
publicGG! '
intGG( +
ActiveGG, 2
{GG3 4
getGG5 8
;GG8 9
setGG: =
;GG= >
}GG? @
[II 
JsonPropertyNameII 
(II 
$strII 
)II 
]II  
publicII! '
boolII( ,
StatusII- 3
{II4 5
getII6 9
;II9 :
setII; >
;II> ?
}II@ A
[KK 
JsonPropertyNameKK 
(KK 
$strKK +
)KK+ ,
]KK, -
publicLL 

intLL 
NetworkScopeLevelLL  
{LL! "
getLL# &
;LL& '
setLL( +
;LL+ ,
}LL- .
[NN 
JsonPropertyNameNN 
(NN 
$strNN #
)NN# $
]NN$ %
publicNN& ,
boolNN- 1

FullPeriodNN2 <
{NN= >
getNN? B
;NNB C
setNND G
;NNG H
}NNI J
}OO ´$
YC:\HeroSystem\walletService\WalletService.Models\DTO\InvoiceDetailDto\InvoiceDetailDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #
InvoiceDetailDto# 3
;3 4
public 
class 
InvoiceDetailDto 
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

int 
	InvoiceId 
{ 
get 
; 
set  #
;# $
}% &
public 

int 
	ProductId 
{ 
get 
; 
set  #
;# $
}% &
public 

int 
PaymentGroupId 
{ 
get  #
;# $
set% (
;( )
}* +
public		 

bool		 
AccumMinPurchase		  
{		! "
get		# &
;		& '
set		( +
;		+ ,
}		- .
public

 

string

 
?

 
ProductName

 
{

  
get

! $
;

$ %
set

& )
;

) *
}

+ ,
public 

decimal 
ProductPrice 
{  !
get" %
;% &
set' *
;* +
}, -
public 

decimal 
ProductPriceBtc "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 

decimal 
? 

ProductIva 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

int 
ProductQuantity 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

decimal 
? !
ProductCommissionable )
{* +
get, /
;/ 0
set1 4
;4 5
}6 7
public 

decimal 
BinaryPoints 
{  !
get" %
;% &
set' *
;* +
}, -
public 

int 
? 
ProductPoints 
{ 
get  #
;# $
set% (
;( )
}* +
public 

decimal 
ProductDiscount "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 

DateTime 
Date 
{ 
get 
; 
set  #
;# $
}% &
public 

int 
CombinationId 
{ 
get "
;" #
set$ '
;' (
}) *
public 

bool 
ProductPack 
{ 
get !
;! "
set# &
;& '
}( )
public 

decimal 
? 

BaseAmount 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

decimal 
? 
DailyPercentage #
{$ %
get& )
;) *
set+ .
;. /
}0 1
public 

int 
? 
WaitingDays 
{ 
get !
;! "
set# &
;& '
}( )
public 

int 
DaysToPayQuantity  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

bool 
ProductStart 
{ 
get "
;" #
set$ '
;' (
}) *
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
set% (
;( )
}* +
public 

DateTime 
? 
	UpdatedAt 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

DateTime 
? 
	DeletedAt 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

virtual 

InvoiceDTO 
Invoice %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
}   è&
SC:\HeroSystem\walletService\WalletService.Models\DTO\InvoiceDetailDto\InvoiceDTO.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #
InvoiceDetailDto# 3
;3 4
public 
class 

InvoiceDTO 
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

int 
InvoiceNumber 
{ 
get "
;" #
set$ '
;' (
}) *
public 

int 
PurchaseOrderId 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public		 

decimal		 
?		 
TotalInvoice		  
{		! "
get		# &
;		& '
set		( +
;		+ ,
}		- .
public

 

decimal

 
TotalInvoiceBtc

 "
{

# $
get

% (
;

( )
set

* -
;

- .
}

/ 0
public 

decimal 
? 
TotalCommissionable '
{( )
get* -
;- .
set/ 2
;2 3
}4 5
public 

int 
? 
TotalPoints 
{ 
get !
;! "
set# &
;& '
}( )
public 

bool 
State 
{ 
get 
; 
set  
;  !
}" #
public 

bool 
Status 
{ 
get 
; 
set !
;! "
}# $
public 

DateTime 
? 
Date 
{ 
get 
;  
set! $
;$ %
}& '
public 

DateTime 
? 
CancellationDate %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
public 

string 
? 
PaymentMethod  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

string 
? 
Bank 
{ 
get 
; 
set "
;" #
}$ %
public 

string 
? 
ReceiptNumber  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

DateTime 
? 
DepositDate  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

bool 
? 
Type 
{ 
get 
; 
set  
;  !
}" #
public 

string 
? 
Reason 
{ 
get 
;  
set! $
;$ %
}& '
public 

string 
? 
InvoiceData 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

string 
? 
InvoiceAddress !
{" #
get$ '
;' (
set) ,
;, -
}- .
public 

string 
? 
ShippingAddress "
{# $
get% (
;( )
set) ,
;, -
}- .
public 

string 
? 
	SecretKey 
{ 
get  
;  !
set! $
;$ %
}% &
public 

string 
? 

BtcAddress 
{ 
get !
;! "
set" %
;% &
}& '
public 

int 
	Recurring 
{ 
get 
; 
set  
;  !
}! "
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
set% (
;( )
}* +
public 

DateTime 
? 
	UpdatedAt 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

DateTime 
? 
	DeletedAt 
{  
get! $
;$ %
set& )
;) *
}+ ,
}!! ë,
MC:\HeroSystem\walletService\WalletService.Models\DTO\InvoiceDto\InvoiceDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #

InvoiceDto# -
;- .
public 
class 

InvoiceDto 
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

int 
InvoiceNumber 
{ 
get "
;" #
set$ '
;' (
}) *
public 

int 
PurchaseOrderId 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public		 

decimal		 
?		 
TotalInvoice		  
{		! "
get		# &
;		& '
set		( +
;		+ ,
}		- .
public

 

decimal

 
TotalInvoiceBtc

 "
{

# $
get

% (
;

( )
set

* -
;

- .
}

/ 0
public 

decimal 
? 
TotalCommissionable '
{( )
get* -
;- .
set/ 2
;2 3
}4 5
public 

int 
? 
TotalPoints 
{ 
get !
;! "
set# &
;& '
}( )
public 

bool 
State 
{ 
get 
; 
set  
;  !
}" #
public 

bool 
Status 
{ 
get 
; 
set !
;! "
}# $
public 

DateTime 
? 
Date 
{ 
get 
;  
set! $
;$ %
}& '
public 

DateTime 
? 
CancellationDate %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
public 

string 
? 
PaymentMethod  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

string 
? 
Bank 
{ 
get 
; 
set "
;" #
}$ %
public 

string 
? 
ReceiptNumber  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

DateTime 
? 
DepositDate  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

bool 
? 
Type 
{ 
get 
; 
set  
;  !
}" #
public 

string 
? 
Reason 
{ 
get 
;  
set! $
;$ %
}& '
public 

string 
? 
InvoiceData 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

string 
? 
InvoiceAddress !
{" #
get$ '
;' (
set) ,
;, -
}- .
public 

string 
? 
ShippingAddress "
{# $
get% (
;( )
set) ,
;, -
}- .
public 

string 
? 
	SecretKey 
{ 
get  
;  !
set! $
;$ %
}% &
public 

string 
? 

BtcAddress 
{ 
get !
;! "
set" %
;% &
}& '
public 

int 
	Recurring 
{ 
get 
; 
set  
;  !
}! "
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
set% (
;( )
}* +
public 

DateTime 
? 
	UpdatedAt 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

DateTime 
? 
	DeletedAt 
{  
get! $
;$ %
set& )
;) *
}+ ,
public!! 

string!! 
?!! 
UserName!! 
{!! 
get!! !
;!!! "
set!!# &
;!!& '
}!!( )
public"" 

string"" 
?"" 
Name"" 
{"" 
get"" 
;"" 
set"" "
;""" #
}""$ %
public## 

string## 
?## 
LastName## 
{## 
get## !
;##! "
set### &
;##& '
}##( )
public%% 

ICollection%% 
<%% 
InvoiceDetailDto%% '
.%%' (
InvoiceDetailDto%%( 8
>%%8 9
InvoicesDetails%%: I
{%%J K
get%%L O
;%%O P
set%%Q T
;%%T U
}%%V W
}&& ≈
[C:\HeroSystem\walletService\WalletService.Models\DTO\InvoiceDto\InvoiceModelOneAndTwoDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #

InvoiceDto# -
;- .
public 
class $
InvoiceModelOneAndTwoDto %
{ 
public 

string 
UserName 
{# $
get% (
;( )
set* -
;- .
}/ 0
=1 2
string3 9
.9 :
Empty: ?
;? @
public 

int 
	InvoiceId 
{# $
get% (
;( )
set* -
;- .
}/ 0
public 

string 
ProductName 
{# $
get% (
;( )
set* -
;- .
}/ 0
=1 2
string3 9
.9 :
Empty: ?
;? @
public 

decimal 

BaseAmount 
{! "
get# &
;& '
set( +
;+ ,
}- .
public		 

int		 
PaymentGroupId		 "
{		# $
get		% (
;		( )
set		* -
;		- .
}		/ 0
public

 

DateTime

 
	CreatedAt

 
{

# $
get

% (
;

( )
set

* -
;

- .
}

/ 0
} Ω
SC:\HeroSystem\walletService\WalletService.Models\DTO\InvoiceDto\InvoiceResultDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #

InvoiceDto# -
;- .
public 
class 
InvoiceResultDto 
{ 
public 

byte 
[ 
] 
? 

PdfContent 
{ 
get  #
;# $
set% (
;( )
}* +
public 

long 
BrandId 
{ 
get 
; 
set "
;" #
}$ %
} †
[C:\HeroSystem\walletService\WalletService.Models\DTO\InvoiceDto\InvoiceTradingAcademyDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #

InvoiceDto# -
;- .
public 
class $
InvoiceTradingAcademyDto %
{ 
public 

int 
	ProductId 
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

string 
UserName 
{! "
get# &
;& '
set( +
;+ ,
}- .
=/ 0
string1 7
.7 8
Empty8 =
;= >
public 

int 
	InvoiceId 
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

string 
ProductName 
{! "
get# &
;& '
set( +
;+ ,
}- .
=/ 0
string1 7
.7 8
Empty8 =
;= >
public		 

decimal		 
ProductPrice		  
{		! "
get		# &
;		& '
set		( +
;		+ ,
}		- .
public

 

DateTime

 
	CreatedAt

 
{

! "
get

# &
;

& '
set

( +
;

+ ,
}

- .
public 

string 
StartDay 
{! "
get# &
;& '
set( +
;+ ,
}- .
=/ 0
string1 7
.7 8
Empty8 =
;= >
public 

string 
EndDay 
{! "
get# &
;& '
set( +
;+ ,
}- .
=/ 0
string1 7
.7 8
Empty8 =
;= >
} †
^C:\HeroSystem\walletService\WalletService.Models\DTO\InvoiceDto\ModelBalancesAndInvoicesDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #

InvoiceDto# -
;- .
public 
class '
ModelBalancesAndInvoicesDto (
{ 
public 

string 
UserName 
{! "
get# &
;& '
set( +
;+ ,
}- .
=/ 0
string1 7
.7 8
Empty8 =
;= >
public 

decimal 
Model1AAmount  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

decimal 
Model1BAmount  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

decimal 
Model2Amount 
{! "
get# &
;& '
set( +
;+ ,
}- .
public		 

long		 
[		 
]		 
	InvoiceId		 
{		" #
get		$ '
;		' (
set		) ,
;		, -
}		. /
=		0 1
Array		2 7
.		7 8
Empty		8 =
<		= >
long		> B
>		B C
(		C D
)		D E
;		E F
}

 ∏
XC:\HeroSystem\walletService\WalletService.Models\DTO\LeaderBoardDto\LeaderBoardModel4.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #
LeaderBoardDto# 1
;1 2
public 
class 
LeaderBoardModel4 
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

int 
? 
FatherModel4 
{ 
get "
;" #
set$ '
;' (
}) *
public 

int 
Level 
{ 
get 
; 
set 
;  
}! "
public		 

DateTime		 
UserCreatedAt		 !
{		" #
get		$ '
;		' (
set		) ,
;		, -
}		. /
public

 

DateTime

 
	CreatedAt

 
{

 
get

  #
;

# $
set

% (
;

( )
}

* +
public 

decimal 
Amount 
{ 
get 
;  
set! $
;$ %
}& '
public 

decimal 
Points 
{ 
get 
;  
set! $
;$ %
}& '
public 

string 
UserName 
{ 
get  
;  !
set" %
;% &
}' (
public 

int 
	GradingId 
{ 
get 
; 
set  #
;# $
}% &
public 

int 
	PositionX 
{ 
get 
; 
set  #
;# $
}% &
public 

int 
	PositionY 
{ 
get 
; 
set  #
;# $
}% &
} ù
XC:\HeroSystem\walletService\WalletService.Models\DTO\LeaderBoardDto\LeaderBoardModel5.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #
LeaderBoardDto# 1
;1 2
public 
class 
LeaderBoardModel5 
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

int 
? 
FatherModel5 
{ 
get "
;" #
set$ '
;' (
}) *
public 

int 
Level 
{ 
get 
; 
set 
;  
}! "
public		 

DateTime		 
UserCreatedAt		 !
{		" #
get		$ '
;		' (
set		) ,
;		, -
}		. /
public

 

DateTime

 
	CreatedAt

 
{

 
get

  #
;

# $
set

% (
;

( )
}

* +
public 

decimal 
Amount 
{ 
get 
;  
set! $
;$ %
}& '
public 

string 
UserName 
{ 
get  
;  !
set" %
;% &
}' (
public 

int 
	GradingId 
{ 
get 
; 
set  #
;# $
}% &
public 

int 
	PositionX 
{ 
get 
; 
set  #
;# $
}% &
public 

int 
	PositionY 
{ 
get 
; 
set  #
;# $
}% &
} ª
XC:\HeroSystem\walletService\WalletService.Models\DTO\LeaderBoardDto\LeaderBoardModel6.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #
LeaderBoardDto# 1
;1 2
public 
class 
LeaderBoardModel6 
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

int 
? 
FatherModel6 
{ 
get "
;" #
set$ '
;' (
}) *
public 

int 
Level 
{ 
get 
; 
set 
;  
}! "
public		 

DateTime		 
UserCreatedAt		 !
{		" #
get		$ '
;		' (
set		) ,
;		, -
}		. /
public

 

DateTime

 
	CreatedAt

 
{

 
get

  #
;

# $
set

% (
;

( )
}

* +
public 

decimal 
Amount 
{ 
get 
;  
set! $
;$ %
}& '
public 

string 
UserName 
{ 
get  
;  !
set" %
;% &
}' (
public 

int 
	GradingId 
{ 
get 
; 
set  #
;# $
}% &
public 

int 
	PositionX 
{ 
get 
; 
set  #
;# $
}% &
public 

int 
	PositionY 
{ 
get 
; 
set  #
;# $
}% &
public 

int 
ChildrenCount 
{ 
get "
;" #
set$ '
;' (
}) *
} ¯
SC:\HeroSystem\walletService\WalletService.Models\DTO\PaginationDto\PaginationDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #
PaginationDto# 0
;0 1
public 
class 
PaginationDto 
< 
T 
> 
{ 
public 

int 
CurrentPage 
{ 
get  
;  !
set" %
;% &
}' (
public 

int 

TotalPages 
{ 
get 
;  
set! $
;$ %
}& '
public 

int 
PageSize 
{ 
get 
; 
set "
;" #
}$ %
public 

int 

TotalCount 
{ 
get 
;  
set! $
;$ %
}& '
public		 

List		 
<		 
T		 
>		 
?		 
Items		 
{		 
get		 
;		  
set		! $
;		$ %
}		& '
public 

bool 
HasPrevious 
=> 
CurrentPage *
>+ ,
$num- .
;. /
public 

bool 
HasNext 
=> 
CurrentPage &
<' (

TotalPages) 3
;3 4
} õ
cC:\HeroSystem\walletService\WalletService.Models\DTO\PaymentTransactionDto\PaymentTransactionDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #!
PaymentTransactionDto# 8
;8 9
public 
class !
PaymentTransactionDto "
{ 
public 

long 
Id 
{ 
get 
; 
set 
; 
}  
public 

string 
? 
UserName 
{ 
get !
;! "
set# &
;& '
}( )
=* +
string, 2
.2 3
Empty3 8
;8 9
public 

string 
IdTransaction 
{  !
get" %
;% &
set' *
;* +
}, -
=. /
string0 6
.6 7
Empty7 <
;< =
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public		 

decimal		 
Amount		 
{		 
get		 
;		  
set		! $
;		$ %
}		& '
public

 

decimal

 
AmountReceived

 !
{

" #
get

$ '
;

' (
set

) ,
;

, -
}

. /
public 

string 
Products 
{ 
get  
;  !
set" %
;% &
}' (
=) *
string+ 1
.1 2
Empty2 7
;7 8
public 

int 
Status 
{ 
get 
; 
set  
;  !
}" #
public 

bool 
	Acredited 
{ 
get 
;  
set! $
;$ %
}& '
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
set% (
;( )
}* +
public 

DateTime 
	UpdatedAt 
{ 
get  #
;# $
set% (
;( )
}* +
public 

DateTime 
? 
	DeletedAt 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

string 
? 
PaymentMethod  
{! "
get# &
;& '
set( +
;+ ,
}- .
} ˘
aC:\HeroSystem\walletService\WalletService.Models\DTO\ProcessGradingDto\EcoPoolConfigurationDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #
ProcessGradingDto# 4
;4 5
public 
class !
ModelConfigurationDto "
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

double 
CompanyPercentage #
{$ %
get& )
;) *
set+ .
;. /
}0 1
public 

double #
CompanyPercentageLevels )
{* +
get, /
;/ 0
set1 4
;4 5
}6 7
public 

double 
ModelPercentage !
{" #
get$ '
;' (
set) ,
;, -
}. /
public		 

double		 
MaxGainLimit		 
{		  
get		! $
;		$ %
set		& )
;		) *
}		+ ,
public

 

DateTime

 
DateInit

 
{

 
get

 "
;

" #
set

$ '
;

' (
}

) *
public 

DateTime 
DateEnd 
{ 
get !
;! "
set# &
;& '
}( )
public 

int 
Case 
{ 
get 
; 
set 
; 
}  !
public 

int 
? 
	Processed 
{ 
get 
;  
set! $
;$ %
}& '
public 

int 
? 
Totals 
{ 
get 
; 
set !
;! "
}# $
public 

string 
	ModelType 
{ 
get !
;! "
set# &
;& '
}( )
public 

DateTime 
? 
CompletedAt  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
set% (
;( )
}* +
public 

DateTime 
	UpdatedAt 
{ 
get  #
;# $
set% (
;( )
}* +
public 

DateTime 
? 
	DeletedAt 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

ICollection 
< 
EcoPoolLevelDto &
>& '$
ModelConfigurationLevels( @
{A B
getC F
;F G
setH K
;K L
}M N
=O P
newQ T
ListU Y
<Y Z
EcoPoolLevelDtoZ i
>i j
(j k
)k l
;l m
} ì

YC:\HeroSystem\walletService\WalletService.Models\DTO\ProcessGradingDto\EcoPoolLevelDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #
ProcessGradingDto# 4
;4 5
public 
class 
EcoPoolLevelDto 
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

int "
EcoPoolConfigurationId %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
public 

int 
Level 
{ 
get 
; 
set 
;  
}! "
public 

double 

Percentage 
{ 
get "
;" #
set$ '
;' (
}) *
public		 

DateTime		 
	CreatedAt		 
{		 
get		  #
;		# $
set		% (
;		( )
}		* +
public

 

DateTime

 
	UpdatedAt

 
{

 
get

  #
;

# $
set

% (
;

( )
}

* +
} Æ&
YC:\HeroSystem\walletService\WalletService.Models\DTO\ProductWalletDto\ProductWalletDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #
ProductWalletDto# 3
;3 4
public 
class 
ProductWalletDto 
{ 
[ 
JsonPropertyName 
( 
$str 
) 
] 
public #
int$ '
Id( *
{+ ,
get- 0
;0 1
set2 5
;5 6
}7 8
[ 
JsonPropertyName 
( 
$str "
)" #
]# $
public% +
int, /

CategoryId0 :
{; <
get= @
;@ A
setB E
;E F
}G H
[		 
JsonPropertyName		 
(		 
$str		 !
)		! "
]		" #
public		$ *
decimal		+ 2
	SalePrice		3 <
{		= >
get		? B
;		B C
set		D G
;		G H
}		I J
[

 
JsonPropertyName

 
(

 
$str

 +
)

+ ,
]

, -
public

. 4
decimal

5 <
CommissionableValue

= P
{

Q R
get

S V
;

V W
set

X [
;

[ \
}

] ^
[ 
JsonPropertyName 
( 
$str $
)$ %
]% &
public' -
decimal. 5
BinaryPoints6 B
{C D
getE H
;H I
setJ M
;M N
}O P
[ 
JsonPropertyName 
( 
$str #
)# $
]$ %
public& ,
int- 0
ValuePoints1 <
{= >
get? B
;B C
setD G
;G H
}I J
[ 
JsonPropertyName 
( 
$str 
) 
] 
public $
decimal% ,
Tax- 0
{1 2
get3 6
;6 7
set8 ;
;; <
}= >
[ 
JsonPropertyName 
( 
$str *
)* +
]+ ,
public- 3
decimal4 ;
?; <
ModelTwoPercentage= O
{P Q
getR U
;U V
setW Z
;Z [
}\ ]
[ 
JsonPropertyName 
( 
$str $
)$ %
]% &
public' -
int. 1
PaymentGroup2 >
{? @
getA D
;D E
setF I
;I J
}K L
[ 
JsonPropertyName 
( 
$str #
)# $
]$ %
public& ,
bool- 1
AcumCompMin2 =
{> ?
get@ C
;C D
setE H
;H I
}J K
[ 
JsonPropertyName 
( 
$str #
)# $
]$ %
public& ,
bool- 1
ProductType2 =
{> ?
get@ C
;C D
setE H
;H I
}J K
[ 
JsonPropertyName 
( 
$str $
)$ %
]% &
public' -
bool. 2
ProductPacks3 ?
{@ A
getB E
;E F
setG J
;J K
}L M
[ 
JsonPropertyName 
( 
$str "
)" #
]# $
public% +
decimal, 3

BaseAmount4 >
{? @
getA D
;D E
setF I
;I J
}K L
[ 
JsonPropertyName 
( 
$str '
)' (
]( )
public* 0
decimal1 8
DailyPercentage9 H
{I J
getK N
;N O
setP S
;S T
}U V
[ 
JsonPropertyName 
( 
$str  
)  !
]! "
public# )
int* -
DaysWait. 6
{7 8
get9 <
;< =
set> A
;A B
}C D
[ 
JsonPropertyName 
( 
$str 
) 
] 
public %
string& ,
?, -
Name. 2
{3 4
get5 8
;8 9
set: =
;= >
}? @
[ 
JsonPropertyName 
( 
$str '
)' (
]( )
public* 0
decimal1 8
ProductDiscount9 H
{I J
getK N
;N O
setP S
;S T
}U V
} à
eC:\HeroSystem\walletService\WalletService.Models\DTO\ResultEcoPoolLevelsDto\ResultEcoPoolLevelsDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #"
ResultEcoPoolLevelsDto# 9
;9 :
public 
class "
ResultEcoPoolLevelsDto #
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

int 
ResultEcoPoolId 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

string 
AffiliateName 
{  !
get" %
;% &
set' *
;* +
}, -
=. /
string0 6
.6 7
Empty7 <
;< =
public		 

int		 
Level		 
{		 
get		 
;		 
set		 
;		  
}		! "
public

 

decimal

 
PercentageLevel

 "
{

# $
get

% (
;

( )
set

* -
;

- .
}

/ 0
public 

decimal "
CompanyPercentageLevel )
{* +
get, /
;/ 0
set1 4
;4 5
}6 7
public 

decimal 
CompanyAmountLevel %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
public 

decimal 
CommisionAmount "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 

decimal 
PaymentAmount  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

virtual 
ResultsEcoPoolDto $
.$ %
ResultsEcoPoolDto% 6
ResultsEcoPoolDto7 H
{I J
getK N
;N O
setP S
;S T
}U V
} ö"
[C:\HeroSystem\walletService\WalletService.Models\DTO\ResultsEcoPoolDto\ResultsEcoPoolDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #
ResultsEcoPoolDto# 4
;4 5
public 
class 
ResultsEcoPoolDto 
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

int "
EcoPoolConfigurationId %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
public 

int 
ProductExternalId  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public		 

string		 
AffiliateName		 
{		  !
get		" %
;		% &
set		' *
;		* +
}		, -
=		. /
string		0 6
.		6 7
Empty		7 <
;		< =
public

 

string

 
ProductName

 
{

 
get

  #
;

# $
set

% (
;

( )
}

* +
=

, -
string

. 4
.

4 5
Empty

5 :
;

: ;
public 

DateTime 
PaymentDate 
{  !
get" %
;% &
set' *
;* +
}, -
public 

DateTime 
LastDaydate 
{  !
get" %
;% &
set' *
;* +
}, -
public 

decimal 
DailyPercentage "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 

decimal 
BasePack 
{ 
get !
;! "
set# &
;& '
}( )
public 

int 

DaysAmount 
{ 
get 
;  
set! $
;$ %
}& '
public 

decimal 

BaseAmount 
{ 
get  #
;# $
set% (
;( )
}* +
public 

decimal 
CompanyAmount  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

decimal 
CompanyPercentage $
{% &
get' *
;* +
set, /
;/ 0
}1 2
public 

decimal #
ProfitDistributedLevels *
{+ ,
get- 0
;0 1
set2 5
;5 6
}7 8
public 

decimal 
TotalPercentage "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 

decimal 
DeductionAmount "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 

decimal 
PaymentAmount  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

string 
Points 
{ 
get 
; 
set  #
;# $
}% &
=' (
string) /
./ 0
Empty0 5
;5 6
public 

string 
CasePool 
{ 
get  
;  !
set" %
;% &
}' (
=) *
string+ 1
.1 2
Empty2 7
;7 8
public 

DateTime 

PeriodPool 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

virtual 
ICollection 
< "
ResultEcoPoolLevelsDto 5
.5 6"
ResultEcoPoolLevelsDto6 L
>L M
ResultEcoPoolLevelsN a
{b c
getd g
;g h
seti l
;l m
}n o
} —

\C:\HeroSystem\walletService\WalletService.Models\DTO\WalletDto\BalanceInformationAdminDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #
	WalletDto# ,
;, -
public 
class &
BalanceInformationAdminDto '
{ 
public 

decimal 
WalletProfit 
{  !
get" %
;% &
set' *
;* +
}, -
public 

decimal 
CommissionsPaid "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 

decimal !
CalculatedCommissions (
{) *
get+ .
;. /
set0 3
;3 4
}5 6
public 

int 
EnabledAffiliates  
{! "
get# &
;& '
set( +
;+ ,
}- .
public		 

decimal		 
TotalReverseBalance		 &
{		' (
get		) ,
;		, -
set		. 1
;		1 2
}		3 4
public 

decimal "
TotalCommissionsEarned )
{* +
get, /
;/ 0
set1 4
;4 5
}6 7
} ´
VC:\HeroSystem\walletService\WalletService.Models\DTO\WalletDto\PurchasesPerMonthDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #
	WalletDto# ,
;, -
public 
class  
PurchasesPerMonthDto !
{ 
public 

int 
Year 
{ 
get 
; 
set 
; 
}  !
public 

int 
Month 
{ 
get 
; 
set 
;  
}! "
public 

int 
TotalPurchases 
{ 
get  #
;# $
set% (
;( )
}* +
} Ó
KC:\HeroSystem\walletService\WalletService.Models\DTO\WalletDto\WalletDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #
	WalletDto# ,
;, -
public 
class 
	WalletDto 
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

string 
? 
AffiliateUserName $
{% &
get' *
;* +
set, /
;/ 0
}1 2
public 

string 
? 
AdminUserName  
{! "
get# &
;& '
set( +
;+ ,
}- .
public		 

int		 
UserId		 
{		 
get		 
;		 
set		  
;		  !
}		" #
public

 

double

 
Credit

 
{

 
get

 
;

 
set

  #
;

# $
}

% &
public 

double 
Debit 
{ 
get 
; 
set "
;" #
}$ %
public 

double 
? 
Deferred 
{ 
get !
;! "
set# &
;& '
}( )
public 

bool 
Status 
{ 
get 
; 
set !
;! "
}# $
public 

string 
? 
Concept 
{ 
get  
;  !
set" %
;% &
}' (
public 

int 
? 
Support 
{ 
get 
; 
set "
;" #
}$ %
public 

DateTime 
Date 
{ 
get 
; 
set  #
;# $
}% &
public 

bool 
Compression 
{ 
get !
;! "
set# &
;& '
}( )
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
set% (
;( )
}* +
public 

DateTime 
? 
	UpdatedAt 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

DateTime 
? 
	DeletedAt 
{  
get! $
;$ %
set& )
;) *
}+ ,
} à
YC:\HeroSystem\walletService\WalletService.Models\DTO\WalletHistoryDto\WalletHistoryDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #
WalletHistoryDto# 3
;3 4
public 
class 
WalletHistoryDto 
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

int 
UserId 
{ 
get 
; 
set  
;  !
}" #
public 

double 
Credit 
{ 
get 
; 
set  #
;# $
}% &
public		 

double		 
Debit		 
{		 
get		 
;		 
set		 "
;		" #
}		$ %
public

 

double

 
Deferred

 
{

 
get

  
;

  !
set

" %
;

% &
}

' (
public 

bool 
Status 
{ 
get 
; 
set !
;! "
}# $
public 

string 
? 
Concept 
{ 
get  
;  !
set" %
;% &
}' (
public 

int 
Support 
{ 
get 
; 
set !
;! "
}# $
public 

DateTime 
Date 
{ 
get 
; 
set  #
;# $
}% &
public 

bool 
Compression 
{ 
get !
;! "
set# &
;& '
}( )
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
set% (
;( )
}* +
public 

DateTime 
? 
	UpdatedAt 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

DateTime 
? 
	DeletedAt 
{  
get! $
;$ %
set& )
;) *
}+ ,
} Ñ

eC:\HeroSystem\walletService\WalletService.Models\DTO\WalletModel1ADto\BalanceInformationModel1ADto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #
WalletModel1ADto# 3
;3 4
public 
class (
BalanceInformationModel1ADto )
{ 
public 

decimal 
? 
ReverseBalance "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 

decimal 
? 
TotalAcquisitions %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
public		 

decimal		 
?		 
AvailableBalance		 $
{		% &
get		' *
;		* +
set		, /
;		/ 0
}		1 2
public 

decimal 
?  
TotalCommissionsPaid (
{) *
get+ .
;. /
set0 3
;3 4
}5 6
public 

decimal 
? 
ServiceBalance "
{# $
get% (
;( )
set* -
;- .
}/ 0
} Ñ

eC:\HeroSystem\walletService\WalletService.Models\DTO\WalletModel1BDto\BalanceInformationModel1BDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #
WalletModel1BDto# 3
;3 4
public 
class (
BalanceInformationModel1BDto )
{ 
public 

decimal 
? 
ReverseBalance "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 

decimal 
? 
TotalAcquisitions %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
public		 

decimal		 
?		 
AvailableBalance		 $
{		% &
get		' *
;		* +
set		, /
;		/ 0
}		1 2
public 

decimal 
?  
TotalCommissionsPaid (
{) *
get+ .
;. /
set0 3
;3 4
}5 6
public 

decimal 
? 
ServiceBalance "
{# $
get% (
;( )
set* -
;- .
}/ 0
} £

WC:\HeroSystem\walletService\WalletService.Models\DTO\WalletPeriodDto\WalletPeriodDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #
WalletPeriodDto# 2
;2 3
public 
class 
WalletPeriodDto 
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

DateTime 
Date 
{ 
get 
; 
set  #
;# $
}% &
public 

bool 
Status 
{ 
get 
; 
set !
;! "
}# $
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
set% (
;( )
}* +
public		 

DateTime		 
?		 
	UpdatedAt		 
{		  
get		! $
;		$ %
set		& )
;		) *
}		+ ,
public

 

DateTime

 
?

 
	DeletedAt

 
{

  
get

! $
;

$ %
set

& )
;

) *
}

+ ,
} ¶
YC:\HeroSystem\walletService\WalletService.Models\DTO\WalletRequestDto\WalletRequestDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #
WalletRequestDto# 3
;3 4
public 
class 
WalletRequestDto 
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

int 
PaymentAffiliateId !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 

string 
? 
OrderNumber 
{  
get! $
;$ %
set& )
;) *
}+ ,
public		 

string		 
?		 
AdminUserName		  
{		! "
get		# &
;		& '
set		( +
;		+ ,
}		- .
public

 

decimal

 
Amount

 
{

 
get

 
;

  
set

! $
;

$ %
}

& '
public 

string 
? 
Concept 
{ 
get  
;  !
set" %
;% &
}' (
public 

string 
? 
Type 
{ 
get 
; 
set "
;" #
}$ %
public 

int 
? 
InvoiceNumber 
{ 
get  #
;# $
set% (
;( )
}* +
public 

DateTime 
CreationDate  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

byte 
? 
Status 
{ 
get 
; 
set "
;" #
}$ %
public 

DateTime 
? 
AttentionDate "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
set% (
;( )
}* +
public 

DateTime 
? 
	UpdatedAt 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

DateTime 
? 
	DeletedAt 
{  
get! $
;$ %
set& )
;) *
}+ ,
} ⁄
iC:\HeroSystem\walletService\WalletService.Models\DTO\WalletRetentionConfigDto\WalletRetentionConfigDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #$
WalletRetentionConfigDto# ;
;; <
public 
class $
WalletRetentionConfigDto %
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

decimal 
WithdrawalFrom !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 

decimal 
WithdrawalTo 
{  !
get" %
;% &
set' *
;* +
}, -
public 

decimal 

Percentage 
{ 
get  #
;# $
set% (
;( )
}* +
public		 

DateTime		 
Date		 
{		 
get		 
;		 
set		  #
;		# $
}		% &
public

 

DateTime

 
?

 
DisableDate

  
{

! "
get

# &
;

& '
set

( +
;

+ ,
}

- .
public 

bool 
Status 
{ 
get 
; 
set !
;! "
}# $
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
set% (
;( )
}* +
public 

DateTime 
? 
	UpdatedAt 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

DateTime 
? 
	DeletedAt 
{  
get! $
;$ %
set& )
;) *
}+ ,
} ‘
SC:\HeroSystem\walletService\WalletService.Models\DTO\WalletWaitDto\WalletWaitDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #
WalletWaitDto# 0
;0 1
public 
class 
WalletWaitDto 
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

decimal 
? 
Credit 
{ 
get  
;  !
set" %
;% &
}' (
public 

string 
? 
PaymentMethod  
{! "
get# &
;& '
set( +
;+ ,
}- .
public		 

string		 
?		 
Bank		 
{		 
get		 
;		 
set		 "
;		" #
}		$ %
public

 

string

 
?

 
Support

 
{

 
get

  
;

  !
set

" %
;

% &
}

' (
public 

DateTime 
? 
DepositDate  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

bool 
Status 
{ 
get 
; 
set !
;! "
}# $
public 

bool 
Attended 
{ 
get 
; 
set  #
;# $
}% &
public 

DateTime 
Date 
{ 
get 
; 
set  #
;# $
}% &
public 

string 
? 
Order 
{ 
get 
; 
set  #
;# $
}% &
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
set% (
;( )
}* +
public 

DateTime 
? 
	UpdatedAt 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

DateTime 
? 
	DeletedAt 
{  
get! $
;$ %
set& )
;) *
}+ ,
} ‡
_C:\HeroSystem\walletService\WalletService.Models\DTO\WalletWithDrawalDto\WalletWithDrawalDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
DTO "
." #
WalletWithDrawalDto# 6
;6 7
public 
class 
WalletWithDrawalDto  
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

string 
AffiliateUserName #
{$ %
get& )
;) *
set+ .
;. /
}0 1
public 

decimal 
Amount 
{ 
get 
;  
set! $
;$ %
}& '
public		 

bool		 
State		 
{		 
get		 
;		 
set		  
;		  !
}		" #
public

 

string

 
?

 
Observation

 
{

  
get

! $
;

$ %
set

& )
;

) *
}

+ ,
public 

string 
? 
AdminObservation #
{$ %
get& )
;) *
set+ .
;. /
}0 1
public 

DateTime 
Date 
{ 
get 
; 
set  #
;# $
}% &
public 

DateTime 
? 
ResponseDate !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 

decimal 
RetentionPercentage &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
public 

bool 
Status 
{ 
get 
; 
set !
;! "
}# $
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
set% (
;( )
}* +
public 

DateTime 
? 
	UpdatedAt 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

DateTime 
? 
	DeletedAt 
{  
get! $
;$ %
set& )
;) *
}+ ,
} ˛
BC:\HeroSystem\walletService\WalletService.Models\Enums\AuthType.cs
	namespace 	
WalletService
 
. 
Models 
. 
Enums $
;$ %
public 
enum 
AuthType 
{ 
None 
=	 

$num 
, 
Bearer 

= 
$num 
, 
BetRadar 
= 
$num 
, 

BasicToken 
= 
$num 
, 
UsernamePassword		 
=		 
$num		 
,		 
}

  
PC:\HeroSystem\walletService\WalletService.Models\Enums\ModelTypeConfiguration.cs
	namespace 	
WalletService
 
. 
Models 
. 
Enums $
;$ %
public 
enum "
ModelTypeConfiguration "
{ 
Model_1A 
, 
Model_1B 
, 
Model_2 
, 
Model_3 
, 
}		 Ù
EC:\HeroSystem\walletService\WalletService.Models\Enums\PaymentType.cs
	namespace 	
WalletService
 
. 
Models 
. 
Enums $
;$ %
public 
enum 
PaymentType 
{ 
Balance 
, 
ReversedBalance 
, !
PaymentToThirdParties 
, %
MembershipPaymentStrategy 
, 
CoinPayments		 
}

 ´
EC:\HeroSystem\walletService\WalletService.Models\Enums\ProductPdfs.cs
	namespace 	
WalletService
 
. 
Models 
. 
Enums $
;$ %
public 
class 
ProductPdfMapping 
{ 
public 

static 
readonly 

Dictionary %
<% &
string& ,
,, -
string. 4
>4 5
ProductToPdfMap6 E
=F G
newH K

DictionaryL V
<V W
stringW ]
,] ^
string_ e
>e f
{ 
{ 	
$str
 
, 
$str )
}* +
,+ ,
{ 	
$str
 
, 
$str *
}+ ,
,, -
{		 	
$str		
 
,		 
$str		 +
}		, -
,		- .
{

 	
$str


 
,

 
$str

 *
}

+ ,
,

, -
{ 	
$str
 
, 
$str ,
}- .
,. /
{ 	
$str
 
, 
$str +
}, -
,- .
{ 	
$str
 
, 
$str *
}+ ,
,, -
{ 	
$str
 
, 
$str )
}* +
,+ ,
{ 	
$str
 
, 
$str )
}* +
,+ ,
{ 	
$str
 
, 
$str +
}, -
,- .
{ 	
$str
 
, 
$str .
}/ 0
,0 1
{ 	
$str
 
, 
$str -
}. /
,/ 0
} 
; 
} Ó
EC:\HeroSystem\walletService\WalletService.Models\Enums\ProductType.cs
	namespace 	
WalletService
 
. 
Models 
. 
Enums $
;$ %
public 
enum 
ProductType 
{ 
EcoPool 
, 

Membership 
, 
RecyCoin 
= 
$num 
, 
HouseCoinPlan 
= 
$num 
, 
ExitoJuntosPlan		 
=		 
$num		 
,		 
Course

 

,


 
} É
KC:\HeroSystem\walletService\WalletService.Models\Enums\WalletConceptType.cs
	namespace 	
WalletService
 
. 
Models 
. 
Enums $
;$ %
public 
enum 
WalletConceptType 
{ 
store_purchase 
, $
commission_passed_wallet 
, %
wallet_withdrawal_request 
, 
balance_transfer 
, 
purchasing_pool		 
,		 
pool_commission

 
,

 
recurring_payment 
, 
model_four_payment 
, 
model_five_payment 
, 
model_six_payment 
, 
revert_pool 
, )
purchase_with_reverse_balance !
,! "
membership_bonus 
} Ú
MC:\HeroSystem\walletService\WalletService.Models\Enums\WalletRequestStatus.cs
	namespace 	
WalletService
 
. 
Models 
. 
Enums $
;$ %
public 
enum 
WalletRequestStatus 
{ 
pending 
= 
$num 
, 
approved 
= 
$num 
, 
cancel 

= 
$num 
} Å
KC:\HeroSystem\walletService\WalletService.Models\Enums\WalletRequestType.cs
	namespace 	
WalletService
 
. 
Models 
. 
Enums $
;$ %
public 
enum 
WalletRequestType 
{ 
withdrawal_request 
, "
revert_invoice_request 
} Ì
JC:\HeroSystem\walletService\WalletService.Models\Enums\WithdrawalStatus.cs
	namespace 	
WalletService
 
. 
Models 
. 
Enums $
;$ %
public 
enum 
WithdrawalStatus 
{ 
Pending 
= 
$num 
, 
	Completed 
= 
$num 
, 
Failed 

= 
$num 
} û'
LC:\HeroSystem\walletService\WalletService.Models\Exceptions\BaseException.cs
	namespace 	
WalletService
 
. 
Models 
. 

Exceptions )
;) *
[ 
Serializable 
] 
public 
class 
BaseException 
: 
	Exception &
{ 
public		 

BaseException		 
(		 
)		 
{		 
}		 
public 

BaseException 
( 
string 
message  '
)' (
:) *
base+ /
(/ 0
message0 7
)7 8
{9 :
}; <
public 

BaseException 
( 
string 
format  &
,& '
params( .
object/ 5
[5 6
]6 7
args8 <
)< =
:> ?
base@ D
(D E
stringE K
.K L
FormatL R
(R S
formatS Y
,Y Z
args[ _
)_ `
)` a
{b c
}d e
public 

BaseException 
( 
string 
message  '
,' (
	Exception) 2
innerException3 A
)A B
:C D
baseE I
(I J
messageJ Q
,Q R
innerExceptionS a
)a b
{c d
}e f
public 

BaseException 
( 
string 
format  &
,& '
	Exception( 1
innerException2 @
,@ A
paramsB H
objectI O
[O P
]P Q
argsR V
)V W
:X Y
baseZ ^
(^ _
string_ e
.e f
Formatf l
(l m
formatm s
,s t
argsu y
)y z
,z {
innerException	| ä
)
ä ã
{
å ç
}
é è
public 

BaseException 
( 
SerializationInfo *
info+ /
,/ 0
StreamingContext1 A
contextB I
)I J
:K L
baseM Q
(Q R
infoR V
,V W
contextX _
)_ `
{a b
}c d
public 

BaseException 
( 
HttpStatusCode '

statusCode( 2
)2 3
{ 

StatusCode 
= 

statusCode 
;  
} 
public 

BaseException 
( 
string 
message  '
,' (
HttpStatusCode) 7

statusCode8 B
)B C
:D E
baseF J
(J K
messageK R
)R S
{ 

StatusCode 
= 

statusCode 
;  
} 
public 

BaseException 
( 
string 
format  &
,& '
HttpStatusCode( 6

statusCode7 A
,A B
paramsC I
objectJ P
[P Q
]Q R
argsS W
)W X
:Y Z
base[ _
(_ `
string` f
.f g
Formatg m
(m n
formatn t
,t u
argsv z
)z {
){ |
{   

StatusCode!! 
=!! 

statusCode!! 
;!!  
}"" 
public$$ 

BaseException$$ 
($$ 
string$$ 
message$$  '
,$$' (
HttpStatusCode$$) 7

statusCode$$8 B
,$$B C
	Exception$$D M
innerException$$N \
)$$\ ]
:$$^ _
base$$` d
($$d e
message$$e l
,$$l m
innerException$$n |
)$$| }
{%% 

StatusCode&& 
=&& 

statusCode&& 
;&&  
}'' 
public)) 

BaseException)) 
()) 
string)) 
format))  &
,))& '
HttpStatusCode))( 6

statusCode))7 A
,))A B
	Exception))C L
innerException))M [
,))[ \
params))] c
object))d j
[))j k
]))k l
args))m q
)))q r
:))s t
base))u y
())y z
string	))z Ä
.
))Ä Å
Format
))Å á
(
))á à
format
))à é
,
))é è
args
))ê î
)
))î ï
,
))ï ñ
innerException
))ó •
)
))• ¶
{** 

StatusCode++ 
=++ 

statusCode++ 
;++  
},, 
public.. 

HttpStatusCode.. 

StatusCode.. $
{..% &
get..' *
;..* +
set.., /
;../ 0
}..1 2
}// ì
NC:\HeroSystem\walletService\WalletService.Models\Exceptions\CustomException.cs
	namespace 	
WalletService
 
. 
Models 
. 

Exceptions )
;) *
public 
class 
CustomException 
: 
BaseException ,
{ 
public 

CustomException 
( 
) 
{ 
}  
public

 

CustomException

 
(

 
string

 !
message

" )
)

) *
:

+ ,
base

- 1
(

1 2
message

2 9
)

9 :
{

; <
}

= >
public 

CustomException 
( 
string !
message" )
,) *
	Exception+ 4
innerException5 C
)C D
:E F
baseG K
(K L
messageL S
,S T
innerExceptionU c
)c d
{e f
}g h
public 

CustomException 
( 
SerializationInfo ,
info- 1
,1 2
StreamingContext3 C
contextD K
)K L
:M N
baseO S
(S T
infoT X
,X Y
contextZ a
)a b
{c d
}e f
public 

CustomException 
( 
HttpStatusCode )

statusCode* 4
,4 5
string6 <
exceptionBody= J
)J K
{ 

StatusCode 
= 

statusCode "
;" #
ExceptionBody 
= 
exceptionBody %
;% &
} 
public 

string 
? 
ExceptionBody  
{! "
get# &
;& '
set( +
;+ ,
}- .
} °
VC:\HeroSystem\walletService\WalletService.Models\Requests\BonusRequest\BonusRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
BonusRequest( 4
;4 5
public 
class 
BonusRequest 
{ 
public 

int 
	InvoiceId 
{ 
get 
; 
set  #
;# $
}% &
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

decimal 
Amount 
{ 
get 
;  
set! $
;$ %
}& '
public 

string 
Comment 
{ 
get 
;  
set! $
;$ %
}& '
=( )
string* 0
.0 1
Empty1 6
;6 7
}		 à	
dC:\HeroSystem\walletService\WalletService.Models\Requests\CoinPayRequest\BuyCryptoContractRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
CoinPayRequest( 6
;6 7
public 
class $
BuyCryptoContractRequest %
{ 
public 

int 
IdWallet 
{ 
get 
; 
set "
;" #
}$ %
public 

int 
IdCurrencyFrom 
{ 
get  #
;# $
set% (
;( )
}* +
public 

int 
IdCurrencyTo 
{ 
get !
;! "
set# &
;& '
}( )
public 

double 
Amount 
{ 
get 
; 
set  #
;# $
}% &
public		 

bool		 

IsAmountTo		 
{		 
get		  
;		  !
set		" %
;		% &
}		' (
}

 Ø
cC:\HeroSystem\walletService\WalletService.Models\Requests\CoinPayRequest\CompleteContractRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
CoinPayRequest( 6
;6 7
public 
class #
CompleteContractRequest $
{ 
public 

int 
IdWallet 
{ 
get 
; 
set "
;" #
}$ %
public 

int 

IdContract 
{ 
get 
;  
set! $
;$ %
}& '
} ¡
_C:\HeroSystem\walletService\WalletService.Models\Requests\CoinPayRequest\CreateAddresRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
CoinPayRequest( 6
;6 7
public 
class 
CreateAddresRequest  
{ 
public 

int 

IdCurrency 
{ 
get 
;  
set! $
;$ %
}& '
public 

int 
	IdNetwork 
{ 
get 
; 
set  #
;# $
}% &
public 

int 
IdWallet 
{ 
get 
; 
set "
;" #
}$ %
} ˝
`C:\HeroSystem\walletService\WalletService.Models\Requests\CoinPayRequest\CreateChannelRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
CoinPayRequest( 6
;6 7
public 
class  
CreateChannelRequest !
{ 
public 

int 

IdCurrency 
{ 
get 
;  
set! $
;$ %
}& '
public 

int 
	IdNetwork 
{ 
get 
; 
set  #
;# $
}% &
public 

int $
IdExternalIdentification '
{( )
get* -
;- .
set/ 2
;2 3
}4 5
public 

string 
? 
TagName 
{ 
get  
;  !
set" %
;% &
}' (
}		 ö	
`C:\HeroSystem\walletService\WalletService.Models\Requests\CoinPayRequest\CreateDepositRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
CoinPayRequest( 6
;6 7
public 
class  
CreateDepositRequest !
{ 
public 

int 
IdUser 
{ 
get 
; 
set  
;  !
}" #
public 

int 

IdCurrency 
{ 
get 
;  
set! $
;$ %
}& '
public 

int 
Amount 
{ 
get 
; 
set  
;  !
}" #
public 

string 
? 
Description 
{  
get! $
;$ %
set& )
;) *
}+ ,
public		 

string		 
?		 
IdReference		 
{		  
get		! $
;		$ %
set		& )
;		) *
}		+ ,
}

 ô	
]C:\HeroSystem\walletService\WalletService.Models\Requests\CoinPayRequest\CreateLinkRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
CoinPayRequest( 6
;6 7
public 
class 
CreateLinkRequest 
{ 
public 

string 
? 
CurrencyCode 
{  !
get" %
;% &
set' *
;* +
}, -
public 

int 
IdCurrencyPay 
{ 
get "
;" #
set$ '
;' (
}) *
public 

int 
Amount 
{ 
get 
; 
set  
;  !
}" #
public 

int 
IdReference 
{ 
get  
;  !
set" %
;% &
}' (
public		 

string		 
?		 
Address		 
{		 
get		  
;		  !
set		" %
;		% &
}		' (
}

 ¿
dC:\HeroSystem\walletService\WalletService.Models\Requests\CoinPayRequest\CreateTransactionRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
CoinPayRequest( 6
;6 7
public 
class $
CreateTransactionRequest %
{ 
[		 
JsonPropertyName		 
(		 
$str		 #
)		# $
]		$ %
public		& ,
int		- 0
AffiliateId		1 <
{		= >
get		? B
;		B C
set		D G
;		G H
}		I J
[ 
JsonPropertyName 
( 
$str  
)  !
]! "
public# )
string* 0
UserName1 9
{: ;
get< ?
;? @
setA D
;D E
}F G
=H I
stringJ P
.P Q
EmptyQ V
;V W
[ 
JsonPropertyName 
( 
$str 
) 
]  
public! '
int( +
Amount, 2
{3 4
get5 8
;8 9
set: =
;= >
}? @
[ 
JsonPropertyName 
( 
$str  
)  !
]! "
public# )
List* .
<. /
ProductRequest/ =
>= >
?> ?
Products@ H
{I J
getK N
;N O
setP S
;S T
}U V
[ 
JsonPropertyName 
( 
$str !
)! "
]" #
public$ *
int+ .
	NetworkId/ 8
{9 :
get; >
;> ?
set@ C
;C D
}E F
[ 
JsonPropertyName 
( 
$str "
)" #
]# $
public% +
int, /

CurrencyId0 :
{; <
get= @
;@ A
setB E
;E F
}G H
} ™

[C:\HeroSystem\walletService\WalletService.Models\Requests\CoinPayRequest\GiftCardRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
CoinPayRequest( 6
;6 7
public 
class 
GiftCardRequest 
{ 
public 

int 

IdCurrency 
{ 
get 
;  
set! $
;$ %
}& '
public 

int 
	IdNetwork 
{ 
get 
; 
set  #
;# $
}% &
public 

string 
? 
Address 
{ 
get  
;  !
set" %
;% &
}' (
public 

int 
Amount 
{ 
get 
; 
set  
;  !
}" #
public		 

string		 
?		 
Details		 
{		 
get		  
;		  !
set		" %
;		% &
}		' (
public

 

bool

 
AmountPlusFee

 
{

 
get

  #
;

# $
set

% (
;

( )
}

* +
} å	
ZC:\HeroSystem\walletService\WalletService.Models\Requests\CoinPayRequest\PaymentRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
CoinPayRequest( 6
;6 7
public 
class 
PaymentRequest 
{ 
[ 
JsonPropertyName 
( 
$str "
)" #
]# $
public% +
int, /

IdCurrency0 :
{; <
get= @
;@ A
setB E
;E F
}G H
[ 
JsonPropertyName 
( 
$str 
) 
]  
public! '
double( .
Amount/ 5
{6 7
get8 ;
;; <
set= @
;@ A
}B C
[		 
JsonPropertyName		 
(		 
$str		 
)		  
]		  !
public		" (
string		) /
?		/ 0
Details		1 8
{		9 :
get		; >
;		> ?
set		@ C
;		C D
}		E F
}

 ™

[C:\HeroSystem\walletService\WalletService.Models\Requests\CoinPayRequest\SendFundRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
CoinPayRequest( 6
;6 7
public 
class 
SendFundRequest 
{ 
public 

int 

IdCurrency 
{ 
get 
;  
set! $
;$ %
}& '
public 

int 
	IdNetwork 
{ 
get 
; 
set  #
;# $
}% &
public 

string 
? 
Address 
{ 
get  
;  !
set" %
;% &
}' (
public 

int 
Amount 
{ 
get 
; 
set  
;  !
}" #
public		 

string		 
?		 
Details		 
{		 
get		  
;		  !
set		" %
;		% &
}		' (
public

 

bool

 
AmountPlusFee

 
{

 
get

  #
;

# $
set

% (
;

( )
}

* +
} ‘
bC:\HeroSystem\walletService\WalletService.Models\Requests\CoinPayRequest\SignatureParamsRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
CoinPayRequest( 6
;6 7
public 
class "
SignatureParamsRequest #
{ 
public 

int 
IdUser 
{ 
get 
; 
set  
;  !
}" #
public 

int 
IdTransaction 
{ 
get "
;" #
set$ '
;' (
}) *
public 

string 

DynamicKey 
{ 
get "
;" #
set$ '
;' (
}) *
=+ ,
string- 3
.3 4
Empty4 9
;9 :
public 

string 
? 
IncomingSignature $
{% &
get' *
;* +
set, /
;/ 0
}1 2
}		 é,
jC:\HeroSystem\walletService\WalletService.Models\Requests\CoinPayRequest\TransactionNotificationRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
CoinPayRequest( 6
;6 7
public 
class *
TransactionNotificationRequest +
{ 
public 

int 
IdUser 
{ 
get 
; 
set  
;  !
}" #
public 

int 
IdWallet 
{ 
get 
; 
set "
;" #
}$ %
public 

int 
IdTransaction 
{ 
get "
;" #
set$ '
;' (
}) *
public 

string 
? 
IdExternalReference &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
public		 

string		 
?		 
Address		 
{		 
get		  
;		  !
set		" %
;		% &
}		' (
public

 

string

 
?

 
TxtId

 
{

 
get

 
;

 
set

  #
;

# $
}

% &
public 

decimal 
Amount 
{ 
get 
;  
set! $
;$ %
}& '
public 

int 
Fee 
{ 
get 
; 
set 
; 
}  
public 

int 
ExchangeRate 
{ 
get !
;! "
set# &
;& '
}( )
public 

PaymentMethod 
? 
PaymentMethod '
{( )
get* -
;- .
set/ 2
;2 3
}4 5
public 

Currency 
? 
Currency 
{ 
get  #
;# $
set% (
;( )
}* +
public 

UserChannel 
? 
UserChannel #
{$ %
get& )
;) *
set+ .
;. /
}0 1
public 

TransactionType 
? 
TransactionType +
{, -
get. 1
;1 2
set3 6
;6 7
}8 9
public 

TransactionStatus 
? 
TransactionStatus /
{0 1
get2 5
;5 6
set7 :
;: ;
}< =
public 

int 
NumberAttemps 
{ 
get "
;" #
set$ '
;' (
}) *
public 

int 
WalletConfirmations "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 

string 
? 

CreateDate 
{ 
get  #
;# $
set% (
;( )
}* +
} 
public 
class 
PaymentMethod 
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

string 
? 
Name 
{ 
get 
; 
set "
;" #
}$ %
} 
public 
class 
Currency 
{ 
public   

int   
Id   
{   
get   
;   
set   
;   
}   
public!! 

string!! 
?!! 
Name!! 
{!! 
get!! 
;!! 
set!! "
;!!" #
}!!$ %
public"" 

string"" 
?"" 
Code"" 
{"" 
get"" 
;"" 
set"" "
;""" #
}""$ %
}## 
public%% 
class%% 
UserChannel%% 
{&& 
public'' 

string'' 
?'' $
IdExternalIdentification'' +
{'', -
get''. 1
;''1 2
set''3 6
;''6 7
}''8 9
public(( 

string(( 
?(( 
TagName(( 
{(( 
get((  
;((  !
set((" %
;((% &
}((' (
})) 
public++ 
class++ 
TransactionType++ 
{,, 
public-- 

int-- 
Id-- 
{-- 
get-- 
;-- 
set-- 
;-- 
}-- 
public.. 

string.. 
?.. 
Name.. 
{.. 
get.. 
;.. 
set.. "
;.." #
}..$ %
}// 
public11 
class11 
TransactionStatus11 
{22 
public33 

int33 
Id33 
{33 
get33 
;33 
set33 
;33 
}33 
public44 

string44 
?44 
Name44 
{44 
get44 
;44 
set44 "
;44" #
}44$ %
public55 

string55 
?55 
Description55 
{55  
get55! $
;55$ %
set55& )
;55) *
}55+ ,
}66 ç	
aC:\HeroSystem\walletService\WalletService.Models\Requests\CoinPayRequest\WalletTransferRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
CoinPayRequest( 6
;6 7
public 
class !
WalletTransferRequest "
{ 
public 

int 
IdWalletFrom 
{ 
get !
;! "
set# &
;& '
}( )
public 

int 

IdWalletTo 
{ 
get 
;  
set! $
;$ %
}& '
public 

int 

IdCurrency 
{ 
get 
;  
set! $
;$ %
}& '
public 

double 
Amount 
{ 
get 
; 
set  #
;# $
}% &
public		 

string		 
?		 
Detail		 
{		 
get		 
;		  
set		! $
;		$ %
}		& '
}

 £
cC:\HeroSystem\walletService\WalletService.Models\Requests\CoinPayRequest\WalletWithDetailRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
CoinPayRequest( 6
;6 7
public 
class #
WalletWithDetailRequest $
{ 
public 

string 
? 
Name 
{ 
get 
; 
set "
;" #
}$ %
public 

string 
? 
LastName 
{ 
get !
;! "
set# &
;& '
}( )
public 

string 
? 
CountryCode 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

string 
? 
Email 
{ 
get 
; 
set  #
;# $
}% &
}		 Ó
fC:\HeroSystem\walletService\WalletService.Models\Requests\CoinPayRequest\WebhookNotificationRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
CoinPayRequest( 6
;6 7
public 
class &
WebhookNotificationRequest '
{ 
public 

int 
IdUser 
{ 
get 
; 
set  
;  !
}" #
public 

int 
IdWallet 
{ 
get 
; 
set "
;" #
}$ %
public 

int 
IdTransaction 
{ 
get "
;" #
set$ '
;' (
}) *
public 

string 
IdExternalReference %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
=4 5
string6 <
.< =
Empty= B
;B C
public		 

string		 
?		 
Address		 
{		 
get		  
;		  !
set		" %
;		% &
}		' (
public

 

string

 
?

 
TxtId

 
{

 
get

 
;

 
set

  #
;

# $
}

% &
public 

decimal 
Amount 
{ 
get 
;  
set! $
;$ %
}& '
public 

decimal 
Fee 
{ 
get 
; 
set !
;! "
}# $
public 

decimal 
ExchangeRate 
{  !
get" %
;% &
set' *
;* +
}, -
public 

PaymentMethod 
? 
PaymentMethod '
{( )
get* -
;- .
set/ 2
;2 3
}4 5
public 

Currency 
? 
Currency 
{ 
get  #
;# $
set% (
;( )
}* +
public 

UserChannel 
? 
UserChannel #
{$ %
get& )
;) *
set+ .
;. /
}0 1
public 

TransactionType 
? 
TransactionType +
{, -
get. 1
;1 2
set3 6
;6 7
}8 9
public 

TransactionStatus 
TransactionStatus .
{/ 0
get1 4
;4 5
set6 9
;9 :
}; <
== >
new? B
TransactionStatusC T
(T U
)U V
;V W
public 

string 
? 
Description 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

int 
NumberAttemps 
{ 
get "
;" #
set$ '
;' (
}) *
public 

int 
WalletConfirmations "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 

DateTime 

CreateDate 
{  
get! $
;$ %
set& )
;) *
}+ ,
} Ì
]C:\HeroSystem\walletService\WalletService.Models\Requests\CoinPayRequest\WithDrawalRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
CoinPayRequest( 6
;6 7
public 
class 
WithDrawalRequest 
{ 
[ 
JsonProperty 
( 
$str 
) 
] 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
[

 
JsonProperty

 
(

 
$str

 
)

  
]

  !
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
[ 
JsonProperty 
( 
$str 
) 
] 
public 

int 
Amount 
{ 
get 
; 
set  
;  !
}" #
} ç
oC:\HeroSystem\walletService\WalletService.Models\Requests\ConPaymentRequest\CoinPaymentMassWithdrawalRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
ConPaymentRequest( 9
;9 :
public 
class ,
 CoinPaymentMassWithdrawalRequest -
{ 
public 

decimal 
Amount 
{ 
get 
;  
set! $
;$ %
}& '
public 

string 
Address 
{ 
get 
;  
set! $
;$ %
}& '
=( )
string* 0
.0 1
Empty1 6
;6 7
public 

string 
Currency 
{ 
get  
;  !
set" %
;% &
}' (
=) *
string+ 1
.1 2
Empty2 7
;7 8
} Û
lC:\HeroSystem\walletService\WalletService.Models\Requests\ConPaymentRequest\CoinPaymentTransactionRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
ConPaymentRequest( 9
;9 :
public 
class )
CoinPaymentTransactionRequest *
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

string 
? 
IdTransaction  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

decimal 
Amount 
{ 
get 
;  
set! $
;$ %
}& '
public		 

decimal		 
AmountReceived		 !
{		" #
get		$ '
;		' (
set		) ,
;		, -
}		. /
public

 

string

 
?

 
Products

 
{

 
get

 !
;

! "
set

# &
;

& '
}

( )
public 

bool 
	Acredited 
{ 
get 
;  
set! $
;$ %
}& '
public 

int 
Status 
{ 
get 
; 
set  
;  !
}" #
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
set% (
;( )
}* +
public 

DateTime 
	UpdatedAt 
{ 
get  #
;# $
set% (
;( )
}* +
public 

DateTime 
? 
	DeletedAt 
{  
get! $
;$ %
set& )
;) *
}+ ,
} Ë#
`C:\HeroSystem\walletService\WalletService.Models\Requests\ConPaymentRequest\ConPaymentRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
ConPaymentRequest( 9
;9 :
public 
class 
ConPaymentRequest 
{ 
[ 
JsonPropertyName 
( 
$str 
) 
]  
public 

decimal 
Amount 
{ 
get 
;  
set! $
;$ %
}& '
[

 
JsonPropertyName

 
(

 
$str

 !
)

! "
]

" #
public 

string 
	Currency1 
{ 
get !
;! "
set# &
;& '
}( )
=* +
string, 2
.2 3
Empty3 8
;8 9
[ 
JsonPropertyName 
( 
$str !
)! "
]" #
public 

string 
	Currency2 
{ 
get !
;! "
set# &
;& '
}( )
=* +
string, 2
.2 3
Empty3 8
;8 9
[ 
JsonPropertyName 
( 
$str #
)# $
]$ %
public 

string 

BuyerEmail 
{ 
get "
;" #
set$ '
;' (
}) *
=+ ,
string- 3
.3 4
Empty4 9
;9 :
[ 
JsonPropertyName 
( 
$str 
)  
]  !
public 

string 
Address 
{ 
get 
;  
set! $
;$ %
}& '
=( )
string* 0
.0 1
Empty1 6
;6 7
[ 
JsonPropertyName 
( 
$str "
)" #
]# $
public 

string 
? 
	BuyerName 
{ 
get "
;" #
set$ '
;' (
}) *
[ 
JsonPropertyName 
( 
$str !
)! "
]" #
public 

string 
? 
ItemName 
{ 
get !
;! "
set# &
;& '
}( )
[ 
JsonPropertyName 
( 
$str #
)# $
]$ %
public 

string 
? 

ItemNumber 
{ 
get  #
;# $
set% (
;( )
}* +
[ 
JsonPropertyName 
( 
$str 
)  
]  !
public   

string   
?   
Invoice   
{   
get    
;    !
set  " %
;  % &
}  ' (
["" 
JsonPropertyName"" 
("" 
$str"" 
)"" 
]""  
public## 

string## 
?## 
Custom## 
{## 
get## 
;##  
set##! $
;##$ %
}##& '
[%% 
JsonPropertyName%% 
(%% 
$str%% 
)%%  
]%%  !
public&& 

string&& 
?&& 
IpnUrl&& 
{&& 
get&& 
;&&  
set&&! $
;&&$ %
}&&& '
[(( 
JsonPropertyName(( 
((( 
$str(( #
)((# $
](($ %
public)) 

string)) 
?)) 

SuccessUrl)) 
{)) 
get))  #
;))# $
set))% (
;))( )
}))* +
[++ 
JsonPropertyName++ 
(++ 
$str++ "
)++" #
]++# $
public,, 

string,, 
?,, 
	CancelUrl,, 
{,, 
get,, "
;,," #
set,,$ '
;,,' (
},,) *
[.. 
JsonPropertyName.. 
(.. 
$str..  
)..  !
]..! "
public// 

List// 
<// 
ProductRequest// 
>// 
Products//  (
{//) *
get//+ .
;//. /
set//0 3
;//3 4
}//5 6
}00 ô
YC:\HeroSystem\walletService\WalletService.Models\Requests\ConPaymentRequest\IpnRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
ConPaymentRequest( 9
;9 :
public 
class 

IpnRequest 
{ 
public 

decimal 
amount1 
{ 
get  
;  !
set" %
;% &
}' (
public 

decimal 
amount2 
{ 
get  
;  !
set" %
;% &
}' (
public 

string 

buyer_name 
{ 
get "
;" #
set$ '
;' (
}) *
public 

string 
	currency1 
{ 
get !
;! "
set# &
;& '
}( )
public		 

string		 
	currency2		 
{		 
get		 !
;		! "
set		# &
;		& '
}		( )
public

 

string

 
email

 
{

 
get

 
;

 
set

 "
;

" #
}

$ %
public 

string 
fee 
{ 
get 
; 
set  
;  !
}" #
public 

string 
ipn_id 
{ 
get 
; 
set  #
;# $
}% &
public 

string 
ipn_mode 
{ 
get  
;  !
set" %
;% &
}' (
public 

string 
ipn_type 
{ 
get  
;  !
set" %
;% &
}' (
public 

string 
ipn_version 
{ 
get  #
;# $
set% (
;( )
}* +
public 

string 
	item_name 
{ 
get !
;! "
set# &
;& '
}( )
public 

int 
item_number 
{ 
get  
;  !
set" %
;% &
}' (
public 

string 
merchant 
{ 
get  
;  !
set" %
;% &
}' (
public 

int 
received_amount 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

int 
received_confirms  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

int 
status 
{ 
get 
; 
set  
;  !
}" #
public 

string 
status_text 
{ 
get  #
;# $
set% (
;( )
}* +
public 

string 
txn_id 
{ 
get 
; 
set  #
;# $
}% &
public 

string 
custom 
{ 
get 
; 
set  #
;# $
}% &
} ¢
]C:\HeroSystem\walletService\WalletService.Models\Requests\ConPaymentRequest\ProductRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
ConPaymentRequest( 9
;9 :
public 
class 
ProductRequest 
{ 
public 

int 
	ProductId 
{ 
get 
; 
set  #
;# $
}% &
public 

int 
Quantity 
{ 
get 
; 
set "
;" #
}$ %
} ’
tC:\HeroSystem\walletService\WalletService.Models\Requests\EcoPoolConfigurationRequest\EcoPoolConfigurationRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' ('
EcoPoolConfigurationRequest( C
;C D
public 
class '
EcoPoolConfigurationRequest (
{ 
public 

int 
? 
Id 
{ 
get 
; 
set 
; 
}  
public 

decimal 
CompanyPercentage $
{% &
get' *
;* +
set, /
;/ 0
}1 2
public 

decimal #
CompanyPercentageLevels *
{+ ,
get- 0
;0 1
set2 5
;5 6
}7 8
public		 

decimal		 
EcoPoolPercentage		 $
{		% &
get		' *
;		* +
set		, /
;		/ 0
}		1 2
public

 

decimal

 
MaxGainLimit

 
{

  !
get

" %
;

% &
set

' *
;

* +
}

, -
public 

DateTime 
DateInit 
{ 
get "
;" #
set$ '
;' (
}) *
public 

DateTime 
DateEnd 
{ 
get !
;! "
set# &
;& '
}( )
public 

int 
Case 
{ 
get 
; 
set 
; 
}  !
public 

int 
? 
	Processed 
{ 
get 
;  
set! $
;$ %
}& '
public 

int 
? 
Totals 
{ 
get 
; 
set !
;! "
}# $
public 

ICollection 
< 
LevelEcoPoolRequest *
>* +
Levels, 2
{3 4
get5 8
;8 9
set: =
;= >
}? @
} ’
lC:\HeroSystem\walletService\WalletService.Models\Requests\EcoPoolConfigurationRequest\LevelEcoPoolRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' ('
EcoPoolConfigurationRequest( C
;C D
public 
class 
LevelEcoPoolRequest  
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

int 
Level 
{ 
get 
; 
set 
;  
}! "
public 

decimal 

Percentage 
{ 
get  #
;# $
set% (
;( )
}* +
} „!
fC:\HeroSystem\walletService\WalletService.Models\Requests\InvoiceDetailRequest\InvoiceDetailRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' ( 
InvoiceDetailRequest( <
;< =
public 
class  
InvoiceDetailRequest !
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

int 
	InvoiceId 
{ 
get 
; 
set  #
;# $
}% &
public 

int 
	ProductId 
{ 
get 
; 
set  #
;# $
}% &
public 

int 
PaymentGroupId 
{ 
get  #
;# $
set% (
;( )
}* +
public		 

bool		 
AccumMinPurchase		  
{		! "
get		# &
;		& '
set		( +
;		+ ,
}		- .
public

 

string

 
?

 
ProductName

 
{

  
get

! $
;

$ %
set

& )
;

) *
}

+ ,
public 

decimal 
ProductPrice 
{  !
get" %
;% &
set' *
;* +
}, -
public 

decimal 
ProductPriceBtc "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 

decimal 
? 

ProductIva 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

int 
ProductQuantity 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

decimal 
? !
ProductCommissionable )
{* +
get, /
;/ 0
set1 4
;4 5
}6 7
public 

decimal 
BinaryPoints 
{  !
get" %
;% &
set' *
;* +
}, -
public 

int 
? 
ProductPoints 
{ 
get  #
;# $
set% (
;( )
}* +
public 

decimal 
ProductDiscount "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 

DateTime 
Date 
{ 
get 
; 
set  #
;# $
}% &
public 

int 
CombinationId 
{ 
get "
;" #
set# &
;& '
}' (
public 

bool 
ProductPack 
{ 
get 
;  
set  #
;# $
}$ %
public 

decimal 
? 

BaseAmount 
{ 
get "
;" #
set# &
;& '
}' (
public 

decimal 
? 
DailyPercentage #
{# $
get$ '
;' (
set( +
;+ ,
}, -
public 

int 
? 
WaitingDays 
{ 
get 
;  
set  #
;# $
}$ %
public 

int 
DaysToPayQuantity  
{  !
get! $
;$ %
set% (
;( )
}) *
public 

bool 
ProductStart 
{ 
get  
;  !
set! $
;$ %
}% &
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
set% (
;( )
}* +
public 

DateTime 
? 
	UpdatedAt 
{  
get! $
;$ %
set& )
;) *
}+ ,
} ˚%
ZC:\HeroSystem\walletService\WalletService.Models\Requests\InvoiceRequest\InvoiceRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
InvoiceRequest( 6
;6 7
public 
class 
InvoiceRequest 
{ 
public		 

int		 
InvoiceNumber		 
{		 
get		 "
;		" #
set		$ '
;		' (
}		) *
public

 

int

 
PurchaseOrderId

 
{

  
get

! $
;

$ %
set

& )
;

) *
}

+ ,
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

decimal 
? 
TotalInvoice  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

decimal 
? 
TotalInvoiceBtc #
{$ %
get& )
;) *
set+ .
;. /
}0 1
public 

decimal 
? 
TotalCommissionable '
{( )
get* -
;- .
set/ 2
;2 3
}4 5
public 

int 
? 
TotalPoints 
{ 
get !
;! "
set# &
;& '
}( )
public 

byte 
? 
Status 
{ 
get 
; 
set "
;" #
}$ %
public 

byte 
? 
State 
{ 
get 
; 
set !
;! "
}# $
public 

DateTime 
? 
Date 
{ 
get 
;  
set! $
;$ %
}& '
public 

DateTime 
? 
CancellationDate %
{& '
get( +
;+ ,
set- 0
;0 1
}1 2
public 

string 
? 
PaymentMethod  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

string 
? 
Bank 
{ 
get 
; 
set "
;" #
}$ %
public 

string 
? 
ReceiptNumber  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

DateTime 
? 
DepositDate  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

byte 
? 
Type 
{ 
get 
; 
set  
;  !
}" #
public 

string 
? 
Reason 
{ 
get 
;  
set! $
;$ %
}& '
public 

string 
InvoiceData 
{ 
get  #
;# $
set% (
;( )
}* +
public 

string 
? 
InvoiceAddress !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 

string 
? 
ShippingAddress "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 

string 
? 
	SecretKey 
{ 
get "
;" #
set$ '
;' (
}) *
public 

string 
? 

BtcAddress 
{ 
get  #
;# $
set% (
;( )
}* +
public 

int 
	Recurring 
{ 
get 
; 
set  #
;# $
}% &
public   

DateTime   
	CreatedAt   
{   
get    #
;  # $
set  % (
;  ( )
}  * +
public!! 

DateTime!! 
	UpdatedAt!! 
{!! 
get!!  #
;!!# $
set!!% (
;!!( )
}!!* +
public%% 

List%% 
<%%  
InvoiceDetailRequest%% $
.%%$ % 
InvoiceDetailRequest%%% 9
>%%9 :
InvoiceDetail%%; H
{%%I J
get%%K N
;%%N O
set%%P S
;%%S T
}%%U V
}&& ∫
kC:\HeroSystem\walletService\WalletService.Models\Requests\InvoiceRequest\ModelBalancesAndInvoicesRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
InvoiceRequest( 6
;6 7
public 
class +
ModelBalancesAndInvoicesRequest ,
{ 
public 

string 
UserName 
{! "
get# &
;& '
set( +
;+ ,
}- .
=/ 0
string1 7
.7 8
Empty8 =
;= >
public 

decimal 
Model1AAmount  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

decimal 
Model1BAmount  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

decimal 
Model2Amount 
{! "
get# &
;& '
set( +
;+ ,
}- .
public		 

long		 
[		 
]		 
	InvoiceId		 
{		" #
get		$ '
;		' (
set		) ,
;		, -
}		. /
=		0 1
Array		2 7
.		7 8
Empty		8 =
<		= >
long		> B
>		B C
(		C D
)		D E
;		E F
}

 ®
dC:\HeroSystem\walletService\WalletService.Models\Requests\PagaditoRequest\AmountCollectionRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
PagaditoRequest( 7
;7 8
public 
class #
AmountCollectionRequest $
{ 
[ 
JsonPropertyName 
( 
$str 
) 
] 
public 

string 
? 
Total 
{ 
get 
; 
set  #
;# $
}% &
[

 
JsonPropertyName

 
(

 
$str

  
)

  !
]

! "
public 

string 
? 
Currency 
{ 
get !
;! "
set# &
;& '
}( )
} π
mC:\HeroSystem\walletService\WalletService.Models\Requests\PagaditoRequest\CreatePagaditoTransactionRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
PagaditoRequest( 7
;7 8
public 
class ,
 CreatePagaditoTransactionRequest -
{ 
[ 
JsonPropertyName 
( 
$str 
) 
]  
public 

decimal 
Amount 
{ 
get 
;  
set! $
;$ %
}& '
[

 
JsonPropertyName

 
(

 
$str

 $
)

$ %
]

% &
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
[ 
JsonPropertyName 
( 
$str 
)  
]  !
public 

List 
< ,
 PagaditoTransactionDetailRequest 0
>0 1
Details2 9
{: ;
get< ?
;? @
setA D
;D E
}F G
=H I
newJ M
ListN R
<R S,
 PagaditoTransactionDetailRequestS s
>s t
(t u
)u v
;v w
[ 
JsonPropertyName 
( 
$str %
)% &
]& '
public 


Dictionary 
< 
string 
, 
string $
>$ %
CustomParams& 2
{3 4
get5 8
;8 9
set: =
;= >
}? @
=A B
newC F

DictionaryG Q
<Q R
stringR X
,X Y
stringZ `
>` a
(a b
)b c
;c d
} ‹
mC:\HeroSystem\walletService\WalletService.Models\Requests\PagaditoRequest\PagaditoTransactionDetailRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
PagaditoRequest( 7
;7 8
public 
class ,
 PagaditoTransactionDetailRequest -
{ 
[ 
JsonPropertyName 
( 
$str  
)  !
]! "
public 

int 
Quantity 
{ 
get 
; 
set "
;" #
}$ %
[

 
JsonPropertyName

 
(

 
$str

 #
)

# $
]

$ %
public 

string 
? 
Description 
{  
get! $
;$ %
set& )
;) *
}+ ,
[ 
JsonPropertyName 
( 
$str 
) 
] 
public 

decimal 
? 
Price 
{ 
get 
;  
set! $
;$ %
}& '
[ 
JsonPropertyName 
( 
$str #
)# $
]$ %
public 

string 
? 

UrlProduct 
{ 
get  #
;# $
set% (
;( )
}* +
} ≤
fC:\HeroSystem\walletService\WalletService.Models\Requests\PagaditoRequest\ResourceCollectionRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
PagaditoRequest( 7
;7 8
public 
class %
ResourceCollectionRequest &
{ 
[ 
JsonPropertyName 
( 
$str 
) 
] 
public		 

string		 
?		 
Token		 
{		 
get		 
;		 
set		  #
;		# $
}		% &
[ 
JsonPropertyName 
( 
$str 
) 
] 
public 

string 
? 
Ern 
{ 
get 
; 
set !
;! "
}# $
[ 
JsonPropertyName 
( 
$str (
)( )
]) *
public 

string 
? 
CreateTimestamp "
{# $
get% (
;( )
set* -
;- .
}/ 0
[ 
JsonPropertyName 
( 
$str 
) 
]  
public 
#
AmountCollectionRequest "
?" #
Amount$ *
{+ ,
get- 0
;0 1
set2 5
;5 6
}7 8
[ 
JsonPropertyName 
( 
$str #
)# $
]$ %
public 

string 
? 
Description 
{  
get! $
;$ %
set& )
;) *
}+ ,
[ 
JsonPropertyName 
( 
$str (
)( )
]) *
public 

string 
? 
UpdateTimestamp "
{# $
get% (
;( )
set* -
;- .
}/ 0
[ 
JsonPropertyName 
( 
$str 
) 
]  
public 

string 
? 
Status 
{ 
get 
;  
set! $
;$ %
}& '
[ 
JsonPropertyName 
( 
$str !
)! "
]" #
public 

string 
? 
	Reference 
{ 
get "
;" #
set$ '
;' (
}) *
[   
JsonPropertyName   
(   
$str   "
)  " #
]  # $
public!! 

List!! 
<!! ,
 PagaditoTransactionDetailRequest!! 0
>!!0 1
?!!1 2
	ItemsList!!3 <
{!!= >
get!!? B
;!!B C
set!!D G
;!!G H
}!!I J
}"" ·
[C:\HeroSystem\walletService\WalletService.Models\Requests\PagaditoRequest\WebHookRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
PagaditoRequest( 7
;7 8
public 
class 
WebHookRequest 
{ 
[ 
JsonPropertyName 
( 
$str 
) 
] 
public 

string 
? 
Id 
{ 
get 
; 
set  
;  !
}" #
[

 
JsonPropertyName

 
(

 
$str

 .
)

. /
]

/ 0
public 

string 
?  
EventCreateTimestamp '
{( )
get* -
;- .
set/ 2
;2 3
}4 5
[ 
JsonPropertyName 
( 
$str "
)" #
]# $
public 

string 
? 
	EventType 
{ 
get "
;" #
set$ '
;' (
}) *
[ 
JsonPropertyName 
( 
$str  
)  !
]! "
public 
%
ResourceCollectionRequest $
?$ %
Resource& .
{/ 0
get1 4
;4 5
set6 9
;9 :
}; <
} Ñ
`C:\HeroSystem\walletService\WalletService.Models\Requests\PaginationRequest\PaginationRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
PaginationRequest( 9
;9 :
public 
class 
PaginationRequest 
{ 
private 
const 
int 
MaxPageSize !
=" #
$num$ '
;' (
private 
int 
	_pageSize 
= 
$num 
; 
public 

int 
PageSize 
{		 
get

 
=>

 
	_pageSize

 
;

 
set 
=> 
	_pageSize 
= 
value  
>! "
MaxPageSize# .
?/ 0
MaxPageSize1 <
:= >
value? D
;D E
} 
public 

int 

PageNumber 
{ 
get 
;  
set! $
;$ %
}& '
=( )
$num* +
;+ ,
public 

DateTime 
? 
	StartDate 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

DateTime 
? 
EndDate 
{ 
get "
;" #
set$ '
;' (
}) *
} ò
pC:\HeroSystem\walletService\WalletService.Models\Requests\PaymentTransaction\ConfirmPaymentTransactionRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
PaymentTransaction( :
;: ;
public 
class ,
 ConfirmPaymentTransactionRequest -
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

string 
UserName 
{ 
get  
;  !
set" %
;% &
}' (
=) *
string+ 1
.1 2
Empty2 7
;7 8
} ∏	
iC:\HeroSystem\walletService\WalletService.Models\Requests\PaymentTransaction\PaymentTransactionRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
PaymentTransaction( :
;: ;
public 
class %
PaymentTransactionRequest &
{ 
public 

string 
? 
IdTransaction  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

decimal 
Amount 
{ 
get 
;  
set! $
;$ %
}& '
public		 

string		 
?		 
Products		 
{		 
get		 !
;		! "
set		# &
;		& '
}		( )
public

 

DateTime

 
	CreatedAt

 
{

 
get

  #
;

# $
set

% (
;

( )
}

* +
} –
hC:\HeroSystem\walletService\WalletService.Models\Requests\RequestValidationCode\RequestValidationCode.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (!
RequestValidationCode( =
;= >
public 
class !
RequestValidationCode "
{ 
public 
int 
UserId 
{ 
get 
;  
set! $
;$ %
}& '
public 
string 
Code 
{ 
get 
;  
set! $
;$ %
}& '
public 
string 
Password 
{  
get! $
;$ %
set& )
;) *
}+ ,
}		 …
jC:\HeroSystem\walletService\WalletService.Models\Requests\TransferBalanceRequest\TransferBalanceRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' ("
TransferBalanceRequest( >
;> ?
public 
class "
TransferBalanceRequest #
{ 
[ 
JsonPropertyName 
( 
$str '
)' (
]( )
public 

int 
FromAffiliateId 
{  
get! $
;$ %
set& )
;) *
}+ ,
[

 
JsonPropertyName

 
(

 
$str

 $
)

$ %
]

% &
public 

string 
FromUserName 
{  
get! $
;$ %
set& )
;) *
}+ ,
=- .
string/ 5
.5 6
Empty6 ;
;; <
[ 
JsonPropertyName 
( 
$str "
)" #
]# $
public 

string 

ToUserName 
{ 
get "
;" #
set$ '
;' (
}) *
=+ ,
string- 3
.3 4
Empty4 9
;9 :
[ 
JsonPropertyName 
( 
$str 
) 
]  
public 

decimal 
? 
Amount 
{ 
get  
;  !
set" %
;% &
}' (
[ 
JsonPropertyName 
( 
$str $
)$ %
]% &
public 

string 
SecurityCode 
{  
get! $
;$ %
set& )
;) *
}+ ,
=- .
string/ 5
.5 6
Empty6 ;
;; <
} Ò
bC:\HeroSystem\walletService\WalletService.Models\Requests\UserGradingRequest\UserGradingRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
UserGradingRequest( :
;: ;
public 
class 
UserGradingRequest 
{ 
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

int 
AffiliateOwnerId 
{  !
get" %
;% &
set' *
;* +
}, -
public		 

int		 
Side		 
{		 
get		 
;		 
set		 
;		 
}		  !
public

 

string

 
UserName

 
{

 
get

  
;

  !
set

" %
;

% &
}

' (
public 

double 
Points 
{ 
get 
; 
set  #
;# $
}% &
public 

double 
Commissions 
{ 
get  #
;# $
set% (
;( )
}* +
public 

DateTime 
UserCreatedAt !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 


GradingDto 
? 
Grading 
{  
get! $
;$ %
set& )
;) *
}+ ,
} ¯
fC:\HeroSystem\walletService\WalletService.Models\Requests\WalletHistoryRequest\WalletHistoryRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' ( 
WalletHistoryRequest( <
;< =
public 
class  
WalletHistoryRequest !
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

int 
? 
UserId 
{ 
get 
; 
set !
;! "
}# $
public 

decimal 
Credit 
{ 
get 
;  
set! $
;$ %
}& '
public		 

decimal		 
Debit		 
{		 
get		 
;		 
set		  #
;		# $
}		% &
public

 

decimal

 
Deferred

 
{

 
get

 !
;

! "
set

# &
;

& '
}

( )
public 

bool 
Status 
{ 
get 
; 
set !
;! "
}# $
public 

string 
Concept 
{ 
get 
;  
set! $
;$ %
}& '
=( )
null* .
!. /
;/ 0
public 

int 
? 
Support 
{ 
get 
; 
set "
;" #
}$ %
public 

DateTime 
Date 
{ 
get 
; 
set  #
;# $
}% &
public 

bool 
Compression 
{ 
get !
;! "
set# &
;& '
}( )
} ç
^C:\HeroSystem\walletService\WalletService.Models\Requests\WalletPayRequest\WalletPayRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
WalletPayRequest( 8
;8 9
public 
class 
WalletPayRequest 
{ 
public 

WalletRequest 
. 
WalletRequest &
	WalletPay' 0
{1 2
get3 6
;6 7
set8 ;
;; <
}= >
public 

InvoiceRequest 
. 
InvoiceRequest (
Invoice) 0
{1 2
get3 6
;6 7
set8 ;
;; <
}= >
} ¬
dC:\HeroSystem\walletService\WalletService.Models\Requests\WalletPeriodRequest\WalletPeriodRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
WalletPeriodRequest( ;
;; <
public 
class 
WalletPeriodRequest  
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

DateOnly 
Date 
{ 
get 
; 
set  #
;# $
}% &
public 

bool 
Status 
{ 
get 
; 
set !
;! "
}# $
}		 √
fC:\HeroSystem\walletService\WalletService.Models\Requests\WalletRequestRequest\WalletRequestRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' ( 
WalletRequestRequest( <
;< =
public 
class  
WalletRequestRequest !
{ 
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

string 
AffiliateName 
{  !
get" %
;% &
set' *
;* +
}, -
=. /
string0 6
.6 7
Empty7 <
;< =
public 

string 
VerificationCode "
{# $
get% (
;( )
set* -
;- .
}/ 0
=1 2
string3 9
.9 :
Empty: ?
;? @
public 

string 
UserPassword 
{  
get! $
;$ %
set& )
;) *
}+ ,
=- .
string/ 5
.5 6
Empty6 ;
;; <
public		 

decimal		 
Amount		 
{		 
get		 
;		  
set		! $
;		$ %
}		& '
public

 

string

 
?

 
Concept

 
{

 
get

  
;

  !
set

" %
;

% &
}

' (
} ∫
pC:\HeroSystem\walletService\WalletService.Models\Requests\WalletRequestRequest\WalletRequestRevertTransaction.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' ( 
WalletRequestRequest( <
;< =
public 
class *
WalletRequestRevertTransaction +
{ 
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

int 
	InvoiceId 
{ 
get 
; 
set  #
;# $
}% &
public 

string 
Concept 
{ 
get 
;  
set! $
;$ %
}& '
=( )
string* 0
.0 1
Empty1 6
;6 7
} ª
hC:\HeroSystem\walletService\WalletService.Models\Requests\WalletRequest\CreditTransactionAdminRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
WalletRequest( 5
;5 6
public 
class )
CreditTransactionAdminRequest *
{ 
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

double 
Amount 
{ 
get 
; 
set  #
;# $
}% &
} Ï
cC:\HeroSystem\walletService\WalletService.Models\Requests\WalletRequest\CreditTransactionRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
WalletRequest( 5
;5 6
public 
class $
CreditTransactionRequest %
{ 
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

int 
UserId 
{ 
get 
; 
set  
;  !
}" #
public 

string 
Concept 
{ 
get 
;  
set! $
;$ %
}& '
=( )
string* 0
.0 1
Empty1 6
;6 7
public 

double 
Credit 
{ 
get 
; 
set  #
;# $
}% &
public		 

string		 
?		 
AffiliateUserName		 $
{		% &
get		' *
;		* +
set		, /
;		/ 0
}		1 2
public

 

string

 
AdminUserName

 
{

  !
get

" %
;

% &
set

' *
;

* +
}

, -
=

. /
string

0 6
.

6 7
Empty

7 <
;

< =
public 

string 
ConceptType 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
string. 4
.4 5
Empty5 :
;: ;
public 

long 
BrandId 
{ 
get 
; 
set "
;" #
}$ %
} á 
bC:\HeroSystem\walletService\WalletService.Models\Requests\WalletRequest\DebitTransactionRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
WalletRequest( 5
;5 6
public 
record #
DebitTransactionRequest %
{ 
public 

int 
AffiliateId 
{ 
get  
;  !
init" &
;& '
}( )
public 

int 
UserId 
{ 
get 
; 
init !
;! "
}# $
public 

string 
Concept 
{ 
get 
;  
init! %
;% &
}' (
=) *
string+ 1
.1 2
Empty2 7
;7 8
public 

decimal 
Points 
{ 
get 
;  
init! %
;% &
}' (
public		 

decimal		 
Commissionable		 !
{		" #
get		$ '
;		' (
init		) -
;		- .
}		/ 0
public

 

string

 
PaymentMethod

 
{

  !
get

" %
;

% &
init

' +
;

+ ,
}

- .
=

/ 0
string

1 7
.

7 8
Empty

8 =
;

= >
public 

short 
Origin 
{ 
get 
; 
init #
;# $
}% &
public 

int 
Level 
{ 
get 
; 
init  
;  !
}" #
public 

decimal 
Debit 
{ 
get 
; 
init  $
;$ %
}& '
public 

string 
AffiliateUserName #
{$ %
get& )
;) *
init+ /
;/ 0
}1 2
=3 4
string5 ;
.; <
Empty< A
;A B
public 

string 
AdminUserName 
{  !
get" %
;% &
init' +
;+ ,
}- .
=/ 0
string1 7
.7 8
Empty8 =
;= >
public 

string 
ConceptType 
{ 
get  #
;# $
init% )
;) *
}+ ,
=- .
string/ 5
.5 6
Empty6 ;
;; <
public 

long 
BrandId 
{ 
get 
; 
init #
;# $
}% &
public 

string 
? 
Bank 
{ 
get 
; 
init #
;# $
}% &
public 

string 
? 
ReceiptNumber  
{! "
get# &
;& '
init( ,
;, -
}. /
public 

bool 
Type 
{ 
get 
; 
init  
;  !
}" #
public 

string 
? 
	SecretKey 
{ 
get "
;" #
init$ (
;( )
}* +
public 

string 
? 
Reason 
{ 
get 
;  
init! %
;% &
}' (
public 

List 
< ,
 InvoiceDetailsTransactionRequest 0
>0 1
?1 2
invoices3 ;
{< =
get> A
;A B
initC G
;G H
}I J
=K L
newM P
ListQ U
<U V,
 InvoiceDetailsTransactionRequestV v
>v w
(w x
)x y
;y z
} §
\C:\HeroSystem\walletService\WalletService.Models\Requests\WalletRequest\DeleteKeysRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
WalletRequest( 5
;5 6
public 
class 
DeleteKeysRequest 
{ 
public 

string 
[ 
] 
Users 
{ 
get 
;  
set! $
;$ %
}& '
} í

gC:\HeroSystem\walletService\WalletService.Models\Requests\WalletRequest\DistributeCommissionsRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
WalletRequest( 5
;5 6
public 
class (
DistributeCommissionsRequest )
{ 
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

decimal 
InvoiceAmount  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

long 
BrandId 
{ 
get 
; 
set "
;" #
}$ %
public		 

string		 
AdminUserName		 
{		  !
get		" %
;		% &
set		' *
;		* +
}		, -
=		. /
string		0 6
.		6 7
Empty		7 <
;		< =
public

 

decimal

 
[

 
]

 
LevelPercentages

 %
{

& '
get

( +
;

+ ,
set

- 0
;

0 1
}

2 3
} ê-
kC:\HeroSystem\walletService\WalletService.Models\Requests\WalletRequest\InvoiceDetailsTransactionRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
WalletRequest( 5
;5 6
public 
class ,
 InvoiceDetailsTransactionRequest -
{ 
[ 
JsonProperty 
( 
$str 
) 
]  
public 

int 
	ProductId 
{ 
get 
; 
init  $
;$ %
}& '
[		 
JsonProperty		 
(		 
$str		 $
)		$ %
]		% &
public

 

int

 
PaymentGroupId

 
{

 
get

  #
;

# $
init

% )
;

) *
}

+ ,
[ 
JsonProperty 
( 
$str %
)% &
]& '
public 

bool 
AccumMinPurchase  
{! "
get# &
;& '
init( ,
;, -
}. /
[ 
JsonProperty 
( 
$str  
)  !
]! "
public 

string 
ProductName 
{ 
get  #
;# $
init% )
;) *
}+ ,
=- .
string/ 5
.5 6
Empty6 ;
;; <
[ 
JsonProperty 
( 
$str !
)! "
]" #
public 

decimal 
ProductPrice 
{  !
get" %
;% &
init' +
;+ ,
}- .
[ 
JsonProperty 
( 
$str %
)% &
]& '
public 

decimal 
ProductPriceBtc "
{# $
get% (
;( )
init* .
;. /
}0 1
[ 
JsonProperty 
( 
$str 
)  
]  !
public 

decimal 

ProductIva 
{ 
get  #
;# $
init% )
;) *
}+ ,
[ 
JsonProperty 
( 
$str $
)$ %
]% &
public 

int 
ProductQuantity 
{  
get! $
;$ %
init& *
;* +
}, -
[ 
JsonProperty 
( 
$str *
)* +
]+ ,
public 

decimal !
ProductCommissionable (
{) *
get+ .
;. /
init0 4
;4 5
}6 7
[!! 
JsonProperty!! 
(!! 
$str!! !
)!!! "
]!!" #
public"" 

decimal"" 
BinaryPoints"" 
{""  !
get""" %
;""% &
init""' +
;""+ ,
}""- .
[$$ 
JsonProperty$$ 
($$ 
$str$$ "
)$$" #
]$$# $
public%% 

int%% 
ProductPoints%% 
{%% 
get%% "
;%%" #
init%%$ (
;%%( )
}%%* +
['' 
JsonProperty'' 
('' 
$str'' $
)''$ %
]''% &
public(( 

decimal(( 
ProductDiscount(( "
{((# $
get((% (
;((( )
init((* .
;((. /
}((0 1
[** 
JsonProperty** 
(** 
$str** "
)**" #
]**# $
public++ 

int++ 
CombinationId++ 
{++ 
get++ "
;++" #
init++$ (
;++( )
}++* +
[-- 
JsonProperty-- 
(-- 
$str--  
)--  !
]--! "
public.. 

bool.. 
ProductPack.. 
{.. 
get.. !
;..! "
init..# '
;..' (
}..) *
[// 
JsonProperty// 
(// 
$str// 
)//  
]//  !
public00 

decimal00 

BaseAmount00 
{00 
get00  #
;00# $
init00% )
;00) *
}00+ ,
[11 
JsonProperty11 
(11 
$str11 $
)11$ %
]11% &
public22 

decimal22 
DailyPercentage22 "
{22# $
get22% (
;22( )
init22* .
;22. /
}220 1
[33 
JsonProperty33 
(33 
$str33  
)33  !
]33! "
public44 

int44 
WaitingDays44 
{44 
get44  
;44  !
init44" &
;44& '
}44( )
[55 
JsonProperty55 
(55 
$str55 (
)55( )
]55) *
public66 

int66 
DaysToPayQuantity66  
{66! "
get66# &
;66& '
init66( ,
;66, -
}66. /
[77 
JsonProperty77 
(77 
$str77 !
)77! "
]77" #
public88 

bool88 
ProductStart88 
{88 
get88 "
;88" #
init88$ (
;88( )
}88* +
[99 
JsonProperty99 
(99 
$str99 
)99 
]99 
public:: 

long:: 
BrandId:: 
{:: 
get:: 
;:: 
set:: 
;::  
}::  !
};; î'
dC:\HeroSystem\walletService\WalletService.Models\Requests\WalletRequest\Model1ATransactionRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
WalletRequest( 5
;5 6
public 
class %
Model1ATransactionRequest &
{ 
public 

int 
Case 
{ 
get 
; 
set 
; 
}  !
public 

decimal 
Points 
{ 
get 
;  
set! $
;$ %
}& '
public 

decimal 
EcoPoolPercentage $
{% &
get' *
;* +
set, /
;/ 0
}1 2
public 

decimal 
CompanyPercentage $
{% &
get' *
;* +
set, /
;/ 0
}1 2
public 

decimal #
CompanyPercentageLevels *
{+ ,
get- 0
;0 1
set2 5
;5 6
}7 8
public		 

int		 "
EcoPoolConfigurationId		 %
{		& '
get		( +
;		+ ,
set		- 0
;		0 1
}		2 3
public

 

double

 !
TotalPercentageLevels

 '
{

( )
get

* -
;

- .
set

/ 2
;

2 3
}

4 5
public 

ICollection 
< 
Model1AType "
>" #
EcoPoolsType$ 0
{1 2
get3 6
;6 7
set8 ;
;; <
}= >
public 

ICollection 
< 
Model1ALevelsType (
>( )

LevelsType* 4
{5 6
get7 :
;: ;
set< ?
;? @
}A B
} 
public 
class 
Model1ALevelsType 
{ 
public 

int 
Level 
{ 
get 
; 
set 
;  
}! "
public 

double 

Percentage 
{ 
get "
;" #
set$ '
;' (
}) *
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

string 
AffiliateName 
{  !
get" %
;% &
set' *
;* +
}, -
public 

int 
PoolId 
{ 
get 
; 
set  
;  !
}" #
public 

int 
Side 
{ 
get 
; 
set 
; 
}  !
public 

DateTime 
? 
UserCreatedAt "
{# $
get% (
;( )
set* -
;- .
}/ 0
} 
public 
class 
Model1AType 
{ 
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

int 
ProductExternalId  
{! "
get# &
;& '
set( +
;+ ,
}- .
public   

string   
AffiliateUserName   #
{  $ %
get  & )
;  ) *
set  + .
;  . /
}  0 1
public!! 

string!! 
ProductName!! 
{!! 
get!!  #
;!!# $
set!!% (
;!!( )
}!!* +
public"" 

int"" 
	CountDays"" 
{"" 
get"" 
;"" 
set""  #
;""# $
}""% &
public## 

int## 
DaysInMonth## 
{## 
get##  
;##  !
set##" %
;##% &
}##' (
public$$ 

decimal$$ 
Amount$$ 
{$$ 
get$$ 
;$$  
set$$! $
;$$$ %
}$$& '
public%% 

DateTime%% 
LastDayDate%% 
{%%  !
get%%" %
;%%% &
set%%' *
;%%* +
}%%, -
public&& 

DateTime&& 
PaymentDate&& 
{&&  !
get&&" %
;&&% &
set&&' *
;&&* +
}&&, -
public'' 

int'' 
PoolId'' 
{'' 
get'' 
;'' 
set''  
;''  !
}''" #
public(( 

DateTime(( 
?(( 
UserCreatedAt(( "
{((# $
get((% (
;((( )
set((* -
;((- .
}((/ 0
})) î'
dC:\HeroSystem\walletService\WalletService.Models\Requests\WalletRequest\Model1BTransactionRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
WalletRequest( 5
;5 6
public 
class %
Model1BTransactionRequest &
{ 
public 

int 
Case 
{ 
get 
; 
set 
; 
}  !
public 

decimal 
Points 
{ 
get 
;  
set! $
;$ %
}& '
public 

decimal 
EcoPoolPercentage $
{% &
get' *
;* +
set, /
;/ 0
}1 2
public 

decimal 
CompanyPercentage $
{% &
get' *
;* +
set, /
;/ 0
}1 2
public 

decimal #
CompanyPercentageLevels *
{+ ,
get- 0
;0 1
set2 5
;5 6
}7 8
public		 

int		 "
EcoPoolConfigurationId		 %
{		& '
get		( +
;		+ ,
set		- 0
;		0 1
}		2 3
public

 

double

 !
TotalPercentageLevels

 '
{

( )
get

* -
;

- .
set

/ 2
;

2 3
}

4 5
public 

ICollection 
< 
Model1BType "
>" #
EcoPoolsType$ 0
{1 2
get3 6
;6 7
set8 ;
;; <
}= >
public 

ICollection 
< 
Model1BLevelsType (
>( )

LevelsType* 4
{5 6
get7 :
;: ;
set< ?
;? @
}A B
} 
public 
class 
Model1BLevelsType 
{ 
public 

int 
Level 
{ 
get 
; 
set 
;  
}! "
public 

double 

Percentage 
{ 
get "
;" #
set$ '
;' (
}) *
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

string 
AffiliateName 
{  !
get" %
;% &
set' *
;* +
}, -
public 

int 
PoolId 
{ 
get 
; 
set  
;  !
}" #
public 

int 
Side 
{ 
get 
; 
set 
; 
}  !
public 

DateTime 
? 
UserCreatedAt "
{# $
get% (
;( )
set* -
;- .
}/ 0
} 
public 
class 
Model1BType 
{ 
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

int 
ProductExternalId  
{! "
get# &
;& '
set( +
;+ ,
}- .
public   

string   
AffiliateUserName   #
{  $ %
get  & )
;  ) *
set  + .
;  . /
}  0 1
public!! 

string!! 
ProductName!! 
{!! 
get!!  #
;!!# $
set!!% (
;!!( )
}!!* +
public"" 

int"" 
	CountDays"" 
{"" 
get"" 
;"" 
set""  #
;""# $
}""% &
public## 

int## 
DaysInMonth## 
{## 
get##  
;##  !
set##" %
;##% &
}##' (
public$$ 

decimal$$ 
Amount$$ 
{$$ 
get$$ 
;$$  
set$$! $
;$$$ %
}$$& '
public%% 

DateTime%% 
LastDayDate%% 
{%%  !
get%%" %
;%%% &
set%%' *
;%%* +
}%%, -
public&& 

DateTime&& 
PaymentDate&& 
{&&  !
get&&" %
;&&% &
set&&' *
;&&* +
}&&, -
public'' 

int'' 
PoolId'' 
{'' 
get'' 
;'' 
set''  
;''  !
}''" #
public(( 

DateTime(( 
?(( 
UserCreatedAt(( "
{((# $
get((% (
;((( )
set((* -
;((- .
}((/ 0
})) é'
cC:\HeroSystem\walletService\WalletService.Models\Requests\WalletRequest\Model2TransactionRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
WalletRequest( 5
;5 6
public 
class $
Model2TransactionRequest %
{ 
public 

int 
Case 
{ 
get 
; 
set 
; 
}  !
public 

decimal 
Points 
{ 
get 
;  
set! $
;$ %
}& '
public 

decimal 
EcoPoolPercentage $
{% &
get' *
;* +
set, /
;/ 0
}1 2
public 

decimal 
CompanyPercentage $
{% &
get' *
;* +
set, /
;/ 0
}1 2
public 

decimal #
CompanyPercentageLevels *
{+ ,
get- 0
;0 1
set2 5
;5 6
}7 8
public		 

int		 "
EcoPoolConfigurationId		 %
{		& '
get		( +
;		+ ,
set		- 0
;		0 1
}		2 3
public

 

double

 !
TotalPercentageLevels

 '
{

( )
get

* -
;

- .
set

/ 2
;

2 3
}

4 5
public 

ICollection 
< 

Model2Type !
>! "
EcoPoolsType# /
{0 1
get2 5
;5 6
set7 :
;: ;
}< =
public 

ICollection 
< 
Model2LevelsType '
>' (

LevelsType) 3
{4 5
get6 9
;9 :
set; >
;> ?
}@ A
} 
public 
class 
Model2LevelsType 
{ 
public 

int 
Level 
{ 
get 
; 
set 
;  
}! "
public 

double 

Percentage 
{ 
get "
;" #
set$ '
;' (
}) *
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

string 
AffiliateName 
{  !
get" %
;% &
set' *
;* +
}, -
public 

int 
PoolId 
{ 
get 
; 
set  
;  !
}" #
public 

int 
Side 
{ 
get 
; 
set 
; 
}  !
public 

DateTime 
? 
UserCreatedAt "
{# $
get% (
;( )
set* -
;- .
}/ 0
} 
public 
class 

Model2Type 
{ 
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

int 
ProductExternalId  
{! "
get# &
;& '
set( +
;+ ,
}- .
public   

string   
AffiliateUserName   #
{  $ %
get  & )
;  ) *
set  + .
;  . /
}  0 1
public!! 

string!! 
ProductName!! 
{!! 
get!!  #
;!!# $
set!!% (
;!!( )
}!!* +
public"" 

int"" 
	CountDays"" 
{"" 
get"" 
;"" 
set""  #
;""# $
}""% &
public## 

int## 
DaysInMonth## 
{## 
get##  
;##  !
set##" %
;##% &
}##' (
public$$ 

decimal$$ 
Amount$$ 
{$$ 
get$$ 
;$$  
set$$! $
;$$$ %
}$$& '
public%% 

DateTime%% 
LastDayDate%% 
{%%  !
get%%" %
;%%% &
set%%' *
;%%* +
}%%, -
public&& 

DateTime&& 
PaymentDate&& 
{&&  !
get&&" %
;&&% &
set&&' *
;&&* +
}&&, -
public'' 

int'' 
PoolId'' 
{'' 
get'' 
;'' 
set''  
;''  !
}''" #
public(( 

DateTime(( 
?(( 
UserCreatedAt(( "
{((# $
get((% (
;((( )
set((* -
;((- .
}((/ 0
})) ö#
cC:\HeroSystem\walletService\WalletService.Models\Requests\WalletRequest\Model3TransactionRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
WalletRequest( 5
;5 6
public 
class $
Model3TransactionRequest %
{ 
public 

int 
Case 
{ 
get 
; 
set 
; 
}  !
public 

decimal 

Percentage 
{ 
get  #
;# $
set% (
;( )
}* +
public 

int "
EcoPoolConfigurationId %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
public 

double !
TotalPercentageLevels '
{( )
get* -
;- .
set/ 2
;2 3
}4 5
public		 

ICollection		 
<		 

Model3Type		 !
>		! "
EcoPoolsType		# /
{		0 1
get		2 5
;		5 6
set		7 :
;		: ;
}		< =
public

 

ICollection

 
<

 
Model3LevelsType

 '
>

' (

LevelsType

) 3
{

4 5
get

6 9
;

9 :
set

; >
;

> ?
}

@ A
} 
public 
class 
Model3LevelsType 
{ 
public 

int 
Level 
{ 
get 
; 
set 
;  
}! "
public 

double 

Percentage 
{ 
get "
;" #
set$ '
;' (
}) *
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

string 
AffiliateName 
{  !
get" %
;% &
set' *
;* +
}, -
public 

int 
PoolId 
{ 
get 
; 
set  
;  !
}" #
public 

int 
Side 
{ 
get 
; 
set 
; 
}  !
public 

DateTime 
? 
UserCreatedAt "
{# $
get% (
;( )
set* -
;- .
}/ 0
} 
public 
class 

Model3Type 
{ 
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

int 
ProductExternalId  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

string 
AffiliateUserName #
{$ %
get& )
;) *
set+ .
;. /
}0 1
public 

string 
ProductName 
{ 
get  #
;# $
set% (
;( )
}* +
public   

int   
	CountDays   
{   
get   
;   
set    #
;  # $
}  % &
public!! 

int!! 
DaysInMonth!! 
{!! 
get!!  
;!!  !
set!!" %
;!!% &
}!!' (
public"" 

decimal"" 
Amount"" 
{"" 
get"" 
;""  
set""! $
;""$ %
}""& '
public## 

DateTime## 
LastDayDate## 
{##  !
get##" %
;##% &
set##' *
;##* +
}##, -
public$$ 

DateTime$$ 
PaymentDate$$ 
{$$  !
get$$" %
;$$% &
set$$' *
;$$* +
}$$, -
public%% 

int%% 
PoolId%% 
{%% 
get%% 
;%% 
set%%  
;%%  !
}%%" #
public&& 

DateTime&& 
?&& 
UserCreatedAt&& "
{&&# $
get&&% (
;&&( )
set&&* -
;&&- .
}&&/ 0
}'' ©
XC:\HeroSystem\walletService\WalletService.Models\Requests\WalletRequest\WalletRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
WalletRequest( 5
;5 6
public 
class 
WalletRequest 
{ 
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

string 
AffiliateUserName #
{$ %
get& )
;) *
set+ .
;. /
}0 1
=2 3
null4 8
!8 9
;9 :
public 

int 
PurchaseFor 
{ 
get  
;  !
set" %
;% &
}' (
public 

string 
? 
Bank 
{ 
get 
; 
set "
;" #
}$ %
public

 

int

 
PaymentMethod

 
{

 
get

 "
;

" #
set

$ '
;

' (
}

) *
public 

string 
? 
	SecretKey 
{ 
get "
;" #
set$ '
;' (
}) *
public 

string 
? 
ReceiptNumber  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

long 
BrandId 
{ 
get 
; 
set "
;" #
}$ %
public 

ICollection 
< 
ProductsRequests '
>' (
ProductsList) 5
{6 7
get8 ;
;; <
set= @
;@ A
}B C
} 
public 
class 
ProductsRequests 
{ 
public 

int 
	IdProduct 
{ 
get 
; 
set  #
;# $
}% &
public 

int 
Count 
{ 
get 
; 
set 
;  
}! "
} ˘
vC:\HeroSystem\walletService\WalletService.Models\Requests\WalletRetentionConfigRequest\WalletRetentionConfigRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' ((
WalletRetentionConfigRequest( D
;D E
public 
class (
WalletRetentionConfigRequest )
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

decimal 
WithdrawalFrom !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 

decimal 
WithdrawalTo 
{  !
get" %
;% &
set' *
;* +
}, -
public 

decimal 

Percentage 
{ 
get  #
;# $
set% (
;( )
}* +
public		 

DateTime		 
Date		 
{		 
get		 
;		 
set		  #
;		# $
}		% &
public

 

DateTime

 
?

 
DisableDate

  
{

! "
get

# &
;

& '
set

( +
;

+ ,
}

- .
public 

bool 
Status 
{ 
get 
; 
set !
;! "
}# $
} Ω
nC:\HeroSystem\walletService\WalletService.Models\Requests\WalletTransactionRequest\WalletTransactionRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' ($
WalletTransactionRequest( @
;@ A
public 
class $
WalletTransactionRequest %
{ 
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

int 
? 
UserId 
{ 
get 
; 
set !
;! "
}# $
public		 

decimal		 
?		 
Credit		 
{		 
get		  
;		  !
set		" %
;		% &
}		' (
public

 

decimal

 
?

 
Debit

 
{

 
get

 
;

  
set

! $
;

$ %
}

& '
public 

decimal 
? 
Deferred 
{ 
get "
;" #
set$ '
;' (
}) *
public 

bool 
? 
Status 
{ 
get 
; 
set "
;" #
}$ %
public 

string 
Concept 
{ 
get 
;  
set! $
;$ %
}& '
public 

string 
Support 
{ 
get 
;  
set! $
;$ %
}& '
public 

DateTime 
Date 
{ 
get 
; 
set  #
;# $
}% &
public 

bool 
? 
Compression 
{ 
get "
;" #
set$ '
;' (
}) *
public 

string 
? 
Detail 
{ 
get 
;  
set! $
;$ %
}& '
public 

DateTime 
CreateAt 
{ 
get "
;" #
set$ '
;' (
}) *
public 

DateTime 
UpdateAt 
{ 
get "
;" #
set$ '
;' (
}) *
public 

DateTime 
? 
DeleteAt 
{ 
get  #
;# $
set% (
;( )
}* +
public 

string 
? 
AffiliateUserName $
{% &
get' *
;* +
set, /
;/ 0
}1 2
public 

string 
? 
AdminUserName  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

WalletConceptType 
? 
ConceptType )
{* +
get, /
;/ 0
set1 4
;4 5
}6 7
public 

long 
BrandId 
{ 
get 
; 
set "
;" #
}$ %
} ë
`C:\HeroSystem\walletService\WalletService.Models\Requests\WalletWaitRequest\WalletWaitRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (
WalletWaitRequest( 9
;9 :
public 
class 
WalletWaitRequest 
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

decimal 
? 
Credit 
{ 
get  
;  !
set" %
;% &
}' (
public 

string 
? 
PaymentMethod  
{! "
get# &
;& '
set( +
;+ ,
}- .
public		 

string		 
?		 
Bank		 
{		 
get		 
;		 
set		 "
;		" #
}		$ %
public

 

string

 
?

 
Support

 
{

 
get

  
;

  !
set

" %
;

% &
}

' (
public 

DateTime 
? 
DepositDate  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

bool 
? 
Status 
{ 
get 
; 
set "
;" #
}$ %
public 

bool 
? 
Attended 
{ 
get 
;  
set! $
;$ %
}& '
public 

DateTime 
Date 
{ 
get 
; 
set  #
;# $
}% &
public 

string 
? 
Order 
{ 
get 
; 
set  #
;# $
}% &
} Å
lC:\HeroSystem\walletService\WalletService.Models\Requests\WalletWithDrawalRequest\WalletWithDrawalRequest.cs
	namespace 	
WalletService
 
. 
Models 
. 
Requests '
.' (#
WalletWithDrawalRequest( ?
;? @
public 
class #
WalletWithDrawalRequest $
{ 
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

string 
? 
AffiliateUserName $
{% &
get' *
;* +
set, /
;/ 0
}1 2
public 

decimal 
Amount 
{ 
get 
;  
set! $
;$ %
}& '
public 

string 
? 
Observation 
{  
get! $
;$ %
set& )
;) *
}+ ,
public		 

string		 
?		 
AdminObservation		 #
{		$ %
get		& )
;		) *
set		+ .
;		. /
}		0 1
public

 

DateTime

 
Date

 
{

 
get

 
;

 
set

  #
;

# $
}

% &
public 

DateTime 
? 
ResponseDate !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 

decimal 
RetentionPercentage &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
public 

bool 
IsProcessed 
{ 
get !
;! "
set# &
;& '
}( )
public 

bool 
Status 
{ 
get 
; 
set !
;! "
}# $
} ù
[C:\HeroSystem\walletService\WalletService.Models\Responses\BaseResponses\CoinPayResponse.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Responses (
.( )
BaseResponses) 6
;6 7
public 
class 
CoinPayResponse 
{ 
public 

	DataModel 
Data 
{ 
get 
;  
set! $
;$ %
}& '
public 

int 

StatusCode 
{ 
get 
;  
set! $
;$ %
}& '
public 

int 
IdTypeStatusCode 
{  !
get" %
;% &
set' *
;* +
}, -
public 

string 
Message 
{ 
get 
;  
set! $
;$ %
}& '
public		 

string		 
[		 
]		 
Messages		 
{		 
get		 "
;		" #
set		$ '
;		' (
}		) *
}

 
public 
class 
	DataModel 
{ 
public 

int 
IdUser 
{ 
get 
; 
set  
;  !
}" #
public 

string 
Token 
{ 
get 
; 
set "
;" #
}$ %
} ˝	
aC:\HeroSystem\walletService\WalletService.Models\Responses\BaseResponses\CreateAddressResponse.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Responses (
.( )
BaseResponses) 6
;6 7
public 
class !
CreateAddressResponse "
{ 
public 
Data 
? 
Data 
{ 
get 
;  
set! $
;$ %
}& '
public 
int 

StatusCode 
{ 
get  #
;# $
set% (
;( )
}* +
public 
int 
IdTypeStatusCode #
{$ %
get& )
;) *
set+ .
;. /
}0 1
public 
string 
? 
Message 
{  
get! $
;$ %
set& )
;) *
}+ ,
}		 
public 
class 
Data 
{ 
public 
string 
? 
Address 
{  
get! $
;$ %
set& )
;) *
}+ ,
} Ì
YC:\HeroSystem\walletService\WalletService.Models\Responses\BaseResponses\IRestResponse.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Responses (
.( )
BaseResponses) 6
;6 7
public 
	interface 
IRestResponse 
{ 
public 	
string
 
? 
Content 
{ 
get 
;  
set! $
;$ %
}& '
public 	
HttpStatusCode
 

StatusCode #
{$ %
get& )
;) *
set+ .
;. /
}0 1
public		 	
string		
 
?		 
StatusDescription		 #
{		$ %
get		& )
;		) *
set		+ .
;		. /
}		0 1
public

 
bool


 
IsSuccessful

 
{

 
get

 !
;

! "
}

# $
} ¡
\C:\HeroSystem\walletService\WalletService.Models\Responses\BaseResponses\PagaditoResponse.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Responses (
.( )
BaseResponses) 6
;6 7
public 
class 
PagaditoResponse 
{ 
[ 
JsonPropertyName 
( 
$str 
) 
] 
public 

string 
? 
Code 
{ 
get 
; 
set "
;" #
}$ %
[

 
JsonPropertyName

 
(

 
$str

 
)

  
]

  !
public 

string 
? 
Message 
{ 
get  
;  !
set" %
;% &
}' (
[ 
JsonPropertyName 
( 
$str 
) 
] 
public 

string 
? 
Value 
{ 
get 
; 
set  #
;# $
}% &
[ 
JsonPropertyName 
( 
$str  
)  !
]! "
public 

string 
? 
Datetime 
{ 
get !
;! "
set# &
;& '
}( )
} ’
XC:\HeroSystem\walletService\WalletService.Models\Responses\BaseResponses\RestResponse.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Responses (
.( )
BaseResponses) 6
;6 7
public 
class 
RestResponse 
: 
IRestResponse (
{ 
public 

string 
? 
Content 
{ 
get  
;  !
set" %
;% &
}' (
public 

HttpStatusCode 

StatusCode $
{% &
get' *
;* +
set, /
;/ 0
}1 2
public		 

string		 
?		 
StatusDescription		 $
{		% &
get		' *
;		* +
set		, /
;		/ 0
}		1 2
public

 

bool

 
IsSuccessful

 
=>

 

StatusCode

  *
==

+ -
HttpStatusCode

. <
.

< =
OK

= ?
;

? @
} Ò
[C:\HeroSystem\walletService\WalletService.Models\Responses\CoinPaymentWithdrawalResponse.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Responses (
;( )
public 
class )
CoinPaymentWithdrawalResponse *
{ 
[ 
JsonPropertyName 
( 
$str 
) 
] 
public  &
string' -
?- .
Error/ 4
{5 6
get7 :
;: ;
set< ?
;? @
}A B
[ 
JsonPropertyName 
( 
$str 
) 
]  
public! '

Dictionary( 2
<2 3
string3 9
,9 :
WithdrawalInfo; I
>I J
?J K
ResultL R
{S T
getU X
;X Y
setZ ]
;] ^
}_ `
}		 
public 
class 
WithdrawalInfo 
{ 
[ 
JsonPropertyName 
( 
$str 
) 
] 
public  &
string' -
?- .
Error/ 4
{5 6
get7 :
;: ;
set< ?
;? @
}A B
[ 
JsonPropertyName 
( 
$str 
) 
] 
public #
string$ *
?* +
Id, .
{/ 0
get1 4
;4 5
set6 9
;9 :
}; <
[ 
JsonPropertyName 
( 
$str 
) 
]  
public! '
int( +
Status, 2
{3 4
get5 8
;8 9
set: =
;= >
}? @
[ 
JsonPropertyName 
( 
$str 
) 
]  
public! '
string( .
?. /
Amount0 6
{7 8
get9 <
;< =
set> A
;A B
}C D
} Æ
SC:\HeroSystem\walletService\WalletService.Models\Responses\CreateChannelResponse.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Responses (
;( )
public 

class !
CreateChannelResponse &
{ 
public 
ChannelData 
? 
Data  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 
int 

StatusCode 
{ 
get  #
;# $
set% (
;( )
}* +
public 
int 
IdTypeStatusCode #
{$ %
get& )
;) *
set+ .
;. /
}0 1
public		 
string		 
?		 
Message		 
{		  
get		! $
;		$ %
set		& )
;		) *
}		+ ,
}

 
public 

class 
ChannelData 
{ 
public 
int 
Id 
{ 
get 
; 
set  
;  !
}" #
public 
int $
IdExternalIdentification +
{, -
get. 1
;1 2
set3 6
;6 7
}8 9
public 
string 
? 
TagName 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
Currency 
? 
Currency !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
string 
? 
Address 
{  
get! $
;$ %
set& )
;) *
}+ ,
} 
public 

class 
Currency 
{ 
public 
int 
Id 
{ 
get 
; 
set  
;  !
}" #
public 
string 
? 
Name 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
? 
Code 
{ 
get !
;! "
set# &
;& '
}( )
} Ó
bC:\HeroSystem\walletService\WalletService.Models\Responses\CreateConPaymentsTransactionResponse.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Responses (
;( )
public 
class 0
$CreateConPaymentsTransactionResponse 1
{ 
[		 
JsonProperty		 
(		 
$str		 
)		 
]		 
public

 

string

 
?

 
Error

 
{

 
get

 
;

 
set

  #
;

# $
}

% &
[ 
JsonProperty 
( 
$str 
) 
] 
public 

PaymentResult 
? 
Result  
{! "
get# &
;& '
set( +
;+ ,
}- .
} 
public 
class 
PaymentResult 
{ 
[ 
JsonProperty 
( 
$str 
) 
] 
public 

string 
? 
Amount 
{ 
get 
;  
set! $
;$ %
}& '
[ 
JsonProperty 
( 
$str 
) 
] 
public 

string 
? 
Txn_Id 
{ 
get 
;  
set! $
;$ %
}& '
[ 
JsonProperty 
( 
$str 
) 
] 
public 

string 
? 
Address 
{ 
get  
;  !
set" %
;% &
}' (
[ 
JsonProperty 
( 
$str #
)# $
]$ %
public 

string 
? 
Confirms_Needed "
{# $
get% (
;( )
set* -
;- .
}/ 0
[ 
JsonProperty 
( 
$str 
) 
] 
public 

int 
Timeout 
{ 
get 
; 
set !
;! "
}# $
[ 
JsonProperty 
( 
$str  
)  !
]! "
public 

string 
? 
Checkout_Url 
{  !
get" %
;% &
set' *
;* +
}, -
[ 
JsonProperty 
( 
$str 
) 
]  
public 

string 
? 

Status_Url 
{ 
get  #
;# $
set% (
;( )
}* +
[   
JsonProperty   
(   
$str   
)   
]    
public!! 

string!! 
?!! 

Qrcode_Url!! 
{!! 
get!!  #
;!!# $
set!!% (
;!!( )
}!!* +
}"" ƒ!
WC:\HeroSystem\walletService\WalletService.Models\Responses\CreateTransactionResponse.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Responses (
;( )
public 
class %
CreateTransactionResponse &
{ 
public 

TransactionData 
? 
Data  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

int 

StatusCode 
{ 
get 
;  
set! $
;$ %
}& '
public 

int 
IdTypeStatusCode 
{  !
get" %
;% &
set' *
;* +
}, -
public 

string 
? 
CodeMessage 
{  
get! $
;$ %
set& )
;) *
}+ ,
public		 

string		 
?		 
CodeMessage2		 
{		  !
get		" %
;		% &
set		' *
;		* +
}		, -
public

 

string

 
?

 
Message

 
{

 
get

  
;

  !
set

" %
;

% &
}

' (
public 

int 
IdTransaction 
{ 
get "
;" #
set$ '
;' (
}) *
public 

int "
IdTypeNotificationSent %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
} 
public 
class 
TransactionData 
{ 
public 

int 
IdTransaction 
{ 
get "
;" #
set$ '
;' (
}) *
public 

string 
? 
Date 
{ 
get 
; 
set "
;" #
}$ %
public 

int 
IdUser 
{ 
get 
; 
set  
;  !
}" #
public 

int 
IdTeller 
{ 
get 
; 
set "
;" #
}$ %
public 

CurrencyInfo 
? 
Currency !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 

decimal 
Amount 
{ 
get 
;  
set! $
;$ %
}& '
public 

string 
? 
Details 
{ 
get  
;  !
set" %
;% &
}' (
public 

string 
? 
QrCode 
{ 
get 
;  
set! $
;$ %
}& '
public 

object 
? 
CustomerNotified #
{$ %
get& )
;) *
set+ .
;. /
}0 1
} 
public 
class 
CurrencyInfo 
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

string 
? 
Name 
{ 
get 
; 
set "
;" #
}$ %
public   

string   
?   
Code   
{   
get   
;   
set   "
;  " #
}  $ %
public!! 

bool!! 
IsErc!! 
{!! 
get!! 
;!! 
set!!  
;!!  !
}!!" #
public"" 

bool"" 
IsDigitalCurrency"" !
{""" #
get""$ '
;""' (
set"") ,
;"", -
}"". /
public## 

string## 
?## 
Description## 
{##  
get##! $
;##$ %
set##& )
;##) *
}##+ ,
}$$ ‡ 
XC:\HeroSystem\walletService\WalletService.Models\Responses\GetAccountsEcoPoolResponse.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Responses (
;( )
public 
class &
GetAccountsEcoPoolResponse '
{ 
public 

bool 
success 
{ 
get 
; 
set "
;" #
}$ %
public 

List 
< 
UserModelResponse !
>! "
data# '
{( )
get* -
;- .
set/ 2
;2 3
}4 5
public		 

string		 
message		 
{		 
get		 
;		  
set		! $
;		$ %
}		& '
public

 

int

 
code

 
{

 
get

 
;

 
set

 
;

 
}

  !
} 
public 
class 
UserModelResponse 
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

string 
UserName 
{ 
get  
;  !
set" %
;% &
}' (
public 

string 
Email 
{ 
get 
; 
set "
;" #
}$ %
public 

string 
? 
Name 
{ 
get 
; 
set "
;" #
}$ %
public 

string 
? 
LastName 
{ 
get !
;! "
set# &
;& '
}( )
public 

int 
Level 
{ 
get 
; 
set 
;  
}! "
public 

int 
Father 
{ 
get 
; 
set  
;  !
}" #
public 

int 
Side 
{ 
get 
; 
set 
; 
}  !
public 

DateTime 
	CreatedAt 
{ 
get  #
;# $
set% (
;( )
}* +
public   

List   
<   
	UserLevel   
>   

FamilyTree   %
{  & '
get  ( +
;  + ,
set  - 0
;  0 1
}  2 3
}!! 
public## 
class## 
	UserLevel## 
{$$ 
public%% 

int%% 
Id%% 
{%% 
get%% 
;%% 
set%% 
;%% 
}%% 
public'' 

string'' 
UserName'' 
{'' 
get''  
;''  !
set''" %
;''% &
}''' (
public)) 

string)) 
Email)) 
{)) 
get)) 
;)) 
set)) "
;))" #
}))$ %
public++ 

string++ 
?++ 
Name++ 
{++ 
get++ 
;++ 
set++ "
;++" #
}++$ %
public-- 

string-- 
?-- 
LastName-- 
{-- 
get-- !
;--! "
set--# &
;--& '
}--( )
public// 

int// 
Level// 
{// 
get// 
;// 
set// 
;//  
}//! "
public11 

int11 
Father11 
{11 
get11 
;11 
set11  
;11  !
}11" #
public33 

int33 
Side33 
{33 
get33 
;33 
set33 
;33 
}33  !
public44 

DateTime44 
	CreatedAt44 
{44 
get44  #
;44# $
set44% (
;44( )
}44* +
}55 Ω	
ZC:\HeroSystem\walletService\WalletService.Models\Responses\GetBalanceByCurrencyResponse.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Responses (
;( )
public 
class (
GetBalanceByCurrencyResponse )
{ 
public 

int 

StatusCode 
{ 
get 
;  
set! $
;$ %
}& '
public 

int 
IdTypeStatusCode 
{  !
get" %
;% &
set' *
;* +
}, -
public		 

string		 
?		 
Message		 
{		 
get		  
;		  !
set		" %
;		% &
}		' (
public

 

List

 
<

 
object

 
>

 
?

 
Messages

 !
{

" #
get

$ '
;

' (
set

) ,
;

, -
}

. /
public 

UserBalanceDto 
? 
Data 
{  !
get" %
;% &
set' *
;* +
}, -
} Ÿ	
PC:\HeroSystem\walletService\WalletService.Models\Responses\GetBalanceResponse.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Responses (
;( )
public 
class 
GetBalanceResponse 
{ 
public 

int 

StatusCode 
{ 
get 
;  
set! $
;$ %
}& '
public 

int 
IdTypeStatusCode 
{  !
get" %
;% &
set' *
;* +
}, -
public		 

string		 
?		 
Message		 
{		 
get		  
;		  !
set		" %
;		% &
}		' (
public

 

List

 
<

 
object

 
>

 
?

 
Messages

 !
{

" #
get

$ '
;

' (
set

) ,
;

, -
}

. /
public 

List 
< 
UserBalanceDto 
> 
?  
Data! %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
} È
RC:\HeroSystem\walletService\WalletService.Models\Responses\GetBasicInfoResponse.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Responses (
;( )
public 
class  
GetBasicInfoResponse !
{ 
public 

string 
? 
Error 
{ 
get 
; 
set  #
;# $
}% &
public 


InfoResult 
? 
Result 
{ 
get  #
;# $
set% (
;( )
}* +
} 
public		 
class		 

InfoResult		 
{

 
} Œ
UC:\HeroSystem\walletService\WalletService.Models\Responses\GetCoinBalancesResponse.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Responses (
;( )
public 
class #
GetCoinBalancesResponse $
{ 
public 

string 
? 
Error 
{ 
get 
; 
set  #
;# $
}% &
public 


Dictionary 
< 
string 
, 
CoinInfo &
>& '
Result( .
{/ 0
get1 4
;4 5
set6 9
;9 :
}; <
}

 
public 
class 
CoinInfo 
{ 
[ 
JsonPropertyName 
( 
$str 
)  
]  !
public 

long 
Balance 
{ 
get 
; 
set "
;" #
}$ %
[ 
JsonPropertyName 
( 
$str  
)  !
]! "
public 

string 
BalanceF 
{ 
get  
;  !
set" %
;% &
}' (
=) *
string+ 1
.1 2
Empty2 7
;7 8
[ 
JsonPropertyName 
( 
$str 
) 
]  
public 

string 
Status 
{ 
get 
; 
set  #
;# $
}% &
=' (
string) /
./ 0
Empty0 5
;5 6
[ 
JsonPropertyName 
( 
$str #
)# $
]$ %
public 

string 

CoinStatus 
{ 
get "
;" #
set$ '
;' (
}) *
=+ ,
string- 3
.3 4
Empty4 9
;9 :
} Ë
WC:\HeroSystem\walletService\WalletService.Models\Responses\GetDepositAddressResponse.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Responses (
;( )
public 
class %
GetDepositAddressResponse &
{ 
public 

string 
? 
Error 
{ 
get 
; 
set  #
;# $
}% &
public 

AddressResult 
? 
Result  
{! "
get# &
;& '
set( +
;+ ,
}- .
} 
public		 
class		 
AddressResult		 
{

 
public 

string 
Address 
{ 
get 
;  
set! $
;$ %
}& '
=( )
string* 0
.0 1
Empty1 6
;6 7
} ì
PC:\HeroSystem\walletService\WalletService.Models\Responses\GetNetworkResponse.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Responses (
;( )
public 
class 
GetNetworkResponse 
{ 
public 

List 
< 
NetworkData 
> 
? 
Data "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 

int 

StatusCode 
{ 
get 
;  
set! $
;$ %
}& '
public 

int 
IdTypeStatusCode 
{  !
get" %
;% &
set' *
;* +
}, -
public 

string 
? 
Message 
{ 
get  
;  !
set" %
;% &
}' (
}		 
public 
class 
NetworkData 
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

int 
IdChain 
{ 
get 
; 
set !
;! "
}# $
public 

string 
? 
Name 
{ 
get 
; 
set "
;" #
}$ %
public 

string 
? 
	ShortName 
{ 
get "
;" #
set$ '
;' (
}) *
public 

decimal !
MinimunTransferAmount (
{) *
get+ .
;. /
set0 3
;3 4
}5 6
public 

decimal 
Fee 
{ 
get 
; 
set !
;! "
}# $
} ä&
YC:\HeroSystem\walletService\WalletService.Models\Responses\GetPayByNameProfileResponse.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Responses (
;( )
public 
class '
GetPayByNameProfileResponse (
{ 
public 

string 
Error 
{ 
get 
; 
set "
;" #
}$ %
=& '
string( .
.. /
Empty/ 4
;4 5
public		 

ProfileResult		 
?		 
Result		  
{		! "
get		# &
;		& '
set		( +
;		+ ,
}		- .
}

 
public 
class 
ProfileResult 
{ 
[ 
JsonPropertyName 
( 
$str 
) 
]  
public 

string 
? 
Pbntag 
{ 
get 
;  
set! $
;$ %
}& '
[ 
JsonPropertyName 
( 
$str  
)  !
]! "
public 

string 
? 
Merchant 
{ 
get !
;! "
set# &
;& '
}( )
[ 
JsonPropertyName 
( 
$str $
)$ %
]% &
public 

string 
? 
ProfileName 
{  
get! $
;$ %
set& )
;) *
}+ ,
[ 
JsonPropertyName 
( 
$str #
)# $
]$ %
public 

string 
? 

ProfileUrl 
{ 
get  #
;# $
set% (
;( )
}* +
[ 
JsonPropertyName 
( 
$str %
)% &
]& '
public 

string 
? 
ProfileEmail 
{  !
get" %
;% &
set' *
;* +
}, -
[ 
JsonPropertyName 
( 
$str %
)% &
]& '
public 

string 
? 
ProfileImage 
{  !
get" %
;% &
set' *
;* +
}, -
[   
JsonPropertyName   
(   
$str   $
)  $ %
]  % &
public!! 

long!! 
MemberSince!! 
{!! 
get!! !
;!!! "
set!!# &
;!!& '
}!!( )
[## 
JsonPropertyName## 
(## 
$str##  
)##  !
]##! "
public$$ 

Feedback$$ 
?$$ 
Feedback$$ 
{$$ 
get$$  #
;$$# $
set$$% (
;$$( )
}$$* +
}&& 
public(( 
class(( 
Feedback(( 
{)) 
[** 
JsonPropertyName** 
(** 
$str** 
)** 
]** 
public++ 

int++ 
Pos++ 
{++ 
get++ 
;++ 
set++ 
;++ 
}++  
[-- 
JsonPropertyName-- 
(-- 
$str-- 
)-- 
]-- 
public.. 

int.. 
Neg.. 
{.. 
get.. 
;.. 
set.. 
;.. 
}..  
[00 
JsonPropertyName00 
(00 
$str00 
)00 
]00 
public11 

int11 
Neut11 
{11 
get11 
;11 
set11 
;11 
}11  !
[33 
JsonPropertyName33 
(33 
$str33 
)33 
]33 
public44 

int44 
Total44 
{44 
get44 
;44 
set44 
;44  
}44! "
[66 
JsonPropertyName66 
(66 
$str66 
)66  
]66  !
public77 

string77 
Percent77 
{77 
get77 
;77  
set77! $
;77$ %
}77& '
=77( )
string77* 0
.770 1
Empty771 6
;776 7
[99 
JsonPropertyName99 
(99 
$str99 #
)99# $
]99$ %
public:: 

string:: 

PercentStr:: 
{:: 
get:: "
;::" #
set::$ '
;::' (
}::) *
=::+ ,
string::- 3
.::3 4
Empty::4 9
;::9 :
};; ˜&
XC:\HeroSystem\walletService\WalletService.Models\Responses\GetTransactionByIdResponse.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Responses (
;( )
public 
partial 
class &
GetTransactionByIdResponse /
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

int 
IdUser 
{ 
get 
; 
set  
;  !
}" #
public 

int 
IdWallet 
{ 
get 
; 
set "
;" #
}$ %
public 

int 
Amount 
{ 
get 
; 
set  
;  !
}" #
public		 

int		 
Fee		 
{		 
get		 
;		 
set		 
;		 
}		  
public

 

int

 
ExchangeRate

 
{

 
get

 !
;

! "
set

# &
;

& '
}

( )
public 

int 
ExchangeRateFlat 
{  !
get" %
;% &
set' *
;* +
}, -
public 

string 
Address 
{ 
get 
;  
set! $
;$ %
}& '
public 

string 
TxtId 
{ 
get 
; 
set "
;" #
}$ %
public 

string 
Description 
{ 
get  #
;# $
set% (
;( )
}* +
public 

Currency 
Currency 
{ 
get "
;" #
set$ '
;' (
}) *
public 

TransactionType 
TransactionType *
{+ ,
get- 0
;0 1
set2 5
;5 6
}7 8
public 

TransactionStatus 
TransactionStatus .
{/ 0
get1 4
;4 5
set6 9
;9 :
}; <
public 

PaymentMethod 
PaymentMethod &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
public 

DateTime 
CreateTransaction %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
public 

DateTime 
ModifiedTransaction '
{( )
get* -
;- .
set/ 2
;2 3
}4 5
} 
public 
class 
TransactionType 
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

string 
Name 
{ 
get 
; 
set !
;! "
}# $
} 
public 
class 
TransactionStatus 
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

string 
Name 
{ 
get 
; 
set !
;! "
}# $
}   
public"" 
class"" 
PaymentMethod"" 
{## 
public$$ 

int$$ 
Id$$ 
{$$ 
get$$ 
;$$ 
set$$ 
;$$ 
}$$ 
public%% 

string%% 
Name%% 
{%% 
get%% 
;%% 
set%% !
;%%! "
}%%# $
public&& 

string&& 
Description&& 
{&& 
get&&  #
;&&# $
set&&% (
;&&( )
}&&* +
}'' 
public)) 
partial)) 
class)) &
GetTransactionByIdResponse)) /
{** 
public++ 

TransactionData++ 
Data++ 
{++  !
get++" %
;++% &
set++' *
;++* +
}++, -
public,, 

int,, 

StatusCode,, 
{,, 
get,, 
;,,  
set,,! $
;,,$ %
},,& '
public-- 

int-- 
IdTypeStatusCode-- 
{--  !
get--" %
;--% &
set--' *
;--* +
}--, -
public.. 

string.. 
Message.. 
{.. 
get.. 
;..  
set..! $
;..$ %
}..& '
}// …%
XC:\HeroSystem\walletService\WalletService.Models\Responses\GetTransactionInfoResponse.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Responses (
;( )
public 
class &
GetTransactionInfoResponse '
{ 
public 

string 
Error 
{ 
get 
; 
set "
;" #
}$ %
public 

TransactionResult 
Result #
{$ %
get& )
;) *
set+ .
;. /
}0 1
} 
public		 
class		 
TransactionResult		 
{

 
public 

long 
TimeCreated 
{ 
get !
;! "
set# &
;& '
}( )
public 

long 
TimeExpires 
{ 
get !
;! "
set# &
;& '
}( )
public 

int 
Status 
{ 
get 
; 
set  
;  !
}" #
public 

string 

StatusText 
{ 
get "
;" #
set$ '
;' (
}) *
public 

string 
Type 
{ 
get 
; 
set !
;! "
}# $
public 

string 
Coin 
{ 
get 
; 
set !
;! "
}# $
public 

long 
Amount 
{ 
get 
; 
set !
;! "
}# $
public 

string 
Amountf 
{ 
get 
;  
set! $
;$ %
}& '
public 

long 
Received 
{ 
get 
; 
set  #
;# $
}% &
public 

string 
	Receivedf 
{ 
get !
;! "
set# &
;& '
}( )
public 

int 
RecvConfirms 
{ 
get !
;! "
set# &
;& '
}( )
public 

string 
PaymentAddress  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

string 
SenderIp 
{ 
get  
;  !
set" %
;% &
}' (
public 

CheckoutInfo 
Checkout  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

List 
< 
object 
> 
Shipping  
{! "
get# &
;& '
set( +
;+ ,
}- .
} 
public 
class 
CheckoutInfo 
{ 
public 

string 
Currency 
{ 
get  
;  !
set" %
;% &
}' (
public 

long 
Amount 
{ 
get 
; 
set !
;! "
}# $
public   

int   
Test   
{   
get   
;   
set   
;   
}    !
public!! 

string!! 

ItemNumber!! 
{!! 
get!! "
;!!" #
set!!$ '
;!!' (
}!!) *
public"" 

string"" 
ItemName"" 
{"" 
get""  
;""  !
set""" %
;""% &
}""' (
public## 

List## 
<## 
object## 
>## 
Details## 
{##  !
get##" %
;##% &
set##' *
;##* +
}##, -
public$$ 

string$$ 
Invoice$$ 
{$$ 
get$$ 
;$$  
set$$! $
;$$$ %
}$$& '
public%% 

string%% 
Custom%% 
{%% 
get%% 
;%% 
set%%  #
;%%# $
}%%% &
public&& 

string&& 
IpnUrl&& 
{&& 
get&& 
;&& 
set&&  #
;&&# $
}&&% &
public'' 

decimal'' 
Amountf'' 
{'' 
get''  
;''  !
set''" %
;''% &
}''' (
}(( Ã
MC:\HeroSystem\walletService\WalletService.Models\Responses\GradingResponse.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Responses (
;( )
public 
class 
GradingResponse 
{ 
[		 
JsonPropertyName		 
(		 
$str		 
)		  
]		  !
public		" (
bool		) -
Success		. 5
{		6 7
get		8 ;
;		; <
set		= @
;		@ A
}		B C
[

 
JsonPropertyName

 
(

 
$str

 
)

 
]

 
public

 %
List

& *
<

* +

GradingDto

+ 5
>

5 6
Data

7 ;
{

< =
get

> A
;

A B
set

C F
;

F G
}

H I
[ 
JsonPropertyName 
( 
$str 
)  
]  !
public" (
string) /
Message0 7
{8 9
get: =
;= >
set? B
;B C
}D E
=F G
stringH N
.N O
EmptyO T
;T U
[ 
JsonPropertyName 
( 
$str 
) 
] 
public %
int& )
Code* .
{/ 0
get1 4
;4 5
set6 9
;9 :
}; <
} À
NC:\HeroSystem\walletService\WalletService.Models\Responses\GradingsResponse.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Responses (
;( )
public 
class 
GradingsResponse 
{ 
public		 

class		 
ProductsResponse		 !
{

 
[ 	
JsonPropertyName	 
( 
$str #
)# $
]$ %
public& ,
bool- 1
Success2 9
{: ;
get< ?
;? @
setA D
;D E
}F G
[ 	
JsonPropertyName	 
( 
$str  
)  !
]! "
public# )
ICollection* 5
<5 6
	WalletDto6 ?
>? @
DataA E
{F G
getH K
;K L
setM P
;P Q
}R S
=T U
newV Y
ListZ ^
<^ _
	WalletDto_ h
>h i
(i j
)j k
;k l
[ 	
JsonPropertyName	 
( 
$str #
)# $
]$ %
public& ,
string- 3
Message4 ;
{< =
get> A
;A B
setC F
;F G
}H I
=J K
stringL R
.R S
EmptyS X
;X Y
[ 	
JsonPropertyName	 
( 
$str  
)  !
]! "
public# )
int* -
Code. 2
{3 4
get5 8
;8 9
set: =
;= >
}? @
} 
} ®
TC:\HeroSystem\walletService\WalletService.Models\Responses\NetworkDetailsResponse.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Responses (
;( )
public 
class "
NetworkDetailsResponse #
{ 
public 
#
StatisticsModel12356Dto "
Model123# +
{, -
get. 1
;1 2
set3 6
;6 7
}8 9
public 

StatisticsModel4Dto 
Model4 %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
public 
#
StatisticsModel12356Dto "
Model5# )
{* +
get, /
;/ 0
set1 4
;4 5
}6 7
public 
#
StatisticsModel12356Dto "
Model6# )
{* +
get, /
;/ 0
set1 4
;4 5
}6 7
}		 
public 
class #
StatisticsModel12356Dto $
{ 
public 

int 
DirectAffiliates 
{  !
get" %
;% &
set' *
;* +
}, -
public 

int 
IndirectAffiliates !
{" #
get$ '
;' (
set) ,
;, -
}. /
} 
public 
class 
StatisticsModel4Dto  
{ 
public 

int 
	LeftCount 
{ 
get 
; 
set  #
;# $
}% &
public 

int 

RightCount 
{ 
get 
;  
set! $
;$ %
}& '
} È
VC:\HeroSystem\walletService\WalletService.Models\Responses\PayWithMyBalanceResponse.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Responses (
;( )
public 
class $
PayWithMyBalanceResponse %
{ 
[ 
JsonPropertyName 
( 
$str 
)  
]  !
public 

bool 
Success 
{ 
get 
; 
set "
;" #
}$ %
[

 
JsonPropertyName

 
(

 
$str

 
)

 
]

 
public 

List 
< 
object 
> 
? 
Data 
{ 
get  #
;# $
set% (
;( )
}* +
[ 
JsonPropertyName 
( 
$str 
)  
]  !
public 

string 
Message 
{ 
get 
;  
set! $
;$ %
}& '
=( )
string* 0
.0 1
Empty1 6
;6 7
[ 
JsonPropertyName 
( 
$str 
) 
] 
public 

int 
Code 
{ 
get 
; 
set 
; 
}  !
} ù
UC:\HeroSystem\walletService\WalletService.Models\Responses\ProductConfigurationDto.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Responses (
;( )
public 
class $
GetConfigurationResponse %
{ 
[ 
JsonPropertyName 
( 
$str 
)  
]  !
public		 

bool		 
Success		 
{		 
get		 
;		 
set		 "
;		" #
}		$ %
[ 
JsonPropertyName 
( 
$str 
) 
] 
public 
#
ProductConfigurationDto "
Data# '
{( )
get* -
;- .
set/ 2
;2 3
}4 5
[ 
JsonPropertyName 
( 
$str 
)  
]  !
public 

string 
Message 
{ 
get 
;  
set! $
;$ %
}& '
[ 
JsonPropertyName 
( 
$str 
) 
] 
public 

int 
Code 
{ 
get 
; 
set 
; 
}  !
} 
public 
class #
ProductConfigurationDto $
{ 
[ 
JsonPropertyName 
( 
$str 0
)0 1
]1 2
public 

bool "
ActivateShippingSystem &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
[ 
JsonPropertyName 
( 
$str 8
)8 9
]9 :
public 

bool )
ActivatePassivePaymentsModule -
{. /
get0 3
;3 4
set5 8
;8 9
}: ;
[ 
JsonPropertyName 
( 
$str ,
), -
]- .
public 

bool 
ActivatePublicShop "
{# $
get% (
;( )
set* -
;- .
}/ 0
[   
JsonPropertyName   
(   
$str   '
)  ' (
]  ( )
public  * 0
string  1 7
?  7 8
CurrencySymbol  9 G
{  H I
get  J M
;  M N
set  O R
;  R S
}  T U
["" 
JsonPropertyName"" 
("" 
$str"" 3
)""3 4
]""4 5
public## 

string## 
?## %
SymbolCommissionableValue## ,
{##- .
get##/ 2
;##2 3
set##4 7
;##7 8
}##9 :
[%% 
JsonPropertyName%% 
(%% 
$str%% -
)%%- .
]%%. /
public&& 

string&& 
?&& 
SymbolPointsQualify&& &
{&&' (
get&&) ,
;&&, -
set&&. 1
;&&1 2
}&&3 4
[(( 
JsonPropertyName(( 
((( 
$str(( ,
)((, -
]((- .
public)) 

string)) 
?)) 
BinaryPointsSymbol)) %
{))& '
get))( +
;))+ ,
set))- 0
;))0 1
}))2 3
[++ 
JsonPropertyName++ 
(++ 
$str++ )
)++) *
]++* +
public,, 

int,, 
NewProductLabel,, 
{,,  
get,,! $
;,,$ %
set,,& )
;,,) *
},,+ ,
}-- ˜
OC:\HeroSystem\walletService\WalletService.Models\Responses\SendFundsResponse.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Responses (
;( )
public 
class 
SendFundsResponse 
{ 
public 

DataDetails 
? 
Data 
{ 
get "
;" #
set$ '
;' (
}) *
public 

int 

StatusCode 
{ 
get 
;  
set! $
;$ %
}& '
public 

int 
IdTypeStatusCode 
{  !
get" %
;% &
set' *
;* +
}, -
public 

string 
? 
Message 
{ 
get  
;  !
set" %
;% &
}' (
public

 

class

 
DataDetails

 
{ 
public 
int 
IdTransaction  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 
int 
IdWallet 
{ 
get !
;! "
set# &
;& '
}( )
public 
decimal 
Amount 
{ 
get  #
;# $
set% (
;( )
}* +
public 
decimal 
Fee 
{ 
get  
;  !
set" %
;% &
}' (
public 
CurrencyDetails 
? 
Currency  (
{) *
get+ .
;. /
set0 3
;3 4
}5 6
public 
string 
? 
Address 
{  
get! $
;$ %
set& )
;) *
}+ ,
} 
public 

class 
CurrencyDetails  
{ 
public 
int 
Id 
{ 
get 
; 
set  
;  !
}" #
public 
string 
? 
Name 
{ 
get !
;! "
set# &
;& '
}( )
public 
string 
? 
Description "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 
string 
? 
Code 
{ 
get !
;! "
set# &
;& '
}( )
public 
bool 
IsErc 
{ 
get 
;  
set! $
;$ %
}& '
} 
} Óg
NC:\HeroSystem\walletService\WalletService.Models\Responses\ServicesResponse.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Responses (
;( )
public		 
class		 
ServicesResponse		 
{

 
[ 
JsonPropertyName 
( 
$str 
)  
]  !
public" (
bool) -
Success. 5
{6 7
get8 ;
;; <
set= @
;@ A
}B C
[ 
JsonPropertyName 
( 
$str 
) 
] 
public %
object& ,
?, -
Data. 2
{3 4
get5 8
;8 9
set: =
;= >
}? @
[ 
JsonPropertyName 
( 
$str 
)  
]  !
public" (
string) /
Message0 7
{8 9
get: =
;= >
set? B
;B C
}D E
=F G
stringH N
.N O
EmptyO T
;T U
[ 
JsonPropertyName 
( 
$str 
) 
] 
public %
int& )
Code* .
{/ 0
get1 4
;4 5
set6 9
;9 :
}; <
} 
public 
class ,
 ServicesValidCodeAccountResponse -
{ 
[ 
JsonPropertyName 
( 
$str 
)  
]  !
public" (
bool) -
Success. 5
{6 7
get8 ;
;; <
set= @
;@ A
}B C
[ 
JsonPropertyName 
( 
$str 
) 
] 
public %
bool& *
Data+ /
{0 1
get2 5
;5 6
set7 :
;: ;
}< =
[ 
JsonPropertyName 
( 
$str 
)  
]  !
public" (
string) /
Message0 7
{8 9
get: =
;= >
set? B
;B C
}D E
=F G
stringH N
.N O
EmptyO T
;T U
[ 
JsonPropertyName 
( 
$str 
) 
] 
public %
int& )
Code* .
{/ 0
get1 4
;4 5
set6 9
;9 :
}; <
} 
public 
class 
ProductsResponse 
{   
[!! 
JsonPropertyName!! 
(!! 
$str!! 
)!!  
]!!  !
public!!" (
bool!!) -
Success!!. 5
{!!6 7
get!!8 ;
;!!; <
set!!= @
;!!@ A
}!!B C
["" 
JsonPropertyName"" 
("" 
$str"" 
)"" 
]"" 
public"" %
List""& *
<""* +
ProductWalletDto""+ ;
>""; <
Data""= A
{""B C
get""D G
;""G H
set""I L
;""L M
}""N O
[## 
JsonPropertyName## 
(## 
$str## 
)##  
]##  !
public##" (
string##) /
Message##0 7
{##8 9
get##: =
;##= >
set##? B
;##B C
}##D E
=##F G
string##H N
.##N O
Empty##O T
;##T U
[$$ 
JsonPropertyName$$ 
($$ 
$str$$ 
)$$ 
]$$ 
public$$ %
int$$& )
Code$$* .
{$$/ 0
get$$1 4
;$$4 5
set$$6 9
;$$9 :
}$$; <
}%% 
public'' 
class'' 
ProductResponse'' 
{(( 
[)) 
JsonPropertyName)) 
()) 
$str)) 
)))  
]))  !
public))" (
bool))) -
Success)). 5
{))6 7
get))8 ;
;)); <
set))= @
;))@ A
}))B C
[** 
JsonPropertyName** 
(** 
$str** 
)** 
]** 
public** %
ProductWalletDto**& 6
Data**7 ;
{**< =
get**> A
;**A B
set**C F
;**F G
}**H I
[++ 
JsonPropertyName++ 
(++ 
$str++ 
)++  
]++  !
public++" (
string++) /
Message++0 7
{++8 9
get++: =
;++= >
set++? B
;++B C
}++D E
=++F G
string++H N
.++N O
Empty++O T
;++T U
[,, 
JsonPropertyName,, 
(,, 
$str,, 
),, 
],, 
public,, %
int,,& )
Code,,* .
{,,/ 0
get,,1 4
;,,4 5
set,,6 9
;,,9 :
},,; <
}-- 
public// 
class// !
UserAffiliateResponse// "
{00 
[11 
NewtonsoftJson11 
.11 
JsonProperty11  
]11  !
public11" (
bool11) -
Success11. 5
{116 7
get118 ;
;11; <
set11= @
;11@ A
}11B C
[33 
NewtonsoftJson33 
.33 
JsonProperty33  
(33  !
$str33! '
)33' (
]33( )
public33* 0
UserInfoResponse331 A
?33A B
Data33C G
{33H I
get33J M
;33M N
set33O R
;33R S
}33T U
[55 
NewtonsoftJson55 
.55 
JsonProperty55  
(55  !
$str55! *
)55* +
]55+ ,
public55- 3
string554 :
Message55; B
{55C D
get55E H
;55H I
set55J M
;55M N
}55O P
=55Q R
string55S Y
.55Y Z
Empty55Z _
;55_ `
[77 
NewtonsoftJson77 
.77 
JsonProperty77  
(77  !
$str77! '
)77' (
]77( )
public77* 0
int771 4
Code775 9
{77: ;
get77< ?
;77? @
set77A D
;77D E
}77F G
}99 
public;; 
class;; )
UserAffiliatePointInformation;; *
{<< 
[== 
JsonPropertyName== 
(== 
$str== 
)==  
]==  !
public==" (
bool==) -
Success==. 5
{==6 7
get==8 ;
;==; <
set=== @
;==@ A
}==B C
[?? 
JsonPropertyName?? 
(?? 
$str?? 
)?? 
]?? 
public?? %
ICollection??& 1
<??1 2!
UserBinaryInformation??2 G
>??G H
Data??I M
{??N O
get??P S
;??S T
set??U X
;??X Y
}??Z [
[AA 
JsonPropertyNameAA 
(AA 
$strAA 
)AA  
]AA  !
publicAA" (
stringAA) /
MessageAA0 7
{AA8 9
getAA: =
;AA= >
setAA? B
;AAB C
}AAD E
=AAF G
stringAAH N
.AAN O
EmptyAAO T
;AAT U
[CC 
JsonPropertyNameCC 
(CC 
$strCC 
)CC 
]CC 
publicCC %
intCC& )
CodeCC* .
{CC/ 0
getCC1 4
;CC4 5
setCC6 9
;CC9 :
}CC; <
}EE 
publicGG 
classGG '
UserPersonalNetworkResponseGG (
{HH 
[II 
JsonPropertyNameII 
(II 
$strII 
)II  
]II  !
publicII" (
boolII) -
SuccessII. 5
{II6 7
getII8 ;
;II; <
setII= @
;II@ A
}IIB C
[KK 
JsonPropertyNameKK 
(KK 
$strKK 
)KK 
]KK 
publicKK %
ListKK& *
<KK* +
PersonalNetworkKK+ :
>KK: ;
DataKK< @
{KKA B
getKKC F
;KKF G
setKKH K
;KKK L
}KKM N
[MM 
JsonPropertyNameMM 
(MM 
$strMM 
)MM  
]MM  !
publicMM" (
stringMM) /
MessageMM0 7
{MM8 9
getMM: =
;MM= >
setMM? B
;MMB C
}MMD E
=MMF G
stringMMH N
.MMN O
EmptyMMO T
;MMT U
[OO 
JsonPropertyNameOO 
(OO 
$strOO 
)OO 
]OO 
publicOO %
intOO& )
CodeOO* .
{OO/ 0
getOO1 4
;OO4 5
setOO6 9
;OO9 :
}OO; <
}QQ 
publicSS 
classSS )
GetTotalActiveMembersResponseSS *
{TT 
[UU 
JsonPropertyNameUU 
(UU 
$strUU 
)UU  
]UU  !
publicUU" (
boolUU) -
SuccessUU. 5
{UU6 7
getUU8 ;
;UU; <
setUU= @
;UU@ A
}UUB C
[WW 
JsonPropertyNameWW 
(WW 
$strWW 
)WW 
]WW 
publicWW %
intWW& )
DataWW* .
{WW/ 0
getWW1 4
;WW4 5
setWW6 9
;WW9 :
}WW; <
[YY 
JsonPropertyNameYY 
(YY 
$strYY 
)YY  
]YY  !
publicYY" (
stringYY) /
MessageYY0 7
{YY8 9
getYY: =
;YY= >
setYY? B
;YYB C
}YYD E
=YYF G
stringYYH N
.YYN O
EmptyYYO T
;YYT U
[[[ 
JsonPropertyName[[ 
([[ 
$str[[ 
)[[ 
][[ 
public[[ %
int[[& )
Code[[* .
{[[/ 0
get[[1 4
;[[4 5
set[[6 9
;[[9 :
}[[; <
}]] 
public__ 
class__  
AffiliateBtcResponse__ !
{`` 
[aa 
NewtonsoftJsonaa 
.aa 
JsonPropertyaa  
(aa  !
$straa! *
)aa* +
]aa+ ,
publicaa- 3
boolaa4 8
Successaa9 @
{aaA B
getaaC F
;aaF G
setaaH K
;aaK L
}aaM N
[cc 
NewtonsoftJsoncc 
.cc 
JsonPropertycc  
(cc  !
$strcc! '
)cc' (
]cc( )
publiccc* 0
AffiliateBtcDtocc1 @
?cc@ A
[ccA B
]ccB C
?ccC D
DataccE I
{ccJ K
getccL O
;ccO P
setccQ T
;ccT U
}ccV W
[ee 
NewtonsoftJsonee 
.ee 
JsonPropertyee  
(ee  !
$stree! *
)ee* +
]ee+ ,
publicee- 3
stringee4 :
Messageee; B
{eeC D
geteeE H
;eeH I
seteeJ M
;eeM N
}eeO P
=eeQ R
stringeeS Y
.eeY Z
EmptyeeZ _
;ee_ `
[gg 
NewtonsoftJsongg 
.gg 
JsonPropertygg  
(gg  !
$strgg! '
)gg' (
]gg( )
publicgg* 0
intgg1 4
Codegg5 9
{gg: ;
getgg< ?
;gg? @
setggA D
;ggD E
}ggF G
}hh 
publicjj 
classjj 
PersonalNetworkjj 
{kk 
publicmm 

intmm 
idmm 
{mm 
getmm 
;mm 
setmm 
;mm 
}mm 
publicnn 

stringnn 
fullNamenn 
{nn 
getnn  
;nn  !
setnn" %
;nn% &
}nn' (
publicoo 

stringoo 
emailoo 
{oo 
getoo 
;oo 
setoo "
;oo" #
}oo$ %
publicpp 

stringpp 
userNamepp 
{pp 
getpp  
;pp  !
setpp" %
;pp% &
}pp' (
publicqq 

intqq 
externalGradingIdqq  
{qq! "
getqq# &
;qq& '
setqq( +
;qq+ ,
}qq- .
publicrr 

byterr 
statusrr 
{rr 
getrr 
;rr 
setrr !
;rr! "
}rr# $
publicss 

decimalss 
latitudess 
{ss 
getss !
;ss! "
setss# &
;ss& '
}ss( )
publictt 

decimaltt 
	longitudett 
{tt 
gettt "
;tt" #
settt$ '
;tt' (
}tt) *
publicuu 

stringuu 
countryNameuu 
{uu 
getuu  #
;uu# $
setuu% (
;uu( )
}uu* +
}vv ⁄
PC:\HeroSystem\walletService\WalletService.Models\Responses\TrcAddressResponse.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Responses (
;( )
public 
class 
TrcAddressResponse 
{ 
public 

int 
Id 
{ 
get 
; 
set 
; 
} 
public 

int 
AffiliateId 
{ 
get  
;  !
set" %
;% &
}' (
public 

string 
Address 
{ 
get 
;  
set! $
;$ %
}& '
=( )
null* .
!. /
;/ 0
public 

bool 
Status 
{ 
get 
; 
set !
;! "
}# $
}		 ®%
NC:\HeroSystem\walletService\WalletService.Models\Responses\UserInfoResponse.cs
	namespace 	
WalletService
 
. 
Models 
. 
	Responses (
;( )
public 
class 
UserInfoResponse 
{ 
[ 
JsonProperty 
( 
$str 
) 
] 
public !
string" (
?( )
Name* .
{/ 0
get1 4
;4 5
set6 9
;9 :
}; <
[		 
JsonProperty		 
(		 
$str		 
)		 
]		 
public		 
int		  #
Id		$ &
{		' (
get		) ,
;		, -
set		. 1
;		1 2
}		3 4
[

 
JsonProperty

 
(

 
$str

 
)

 
]

 
public

 #
byte

$ (
?

( )
Status

* 0
{

1 2
get

3 6
;

6 7
set

8 ;
;

; <
}

= >
[ 
JsonProperty 
( 
$str #
)# $
]$ %
public% +
DateTime, 4
?4 5
ActivationDate6 D
{E F
getG J
;J K
setL O
;O P
}Q R
[ 
JsonProperty 
( 
$str 
) 
] 
public  &
string' -
?- .
UserName/ 7
{8 9
get: =
;= >
set? B
;B C
}D E
[ 
JsonProperty 
( 
$str 
) 
] 
public  &
string' -
?- .
LastName/ 7
{8 9
get: =
;= >
set? B
;B C
}D E
[ 
JsonProperty 
( 
$str 
) 
] 
public !
string" (
?( )
City* .
{/ 0
get1 4
;4 5
set6 9
;9 :
}; <
[ 
JsonProperty 
( 
$str 
) 
] 
public "
string# )
?) *
Phone+ 0
{1 2
get3 6
;6 7
set8 ;
;; <
}= >
[ 
JsonProperty 
( 
$str 
) 
] 
public "
string# )
?) *
Email+ 0
{1 2
get3 6
;6 7
set8 ;
;; <
}= >
[ 
JsonProperty 
( 
$str 
) 
] 
public $
string% +
?+ ,
Address- 4
{5 6
get7 :
;: ;
set< ?
;? @
}A B
[ 
JsonProperty 
( 
$str "
)" #
]# $
public% +
string, 2
?2 3
AffiliateType4 A
{B C
getD G
;G H
setI L
;L M
}N O
[ 
JsonProperty 
( 
$str )
)) *
]* +
public, 2
bool3 7
?7 8
CardIdAuthorization9 L
{M N
getO R
;R S
setT W
;W X
}Y Z
[ 
JsonProperty 
( 
$str &
)& '
]' (
public) /
CountryNavigation0 A
?A B
CountryC J
{K L
getM P
;P Q
setR U
;U V
}W X
[ 
JsonProperty 
( 
$str 
) 
] 
public #
int$ '
Father( .
{/ 0
get1 4
;4 5
set6 9
;9 :
}; <
[ 
JsonProperty 
( 
$str %
)% &
]& '
public( .
string/ 5
?5 6
VerificationCode7 G
{H I
getJ M
;M N
setO R
;R S
}T U
} 
public 
class 
CountryNavigation 
{ 
[ 
JsonProperty 
( 
$str 
) 
] 
public !
string" (
?( )
Name* .
{/ 0
get1 4
;4 5
set6 9
;9 :
}; <
} 