namespace Dto.Abstract.Result
{
    /// <summary>
    /// Dto результата выполнения операции
    /// </summary>
    public class ResultDto
    {
        /// <summary>
        /// Флаг успешной операции
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Возвращаемое сообщение
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
