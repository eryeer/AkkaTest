using Akka.Actor;
using Akka.Event;
using Akka.IO;
using System;
using System.Net;
using System.Text;
using System.Threading;

namespace AkkaServer
{
    public class StartServer
    {
        static void Main(string[] args)
        {
            var sys = ActorSystem.Create("ServerSystem");
            var server1 = sys.ActorOf(TcpServer.Props(8088), "tcp-server1");
            var server2 = sys.ActorOf(TcpServer.Props(8089), "tcp-server2");
            var server3 = sys.ActorOf(TcpServer.Props(8090), "tcp-server3");
            var server4 = sys.ActorOf(TcpServer.Props(8091), "tcp-server4");
            Console.ReadLine();
        }
    }

    public class TcpServer : UntypedActor
    {
        protected ILoggingAdapter Log { get; } = Context.GetLogger();
        public static Props Props(int port) => Akka.Actor.Props.Create(() => new TcpServer(port));

        public TcpServer(int port)
        {
            Context.System.Tcp().Tell(new Tcp.Bind(Self, new IPEndPoint(IPAddress.Any, port)));
            Log.Info($"start to listen port {port}");
        }

        protected override void OnReceive(object message)
        {
            if (message is Tcp.Bound)
            {
                var bound = message as Tcp.Bound;
                Log.Info("Listening on {0}", bound.LocalAddress);
            }
            else if (message is Tcp.Connected)
            {
                var connectMsg = message as Tcp.Connected;
                var connection = Context.ActorOf(Akka.Actor.Props.Create(() => new EchoConnection(Sender)));
                Sender.Tell(new Tcp.Register(connection));
                Log.Info("Connected to {0}", connectMsg.RemoteAddress);
            }
            else Unhandled(message);
        }
    }

    public class EchoConnection : UntypedActor
    {
        private long receivedCount = 0;
        protected ILoggingAdapter Log { get; } = Context.GetLogger();
        private readonly IActorRef _connection;
        private ICancelable scheduler;

        public EchoConnection(IActorRef connection)
        {
            _connection = connection;
            scheduler = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(1000, 1000, Self, new TPSTimer(), Self);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case Tcp.Received received:
                    if ("stop_server".Equals(Encoding.ASCII.GetString(received.Data.ToArray())))
                        Context.Stop(Self);
                    else
                    {
                        receivedCount++;
                        //Log.Info($"{receivedCount} Data received");
                        //for (int i = 0; i <= 100000; i++)
                        //{ }
                        //Log.Info("msg size is {0}", received.Data.Count);
                        //var arr = received.Data.ToArray().ToHexString();
                        //Log.Info("msg content: {0}", arr);
                    }
                    break;
                case TPSTimer _:
                    Log.Info("Tps is {0}/s", receivedCount);
                    receivedCount = 0;
                    break;
                case Tcp.ConnectionClosed _:
                    Log.Warning("Connection Closed");
                    scheduler.CancelIfNotNull();
                    Context.Stop(Self);
                    break;
                default:
                    Unhandled(message);
                    break;
            }
        }
    }

    internal class TPSTimer { }
}
