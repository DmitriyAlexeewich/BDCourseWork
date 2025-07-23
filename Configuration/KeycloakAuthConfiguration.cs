using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Keycloak.AuthServices.Sdk;

namespace TradeManagementAPI.Configuration
{
    public static class KeycloakAuthConfiguration
    {
        public static IServiceCollection AddKeycloakServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddKeycloakWebApiAuthentication(configuration);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireTradeAdmin", policy => policy.RequireClaim("client_roles", "trade-admin"));

                options.AddPolicy("RequireStoreManager", policy => policy.RequireClaim("client_roles", ["store_admin", "trade-admin"]));

                options.AddPolicy("RequireWarehouseManager", policy => policy.RequireClaim("client_roles", ["warehouse_admin", "trade-admin"]));

                options.AddPolicy("RequireReportViewer", policy => policy.RequireClaim("client_roles", ["staff_user", "trade-admin"]));
            });

            services.AddKeycloakAuthorization(configuration);
            services.AddKeycloakAdminHttpClient(configuration);

            return services;
        }
    }
}
