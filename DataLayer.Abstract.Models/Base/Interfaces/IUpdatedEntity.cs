namespace DataLayer.Abstract.Models.Base.Interfaces
{
    public interface IUpdatedEntity : IRemovedEntity
    {
        /// <summary>
        /// Дата последнего обновления
        /// </summary>
        DateTime? LastUpdatedAt { get; set; }
    }
}
