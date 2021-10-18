using Dining.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dining.Models
{
    public class Table
    {
        private static ObjectIDGenerator idGenerator = new ObjectIDGenerator();

        public long Id { get; private set; }
        public TableState State;
        public DateTime orderedAt;

        public Table()
        {
            Id = idGenerator.GetId(this, out bool firstTime);
            State = TableState.NoClient;
        }

        public void GetClients()
        {
            Task.Run(() =>
            {
                var time = RandomGenerator.GenerateTime(Values.TABLE_WAIT_FOR_CLIENTS_MAX);
                Thread.Sleep(time);
                State = TableState.WatingForWaiter;
            });
        }

        public Order GetOrder(Waiter waiter, List<Food> menu)
        {
            waiter.State = WaiterState.GettingOrder;
            var time = RandomGenerator.GenerateTime(Values.TABLE_MAKING_ORDER_MAX, Values.TABLE_MAKING_ORDER_MIN);
            Thread.Sleep(time);
            Logger.Log($"Table {Id} is making order for {time} ms");

            var order = new Order();

            int amount = RandomGenerator.GenerateNumber(4);

            for (int i = 0; i < amount; i++)
            {
                order.Items.Add(menu[RandomGenerator.GenerateNumber(menu.Count - 1)].Id);
            }
            order.MaxWaitTime = order.Items.Select(o => menu.Find(i => i.Id == o).PreparitionTime).OrderByDescending(t => t).First() * 1.3f;
            order.TableId = Id;
            order.Priority = GeneratePriority(order);

            State = TableState.WatingForOrder;
            return order;
        }

        private static int GeneratePriority(Order order)
        {
            var priority = 1;
            if ((new int[]{ 1, 4}).ToList().Contains(order.Items.Count))
                priority += 2;

            if (order.MaxWaitTime <= 30)
                priority += 1;

            return priority;
        }
    }

    public enum TableState
    {
        NoClient,
        WatingForWaiter,
        Ordering,
        WatingForOrder,
    }
}
