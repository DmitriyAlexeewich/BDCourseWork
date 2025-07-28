using DataLayer.Models.Models;

namespace Infrastructure.GraphQlConfiguration
{
    public class WarehouseType : ObjectType<Warehouse>
    {
        protected override void Configure(IObjectTypeDescriptor<Warehouse> descriptor)
        {
            descriptor.Field(w => w.Name).Type<NonNullType<StringType>>();
            descriptor.Field(w => w.Address).Type<NonNullType<StringType>>();
            descriptor.Field(w => w.Stores).UseProjection().UseFiltering().UseSorting();
            descriptor.Field(w => w.WarehouseProducts).UseProjection().UseFiltering().UseSorting();
        }
    }
}
