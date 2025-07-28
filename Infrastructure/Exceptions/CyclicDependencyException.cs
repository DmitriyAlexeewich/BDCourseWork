namespace Infrastructure.Exceptions
{
    /// <summary>
    /// Исключение возникающее при возникновении циклической зависимости
    /// </summary>
    public class CyclicDependencyException : Exception
    {
        /// <summary>
        /// Путь на котором возникла циклическая зависимость
        /// </summary>
        public object[] Path { get; set; }

        public CyclicDependencyException(object[] path) : this(path, $"Найдена циклическая зависимость.")
        {
        }

        public CyclicDependencyException(object[] path, string message) : base(message)
        {
            Path = path;
        }
    }

}
