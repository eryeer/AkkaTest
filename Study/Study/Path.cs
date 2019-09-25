using Akka.Actor;
using System;

namespace Study
{
    public class Path
    {
        static void Main01(string[] args)
        {
            var sys = ActorSystem.Create("MySystem");
            var firstRef = sys.ActorOf(Props.Create<PrintMyActorRefActor>(), "first-actor");
            Console.WriteLine($"First: {firstRef}");
            firstRef.Tell("printit", ActorRefs.NoSender);
            Console.ReadLine();
        }
    }

    public class PrintMyActorRefActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case "printit":
                    IActorRef secondRef = Context.ActorOf(Props.Empty, "second-actor");
                    Console.WriteLine($"Second: {secondRef}");
                    break;
            }
        }
    }
}
