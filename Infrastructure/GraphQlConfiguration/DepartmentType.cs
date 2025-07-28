using DataLayer.Models.Models;

namespace Infrastructure.GraphQlConfiguration
{
    public class DepartmentType : ObjectType<Department>
    {
        protected override void Configure(IObjectTypeDescriptor<Department> descriptor)
        {
            descriptor.Field(d => d.StoreNumber).Type<NonNullType<IntType>>();
            descriptor.Field(d => d.Name).Type<NonNullType<StringType>>();
            descriptor.Field(d => d.Head).Type<NonNullType<StringType>>();
            descriptor.Field(d => d.Store).Type<StoreType>();
            descriptor.Field(d => d.DepartmentProducts).UseProjection().UseFiltering().UseSorting();
        }
    }
}
