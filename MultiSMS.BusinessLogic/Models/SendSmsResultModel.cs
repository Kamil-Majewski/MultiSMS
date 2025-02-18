namespace MultiSMS.BusinessLogic.Models
{
    public class SendSmsResultModel
    {
        public string ResponseContent { get; init; }
        public Dictionary<string, string> Parameters { get; init; }

        public SendSmsResultModel(string responseContent, Dictionary<string, string> parameters)
        {
            ResponseContent = responseContent;
            Parameters = parameters;
        }
    }
}
