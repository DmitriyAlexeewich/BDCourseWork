using Infrastructure.Middleware;
using TradeManagementAPI.Configuration;

namespace TradeManagementAPI
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwagger();

            services.AddLogging(logging =>
            {
                logging.AddConfiguration(_configuration.GetSection("Logging"));
            });

            services.AddAuth(_configuration);

            //services.AddApplicationServices();

            //services.AddKeycloakServices(_configuration);

            services.AddControllers();

            services.AddDbConfiguration(_configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<AccessControlMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
