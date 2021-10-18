using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Dining.Models
{
    public class CookingApparatus
    {
        public long Id;
        public CookingApparatusType Type;

        private static ObjectIDGenerator idGenerator = new ObjectIDGenerator();

        public CookingApparatus()
        {
            Id = idGenerator.GetId(this, out bool firstTime);
        }

    }

    public enum CookingApparatusType
    {
        Oven,
        Stove
    }
}
