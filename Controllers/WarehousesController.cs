using DataLayer.DataAccess.Context;
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
    public class WarehousesController : ControllerBase
    {
        private readonly PostgreDbContext _context;

        public WarehousesController(PostgreDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ObjectResultDto<IEnumerable<Warehouse>>>> GetAll(CancellationToken ct = default)
        {
            var query = _context.Warehouses
                .Include(x => x.WarehouseProducts)
                .Include(x => x.Stores);

            if (User.IsSystemAdmin())
                return ObjectResultDto<IEnumerable<Warehouse>>.Ok(await query.ToArrayAsync(ct));

            if (!User.TryGetWarehouse(out var warehouseName))
                return ObjectResultDto<IEnumerable<Warehouse>>.Error("No access");

            return ObjectResultDto<IEnumerable<Warehouse>>.Ok(await query.Where(x => x.Name == warehouseName).ToArrayAsync(ct));
        }

        // Создание торговой базы
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ObjectResultDto<Warehouse>>> CreateWarehouse(Warehouse warehouse, CancellationToken ct = default)
        {
            try
            {
                _context.Warehouses.Add(warehouse);
                await _context.SaveChangesAsync(ct);

                return ObjectResultDto<Warehouse>.Ok(warehouse, "Warehouse created successfully");
            }
            catch (Exception ex)
            {
                return ObjectResultDto<Warehouse>.Error($"Error creating warehouse: {ex.Message}");
            }
        }

        // Обновление торговой базы
        [HttpPut("{name}")]
        [Authorize]
        public async Task<ActionResult<ObjectResultDto<Warehouse>>> UpdateWarehouse(string name, Warehouse updatedWarehouse, CancellationToken ct = default)
        {
            try
            {
                if (name != updatedWarehouse.Name)
                    return ObjectResultDto<Warehouse>.Error("Warehouse name mismatch");

                _context.Entry(updatedWarehouse).State = EntityState.Modified;
                await _context.SaveChangesAsync(ct);

                return ObjectResultDto<Warehouse>.Ok(updatedWarehouse, "Warehouse updated successfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WarehouseExists(name))
                    return ObjectResultDto<Warehouse>.Error("Warehouse not found");
                throw;
            }
            catch (Exception ex)
            {
                return ObjectResultDto<Warehouse>.Error($"Error updating warehouse: {ex.Message}");
            }
        }

        // Удаление торговой базы
        [HttpDelete("{name}")]
        [Authorize]
        public async Task<ActionResult<ObjectResultDto<bool>>> DeleteWarehouse(string name, CancellationToken ct = default)
        {
            try
            {
                var warehouse = await _context.Warehouses.FindAsync(name);
                if (warehouse == null)
                    return ObjectResultDto<bool>.Error("Warehouse not found", false);

                _context.Warehouses.Remove(warehouse);
                await _context.SaveChangesAsync(ct);

                return ObjectResultDto<bool>.Ok(true, "Warehouse deleted successfully");
            }
            catch (Exception ex)
            {
                return ObjectResultDto<bool>.Error($"Error deleting warehouse: {ex.Message}", false);
            }
        }

        // Добавление товара на базу
        [HttpPost("{name}/products")]
        [Authorize]
        public async Task<ActionResult<ObjectResultDto<WarehouseProduct>>> AddProductToWarehouse(string name, WarehouseProduct warehouseProduct, CancellationToken ct = default)
        {
            try
            {
                if (name != warehouseProduct.WarehouseName)
                    return ObjectResultDto<WarehouseProduct>.Error("Warehouse name mismatch");

                // Проверка существования товара
                if (!await _context.Products.AnyAsync(p =>
                    p.Name == warehouseProduct.ProductName &&
                    p.Grade == warehouseProduct.ProductGrade, ct))
                    return ObjectResultDto<WarehouseProduct>.Error("Product not found");

                _context.WarehouseProducts.Add(warehouseProduct);
                await _context.SaveChangesAsync(ct);

                return ObjectResultDto<WarehouseProduct>.Ok(
                    warehouseProduct,
                    "Product added to warehouse successfully");
            }
            catch (Exception ex)
            {
                return ObjectResultDto<WarehouseProduct>.Error(
                    $"Error adding product to warehouse: {ex.Message}");
            }
        }

        // Обновление товара на базе
        [HttpPut("{name}/products/{productName}/{grade}")]
        [Authorize]
        public async Task<ActionResult<ObjectResultDto<WarehouseProduct>>> UpdateWarehouseProduct(
            string name,
            string productName,
            string grade,
            WarehouseProduct updatedProduct, 
            CancellationToken ct = default)
        {
            try
            {
                if (name != updatedProduct.WarehouseName ||
                    productName != updatedProduct.ProductName ||
                    grade != updatedProduct.ProductGrade)
                    return ObjectResultDto<WarehouseProduct>.Error("Identifier mismatch");

                _context.Entry(updatedProduct).State = EntityState.Modified;
                await _context.SaveChangesAsync(ct);

                return ObjectResultDto<WarehouseProduct>.Ok(
                    updatedProduct,
                    "Warehouse product updated successfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WarehouseProductExists(name, productName, grade))
                    return ObjectResultDto<WarehouseProduct>.Error("Warehouse product not found");
                throw;
            }
            catch (Exception ex)
            {
                return ObjectResultDto<WarehouseProduct>.Error(
                    $"Error updating warehouse product: {ex.Message}");
            }
        }

        // Удаление товара с базы
        [HttpDelete("{name}/products/{productName}/{grade}")]
        [Authorize]
        public async Task<ActionResult<ObjectResultDto<bool>>> RemoveProductFromWarehouse(
            string name,
            string productName,
            string grade,
            CancellationToken ct = default)
        {
            try
            {
                var warehouseProduct = await _context.WarehouseProducts.FindAsync(name, productName, grade);
                if (warehouseProduct == null)
                    return ObjectResultDto<bool>.Error("Warehouse product not found", false);

                _context.WarehouseProducts.Remove(warehouseProduct);
                await _context.SaveChangesAsync(ct);

                return ObjectResultDto<bool>.Ok(true, "Product removed from warehouse successfully");
            }
            catch (Exception ex)
            {
                return ObjectResultDto<bool>.Error(
                    $"Error removing product from warehouse: {ex.Message}",
                    false);
            }
        }

        private bool WarehouseExists(string name) =>
            _context.Warehouses.Any(e => e.Name == name);

        private bool WarehouseProductExists(string warehouseName, string productName, string grade) =>
            _context.WarehouseProducts.Any(e =>
                e.WarehouseName == warehouseName &&
                e.ProductName == productName &&
                e.ProductGrade == grade);
    }
}
