using System.Security.Claims;

namespace Infrastructure.Extensions
{
    public static class AuthorizationExtensions
    {
        public static bool IsSystemAdmin(this ClaimsPrincipal user)
        {
            return user.IsInRole("SystemAdmin");
        }

        public static bool TryGetUserStore(this ClaimsPrincipal user, out int userStoreNum)
        {
            var userStore = user.FindFirstValue("StoreNumber");

            return int.TryParse(userStore, out userStoreNum);
        }

        public static bool TryGetDepartmentHead(this ClaimsPrincipal user, out int userStoreNum, out string departmentName)
        {
            var userStore = user.FindFirstValue("StoreNumber");
            var userDept = user.FindFirstValue("DepartmentName");
            departmentName = string.Empty;

            if (int.TryParse(userStore, out userStoreNum) && !string.IsNullOrEmpty(userDept))
            {
                departmentName = userDept;

                return true;
            }

            return false;
        }

        public static bool TryGetWarehouse(this ClaimsPrincipal user, out string warehouseName)
        {
            var userWarehouse = user.FindFirstValue("WarehouseName");
            warehouseName = string.Empty;

            if (!string.IsNullOrEmpty(userWarehouse))
            {
                warehouseName = userWarehouse;

                return true;
            }

            return false;
        }
    }
}
