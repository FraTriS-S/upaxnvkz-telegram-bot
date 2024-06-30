using System.Diagnostics;
using Serilog;
using Upax;
using Upax.Options;

try
{
    Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
    Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("start-log.txt")
        .CreateBootstrapLogger();
    Log.Information($"Let`s start {Process.GetCurrentProcess().ProcessName}...");

    var host = Host.CreateDefaultBuilder()
        .ConfigureServices((context, services) =>
        {
            services.AddHostedService<TelegramBotBackgroundService>();
            services.Configure<TelegramOptions>(context.Configuration.GetSection(TelegramOptions.Telegram));
        })
        .UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration))
        .UseWindowsService()
        .Build();
    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.CloseAndFlush();
}