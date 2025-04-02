ÑÇ
@C:\HeroSystem\walletService\WalletService.Ioc\IocExtensionApp.cs
	namespace'' 	
WalletService''
 
.'' 
Ioc'' 
;'' 
public)) 
static)) 
class)) 
IocExtensionApp)) #
{** 
public++ 

static++ 
void++ $
IocAppInjectDependencies++ /
(++/ 0
this++0 4
IServiceCollection++5 G
services++H P
,++P Q
string++R X
[++X Y
]++Y Z
?++Z [
args++\ `
=++a b
null++c g
)++g h
{,, 
InjectSwagger-- 
(-- 
services-- 
)-- 
;--  
InjectConfiguration.. 
(.. 
services.. $
)..$ %
;..% & 
InjectAuthentication// 
(// 
services// %
)//% &
;//& '-
!InjectControllersAndDocumentation00 )
(00) *
services00* 2
)002 3
;003 4
InjectCaching11 
(11 
services11 
)11 
;11  
InjectDataBases22 
(22 
services22  
)22  !
;22! "
InjectUnitOfWork33 
(33 
services33 !
)33! "
;33" #
InjectRepositories44 
(44 
services44 #
)44# $
;44$ %
InjectAdapters55 
(55 
services55 
)55  
;55  !
InjectServices66 
(66 
services66 
)66  
;66  !
InjectPackages77 
(77 
services77 
)77  
;77  !
InjectLogging88 
(88 
services88 
)88 
;88  
InjectStrategies99 
(99 
services99 !
)99! "
;99" #(
InjectSingletonsAndFactories:: $
(::$ %
services::% -
)::- .
;::. /#
RegisterServiceProvider;; 
(;;  
services;;  (
);;( )
;;;) *
}<< 
private>> 
static>> 
void>> 
InjectCaching>> %
(>>% &
IServiceCollection>>& 8
services>>9 A
)>>A B
{?? 
var@@ 
serviceProvider@@ 
=@@ 
services@@ &
.@@& ' 
BuildServiceProvider@@' ;
(@@; <
)@@< =
;@@= >
varAA 
settingsAA 
=AA 
serviceProviderAA &
.BB 
GetRequiredServiceBB 
<BB  
IOptionsBB  (
<BB( )$
ApplicationConfigurationBB) A
>BBA B
>BBB C
(BBC D
)BBD E
.BBE F
ValueBBF K
;BBK L
varCC 
multiplexerCC 
=CC !
ConnectionMultiplexerCC /
.DD 
ConnectDD 
(DD 
settingsDD 
.DD 
ConnectionStringsDD /
!DD/ 0
.DD0 1
RedisConnectionDD1 @
!DD@ A
)DDA B
;DDB C
servicesFF 
.FF 
AddSingletonFF 
<FF "
IConnectionMultiplexerFF 4
>FF4 5
(FF5 6
multiplexerFF6 A
)FFA B
;FFB C
servicesGG 
.GG 
AddSingletonGG 
<GG 

RedisCacheGG (
>GG( )
(GG) *
)GG* +
;GG+ ,
servicesHH 
.HH 
AddSingletonHH 
<HH 
InMemoryCacheHH +
>HH+ ,
(HH, -
)HH- .
;HH. /
servicesII 
.II 
AddSingletonII 
<II 
ILockManagerII *
,II* +
LockManagerII, 7
>II7 8
(II8 9
)II9 :
;II: ;
}JJ 
privateLL 
staticLL 
voidLL  
InjectAuthenticationLL ,
(LL, -
IServiceCollectionLL- ?
servicesLL@ H
)LLH I
=>MM 

servicesMM 
.MM 
AddAuthenticationMM %
(MM% &
)MM& '
.MM' (
AddJwtBearerMM( 4
(MM4 5
)MM5 6
;MM6 7
privateOO 
staticOO 
voidOO 
InjectConfigurationOO +
(OO+ ,
IServiceCollectionOO, >
servicesOO? G
)OOG H
{PP 
varQQ 
serviceProviderQQ 
=QQ 
servicesQQ &
.QQ& ' 
BuildServiceProviderQQ' ;
(QQ; <
)QQ< =
;QQ= >
varRR 
envRR 
=RR 
serviceProviderRR !
.RR! "
GetRequiredServiceRR" 4
<RR4 5
IHostEnvironmentRR5 E
>RRE F
(RRF G
)RRG H
;RRH I
varSS  
lowercaseEnvironmentSS  
=SS! "
envSS# &
.SS& '
EnvironmentNameSS' 6
.SS6 7
ToLowerSS7 >
(SS> ?
)SS? @
;SS@ A
varTT 
executableLocationTT 
=TT  
PathTT! %
.TT% &
GetDirectoryNameTT& 6
(TT6 7
AssemblyTT7 ?
.TT? @ 
GetExecutingAssemblyTT@ T
(TTT U
)TTU V
.TTV W
LocationTTW _
)TT_ `
??TTa c
$strTTd f
;TTf g
varUU 
builderUU 
=UU 
newUU  
ConfigurationBuilderUU .
(UU. /
)UU/ 0
.VV 
SetBasePathVV 
(VV 
executableLocationVV +
)VV+ ,
.WW 
AddJsonFileWW 
(WW 
$"WW 
$strWW '
{WW' ( 
lowercaseEnvironmentWW( <
}WW< =
$strWW= B
"WWB C
,WWC D
falseWWE J
,WWJ K
trueWWL P
)WWP Q
.XX #
AddEnvironmentVariablesXX $
(XX$ %
)XX% &
;XX& '
varZZ 
configurationZZ 
=ZZ 
builderZZ #
.ZZ# $
BuildZZ$ )
(ZZ) *
)ZZ* +
;ZZ+ ,
var[[ 
appSettingsSection[[ 
=[[  
configuration[[! .
.[[. /

GetSection[[/ 9
([[9 :
$str[[: G
)[[G H
;[[H I
services]] 
.]] 
	Configure]] 
<]] $
ApplicationConfiguration]] 3
>]]3 4
(]]4 5
appSettingsSection]]5 G
)]]G H
;]]H I
services^^ 
.^^ 
AddSingleton^^ 
(^^ 
configuration^^ +
)^^+ ,
;^^, -
services__ 
.__ 
RegisterKafkaTopics__ $
(__$ % 
lowercaseEnvironment__% 9
)__9 :
;__: ;
}`` 
privatebb 
staticbb 
voidbb 
InjectSwaggerbb %
(bb% &
thisbb& *
IServiceCollectionbb+ =
servicesbb> F
)bbF G
=>cc 

servicescc 
.cc 
AddSwaggerGencc !
(cc! "
ccc" #
=>cc$ &
{dd 	
cee 
.ee 

SwaggerDocee 
(ee 
$stree 
,ee 
newee "
OpenApiInfoee# .
{ee/ 0
Titleee1 6
=ee7 8
$stree9 H
,eeH I
VersioneeJ Q
=eeR S
$streeT X
}eeY Z
)eeZ [
;ee[ \
}ff 	
)ff	 

;ff
 
privateii 
staticii 
voidii 
InjectDataBasesii '
(ii' (
IServiceCollectionii( :
servicesii; C
)iiC D
{jj 
varkk 
	appConfigkk 
=kk 
serviceskk  
.kk  ! 
BuildServiceProviderkk! 5
(kk5 6
)kk6 7
.kk7 8
GetRequiredServicekk8 J
<kkJ K
IOptionskkK S
<kkS T$
ApplicationConfigurationkkT l
>kkl m
>kkm n
(kkn o
)kko p
.ll 
Valuell 
;ll 
varnn 
connectionStringnn 
=nn 
	appConfignn (
.nn( )
ConnectionStringsnn) :
?nn: ;
.nn; < 
PostgreSqlConnectionnn< P
;nnP Q
varpp 
dataSourceBuilderpp 
=pp 
newpp  ##
NpgsqlDataSourceBuilderpp$ ;
(pp; <
connectionStringpp< L
)ppL M
;ppM N
dataSourceBuilderqq 
.qq 
MapCompositeqq &
<qq& ',
 InvoiceDetailsTransactionRequestqq' G
>qqG H
(qqH I
$strrr =
)rr= >
;rr> ?
varss 

dataSourcess 
=ss 
dataSourceBuilderss *
.ss* +
Buildss+ 0
(ss0 1
)ss1 2
;ss2 3
servicesuu 
.uu 
AddDbContextuu 
<uu "
WalletServiceDbContextuu 4
>uu4 5
(uu5 6
optionsuu6 =
=>uu> @
{vv 	
optionsww 
.ww 
	UseNpgsqlww 
(ww 

dataSourceww (
)ww( )
.xx &
EnableSensitiveDataLoggingxx +
(xx+ ,
)xx, -
.yy  
EnableDetailedErrorsyy %
(yy% &
)yy& '
;yy' (
}zz 	
)zz	 

;zz
 
}{{ 
private}} 
static}} 
void}} 
InjectLogging}} %
(}}% &
IServiceCollection}}& 8
services}}9 A
)}}A B
{~~ 
var 
serviceProvider 
= 
services &
.& ' 
BuildServiceProvider' ;
(; <
)< =
;= >
var
ÄÄ 
env
ÄÄ 
=
ÄÄ 
serviceProvider
ÄÄ !
.
ÄÄ! " 
GetRequiredService
ÄÄ" 4
<
ÄÄ4 5!
IWebHostEnvironment
ÄÄ5 H
>
ÄÄH I
(
ÄÄI J
)
ÄÄJ K
;
ÄÄK L
var
ÅÅ "
lowerCaseEnvironment
ÅÅ  
=
ÅÅ! "
env
ÅÅ# &
.
ÅÅ& '
EnvironmentName
ÅÅ' 6
.
ÅÅ6 7
ToLower
ÅÅ7 >
(
ÅÅ> ?
)
ÅÅ? @
;
ÅÅ@ A
services
ÉÉ 
.
ÉÉ 

AddLogging
ÉÉ 
(
ÉÉ 
config
ÉÉ "
=>
ÉÉ# %
{
ÑÑ 	
config
ÖÖ 
.
ÖÖ 
ClearProviders
ÖÖ !
(
ÖÖ! "
)
ÖÖ" #
;
ÖÖ# $
config
ÜÜ 
.
ÜÜ 
AddNLog
ÜÜ 
(
ÜÜ 
$"
ÜÜ 
$str
ÜÜ "
{
ÜÜ" #"
lowerCaseEnvironment
ÜÜ# 7
}
ÜÜ7 8
$str
ÜÜ8 ?
"
ÜÜ? @
)
ÜÜ@ A
;
ÜÜA B
}
áá 	
)
áá	 

;
áá
 
Console
ââ 
.
ââ 
	WriteLine
ââ 
(
ââ 
$"
ââ 
$str
ââ 7
{
ââ7 8"
lowerCaseEnvironment
ââ8 L
}
ââL M
$str
ââM T
"
ââT U
)
ââU V
;
ââV W
}
ää 
private
åå 
static
åå 
void
åå /
!InjectControllersAndDocumentation
åå 9
(
åå9 : 
IServiceCollection
åå: L
services
ååM U
,
ååU V
int
ååW Z
majorVersion
åå[ g
=
ååh i
$num
ååj k
,
ååk l
int
çç 
minorVersion
çç 
=
çç 
$num
çç 
)
çç 
{
éé 
services
èè 
.
èè $
AddResponseCompression
èè '
(
èè' (
options
èè( /
=>
èè0 2
{
êê 	
options
ëë 
.
ëë 
	Providers
ëë 
.
ëë 
Add
ëë !
<
ëë! "%
GzipCompressionProvider
ëë" 9
>
ëë9 :
(
ëë: ;
)
ëë; <
;
ëë< =
options
íí 
.
íí 
	MimeTypes
íí 
=
íí )
ResponseCompressionDefaults
íí  ;
.
íí; <
	MimeTypes
íí< E
.
ííE F
Concat
ííF L
(
ííL M
new
ííM P
[
ííP Q
]
ííQ R
{
ííS T
$str
ííU a
}
ííb c
)
ííc d
;
ííd e
}
ìì 	
)
ìì	 

;
ìì
 
services
ïï 
.
ïï 
AddControllers
ïï 
(
ïï  
)
ïï  !
.
ïï! "
AddJsonOptions
ïï" 0
(
ïï0 1
options
ïï1 8
=>
ïï9 ;
{
ññ 	
options
óó 
.
óó #
JsonSerializerOptions
óó )
.
óó) *
ReferenceHandler
óó* :
=
óó; <
ReferenceHandler
óó= M
.
óóM N
IgnoreCycles
óóN Z
;
óóZ [
options
òò 
.
òò #
JsonSerializerOptions
òò )
.
òò) *
TypeInfoResolver
òò* :
=
òò; <
new
òò= @)
DefaultJsonTypeInfoResolver
òòA \
(
òò\ ]
)
òò] ^
;
òò^ _
}
ôô 	
)
ôô	 

