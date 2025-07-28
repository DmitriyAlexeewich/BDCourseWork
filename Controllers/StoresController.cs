using DataLayer.DataBaseUpdater.DataContext;
using DataLayer.Models.Models;
using DataLayer.Models.Models.Products;
using Dto.Abstract.Result;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TradeManagementAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class StoresController : ControllerBase
    {
        private readonly PostgreDbContext _context;

        public StoresController(PostgreDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ObjectResultDto<IEnumerable<Store>>>> GetAll(CancellationToken ct = default)
        {
            var query = _context.Stores
                .Include(x => x.Warehouse)
                .Include(x => x.Departments)
                .ThenInclude(x => x.DepartmentProducts);

            if (User.IsSystemAdmin())
                return ObjectResultDto<IEnumerable<Store>>.Ok(await query.ToArrayAsync(ct));

            if (!User.TryGetUserStore(out var userStore))
                return ObjectResultDto<IEnumerable<Store>>.Error("No access");

            return ObjectResultDto<IEnumerable<Store>>.Ok(await query.Where(x => x.Number == userStore).ToArrayAsync(ct));
        }

        // Создание магазина
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ObjectResultDto<Store>>> CreateStore(Store store, CancellationToken ct = default)
        {
            try
            {
                // Проверка существования торговой базы
                if (!await _context.Warehouses.AnyAsync(w => w.Name == store.WarehouseName, ct))
                    return ObjectResultDto<Store>.Error("Trade base not found");

                _context.Stores.Add(store);
                await _context.SaveChangesAsync(ct);

                return ObjectResultDto<Store>.Ok(store, "Store created successfully");
            }
            catch (Exception ex)
            {
                return ObjectResultDto<Store>.Error($"Error creating store: {ex.Message}");
            }
        }

        // Обновление магазина
        [HttpPut("{number}")]
        [Authorize]
        public async Task<ActionResult<ObjectResultDto<Store>>> UpdateStore(int number, Store updatedStore, CancellationToken ct = default)
        {
            try
            {
                if (number != updatedStore.Number)
                    return ObjectResultDto<Store>.Error("Store number mismatch");

                // Проверка существования торговой базы
                if (!await _context.Warehouses.AnyAsync(w => w.Name == updatedStore.WarehouseName, ct))
                    return ObjectResultDto<Store>.Error("Trade base not found");

                _context.Entry(updatedStore).State = EntityState.Modified;
                await _context.SaveChangesAsync(ct);

                return ObjectResultDto<Store>.Ok(updatedStore, "Store updated successfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StoreExists(number))
                    return ObjectResultDto<Store>.Error("Store not found");
                throw;
            }
            catch (Exception ex)
            {
                return ObjectResultDto<Store>.Error($"Error updating store: {ex.Message}");
            }
        }

        // Удаление магазина
        [HttpDelete("{number}")]
        [Authorize]
        public async Task<ActionResult<ObjectResultDto<bool>>> DeleteStore(int number, CancellationToken ct = default)
        {
            try
            {
                var store = await _context.Stores.FindAsync(number);
                if (store == null)
                    return ObjectResultDto<bool>.Error("Store not found", false);

                _context.Stores.Remove(store);
                await _context.SaveChangesAsync(ct);

                return ObjectResultDto<bool>.Ok(true, "Store deleted successfully");
            }
            catch (Exception ex)
            {
                return ObjectResultDto<bool>.Error($"Error deleting store: {ex.Message}", false);
            }
        }

        // Создание отдела
        [HttpPost("{storeNumber}/departments")]
        [Authorize]
        public async Task<ActionResult<ObjectResultDto<Department>>> CreateDepartment(int storeNumber, Department department, CancellationToken ct = default)
        {
            try
            {
                if (!StoreExists(storeNumber))
                    return ObjectResultDto<Department>.Error("Store not found");

                if (department.StoreNumber != storeNumber)
                    return ObjectResultDto<Department>.Error("Store number mismatch");

                _context.Departments.Add(department);
                await _context.SaveChangesAsync(ct);

                return ObjectResultDto<Department>.Ok(department, "Department created successfully");
            }
            catch (Exception ex)
            {
                return ObjectResultDto<Department>.Error($"Error creating department: {ex.Message}");
            }
        }

        // Обновление отдела
        [HttpPut("{storeNumber}/departments/{departmentName}")]
        [Authorize]
        public async Task<ActionResult<ObjectResultDto<Department>>> UpdateDepartment(int storeNumber, string departmentName, Department updatedDepartment, CancellationToken ct = default)
        {
            try
            {
                if (storeNumber != updatedDepartment.StoreNumber || departmentName != updatedDepartment.Name)
                    return ObjectResultDto<Department>.Error("Identifier mismatch");

                _context.Entry(updatedDepartment).State = EntityState.Modified;
                await _context.SaveChangesAsync(ct);

                return ObjectResultDto<Department>.Ok(updatedDepartment, "Department updated successfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(storeNumber, departmentName))
                    return ObjectResultDto<Department>.Error("Department not found");
                throw;
            }
            catch (Exception ex)
            {
                return ObjectResultDto<Department>.Error($"Error updating department: {ex.Message}");
            }
        }

        // Удаление отдела
        [HttpDelete("{storeNumber}/departments/{departmentName}")]
        [Authorize]
        public async Task<ActionResult<ObjectResultDto<bool>>> DeleteDepartment(int storeNumber, string departmentName, CancellationToken ct = default)
        {
            try
            {
                var department = await _context.Departments.FindAsync(storeNumber, departmentName);
                if (department == null)
                    return ObjectResultDto<bool>.Error("Department not found", false);

                _context.Departments.Remove(department);
                await _context.SaveChangesAsync(ct);

                return ObjectResultDto<bool>.Ok(true, "Department deleted successfully");
            }
            catch (Exception ex)
            {
                return ObjectResultDto<bool>.Error($"Error deleting department: {ex.Message}", false);
            }
        }

        // Добавление товара в отдел
        [HttpPost("{storeNumber}/departments/{departmentName}/products")]
        [Authorize]
        public async Task<ActionResult<ObjectResultDto<DepartmentProduct>>> AddProductToDepartment(int storeNumber, 
            string departmentName, 
            DepartmentProduct departmentProduct, 
            CancellationToken ct = default)
        {
            try
            {
                if (!DepartmentExists(storeNumber, departmentName))
                    return ObjectResultDto<DepartmentProduct>.Error("Department not found");

                if (departmentProduct.StoreNumber != storeNumber ||
                    departmentProduct.DepartmentName != departmentName)
                    return ObjectResultDto<DepartmentProduct>.Error("Department identifier mismatch");

                // Проверка существования товара
                if (!await _context.Products.AnyAsync(p =>
                    p.Name == departmentProduct.ProductName &&
                    p.Grade == departmentProduct.ProductGrade, ct))
                    return ObjectResultDto<DepartmentProduct>.Error("Product not found");

                _context.DepartmentProducts.Add(departmentProduct);
                await _context.SaveChangesAsync(ct);

                return ObjectResultDto<DepartmentProduct>.Ok(
                    departmentProduct,
                    "Product added to department successfully");
            }
            catch (Exception ex)
            {
                return ObjectResultDto<DepartmentProduct>.Error(
                    $"Error adding product to department: {ex.Message}");
            }
        }

        // Обновление товара в отделе
        [HttpPut("{storeNumber}/departments/{departmentName}/products/{productName}/{grade}")]
        [Authorize]
        public async Task<ActionResult<ObjectResultDto<DepartmentProduct>>> UpdateDepartmentProduct(
            int storeNumber,
            string departmentName,
            string productName,
            string grade,
            DepartmentProduct updatedProduct,
            CancellationToken ct = default)
        {
            try
            {
                if (storeNumber != updatedProduct.StoreNumber ||
                    departmentName != updatedProduct.DepartmentName ||
                    productName != updatedProduct.ProductName ||
                    grade != updatedProduct.ProductGrade)
                    return ObjectResultDto<DepartmentProduct>.Error("Identifier mismatch");

                _context.Entry(updatedProduct).State = EntityState.Modified;
                await _context.SaveChangesAsync(ct);

                return ObjectResultDto<DepartmentProduct>.Ok(
                    updatedProduct,
                    "Department product updated successfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentProductExists(storeNumber, departmentName, productName, grade))
                    return ObjectResultDto<DepartmentProduct>.Error("Department product not found");
                throw;
            }
            catch (Exception ex)
            {
                return ObjectResultDto<DepartmentProduct>.Error(
                    $"Error updating department product: {ex.Message}");
            }
        }

        // Удаление товара из отдела
        [HttpDelete("{storeNumber}/departments/{departmentName}/products/{productName}/{grade}")]
        [Authorize]
        public async Task<ActionResult<ObjectResultDto<bool>>> RemoveProductFromDepartment(
            int storeNumber,
            string departmentName,
            string productName,
            string grade,
            CancellationToken ct = default)
        {
            try
            {
                var departmentProduct = await _context.DepartmentProducts.FindAsync(
                    storeNumber, departmentName, productName, grade);

                if (departmentProduct == null)
                    return ObjectResultDto<bool>.Error("Department product not found", false);

                _context.DepartmentProducts.Remove(departmentProduct);
                await _context.SaveChangesAsync(ct);

                return ObjectResultDto<bool>.Ok(true, "Product removed from department successfully");
            }
            catch (Exception ex)
            {
                return ObjectResultDto<bool>.Error(
                    $"Error removing product from department: {ex.Message}",
                    false);
            }
        }

        private bool StoreExists(int number) =>
            _context.Stores.Any(e => e.Number == number);

        private bool DepartmentExists(int storeNumber, string name) =>
            _context.Departments.Any(e => e.StoreNumber == storeNumber && e.Name == name);

        private bool DepartmentProductExists(
            int storeNumber, string departmentName, string productName, string grade) =>
            _context.DepartmentProducts.Any(e =>
                e.StoreNumber == storeNumber &&
                e.DepartmentName == departmentName &&
                e.ProductName == productName &&
                e.ProductGrade == grade);
    }
}
