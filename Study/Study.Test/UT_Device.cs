using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.Xunit2;
using FluentAssertions;
using Study.IOT.Actor;
using Study.IOT.Model;
using Xunit;

namespace Study.Test
{
    public class UT_Device
    {
        [Fact]
        public void Device_actor_must_reply_with_empty_reading_if_no_temperature_is_known()
        {
            var sys = ActorSystem.Create("mysys");
            var probe = new TestKit().CreateTestProbe();
            var deviceActor = sys.ActorOf(Device.Props("group", "device"));

            deviceActor.Tell(new ReadTemperature(requestId: 42), probe.Ref);
            var response = probe.ExpectMsg<RespondTemperature>();
            response.RequestId.Should().Be(42);
            response.Value.Should().BeNull();
        }

        [Fact]
        public void Device_actor_must_reply_with_latest_temperature_reading()
        {
            var sys = ActorSystem.Create("mysys");
            var probe = new TestKit().CreateTestProbe();
            var deviceActor = sys.ActorOf(Device.Props("group", "device"));

            deviceActor.Tell(new RecordTemperature(requestId: 1, value: 24.0), probe.Ref);
            probe.ExpectMsg<TemperatureRecorded>(s => s.RequestId == 1);

            deviceActor.Tell(new ReadTemperature(requestId: 2), probe.Ref);
            var response1 = probe.ExpectMsg<RespondTemperature>();
            response1.RequestId.Should().Be(2);
            response1.Value.Should().Be(24.0);

            deviceActor.Tell(new RecordTemperature(requestId: 3, value: 55.0), probe.Ref);
            probe.ExpectMsg<TemperatureRecorded>(s => s.RequestId == 3);

            deviceActor.Tell(new ReadTemperature(requestId: 4), probe.Ref);
            var response2 = probe.ExpectMsg<RespondTemperature>();
            response2.RequestId.Should().Be(4);
            response2.Value.Should().Be(55.0);
        }

        [Fact]
        public void Device_actor_must_reply_to_registration_requests()
        {
            var sys = ActorSystem.Create("mysys");
            var probe = new TestKit().CreateTestProbe();
            var deviceActor = sys.ActorOf(Device.Props("group", "device"));
            deviceActor.Tell(new RequestTrackDevice("group", "device"),probe.Ref);
            probe.ExpectMsg<DeviceRegistered>();
            probe.LastSender.Should().Be(deviceActor);
        }

        [Fact]
        public void Device_actor_must_ignore_wrong_registration_requests()
        {
            var sys = ActorSystem.Create("mysys");
            var probe = new TestKit().CreateTestProbe();
            var deviceActor = sys.ActorOf(Device.Props("group", "device"));

            deviceActor.Tell(new RequestTrackDevice("wrongGroup", "device"), probe.Ref);
            probe.ExpectNoMsg(TimeSpan.FromMilliseconds(500));

            deviceActor.Tell(new RequestTrackDevice("group", "Wrongdevice"), probe.Ref);
            probe.ExpectNoMsg(TimeSpan.FromMilliseconds(500));
        }
    }
}
