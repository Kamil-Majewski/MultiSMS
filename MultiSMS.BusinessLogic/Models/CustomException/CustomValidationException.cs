namespace MultiSMS.BusinessLogic.Models.CustomException
{
    public class CustomValidationException : Exception
    {
        public List<string> Errors { get; }

        public CustomValidationException(string message, List<string> errors) : base(message)
        {
            Errors = errors;
        }
    }
}
