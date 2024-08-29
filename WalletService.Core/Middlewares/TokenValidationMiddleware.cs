using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using System.Net;
using WalletService.Data.Repositories.IRepositories;
using WalletService.Models.Responses;
using WalletService.Utility.Extensions;

namespace WalletService.Core.Middlewares;

public class TokenValidationMiddleware
{
    private readonly RequestDelegate _next;

    public TokenValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IApiClientRepository apiClientService,
        IBrandRepository brandRepository, ILogger<TokenValidationMiddleware> logger)
    {
        if (context.Request.Path.Value!.Equals("/health", StringComparison.OrdinalIgnoreCase) ||
            context.Request.Path.Value.Equals("/api/v1/ConPayments/coinPaymentsIPN",
                StringComparison.OrdinalIgnoreCase) ||
            context.Request.Path.Value.Equals("/api/v1/Pagadito/webhook", StringComparison.OrdinalIgnoreCase) ||
            context.Request.Path.Value.Equals("/api/v1/CoinPay/Webhook", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var token = context.Request.Headers["Authorization"].ToString();
        var secretKey = context.Request.Headers["X-Client-ID"].ToString();

        if (string.IsNullOrEmpty(token) && context.Request.HasFormContentType)
        {
            var form = await context.Request.ReadFormAsync();
            token = form["Authorization"];
        }

        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(secretKey))
        {
            var response = new ServicesResponse
            {
                Success = false,
                Code = (int)HttpStatusCode.Unauthorized,
                Message = "Unauthorized"
            };

            logger.LogInformation($"Unauthorized, token non found. URL:{context.Request.GetDisplayUrl()}");
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync(response.ToJsonString());
            return;
        }

        var isValidate = await apiClientService.ValidateApiClient(token);
        var brand = await brandRepository.GetBrandByIdAsync(secretKey);
        if (!isValidate || brand == null)
        {
            var response = new ServicesResponse
            {
                Success = false,
                Code = (int)HttpStatusCode.Unauthorized,
                Message = "Unauthorized"
            };

            logger.LogInformation($"Unauthorized, token non found. URL:{context.Request.GetDisplayUrl()}");
            context.Response.StatusCode = context.Response.StatusCode = 401;
            await context.Response.WriteAsync(response.ToJsonString());
            return;
        }

        context.Items["brandId"] = brand.Id;
        logger.LogInformation($"Request accepted. CLIENT URL:{context.Request.GetDisplayUrl()}");

        await _next(context);
    }
}