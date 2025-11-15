using CommUnityHub.Data;
using CommUnityHub.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Reflection.PortableExecutable;

namespace CommUnityHub.Services;

/* Class developed by Harsheta Sharma
 * The purpose of this class is to help import the CSV data into the SQL server database using EF core
 * This cannot go into the ResourceDBContext because that is only for the EF core database context
 * and migrations and putting this info there would go againts the single responsibility principle
 * and SOLID principles of OOP
 */
public class CSVImporter
{

    private readonly ResourceDbContext _context;

    public CSVImporter(ResourceDbContext context)
    {
        _context = context;
    }


    //method to import the resources from the CSV file to the database using CSV helper package
    public void ImportResourcesFromCSV(string filePath) 
    {
        var config= new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null,
            HeaderValidated = null
        };


        using (var reader = new StreamReader(filePath))  //import the filepath into streamreader
        using (var csv = new CsvReader(reader, config))  //create a csv reader object with the streamreader and config

        {
            var csvRecords = csv.GetRecords<ResourceCSVModel>();

            foreach (var record in csvRecords)
            {
                var resource = new Resource //map the ResourceCSVModel to Resource EF core model attributes
                {
                    Name = record.ServiceName,
                    Description = record.PublicComments,
                    Phone = record.OfficePhone,
                    Email = record.Email,
                    HoursOfOperation = record.HoursOfOperation,
                    Address = record.StreetAddress,
                    City = record.PhysicalCity,
                    Category = record.Taxonomy,
                    Website = record.URL
                };

                _context.Resources.Add(resource); //add the resource to the DbSet

            }
            _context.SaveChanges(); //save changes to the database

        }

                                                          
    }




}
