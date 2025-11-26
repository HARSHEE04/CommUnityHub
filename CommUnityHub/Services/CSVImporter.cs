using CommUnityHub.Data;
using CommUnityHub.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace CommUnityHub.Services
{
    /* Class developed by Harsheta Sharma
     * This class handles importing CSV resource data into the SQL Server database (EF Core).
     * Following SOLID principles, data import logic does NOT belong in DbContext.
     * 
 
     * ResourcesCSVMap maps the csv columns to the resourceCSVModel properties which doesnt have any validation checks.
     * Then csv importer maps the resourceCSVModel properties to Resource entity properties which has validation checks.
     * our DbContext only deals with Resource entity which has validation checks.
     */
    public class CSVImporter
    {
        private readonly ResourceDbContext _context;

        public CSVImporter(ResourceDbContext context)
        {
            _context = context;
        }

        // Imports CSV records into the Resource table
        public void ImportResourcesFromCSV(string filePath)
        {
            Console.WriteLine("=== CSV Importer Starting ===");

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {

                //for data validation and cleaning purposes while importing CSV
                MissingFieldFound = null,
                HeaderValidated = null,
                IgnoreBlankLines = true,
                TrimOptions = TrimOptions.Trim
            };

            using (var reader = new StreamReader(filePath)) //READS THE CSV FILE
            using (var csv = new CsvReader(reader, config)) //PARSES THE CSV FILE
            {
                // REGISTER THE CLASS MAP (MOST IMPORTANT STEP)
                csv.Context.RegisterClassMap<ResourceCSVMap>();  //register the mapper class to ensure proper mapping between ResourceCSVModel and CSV columns

                var csvRecords = csv.GetRecords<ResourceCSVModel>().ToList();
                var limitedRecords = csvRecords.Take(300).ToList(); //only read first 500 records to avoid memory overload


                Console.WriteLine($"Loaded {csvRecords.Count} rows from CSV.");

                foreach (var record in limitedRecords)
                {
                    
                  

                    var resource = new Resource  //map the CSV record to Resource entity
                    {
                        Name = record.AgencyName,
                        Description = record.DescriptionService,
                        Phone = record.OfficePhone,
                        Email = record.PrimaryContactEmail,
                        HoursOfOperation = record.Hours,
                        Address = record.Geocoding,
                        Region = record.AreasServed,
                        Category = record.Taxonomy,
                        Website = record.Website
                    };

                    _context.Resources.Add(resource);
                }

                _context.SaveChanges();
                Console.WriteLine("=== CSV Import Complete & Saved to Database ===");
            }
        }
    }
}
