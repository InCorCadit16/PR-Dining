using Kitchen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dining
{
    public class Table
    {
        private static ObjectIDGenerator idGenerator = new ObjectIDGenerator();

        public long Id { get; private set; }
        public TableState State;

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
                Logger.Log($"Table {Id} waited for the client for {time} ms");
            });
        }

        public Order GetOrder(Waiter waiter, List<Food> menu)
        {
            lock(waiter)
            {
                State = TableState.Ordering;
                waiter.State = WaiterState.GettingsOrder;
                var time = RandomGenerator.GenerateTime(Values.TABLE_MAKING_ORDER_MAX, Values.TABLE_MAKING_ORDER_MIN);
                Thread.Sleep(time);
                Logger.Log($"Table {Id} is making order for {time} ms");

                var order = new Order();

                int amount = RandomGenerator.GenerateNumber(5);

                for (int i = 0; i < amount; i++)
                {
                    order.Items.Add(menu[RandomGenerator.GenerateNumber(menu.Count - 1)].Id);
                }
                order.MaxWaitTime = order.Items.Select(o => menu.Find(i => i.Id == o).PreparitionTime).OrderBy(t => t).First() * 1.3f;
                order.TableId = Id;
                order.Priority = GeneratePriority(order);

                State = TableState.WatingForOrder;
                return order;
            }
            
        }

        private static int GeneratePriority(Order order)
        {
            var priority = 1;
            if ((new int[]{ 1, 2, 5}).ToList().Contains(order.Items.Count))
                priority += 2;

            if (order.MaxWaitTime < 25)
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
