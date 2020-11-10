using Fireasy.Web.Sockets;

namespace Fireasy.Zero.WebSockets
{
    public class ChatHandler : WebSocketHandler
    {
        private SessionManager<string> _manager = new SessionManager<string>();

        public void Connect(string name)
        {
            _manager.Add(ConnectionId, name);
        }

        public void Send(string to, string message)
        {
            var connectioId = _manager.FindConnection(to);
            if (!string.IsNullOrEmpty(connectioId))
            {
                Clients.Client(connectioId).SendAsync("OnReceiveMsg", message);
            }
        }

        public void Notify(string message)
        {
            Clients.All.SendAsync("OnReceiveNotify", message);
        }

        protected override void OnDisconnected()
        {
            _manager.Remove(ConnectionId);

            base.OnDisconnected();
        }
    }
}
