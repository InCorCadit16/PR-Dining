using Dining.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dining.Infrastructure.Utils
{
    class Assessor
    {
        private static float total = 0;
        private static int count = 0;

        public static void Assess(float realTime, Order order)
        {
            int rating;
            if (realTime < order.MaxWaitTime)
                rating = 5;
            else if (realTime <= order.MaxWaitTime * 1.1)
                rating = 4;
            else if (realTime <= order.MaxWaitTime * 1.2)
                rating = 3;
            else if (realTime <= order.MaxWaitTime * 1.3)
                rating = 2;
            else if (realTime <= order.MaxWaitTime * 1.4)
                rating = 1;
            else
                rating = 0;


            Logger.Log($"Order {order.Id} has rating {rating} (Waited for {realTime}. Max is {order.MaxWaitTime})");

            count++;
            total += rating;
            Logger.Log($"\nStats:\nTotal number of orders: {count}\nAverage rating: {(total/count):f2}\n");
        }
    }
}
