using DataLayer.Abstract.Models.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.Rules
{
    /// <summary>
    /// Правило ценообразования для товара
    /// </summary>
    [Table("priceRule")]
    [Description("Правило ценообразования")]
    public class PriceRule : RemovedEntity
    {
        /// <summary>
        /// Категория магазина
        /// </summary>
        [Column("storeClass")]
        [Description("Категория магазина")]
        public string StoreClass { get; set; } = null!;

        /// <summary>
        /// Качественная категория товара
        /// </summary>
        [Column("productGrade")]
        [Description("Качественная категория")]
        public string ProductGrade { get; set; } = null!;

        /// <summary>
        /// Начало действия цены
        /// </summary>
        [Column("startDate")]
        [Description("Начало действия цены")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Окончание действия цены
        /// </summary>
        [Column("endDate")]
        [Description("Окончание действия цены")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Розничная стоимость
        /// </summary>
        [Column("price")]
        [Description("Розничная стоимость")]
        public decimal Price { get; set; }
    }
}
