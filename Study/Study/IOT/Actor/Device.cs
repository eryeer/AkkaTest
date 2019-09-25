using Akka.Actor;
using Akka.Event;
using Study.IOT.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Study.IOT.Actor
{
    public class Device : UntypedActor
    {

        private double? _lastTemperatureReading = null;

        protected ILoggingAdapter Log { get; } = Context.GetLogger();
        protected string GroupId { get; }
        protected string DeviceId { get; }

        protected override void PreStart() => Log.Info($"Device actor {GroupId}-{DeviceId} started");
        protected override void PostStop() => Log.Info($"Device actor {GroupId}-{DeviceId} stopped");

        public Device(string groupId, string deviceId)
        {
            GroupId = groupId;
            DeviceId = deviceId;
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case ReadTemperature read:
                    Sender.Tell(new RespondTemperature(read.RequestId, _lastTemperatureReading));
                    break;
                case RecordTemperature rec:
                    Log.Info($"Recorded temperature reading {rec.Value} with {rec.RequestId}");
                    _lastTemperatureReading = rec.Value;
                    Sender.Tell(new TemperatureRecorded(rec.RequestId));
                    break;
            }
        }

        public static Props Props(string grouId, string deviceId) => 
            Akka.Actor.Props.Create(() => new Device(grouId, deviceId));
    }

    //ack
    public sealed class TemperatureRecorded
    {
        public TemperatureRecorded(long requestId)
        {
            RequestId = requestId;
        }

        public long RequestId { get; }
    }
}
