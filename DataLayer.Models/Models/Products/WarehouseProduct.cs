using DataLayer.Abstract.Models.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.Models.Products
{
    /// <summary>
    /// Товар на торговой базе
    /// </summary>
    [Table("warehouseProduct")]
    [Description("Товар на торговой базе")]
    public class WarehouseProduct : UpdatedEntity
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
        /// Название торговой базы
        /// </summary>
        [Column("warehouseName")]
        [Description("Название торговой базы")]
        public string WarehouseName { get; set; } = null!;

        /// <summary>
        /// Количество товара на базе
        /// </summary>
        [Column("quantity")]
        [Description("Количество")]
        public int Quantity { get; set; }

        /// <summary>
        /// Цена товара
        /// </summary>
        [Column("price")]
        [Description("Цена")]
        public decimal Price { get; set; }

        /// <summary>
        /// Торговая база откуда поставляется товар
        /// </summary>
        public Warehouse Warehouse { get; set; } = null!;

        /// <summary>
        /// Продукт
        /// </summary>
        public Product Product { get; set; } = null!;
    }
}
