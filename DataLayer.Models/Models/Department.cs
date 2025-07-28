using DataLayer.Abstract.Models.Base;
using DataLayer.Models.Models.Products;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models.Models
{
    /// <summary>
    /// Отдел магазина
    /// </summary>
    [Table("department")]
    [Description("Отдел магазина")]
    public class Department : UpdatedEntity
    {
        /// <summary>
        /// Номер магазина
        /// </summary>
        [Column("number")]
        [Description("Номер магазина")]
        public int StoreNumber { get; set; }

        /// <summary>
        /// Уникальное название отдела в рамках магазина
        /// </summary>
        [Column("name")]
        [Description("Название отдела")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// ФИО ответственного лица
        /// </summary>
        [Column("head")]
        [Description("ФИО ответственного лица")]
        public string Head { get; set; } = null!;

        /// <summary>
        /// Магазин в котором располагается отдел
        /// </summary>
        public Store Store { get; set; } = null!;

        /// <summary>
        /// Продукты представленные в отделе
        /// </summary>
        public ICollection<DepartmentProduct> DepartmentProducts { get; set; } = new List<DepartmentProduct>();
    }
}
