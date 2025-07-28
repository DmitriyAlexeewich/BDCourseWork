using DataLayer.Models.Models.Products;

namespace Infrastructure.GraphQlConfiguration
{
    public class ProductType : ObjectType<Product>
    {
        protected override void Configure(IObjectTypeDescriptor<Product> descriptor)
        {
            descriptor.Field(p => p.Name).Type<NonNullType<StringType>>();
            descriptor.Field(p => p.Grade).Type<NonNullType<StringType>>();
            descriptor.Field(p => p.DepartmentProducts).UseProjection().UseFiltering().UseSorting();
            descriptor.Field(p => p.WarehouseProducts).UseProjection().UseFiltering().UseSorting();
        }
    }
}
