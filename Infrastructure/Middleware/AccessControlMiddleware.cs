using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Middleware
{
    public class AccessControlMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AccessControlMiddleware> _logger;
        private readonly string[] _includeController =
        {
            "api/products",
            "api/stores",
            "api/users",
            "api/warehouses"
        };

        public AccessControlMiddleware(RequestDelegate next, ILogger<AccessControlMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Пропускаем запросы, не требующие авторизации
            if (context.GetEndpoint()?.Metadata.GetMetadata<AllowAnonymousAttribute>() != null)
            {
                await _next(context);
                return;
            }

            var path = context.Request.Path.Value ?? "";
            path = path.ToLower();

            if (!_includeController.Any(x => path.EndsWith(x)))
            {
                await _next(context);
                return;
            }

            // Проверяем аутентификацию
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            // Проверяем права доступа
            if (!HasAccess(context))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden: You don't have permission to access this resource");
                return;
            }

            await _next(context);
        }

        private bool HasAccess(HttpContext context)
        {
            var user = context.User;
            var path = context.Request.Path.Value ?? "";
            var method = context.Request.Method;

            // Системный администратор имеет полный доступ
            if (user.IsInRole("SystemAdmin"))
                return true;

            // Проверка для магазинов
            if (path.StartsWith("/api/stores", StringComparison.OrdinalIgnoreCase))
                return CheckStoreAccess(user, path, method);

            // Проверка для товаров
            if (path.StartsWith("/api/products", StringComparison.OrdinalIgnoreCase))
                return CheckProductAccess(user, path, method);

            // Проверка для торговых баз
            if (path.StartsWith("/api/warehouses", StringComparison.OrdinalIgnoreCase))
                return CheckWarehouseAccess(user, path, method);

            // Проверка для пользователей
            if (path.StartsWith("/api/users", StringComparison.OrdinalIgnoreCase))
                return user.IsInRole("SystemAdmin");

            // Разрешаем доступ по умолчанию для других путей
            return true;
        }

        private bool CheckStoreAccess(ClaimsPrincipal user, string path, string method)
        {
            // Извлекаем номер магазина из пути
            int? storeNumber = null;
            if (path.Split('/').Length > 3 && int.TryParse(path.Split('/')[3], out int storeNum))
                storeNumber = storeNum;

            // Проверка операций с магазином
            switch (method)
            {
                case "POST": // Создание магазина
                    return user.IsInRole("SystemAdmin");

                case "PUT": // Обновление магазина
                case "DELETE": // Удаление магазина
                    return user.IsInRole("SystemAdmin") ||
                           (user.IsInRole("StoreAdmin") && user.HasStoreAccess(storeNumber));

                default: // GET и другие операции
                    return true;
            }
        }

        private bool CheckProductAccess(ClaimsPrincipal user, string path, string method)
        {
            // Проверка операций с товарами
            switch (method)
            {
                case "POST": // Создание товара
                    if(path.EndsWith("/prices")) // Создание прайс-правила
                        return user.IsInRole("SystemAdmin") || user.IsInRole("StoreAdmin");
                    return user.IsInRole("SystemAdmin");

                case "PUT": // Обновление товара
                case "DELETE": // Удаление товара
                    return user.IsInRole("SystemAdmin");

                default: // GET и другие операции
                    return true;
            }
        }

        private bool CheckWarehouseAccess(ClaimsPrincipal user, string path, string method)
        {
            // Извлекаем название базы из пути
            string? warehouseName = null;
            if (path.Split('/').Length > 3)
                warehouseName = path.Split('/')[3];

            // Проверка операций с торговыми базами
            switch (method)
            {
                case "POST": // Создание базы
                    return user.IsInRole("SystemAdmin");

                case "PUT": // Обновление базы
                case "DELETE": // Удаление базы
                    return user.IsInRole("SystemAdmin") ||
                           (user.IsInRole("WarehouseAdmin") && user.HasWarehouseAccess(warehouseName));

                default: // GET и другие операции
                    return true;
            }
        }
    }

    public static class ClaimsPrincipalExtensions
    {
        public static bool HasStoreAccess(this ClaimsPrincipal user, int? storeNumber)
        {
            if (!storeNumber.HasValue) return false;

            var userStore = user.FindFirstValue("StoreNumber");
            return int.TryParse(userStore, out int userStoreNum) && userStoreNum == storeNumber.Value;
        }

        public static bool HasWarehouseAccess(this ClaimsPrincipal user, string? warehouseName)
        {
            if (string.IsNullOrEmpty(warehouseName)) return false;

            var userWarehouse = user.FindFirstValue("WarehouseName");
            return userWarehouse == warehouseName;
        }
    }
}
