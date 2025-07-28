using DataLayer.Abstract.Models.Base.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Abstract.Models.Base
{
    /// <summary>
    /// Обновляемая сущность
    /// </summary>
    [Description("Обновляемая сущность")]
    public abstract class UpdatedEntity : RemovedEntity, IUpdatedEntity
    {
        /// <summary>
        /// Дата последнего обновления
        /// </summary>
        [Column("updated_at")]
        [Description("Дата последнего обновления")]
        public DateTime? LastUpdatedAt { get; set; }
    }
}
