using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Study
{
    public class ErrorHandle
    {
        static void Main01(string[] args)
        {
            var sys = ActorSystem.Create("MySystem");
            var firstRef = sys.ActorOf(Props.Create<SupervisedActor>(), "supervising-actor");
            firstRef.Tell("fail");
            Console.ReadLine();
        }
    }

    public class SupervisingActor : UntypedActor
    {
        private IActorRef child = Context.ActorOf(Props.Create<SupervisedActor>(), "supervised-actor");

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case "failChild":
                    child.Tell("fail");
                    break;
            }
        }
    }

    public class SupervisedActor : UntypedActor
    {
        protected override void PreStart() => Console.WriteLine("supervised actor started");
        protected override void PreRestart(Exception reason, object message)
        {
            Console.WriteLine($"supervised actor start to restart,{reason.Message},{message.ToString()}");
        }
        protected override void PostStop() => Console.WriteLine("supervised actor stopped");
        protected override void PostRestart(Exception reason)
        {
            Console.WriteLine("supervised actor finish restart");
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case "fail":
                    Console.WriteLine("supervised actor fails now");
                    throw new Exception("I failed!");
            }
        }
    }
}
