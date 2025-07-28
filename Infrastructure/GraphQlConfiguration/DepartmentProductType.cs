using DataLayer.Models.Models.Products;

namespace Infrastructure.GraphQlConfiguration
{
    public class DepartmentProductType : ObjectType<DepartmentProduct>
    {
        protected override void Configure(IObjectTypeDescriptor<DepartmentProduct> descriptor)
        {
            descriptor.Field(dp => dp.StoreNumber).Type<NonNullType<IntType>>();
            descriptor.Field(dp => dp.DepartmentName).Type<NonNullType<StringType>>();
            descriptor.Field(dp => dp.ProductName).Type<NonNullType<StringType>>();
            descriptor.Field(dp => dp.ProductGrade).Type<NonNullType<StringType>>();
            descriptor.Field(dp => dp.Quantity).Type<NonNullType<IntType>>();
            descriptor.Field(dp => dp.Department).Type<DepartmentType>();
            descriptor.Field(dp => dp.Product).Type<ProductType>();
        }
    }
}
