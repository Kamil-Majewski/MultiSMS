using Microsoft.AspNetCore.SignalR;
using MultiSMS.BusinessLogic.Services.Interfaces;

namespace MultiSMS.MVC.Hubs
{
    public class ProgressRelay : IProgressRelay
    {
        private readonly IHubContext<ProgressHub> _hubContext;

        public ProgressRelay(IHubContext<ProgressHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task RelayProgressAsync(string method, string progress)
        {
            await _hubContext.Clients.All.SendAsync(method, progress);
        }
    }
}
