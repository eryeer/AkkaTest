using Akka.Actor;
using Akka.IO;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Study.TCP
{
    public class StartServer
    {
        static void Main01(string[] args)
        {
            var sys = ActorSystem.Create("MySystem");
            //var manager = sys.Tcp();
            var server = sys.ActorOf(TcpServer.Props(8089), "tcp-server");
            Console.ReadLine();
        }
    }

    public class TcpServer : UntypedActor
    {
        public static Props Props(int port) => Akka.Actor.Props.Create(() => new TcpServer(port));

        public TcpServer(int port)
        {
            Context.System.Tcp().Tell(new Tcp.Bind(Self, new IPEndPoint(IPAddress.Any, port)));
            Console.WriteLine($"start to listen port {port}");
        }

        protected override void OnReceive(object message)
        {
            if (message is Tcp.Bound)
            {
                var bound = message as Tcp.Bound;
                Console.WriteLine("Listening on {0}", bound.LocalAddress);
            }
            else if (message is Tcp.Connected)
            {
                var connection = Context.ActorOf(Akka.Actor.Props.Create(() => new EchoConnection(Sender)));
                Sender.Tell(new Tcp.Register(connection));
            }
            else Unhandled(message);
        }
    }

    public class EchoConnection : UntypedActor
    {
        private readonly IActorRef _connection;

        public EchoConnection(IActorRef connection)
        {
            _connection = connection;
        }

        protected override void OnReceive(object message)
        {
            if (message is Tcp.Received)
            {
                var received = message as Tcp.Received;
                if (received.Data[0] == 'x')
                    Context.Stop(Self);
                else
                    _connection.Tell(Tcp.Write.Create(received.Data));
            }
            else Unhandled(message);
        }
    }
}
