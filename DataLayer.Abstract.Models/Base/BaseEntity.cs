using DataLayer.Abstract.Models.Base.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Abstract.Models.Base
{
    /// <summary>
    /// Базовая сущность
    /// </summary>
    [Description("Базовая сущность")]
    public abstract class BaseEntity : IBaseEntity
    {
        /// <summary>
        /// Ключ сущности
        /// </summary>
        [Column("id")]
        [Description("Ключ сущности")]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Дата создания сущности
        /// </summary>
        [Column("created_at")]
        [Description("Дата создания сущности")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
