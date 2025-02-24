
namespace MultiSMS.BusinessLogic.Helpers
{
    public static class ValidationHelper
    {
        public static void ValidateId(int id, string paramName)
        {
            if (id <= 0) throw new ArgumentException($"Provided {paramName} was invalid (0 or negative)", paramName);
        }

        public static void ValidateObject(object? obj, string paramName)
        {
            if (obj == null) throw new ArgumentException($"Provided {paramName} was null", paramName);
        }

        public static void ValidateString(string? value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"Provided {paramName} was null, empty, or whitespace", paramName);
        }

        public static void ValidateCollection<T>(IEnumerable<T>? collection, string paramName)
        {
            if (collection == null || !collection.Any())
                throw new ArgumentException($"Provided {paramName} was null or empty", paramName);
        }
    }
}
