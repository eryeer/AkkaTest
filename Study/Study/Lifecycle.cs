using Akka.Actor;
using System;

namespace Study
{
    public class Lifecycle
    {
        static void Main(string[] args)
        {
            var sys = ActorSystem.Create("MySystem");
            var firstRef = sys.ActorOf(Props.Create<StartStopActor1>(), "first");
            firstRef.Tell("stop");
            Console.ReadLine();
        }
    }

    public class StartStopActor1 : UntypedActor
    {
        protected override void PreStart()
        {
            Console.WriteLine("first started");
            Context.ActorOf(Props.Create<StartStopActor2>(), "second");
        }

        protected override void PostStop() => Console.WriteLine("first stopped");

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case "stop":
                    Context.Stop(Self);
                    break;
            }
        }
    }

    public class StartStopActor2 : UntypedActor
    {
        protected override void PreStart() => Console.WriteLine("second started");
        protected override void PostStop() => Console.WriteLine("second stopped");

        protected override void OnReceive(object message)
        {
        }
    }
}
