using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Dining.Models
{
    public class Order
    {
        
        private static ObjectIDGenerator idGenerator = new ObjectIDGenerator();

        public long Id { get; private set; }
        public List<long> Items { get; set; }
        public int Priority { get; set; }
        public float MaxWaitTime { get; set; }
        public long TableId { get; set; }

        public Order()
        {
            Items = new List<long>();
            Id = idGenerator.GetId(this, out bool firstTime);
        }

        internal List<Order> Where(Func<object, bool> p)
        {
            throw new NotImplementedException();
        }
    }
}
