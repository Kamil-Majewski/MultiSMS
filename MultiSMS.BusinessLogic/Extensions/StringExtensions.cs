namespace MultiSMS.BusinessLogic.Extensions
{
    public static class StringExtensions
    {
        public static bool ContainsCaseInsensitive(this string source, string value)
        {
            return source.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
