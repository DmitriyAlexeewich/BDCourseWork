using DataLayer.Abstract.Models.Base;
using DataLayer.Models.Models.Products;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.Models
{
    /// <summary>
    /// Торговая база
    /// </summary>
    [Table("warehouse")]
    [Description("Торговая база")]
    public class Warehouse : UpdatedEntity
    {
        /// <summary>
        /// Название торговой базы
        /// </summary>
        [Column("name")]
        [Description("Название торговой базы")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Адрес физического расположения тарговой базы
        /// </summary>
        [Column("address")]
        [Description("Адрес физического расположения тарговой базы")]
        public string Address { get; set; } = null!;

        /// <summary>
        /// Магазины куда поставляются товары с торговой базы
        /// </summary>
        public ICollection<Store> Stores { get; set; } = new List<Store>();

        /// <summary>
        /// Продукция поставляемая торговой базой
        /// </summary>
        public ICollection<WarehouseProduct> WarehouseProducts { get; set; } = new List<WarehouseProduct>();
    }
}