;
ôô
 
services
õõ 
.
õõ /
!AddFluentValidationAutoValidation
õõ 2
(
õõ2 3
)
õõ3 4
;
õõ4 5
services
úú 
.
úú 3
%AddFluentValidationClientsideAdapters
úú 6
(
úú6 7
)
úú7 8
;
úú8 9
services
ùù 
.
ùù 
AddApiVersioning
ùù !
(
ùù! "
config
ùù" (
=>
ùù) +
{
ûû 	
config
üü 
.
üü 
DefaultApiVersion
üü $
=
üü% &
new
üü' *

ApiVersion
üü+ 5
(
üü5 6
majorVersion
üü6 B
,
üüB C
minorVersion
üüD P
)
üüP Q
;
üüQ R
config
†† 
.
†† 1
#AssumeDefaultVersionWhenUnspecified
†† 6
=
††7 8
true
††9 =
;
††= >
}
°° 	
)
°°	 

;
°°
 
services
££ 
.
££ 
AddCors
££ 
(
££ 
options
££  
=>
££! #
{
§§ 	
options
•• 
.
•• 
AddDefaultPolicy
•• $
(
••$ %
builder
¶¶ 
=>
¶¶ 
{
ßß 
builder
®® 
.
®® 
AllowAnyOrigin
®® *
(
®®* +
)
®®+ ,
.
©© 
AllowAnyHeader
©© '
(
©©' (
)
©©( )
.
™™ 
AllowAnyMethod
™™ '
(
™™' (
)
™™( )
;
™™) *
}
´´ 
)
´´ 
;
´´ 
}
¨¨ 	
)
¨¨	 

