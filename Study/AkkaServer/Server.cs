using Akka.Actor;
using Akka.Event;
using Akka.IO;
using System;
using System.Net;
using System.Text;

namespace AkkaServer
{
    public class StartServer
    {
        static void Main(string[] args)
        {
            var sys = ActorSystem.Create("ServerSystem");
            //var manager = sys.Tcp();
            var server = sys.ActorOf(TcpServer.Props(8089), "tcp-server");
            
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

        public EchoConnection(IActorRef connection)
        {
            _connection = connection;
            Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(1000, 1000, Self, new TPSTimer(), Self);
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
                        Log.Info($"{receivedCount} Data received");
                        Log.Info("msg size is {0}", received.Data.Count);
                        var arr = received.Data.ToArray().ToHexString();
                        Log.Info("msg content: {0}", arr);
                    }
                    break;
                case TPSTimer _:
                    Log.Info("Tps is {0}/s", receivedCount);
                    receivedCount = 0;
                    break;
                default:
                    Unhandled(message);
                    break;
            }
        }
    }

    internal class TPSTimer { }
}
