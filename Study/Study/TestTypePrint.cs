using System;
using System.Collections.Generic;
using System.Text;

namespace Study
{
    public class TestTypePrint
    {
        static void Main01(string[] args)
        {
            TypeMessage msg = new TypeMessage();
            Console.WriteLine(msg);
            Console.WriteLine(msg.GetType());
            Console.ReadLine();
        }
    }

    public class TypeMessage { }
}