;
¨¨
 
}
≠≠ 
private
ØØ 
static
ØØ 
void
ØØ 
InjectUnitOfWork
ØØ (
(
ØØ( ) 
IServiceCollection
ØØ) ;
services
ØØ< D
)
ØØD E
{
∞∞ 
services
±± 
.
±± 
	AddScoped
±± 
<
±± 
	DbContext
±± $
>
±±$ %
(
±±% &
provider
±±& .
=>
±±/ 1
provider
≤≤ 
.
≤≤ 

GetService
≤≤ 
<
≤≤  $
WalletServiceDbContext
≤≤  6
>
≤≤6 7
(
≤≤7 8
)
≤≤8 9
)
≤≤9 :
;
≤≤: ;
services
≥≥ 
.
≥≥ 
	AddScoped
≥≥ 
<
≥≥ 
IUnitOfWork
≥≥ &
,
≥≥& '

UnitOfWork
≥≥( 2
>
≥≥2 3
(
≥≥3 4
)
≥≥4 5
;
≥≥5 6
}
¥¥ 
private
∂∂ 
static
∂∂ 
void
∂∂  
InjectRepositories
∂∂ *
(
∂∂* + 
IServiceCollection
∂∂+ =
services
∂∂> F
)
∂∂F G
{
∑∑ 
services
∏∏ 
.
∏∏ 
	AddScoped
∏∏ 
<
∏∏ &
IWalletHistoryRepository
∏∏ 3
,
∏∏3 4%
WalletHistoryRepository
∏∏5 L
>
∏∏L M
(
∏∏M N
)
∏∏N O
;
∏∏O P
services
ππ 
.
ππ 
	AddScoped
ππ 
<
ππ %
IWalletPeriodRepository
ππ 2
,
ππ2 3$
WalletPeriodRepository
ππ4 J
>
ππJ K
(
ππK L
)
ππL M
;
ππM N
services
∫∫ 
.
∫∫ 
	AddScoped
∫∫ 
<
∫∫ &
IWalletRequestRepository
∫∫ 3
,
∫∫3 4%
WalletRequestRepository
∫∫5 L
>
∫∫L M
(
∫∫M N
)
∫∫N O
;
∫∫O P
services
ªª 
.
ªª 
	AddScoped
ªª 
<
ªª .
 IWalletRetentionConfigRepository
ªª ;
,
ªª; <-
WalletRetentionConfigRepository
ªª= \
>
ªª\ ]
(
ªª] ^
)
ªª^ _
;
ªª_ `
services
ºº 
.
ºº 
	AddScoped
ºº 
<
ºº 
IWalletRepository
ºº ,
,
ºº, -
WalletRepository
ºº. >
>
ºº> ?
(
ºº? @
)
ºº@ A
;
ººA B
services
ΩΩ 
.
ΩΩ 
	AddScoped
ΩΩ 
<
ΩΩ &
IWalletModel1ARepository
ΩΩ 3
,
ΩΩ3 4%
WalletModel1ARepository
ΩΩ5 L
>
ΩΩL M
(
ΩΩM N
)
ΩΩN O
;
ΩΩO P
services
ææ 
.
ææ 
	AddScoped
ææ 
<
ææ &
IWalletModel1BRepository
ææ 3
,
ææ3 4%
WalletModel1BRepository
ææ5 L
>
ææL M
(
ææM N
)
ææN O
;
ææO P
services
øø 
.
øø 
	AddScoped
øø 
<
øø #
IWalletWaitRepository
øø 0
,
øø0 1"
WalletWaitRepository
øø2 F
>
øøF G
(
øøG H
)
øøH I
;
øøI J
services
¿¿ 
.
¿¿ 
	AddScoped
¿¿ 
<
¿¿ )
IWalletWithDrawalRepository
¿¿ 6
,
¿¿6 7(
WalletWithDrawalRepository
¿¿8 R
>
¿¿R S
(
¿¿S T
)
¿¿T U
;
¿¿U V
services
¡¡ 
.
¡¡ 
	AddScoped
¡¡ 
<
¡¡ &
IInvoiceDetailRepository
¡¡ 3
,
¡¡3 4%
InvoiceDetailRepository
¡¡5 L
>
¡¡L M
(
¡¡M N
)
¡¡N O
;
¡¡O P
services
¬¬ 
.
¬¬ 
	AddScoped
¬¬ 
<
¬¬  
IInvoiceRepository
¬¬ -
,
¬¬- .
InvoiceRepository
¬¬/ @
>
¬¬@ A
(
¬¬A B
)
¬¬B C
;
¬¬C D
services
√√ 
.
√√ 
	AddScoped
√√ 
<
√√  
IInvoiceRepository
√√ -
,
√√- .
InvoiceRepository
√√/ @
>
√√@ A
(
√√A B
)
√√B C
;
√√C D
services
ƒƒ 
.
ƒƒ 
	AddScoped
ƒƒ 
<
ƒƒ (
INetworkPurchaseRepository
ƒƒ 5
,
ƒƒ5 6'
NetworkPurchaseRepository
ƒƒ7 P
>
ƒƒP Q
(
ƒƒQ R
)
ƒƒR S
;
ƒƒS T
services
≈≈ 
.
≈≈ 
	AddScoped
≈≈ 
<
≈≈ -
IEcoPoolConfigurationRepository
≈≈ :
,
≈≈: ;,
EcoPoolConfigurationRepository
≈≈< Z
>
≈≈Z [
(
≈≈[ \
)
≈≈\ ]
;
≈≈] ^
services
∆∆ 
.
∆∆ 
	AddScoped
∆∆ 
<
∆∆ '
IResultsEcoPoolRepository
∆∆ 4
,
∆∆4 5&
ResultsEcoPoolRepository
∆∆6 N
>
∆∆N O
(
∆∆O P
)
∆∆P Q
;
∆∆Q R
services
«« 
.
«« 
	AddScoped
«« 
<
«« "
IApiClientRepository
«« /
,
««/ 0!
ApiClientRepository
««1 D
>
««D E
(
««E F
)
««F G
;
««G H
services
»» 
.
»» 
	AddScoped
»» 
<
»» /
!ICoinPaymentTransactionRepository
»» <
,
»»< =.
 CoinPaymentTransactionRepository
»»> ^
>
»»^ _
(
»»_ `
)
»»` a
;
»»a b
services
…… 
.
…… 
	AddScoped
…… 
<
…… 
IBrandRepository
…… +
,
……+ ,
BrandRepository
……- <
>
……< =
(
……= >
)
……> ?
;
……? @
services
   
.
   
	AddScoped
   
<
   
IBonusRepository
   +
,
  + ,
BonusRepository
  - <
>
  < =
(
  = >
)
  > ?
;
  ? @
services
ÀÀ 
.
ÀÀ 
	AddScoped
ÀÀ 
<
ÀÀ 
ICreditRepository
ÀÀ ,
,
ÀÀ, -
CreditRepository
ÀÀ. >
>
ÀÀ> ?
(
ÀÀ? @
)
ÀÀ@ A
;
ÀÀA B
}
ÃÃ 
private
ŒŒ 
static
ŒŒ 
void
ŒŒ 
InjectAdapters
ŒŒ &
(
ŒŒ& ' 
IServiceCollection
ŒŒ' 9
services
ŒŒ: B
)
ŒŒB C
{
œœ 
services
–– 
.
–– 
	AddScoped
–– 
<
–– 
ICoinPayAdapter
–– *
,
––* +
CoinPayAdapter
––, :
>
––: ;
(
––; <
)
––< =
;
––= >
services
—— 
.
—— 
	AddScoped
—— 
<
—— $
IAccountServiceAdapter
—— 1
,
——1 2#
AccountServiceAdapter
——3 H
>
——H I
(
——I J
)
——J K
;
——K L
services
““ 
.
““ 
	AddScoped
““ 
<
““ &
IInventoryServiceAdapter
““ 3
,
““3 4%
InventoryServiceAdapter
““5 L
>
““L M
(
““M N
)
““N O
;
““O P
services
”” 
.
”” 
	AddScoped
”” 
<
”” #
IConfigurationAdapter
”” 0
,
””0 1"
ConfigurationAdapter
””2 F
>
””F G
(
””G H
)
””H I
;
””I J
services
‘‘ 
.
‘‘ 
	AddScoped
‘‘ 
<
‘‘ 
IPagaditoAdapter
‘‘ +
,
‘‘+ ,
PagaditoAdapter
‘‘- <
>
‘‘< =
(
‘‘= >
)
‘‘> ?
;
‘‘? @
}
’’ 
private
◊◊ 
static
◊◊ 
void
◊◊ 
InjectStrategies
◊◊ (
(
◊◊( ) 
IServiceCollection
◊◊) ;
services
◊◊< D
)
◊◊D E
{
ÿÿ 
services
ŸŸ 
.
ŸŸ 
	AddScoped
ŸŸ 
<
ŸŸ %
IBalancePaymentStrategy
ŸŸ 2
,
ŸŸ2 3$
BalancePaymentStrategy
ŸŸ4 J
>
ŸŸJ K
(
ŸŸK L
)
ŸŸL M
;
ŸŸM N
services
⁄⁄ 
.
⁄⁄ 
	AddScoped
⁄⁄ 
<
⁄⁄ +
IBalancePaymentStrategyModel2
⁄⁄ 8
,
⁄⁄8 9*
BalancePaymentStrategyModel2
⁄⁄: V
>
⁄⁄V W
(
⁄⁄W X
)
⁄⁄X Y
;
⁄⁄Y Z
services
€€ 
.
€€ 
	AddScoped
€€ 
<
€€ +
ToThirdPartiesPaymentStrategy
€€ 8
>
€€8 9
(
€€9 :
)
€€: ;
;
€€; <
services
‹‹ 
.
‹‹ 
	AddScoped
‹‹ 
<
‹‹ %
ICoinPayPaymentStrategy
‹‹ 2
,
‹‹2 3$
CoinPayPaymentStrategy
‹‹4 J
>
‹‹J K
(
‹‹K L
)
‹‹L M
;
‹‹M N
services
›› 
.
›› 
	AddScoped
›› 
<
›› *
ICoinPaymentsPaymentStrategy
›› 7
,
››7 8)
CoinPaymentsPaymentStrategy
››9 T
>
››T U
(
››U V
)
››V W
;
››W X
services
ﬁﬁ 
.
ﬁﬁ 
	AddScoped
ﬁﬁ 
<
ﬁﬁ #
IWireTransferStrategy
ﬁﬁ 0
,
ﬁﬁ0 1"
WireTransferStrategy
ﬁﬁ2 F
>
ﬁﬁF G
(
ﬁﬁG H
)
ﬁﬁH I
;
ﬁﬁI J
services
ﬂﬂ 
.
ﬂﬂ 
	AddScoped
ﬂﬂ 
<
ﬂﬂ ,
IBalancePaymentStrategyModel1A
ﬂﬂ 9
,
ﬂﬂ9 :&
BalancePaymentStrategy1A
ﬂﬂ; S
>
ﬂﬂS T
(
ﬂﬂT U
)
ﬂﬂU V
;
ﬂﬂV W
services
‡‡ 
.
‡‡ 
	AddScoped
‡‡ 
<
‡‡ ,
IBalancePaymentStrategyModel1B
‡‡ 9
,
‡‡9 :&
BalancePaymentStrategy1B
‡‡; S
>
‡‡S T
(
‡‡T U
)
‡‡U V
;
‡‡V W
services
·· 
.
·· 
	AddScoped
·· 
<
·· &
IPagaditoPaymentStrategy
·· 3
,
··3 4%
PagaditoPaymentStrategy
··5 L
>
··L M
(
··M N
)
··N O
;
··O P
}
‚‚ 
private
‰‰ 
static
‰‰ 
void
‰‰ 
InjectServices
‰‰ &
(
‰‰& ' 
IServiceCollection
‰‰' 9
services
‰‰: B
)
‰‰B C
{
ÂÂ 
services
ÊÊ 
.
ÊÊ 
	AddScoped
ÊÊ 
<
ÊÊ 
ICoinPayService
ÊÊ *
,
ÊÊ* +
CoinPayService
ÊÊ, :
>
ÊÊ: ;
(
ÊÊ; <
)
ÊÊ< =
;
ÊÊ= >
services
ÁÁ 
.
ÁÁ 
	AddScoped
ÁÁ 
<
ÁÁ #
IWalletHistoryService
ÁÁ 0
,
ÁÁ0 1"
WalletHistoryService
ÁÁ2 F
>
ÁÁF G
(
ÁÁG H
)
ÁÁH I
;
ÁÁI J
services
ËË 
.
ËË 
	AddScoped
ËË 
<
ËË "
IWalletPeriodService
ËË /
,
ËË/ 0!
WalletPeriodService
ËË1 D
>
ËËD E
(
ËËE F
)
ËËF G
;
ËËG H
services
ÈÈ 
.
ÈÈ 
	AddScoped
ÈÈ 
<
ÈÈ #
IWalletRequestService
ÈÈ 0
,
ÈÈ0 1"
WalletRequestService
ÈÈ2 F
>
ÈÈF G
(
ÈÈG H
)
ÈÈH I
;
ÈÈI J
services
ÍÍ 
.
ÍÍ 
	AddScoped
ÍÍ 
<
ÍÍ +
IWalletRetentionConfigService
ÍÍ 8
,
ÍÍ8 9*
WalletRetentionConfigService
ÍÍ: V
>
ÍÍV W
(
ÍÍW X
)
ÍÍX Y
;
ÍÍY Z
services
ÎÎ 
.
ÎÎ 
	AddScoped
ÎÎ 
<
ÎÎ 
IWalletService
ÎÎ )
,
ÎÎ) *
Core
ÎÎ+ /
.
ÎÎ/ 0
Services
ÎÎ0 8
.
ÎÎ8 9
WalletService
ÎÎ9 F
>
ÎÎF G
(
ÎÎG H
)
ÎÎH I
;
ÎÎI J
services
ÏÏ 
.
ÏÏ 
	AddScoped
ÏÏ 
<
ÏÏ  
IWalletWaitService
ÏÏ -
,
ÏÏ- .
WalletWaitService
ÏÏ/ @
>
ÏÏ@ A
(
ÏÏA B
)
ÏÏB C
;
ÏÏC D
services
ÌÌ 
.
ÌÌ 
	AddScoped
ÌÌ 
<
ÌÌ &
IWalletWithdrawalService
ÌÌ 3
,
ÌÌ3 4%
WalletWithDrawalService
ÌÌ5 L
>
ÌÌL M
(
ÌÌM N
)
ÌÌN O
;
ÌÌO P
services
ÓÓ 
.
ÓÓ 
	AddScoped
ÓÓ 
<
ÓÓ #
IInvoiceDetailService
ÓÓ 0
,
ÓÓ0 1"
InvoiceDetailService
ÓÓ2 F
>
ÓÓF G
(
ÓÓG H
)
ÓÓH I
;
ÓÓI J
services
ÔÔ 
.
ÔÔ 
	AddScoped
ÔÔ 
<
ÔÔ 
IInvoiceService
ÔÔ *
,
ÔÔ* +
InvoiceService
ÔÔ, :
>
ÔÔ: ;
(
ÔÔ; <
)
ÔÔ< =
;
ÔÔ= >
services
 
.
 
	AddScoped
 
<
 $
IProcessGradingService
 1
,
1 2#
ProcessGradingService
3 H
>
H I
(
I J
)
J K
;
K L
services
ÒÒ 
.
ÒÒ 
	AddScoped
ÒÒ 
<
ÒÒ *
IEcoPoolConfigurationService
ÒÒ 7
,
ÒÒ7 8)
EcoPoolConfigurationService
ÒÒ9 T
>
ÒÒT U
(
ÒÒU V
)
ÒÒV W
;
ÒÒW X
services
ÚÚ 
.
ÚÚ 
	AddScoped
ÚÚ 
<
ÚÚ "
IEcosystemPdfService
ÚÚ /
,
ÚÚ/ 0!
EcosystemPdfService
ÚÚ1 D
>
ÚÚD E
(
ÚÚE F
)
ÚÚF G
;
ÚÚG H
services
ÛÛ 
.
ÛÛ 
	AddScoped
ÛÛ 
<
ÛÛ $
IResultsEcoPoolService
ÛÛ 1
,
ÛÛ1 2#
ResultsEcoPoolService
ÛÛ3 H
>
ÛÛH I
(
ÛÛI J
)
ÛÛJ K
;
ÛÛK L
services
ÙÙ 
.
ÙÙ 
	AddScoped
ÙÙ 
<
ÙÙ  
IConPaymentService
ÙÙ -
,
ÙÙ- .
ConPaymentService
ÙÙ/ @
>
ÙÙ@ A
(
ÙÙA B
)
ÙÙB C
;
ÙÙC D
services
ıı 
.
ıı 
	AddScoped
ıı 
<
ıı  
IBrevoEmailService
ıı -
,
ıı- .
BrevoEmailService
ıı/ @
>
ıı@ A
(
ııA B
)
ııB C
;
ııC D
services
ˆˆ 
.
ˆˆ 
	AddScoped
ˆˆ 
<
ˆˆ (
IPaymentTransactionService
ˆˆ 5
,
ˆˆ5 6'
PaymentTransactionService
ˆˆ7 P
>
ˆˆP Q
(
ˆˆQ R
)
ˆˆR S
;
ˆˆS T
services
˜˜ 
.
˜˜ 
	AddScoped
˜˜ 
<
˜˜ #
IWalletModel1AService
˜˜ 0
,
˜˜0 1"
WalletModel1AService
˜˜2 F
>
˜˜F G
(
˜˜G H
)
˜˜H I
;
˜˜I J
services
¯¯ 
.
¯¯ 
	AddScoped
¯¯ 
<
¯¯ #
IWalletModel1BService
¯¯ 0
,
¯¯0 1"
WalletModel1BService
¯¯2 F
>
¯¯F G
(
¯¯G H
)
¯¯H I
;
¯¯I J
services
˘˘ 
.
˘˘ 
	AddScoped
˘˘ 
<
˘˘ 
IPagaditoService
˘˘ +
,
˘˘+ ,
PagaditoService
˘˘- <
>
˘˘< =
(
˘˘= >
)
˘˘> ?
;
˘˘? @
services
˙˙ 
.
˙˙ 
	AddScoped
˙˙ 
<
˙˙ $
IUserStatisticsService
˙˙ 1
,
˙˙1 2#
UserStatisticsService
˙˙3 H
>
˙˙H I
(
˙˙I J
)
˙˙J K
;
˙˙K L
services
˚˚ 
.
˚˚ 
	AddScoped
˚˚ 
<
˚˚ 
IBrandService
˚˚ (
,
˚˚( )
BrandService
˚˚* 6
>
˚˚6 7
(
˚˚7 8
)
˚˚8 9
;
˚˚9 :
services
¸¸ 
.
¸¸ 
	AddScoped
¸¸ 
<
¸¸ !
IRecyCoinPdfService
¸¸ .
,
¸¸. / 
RecyCoinPdfService
¸¸0 B
>
¸¸B C
(
¸¸C D
)
¸¸D E
;
¸¸E F
services
˝˝ 
.
˝˝ 
	AddScoped
˝˝ 
<
˝˝ "
IHouseCoinPdfService
˝˝ /
,
˝˝/ 0!
HouseCoinPdfService
˝˝1 D
>
˝˝D E
(
˝˝E F
)
˝˝F G
;
˝˝G H
services
˛˛ 
.
˛˛ 
	AddScoped
˛˛ 
<
˛˛ '
IRedisCacheCleanupService
˛˛ 4
,
˛˛4 5&
RedisCacheCleanupService
˛˛6 N
>
˛˛N O
(
˛˛O P
)
˛˛P Q
;
˛˛Q R
services
ˇˇ 
.
ˇˇ 
	AddScoped
ˇˇ 
<
ˇˇ $
IExitoJuntosPdfService
ˇˇ 1
,
ˇˇ1 2#
ExitoJuntosPdfService
ˇˇ3 H
>
ˇˇH I
(
ˇˇI J
)
ˇˇJ K
;
ˇˇK L
}
ÄÄ 
private
ÇÇ 
static
ÇÇ 
void
ÇÇ 
InjectPackages
ÇÇ &
(
ÇÇ& ' 
IServiceCollection
ÇÇ' 9
services
ÇÇ: B
)
ÇÇB C
=>
ÉÉ 

services
ÉÉ 
.
ÉÉ 
AddAutoMapper
ÉÉ !
(
ÉÉ! "
x
ÉÉ" #
=>
ÉÉ$ &
{
ÉÉ' (
x
ÉÉ) *
.
ÉÉ* +

AddProfile
ÉÉ+ 5
(
ÉÉ5 6
new
ÉÉ6 9
MapperProfile
ÉÉ: G
(
ÉÉG H
)
ÉÉH I
)
ÉÉI J
;
ÉÉJ K
}
ÉÉL M
)
ÉÉM N
;
ÉÉN O
private
ÖÖ 
static
ÖÖ 
void
ÖÖ %
RegisterServiceProvider
ÖÖ /
(
ÖÖ/ 0 
IServiceCollection
ÖÖ0 B
services
ÖÖC K
)
ÖÖK L
{
ÜÜ 
services
áá 
.
áá 
AddSingleton
áá 
<
áá 

HttpClient
áá (
>
áá( )
(
áá) *
)
áá* +
;
áá+ ,
services
àà 
.
àà 
AddSingleton
àà 
(
àà 
services
àà &
.
àà& '"
BuildServiceProvider
àà' ;
(
àà; <
)
àà< =
)
àà= >
;
àà> ?
}
ââ 
private
ãã 
static
ãã 
void
ãã *
InjectSingletonsAndFactories
ãã 4
(
ãã4 5 
IServiceCollection
ãã5 G
services
ããH P
)
ããP Q
{
åå 
services
çç 
.
çç 
AddHttpClient
çç 
(
çç 
)
çç  
;
çç  !
services
éé 
.
éé $
AddHttpContextAccessor
éé '
(
éé' (
)
éé( )
;
éé) *
services
êê 
.
êê 
AddSingleton
êê 
(
êê 
_
êê 
=
êê  !
new
êê" %
KafkaProducer
êê& 3
(
êê3 4
services
êê4 <
)
êê< =
)
êê= >
;
êê> ?
}
ëë 
}íí œ÷
CC:\HeroSystem\walletService\WalletService.Ioc\IocExtensionWorker.cs
	namespace 	
WalletService
 
. 
Ioc 
; 
public!! 
static!! 
class!! 
IocExtensionWorker!! &
{"" 
public## 

static## 
void## '
IocWorkerInjectDependencies## 2
(##2 3
this##3 7
IServiceCollection##8 J
services##K S
,##S T
string##U [
[##[ \
]##\ ]
?##] ^
args##_ c
=##d e
null##f j
)##j k
{$$ 
InjectConfiguration%% 
(%% 
services%% $
)%%$ %
;%%% &
InjectCaching&& 
(&& 
services&& 
)&& 
;&&  
InjectDataBases'' 
('' 
services''  
)''  !
;''! "
InjectPackages(( 
((( 
services(( 
)((  
;((  !
InjectUnitOfWork)) 
()) 
services)) !
)))! "
;))" #
InjectRepositories** 
(** 
services** #
)**# $
;**$ %
InjectServices++ 
(++ 
services++ 
)++  
;++  !
InjectAdapters,, 
(,, 
services,, 
),,  
;,,  !
InjectLogging-- 
(-- 
services-- 
)-- 
;--  
InjectStrategies.. 
(.. 
services.. !
)..! "
;.." #(
InjectSingletonsAndFactories// $
(//$ %
services//% -
)//- .
;//. /#
RegisterServiceProvider00 
(00  
services00  (
)00( )
;00) *
}11 
private33 
static33 
void33 
InjectCaching33 %
(33% &
IServiceCollection33& 8
services339 A
)33A B
{44 
var55 
serviceProvider55 
=55 
services55 &
.55& ' 
BuildServiceProvider55' ;
(55; <
)55< =
;55= >
var66 
settings66 
=66 
serviceProvider66 &
.66& '
GetRequiredService66' 9
<669 :
IOptions66: B
<66B C$
ApplicationConfiguration66C [
>66[ \
>66\ ]
(66] ^
)66^ _
.66_ `
Value66` e
;66e f
var77 
multiplexer77 
=77 !
ConnectionMultiplexer77 /
.77/ 0
Connect770 7
(777 8
settings778 @
.77@ A
ConnectionStrings77A R
!77R S
.77S T
RedisConnection77T c
!77c d
)77d e
;77e f
services99 
.99 
AddSingleton99 
<99 "
IConnectionMultiplexer99 4
>994 5
(995 6
multiplexer996 A
)99A B
;99B C
services:: 
.:: 
AddSingleton:: 
<:: 

RedisCache:: (
>::( )
(::) *
)::* +
;::+ ,
services;; 
.;; 
AddSingleton;; 
<;; 
InMemoryCache;; +
>;;+ ,
(;;, -
);;- .
;;;. /
services<< 
.<< 
AddSingleton<< 
<<< 
ILockManager<< *
,<<* +
LockManager<<, 7
><<7 8
(<<8 9
)<<9 :
;<<: ;
}== 
private?? 
static?? 
void?? 
InjectConfiguration?? +
(??+ ,
IServiceCollection??, >
services??? G
)??G H
{@@ 
varAA 
serviceProviderAA 
=AA 
servicesAA &
.AA& ' 
BuildServiceProviderAA' ;
(AA; <
)AA< =
;AA= >
varBB 
envBB 
=BB 
serviceProviderBB !
.BB! "
GetRequiredServiceBB" 4
<BB4 5
IHostEnvironmentBB5 E
>BBE F
(BBF G
)BBG H
;BBH I
varCC  
lowercaseEnvironmentCC  
=CC! "
envCC# &
.CC& '
EnvironmentNameCC' 6
.CC6 7
ToLowerCC7 >
(CC> ?
)CC? @
;CC@ A
varDD 
executableLocationDD 
=DD  
PathDD! %
.DD% &
GetDirectoryNameDD& 6
(DD6 7
AssemblyDD7 ?
.DD? @ 
GetExecutingAssemblyDD@ T
(DDT U
)DDU V
.DDV W
LocationDDW _
)DD_ `
??DDa c
$strDDd f
;DDf g
varEE 
builderEE 
=EE 
newEE  
ConfigurationBuilderEE .
(EE. /
)EE/ 0
.FF 
SetBasePathFF 
(FF 
executableLocationFF +
)FF+ ,
.GG 
AddJsonFileGG 
(GG 
$"GG 
$strGG '
{GG' ( 
lowercaseEnvironmentGG( <
}GG< =
$strGG= B
"GGB C
,GGC D
falseGGE J
,GGJ K
trueGGL P
)GGP Q
.HH #
AddEnvironmentVariablesHH $
(HH$ %
)HH% &
;HH& '
varJJ 
configurationJJ 
=JJ 
builderJJ #
.JJ# $
BuildJJ$ )
(JJ) *
)JJ* +
;JJ+ ,
varKK 
appSettingsSectionKK 
=KK  
configurationKK! .
.KK. /

