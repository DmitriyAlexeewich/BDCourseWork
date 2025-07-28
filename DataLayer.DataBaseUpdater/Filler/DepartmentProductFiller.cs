using DataLayer.Abstract.DataAccess.Context;
using DataLayer.Abstract.DataBaseUpdater.Filler;
using DataLayer.Models.Models.Products;

namespace DataLayer.DataBaseUpdater.Filler
{
    public class DepartmentProductFiller : BaseFillProfile<DepartmentProduct>
    {
        public DepartmentProductFiller()
        {
            DependsOn<DepartmentFiller>();
            DependsOn<ProductFiller>();
        }

        private readonly IEnumerable<DepartmentProduct> _departmentProducts = new DepartmentProduct[]
        {
            new DepartmentProduct
            {
                StoreNumber = 1,
                DepartmentName = "Молочный",
                ProductName = "Молоко",
                ProductGrade = "Высший",
                Quantity = 20
            },
            new DepartmentProduct
            {
                StoreNumber = 1,
                DepartmentName = "Молочный",
                ProductName = "Молоко",
                ProductGrade = "Первый",
                Quantity = 30
            },
            new DepartmentProduct
            {
                StoreNumber = 1,
                DepartmentName = "Мясной",
                ProductName = "Говядина",
                ProductGrade = "Высший",
                Quantity = 10
            },
            new DepartmentProduct
            {
                StoreNumber = 2,
                DepartmentName = "Бакалея",
                ProductName = "Хлеб",
                ProductGrade = "Первый",
                Quantity = 25
            }
        };

        protected override Task FillEntities(IBaseDbContext context)
        {
            //Add(_departmentProducts);

            return Task.CompletedTask;
        }
    }
}
