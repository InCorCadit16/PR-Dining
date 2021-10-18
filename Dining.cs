using Dining.Infrastructure.Seeding;
using Dining.Infrastructure.Utils;
using Dining.Models;
using Dining.Server;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dining
{
    public class Dining: BackgroundService
    {
        public DiningServer server;

        public List<Table> tables = new List<Table>();
        public List<Waiter> waiters = new List<Waiter>();
        public List<Food> menu = new List<Food>();
        public List<Order> orders = new List<Order>();

        public Dining(DiningServer server)
        {
            this.server = server;
            this.server.StartAsync(this);
            Seeding.SeedMenu(menu);
        }

        public void InitTables()
        {
            Seeding.SeedTables(tables, 5);

            foreach (var table in tables)
                table.GetClients();
        }

        public void InitWaiters()
        {
            Seeding.SeedWaiters(waiters, 3);

            foreach (var waiter in waiters)
                waiter.StartWaiterWork(this);
        }

        public void ServeOrder(Order order)
        {
            var table = tables.First(t => t.Id == order.TableId);
            float waitTime = (DateTime.Now.Ticks - table.orderedAt.Ticks) / (10000 * Values.TIME_UNIT);

            Logger.Log($"Table {table.Id} received order {order.Id}:");

            Assessor.Assess(waitTime, order);
            orders = orders.Where(o => o.Id != order.Id).ToList();
            table.State = TableState.NoClient;
            table.GetClients();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            InitTables();
            InitWaiters();
            return Task.CompletedTask;
        }
    }
}
