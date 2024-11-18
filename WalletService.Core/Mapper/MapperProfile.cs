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
using WalletService.Models.Requests.PagaditoRequest;
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
        CreateMap<Wallet, WalletDto>();
        CreateMap<WalletsHistory, WalletHistoryDto>();
        CreateMap<WalletsPeriod, WalletPeriodDto>();
        CreateMap<WalletsRetentionsConfig, WalletRetentionConfigDto>();
        CreateMap<WalletsWait, WalletWaitDto>();
        CreateMap<WalletsWithdrawal, WalletWithDrawalDto>();
        CreateMap<WalletsRequest, WalletRequestDto>();
        CreateMap<Invoice, InvoiceDto>();
        CreateMap<Invoice, InvoiceDTO>();
        CreateMap<InvoicesDetail, InvoiceDetailDto>();
        CreateMap<ModelConfiguration, ModelConfigurationDto>();
        CreateMap<ModelConfigurationLevel, EcoPoolLevelDto>();
        CreateMap<WalletTransactionRequest, Wallet>();
        CreateMap<ResultsModel2, ResultsEcoPoolDto>();
        CreateMap<ResultsModel2Level, ResultEcoPoolLevelsDto>();
        CreateMap<CoinpaymentTransaction, PaymentTransactionDto>();
        CreateMap<InvoicesTradingAcademyResponse, InvoiceTradingAcademyDto>();
        CreateMap<InvoiceModelOneAndTwoResponse, InvoiceModelOneAndTwoDto>();


        CreateMap<WalletRequestRequest, WalletsRequest>()
            .ForMember(d => d.Id, map => map.Ignore())
            .ForMember(d => d.UpdatedAt, map => map.Ignore())
            .ForMember(d => d.CreatedAt, map => map.Ignore());

        CreateMap<WalletRequest, Wallet>()
            .ForMember(d => d.Id, map => map.Ignore())
            .ForMember(d => d.UpdatedAt, map => map.Ignore())
            .ForMember(d => d.CreatedAt, map => map.Ignore());

        CreateMap<WalletHistoryRequest, WalletsHistory>()
            .ForMember(d => d.Id, map => map.Ignore())
            .ForMember(d => d.UpdatedAt, map => map.Ignore())
            .ForMember(d => d.CreatedAt, map => map.Ignore());

        CreateMap<WalletPeriodRequest, WalletsPeriod>()
            .ForMember(d => d.Id, map => map.Ignore())
            .ForMember(d => d.UpdatedAt, map => map.Ignore())
            .ForMember(d => d.CreatedAt, map => map.Ignore());

        CreateMap<WalletRetentionConfigRequest, WalletsRetentionsConfig>()
            .ForMember(d => d.Id, map => map.Ignore())
            .ForMember(d => d.UpdatedAt, map => map.Ignore())
            .ForMember(d => d.CreatedAt, map => map.Ignore());

        CreateMap<WalletWaitRequest, WalletsWait>()
            .ForMember(d => d.Id, map => map.Ignore())
            .ForMember(d => d.UpdatedAt, map => map.Ignore())
            .ForMember(d => d.CreatedAt, map => map.Ignore());

        CreateMap<WalletWithDrawalRequest, WalletsWithdrawal>()
            .ForMember(d => d.Id, map => map.Ignore())
            .ForMember(d => d.UpdatedAt, map => map.Ignore())
            .ForMember(d => d.CreatedAt, map => map.Ignore());

        CreateMap<InvoiceRequest, Invoice>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.InvoicesDetails, opt => opt.Ignore());

        CreateMap<Invoice, InvoiceDto>()
            .ForMember(dest => dest.InvoiceDetail, opt => opt.MapFrom(src => src.InvoicesDetails));

        CreateMap<InvoiceDetailRequest, InvoicesDetail>()
            .ForMember(dest => dest.Invoice, opt => opt.Ignore());

        CreateMap<WalletTransactionRequest, Wallet>()
            .ForMember(d => d.Id, map => map.Ignore())
            .ForMember(d => d.UpdatedAt, map => map.Ignore())
            .ForMember(d => d.CreatedAt, map => map.Ignore());

        CreateMap<ResultsEcoPoolDto, ResultsModel2>()
            .ForMember(d => d.Id, map => map.Ignore())
            .ForMember(dest => dest.ResultsModel2Levels, opt => opt.MapFrom(src => src.ResultEcoPoolLevels));

        CreateMap<ResultEcoPoolLevelsDto, ResultsModel2Level>()
            .ForMember(d => d.Id, map => map.Ignore());

        CreateMap<CoinPaymentTransactionRequest, CoinpaymentTransaction>()
            .ForMember(d => d.Id, map => map.Ignore())
            .ForMember(d => d.UpdatedAt, map => map.Ignore())
            .ForMember(d => d.CreatedAt, map => map.Ignore());

        CreateMap<PaymentTransactionRequest, CoinpaymentTransaction>()
            .ForMember(d => d.Id, map => map.Ignore())
            .ForMember(d => d.UpdatedAt, map => map.Ignore())
            .ForMember(d => d.CreatedAt, map => map.Ignore());

        CreateMap<CreatePagaditoTransactionRequest, CreatePagaditoTransaction>()
            .ForMember(d => d.Token, map => map.Ignore())
            .ForMember(d => d.Ern, map => map.Ignore());

        CreateMap<PagaditoTransactionDetailRequest, PagaditoTransactionDetail>();
    }
}