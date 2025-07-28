using DataLayer.Abstract.DataAccess.Context;
using DataLayer.Abstract.DataBaseUpdater.Filler;
using DataLayer.Models.Rules;

namespace DataLayer.DataBaseUpdater.Filler
{
    public class PriceRuleFiller : BaseFillProfile<PriceRule>
    {
        private readonly IEnumerable<PriceRule> _priceRules = new PriceRule[]
        {
            new PriceRule
            {
                StoreClass = "Премиум",
                ProductGrade = "Высший",
                StartDate = new DateTime(2023, 1, 1).ToUniversalTime(),
                EndDate = null,
                Price = 100.0m
            },
            new PriceRule
            {
                StoreClass = "Премиум",
                ProductGrade = "Первый",
                StartDate = new DateTime(2023, 1, 1).ToUniversalTime(),
                EndDate = null,
                Price = 80.0m
            },
            new PriceRule
            {
                StoreClass = "Эконом",
                ProductGrade = "Высший",
                StartDate = new DateTime(2023, 1, 1).ToUniversalTime(),
                EndDate = new DateTime(2023, 12, 31).ToUniversalTime(),
                Price = 85.0m
            },
            new PriceRule
            {
                StoreClass = "Эконом",
                ProductGrade = "Первый",
                StartDate = new DateTime(2023, 1, 1).ToUniversalTime(),
                EndDate = null,
                Price = 65.0m
            }
        };

        protected override Task FillEntities(IBaseDbContext context)
        {
            //Add(_priceRules);

            return Task.CompletedTask;
        }
    }
}
