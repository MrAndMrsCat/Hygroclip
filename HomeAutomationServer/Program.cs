using HomeAutomationServer.Data;
using HomeAutomationServer.Devices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog.Events;
using Serilog.Filters;
using Serilog;
using System.Configuration;

namespace HomeAutomationServer;
public class Program
{
    public static bool Simulated { get; private set; }
    public static SystemDevicesModel HardwareModel { get; private set; } = new();

    public static void Main(string[] args)
    {
        Serilog.Log.Logger = new LoggerConfiguration()
           .MinimumLevel.ControlledBy(new(LogEventLevel.Debug))
           //.WriteTo.Async(a => a.File(LogFilePath, fileSizeLimitBytes: 10000000, retainedFileCountLimit: 10))
           .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Debug, theme: Serilog.Sinks.SystemConsole.Themes.SystemConsoleTheme.Literate)
           //.Filter.ByIncludingOnly(Matching.FromSource<Log>())
           .CreateLogger();

        var argAndParms = CommandLineParser.GetCommandLineArgs(args);

        AppConfiguration.Load();

        Simulated = argAndParms.ContainsKey("simulated");

        HardwareModel.Initialize(Simulated);

        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        string connectionStringName = Simulated
            ? "EnvironmentalMeasurementDBSim"
            : "EnvironmentalMeasurementDB";

        string connectionString = builder.Configuration.GetConnectionString(connectionStringName);

        if (argAndParms.TryGetValue("sqladdress", out string[]? sqladdress))
        {
            connectionString = connectionString.Replace("localhost", sqladdress.First());
        }

        if (Simulated)
        {
            builder.Services.AddDbContext<EnvironmentalMeasurementContext>(options => options.UseInMemoryDatabase("EnvironmentalMeasurement"));
        }
        else
        {
            builder.Services.AddDbContext<EnvironmentalMeasurementContext>(options => options.UseSqlServer(connectionString));
        }

        builder.Services.AddSingleton<EnvironmentalMeasurementService>();

        var app = builder.Build();

        app.Logger.LogInformation($"Application entry with arguments");
        app.Logger.LogInformation(string.Join(' ', args));

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }
        else
        {
            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();
        }

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<EnvironmentalMeasurementContext>();
            //context.Database.EnsureCreated();
            app.Logger.LogInformation($"Attempt to initialize database, with connection string:\n{connectionString}");
            DbInitializer.Initialize(context);

            var measService = services.GetRequiredService<EnvironmentalMeasurementService>();
            var _ = measService.GetMeasurementsAsync(DateTime.Now); // instantiate now
        }

        app.UseStaticFiles();

        app.UseRouting();
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        app.Run();
    }
}