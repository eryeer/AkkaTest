using Akka.Actor;
using Akka.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Study.LocalActorTest
{
    public class ReceiverActor : UntypedActor
    {
        public ILoggingAdapter Log { get; } = Context.GetLogger();
        public int count = 0;
        public double timespanEachSum = 0;
        public static System.Diagnostics.Stopwatch stopwatchEach = new System.Diagnostics.Stopwatch();
        public static System.Diagnostics.Stopwatch stopwatchTotal = new System.Diagnostics.Stopwatch();

        protected override void PreStart() => Log.Info("ReceiverActor Application started");
        protected override void PostStop() => Log.Info("ReceiverActor Application stopped");

        protected override void OnReceive(object message)
        {
            if (count == 0)
            {
                stopwatchTotal.Start();
            }
            stopwatchEach.Start();
            Message myMsg = null;
            switch (message)
            {
                case Message msg:
                    myMsg = msg;
                    for (int i = 0; i < 1000000000; i++)
                    { }
                    count++;
                    break;
            }
            stopwatchEach.Stop();
            var timespanEach = stopwatchEach.Elapsed.TotalSeconds;
            timespanEachSum += timespanEach;
            stopwatchEach.Reset();
            Console.WriteLine($"each msg cosume time = {timespanEach}");
            if (count == myMsg.Number)
            {
                stopwatchTotal.Stop();
                var timespanTotal = stopwatchTotal.Elapsed.TotalSeconds;
                Console.WriteLine($"**Total msg cosume time = {timespanTotal}");
                Console.WriteLine($"***timeSpanSum time = {timespanEachSum}");
                stopwatchTotal.Reset();
                count = 0;
                timespanEachSum = 0;
            }
        }
        public static Props Props() => Akka.Actor.Props.Create<ReceiverActor>();
    }
}
