using Akka.Actor;
using Akka.Event;
using Study.IOT.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Study.IOT.Actor
{
    public class DeviceGroup : UntypedActor
    {
        private readonly Dictionary<string, IActorRef> deviceIdToActor = new Dictionary<string, IActorRef>();
        private readonly Dictionary<IActorRef, string> ActorToDeviceId = new Dictionary<IActorRef, string>();

        protected ILoggingAdapter Log { get; } = Context.GetLogger();
        protected string GroupId { get; }

        public DeviceGroup(string groupId)
        {
            GroupId = groupId;
        }
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RequestAllTemperatures r:
                    Context.ActorOf(DeviceGroupQuery.Props(ActorToDeviceId, r.RequestId, Sender, TimeSpan.FromSeconds(3)));
                    break;
                case RequestTrackDevice trackMsg when trackMsg.GroupId.Equals(GroupId):
                    if (deviceIdToActor.TryGetValue(trackMsg.DeviceId, out var actorRef))
                    {
                        actorRef.Forward(trackMsg);
                    }
                    else
                    {
                        Log.Info($"Creating device actor for {trackMsg.DeviceId}");
                        var deviceActor = Context.ActorOf(Device.Props(trackMsg.GroupId, trackMsg.DeviceId), $"device-{trackMsg.DeviceId}");
                        Context.Watch(deviceActor);
                        ActorToDeviceId.Add(deviceActor,trackMsg.DeviceId);
                        deviceIdToActor.Add(trackMsg.DeviceId, deviceActor);
                        deviceActor.Forward(trackMsg);
                    }
                    break;
                case RequestTrackDevice trackMsg:
                    Log.Warning($"Ignoring TrackDevice request for {trackMsg.GroupId}. This actor is responsible for {GroupId}.");
                    break;
                case RequestDeviceList deviceList:
                    Sender.Tell(new ReplyDeviceList(deviceList.RequestId, new HashSet<string>(deviceIdToActor.Keys)));
                    break;
                case Terminated t:
                    var deviceId = ActorToDeviceId[t.ActorRef];
                    Log.Info($"Device actor for {deviceId} has been terminated");
                    ActorToDeviceId.Remove(t.ActorRef);
                    deviceIdToActor.Remove(deviceId);
                    break;
            }
        }
        public static Props Props(string groupId) => Akka.Actor.Props.Create(() => new DeviceGroup(groupId));
    }

    public sealed class RequestDeviceList
    {
        public long RequestId { get; }
        public RequestDeviceList(long requestId)
        {
            RequestId = requestId;
        }
    }

    public sealed class ReplyDeviceList
    {
        public long RequestId { get; }
        public ISet<string> Ids { get; }
        public ReplyDeviceList(long requestId, ISet<string> ids)
        {
            RequestId = requestId;
            Ids = ids;
        }
    }

    public sealed class Temperature : ITemperatureReading
    {
        public Temperature(double value)
        {
            Value = value;
        }

        public double Value { get; }
    }

    public sealed class TemperatureNotAvailable : ITemperatureReading
    {
        public static TemperatureNotAvailable Instance { get; } = new TemperatureNotAvailable();
        private TemperatureNotAvailable() { }
    }

    public sealed class DeviceNotAvailable : ITemperatureReading
    {
        public static DeviceNotAvailable Instance { get; } = new DeviceNotAvailable();
        private DeviceNotAvailable() { }
    }

    public sealed class DeviceTimedOut : ITemperatureReading
    {
        public static DeviceTimedOut Instance { get; } = new DeviceTimedOut();
        private DeviceTimedOut() { }
    }
}
