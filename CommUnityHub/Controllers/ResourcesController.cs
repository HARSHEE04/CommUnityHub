using CommUnityHub.Models;
using CommUnityHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace CommUnityHub.Controllers
{

    //the home controller is acutally the resources controller that shows the list of resources on the home page
    [AllowAnonymous]
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
            resources = resources.Take(20).ToList(); //only show first 20 results initially
            ViewBag.SearchTerm = keyword;
            return View("loadDashboard", resources);
        }

        //filter by region
        public async Task<IActionResult> FilterByRegion(string region)
        {
            // Retrieve last used search keyword
            var keyword = TempData["LastSearch"]?.ToString()
                 ?? Request.Query["keyword"].ToString();

            List<Resource> resources;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                //  Search using keyword first
                var searchedResults = await resourceManager.SearchByKeywords(keyword);

                // Filter ONLY those results by region (using Contains to match messy DB values)
                resources = searchedResults
                    .Where(r => !string.IsNullOrWhiteSpace(r.Region) &&
                                r.Region.Contains(region, StringComparison.OrdinalIgnoreCase))
                    .Take(20)
                    .ToList();
            }
            else
            {
                // No search keyword used → filter whole DB
               
                resources = (await resourceManager.FilterByRegion(region))
                    .Where(r => r.Region.Contains(region, StringComparison.OrdinalIgnoreCase))
                    .Take(20)
                    .ToList();
            }

            // Keep keyword alive for next operation
            TempData["LastSearch"] = keyword;

            ViewBag.SelectedRegion = region;

            return View("loadDashboard", resources);
        }



        //sort A-Z or Z-A
        public async Task<IActionResult> SortResources(bool ascending, string keyword, string region)
        {
            // use last search 
            keyword = keyword
                   ?? TempData["LastSearch"]?.ToString();

            //use last region filter to ensure those two are maintained
            region = region
                  ?? TempData["LastRegion"]?.ToString();

            // Users must search before sorting
            if (string.IsNullOrWhiteSpace(keyword))
            {
                TempData["Error"] = "Please search before sorting.";
                return RedirectToAction("loadDashboard");
            }

            //Start with the search results
            var results = await resourceManager.SearchByKeywords(keyword);

            // ensure to take region filter results into account
            if (!string.IsNullOrWhiteSpace(region))
            {
                results = results
                    .Where(r =>
                        !string.IsNullOrWhiteSpace(r.Region) &&
                        r.Region.Contains(region, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            
            var sortedAll = await resourceManager.SortResourcesAZ(ascending);

            //display proper sorted results
            results = sortedAll
                .Where(sorted => results.Any(r => r.Id == sorted.Id))
                .Take(20)  // first 20 only
                .ToList();

            // keep keywords
            TempData["LastSearch"] = keyword;
            TempData["LastRegion"] = region;

            // use those values in view
            ViewBag.SearchTerm = keyword;
            ViewBag.SelectedRegion = region;
            ViewBag.SortAscending = ascending;

            return View("loadDashboard", results);
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
