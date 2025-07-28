using DataLayer.Models.Rules;

namespace Infrastructure.GraphQlConfiguration
{
    public class PriceRuleType : ObjectType<PriceRule>
    {
        protected override void Configure(IObjectTypeDescriptor<PriceRule> descriptor)
        {
            descriptor.Field(pr => pr.StoreClass).Type<NonNullType<StringType>>();
            descriptor.Field(pr => pr.ProductGrade).Type<NonNullType<StringType>>();
            descriptor.Field(pr => pr.StartDate).Type<NonNullType<DateTimeType>>();
            descriptor.Field(pr => pr.EndDate).Type<DateTimeType>();
            descriptor.Field(pr => pr.Price).Type<NonNullType<DecimalType>>();
        }
    }
}
