using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Dining
{
    public class Order
    {
        
        private static ObjectIDGenerator idGenerator = new ObjectIDGenerator();

        public long Id { get; private set; }
        public List<long> Items;
        public int Priority;
        public float MaxWaitTime;
        public long TableId;

        public Order()
        {
            Items = new List<long>();
            Id = idGenerator.GetId(this, out bool firstTime);
        }
    }
}
