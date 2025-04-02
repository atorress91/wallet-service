using AutoMapper;
using Microsoft.AspNetCore.Http;
using WalletService.Core.Services.IServices;

namespace WalletService.Core.Services;

public class BrandService : BaseService, IBrandService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BrandService(IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(mapper)
        => _httpContextAccessor = httpContextAccessor;
    
    public long BrandId
    {
        get
        {
            var brandId = _httpContextAccessor.HttpContext?.Items["brandId"];
            if (brandId == null) return 0;
            
            return Convert.ToInt64(brandId);
        }
    }
}