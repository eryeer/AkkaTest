using Akka.Actor;
using Akka.IO;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AkkaClient
{
    public class StartClient
    {
        static void Main(string[] args)
        {
            var sys = ActorSystem.Create("ClientSystem");
            //var manager = sys.Tcp();
            var client = sys.ActorOf(TcpClient.Props("127.0.0.1", 8089), "tcp-client");
            string msg = "";
            while (true)
            {
                msg = Console.ReadLine();
                if (msg.Equals("exit"))
                {
                    break;
                }
                else
                {
                    client.Tell(msg);
                }
            }
        }
    }

    public class TcpClient : UntypedActor
    {
        public static Props Props(string host, int port) => Akka.Actor.Props.Create(() => new TcpClient(host, port));
        public TcpClient(string host, int port)
        {
            Console.WriteLine("start construct");
            var endpoint = new DnsEndPoint(host, port);
            Context.System.Tcp().Tell(new Tcp.Connect(endpoint));
            Console.WriteLine("finish construct");
        }

        protected override void OnReceive(object message)
        {
            if (message is Tcp.Connected)
            {
                var connected = message as Tcp.Connected;
                Console.WriteLine("Connected to {0}", connected.RemoteAddress);

                // Register self as connection handler
                Sender.Tell(new Tcp.Register(Self));
                Become(Connected(Sender));
            }
            else if (message is Tcp.CommandFailed)
            {
                Console.WriteLine("Connection failed");
            }
            else Unhandled(message);
        }

        private UntypedReceive Connected(IActorRef connection)
        {
            return message =>
            {
                if (message is Tcp.Received)  // data received from network
                {
                    var received = message as Tcp.Received;
                    Console.WriteLine("response from server for {0}", Encoding.ASCII.GetString(received.Data.ToArray()));
                }
                else if (message is string)   // data received from console
                {
                    connection.Tell(Tcp.Write.Create(ByteString.FromString((string)message + "\n")));
                }
                else if (message is Tcp.PeerClosed)
                {
                    Console.WriteLine("Connection closed");
                }
                else Unhandled(message);
            };
        }
    }
}
