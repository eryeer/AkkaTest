using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Study
{
    public class QueueStudy
    {
        static void Main01(string[] args)
        {
            var queue = new Queue<int>(5);
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);
            queue.Enqueue(4);
            var first = queue.Dequeue();
            var second = queue.Dequeue();
            var third = queue.Dequeue();
            Console.WriteLine(first);
            Console.WriteLine(second);
            Console.WriteLine(third);
            Console.ReadLine();
        }
    }
}