GetSectionKK/ 9
(KK9 :
$strKK: G
)KKG H
;KKH I
servicesMM 
.MM 
	ConfigureMM 
<MM $
ApplicationConfigurationMM 3
>MM3 4
(MM4 5
appSettingsSectionMM5 G
)MMG H
;MMH I
servicesNN 
.NN 
AddSingletonNN 
(NN 
configurationNN +
)NN+ ,
;NN, -
servicesOO 
.OO 
RegisterKafkaTopicsOO $
(OO$ % 
lowercaseEnvironmentOO% 9
)OO9 :
;OO: ;
}PP 
privateRR 
staticRR 
voidRR 
InjectStrategiesRR (
(RR( )
IServiceCollectionRR) ;
servicesRR< D
)RRD E
{SS 
servicesTT 
.TT 
	AddScopedTT 
<TT #
IBalancePaymentStrategyTT 2
,TT2 3"
BalancePaymentStrategyTT4 J
>TTJ K
(TTK L
)TTL M
;TTM N
servicesUU 
.UU 
	AddScopedUU 
<UU )
IBalancePaymentStrategyModel2UU 8
,UU8 9(
BalancePaymentStrategyModel2UU: V
>UUV W
(UUW X
)UUX Y
;UUY Z
servicesVV 
.VV 
	AddScopedVV 
<VV )
ToThirdPartiesPaymentStrategyVV 8
>VV8 9
(VV9 :
)VV: ;
;VV; <
servicesWW 
.WW 
	AddScopedWW 
