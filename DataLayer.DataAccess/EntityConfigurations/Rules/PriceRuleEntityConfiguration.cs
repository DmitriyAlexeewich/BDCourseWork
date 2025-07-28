using DataLayer.Abstract.DataAccess.EntityConfigurations;
using DataLayer.Models.Rules;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.DataAccess.EntityConfigurations.Rules
{
    /// <summary>
    /// Конфигурация сущности правило ценообразования
    /// </summary>
    public class PriceRuleEntityConfiguration : RemovedEntityConfiguration<PriceRule>
    {
        /// <summary>
        /// Конфигурация сущности
        /// </summary>
        /// <param name="builder">Билдер сущности</param>
        public override void Configure(EntityTypeBuilder<PriceRule> builder)
        {
            _idIsKey = false;

            base.Configure(builder);

            builder.HasKey(pr => new {
                pr.StoreClass,
                pr.ProductGrade,
                pr.StartDate
            });
        }
    }
}
