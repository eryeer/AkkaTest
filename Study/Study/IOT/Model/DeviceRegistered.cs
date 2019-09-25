using System;
using System.Collections.Generic;
using System.Text;

namespace Study.IOT.Model
{
    public class DeviceRegistered
    {
        public static DeviceRegistered Instance { get; } = new DeviceRegistered();
        private DeviceRegistered() { }
    }
}
