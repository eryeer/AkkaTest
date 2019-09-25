using System;
using System.Collections.Generic;
using System.Text;

namespace Study.IOT.Model
{
    public sealed class ReadTemperature
    {
        public ReadTemperature(long requestId) {
            RequestId = requestId;
        }

        public long RequestId { get; }
    }
}
