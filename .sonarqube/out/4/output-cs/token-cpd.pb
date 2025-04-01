��
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
�� 
env
�� 
=
�� 
serviceProvider
�� !
.
��! " 
GetRequiredService
��" 4
<
��4 5!
IWebHostEnvironment
��5 H
>
��H I
(
��I J
)
��J K
;
��K L
var
�� "
lowerCaseEnvironment
��  
=
��! "
env
��# &
.
��& '
EnvironmentName
��' 6
.
��6 7
ToLower
��7 >
(
��> ?
)
��? @
;
��@ A
services
�� 
.
�� 

AddLogging
�� 
(
�� 
config
�� "
=>
��# %
{
�� 	
config
�� 
.
�� 
ClearProviders
�� !
(
��! "
)
��" #
;
��# $
config
�� 
.
�� 
AddNLog
�� 
(
�� 
$"
�� 
$str
�� "
{
��" #"
lowerCaseEnvironment
��# 7
}
��7 8
$str
��8 ?
"
��? @
)
��@ A
;
��A B
}
�� 	
)
��	 

;
��
 
Console
�� 
.
�� 
	WriteLine
�� 
(
�� 
$"
�� 
$str
�� 7
{
��7 8"
lowerCaseEnvironment
��8 L
}
��L M
$str
��M T
"
��T U
)
��U V
;
��V W
}
�� 
private
�� 
static
�� 
void
�� /
!InjectControllersAndDocumentation
�� 9
(
��9 : 
IServiceCollection
��: L
services
��M U
,
��U V
int
��W Z
majorVersion
��[ g
=
��h i
$num
��j k
,
��k l
int
�� 
minorVersion
�� 
=
�� 
$num
�� 
)
�� 
{
�� 
services
�� 
.
�� $
AddResponseCompression
�� '
(
��' (
options
��( /
=>
��0 2
{
�� 	
options
�� 
.
�� 
	Providers
�� 
.
�� 
Add
�� !
<
��! "%
GzipCompressionProvider
��" 9
>
��9 :
(
��: ;
)
��; <
;
��< =
options
�� 
.
�� 
	MimeTypes
�� 
=
�� )
ResponseCompressionDefaults
��  ;
.
��; <
	MimeTypes
��< E
.
��E F
Concat
��F L
(
��L M
new
��M P
[
��P Q
]
��Q R
{
��S T
$str
��U a
}
��b c
)
��c d
;
��d e
}
�� 	
)
��	 

;
��
 
services
�� 
.
�� 
AddControllers
�� 
(
��  
)
��  !
.
��! "
AddJsonOptions
��" 0
(
��0 1
options
��1 8
=>
��9 ;
{
�� 	
options
�� 
.
�� #
JsonSerializerOptions
�� )
.
��) *
ReferenceHandler
��* :
=
��; <
ReferenceHandler
��= M
.
��M N
IgnoreCycles
��N Z
;
��Z [
options
�� 
.
�� #
JsonSerializerOptions
�� )
.
��) *
TypeInfoResolver
��* :
=
��; <
new
��= @)
DefaultJsonTypeInfoResolver
��A \
(
��\ ]
)
��] ^
;
��^ _
}
�� 	
)
��	 

;
��
 
services
�� 
.
�� /
!AddFluentValidationAutoValidation
�� 2
(
��2 3
)
��3 4
;
��4 5
services
�� 
.
�� 3
%AddFluentValidationClientsideAdapters
�� 6
(
��6 7
)
��7 8
;
��8 9
services
�� 
.
�� 
AddApiVersioning
�� !
(
��! "
config
��" (
=>
��) +
{
�� 	
config
�� 
.
�� 
DefaultApiVersion
�� $
=
��% &
new
��' *

ApiVersion
��+ 5
(
��5 6
majorVersion
��6 B
,
��B C
minorVersion
��D P
)
��P Q
;
��Q R
config
�� 
.
�� 1
#AssumeDefaultVersionWhenUnspecified
�� 6
=
��7 8
true
��9 =
;
��= >
}
�� 	
)
��	 

;
��
 
services
�� 
.
�� 
AddCors
�� 
(
�� 
options
��  
=>
��! #
{
�� 	
options
�� 
.
�� 
AddDefaultPolicy
�� $
(
��$ %
builder
�� 
=>
�� 
{
�� 
builder
�� 
.
�� 
AllowAnyOrigin
�� *
(
��* +
)
��+ ,
.
�� 
AllowAnyHeader
�� '
(
��' (
)
��( )
.
�� 
AllowAnyMethod
�� '
(
��' (
)
��( )
;
��) *
}
�� 
)
�� 
;
�� 
}
�� 	
)
��	 

