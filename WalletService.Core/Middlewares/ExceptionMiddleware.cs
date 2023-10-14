using Microsoft.AspNetCore.Http;
using System.Net.Mime;
using System.Net;
using WalletService.Models.Exceptions;
using WalletService.Models.Responses;
using WalletService.Utility.Extensions;

namespace WalletService.Core.Middlewares;


public class ExceptionMiddleware
{
	private readonly RequestDelegate _next;

	public ExceptionMiddleware(RequestDelegate next)
	{
		_next = next;
	}


	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception e)
		{
			await HandleExceptionAsync(context, e);
		}
	}


	private static Task HandleExceptionAsync(HttpContext context, Exception exception)
	{
		string exceptionBody;

		context.Response.ContentType = MediaTypeNames.Application.Json;
		context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

		switch (exception)
		{
			case CustomException customException:
				context.Response.StatusCode = (int)customException.StatusCode;
				exceptionBody = customException.ExceptionBody ?? string.Empty;

				return context.Response.WriteAsync(exceptionBody);
			default:
				context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				exceptionBody = exception.Message;

				var response = new ServicesResponse
				{
					Success = false,
					Code = context.Response.StatusCode,
					Message = exceptionBody
				};

				return context.Response.WriteAsync(response.ToJsonString());
		}
	}
}