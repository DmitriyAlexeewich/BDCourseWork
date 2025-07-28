using DataLayer.Abstract.DataAccess.Context;
using DataLayer.Abstract.DataBaseUpdater.Filler;
using DataLayer.Models.Models;

namespace DataLayer.DataBaseUpdater.Filler
{
    public class StoreFiller : BaseFillProfile<Store>
    {
        public StoreFiller()
        {
            DependsOn<WarehouseFiller>();
        }

        private readonly IEnumerable<Store> _stores = new Store[]
        {
            new Store
            {
                Number = 1,
                Class = "Премиум",
                WarehouseName = "Главный склад"
            },
            new Store
            {
                Number = 2,
                Class = "Эконом",
                WarehouseName = "Главный склад"
            },
            new Store
            {
                Number = 3,
                Class = "Стандарт",
                WarehouseName = "Склад Северный"
            }
        };

        protected override Task FillEntities(IBaseDbContext context)
        {
            //Add(_stores);

            return Task.CompletedTask;
        }
    }
}
