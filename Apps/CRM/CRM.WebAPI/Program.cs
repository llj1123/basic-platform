SelfLog.Enable(Console.Error);

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;
var host = builder.Host;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
// Add services to the container.
services.AddAthenaProvider();
// services.AddCustomOpenTelemetry<Program>(configuration);
services.AddCustomMediatR(
    Assembly.Load("CRM.CommandHandlers.SqlSugar"),
    Assembly.Load("CRM.DomainEventHandlers.SqlSugar")
);
services.AddCustomServiceComponent(
    Assembly.Load("CRM.QueryServices.SqlSugar"),
    Assembly.Load("App.Infrastructure"),
    Assembly.GetExecutingAssembly()
);
services.AddCustomSwaggerGen(configuration);
// 添加ORM
services.AddCustomSqlSugar(configuration);
// 添加集成事件支持
services.AddCustomIntegrationEvent(configuration, capOptions =>
{
    // Dashboard
    capOptions.UseDashboard();
}, new[]
{
    Assembly.Load("CRM.ProcessManagers")
});

services.AddCustomCsRedisCache(configuration);
services.AddCustomApiPermission();
services.AddCustomDataPermission(configuration);
services.AddCustomJwtAuthWithSignalR(configuration);
services.AddCustomSignalRWithRedis(configuration);
services.AddCustomCors(configuration);
services.AddCustomStorageLogger(configuration);
services.AddCustomController().AddNewtonsoftJson();

host.ConfigureLogging((_, loggingBuilder) => loggingBuilder.ClearProviders())
    .UseSerilog((ctx, cfg) =>
        cfg.ReadFrom.Configuration(ctx.Configuration)
    )
    .UseDefaultServiceProvider(options => { options.ValidateScopes = false; });
var app = builder.Build();

app.UseAthenaProvider();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCustomSwagger();
}

app.UseStaticFiles();
app.UseCors();
//启用验证
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseCustomAuditLog();
app.MapCustomSignalR();
// app.MapSpaFront();
// app.MapHealth();

app.Run();