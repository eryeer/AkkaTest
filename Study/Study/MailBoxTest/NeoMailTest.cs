using Akka.Actor;
using Akka.Configuration;
using Akka.Dispatch;
using Akka.Dispatch.MessageQueues;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Study.MailBoxTest
{
    public class NeoMailTest
    {
        static void Main(string[] args)
        {
            ActorSystem system = ActorSystem.Create("mysystem",
            $"protocol-handler-mailbox {{ mailbox-type: \"{typeof(PriorityMailbox).AssemblyQualifiedName}\" }}");
            var protocol = system.ActorOf(ProtocolHandler.Props());
            for (int i = 0; i < 10; i++)
            {
                protocol.Tell(Message.Create(MessageCommand.GetData));
            }
            Console.ReadLine();
        }
    }

    public class ProtocolHandler : UntypedActor
    {
        public int countIdle = 0;
        public int countGetData = 0;

        protected override void OnReceive(object message)
        {
            if (!(message is Message msg))
            {
                countIdle++;
                Console.WriteLine($"Message Type: {message.GetType()}, count: {countIdle}");
                return;
            }
            switch (msg.Command)
            {
                case MessageCommand.GetData:
                    countGetData++;
                    Console.WriteLine($"Message Type: GetData, count: {countGetData}");
                    break;
                default:
                    throw new Exception();
            }
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new ProtocolHandler()).WithMailbox("protocol-handler-mailbox");
        }
    }

    internal class PriorityMailbox : MailboxType, IProducesMessageQueue<PriorityMessageQueue>
    {
        public PriorityMailbox(Settings settings, Config config)
            : base(settings, config)
        {
        }

        public override IMessageQueue Create(IActorRef owner, ActorSystem system)
        {
            return new PriorityMessageQueue(ShallDrop, IsHighPriority);
        }

        internal protected bool IsHighPriority(object message)
        {
            if (!(message is Message msg)) return false;
            switch (msg.Command)
            {
                case MessageCommand.Consensus:
                case MessageCommand.FilterAdd:
                case MessageCommand.FilterClear:
                case MessageCommand.FilterLoad:
                case MessageCommand.Verack:
                case MessageCommand.Version:
                case MessageCommand.Alert:
                    return true;
                default:
                    return false;
            }
        }
        internal protected bool ShallDrop(object message, IEnumerable queue)
        {
            if (!(message is Message msg)) return true;
            switch (msg.Command)
            {
                case MessageCommand.GetAddr:
                case MessageCommand.GetBlocks:
                //case MessageCommand.GetData:
                case MessageCommand.GetHeaders:
                case MessageCommand.Mempool:
                    return queue.OfType<Message>().Any(p => p.Command == msg.Command);
                default:
                    return false;
            }
        }
    }

    internal class PriorityMessageQueue : IMessageQueue, IUnboundedMessageQueueSemantics
    {
        private readonly ConcurrentQueue<Envelope> high = new ConcurrentQueue<Envelope>();
        private readonly ConcurrentQueue<Envelope> low = new ConcurrentQueue<Envelope>();
        private readonly Func<object, IEnumerable, bool> dropper;
        private readonly Func<object, bool> priority_generator;
        private int idle = 1;

        public bool HasMessages => !high.IsEmpty || !low.IsEmpty;
        public int Count => high.Count + low.Count;

        public PriorityMessageQueue(Func<object, IEnumerable, bool> dropper, Func<object, bool> priority_generator)
        {
            this.dropper = dropper;
            this.priority_generator = priority_generator;
        }

        public void CleanUp(IActorRef owner, IMessageQueue deadletters)
        {
        }

        public void Enqueue(IActorRef receiver, Envelope envelope)
        {
            Interlocked.Increment(ref idle);
            if (envelope.Message is Idle) return;
            if (dropper(envelope.Message, high.Concat(low).Select(p => p.Message)))
            {
                Console.WriteLine($"Message Droped Queue Count: {Count}");
                return;
            }
            ConcurrentQueue<Envelope> queue = priority_generator(envelope.Message) ? high : low;
            queue.Enqueue(envelope);
        }

        public bool TryDequeue(out Envelope envelope)
        {
            if (high.TryDequeue(out envelope)) return true;
            if (low.TryDequeue(out envelope)) return true;
            if (Interlocked.Exchange(ref idle, 0) > 0)
            {
                envelope = new Envelope(Idle.Instance, ActorRefs.NoSender);
                return true;
            }
            return false;
        }
    }

    internal sealed class Idle
    {
        public static Idle Instance { get; } = new Idle();
    }

    internal class Message
    {
        public MessageCommand Command;

        public static Message Create(MessageCommand command)
        {
            Message message = new Message
            {
                Command = command
            };
            return message;
        }
    }
}
