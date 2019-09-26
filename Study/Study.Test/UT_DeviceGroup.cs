using Akka.Actor;
using Akka.TestKit.Xunit2;
using Akka.Util.Internal;
using FluentAssertions;
using Study.IOT.Actor;
using Study.IOT.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Study.Test
{
    public class UT_DeviceGroup
    {
        [Fact]
        public void DeviceGroup_actor_must_be_able_to_register_a_device_actor()
        {
            var sys = ActorSystem.Create("mysys");
            var probe = new TestKit().CreateTestProbe();
            var groupActor = sys.ActorOf(DeviceGroup.Props("group"));

            groupActor.Tell(new RequestTrackDevice("group", "device1"), probe.Ref);
            probe.ExpectMsg<DeviceRegistered>();
            var deviceActor1 = probe.LastSender;

            groupActor.Tell(new RequestTrackDevice("group", "device2"), probe.Ref);
            probe.ExpectMsg<DeviceRegistered>();
            var deviceActor2 = probe.LastSender;
            deviceActor1.Should().NotBe(deviceActor2);

            // Check that the device actors are working
            deviceActor1.Tell(new RecordTemperature(requestId: 0, value: 1.0), probe.Ref);
            probe.ExpectMsg<TemperatureRecorded>(s => s.RequestId == 0);
            deviceActor2.Tell(new RecordTemperature(requestId: 1, value: 2.0), probe.Ref);
            probe.ExpectMsg<TemperatureRecorded>(s => s.RequestId == 1);
        }

        [Fact]
        public void DeviceGroup_actor_must_ignore_requests_for_wrong_groupId()
        {
            var sys = ActorSystem.Create("mysys");
            var probe = new TestKit().CreateTestProbe();
            var groupActor = sys.ActorOf(DeviceGroup.Props("group"));

            groupActor.Tell(new RequestTrackDevice("wrongGroup", "device1"), probe.Ref);
            probe.ExpectNoMsg(TimeSpan.FromMilliseconds(500));
        }

        [Fact]
        public void DeviceGroup_actor_must_return_same_actor_for_same_deviceId()
        {
            var sys = ActorSystem.Create("mysys");
            var probe = new TestKit().CreateTestProbe();
            var groupActor = sys.ActorOf(DeviceGroup.Props("group"));

            groupActor.Tell(new RequestTrackDevice("group", "device1"), probe.Ref);
            probe.ExpectMsg<DeviceRegistered>();
            var deviceActor1 = probe.LastSender;

            groupActor.Tell(new RequestTrackDevice("group", "device1"), probe.Ref);
            probe.ExpectMsg<DeviceRegistered>();
            var deviceActor2 = probe.LastSender;

            deviceActor1.Should().Be(deviceActor2);
        }

        [Fact]
        public void DeviceGroup_actor_must_be_able_to_list_active_devices()
        {
            var sys = ActorSystem.Create("mysys");
            var probe = new TestKit().CreateTestProbe();
            var groupActor = sys.ActorOf(DeviceGroup.Props("group"));

            groupActor.Tell(new RequestTrackDevice("group","device1"),probe.Ref);
            probe.ExpectMsg<DeviceRegistered>();
            var device1 = probe.LastSender;

            groupActor.Tell(new RequestTrackDevice("group", "device2"), probe.Ref);
            probe.ExpectMsg<DeviceRegistered>();
            var device2 = probe.LastSender;

            groupActor.Tell(new RequestDeviceList(requestId:0), probe.Ref);
            probe.ExpectMsg<ReplyDeviceList>(rep=>rep.RequestId == 0 
            && rep.Ids.Contains("device1")&& rep.Ids.Contains("device2"));
        }

        [Fact]
        public void DeviceGroup_actor_must_be_able_to_list_active_devices_after_one_shuts_down()
        {
            var sys = ActorSystem.Create("mysys");
            var probe = new TestKit().CreateTestProbe();
            var groupActor = sys.ActorOf(DeviceGroup.Props("group"));
            groupActor.Tell(new RequestTrackDevice("group", "device1"), probe.Ref);
            probe.ExpectMsg<DeviceRegistered>();
            var toShutDown = probe.LastSender;

            groupActor.Tell(new RequestTrackDevice("group", "device2"), probe.Ref);
            probe.ExpectMsg<DeviceRegistered>();

            groupActor.Tell(new RequestDeviceList(requestId: 0), probe.Ref);
            probe.ExpectMsg<ReplyDeviceList>(s => s.RequestId == 0
                                                  && s.Ids.Contains("device1")
                                                  && s.Ids.Contains("device2"));

            probe.Watch(toShutDown);
            toShutDown.Tell(PoisonPill.Instance);
            probe.ExpectTerminated(toShutDown);

            // using awaitAssert to retry because it might take longer for the groupActor
            // to see the Terminated, that order is undefined
            probe.AwaitAssert(() =>
            {
                groupActor.Tell(new RequestDeviceList(requestId: 1), probe.Ref);
                probe.ExpectMsg<ReplyDeviceList>(s => s.RequestId == 1 && s.Ids.Contains("device2")
                && !s.Ids.Contains("device1"));
            });
        }

        [Fact]
        public void DeviceGroup_actor_must_be_able_to_collect_temperatures_from_all_active_devices()
        {
            var sys = ActorSystem.Create("mysys");
            var probe = new TestKit().CreateTestProbe();
            var groupActor = sys.ActorOf(DeviceGroup.Props("group"));

            groupActor.Tell(new RequestTrackDevice("group", "device1"), probe.Ref);
            probe.ExpectMsg<DeviceRegistered>();
            var deviceActor1 = probe.LastSender;

            groupActor.Tell(new RequestTrackDevice("group", "device2"), probe.Ref);
            probe.ExpectMsg<DeviceRegistered>();
            var deviceActor2 = probe.LastSender;

            groupActor.Tell(new RequestTrackDevice("group", "device3"), probe.Ref);
            probe.ExpectMsg<DeviceRegistered>();
            var deviceActor3 = probe.LastSender;

            // Check that the device actors are working
            deviceActor1.Tell(new RecordTemperature(requestId: 0, value: 1.0), probe.Ref);
            probe.ExpectMsg<TemperatureRecorded>(s => s.RequestId == 0);
            deviceActor2.Tell(new RecordTemperature(requestId: 1, value: 2.0), probe.Ref);
            probe.ExpectMsg<TemperatureRecorded>(s => s.RequestId == 1);
            // No temperature for device3

            groupActor.Tell(new RequestAllTemperatures(0), probe.Ref);
            probe.ExpectMsg<RespondAllTemperatures>(msg =>
              msg.Temperatures["device1"].AsInstanceOf<Temperature>().Value == 1.0 &&
              msg.Temperatures["device2"].AsInstanceOf<Temperature>().Value == 2.0 &&
              msg.Temperatures["device3"] is TemperatureNotAvailable &&
              msg.RequestId == 0);
        }
    }

}
