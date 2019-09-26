﻿using Akka.Actor;
using Akka.Event;
using Study.IOT.Model;
using System.Collections.Generic;

namespace Study.IOT.Actor
{
    public class DeviceManager : UntypedActor
    {
        private readonly Dictionary<string, IActorRef> groupIdToActor = new Dictionary<string, IActorRef>();
        private readonly Dictionary<IActorRef, string> actorToGroupId = new Dictionary<IActorRef, string>();

        protected override void PreStart() => Log.Info("DeviceManager started");
        protected override void PostStop() => Log.Info("DeviceManager stopped");

        protected ILoggingAdapter Log { get; } = Context.GetLogger();

        protected override void OnReceive(object message)
        {
            switch(message)
            {
                case RequestTrackDevice trackMsg:
                    if (groupIdToActor.TryGetValue(trackMsg.GroupId, out var actorRef))
                    {
                        actorRef.Forward(trackMsg);
                    }
                    else
                    {
                        Log.Info($"Creating device group actor for {trackMsg.GroupId}");
                        var groupActor = Context.ActorOf(DeviceGroup.Props(trackMsg.GroupId));
                        Context.Watch(groupActor);
                        groupIdToActor.Add(trackMsg.GroupId, groupActor);
                        actorToGroupId.Add(groupActor, trackMsg.GroupId);
                        groupActor.Forward(trackMsg);
                    }
                    break;
                case Terminated t:
                    var groupId = actorToGroupId[t.ActorRef];
                    Log.Info($"Device group actor for {groupId} has been terminated");
                    actorToGroupId.Remove(t.ActorRef);
                    groupIdToActor.Remove(groupId);
                    break;
            }
        }

        public static Props Props() => Akka.Actor.Props.Create<DeviceManager>();
    }
}
