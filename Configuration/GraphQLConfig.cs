using System.Security.Claims;
using TradeManagementAPI.GraphQl;

namespace TradeManagementAPI.Configuration
{
    public static class GraphQLConfig
    {
        public static IServiceCollection AddGraphQLServices(this IServiceCollection services)
        {
            services.AddGraphQLServer()
                .ModifyOptions(x => x.EnableTag = false)
                .AddQueryType<Query>()
                .AddType(new UuidType(defaultFormat: 'D'))
                .ModifyPagingOptions(x =>
                {
                    x.DefaultPageSize = 100;
                    x.MaxPageSize = 1000;
                    x.IncludeTotalCount = true;
                })
                .AddProjections()
                .AddFiltering()
                .AddSorting()
                .AddInstrumentation()
                .InitializeOnStartup();

            return services;
        }
    }
}
