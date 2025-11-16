namespace CommUnityHub.Models
{

    /*Class developed by Harsheta Sharma
     * The purpose of this file is to represent the structure of the CSV resource model
     * object that will be used to import data from the CSV file into the database.
     * the attributes of this class correspond to the columns in the CSV file to ensure proper mapping
     * this class only represents the csv model while
     * the resource class represents our EF entity model for the database
     */
    public class ResourceCSVModel
    {
        
        public string AgencyName { get; set; }
        public string DescriptionService { get; set; }
        public string OfficePhone { get; set; }
        public string PrimaryContactEmail { get; set; }
        public string Hours { get; set; }
        public string Geocoding { get; set; } 
        public string AreasServed { get; set; }
        public string Taxonomy { get; set; }
        public string Website { get; set; }
    }
}
