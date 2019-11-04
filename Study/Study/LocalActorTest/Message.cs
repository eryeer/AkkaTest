using System;
using System.Collections.Generic;
using System.Text;

namespace Study.LocalActorTest
{
    public class Message
    {
        public int Number { set; get; }

        public Message(int num)
        {
            Number = num;
        }
    }
}
