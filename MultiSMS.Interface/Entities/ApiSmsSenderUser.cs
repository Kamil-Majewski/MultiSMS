
namespace MultiSMS.Interface.Entities
{
    public class ApiSmsSenderUser
    {
        public int ApiSmsSenderId { get; set; }
        public ApiSmsSender ApiSmsSender { get; set; } = default!;

        public int UserId { get; set; }
    }
}
