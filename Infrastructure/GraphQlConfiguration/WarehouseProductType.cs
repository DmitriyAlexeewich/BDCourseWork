using DataLayer.Models.Models.Products;

namespace Infrastructure.GraphQlConfiguration
{
    public class WarehouseProductType : ObjectType<WarehouseProduct>
    {
        protected override void Configure(IObjectTypeDescriptor<WarehouseProduct> descriptor)
        {
            descriptor.Field(wp => wp.WarehouseName).Type<NonNullType<StringType>>();
            descriptor.Field(wp => wp.ProductName).Type<NonNullType<StringType>>();
            descriptor.Field(wp => wp.ProductGrade).Type<NonNullType<StringType>>();
            descriptor.Field(wp => wp.Quantity).Type<NonNullType<IntType>>();
            descriptor.Field(wp => wp.Price).Type<NonNullType<DecimalType>>();
            descriptor.Field(wp => wp.Warehouse).Type<WarehouseType>();
            descriptor.Field(wp => wp.Product).Type<ProductType>();
        }
    }
}
