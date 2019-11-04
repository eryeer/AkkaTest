using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Study
{
    public class AtomicTest
    {
        static void Main01(string[] args)
        {
            double initialValue, computedValue;
            double totalTimeVersionPayload = 1;
            do
            {
                initialValue = totalTimeVersionPayload;
                computedValue = initialValue + 2;
            }
            while (initialValue != Interlocked.CompareExchange(ref totalTimeVersionPayload, computedValue, initialValue));
            Console.Read();
        }
    }
}