<WW #
ICoinPayPaymentStrategyWW 2
,WW2 3"
CoinPayPaymentStrategyWW4 J
>WWJ K
(WWK L
)WWL M
;WWM N
servicesXX 
.XX 
	AddScopedXX 
<XX (
ICoinPaymentsPaymentStrategyXX 7
,XX7 8'
CoinPaymentsPaymentStrategyXX9 T
>XXT U
(XXU V
)XXV W
;XXW X
servicesYY 
.YY 
	AddScopedYY 
<YY !
IWireTransferStrategyYY 0
,YY0 1 
WireTransferStrategyYY2 F
>YYF G
(YYG H
)YYH I
;YYI J
servicesZZ 
.ZZ 
	AddScopedZZ 
<ZZ *
IBalancePaymentStrategyModel1AZZ 9
,ZZ9 :$
BalancePaymentStrategy1AZZ; S
>ZZS T
(ZZT U
)ZZU V
;ZZV W
services[[ 
.[[ 
	AddScoped[[ 
<[[ *
IBalancePaymentStrategyModel1B[[ 9
,[[9 :$
BalancePaymentStrategy1B[[; S
>[[S T
([[T U
)[[U V
;[[V W
}\\ 
private^^ 
static^^ 
void^^ 
InjectDataBases^^ '
(^^' (
IServiceCollection^^( :
services^^; C
)^^C D
{__ 
var`` 
	appConfig`` 
=`` 
services``  
.``  ! 
BuildServiceProvider``! 5
(``5 6
)``6 7
.``7 8
GetRequiredService``8 J
<``J K
IOptions``K S
<``S T$
ApplicationConfiguration``T l
>``l m
>``m n
(``n o
)``o p
.aa 
Valueaa 
;aa 
varcc 
connectionStringcc 
=cc 
	appConfigcc (
.cc( )
ConnectionStringscc) :
?cc: ;
.cc; < 
PostgreSqlConnectioncc< P
;ccP Q
varee 
dataSourceBuilderee 
=ee 
newee  ##
NpgsqlDataSourceBuilderee$ ;
(ee; <
connectionStringee< L
)eeL M
;eeM N
dataSourceBuilderff 
.ff 
MapCompositeff &
<ff& ',
 InvoiceDetailsTransactionRequestff' G
>ffG H
(ffH I
$strgg =
)gg= >
;gg> ?
varhh 

dataSourcehh 
=hh 
dataSourceBuilderhh *
.hh* +
Buildhh+ 0
(hh0 1
)hh1 2
;hh2 3
servicesjj 
.jj 
AddDbContextjj 
<jj "
WalletServiceDbContextjj 4
>jj4 5
(jj5 6
optionsjj6 =
=>jj> @
{kk 	
optionsll 
.ll 
	UseNpgsqlll 
(ll 

dataSourcell (
)ll( )
.mm &
EnableSensitiveDataLoggingmm +
(mm+ ,
)mm, -
.nn  
EnableDetailedErrorsnn %
(nn% &
)nn& '
;nn' (
}oo 	
)oo	 

;oo
 
}pp 
privaterr 
staticrr 
voidrr 
InjectLoggingrr %
(rr% &
IServiceCollectionrr& 8
servicesrr9 A
)rrA B
{ss 
vartt 
serviceProvidertt 
=tt 
servicestt &
.tt& ' 
BuildServiceProvidertt' ;
(tt; <
)tt< =
;tt= >
varuu 
envuu 
=uu 
serviceProvideruu !
.uu! "
GetRequiredServiceuu" 4
<uu4 5
IHostEnvironmentuu5 E
>uuE F
(uuF G
)uuG H
;uuH I
varvv 
configurationvv 
=vv 
serviceProvidervv +
.vv+ ,
GetRequiredServicevv, >
<vv> ?
IConfigurationvv? M
>vvM N
(vvN O
)vvO P
;vvP Q
varww  
lowerCaseEnvironmentww  
=ww! "
envww# &
.ww& '
EnvironmentNameww' 6
.ww6 7
ToLowerww7 >
(ww> ?
)ww? @
;ww@ A
servicesyy 
.yy 

AddLoggingyy 
(yy 
configyy "
=>yy# %
{zz 	
config{{ 
.{{ 
ClearProviders{{ !
({{! "
){{" #
;{{# $
config|| 
.|| 
AddConfiguration|| #
(||# $
configuration||$ 1
)||1 2
;||2 3
config}} 
.}} 
AddNLog}} 
(}} 
$"}} 
$str}} "
{}}" # 
lowerCaseEnvironment}}# 7
}}}7 8
$str}}8 ?
"}}? @
)}}@ A
;}}A B
}~~ 	
)~~	 

;~~
 
Console
ÄÄ 
.
ÄÄ 
	WriteLine
ÄÄ 
(
ÄÄ 
$"
ÄÄ 
$str
ÄÄ 7
{
ÄÄ7 8"
lowerCaseEnvironment
ÄÄ8 L
}
ÄÄL M
$str
ÄÄM T
"
ÄÄT U
)
ÄÄU V
;
ÄÄV W
}
ÅÅ 
private
ÉÉ 
static
ÉÉ 
void
ÉÉ  
InjectRepositories
ÉÉ *
(
ÉÉ* + 
IServiceCollection
ÉÉ+ =
services
ÉÉ> F
)
ÉÉF G
{
ÑÑ 
services
ÖÖ 
.
ÖÖ 
	AddScoped
ÖÖ 
<
ÖÖ &
IWalletHistoryRepository
ÖÖ 3
,
ÖÖ3 4%
WalletHistoryRepository
ÖÖ5 L
>
ÖÖL M
(
ÖÖM N
)
ÖÖN O
;
ÖÖO P
services
ÜÜ 
.
ÜÜ 
	AddScoped
ÜÜ 
<
ÜÜ %
IWalletPeriodRepository
ÜÜ 2
,
ÜÜ2 3$
WalletPeriodRepository
ÜÜ4 J
>
ÜÜJ K
(
ÜÜK L
)
ÜÜL M
;
ÜÜM N
services
áá 
.
áá 
	AddScoped
áá 
<
áá &
IWalletRequestRepository
áá 3
,
áá3 4%
WalletRequestRepository
áá5 L
>
ááL M
(
ááM N
)
ááN O
;
ááO P
services
àà 
.
àà 
	AddScoped
àà 
<
àà .
 IWalletRetentionConfigRepository
àà ;
,
àà; <-
WalletRetentionConfigRepository
àà= \
>
àà\ ]
(
àà] ^
)
àà^ _
;
àà_ `
services
ââ 
.
ââ 
	AddScoped
ââ 
<
ââ 
IWalletRepository
ââ ,
,
ââ, -
WalletRepository
ââ. >
>
ââ> ?
(
ââ? @
)
ââ@ A
;
ââA B
services
ää 
.
ää 
	AddScoped
ää 
<
ää #
IWalletWaitRepository
ää 0
,
ää0 1"
WalletWaitRepository
ää2 F
>
ääF G
(
ääG H
)
ääH I
;
ääI J
services
ãã 
.
ãã 
	AddScoped
ãã 
<
ãã )
IWalletWithDrawalRepository
ãã 6
,
ãã6 7(
WalletWithDrawalRepository
ãã8 R
>
ããR S
(
ããS T
)
ããT U
;
ããU V
services
åå 
.
åå 
	AddScoped
åå 
<
åå &
IInvoiceDetailRepository
åå 3
,
åå3 4%
InvoiceDetailRepository
åå5 L
>
ååL M
(
ååM N
)
ååN O
;
ååO P
services
çç 
.
çç 
	AddScoped
çç 
<
çç  
IInvoiceRepository
çç -
,
çç- .
InvoiceRepository
çç/ @
>
çç@ A
(
ççA B
)
ççB C
;
ççC D
services
éé 
.
éé 
	AddScoped
éé 
<
éé  
IInvoiceRepository
éé -
,
éé- .
InvoiceRepository
éé/ @
>
éé@ A
(
ééA B
)
ééB C
;
ééC D
services
èè 
.
èè 
	AddScoped
èè 
<
èè (
INetworkPurchaseRepository
èè 5
,
èè5 6'
NetworkPurchaseRepository
èè7 P
>
èèP Q
(
èèQ R
)
èèR S
;
èèS T
services
êê 
.
êê 
	AddScoped
êê 
<
êê -
IEcoPoolConfigurationRepository
êê :
,
êê: ;,
EcoPoolConfigurationRepository
êê< Z
>
êêZ [
(
êê[ \
)
êê\ ]
;
êê] ^
services
ëë 
.
ëë 
	AddScoped
ëë 
<
ëë '
IResultsEcoPoolRepository
ëë 4
,
ëë4 5&
ResultsEcoPoolRepository
ëë6 N
>
ëëN O
(
ëëO P
)
ëëP Q
;
ëëQ R
services
íí 
.
íí 
	AddScoped
íí 
<
íí "
IApiClientRepository
íí /
,
íí/ 0!
ApiClientRepository
íí1 D
>
ííD E
(
ííE F
)
ííF G
;
ííG H
services
ìì 
.
ìì 
	AddScoped
ìì 
<
ìì /
!ICoinPaymentTransactionRepository
ìì <
,
ìì< =.
 CoinPaymentTransactionRepository
ìì> ^
>
ìì^ _
(
ìì_ `
)
ìì` a
;
ììa b
services
îî 
.
îî 
	AddScoped
îî 
<
îî &
IWalletModel1ARepository
îî 3
,
îî3 4%
WalletModel1ARepository
îî5 L
>
îîL M
(
îîM N
)
îîN O
;
îîO P
services
ïï 
.
ïï 
	AddScoped
ïï 
<
ïï &
IWalletModel1BRepository
ïï 3
,
ïï3 4%
WalletModel1BRepository
ïï5 L
>
ïïL M
(
ïïM N
)
ïïN O
;
ïïO P
services
ññ 
.
ññ 
	AddScoped
ññ 
<
ññ 
IBrandRepository
ññ +
,
ññ+ ,
BrandRepository
ññ- <
>
ññ< =
(
ññ= >
)
ññ> ?
;
ññ? @
services
óó 
.
óó 
	AddScoped
óó 
<
óó 
IBonusRepository
óó +
,
óó+ ,
BonusRepository
óó- <
>
óó< =
(
óó= >
)
óó> ?
;
óó? @
services
òò 
.
òò 
	AddScoped
òò 
<
òò 
ICreditRepository
òò ,
,
òò, -
CreditRepository
òò. >
>
òò> ?
(
òò? @
)
òò@ A
;
òòA B
}
ôô 
private
õõ 
static
õõ 
void
õõ 
InjectUnitOfWork
õõ (
(
õõ( ) 
IServiceCollection
õõ) ;
services
õõ< D
)
õõD E
{
úú 
services
ùù 
.
ùù 
	AddScoped
ùù 
<
ùù 
	DbContext
ùù $
>
ùù$ %
(
ùù% &
provider
ùù& .
=>
ùù/ 1
provider
ûû 
.
ûû 

GetService
ûû 
<
ûû  $
WalletServiceDbContext
ûû  6
>
ûû6 7
(
ûû7 8
)
ûû8 9
)
ûû9 :
;
ûû: ;
services
üü 
.
üü 
	AddScoped
üü 
<
üü 
IUnitOfWork
üü &
,
üü& '

UnitOfWork
üü( 2
>
üü2 3
(
üü3 4
)
üü4 5
;
üü5 6
}
†† 
private
¢¢ 
static
¢¢ 
void
¢¢ 
InjectAdapters
¢¢ &
(
¢¢& ' 
IServiceCollection
¢¢' 9
services
¢¢: B
)
¢¢B C
{
££ 
services
§§ 
.
§§ 
	AddScoped
§§ 
<
§§ 
ICoinPayAdapter
§§ *
,
§§* +
CoinPayAdapter
§§, :
>
§§: ;
(
§§; <
)
§§< =
;
§§= >
services
•• 
.
•• 
	AddScoped
•• 
<
•• $
IAccountServiceAdapter
•• 1
,
••1 2#
AccountServiceAdapter
••3 H
>
••H I
(
••I J
)
••J K
;
••K L
services
¶¶ 
.
¶¶ 
	AddScoped
¶¶ 
<
¶¶ &
IInventoryServiceAdapter
¶¶ 3
,
¶¶3 4%
InventoryServiceAdapter
¶¶5 L
>
¶¶L M
(
¶¶M N
)
¶¶N O
;
¶¶O P
services
ßß 
.
ßß 
	AddScoped
ßß 
<
ßß #
IConfigurationAdapter
ßß 0
,
ßß0 1"
ConfigurationAdapter
ßß2 F
>
ßßF G
(
ßßG H
)
ßßH I
;
ßßI J
}
®® 
private
™™ 
static
™™ 
void
™™ 
InjectServices
™™ &
(
™™& ' 
IServiceCollection
™™' 9
services
™™: B
)
™™B C
{
´´ 
services
¨¨ 
.
¨¨ 
	AddScoped
¨¨ 
<
¨¨ 
ICoinPayService
¨¨ *
,
¨¨* +
CoinPayService
¨¨, :
>
¨¨: ;
(
¨¨; <
)
¨¨< =
;
¨¨= >
services
≠≠ 
.
≠≠ 
	AddScoped
≠≠ 
<
≠≠ #
IWalletHistoryService
≠≠ 0
,
≠≠0 1"
WalletHistoryService
≠≠2 F
>
≠≠F G
(
≠≠G H
)
≠≠H I
;
≠≠I J
services
ÆÆ 
.
ÆÆ 
	AddScoped
ÆÆ 
<
ÆÆ "
IWalletPeriodService
ÆÆ /
,
ÆÆ/ 0!
WalletPeriodService
ÆÆ1 D
>
ÆÆD E
(
ÆÆE F
)
ÆÆF G
;
ÆÆG H
services
ØØ 
.
ØØ 
	AddScoped
ØØ 
<
ØØ #
IWalletRequestService
ØØ 0
,
ØØ0 1"
WalletRequestService
ØØ2 F
>
ØØF G
(
ØØG H
)
ØØH I
;
ØØI J
services
∞∞ 
.
∞∞ 
	AddScoped
∞∞ 
<
∞∞ +
IWalletRetentionConfigService
∞∞ 8
,
∞∞8 9*
WalletRetentionConfigService
∞∞: V
>
∞∞V W
(
∞∞W X
)
∞∞X Y
;
∞∞Y Z
services
±± 
.
±± 
	AddScoped
±± 
<
±± 
IWalletService
±± )
,
±±) *
Core
±±+ /
.
±±/ 0
Services
±±0 8
.
±±8 9
WalletService
±±9 F
>
±±F G
(
±±G H
)
±±H I
;
±±I J
services
≤≤ 
.
≤≤ 
	AddScoped
≤≤ 
<
≤≤  
IWalletWaitService
≤≤ -
,
≤≤- .
WalletWaitService
≤≤/ @
>
≤≤@ A
(
≤≤A B
)
≤≤B C
;
≤≤C D
services
≥≥ 
.
≥≥ 
	AddScoped
≥≥ 
<
≥≥ &
IWalletWithdrawalService
≥≥ 3
,
≥≥3 4%
WalletWithDrawalService
≥≥5 L
>
≥≥L M
(
≥≥M N
)
≥≥N O
;
≥≥O P
services
¥¥ 
.
¥¥ 
	AddScoped
¥¥ 
<
¥¥ #
IInvoiceDetailService
¥¥ 0
,
¥¥0 1"
InvoiceDetailService
¥¥2 F
>
¥¥F G
(
¥¥G H
)
¥¥H I
;
¥¥I J
services
µµ 
.
µµ 
	AddScoped
µµ 
<
µµ 
IInvoiceService
µµ *
,
µµ* +
InvoiceService
µµ, :
>
µµ: ;
(
µµ; <
)
µµ< =
;
µµ= >
services
∂∂ 
.
∂∂ 
	AddScoped
∂∂ 
<
∂∂ $
IProcessGradingService
∂∂ 1
,
∂∂1 2#
ProcessGradingService
∂∂3 H
>
∂∂H I
(
∂∂I J
)
∂∂J K
;
∂∂K L
services
∑∑ 
.
∑∑ 
	AddScoped
∑∑ 
<
∑∑ *
IEcoPoolConfigurationService
∑∑ 7
,
∑∑7 8)
EcoPoolConfigurationService
∑∑9 T
>
∑∑T U
(
∑∑U V
)
∑∑V W
;
∑∑W X
services
∏∏ 
.
∏∏ 
	AddScoped
∏∏ 
<
∏∏ "
IEcosystemPdfService
∏∏ /
,
∏∏/ 0!
EcosystemPdfService
∏∏1 D
>
∏∏D E
(
∏∏E F
)
∏∏F G
;
∏∏G H
services
ππ 
.
ππ 
	AddScoped
ππ 
<
ππ $
IResultsEcoPoolService
ππ 1
,
ππ1 2#
ResultsEcoPoolService
ππ3 H
>
ππH I
(
ππI J
)
ππJ K
;
ππK L
services
∫∫ 
.
∫∫ 
	AddScoped
∫∫ 
<
∫∫  
IConPaymentService
∫∫ -
,
∫∫- .
ConPaymentService
∫∫/ @
>
∫∫@ A
(
∫∫A B
)
∫∫B C
;
∫∫C D
services
ªª 
.
ªª 
	AddScoped
ªª 
<
ªª  
IBrevoEmailService
ªª -
,
ªª- .
BrevoEmailService
ªª/ @
>
ªª@ A
(
ªªA B
)
ªªB C
;
ªªC D
services
ºº 
.
ºº 
	AddScoped
ºº 
<
ºº (
IPaymentTransactionService
ºº 5
,
ºº5 6'
PaymentTransactionService
ºº7 P
>
ººP Q
(
ººQ R
)
ººR S
;
ººS T
services
ΩΩ 
.
ΩΩ 
	AddScoped
ΩΩ 
<
ΩΩ #
IWalletModel1AService
ΩΩ 0
,
ΩΩ0 1"
WalletModel1AService
ΩΩ2 F
>
ΩΩF G
(
ΩΩG H
)
ΩΩH I
;
ΩΩI J
services
ææ 
.
ææ 
	AddScoped
ææ 
<
ææ #
IWalletModel1BService
ææ 0
,
ææ0 1"
WalletModel1BService
ææ2 F
>
ææF G
(
ææG H
)
ææH I
;
ææI J
services
øø 
.
øø 
	AddScoped
øø 
<
øø $
IUserStatisticsService
øø 1
,
øø1 2#
UserStatisticsService
øø3 H
>
øøH I
(
øøI J
)
øøJ K
;
øøK L
services
¿¿ 
.
¿¿ 
	AddScoped
¿¿ 
<
¿¿ 
IBrandService
¿¿ (
,
¿¿( )
BrandService
¿¿* 6
>
¿¿6 7
(
¿¿7 8
)
¿¿8 9
;
¿¿9 :
services
¡¡ 
.
¡¡ 
	AddScoped
¡¡ 
<
¡¡ !
IRecyCoinPdfService
¡¡ .
,
¡¡. / 
RecyCoinPdfService
¡¡0 B
>
¡¡B C
(
¡¡C D
)
¡¡D E
;
¡¡E F
services
¬¬ 
.
¬¬ 
	AddScoped
¬¬ 
<
¬¬ "
IHouseCoinPdfService
¬¬ /
,
¬¬/ 0!
HouseCoinPdfService
¬¬1 D
>
¬¬D E
(
¬¬E F
)
¬¬F G
;
¬¬G H
services
√√ 
.
√√ 
	AddScoped
√√ 
<
√√ '
IRedisCacheCleanupService
√√ 4
,
√√4 5&
RedisCacheCleanupService
√√6 N
>
√√N O
(
√√O P
)
√√P Q
;
√√Q R
services
ƒƒ 
.
ƒƒ 
	AddScoped
ƒƒ 
<
ƒƒ $
IExitoJuntosPdfService
ƒƒ 1
,
ƒƒ1 2#
ExitoJuntosPdfService
ƒƒ3 H
>
ƒƒH I
(
ƒƒI J
)
ƒƒJ K
;
ƒƒK L
}
≈≈ 
private
«« 
static
«« 
void
«« 
InjectPackages
«« &
(
««& ' 
IServiceCollection
««' 9
services
««: B
)
««B C
{
»» 
services
…… 
.
…… 
AddAutoMapper
…… 
(
…… 
x
……  
=>
……! #
{
……$ %
x
……& '
.
……' (

AddProfile
……( 2
(
……2 3
new
……3 6
MapperProfile
……7 D
(
……D E
)
……E F
)
……F G
;
……G H
}
……I J
)
……J K
;
……K L
}
   
private
ÃÃ 
static
ÃÃ 
void
ÃÃ %
RegisterServiceProvider
ÃÃ /
(
ÃÃ/ 0 
IServiceCollection
ÃÃ0 B
services
ÃÃC K
)
ÃÃK L
{
ÕÕ 
services
ŒŒ 
.
ŒŒ 
AddSingleton
ŒŒ 
<
ŒŒ 

HttpClient
ŒŒ (
>
ŒŒ( )
(
ŒŒ) *
)
ŒŒ* +
;
ŒŒ+ ,
services
œœ 
.
œœ 
AddSingleton
œœ 
(
œœ 
services
œœ &
.
œœ& '"
BuildServiceProvider
œœ' ;
(
œœ; <
)
œœ< =
)
œœ= >
;
œœ> ?
}
–– 
private
““ 
static
““ 
void
““ *
InjectSingletonsAndFactories
““ 4
(
““4 5 
IServiceCollection
““5 G
services
““H P
)
““P Q
{
”” 
services
‘‘ 
.
‘‘ 
AddHttpClient
‘‘ 
(
‘‘ 
)
‘‘  
;
‘‘  !
services
’’ 
.
’’ $
AddHttpContextAccessor
’’ '
(
’’' (
)
’’( )
;
’’) *
services
◊◊ 
.
◊◊ 
AddSingleton
◊◊ 
(
◊◊ 
_
◊◊ 
=
◊◊  !
new
◊◊" %
KafkaProducer
◊◊& 3
(
◊◊3 4
services
◊◊4 <
)
◊◊< =
)
◊◊= >
;
◊◊> ?
}
ÿÿ 
}ŸŸ 