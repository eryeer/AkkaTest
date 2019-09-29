using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Study
{
    public class Strategy
    {
        static void Main01(string[] args)
        {
            var sys = ActorSystem.Create("MySystem");
            var firstRef = sys.ActorOf(Props.Create<SupervisingActor>(), "supervising-actor");
            firstRef.Tell("failChild");
            Console.ReadLine();
        }

        public class SupervisingActor : UntypedActor
        {
            private IActorRef child = Context.ActorOf(Props.Create<SupervisedActor>(), "supervised-actor");

            protected override void OnReceive(object message)
            {
                switch (message)
                {
                    case "failChild":
                        child.Tell("ArgumentException");
                        break;
                }
            }

            protected override SupervisorStrategy SupervisorStrategy()
            {
                return new OneForOneStrategy(
                maxNrOfRetries: 10,
                withinTimeMilliseconds: 60000,
                localOnlyDecider: ex =>
                {
                    switch (ex)
                    {
                        case ArithmeticException ae:
                            Console.WriteLine("ArithmeticException, Supervisor is doning Resume");
                            return Directive.Resume;
                        case NullReferenceException nre:
                            Console.WriteLine("NullReferenceException, Supervisor is doning Restart");
                            return Directive.Restart;
                        case ArgumentException are:
                            Console.WriteLine("ArgumentException, Supervisor is doning Stop");
                            return Directive.Stop;
                        default:
                            return Directive.Escalate;
                    }
                },
                loggingEnabled:false);
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
                    case "ArithmeticException":
                        Console.WriteLine("~ArithmeticException, supervised actor fails now");
                        throw new ArithmeticException("I failed!");
                    case "NullReferenceException":
                        Console.WriteLine("~NullReferenceException, supervised actor fails now");
                        throw new NullReferenceException("I failed!");
                    case "ArgumentException":
                        Console.WriteLine("~ArgumentException, supervised actor fails now");
                        throw new ArgumentException("I failed!");
                }
            }

        }
    }
}
