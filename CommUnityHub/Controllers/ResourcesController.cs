using Microsoft.AspNetCore.Mvc;
using CommUnityHub.Services;
using System.Net.WebSockets;
using CommUnityHub.Models;

namespace CommUnityHub.Controllers
{

    //the home controller is acutally the resources controller that shows the list of resources on the home page
    public class ResourcesController : Controller
    {

        private readonly ResourceManager resourceManager;

        public ResourcesController(ResourceManager resourceManager)
        {
            this.resourceManager = resourceManager;
        }

        //the resources controller needs to a list of resources to show on the home page (only around 10)


        //Resource controoler will load a few resources, allow for searching and filtering and sorting of resource 
        //Will also allow use to view a specific resource with the google maps api embedded in that page.



        //show main dashbaord with no initial resources
        public async Task<IActionResult> loadDashboard() 
        {

            return View(new List<Resource>());//initially no resources shown and empty list is sent
        }

        //serach for resources based on keywords
        [HttpGet]
      public async Task<IActionResult> Search (string keyword) 
        {
            TempData["LastSearch"] = keyword;
            var resources = await resourceManager.SearchByKeywords(keyword);
            ViewBag.SearchTerm = keyword;
            return View("loadDashboard", resources);
        }

        //filter by region
        public async Task<IActionResult> FilterByRegion(string region)
        {
            // Retrieve last used search keyword
            var keyword = TempData["LastKeyword"]?.ToString();

            List<Resource> resources;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                //  Search using keyword first
                var searchedResults = await resourceManager.SearchByKeywords(keyword);

                // Filter ONLY those results by region (using Contains to match messy DB values)
                resources = searchedResults
                    .Where(r => !string.IsNullOrWhiteSpace(r.Region) &&
                                r.Region.Contains(region, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            else
            {
                // No search keyword used → filter whole DB
               
                resources = (await resourceManager.FilterByRegion(region))
                    .Where(r => r.Region.Contains(region, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Keep keyword alive for next operation
            TempData["LastKeyword"] = keyword;

            ViewBag.SelectedRegion = region;

            return View("loadDashboard", resources);
        }



        //sort A-Z or Z-A
        public async Task<IActionResult> SortResources (bool ascending) 
        {
            var resources = await resourceManager.SortResourcesAZ(ascending);
            ViewBag.SortAscending = ascending; 
            return View("loadDashboard", resources);
        }

        //view resource details page with google maps api
        public async Task<IActionResult> viewDetails(int id) 
        {
            var resources =await resourceManager.GetResourceById(id);
            if(resources == null) 
            {
                return NotFound();
            }

            //generate the google maps url
            ViewBag.MapsUrl = resourceManager.GetGoogleMapsUrl(resources);
            return View(resources);
        }



    }
}
