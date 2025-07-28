using DataLayer.Abstract.DataAccess.Context;
using DataLayer.Abstract.DataBaseUpdater.Filler;
using DataLayer.Models.Models;

namespace DataLayer.DataBaseUpdater.Filler
{
    public class WarehouseFiller : BaseFillProfile<Warehouse>
    {
        private readonly IEnumerable<Warehouse> _warehouses = new Warehouse[]
        {
            new Warehouse
            {
                Name = "Главный склад",
                Address = "ул. Центральная, 1"
            },
            new Warehouse
            {
                Name = "Склад Северный",
                Address = "ул. Северная, 5"
            },
            new Warehouse
            {
                Name = "Склад Западный",
                Address = "ул. Западная, 10"
            }
        };

        protected override Task FillEntities(IBaseDbContext context)
        {
            //Add(_warehouses);

            return Task.CompletedTask;
        }
    }
}
