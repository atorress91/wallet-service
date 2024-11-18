using QuestPDF.Infrastructure;
using WalletService.Core.BackgroundServices;
using WalletService.Core.Middlewares;
using WalletService.Ioc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.IocAppInjectDependencies();


QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHostedService<RedisCacheCleanupBackgroundService>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseCors();
app.UseSwagger();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
app.UseSwaggerUI(s => { s.SwaggerEndpoint("/swagger/v1/swagger.json", "WalletService API"); });
app.UseHttpsRedirection();
app.UseMiddleware<TokenValidationMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.UseRouting();
app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllers();
app.Run();