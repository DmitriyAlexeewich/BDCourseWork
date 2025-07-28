using DataLayer.Abstract.DataAccess.Context;
using DataLayer.Abstract.DataBaseUpdater.Filler;
using DataLayer.Models.Models.Products;

namespace DataLayer.DataBaseUpdater.Filler
{
    public class WarehouseProductFiller : BaseFillProfile<WarehouseProduct>
    {
        public WarehouseProductFiller()
        {
            DependsOn<WarehouseFiller>();
            DependsOn<ProductFiller>();
        }

        private readonly IEnumerable<WarehouseProduct> _warehouseProducts = new WarehouseProduct[]
        {
            new WarehouseProduct
            {
                WarehouseName = "Главный склад",
                ProductName = "Молоко",
                ProductGrade = "Высший",
                Quantity = 100,
                Price = 80.0m
            },
            new WarehouseProduct
            {
                WarehouseName = "Главный склад",
                ProductName = "Молоко",
                ProductGrade = "Первый",
                Quantity = 200,
                Price = 60.0m
            },
            new WarehouseProduct
            {
                WarehouseName = "Главный склад",
                ProductName = "Хлеб",
                ProductGrade = "Первый",
                Quantity = 150,
                Price = 40.0m
            },
            new WarehouseProduct
            {
                WarehouseName = "Склад Северный",
                ProductName = "Говядина",
                ProductGrade = "Высший",
                Quantity = 50,
                Price = 500.0m
            }
        };

        protected override Task FillEntities(IBaseDbContext context)
        {
            //Add(_warehouseProducts);

            return Task.CompletedTask;
        }
    }
}
