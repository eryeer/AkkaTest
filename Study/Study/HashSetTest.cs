using System;
using System.Collections.Generic;
using System.Text;

namespace Study
{
    public class HashSetTest
    {
        static void Main01(string[] args)
        {
            HashSet<int> set1 = new HashSet<int>()
            { 1,2,3 };
            HashSet<int> set2 = new HashSet<int>()
            { 2,3,4 };

            set1.ExceptWith(set2);
            foreach (var i in set1)
            {
                Console.Write(i+",");
            }
            Console.WriteLine();
            foreach (var i in set2)
            {
                Console.Write(i + ",");
            }
            Console.WriteLine();
            Console.Read();
        }
    }
}
