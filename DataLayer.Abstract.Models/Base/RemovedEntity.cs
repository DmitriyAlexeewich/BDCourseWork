using DataLayer.Abstract.Models.Base.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Abstract.Models.Base
{
    /// <summary>
    /// Удаляемая сущность
    /// </summary>
    [Description("Удаляемая сущность")]
    public abstract class RemovedEntity : BaseEntity, IRemovedEntity
    {
        /// <summary>
        /// Флаг удаления сущности
        /// </summary>
        [Column("is_removed")]
        [Description("Флаг удаления сущности")]
        public bool IsRemoved { get; set; }

        /// <summary>
        /// Дата удаления сущности
        /// </summary>
        [Column("removed_at")]
        [Description("Дата удаления сущности")]
        public DateTime? RemovedAt { get; set; }
    }
}
