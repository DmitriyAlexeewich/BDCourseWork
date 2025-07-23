using DataLayer.DataBaseUpdater.DataContext;
using Microsoft.EntityFrameworkCore;

namespace TradeManagementAPI.Configuration
{
    public static class DbConfiguration
    {
        public static IServiceCollection AddDbConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConfiguration(configuration.GetSection("Logging")));
            var connectionStr = Environment.GetEnvironmentVariable("DB_CONNECTION_STR");

            services.AddDbContext<PostgreDbContext>(options =>
            {
                options.UseNpgsql(connectionStr);
            });

            services.AddScoped<PostgreDbContext>();

            return services;
        }
    }
}
