using Microsoft.AspNetCore.SignalR;

namespace server.Services
{

    public class VendingMachineHub(ILogger<VendingMachineHub> logger) : SingleConnectionHub(logger)
    {
        public async Task SendDrinkUpdate(string message)
        {
            await Clients.All.SendAsync("ReceiveDrinkUpdate", message);
        }
    }

    public class SingleConnectionHub(ILogger<SingleConnectionHub> logger) : Hub
    {

        private readonly ILogger<SingleConnectionHub> _logger = logger;
        private static readonly HashSet<string> ConnectedIds = new();
        private static int connected = 0;

        public override async Task OnConnectedAsync()
        {
            
            if (connected>=1)
            {
                await Clients.Caller.SendAsync("ConnectionRejected", "already-connected");
                Context.Abort();
                connected += 1;
                return;
            }
            connected += 1;
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            connected -= 1;
            await base.OnDisconnectedAsync(exception);
        }
    }
}