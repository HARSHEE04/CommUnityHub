using CsvHelper.Configuration;
using CommUnityHub.Models;

public sealed class ResourceCSVMap : ClassMap<ResourceCSVModel>
{

    //class developed by Harsheta Sharma
    /*The purpose of this class is to define the mapping between the ResourceCSVModel properties
     * and the csv columns while importing the csv file using CsvHelper library
     * It uses the ClassMap library to define how columns in a CSV file map to properties in ResourceCSVModel
     */
    public ResourceCSVMap()
    {
        Map(m => m.AgencyName).Name("AgencyName");//Reads "AgencyName" column from CSV and maps it to AgencyName property in ResourceCSVModel
        Map(m => m.DescriptionService).Name("DescriptionService");
        Map(m => m.OfficePhone).Name("OfficePhone");
        Map(m => m.PrimaryContactEmail).Name("PrimaryContactEmail");
        Map(m => m.Hours).Name("Hours");
        Map(m => m.Geocoding).Name("Geocoding");
        Map(m => m.AreasServed).Name("AreasServed");
        Map(m => m.Taxonomy).Name("Taxonomy");
        Map(m => m.Website).Name("Website");
    }
}
