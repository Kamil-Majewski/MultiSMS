namespace MultiSMS.BusinessLogic.Models
{
    public class SendSmsResultModel
    {
        public string ResponseContent { get; init; }
        public Dictionary<string, object> Parameters { get; init; }

        public SendSmsResultModel(string responseContent, Dictionary<string, object> parameters)
        {
            ResponseContent = responseContent;
            Parameters = parameters;
        }
    }
}
