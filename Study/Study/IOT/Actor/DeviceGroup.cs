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
                        deviceIdToActor.Add(trackMsg.DeviceId, deviceActor);
                        deviceActor.Forward(trackMsg);
                    }
                    break;
                case RequestTrackDevice trackMsg:
                    Log.Warning($"Ignoring TrackDevice request for {trackMsg.GroupId}. This actor is responsible for {GroupId}.");
                    break;
            }
        }

        public static Props Props(string groupId) => Akka.Actor.Props.Create(() => new DeviceGroup(groupId));
    }
}
