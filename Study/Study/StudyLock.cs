using System;
using System.Collections.Generic;
using System.Text;

namespace Study
{
    public class StudyLock
    {
        static void Main(string[] args)
        {
            Test();

            Console.ReadLine();
        }

        static int Test()
        {
            try
            {
                Console.WriteLine("in try");
            }
            finally
            {
                Console.WriteLine("in finally");
            }
            Console.WriteLine("act return");
            return 0;
        }

    }
}
