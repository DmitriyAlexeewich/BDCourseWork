using DataLayer.Abstract.DataAccess.Context;
using Microsoft.Extensions.Logging;

namespace DataLayer.Abstract.DataBaseUpdater.Filler.Interfaces
{
    /// <summary>
    /// Интерфейс профиля первичного заполнения
    /// </summary>
    public interface IDbFillProfile
    {
        /// <summary>
        /// Зависит от профилей
        /// </summary>
        IEnumerable<Type> DependsOn { get; }

        /// <summary>
        /// Метод вызываемый для заполнения той или иной таблицы
        /// </summary>
        /// <param name="context"></param>
        Task Fill(IBaseDbContext context, ILogger logger);
    }
}