;
��
 
}
�� 
private
�� 
static
�� 
void
�� 
InjectUnitOfWork
�� (
(
��( ) 
IServiceCollection
��) ;
services
��< D
)
��D E
{
�� 
services
�� 
.
�� 
	AddScoped
�� 
<
�� 
	DbContext
�� $
>
��$ %
(
��% &
provider
��& .
=>
��/ 1
provider
�� 
.
�� 

GetService
�� 
<
��  $
WalletServiceDbContext
��  6
>
��6 7
(
��7 8
)
��8 9
)
��9 :
;
��: ;
services
�� 
.
�� 
	AddScoped
�� 
<
�� 
IUnitOfWork
�� &
,
��& '

UnitOfWork
��( 2
>
��2 3
(
��3 4
)
��4 5
;
��5 6
}
�� 
private
�� 
static
�� 
void
��  
InjectRepositories
�� *
(
��* + 
IServiceCollection
��+ =
services
��> F
)
��F G
{
�� 
services
�� 
.
�� 
	AddScoped
�� 
<
�� &
IWalletHistoryRepository
�� 3
,
��3 4%
WalletHistoryRepository
��5 L
>
��L M
(
��M N
)
��N O
;
��O P
services
�� 
.
�� 
	AddScoped
�� 
<
�� %
IWalletPeriodRepository
�� 2
,
��2 3$
WalletPeriodRepository
��4 J
>
��J K
(
��K L
)
��L M
;
��M N
services
�� 
.
�� 
	AddScoped
�� 
<
�� &
IWalletRequestRepository
�� 3
,
��3 4%
WalletRequestRepository
��5 L
>
��L M
(
��M N
)
��N O
;
��O P
services
�� 
.
�� 
	AddScoped
�� 
<
�� .
 IWalletRetentionConfigRepository
�� ;
,
��; <-
WalletRetentionConfigRepository
��= \
>
��\ ]
(
��] ^
)
��^ _
;
��_ `
services
�� 
.
�� 
	AddScoped
�� 
<
�� 
IWalletRepository
�� ,
,
��, -
WalletRepository
��. >
>
��> ?
(
��? @
)
��@ A
;
��A B
services
�� 
.
�� 
	AddScoped
�� 
<
�� &
IWalletModel1ARepository
�� 3
,
��3 4%
WalletModel1ARepository
��5 L
>
��L M
(
��M N
)
��N O
;
��O P
services
�� 
.
�� 
	AddScoped
�� 
<
�� &
IWalletModel1BRepository
�� 3
,
��3 4%
WalletModel1BRepository
��5 L
>
��L M
(
��M N
)
��N O
;
��O P
services
�� 
.
�� 
	AddScoped
�� 
<
�� #
IWalletWaitRepository
�� 0
,
��0 1"
WalletWaitRepository
��2 F
>
��F G
(
��G H
)
��H I
;
��I J
services
�� 
.
�� 
	AddScoped
�� 
<
�� )
IWalletWithDrawalRepository
�� 6
,
��6 7(
WalletWithDrawalRepository
��8 R
>
��R S
(
��S T
)
��T U
;
��U V
services
�� 
.
�� 
	AddScoped
�� 
<
�� &
IInvoiceDetailRepository
�� 3
,
��3 4%
InvoiceDetailRepository
��5 L
>
��L M
(
��M N
)
��N O
;
��O P
services
�� 
.
�� 
	AddScoped
�� 
<
��  
IInvoiceRepository
�� -
,
��- .
InvoiceRepository
��/ @
>
��@ A
(
��A B
)
��B C
;
��C D
services
�� 
.
�� 
	AddScoped
�� 
<
��  
IInvoiceRepository
�� -
,
��- .
InvoiceRepository
��/ @
>
��@ A
(
��A B
)
��B C
;
��C D
services
�� 
.
�� 
	AddScoped
�� 
<
�� (
INetworkPurchaseRepository
�� 5
,
��5 6'
NetworkPurchaseRepository
��7 P
>
��P Q
(
��Q R
)
��R S
;
��S T
services
�� 
.
�� 
	AddScoped
�� 
<
�� -
IEcoPoolConfigurationRepository
�� :
,
��: ;,
EcoPoolConfigurationRepository
��< Z
>
��Z [
(
��[ \
)
��\ ]
;
��] ^
services
�� 
.
�� 
	AddScoped
�� 
<
�� '
IResultsEcoPoolRepository
�� 4
,
��4 5&
ResultsEcoPoolRepository
��6 N
>
��N O
(
��O P
)
��P Q
;
��Q R
services
�� 
.
�� 
	AddScoped
�� 
<
�� "
IApiClientRepository
�� /
,
��/ 0!
ApiClientRepository
��1 D
>
��D E
(
��E F
)
��F G
;
��G H
services
�� 
.
�� 
	AddScoped
�� 
<
�� /
!ICoinPaymentTransactionRepository
�� <
,
��< =.
 CoinPaymentTransactionRepository
��> ^
>
��^ _
(
��_ `
)
��` a
;
��a b
services
�� 
.
�� 
	AddScoped
�� 
<
�� 
IBrandRepository
�� +
,
��+ ,
BrandRepository
��- <
>
��< =
(
��= >
)
��> ?
;
��? @
services
�� 
.
�� 
	AddScoped
�� 
<
�� 
IBonusRepository
�� +
,
��+ ,
BonusRepository
��- <
>
��< =
(
��= >
)
��> ?
;
��? @
services
�� 
.
�� 
	AddScoped
�� 
<
�� 
ICreditRepository
�� ,
,
��, -
CreditRepository
��. >
>
��> ?
(
��? @
)
��@ A
;
��A B
}
�� 
private
�� 
static
�� 
void
�� 
InjectAdapters
�� &
(
��& ' 
IServiceCollection
��' 9
services
��: B
)
��B C
{
�� 
services
�� 
.
�� 
	AddScoped
�� 
<
�� 
ICoinPayAdapter
�� *
,
��* +
CoinPayAdapter
��, :
>
��: ;
(
��; <
)
��< =
;
��= >
services
�� 
.
�� 
	AddScoped
�� 
<
�� $
IAccountServiceAdapter
�� 1
,
��1 2#
AccountServiceAdapter
��3 H
>
��H I
(
��I J
)
��J K
;
��K L
services
�� 
.
�� 
	AddScoped
�� 
<
�� &
IInventoryServiceAdapter
�� 3
,
��3 4%
InventoryServiceAdapter
��5 L
>
��L M
(
��M N
)
��N O
;
��O P
services
�� 
.
�� 
	AddScoped
�� 
<
�� #
IConfigurationAdapter
�� 0
,
��0 1"
ConfigurationAdapter
��2 F
>
��F G
(
��G H
)
��H I
;
��I J
services
�� 
.
�� 
	AddScoped
�� 
<
�� 
IPagaditoAdapter
�� +
,
��+ ,
PagaditoAdapter
��- <
>
��< =
(
��= >
)
��> ?
;
��? @
}
�� 
private
�� 
static
�� 
void
�� 
InjectStrategies
�� (
(
��( ) 
IServiceCollection
��) ;
services
��< D
)
��D E
{
�� 
services
�� 
.
�� 
	AddScoped
�� 
<
�� %
IBalancePaymentStrategy
�� 2
,
��2 3$
BalancePaymentStrategy
��4 J
>
��J K
(
��K L
)
��L M
;
��M N
services
�� 
.
�� 
	AddScoped
�� 
<
�� +
IBalancePaymentStrategyModel2
�� 8
,
��8 9*
BalancePaymentStrategyModel2
��: V
>
��V W
(
��W X
)
��X Y
;
��Y Z
services
�� 
.
�� 
	AddScoped
�� 
<
�� +
ToThirdPartiesPaymentStrategy
�� 8
>
��8 9
(
��9 :
)
��: ;
;
��; <
services
�� 
.
�� 
	AddScoped
�� 
<
�� %
ICoinPayPaymentStrategy
�� 2
,
��2 3$
CoinPayPaymentStrategy
��4 J
>
��J K
(
��K L
)
��L M
;
��M N
services
�� 
.
�� 
	AddScoped
�� 
<
�� *
ICoinPaymentsPaymentStrategy
�� 7
,
��7 8)
CoinPaymentsPaymentStrategy
��9 T
>
��T U
(
��U V
)
��V W
;
��W X
services
�� 
.
�� 
	AddScoped
�� 
<
�� #
IWireTransferStrategy
�� 0
,
��0 1"
WireTransferStrategy
��2 F
>
��F G
(
��G H
)
��H I
;
��I J
services
�� 
.
�� 
	AddScoped
�� 
<
�� ,
IBalancePaymentStrategyModel1A
�� 9
,
��9 :&
BalancePaymentStrategy1A
��; S
>
��S T
(
��T U
)
��U V
;
��V W
services
�� 
.
�� 
	AddScoped
�� 
<
�� ,
IBalancePaymentStrategyModel1B
�� 9
,
��9 :&
BalancePaymentStrategy1B
��; S
>
��S T
(
��T U
)
��U V
;
��V W
services
�� 
.
�� 
	AddScoped
�� 
<
�� &
IPagaditoPaymentStrategy
�� 3
,
��3 4%
PagaditoPaymentStrategy
��5 L
>
��L M
(
��M N
)
��N O
;
��O P
}
�� 
private
�� 
static
�� 
void
�� 
InjectServices
�� &
(
��& ' 
IServiceCollection
��' 9
services
��: B
)
��B C
{
�� 
services
�� 
.
�� 
	AddScoped
�� 
<
�� 
ICoinPayService
�� *
,
��* +
CoinPayService
��, :
>
��: ;
(
��; <
)
��< =
;
��= >
services
�� 
.
�� 
	AddScoped
�� 
<
�� #
IWalletHistoryService
�� 0
,
��0 1"
WalletHistoryService
��2 F
>
��F G
(
��G H
)
��H I
;
��I J
services
�� 
.
�� 
	AddScoped
�� 
<
�� "
IWalletPeriodService
�� /
,
��/ 0!
WalletPeriodService
��1 D
>
��D E
(
��E F
)
��F G
;
��G H
services
�� 
.
�� 
	AddScoped
�� 
<
�� #
IWalletRequestService
�� 0
,
��0 1"
WalletRequestService
��2 F
>
��F G
(
��G H
)
��H I
;
��I J
services
�� 
.
�� 
	AddScoped
�� 
<
�� +
IWalletRetentionConfigService
�� 8
,
��8 9*
WalletRetentionConfigService
��: V
>
��V W
(
��W X
)
��X Y
;
��Y Z
services
�� 
.
�� 
	AddScoped
�� 
<
�� 
IWalletService
�� )
,
��) *
Core
��+ /
.
��/ 0
Services
��0 8
.
��8 9
WalletService
��9 F
>
��F G
(
��G H
)
��H I
;
��I J
services
�� 
.
�� 
	AddScoped
�� 
<
��  
IWalletWaitService
�� -
,
��- .
WalletWaitService
��/ @
>
��@ A
(
��A B
)
��B C
;
��C D
services
�� 
.
�� 
	AddScoped
�� 
<
�� &
IWalletWithdrawalService
�� 3
,
��3 4%
WalletWithDrawalService
��5 L
>
��L M
(
��M N
)
��N O
;
��O P
services
�� 
.
�� 
	AddScoped
�� 
<
�� #
IInvoiceDetailService
�� 0
,
��0 1"
InvoiceDetailService
��2 F
>
��F G
(
��G H
)
��H I
;
��I J
services
�� 
.
�� 
	AddScoped
�� 
<
�� 
IInvoiceService
�� *
,
��* +
InvoiceService
��, :
>
��: ;
(
��; <
)
��< =
;
��= >
services
�� 
.
�� 
	AddScoped
�� 
<
�� $
IProcessGradingService
�� 1
,
��1 2#
ProcessGradingService
��3 H
>
��H I
(
��I J
)
��J K
;
��K L
services
�� 
.
�� 
	AddScoped
�� 
<
�� *
IEcoPoolConfigurationService
�� 7
,
��7 8)
EcoPoolConfigurationService
��9 T
>
��T U
(
��U V
)
��V W
;
��W X
services
�� 
.
�� 
	AddScoped
�� 
<
�� "
IEcosystemPdfService
�� /
,
��/ 0!
EcosystemPdfService
��1 D
>
��D E
(
��E F
)
��F G
;
��G H
services
�� 
.
�� 
	AddScoped
�� 
<
�� $
IResultsEcoPoolService
�� 1
,
��1 2#
ResultsEcoPoolService
��3 H
>
��H I
(
��I J
)
��J K
;
��K L
services
�� 
.
�� 
	AddScoped
�� 
<
��  
IConPaymentService
�� -
,
��- .
ConPaymentService
��/ @
>
��@ A
(
��A B
)
��B C
;
��C D
services
�� 
.
�� 
	AddScoped
�� 
<
��  
IBrevoEmailService
�� -
,
��- .
BrevoEmailService
��/ @
>
��@ A
(
��A B
)
��B C
;
��C D
services
�� 
.
�� 
	AddScoped
�� 
<
�� (
IPaymentTransactionService
�� 5
,
��5 6'
PaymentTransactionService
��7 P
>
��P Q
(
��Q R
)
��R S
;
��S T
services
�� 
.
�� 
	AddScoped
�� 
<
�� #
IWalletModel1AService
�� 0
,
��0 1"
WalletModel1AService
��2 F
>
��F G
(
��G H
)
��H I
;
��I J
services
�� 
.
�� 
	AddScoped
�� 
<
�� #
IWalletModel1BService
�� 0
,
��0 1"
WalletModel1BService
��2 F
>
��F G
(
��G H
)
��H I
;
��I J
services
�� 
.
�� 
	AddScoped
�� 
<
�� 
IPagaditoService
�� +
,
��+ ,
PagaditoService
��- <
>
��< =
(
��= >
)
��> ?
;
��? @
services
�� 
.
�� 
	AddScoped
�� 
<
�� $
IUserStatisticsService
�� 1
,
��1 2#
UserStatisticsService
��3 H
>
��H I
(
��I J
)
��J K
;
��K L
services
�� 
.
�� 
	AddScoped
�� 
<
�� 
IBrandService
�� (
,
��( )
BrandService
��* 6
>
��6 7
(
��7 8
)
��8 9
;
��9 :
services
�� 
.
�� 
	AddScoped
�� 
<
�� !
IRecyCoinPdfService
�� .
,
��. / 
RecyCoinPdfService
��0 B
>
��B C
(
��C D
)
��D E
;
��E F
services
�� 
.
�� 
	AddScoped
�� 
<
�� "
IHouseCoinPdfService
�� /
,
��/ 0!
HouseCoinPdfService
��1 D
>
��D E
(
��E F
)
��F G
;
��G H
services
�� 
.
�� 
	AddScoped
�� 
<
�� '
IRedisCacheCleanupService
�� 4
,
��4 5&
RedisCacheCleanupService
��6 N
>
��N O
(
��O P
)
��P Q
;
��Q R
services
�� 
.
�� 
	AddScoped
�� 
<
�� $
IExitoJuntosPdfService
�� 1
,
��1 2#
ExitoJuntosPdfService
��3 H
>
��H I
(
��I J
)
��J K
;
��K L
}
�� 
private
�� 
static
�� 
void
�� 
InjectPackages
�� &
(
��& ' 
IServiceCollection
��' 9
services
��: B
)
��B C
=>
�� 

services
�� 
.
�� 
AddAutoMapper
�� !
(
��! "
x
��" #
=>
��$ &
{
��' (
x
��) *
.
��* +

AddProfile
��+ 5
(
��5 6
new
��6 9
MapperProfile
��: G
(
��G H
)
��H I
)
��I J
;
��J K
}
��L M
)
��M N
;
��N O
private
�� 
static
�� 
void
�� %
RegisterServiceProvider
�� /
(
��/ 0 
IServiceCollection
��0 B
services
��C K
)
��K L
{
�� 
services
�� 
.
�� 
AddSingleton
�� 
<
�� 

HttpClient
�� (
>
��( )
(
��) *
)
��* +
;
��+ ,
services
�� 
.
�� 
AddSingleton
�� 
(
�� 
services
�� &
.
��& '"
BuildServiceProvider
��' ;
(
��; <
)
��< =
)
��= >
;
��> ?
}
�� 
private
�� 
static
�� 
void
�� *
InjectSingletonsAndFactories
�� 4
(
��4 5 
IServiceCollection
��5 G
services
��H P
)
��P Q
{
�� 
services
�� 
.
�� 
AddHttpClient
�� 
(
�� 
)
��  
;
��  !
services
�� 
.
�� $
AddHttpContextAccessor
�� '
(
��' (
)
��( )
;
��) *
services
�� 
.
�� 
AddSingleton
�� 
(
�� 
_
�� 
=
��  !
new
��" %
KafkaProducer
��& 3
(
��3 4
services
��4 <
)
��< =
)
��= >
;
��> ?
}
�� 
}�� ��
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
�� 
.
�� 
	WriteLine
�� 
(
�� 
$"
�� 
$str
�� 7
{
��7 8"
lowerCaseEnvironment
��8 L
}
��L M
$str
��M T
"
��T U
)
��U V
;
��V W
}
�� 
private
�� 
static
�� 
void
��  
InjectRepositories
�� *
(
��* + 
IServiceCollection
��+ =
services
��> F
)
��F G
{
�� 
services
�� 
.
�� 
	AddScoped
�� 
<
�� &
IWalletHistoryRepository
�� 3
,
��3 4%
WalletHistoryRepository
��5 L
>
��L M
(
��M N
)
��N O
;
��O P
services
�� 
.
�� 
	AddScoped
�� 
<
�� %
IWalletPeriodRepository
�� 2
,
��2 3$
WalletPeriodRepository
��4 J
>
��J K
(
��K L
)
��L M
;
��M N
services
�� 
.
�� 
	AddScoped
�� 
<
�� &
IWalletRequestRepository
�� 3
,
��3 4%
WalletRequestRepository
��5 L
>
��L M
(
��M N
)
��N O
;
��O P
services
�� 
.
�� 
	AddScoped
�� 
<
�� .
 IWalletRetentionConfigRepository
�� ;
,
��; <-
WalletRetentionConfigRepository
��= \
>
��\ ]
(
��] ^
)
��^ _
;
��_ `
services
�� 
.
�� 
	AddScoped
