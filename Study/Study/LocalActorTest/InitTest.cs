using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Study.LocalActorTest
{
    public class InitTest
    {
        public static IActorRef receiver;
        static void Main01(string[] args)
        {
            //Init();
            InitMultiThread();
        }

        private static void InitMultiThread()
        {
            using (var system = ActorSystem.Create("Actor-system"))
            {
                receiver = system.ActorOf(ReceiverActor.Props(), "actor-supervisor");
                string numStr = "";
                Thread t1 = new Thread(new ParameterizedThreadStart(SendMsg))
                {
                    IsBackground = true
                };
                Thread t2 = new Thread(new ParameterizedThreadStart(SendMsg))
                {
                    IsBackground = true
                };
                while (true)
                {
                    Console.WriteLine("enter the msg num");
                    numStr = Console.ReadLine();
                    if (numStr.Equals("q"))
                    {
                        break;
                    }
                    else
                    {
                        int num = int.Parse(numStr);
                        t1.Start(num);
                        t2.Start(num);
                    }
                }
            }
        }

        public static void SendMsg(object numObj)
        {
            int num = (int)numObj;
            for (int i = 0; i < num; i++)
            {
                receiver.Tell(new Message(num*2));
            }
        }

        public static void Init()
        {
            using (var system = ActorSystem.Create("Actor-system"))
            {
                var receiver = system.ActorOf(ReceiverActor.Props(),"actor-supervisor");
                string numStr = "";
                while (true)
                {
                    Console.WriteLine("enter the msg num");
                    numStr = Console.ReadLine();
                    if (numStr.Equals("q"))
                    {
                        break;
                    }
                    else
                    {
                        int num = int.Parse(numStr);
                        for (int i = 0; i < num; i++)
                        {
                            receiver.Tell(new Message(num));
                        }
                    }
                }
            }
        }
    }
}
