using System;
using System.Collections.Generic;
using System.Text;

namespace Study
{
    public class RefStudy
    {
        internal class Person
        {
            public string name;

            public override string ToString() {
                return $"person name = {name}";
            }
        }
        static void Main01(string[] args)
        {
            ulong i = 1;
            ulong j = 2;
            var ret = i - j;
            Console.WriteLine(ret);
            //NewMethod();
            Console.ReadLine();
        }

        private static void NewMethod()
        {
            var person1 = new Person
            {
                name = "tom"
            };

            ref var person2 = ref person1;

            person1 = new Person
            {
                name = "jerry"
            };
            Console.WriteLine(person1.ToString());
            Console.WriteLine(person2.ToString());
        }
    }
    
}