�� 
<
�� 
IWalletRepository
�� ,
,
��, -
WalletRepository
��. >
>
��> ?
(
��? @
)
��@ A
;
��A B
services
�� 
.
�� 
	AddScoped
�� 
<
�� #
IWalletWaitRepository
�� 0
,
��0 1"
WalletWaitRepository
��2 F
>
��F G
(
��G H
)
��H I
;
��I J
services
�� 
.
�� 
	AddScoped
�� 
<
�� )
IWalletWithDrawalRepository
�� 6
,
��6 7(
WalletWithDrawalRepository
��8 R
>
��R S
(
��S T
)
��T U
;
��U V
services
�� 
.
�� 
	AddScoped
�� 
<
�� &
IInvoiceDetailRepository
�� 3
,
��3 4%
InvoiceDetailRepository
��5 L
>
��L M
(
��M N
)
��N O
;
��O P
services
�� 
.
�� 
	AddScoped
�� 
<
��  
IInvoiceRepository
�� -
,
��- .
InvoiceRepository
��/ @
>
��@ A
(
��A B
)
��B C
;
��C D
services
�� 
.
�� 
	AddScoped
�� 
<
��  
IInvoiceRepository
�� -
,
��- .
InvoiceRepository
��/ @
>
��@ A
(
��A B
)
��B C
;
��C D
services
�� 
.
�� 
	AddScoped
�� 
<
�� (
INetworkPurchaseRepository
�� 5
,
��5 6'
NetworkPurchaseRepository
��7 P
>
��P Q
(
��Q R
)
��R S
;
��S T
services
�� 
.
�� 
	AddScoped
�� 
<
�� -
IEcoPoolConfigurationRepository
�� :
,
��: ;,
EcoPoolConfigurationRepository
��< Z
>
��Z [
(
��[ \
)
��\ ]
;
��] ^
services
�� 
.
�� 
	AddScoped
�� 
<
�� '
IResultsEcoPoolRepository
�� 4
,
��4 5&
ResultsEcoPoolRepository
��6 N
>
��N O
(
��O P
)
��P Q
;
��Q R
services
�� 
.
�� 
	AddScoped
�� 
<
�� "
IApiClientRepository
�� /
,
��/ 0!
ApiClientRepository
��1 D
>
��D E
(
��E F
)
��F G
;
��G H
services
�� 
.
�� 
	AddScoped
�� 
<
�� /
!ICoinPaymentTransactionRepository
�� <
,
��< =.
 CoinPaymentTransactionRepository
��> ^
>
��^ _
(
��_ `
)
��` a
;
��a b
services
�� 
.
�� 
	AddScoped
�� 
<
�� &
IWalletModel1ARepository
�� 3
,
��3 4%
WalletModel1ARepository
��5 L
>
��L M
(
��M N
)
��N O
;
��O P
services
�� 
.
�� 
	AddScoped
�� 
<
�� &
IWalletModel1BRepository
�� 3
,
��3 4%
WalletModel1BRepository
��5 L
>
��L M
(
��M N
)
��N O
;
��O P
services
�� 
.
�� 
	AddScoped
�� 
<
�� 
IBrandRepository
�� +
,
��+ ,
BrandRepository
��- <
>
��< =
(
��= >
)
��> ?
;
��? @
services
�� 
.
�� 
	AddScoped
�� 
<
�� 
IBonusRepository
�� +
,
��+ ,
BonusRepository
��- <
>
��< =
(
��= >
)
��> ?
;
��? @
services
�� 
.
�� 
	AddScoped
�� 
<
�� 
ICreditRepository
�� ,
,
��, -
CreditRepository
��. >
>
��> ?
(
��? @
)
��@ A
;
��A B
}
�� 
private
�� 
static
�� 
void
�� 
InjectUnitOfWork
�� (
(
��( ) 
IServiceCollection
��) ;
services
��< D
)
��D E
{
�� 
services
�� 
.
�� 
	AddScoped
�� 
<
�� 
	DbContext
�� $
>
��$ %
(
��% &
provider
��& .
=>
��/ 1
provider
�� 
.
�� 

GetService
�� 
<
��  $
WalletServiceDbContext
��  6
>
��6 7
(
��7 8
)
��8 9
)
��9 :
;
��: ;
services
�� 
.
�� 
	AddScoped
�� 
<
�� 
IUnitOfWork
�� &
,
��& '

UnitOfWork
��( 2
>
��2 3
(
��3 4
)
��4 5
;
��5 6
}
�� 
private
�� 
static
�� 
void
�� 
InjectAdapters
�� &
(
��& ' 
IServiceCollection
��' 9
services
��: B
)
��B C
{
�� 
services
�� 
.
�� 
	AddScoped
�� 
<
�� 
ICoinPayAdapter
�� *
,
��* +
CoinPayAdapter
��, :
>
��: ;
(
��; <
)
��< =
;
��= >
services
�� 
.
�� 
	AddScoped
�� 
<
�� $
IAccountServiceAdapter
�� 1
,
��1 2#
AccountServiceAdapter
��3 H
>
��H I
(
��I J
)
��J K
;
��K L
services
�� 
.
�� 
	AddScoped
�� 
<
�� &
IInventoryServiceAdapter
�� 3
,
��3 4%
InventoryServiceAdapter
��5 L
>
��L M
(
��M N
)
��N O
;
��O P
services
�� 
.
�� 
	AddScoped
�� 
<
�� #
IConfigurationAdapter
�� 0
,
��0 1"
ConfigurationAdapter
��2 F
>
��F G
(
��G H
)
��H I
;
��I J
}
�� 
private
�� 
static
�� 
void
�� 
InjectServices
�� &
(
��& ' 
IServiceCollection
��' 9
services
��: B
)
��B C
{
�� 
services
�� 
.
�� 
	AddScoped
�� 
<
�� 
ICoinPayService
�� *
,
��* +
CoinPayService
��, :
>
��: ;
(
��; <
)
��< =
;
��= >
services
�� 
.
�� 
	AddScoped
�� 
<
�� #
IWalletHistoryService
�� 0
,
��0 1"
WalletHistoryService
��2 F
>
��F G
(
��G H
)
��H I
;
��I J
services
�� 
.
�� 
	AddScoped
�� 
<
�� "
IWalletPeriodService
�� /
,
��/ 0!
WalletPeriodService
��1 D
>
��D E
(
��E F
)
��F G
;
��G H
services
�� 
.
�� 
	AddScoped
�� 
<
�� #
IWalletRequestService
�� 0
,
��0 1"
WalletRequestService
��2 F
>
��F G
(
��G H
)
��H I
;
��I J
services
�� 
.
�� 
	AddScoped
�� 
<
�� +
IWalletRetentionConfigService
�� 8
,
��8 9*
WalletRetentionConfigService
��: V
>
��V W
(
��W X
)
��X Y
;
��Y Z
services
�� 
.
�� 
	AddScoped
�� 
<
�� 
IWalletService
�� )
,
��) *
Core
��+ /
.
��/ 0
Services
��0 8
.
��8 9
WalletService
��9 F
>
��F G
(
��G H
)
��H I
;
��I J
services
�� 
.
�� 
	AddScoped
�� 
<
��  
IWalletWaitService
�� -
,
��- .
WalletWaitService
��/ @
>
��@ A
(
��A B
)
��B C
;
��C D
services
�� 
.
�� 
	AddScoped
�� 
<
�� &
IWalletWithdrawalService
�� 3
,
��3 4%
WalletWithDrawalService
��5 L
>
��L M
(
��M N
)
��N O
;
��O P
services
�� 
.
�� 
	AddScoped
�� 
<
�� #
IInvoiceDetailService
�� 0
,
��0 1"
InvoiceDetailService
��2 F
>
��F G
(
��G H
)
��H I
;
��I J
services
�� 
.
�� 
	AddScoped
�� 
<
�� 
IInvoiceService
�� *
,
��* +
InvoiceService
��, :
>
��: ;
(
��; <
)
��< =
;
��= >
services
�� 
.
�� 
	AddScoped
�� 
<
�� $
IProcessGradingService
�� 1
,
��1 2#
ProcessGradingService
��3 H
>
��H I
(
��I J
)
��J K
;
��K L
services
�� 
.
�� 
	AddScoped
�� 
<
�� *
IEcoPoolConfigurationService
�� 7
,
��7 8)
EcoPoolConfigurationService
��9 T
>
��T U
(
��U V
)
��V W
;
��W X
services
�� 
.
�� 
	AddScoped
�� 
<
�� "
IEcosystemPdfService
�� /
,
��/ 0!
EcosystemPdfService
��1 D
>
��D E
(
��E F
)
��F G
;
��G H
services
�� 
.
�� 
	AddScoped
�� 
<
�� $
IResultsEcoPoolService
�� 1
,
��1 2#
ResultsEcoPoolService
��3 H
>
��H I
(
��I J
)
��J K
;
��K L
services
�� 
.
�� 
	AddScoped
�� 
<
��  
IConPaymentService
�� -
,
��- .
ConPaymentService
��/ @
>
��@ A
(
��A B
)
��B C
;
��C D
services
�� 
.
�� 
	AddScoped
�� 
<
��  
IBrevoEmailService
�� -
,
��- .
BrevoEmailService
��/ @
>
��@ A
(
��A B
)
��B C
;
��C D
services
�� 
.
�� 
	AddScoped
�� 
<
�� (
IPaymentTransactionService
�� 5
,
��5 6'
PaymentTransactionService
��7 P
>
��P Q
(
��Q R
)
��R S
;
��S T
services
�� 
.
�� 
	AddScoped
�� 
<
�� #
IWalletModel1AService
�� 0
,
��0 1"
WalletModel1AService
��2 F
>
��F G
(
��G H
)
��H I
;
��I J
services
�� 
.
�� 
	AddScoped
�� 
<
�� #
IWalletModel1BService
�� 0
,
��0 1"
WalletModel1BService
��2 F
>
��F G
(
��G H
)
��H I
;
��I J
services
�� 
.
�� 
	AddScoped
�� 
<
�� $
IUserStatisticsService
�� 1
,
��1 2#
UserStatisticsService
��3 H
>
��H I
(
��I J
)
��J K
;
��K L
services
�� 
.
�� 
	AddScoped
�� 
<
�� 
IBrandService
�� (
,
��( )
BrandService
��* 6
>
��6 7
(
��7 8
)
��8 9
;
��9 :
services
�� 
.
�� 
	AddScoped
�� 
<
�� !
IRecyCoinPdfService
�� .
,
��. / 
RecyCoinPdfService
��0 B
>
��B C
(
��C D
)
��D E
;
��E F
services
�� 
.
�� 
	AddScoped
�� 
<
�� "
IHouseCoinPdfService
�� /
,
��/ 0!
HouseCoinPdfService
��1 D
>
��D E
(
��E F
)
��F G
;
��G H
services
�� 
.
�� 
	AddScoped
�� 
<
�� '
IRedisCacheCleanupService
�� 4
,
��4 5&
RedisCacheCleanupService
��6 N
>
��N O
(
��O P
)
��P Q
;
��Q R
services
�� 
.
�� 
	AddScoped
�� 
<
�� $
IExitoJuntosPdfService
�� 1
,
��1 2#
ExitoJuntosPdfService
��3 H
>
��H I
(
��I J
)
��J K
;
��K L
}
�� 
private
�� 
static
�� 
void
�� 
InjectPackages
�� &
(
��& ' 
IServiceCollection
��' 9
services
��: B
)
��B C
{
�� 
services
�� 
.
�� 
AddAutoMapper
�� 
(
�� 
x
��  
=>
��! #
{
��$ %
x
��& '
.
��' (

AddProfile
��( 2
(
��2 3
new
��3 6
MapperProfile
��7 D
(
��D E
)
��E F
)
��F G
;
��G H
}
��I J
)
��J K
;
��K L
}
�� 
private
�� 
static
�� 
void
�� %
RegisterServiceProvider
�� /
(
��/ 0 
IServiceCollection
��0 B
services
��C K
)
��K L
{
�� 
services
�� 
.
�� 
AddSingleton
�� 
<
�� 

HttpClient
�� (
>
��( )
(
��) *
)
��* +
;
��+ ,
services
�� 
.
�� 
AddSingleton
�� 
(
�� 
services
�� &
.
��& '"
BuildServiceProvider
��' ;
(
��; <
)
��< =
)
��= >
;
��> ?
}
�� 
private
�� 
static
�� 
void
�� *
InjectSingletonsAndFactories
�� 4
(
��4 5 
IServiceCollection
��5 G
services
��H P
)
��P Q
{
�� 
services
�� 
.
�� 
AddHttpClient
�� 
(
�� 
)
��  
;
��  !
services
�� 
.
�� $
AddHttpContextAccessor
�� '
(
��' (
)
��( )
;
��) *
services
�� 
.
�� 
AddSingleton
�� 
(
�� 
_
�� 
=
��  !
new
��" %
KafkaProducer
��& 3
(
��3 4
services
��4 <
)
��< =
)
��= >
;
��> ?
}
�� 
}�� 