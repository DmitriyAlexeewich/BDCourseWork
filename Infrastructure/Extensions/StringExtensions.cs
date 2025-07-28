using System.Text;

namespace Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static string AppendToString(this string source, params object[] args)
        {
            var strBuilder = new StringBuilder();

            strBuilder.AppendFormat(source, args);

            return strBuilder.ToString();
        }
    }
}
