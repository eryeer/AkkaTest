using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Study
{
    public class TimeWatch
    {
        static void Main01(string[] args)
        {
            var wat = new Stopwatch();
            for (int i = 0; i < 2; i++)
            {
                wat.Start();
                Thread.Sleep(TimeSpan.FromMilliseconds(500));
                //int a = 0;
                //for (int i = 0; i < 1000; i++)
                //{
                //    a++;
                //}
                wat.Stop();
            }
            Console.WriteLine("time is {0}", wat.ElapsedMilliseconds);
            Console.WriteLine("time is {0}", wat.Elapsed.TotalSeconds);
            Console.WriteLine("time is {0}", wat.ElapsedTicks);
            Console.ReadLine();
        }
    }
}
