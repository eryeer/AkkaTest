﻿using System.Collections.Generic;

namespace Study.IOT.Model
{
    public sealed class RespondAllTemperatures
    {
        public RespondAllTemperatures(long requestId, Dictionary<string, ITemperatureReading> temperatures)
        {
            RequestId = requestId;
            Temperatures = temperatures;
        }

        public long RequestId { get; }
        public Dictionary<string, ITemperatureReading> Temperatures { get; }
    }
}
