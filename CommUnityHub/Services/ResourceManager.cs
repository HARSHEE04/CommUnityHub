using CommUnityHub.Data;
using CommUnityHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace CommUnityHub.Services

//class developed by Harsheta Sharma
/* Resource Manager:Resource manager class will be responsible for managing the list of resources from the SQL server database.
 * This becomes a service layer class which will use the EF core to load the resources from the csv,  
 * search/query the database,filter the resources based on location and A-Z sorting, method for showing 
 * the resource on google maps api. Resource will be presented through the controller
 */
{
    public class ResourceManager 
    {


        //have connection to the resources database
        private readonly ResourceDbContext _dbContext;

        public ResourceManager(ResourceDbContext context)  //perform the dependency injection of ResourceDbContext
        {
            _dbContext = context;
        }

        //load all the resources from the database

        public async Task<List<Resource>> GetAllResourcesAsync()
        {
            return await _dbContext.Resources.ToListAsync();
        }


        //search for resources based on key words present in all fields of the resources

        public async Task<List<Resource>> SearchByKeywords(string Keyword)
        {
            if (string.IsNullOrWhiteSpace(Keyword))
            {
                return await GetAllResourcesAsync();
            }

            Keyword = Keyword.ToLower(); //convert keyword to lowercase to ensure case insensitive search

            return await _dbContext.Resources
                .Where(r =>
                    r.Name.ToLower().Contains(Keyword.ToLower()) ||
                    r.Region.ToLower().Contains(Keyword.ToLower()) ||
                    r.Category.ToLower().Contains(Keyword.ToLower())
                )
                .ToListAsync();
        }


        //filter resources based on region

        public async Task<List<Resource>> FilterByRegion(string region)
        {
            if (string.IsNullOrWhiteSpace(region))
                {
                return await GetAllResourcesAsync();
            }

            return await  _dbContext.Resources
                .Where(r => r.Region.ToLower().Contains(region.ToLower()))
                .ToListAsync();
        }


        //sort resources alphabetically A-Z or Z-A based on user preference
        public async Task<List<Resource>> SortResourcesAZ(bool ascending) //check to see if the order needs to be ascending or descending
        {

            if (ascending) 
            {
                return await _dbContext.Resources
                    .OrderBy(r => r.Name)//order by a-z
                    .ToListAsync();
            }
            else
            {
                return await _dbContext.Resources
                    .OrderByDescending(r => r.Name) //order by z-a in descending order
                    .ToListAsync();
            }
        }

        //after a resource is chosen, allow the user to select a resource and view it's details
        public async Task<Resource> GetResourceById(int id)
        {
            return await _dbContext.Resources
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        //Present the resource on google maps api based on the address field of the resource
        public string GetGoogleMapsUrl(Resource resource)
        {
            if (resource == null || string.IsNullOrWhiteSpace(resource.Address))
                return null;

            // Regex that finds coordinates in the form: 43.63, -79.87
            var match = Regex.Match(resource.Address, @"(-?\d+\.\d+),\s*(-?\d+\.\d+)");

            if (match.Success)
            {
                // First number = LATITUDE
                string latitude = match.Groups[1].Value;

                // Second number = LONGITUDE
                string longitude = match.Groups[2].Value;

                return $"https://www.google.com/maps?q={latitude},{longitude}&z=15&output=embed";
            }

            // Fallback if no coordinates are present
            return $"https://www.google.com/maps?q={Uri.EscapeDataString(resource.Address)}&z=15&output=embed";
        }

    }
}
