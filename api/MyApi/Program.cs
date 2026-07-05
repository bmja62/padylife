using Application.Cqrs;
using Application.Points.Extentions;
using Common;
using Hangfire;
using Hangfire.PostgreSql;
using IdGen.DependencyInjection;
using Serilog;
using Services.Hubs;
using Services.Services.SmsStrategy.Kavenegar;
using Services.Services.SmsStrategy.SmsIrs;
using Services.Services.SmsStrategy;
using Services.Services.SmsStrategy.SmsServices;
using WebFramework.Configuration;
using WebFramework.CustomMapping;
using WebFramework.Middlewares;
using WebFramework.Swagger;
using Services.Services.KavenegarServices;
using Microsoft.AspNetCore.HttpOverrides;
using Services.Services.IpServices;
using Services.JobsServices;
using WebFramework.Filters;
using Microsoft.Extensions.Options;

var configuration = GetConfiguration();
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddConfiguration(configuration);
builder.Host.UseContentRoot(Directory.GetCurrentDirectory());
builder.Host.UseSerilog(SerilogConfig.ConfigureLogger);

PointsConfiguration.Initialize(builder.Configuration);

builder.Services.AddSignalR(e =>
{
    e.MaximumReceiveMessageSize = 102400000;
    e.EnableDetailedErrors = true;
});
builder.Services.Configure<SiteSettings>(configuration.GetSection(nameof(SiteSettings)));
var _siteSetting = configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();
builder.Services.Configure<KavenegarOptions>(
    builder.Configuration.GetSection("SiteSettings:Kavenegar"));
builder.Services.Configure<SmsSettings>(builder.Configuration.GetSection("SiteSettings:Sms"));
builder.Services.AddKeyedTransient<ISmsProvider, KavenegarSmsProvider>("kavenegar");
builder.Services.AddKeyedTransient<ISmsProvider, SmsIrProvider>("smsir");
builder.Services.AddTransient<Func<string, ISmsProvider>>(sp => key =>
{
    return sp.GetRequiredKeyedService<ISmsProvider>(key);
});
builder.Services.AddHttpClient("kavenegar", client =>
{
    client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
});

builder.Services.AddTransient<KavenegarHttpClient>();
builder.Services.AddTransient<KavenegarHelpers>();
builder.Services.AddTransient<IKavenegarService,KavenegarService>();
builder.Services.AddTransient<ISmsService, SmsService>();
builder.Services.InitializeAutoMapper();

builder.Services.AddDbContext(configuration);
builder.Services.AddCustomIdentity(_siteSetting.IdentitySettings);
builder.Services.AddMinimalMvc();
// اضافه کردن configuration برای salt
builder.Services.Configure<IpServiceSettings>(builder.Configuration.GetSection("IpServiceSettings"));


// اضافه کردن سرویس Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.Name = "PadyLife.Session";
});
builder.Services.AddCustomAuthentication(_siteSetting.JwtSettings, _siteSetting.GoogleSettings);
builder.Services.AddServices();
builder.Services.AddCustomApiVersioning();
builder.Services.AddIdGen(0);
// builder.Services.AddStackExchangeRedisCache(options => options.Configuration = configuration.GetConnectionString("Redis"));
builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSwagger();
builder.Services.AddZarinPalPaymentGateway(configuration);

builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options =>
    {
        options.UseNpgsqlConnection(configuration.GetConnectionString("PostgreSQL"));
    }, new PostgreSqlStorageOptions
    {
        QueuePollInterval = TimeSpan.FromSeconds(1),
        InvisibilityTimeout = TimeSpan.FromMinutes(5),
        PrepareSchemaIfNecessary = true
    }));

builder.Services.AddHangfireServer();

// ثبت Job ها
builder.Services.AddScoped<CheckUserPlansJob>();
//builder.Services.AddScoped<ProcessDailyStatsJob>();
builder.Services.AddScoped<ProcessPer10MINStatsJob>();
builder.Services.AddScoped<CleanupOldVisitDataJob>();

builder.Services.AddCqrs();
builder.Services.AddCors(options =>
{

    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod() // شامل PUT می‌شود
               .AllowAnyHeader();

    });
});

//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();


app.IntializeDatabase();

app.UseCustomExceptionHandler();
app.UseHsts(app.Environment);

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = r =>
    {
        string path = r?.File?.PhysicalPath;
        if ((path is not null) && (path.EndsWith(".gif") || path.EndsWith(".jpg") || path.EndsWith(".jpeg") || path.EndsWith(".png") || path.EndsWith(".svg") || path.EndsWith(".webp")))
        {
            TimeSpan maxAge = new(365, 0, 0, 0);
            r.Context.Response.Headers.Append("Cache-Control", "max-age=" + maxAge.TotalSeconds.ToString("0"));
        }
    }
});

app.UseSwaggerAndUI();
app.UseRouting();

// و در پایین:
app.UseCors("AllowAll");
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
    ForwardLimit = 2
});

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});

// ثبت Jobهای Hangfire
using (var scope = app.Services.CreateScope())
{
    var jobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    WebFramework.Configuration.HangfireJobsConfig.RegisterRecurringJobs(jobManager);
}

//// اجرای اولیه Jobها (اختیاری)
//using (var scope = app.Services.CreateScope())
//{
//    var job = scope.ServiceProvider.GetRequiredService<CheckUserPlansJob>();
//    await job.RunAsync(CancellationToken.None);
//}

//// اجرای اولیه Job آمار روزانه (اختیاری)
//using (var scope = app.Services.CreateScope())
//{
//    var job = scope.ServiceProvider.GetRequiredService<ProcessDailyStatsJob>();
//    await job.RunAsync(CancellationToken.None);
//}

app.UseEndpoints(config =>
{

});
app.Map("/", () =>
{
    return Results.Redirect("/swagger/index.html");
});
app.MapControllers();
app.MapHub<NotifyHub>("/hub/NotifyHub")
    .RequireCors(builder =>
       builder.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
app.MapHub<ChatHub>("/hub/ChatHub")
   .RequireCors(builder =>
       builder.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());

app.MapHub<OnlineUsersHub>("/onlineUsersHub")
    .RequireCors(builder =>
       builder.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
app.Run();


public partial class Program
{
    static IConfiguration GetConfiguration()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (string.IsNullOrWhiteSpace(env))
        {
            env = Environment.GetEnvironmentVariable("RAILWAY_ENVIRONMENT_NAME");
        }
        if (string.IsNullOrWhiteSpace(env))
        {
            throw new Exception("Deployment environment is not set. Expected ASPNETCORE_ENVIRONMENT or RAILWAY_ENVIRONMENT_NAME.");
        }

        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory) // always points to DLL folder
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env}.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        return builder.Build();
    }
}


