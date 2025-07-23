using DataLayer.DataBaseUpdater.DataContext;
using DataLayer.Models.Models.Products;
using DataLayer.Models.Rules;
using Dto.Abstract.Result;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TradeManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly PostgreDbContext _context;

        public ProductsController(PostgreDbContext context)
        {
            _context = context;
        }

        // Создание товара
        [HttpPost]
        public async Task<ActionResult<ObjectResultDto<Product>>> CreateProduct(Product product)
        {
            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return ObjectResultDto<Product>.Ok(product, "Product created successfully");
            }
            catch (Exception ex)
            {
                return ObjectResultDto<Product>.Error($"Error creating product: {ex.Message}");
            }
        }

        // Обновление товара
        [HttpPut("{name}/{grade}")]
        public async Task<ActionResult<ObjectResultDto<Product>>> UpdateProduct(
            string name, string grade, Product updatedProduct)
        {
            try
            {
                if (name != updatedProduct.Name || grade != updatedProduct.Grade)
                    return ObjectResultDto<Product>.Error("Product identifier mismatch");

                _context.Entry(updatedProduct).State = EntityState.Modified;
                await _context.SaveChangesAsync();

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
        public async Task<ActionResult<ObjectResultDto<bool>>> DeleteProduct(string name, string grade)
        {
            try
            {
                var product = await _context.Products.FindAsync(name, grade);
                if (product == null)
                    return ObjectResultDto<bool>.Error("Product not found", false);

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return ObjectResultDto<bool>.Ok(true, "Product deleted successfully");
            }
            catch (Exception ex)
            {
                return ObjectResultDto<bool>.Error($"Error deleting product: {ex.Message}", false);
            }
        }

        // Создание прайс-правила
        [HttpPost("prices")]
        public async Task<ActionResult<ObjectResultDto<PriceRule>>> CreatePriceRule(PriceRule priceRule)
        {
            try
            {
                _context.PriceRules.Add(priceRule);
                await _context.SaveChangesAsync();

                return ObjectResultDto<PriceRule>.Ok(priceRule, "Price rule created successfully");
            }
            catch (Exception ex)
            {
                return ObjectResultDto<PriceRule>.Error($"Error creating price rule: {ex.Message}");
            }
        }

        // Обновление прайс-правила
        [HttpPut("prices")]
        public async Task<ActionResult<ObjectResultDto<PriceRule>>> UpdatePriceRule(
            [FromQuery] string storeClass,
            [FromQuery] string productGrade,
            [FromQuery] DateTime startDate,
            PriceRule updatedRule)
        {
            try
            {
                if (storeClass != updatedRule.StoreClass ||
                    productGrade != updatedRule.ProductGrade ||
                    startDate != updatedRule.StartDate)
                    return ObjectResultDto<PriceRule>.Error("Price rule identifier mismatch");

                _context.Entry(updatedRule).State = EntityState.Modified;
                await _context.SaveChangesAsync();

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
        public async Task<ActionResult<ObjectResultDto<bool>>> DeletePriceRule(
            [FromQuery] string storeClass,
            [FromQuery] string productGrade,
            [FromQuery] DateTime startDate)
        {
            try
            {
                var priceRule = await _context.PriceRules.FindAsync(storeClass, productGrade, startDate);
                if (priceRule == null)
                    return ObjectResultDto<bool>.Error("Price rule not found", false);

                _context.PriceRules.Remove(priceRule);
                await _context.SaveChangesAsync();

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
