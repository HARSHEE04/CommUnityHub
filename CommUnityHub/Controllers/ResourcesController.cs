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



        //introduce paigination to ensure only 20 resources are shown at a time and webpage doesnt crash 
        //REFERENCE: https://www.geeksforgeeks.org/blogs/pagination-design-pattern/

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
            var keyword = TempData["LastSearch"]?.ToString();

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
            TempData["LastSearch"] = keyword;

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

        //add a new controller for load more options to ensure webpage doesnt crash
        [HttpGet]
        public async Task<IActionResult> LoadMore(string keyword, string region, bool? ascending, int page = 1)
        {
            int pageSize = 20;

            // Start with full set depending on user's previous actions
            List<Resource> results;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                results = await resourceManager.SearchByKeywords(keyword);

                if (!string.IsNullOrWhiteSpace(region))
                {
                    results = results
                        .Where(r => r.Region.Contains(region, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
            }
            else if (!string.IsNullOrWhiteSpace(region))
            {
                results = await resourceManager.FilterByRegion(region);
            }
            else
            {
                results = await resourceManager.GetAllResourcesAsync();
            }

            // Sorting
            if (ascending.HasValue)
            {
                results = ascending.Value
                    ? results.OrderBy(r => r.Name).ToList()
                    : results.OrderByDescending(r => r.Name).ToList();
            }

            // Pagination: Pick next 20 results
            var pageResults = results
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Return partial HTML ONLY for these 20 cards
            return PartialView("_ResourceCardsPartial", pageResults);
        }




    }
}
