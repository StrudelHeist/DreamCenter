using System;

namespace SharedLib
{
    [Serializable]
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ID { get; set; }
        public string Phone { get; set; }
        public bool IsHead { get; set; }
    }
}
