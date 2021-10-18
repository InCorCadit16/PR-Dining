using System;
using System.Collections.Generic;
using System.Text;

namespace Dining.Infrastructure.Utils
{
    public class RandomGenerator
    {
        

        private static Random random = new Random(DateTime.Now.Millisecond);

        public static int GenerateTime(int max, int min = 0)
        {
            return random.Next(min * Values.TIME_UNIT, max * Values.TIME_UNIT);
        }

        public static int GenerateNumber(int max, int min = 1)
        {
            return random.Next(min, max + 1);
        }
    }
}
