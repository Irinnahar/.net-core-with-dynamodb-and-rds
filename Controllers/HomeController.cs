using _301173198_Nahar__Lab3.Models;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace _301173198_Nahar__Lab3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        IConfiguration configuration;
        DbContext context;
        public HomeController(IConfiguration configuration, ILogger<HomeController> logger)
        {
            this.configuration = configuration;
            context = new DbContext(configuration);
            _logger = logger;

        }
        

        public async Task<IActionResult> Index( )
        {
            //var response = await context.GetMovieRank(movieTitle);
            var movieList = await context.GetAll();
            var result = (IEnumerable<Movie>)(from x in movieList
                          group x by x.MovieTitle into grp
                          select new Movie
                          {
                              MovieTitle = grp.Key,
                              Rating = Math.Round(grp.Average(obj => obj.Rating))
                          }).ToList();

            var sortedItems = result.OrderByDescending (m => m.Rating).ToList();
            return View(sortedItems);


        }

        public async Task<IActionResult> Comments(string title)
        {
            
            return View(await context.GetByTitleAsync(title));

        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
