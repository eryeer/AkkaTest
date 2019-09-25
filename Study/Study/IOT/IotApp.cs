using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Study.IOT
{
    public class IotApp
    {
        static void Main(string[] args)
        {
            Init();
        }

        public static void Init()
        {
            using (var system = ActorSystem.Create("iot-system"))
            {
                var supervisor = system.ActorOf(IotSupervisor.Props(),"iot-supervisor");
                Console.ReadLine();
            }

        }
    }
}
