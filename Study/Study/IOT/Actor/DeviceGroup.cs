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
}
