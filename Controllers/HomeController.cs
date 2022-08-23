using ClientAppBigBazzar.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ClientAppBigBazzar.Controllers
{
    public class HomeController : Controller
    {
        ProductsController P=new ProductsController();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
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
        public IActionResult Search()
        {
            return View();
        }
        public async Task<IActionResult> SearchResult(string SearchPhrase,string option)
        {
            //p.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0

            List<Products> result = new List<Products>();

            var products = await  P.ReturnAllProducts();
            foreach (var product in products)
            {
                if (option.Equals("ProductName") &&  product.ProductName.IndexOf(SearchPhrase, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    result.Add(product);
                }
                else if (option.Equals("CategoryName") && product.Categories.CategoryName.IndexOf(SearchPhrase, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    result.Add(product);
                }
                else
                {
                    ViewBag.ErrorMessage = "No Result Found";
                }
            }
            return View(result);
        }
    }
}