using DataLayer.DataBaseUpdater.DataContext;
using DataLayer.Models.Models.Products;
using DataLayer.Models.Rules;
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
    public class ProductsController : ControllerBase
    {
        private readonly PostgreDbContext _context;

        public ProductsController(PostgreDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ObjectResultDto<IEnumerable<Product>>>> GetAll(CancellationToken ct = default)
        {
            if (!User.IsSystemAdmin())
                return ObjectResultDto<IEnumerable<Product>>.Error("No access");

            var adminProducts = await _context.Products
                .Include(x => x.WarehouseProducts)
                .Include(x => x.DepartmentProducts)
                .ToArrayAsync(ct);

            return ObjectResultDto<IEnumerable<Product>>.Ok(adminProducts);
        }

        // Создание товара
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ObjectResultDto<Product>>> CreateProduct(Product product, CancellationToken ct = default)
        {
            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync(ct);

                return ObjectResultDto<Product>.Ok(product, "Product created successfully");
            }
            catch (Exception ex)
            {
                return ObjectResultDto<Product>.Error($"Error creating product: {ex.Message}");
            }
        }

        // Обновление товара
        [HttpPut("{name}/{grade}")]
        [Authorize]
        public async Task<ActionResult<ObjectResultDto<Product>>> UpdateProduct(string name, string grade, Product updatedProduct, CancellationToken ct = default)
        {
            try
            {
                if (name != updatedProduct.Name || grade != updatedProduct.Grade)
                    return ObjectResultDto<Product>.Error("Product identifier mismatch");

                _context.Entry(updatedProduct).State = EntityState.Modified;
                await _context.SaveChangesAsync(ct);

                return ObjectResultDto<Product>.Ok(updatedProduct, "Product updated successfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(name, grade))
                    return ObjectResultDto<Product>.Error("Product not found");
                throw;
            }
            catch (Exception ex)
            {
                return ObjectResultDto<Product>.Error($"Error updating product: {ex.Message}");
            }
        }

        // Удаление товара
        [HttpDelete("{name}/{grade}")]
        [Authorize]
        public async Task<ActionResult<ObjectResultDto<bool>>> DeleteProduct(string name, string grade, CancellationToken ct = default)
        {
            try
            {
                var product = await _context.Products.FindAsync(name, grade);
                if (product == null)
                    return ObjectResultDto<bool>.Error("Product not found", false);

                _context.Products.Remove(product);
                await _context.SaveChangesAsync(ct);

                return ObjectResultDto<bool>.Ok(true, "Product deleted successfully");
            }
            catch (Exception ex)
            {
                return ObjectResultDto<bool>.Error($"Error deleting product: {ex.Message}", false);
            }
        }

        // Создание прайс-правила
        [HttpPost("prices")]
        [Authorize]
        public async Task<ActionResult<ObjectResultDto<PriceRule>>> CreatePriceRule(PriceRule priceRule, CancellationToken ct = default)
        {
            try
            {
                _context.PriceRules.Add(priceRule);
                await _context.SaveChangesAsync(ct);

                return ObjectResultDto<PriceRule>.Ok(priceRule, "Price rule created successfully");
            }
            catch (Exception ex)
            {
                return ObjectResultDto<PriceRule>.Error($"Error creating price rule: {ex.Message}");
            }
        }

        // Обновление прайс-правила
        [HttpPut("prices")]
        [Authorize]
        public async Task<ActionResult<ObjectResultDto<PriceRule>>> UpdatePriceRule(
            [FromQuery] string storeClass,
            [FromQuery] string productGrade,
            [FromQuery] DateTime startDate,
            PriceRule updatedRule,
            CancellationToken ct = default)
        {
            try
            {
                if (storeClass != updatedRule.StoreClass ||
                    productGrade != updatedRule.ProductGrade ||
                    startDate != updatedRule.StartDate)
                    return ObjectResultDto<PriceRule>.Error("Price rule identifier mismatch");

                _context.Entry(updatedRule).State = EntityState.Modified;
                await _context.SaveChangesAsync(ct);

                return ObjectResultDto<PriceRule>.Ok(updatedRule, "Price rule updated successfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PriceRuleExists(storeClass, productGrade, startDate))
                    return ObjectResultDto<PriceRule>.Error("Price rule not found");
                throw;
            }
            catch (Exception ex)
            {
                return ObjectResultDto<PriceRule>.Error($"Error updating price rule: {ex.Message}");
            }
        }

        // Удаление прайс-правила
        [HttpDelete("prices")]
        [Authorize]
        public async Task<ActionResult<ObjectResultDto<bool>>> DeletePriceRule(
            [FromQuery] string storeClass,
            [FromQuery] string productGrade,
            [FromQuery] DateTime startDate,
            CancellationToken ct = default)
        {
            try
            {
                var priceRule = await _context.PriceRules.FindAsync(storeClass, productGrade, startDate);
                if (priceRule == null)
                    return ObjectResultDto<bool>.Error("Price rule not found", false);

                _context.PriceRules.Remove(priceRule);
                await _context.SaveChangesAsync(ct);

                return ObjectResultDto<bool>.Ok(true, "Price rule deleted successfully");
            }
            catch (Exception ex)
            {
                return ObjectResultDto<bool>.Error($"Error deleting price rule: {ex.Message}", false);
            }
        }

        private bool ProductExists(string name, string grade) =>
            _context.Products.Any(e => e.Name == name && e.Grade == grade);

        private bool PriceRuleExists(string storeClass, string productGrade, DateTime startDate) =>
            _context.PriceRules.Any(e =>
                e.StoreClass == storeClass &&
                e.ProductGrade == productGrade &&
                e.StartDate == startDate);
    }
}
