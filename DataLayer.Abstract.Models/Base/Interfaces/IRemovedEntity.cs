namespace DataLayer.Abstract.Models.Base.Interfaces
{
    public interface IRemovedEntity : IBaseEntity
    {
        /// <summary>
        /// Флаг удаления сущности
        /// </summary>
        public bool IsRemoved { get; set; }

        /// <summary>
        /// Дата удаления сущности
        /// </summary>
        public DateTime? RemovedAt { get; set; }
    }
}
