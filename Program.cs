using TradeManagementAPI;
using DataLayer.Abstract.DataBaseUpdater.Migrator;
using DataLayer.DataBaseUpdater.DataContext;

var host = CreateHostBuilder(args);

await host.MigrateDatabase<PostgreDbContext>("DB_CONNECTION_STR");

await host.RunConsoleAsync();

static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((ctx, conf) =>
    {
        conf.AddJsonFile("appsettings.json", false);
    })
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.ConfigureKestrel(opt =>
        {
            opt.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(180);
            opt.Limits.MaxResponseBufferSize = 20 * 1024 * 1024;
        });


        webBuilder.UseStartup<Startup>();
    });