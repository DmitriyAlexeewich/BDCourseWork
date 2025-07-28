using DataLayer.Abstract.Models.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.Models.Products
{
    /// <summary>
    /// Товар в отделе
    /// </summary>
    [Table("departmentProduct")]
    [Description("Товар в отделе")]
    public class DepartmentProduct : UpdatedEntity
    {
        /// <summary>
        /// Наименование товара
        /// </summary>
        [Column("productName")]
        [Description("Наименование товара")]
        public string ProductName { get; set; } = null!;

        /// <summary>
        /// Качественная категория
        /// </summary>
        [Column("productGrade")]
        [Description("Качественная категория")]
        public string ProductGrade { get; set; } = null!;

        /// <summary>
        /// Номер магазина
        /// </summary>
        [Column("storeNumber")]
        [Description("Номер магазина")]
        public int StoreNumber { get; set; }

        /// <summary>
        /// Имя отдела
        /// </summary>
        [Column("departmentName")]
        [Description("Имя отдела")]
        public string DepartmentName { get; set; } = null!;

        /// <summary>
        /// Количество товара в отделе
        /// </summary>
        [Column("quantity")]
        [Description("Количество")]
        public int Quantity { get; set; }

        /// <summary>
        /// Отдел
        /// </summary>
        public Department Department { get; set; } = null!;

        /// <summary>
        /// Продукт
        /// </summary>
        public Product Product { get; set; } = null!;
    }
}
