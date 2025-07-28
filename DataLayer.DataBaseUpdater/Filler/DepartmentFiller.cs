using DataLayer.Abstract.DataAccess.Context;
using DataLayer.Abstract.DataBaseUpdater.Filler;
using DataLayer.Models.Models;

namespace DataLayer.DataBaseUpdater.Filler
{
    public class DepartmentFiller : BaseFillProfile<Department>
    {
        public DepartmentFiller()
        {
            DependsOn<StoreFiller>();
        }

        private readonly IEnumerable<Department> _departments = new Department[]
        {
            new Department
            {
                StoreNumber = 1,
                Name = "Молочный",
                Head = "Иванов И.И."
            },
            new Department
            {
                StoreNumber = 1,
                Name = "Мясной",
                Head = "Петров П.П."
            },
            new Department
            {
                StoreNumber = 2,
                Name = "Бакалея",
                Head = "Сидоров С.С."
            },
            new Department
            {
                StoreNumber = 3,
                Name = "Овощи-Фрукты",
                Head = "Кузнецова А.В."
            }
        };

        protected override Task FillEntities(IBaseDbContext context)
        {
            //Add(_departments);

            return Task.CompletedTask;
        }
    }
}
