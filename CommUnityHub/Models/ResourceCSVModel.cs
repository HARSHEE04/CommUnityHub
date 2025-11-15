namespace CommUnityHub.Models
{

    /*Class developed by Harsheta Sharma
     * The purpose of this file is to represent the structure of the CSV resource model
     * object that will be used to import data from the CSV file into the database.
     * the attributes of this class correspond to the columns in the CSV file and ensure proper mapping
     * this class only represents the csv model while
     * the resource class represents our EF entity model for the database
     * 
     */
    public class ResourceCSVModel
    {

        public string ServiceName { get; set; }
        public string PublicComments { get; set; }
        public string OfficePhone { get; set; }
        public string Email { get; set; }
        public string HoursOfOperation { get; set; }
        public string StreetAddress { get; set; }
        public string PhysicalCity { get; set; }
        public string Taxonomy { get; set; }
        public string URL { get; set; }
    }
}
