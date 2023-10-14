using AutoMapper;

namespace WalletService.Core.Services;

public class BaseService
{
    protected readonly IMapper Mapper;
    protected BaseService(IMapper mapper)
        => Mapper = mapper;
}