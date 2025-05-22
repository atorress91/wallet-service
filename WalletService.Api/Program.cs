using Hangfire;
using QuestPDF.Infrastructure;
using WalletService.Core.BackgroundServices;
using WalletService.Core.Job;
using WalletService.Core.Middlewares;
using WalletService.Ioc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.IocAppInjectDependencies();

builder.Services.AddTransient<MatrixQualificationJob>();

QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddEndpointsApiExplorer();
var cleanupOptions = builder.Configuration.GetSection("CacheCleanup");
var cleanupEnabled = cleanupOptions.GetValue<bool>("Enabled");

if (cleanupEnabled)
    builder.Services.AddHostedService<RedisCacheCleanupBackgroundService>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.UseRouting();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new AllowAllDashboardAuthorizationFilter() }
});
// RecurringJob.AddOrUpdate<MatrixQualificationJob>(
//     "matrix-qualifications-job",        
//     job => job.ExecuteAsync(),           
//     Cron.Hourly                         
// );

app.UseCors();
app.UseSwagger();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
app.UseSwaggerUI(s => { s.SwaggerEndpoint("/swagger/v1/swagger.json", "WalletService API"); });
app.UseHttpsRedirection();
app.UseMiddleware<TokenValidationMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllers();
app.Run();