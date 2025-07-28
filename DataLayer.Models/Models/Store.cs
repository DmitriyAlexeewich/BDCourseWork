using DataLayer.Abstract.Models.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.Models
{
    /// <summary>
    /// Магазин
    /// </summary>
    [Table("store")]
    [Description("Магазин")]
    public class Store : UpdatedEntity
    {
        /// <summary>
        /// Номер магазина
        /// </summary>
        [Column("number")]
        [Description("Номер магазина")]
        public int Number { get; set; }

        /// <summary>
        /// Категория магазина (эконом, премиум и т.д.)
        /// </summary>
        [Column("class")]
        [Description("Категория магазина")]
        public string Class { get; set; } = null!;

        /// <summary>
        /// Название торговой базы
        /// </summary>
        [Column("warehouseName")]
        [Description("Название торговой базы")]
        public string WarehouseName { get; set; } = null!;

        /// <summary>
        /// Торговая база откуда поставляются товары
        /// </summary>
        public Warehouse Warehouse { get; set; } = null!;

        /// <summary>
        /// Торговой отдел магазина
        /// </summary>
        public ICollection<Department> Departments { get; set; } = new List<Department>();
    }
}
