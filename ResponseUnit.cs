namespace Incri1_Galang
{
    public class ResponseUnit
    {
        public int UnitID { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public string UnitType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;

        // New properties to capture all Form8 inputs
        public System.DateTime DateAdded { get; set; }
        public string Resources { get; set; } = string.Empty;
        public string IncidentLocation { get; set; } = string.Empty;
        public string AlarmDescription { get; set; } = string.Empty;

        public ResponseUnit()
        {
            // Default constructor
        }

        public ResponseUnit(int unitID, string unitName, string unitType, string status, string location, System.DateTime dateAdded, string resources)
        {
            UnitID = unitID;
            UnitName = unitName;
            UnitType = unitType;
            Status = status;
            Location = location;
            DateAdded = dateAdded;
            Resources = resources;
            IncidentLocation = string.Empty;
            AlarmDescription = string.Empty;
        }
    }
}