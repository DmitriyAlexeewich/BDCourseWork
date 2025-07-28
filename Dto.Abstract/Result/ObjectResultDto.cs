namespace Dto.Abstract.Result
{
    /// <summary>
    /// Dto результата выполнения операции с объектом
    /// </summary>
    /// <typeparam name="T">Тип объекта</typeparam>
    public class ObjectResultDto<T> : ResultDto
    {
        /// <summary>
        /// Возвращаемый объект
        /// </summary>
        public required T Result { get; set; }

        public static ObjectResultDto<T> Ok(T result, string message = "")
        {
            return new ObjectResultDto<T>
            {
                Success = true,
                Message = message,
                Result = result
            };
        }

        public static ObjectResultDto<T> Error(string message, T? result = default)
        {
            return new ObjectResultDto<T>
            {
                Success = false,
                Message = message,
                Result = result
            };
        }
    }
}
