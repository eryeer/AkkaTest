using System;
using Xunit;

namespace Study.Test
{
    public class UnitTest1
    {
        public int Add(int a, int b)
        { return a + b; }

        [Fact]
        public void Test1()
        {
            Assert.Equal(4, Add(2, 2));
        }


    }
}
