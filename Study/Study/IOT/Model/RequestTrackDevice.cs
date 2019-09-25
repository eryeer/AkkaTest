using System;
using System.Collections.Generic;
using System.Text;

namespace Study.IOT.Model
{
    public class RequestTrackDevice
    {
        public RequestTrackDevice(string groupId, string deviceId)
        {
            GroupId = groupId;
            DeviceId = deviceId;
        }

        public string GroupId { get; }
        public string DeviceId { get; }
    }
}
