using DataLayer.Models.Models;

namespace Infrastructure.GraphQlConfiguration
{
    public class StoreType : ObjectType<Store>
    {
        protected override void Configure(IObjectTypeDescriptor<Store> descriptor)
        {
            descriptor.Field(s => s.Number).Type<NonNullType<IntType>>();
            descriptor.Field(s => s.Class).Type<NonNullType<StringType>>();
            descriptor.Field(s => s.WarehouseName).Type<NonNullType<StringType>>();
            descriptor.Field(s => s.Warehouse).Type<WarehouseType>();
            descriptor.Field(s => s.Departments).UseProjection().UseFiltering().UseSorting();
        }
    }
}
