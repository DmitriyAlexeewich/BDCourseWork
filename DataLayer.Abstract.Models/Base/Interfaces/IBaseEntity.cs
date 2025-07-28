namespace DataLayer.Abstract.Models.Base.Interfaces
{
    public interface IBaseEntity
    {
        /// <summary>
        /// Ключ сущности
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Дата создания сущности
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
