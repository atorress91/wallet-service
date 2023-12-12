namespace WalletService.Models.Constants;

public static class Constants
{
    public const int    OriginEcoPoolPurchase             = 2;
    public const string EcoPoolProductCategory            = "Pool Adquisitivo";
    public const string EcoPoolProductCategoryForAdmin    = "Pool Adquisitivo por AdminEcosystem";
    public const string SubjectConfirmPurchase            = "Confirmación de compra";
    public const string SubjectConfirmAffiliation         = "Membresía activa";
    public const string RevertEcoPoolConcept              = "Devolucion Pool Adquisitivo";
    public const string AdminEcosystemUserName            = "adminecosystem";
    public const int    AdminUserId                       = 1;
    public const string WalletBalance                     = "Saldo de billetera";
    public const string AdminPayment                      = "Administrativo";
    public const string ReverseBalance                    = "Saldo revertido";
    public const string PurchasingPoolFor                 = "Pool Adquisitivo para";
    public const string PurchasingPoolTo                  = "Pool Adquisitivo de";
    public const string ConPaymentAddress                 = "THiJ78d6DHm1575GfFyfe1K6k2Fp6Bq5pP";
    public const string ConPaymentCurrency                = "USDT.TRC20";
    public const int    MembershipBonus                   = 5;
    public const string SubjectConfirmBonus               = "Comisión de referido";
    public const int    DaysToPayQuantity                 = 999;
    public const string CoinPayments                      = "CoinPayments";
    public const string CoinPay                           = "CoinPay";
    public const int    Batches                           = 1000;
    public const string WithdrawalBalance                 = "Retiro de saldo";
    public const int    None                              = 0;
    public const string Membership                        = "Membresía";
    public const string TransferForMembership             = "Transferencia para membresía al afiliado";
    public const string TransferToMembership              = "Transferencia para membresía del afiliado";
    public const int    CoinPaymentTax                    = 2;
    public const string DebitTransationSP                 = "dbo.Debit_Transation_SP";
    public const string DebitEcoPoolTransationSP          = "dbo.Debit_Transation_EcoPool_SP";
    public const string AdminDebitTransactionSp           = "dbo.Admin_Debit_Transation_SP";
    public const string HandleDebitTransationSP           = "dbo.HandleDebit_Transation_SP";
    public const string CreditTransationSP                = "dbo.Credit_Transation_SP";
    public const string ModelThreeRequestSP               = "dbo.CreateEcoPoolSP";
    public const string ModelTwoRequestSP                 = "dbo.CreateModel2ResultsSP";
    public const string RevertDebitTransaction            = "dbo.Revert_Debit_Transaction_SP";
    public const string CoinPaymentRevertTransactions     = "dbo.CoinPaymentsRevertTransactions_SP";
    public const string HandleMembershipTransactions      = "dbo.HandleMembershipTransactionSp";
    public const string MembershipDebitTransactions       = "dbo.MembershipDebit_Transation_SP";
    public const string GetTotalPurchasesInMyNetworkSp    = "dbo.sp_GetTotalPurchasesInMyNetworkByAffiliate";
    public const string TypeTableAffiliateId              = "dbo.TypeTableAffiliateId";
    public const string BulkAdministrativeDebitSp         = "dbo.Bulk_Administrative_Debit_Sp";
    public const string CommissionModelTwoDescription     = "Comisión Modelo 3 por {0}, nivel {1}";
    public const string CommissionModelTwoDescriptionNormal   = "Comisión Modelo 3";
    public const string CommissionModelThreeDescription     = "Comisión Modelo 2 por {0}, nivel {1}";
    public const string CommissionModelThreeDescriptionNormal   = "Comisión Modelo 2";
    public const string CommissionMembership              = "Comisión de membresía por";
    public const string ConceptBinaryPayment              = "Pago Modelo 4 - ${0} - Calificacion {1}";
    public const string ConceptModelFivePayment           = "Pago Modelo 5 - ${0} - Calificacion {1}";
    public const string ConceptModelSixPayment            = "Pago Modelo 6 - ${0} - Calificacion {1}";
    public const string ConceptCommissionBinaryPayment    = "Comisión Modelo 4 - ${0} - Calificacion {1}";
    public const string ConceptCommissionModelFivePayment = "Comisión Modelo 5 - ${0} - Calificacion {1}";
    public const string ConceptCommissionModelSixPayment  = "Comisión Modelo 6 - ${0} - Calificacion {1}";
    public const string DefaultWithdrawalZone             = "Central America Standard Time";
    public const int    UsdtIdCurrency                    = 19;
    public const int    UsdtIdNetwork                     = 99;
    public const int    CoinPayIdWallet                   = 46616;
    public const int ChildrenLimitModel4                  = 2;
    public const int ChildrenLimitModel5                  = 4;
    public const int LevelsLimitModel5                    = 8;
    public const int ChildrenLimitModel6                  = 3;
    public const int LevelsLimitModel6                   = 10;
    
    //Models
    public static string[] Model4Level      = { "Cliente", "Colaborador" };
    public static int[] CustomerModel4Scope = { 1, 2, 3};
    public const int CustomerModel5Scope    = 4;
    public static int[] CustomerModel6Scope = { 5,6,7,8,9,10, 11 };
    public static string[] Model5Level      = { "Ejecutivo" };
    public static string[] Model6Level      =
    {
        "Gestor",
        "Coordinador",
        "Supervisor",
        "Asesor",
        "VP_Comercial",
        "Subdirector",
        "Director"
    };
    public static string[] PartitionKeys    =
        { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "12", "13", "14", "18", "29", "30" };
}