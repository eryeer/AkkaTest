using Akka.Actor;
using Akka.Configuration;
using Akka.Dispatch;
using Akka.Dispatch.MessageQueues;
using Akka.Event;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Study.MailBoxTest
{
    public class Test
    {
        static void Main01(string[] args)
        {
            ActorSystem system = ActorSystem.Create("mysystem",
            $"custom-mailbox {{ mailbox-type: \"{typeof(CustomMailbox).AssemblyQualifiedName}\" }}");
            var receiver1 = system.ActorOf(Receiver.Props());
            var receiver2 = system.ActorOf(Receiver.Props());
            for (int i = 0; i<= 10;i++)
            {
                receiver1.Tell(new MyMessage());
                receiver2.Tell(new MyMessage());
            }

            Console.ReadLine();
        }
    }

    public class Receiver : UntypedActor
    {
        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new Receiver()).WithMailbox("custom-mailbox");
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case MyMessage msg:
                    for (int i = 0; i < 1000000000; i++)
                    { }
                    break;
                default:
                    throw new Exception();

            }
        }
    }

    internal class MyMessage {}


    internal class CustomMailbox : MailboxType, IProducesMessageQueue<CustomMessageQueue>
    {
        public CustomMailbox(Settings settings, Config config) : base(settings, config)
        {
        }

        public override IMessageQueue Create(IActorRef owner, ActorSystem system)
        {
            return new CustomMessageQueue();
        }
    }

    internal class CustomMessageQueue : IMessageQueue, IUnboundedMessageQueueSemantics
    {
        private readonly ConcurrentQueue<Envelope> queue = new ConcurrentQueue<Envelope>();

        public bool HasMessages => queue.IsEmpty;

        public int Count => queue.Count;

        public void CleanUp(IActorRef owner, IMessageQueue deadletters)
        {
            foreach (Envelope envelope in queue)
            {
                deadletters.Enqueue(owner, envelope);
            }
        }

        public void Enqueue(IActorRef receiver, Envelope envelope)
        {
            queue.Enqueue(envelope);
            Console.WriteLine($"Enqueued. QueueCount: {Count}. Time: {DateTime.Now.ToString()}");
            
        }

        public bool TryDequeue(out Envelope envelope)
        {
            if (queue.TryDequeue(out envelope))
            {
                Console.WriteLine($"Dequeued. QueueCount: {Count}. Time: {DateTime.Now.ToString()}");
                return true;
            }
            return false;
        }
    }
}
