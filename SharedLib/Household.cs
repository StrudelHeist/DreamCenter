using System;
using System.Collections.Generic;

namespace SharedLib
{
    [Serializable]
    public class Household
    {
        public List<Person> Members { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public List<DateTime> Visits { get; set; }
        public Guid ID { get; set; }
    }
}
