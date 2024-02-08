using AutoMapper;
using WalletService.Data.Database.CustomModels;
using WalletService.Data.Database.Models;
using WalletService.Models.DTO.CoinPaymentTransactionDto;
using WalletService.Models.DTO.InvoiceDetailDto;
using WalletService.Models.DTO.InvoiceDto;
using WalletService.Models.DTO.ProcessGradingDto;
using WalletService.Models.DTO.ResultEcoPoolLevelsDto;
using WalletService.Models.DTO.ResultsEcoPoolDto;
using WalletService.Models.DTO.WalletDto;
using WalletService.Models.DTO.WalletHistoryDto;
using WalletService.Models.DTO.WalletPeriodDto;
using WalletService.Models.DTO.WalletRequestDto;
using WalletService.Models.DTO.WalletRetentionConfigDto;
using WalletService.Models.DTO.WalletWaitDto;
using WalletService.Models.DTO.WalletWithDrawalDto;
using WalletService.Models.Requests.ConPaymentRequest;
using WalletService.Models.Requests.InvoiceDetailRequest;
using WalletService.Models.Requests.InvoiceRequest;
using WalletService.Models.Requests.PaymentTransaction;
using WalletService.Models.Requests.WalletHistoryRequest;
using WalletService.Models.Requests.WalletPeriodRequest;
using WalletService.Models.Requests.WalletRequest;
using WalletService.Models.Requests.WalletRequestRequest;
using WalletService.Models.Requests.WalletRetentionConfigRequest;
using WalletService.Models.Requests.WalletTransactionRequest;
using WalletService.Models.Requests.WalletWaitRequest;
using WalletService.Models.Requests.WalletWithDrawalRequest;

namespace WalletService.Core.Mapper;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        MapDto();
    }

    private void MapDto()
    {
        CreateMap<Wallets, WalletDto>();
        CreateMap<WalletsHistories, WalletHistoryDto>();
        CreateMap<WalletsPeriods, WalletPeriodDto>();
        CreateMap<WalletsRetentionsConfigs, WalletRetentionConfigDto>();
        CreateMap<WalletsWaits, WalletWaitDto>();
        CreateMap<WalletsWithdrawals, WalletWithDrawalDto>();
        CreateMap<WalletsRequests, WalletRequestDto>();
        CreateMap<Invoices, InvoiceDto>();
        CreateMap<Invoices, InvoiceDTO>();
        CreateMap<InvoicesDetails, InvoiceDetailDto>();
        CreateMap<ModelConfiguration, ModelConfigurationDto>();
        CreateMap<ModelConfigurationLevels, EcoPoolLevelDto>();
        CreateMap<WalletTransactionRequest, Wallets>();
        CreateMap<ResultsModel2, ResultsEcoPoolDto>();
        CreateMap<ResultsModel2Levels, ResultEcoPoolLevelsDto>();
        CreateMap<PaymentTransaction, PaymentTransactionDto>();
        CreateMap<InvoicesTradingAcademyResponse, InvoiceTradingAcademyDto>();
        CreateMap<InvoiceModelOneAndTwoResponse, InvoiceModelOneAndTwoDto>();
     

        CreateMap<WalletRequestRequest, WalletsRequests>()
            .ForMember(d => d.Id, map => map.Ignore())
            .ForMember(d => d.UpdatedAt, map => map.Ignore())
            .ForMember(d => d.CreatedAt, map => map.Ignore());

        CreateMap<WalletRequest, Wallets>()
            .ForMember(d => d.Id, map => map.Ignore())
            .ForMember(d => d.UpdatedAt, map => map.Ignore())
            .ForMember(d => d.CreatedAt, map => map.Ignore());

        CreateMap<WalletHistoryRequest, WalletsHistories>()
            .ForMember(d => d.Id, map => map.Ignore())
            .ForMember(d => d.UpdatedAt, map => map.Ignore())
            .ForMember(d => d.CreatedAt, map => map.Ignore());

        CreateMap<WalletPeriodRequest, WalletsPeriods>()
            .ForMember(d => d.Id, map => map.Ignore())
            .ForMember(d => d.UpdatedAt, map => map.Ignore())
            .ForMember(d => d.CreatedAt, map => map.Ignore());

        CreateMap<WalletRetentionConfigRequest, WalletsRetentionsConfigs>()
            .ForMember(d => d.Id, map => map.Ignore())
            .ForMember(d => d.UpdatedAt, map => map.Ignore())
            .ForMember(d => d.CreatedAt, map => map.Ignore());

        CreateMap<WalletWaitRequest, WalletsWaits>()
            .ForMember(d => d.Id, map => map.Ignore())
            .ForMember(d => d.UpdatedAt, map => map.Ignore())
            .ForMember(d => d.CreatedAt, map => map.Ignore());

        CreateMap<WalletWithDrawalRequest, WalletsWithdrawals>()
            .ForMember(d => d.Id, map => map.Ignore())
            .ForMember(d => d.UpdatedAt, map => map.Ignore())
            .ForMember(d => d.CreatedAt, map => map.Ignore());

        CreateMap<InvoiceRequest, Invoices>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.InvoiceDetail, opt => opt.Ignore());

        CreateMap<Invoices, InvoiceDto>()
            .ForMember(dest => dest.InvoiceDetail, opt => opt.MapFrom(src => src.InvoiceDetail));
        
        CreateMap<InvoiceDetailRequest, InvoicesDetails>()
            .ForMember(dest => dest.Invoice, opt => opt.Ignore());

        CreateMap<WalletTransactionRequest, Wallets>()
            .ForMember(d => d.Id, map => map.Ignore())
            .ForMember(d => d.UpdatedAt, map => map.Ignore())
            .ForMember(d => d.CreatedAt, map => map.Ignore());

        CreateMap<ResultsEcoPoolDto, ResultsModel2>()
            .ForMember(d => d.Id, map => map.Ignore())
            .ForMember(dest => dest.ResultsModel2Levels, opt => opt.MapFrom(src => src.ResultEcoPoolLevels));

        CreateMap<ResultEcoPoolLevelsDto, ResultsModel2Levels>()
            .ForMember(d => d.Id, map => map.Ignore());

        CreateMap<CoinPaymentTransactionRequest, PaymentTransaction>()
            .ForMember(d => d.Id, map => map.Ignore())
            .ForMember(d => d.UpdatedAt, map => map.Ignore())
            .ForMember(d => d.CreatedAt, map => map.Ignore());
        
        CreateMap<PaymentTransactionRequest, PaymentTransaction>()
            .ForMember(d => d.Id, map => map.Ignore())
            .ForMember(d => d.UpdatedAt, map => map.Ignore())
            .ForMember(d => d.CreatedAt, map => map.Ignore());
    }
}