﻿using Akka.Actor;
using Akka.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Study.IOT
{
    public class IotSupervisor : UntypedActor
    {
        public ILoggingAdapter Log { get; } = Context.GetLogger();

        protected override void PreStart() => Log.Info("IoT Application started");
        protected override void PostStop() => Log.Info("IoT Application stopped");

        protected override void OnReceive(object message)
        {
        }
        public static Props Props() => Akka.Actor.Props.Create<IotSupervisor>();
    }
}
