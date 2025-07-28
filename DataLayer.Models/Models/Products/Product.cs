using DataLayer.Abstract.Models.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.Models.Products
{
    /// <summary>
    /// Товар
    /// </summary>
    [Table("product")]
    [Description("Товар")]
    public class Product : UpdatedEntity
    {
        /// <summary>
        /// Наименование товара
        /// </summary>
        [Column("name")]
        [Description("Наименование товара")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Качественная категория
        /// </summary>
        [Column("grade")]
        [Description("Качественная категория")]
        public string Grade { get; set; } = null!;

        /// <summary>
        /// Товары в отделах
        /// </summary>
        public ICollection<DepartmentProduct> DepartmentProducts { get; set; } = new List<DepartmentProduct>();

        /// <summary>
        /// Товары на базах 
        /// </summary>
        public ICollection<WarehouseProduct> WarehouseProducts { get; set; } = new List<WarehouseProduct>();
    }
}
