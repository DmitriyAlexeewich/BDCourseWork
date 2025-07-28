using DataLayer.Abstract.DataAccess.Context;
using DataLayer.Abstract.DataBaseUpdater.Filler;
using DataLayer.Models.Models.Products;

namespace DataLayer.DataBaseUpdater.Filler
{
    public class ProductFiller : BaseFillProfile<Product>
    {
        private readonly IEnumerable<Product> _products = new Product[]
        {
            new Product
            {
                Name = "Молоко",
                Grade = "Высший"
            },
            new Product
            {
                Name = "Молоко",
                Grade = "Первый"
            },
            new Product
            {
                Name = "Хлеб",
                Grade = "Первый"
            },
            new Product
            {
                Name = "Говядина",
                Grade = "Высший"
            }
        };

        protected override Task FillEntities(IBaseDbContext context)
        {
            //Add(_products);

            return Task.CompletedTask;
        }
    }
}
