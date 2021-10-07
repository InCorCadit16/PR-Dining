using Kitchen;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dining
{
    public class Waiter
    {
        private static ObjectIDGenerator idGenerator = new ObjectIDGenerator();

        public long Id { get; private set; }
        public WaiterState State;

        public Waiter()
        {
            Id = idGenerator.GetId(this, out bool firstTime);
            State = WaiterState.WaitingForOrder;
        }

        public void StartWaiterWork(List<Table> tables, List<Food> menu, List<Order> orders, DiningServer server)
        {
            
            Thread.Sleep(100);
            new Thread(() =>
            {
                while (true)
                {
                    if (State == WaiterState.WaitingForOrder) {
                        foreach (var table in tables)
                        {
                            lock (tables)
                            {
                                if (table.State == TableState.WatingForWaiter)
                                {

                                    Logger.Log($"Waiter {Id} approched the Table {table.Id}");
                                    Order order = table.GetOrder(this, menu);
                                    orders.Add(order);
                                    server.SendOrder(this, order, table);
                                }

                            }
                        }
                    }
                }
            }).Start();
        }
    }

    public enum WaiterState
    {
        WaitingForOrder,
        GettingsOrder,
        SendingOrderToKitchen
    }
}
