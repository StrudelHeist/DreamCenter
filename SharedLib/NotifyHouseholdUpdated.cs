using System;

namespace SharedLib
{
    [Serializable]
    public class NotifyHouseholdUpdated
    {
        public Household UpdatedHousehold { get; set; }
    }
}
