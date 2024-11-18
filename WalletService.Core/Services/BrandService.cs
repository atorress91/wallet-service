using AutoMapper;
using Microsoft.AspNetCore.Http;
using WalletService.Core.Services.IServices;

namespace WalletService.Core.Services;

public class BrandService : BaseService, IBrandService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BrandService(IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(mapper)
        => _httpContextAccessor = httpContextAccessor;


    public long BrandId => (long)(_httpContextAccessor.HttpContext?.Items["brandId"] ?? 0);
}