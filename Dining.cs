using Kitchen;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dining
{
    public class Dining: BackgroundService
    {
        public DiningServer server;

        public List<Table> tables = new List<Table>();
        public List<Waiter> waiters;
        public List<Food> menu = new List<Food>();
        public List<Order> orders = new List<Order>();

        public Dining(DiningServer server)
        {
            menu.AddRange(new Food[]
            {
                new Food 
                {
                    Id = 1,
                    Name = "Pizza",
                    PreparitionTime = 20,
                    Comlexity = 2,
                    CookingApparatus = CookingApparatus.CookingApparatusType.Oven
                },
                new Food
                {
                    Id = 2,
                    Name = "Salad",
                    PreparitionTime = 10,
                    Comlexity = 1,
                    CookingApparatus = null
                },
                new Food
                {
                    Id = 3,
                    Name = "Zeama",
                    PreparitionTime = 7,
                    Comlexity = 1,
                    CookingApparatus = CookingApparatus.CookingApparatusType.Stove
                },
                new Food
                {
                    Id = 4,
                    Name = "Scallop Sashami with Meyer Lemon Confit",
                    PreparitionTime = 32,
                    Comlexity = 3,
                    CookingApparatus = null
                },
                new Food
                {
                    Id = 5,
                    Name = "Island Duck with Mulberry Mustard",
                    PreparitionTime = 35,
                    Comlexity = 3,
                    CookingApparatus = CookingApparatus.CookingApparatusType.Oven
                },
                new Food
                {
                    Id = 6,
                    Name = "Waffles",
                    PreparitionTime = 10,
                    Comlexity = 1,
                    CookingApparatus = CookingApparatus.CookingApparatusType.Stove
                },
            });
            this.server = server;
            server.StartAsync(CancellationToken.None);
            
        }

        public void InitTables()
        {
            tables.AddRange(new Table[]
            {
                new Table(),
                new Table(),
                new Table(),
                new Table(),
                new Table()
            });

            foreach (var table in tables)
            {
                table.GetClients();
            }
        }

        public void InitWaiters()
        {
            waiters = new List<Waiter>(new Waiter[]
            {
                new Waiter(),
                new Waiter(),
                new Waiter(),
            });

            foreach (var waiter in waiters)
            {
                waiter.StartWaiterWork(tables, menu, orders, server);
            }


        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            InitTables();
            // Thread.Sleep(Values.TABLE_WAIT_FOR_CLIENTS_MAX * Values.TIME_UNIT);
            InitWaiters();
            return Task.CompletedTask;
        }
    }
}
