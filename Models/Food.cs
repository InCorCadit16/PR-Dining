using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Dining.Models
{
    public class Food
    {
        public long Id;
        public string Name;
        public int PreparitionTime;
        public int Comlexity;
        public CookingApparatusType? CookingApparatus;
    }
}
