using ClientAppBigBazzar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace ClientAppBigBazzar.Controllers
{
    public class ProductsController : Controller
    {
        string BaseUrl = "https://localhost:7210/";
       
        public async Task<List<Products>> ReturnAllProducts()
        {
            List<Products> products = new List<Products>();
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(BaseUrl);
                Client.DefaultRequestHeaders.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await Client.GetAsync("api/Products");
                if (Res.IsSuccessStatusCode)
                {
                    var ProductResponse = Res.Content.ReadAsStringAsync().Result;
                    products = JsonConvert.DeserializeObject<List<Products>>(ProductResponse);
                }
                return products;
            }
        }
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await ReturnAllProducts();
            return View(products);
        }
            [HttpPost]
        public async Task<Products> GetProductById(int id)  
            //It is one of the calling a method it should call on the get method (edit method)
        {
            Products P2=new Products();
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(BaseUrl);
                Client.DefaultRequestHeaders.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await Client.GetAsync("api/Products/" + id);

                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsStringAsync().Result;
                    P2 = JsonConvert.DeserializeObject<Products>(Response);

                }
                return P2;
            }

        }
        [HttpPost]
        public async Task<List<Categories>> CategoryChoose()
        //It is one of the calling a method it should call on the get method (create products method)

        {
            List<Categories> c = new List<Categories>();
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(BaseUrl);
                Client.DefaultRequestHeaders.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await Client.GetAsync("api/Admin");
                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsStringAsync().Result;
                    c = JsonConvert.DeserializeObject<List<Categories>>(Response);
                }
                return c;
            }
        }



        [HttpGet]
        
        public async Task<ActionResult> ProductsCreate()
        {
            var categorylist=await CategoryChoose();
            ViewBag.categorylist = new SelectList(categorylist,"CategoryId","CategoryName");
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ProductsCreate(Products P)
        {
            TempData["AlertMessage"] = "Product Created Successfully...!";
            P.TraderId = (int)HttpContext.Session.GetInt32("TraderId");

            using (var httpClient=new HttpClient())
            {
                httpClient.BaseAddress = new Uri(BaseUrl);
                httpClient.DefaultRequestHeaders.Clear();
                StringContent content = new StringContent(JsonConvert.SerializeObject(P), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("api/Products/AddProduct", content);
                return RedirectToAction("GetAllProducts");
            }
        }
        [HttpGet]
        public async Task<ActionResult> EditProducts(int id)
        {
            //Products P = new Products();
            //using (var httpClient = new HttpClient())
            //{
            //    using (var response = await httpClient.GetAsync("https://localhost:7210/api/Products/" + id))
            //    {
            //        string apiResponse = await response.Content.ReadAsStringAsync();
            //        P = JsonConvert.DeserializeObject<Products>(apiResponse);
            //    }

            //}
            Products P = await GetProductById(id);
            var CategoryList = await CategoryChoose();
            ViewBag.CategoryList = new SelectList(CategoryList, "CategoryId", "CategoryName");
            return View(P);
        }
        [HttpPost]
        public async Task<ActionResult> EditProducts(Products P)
        {
            TempData["AlertMessage"] = "Product Updated Successfully...!";

            //P.TraderId = Convert.ToInt32(TempData["TraderId"]);//TempData is set on the traderlogin in traderscontroller
            P.TraderId = HttpContext.Session.GetInt32("TraderId");//Once we set the session in any of the controller it can be used in entire project.
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(BaseUrl);
                StringContent content = new StringContent(JsonConvert.SerializeObject(P), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync("api/Products/" + P,content);
                return RedirectToAction("GetAllProducts");
            }
            //Products p1 = new Products();
            //using (var httpClient = new HttpClient())
            //{
            //    int id = P.ProductId;
            //    StringContent content1 = new StringContent(JsonConvert.SerializeObject(P), Encoding.UTF8, "application/json");
            //    using (var response = await httpClient.PutAsync("https://localhost:7210/api/Products/" + P, content1))
            //    {
            //        string apiResponse = await response.Content.ReadAsStringAsync();
            //        p1 = JsonConvert.DeserializeObject<Products>(apiResponse);

            //    }
            //}
            //return RedirectToAction("GetAllProducts");
        }
        [HttpGet]
        public async Task<ActionResult> DeleteProducts(int id)
        {
            TempData["ProductId"]=id;
            Products e=new Products();
            using (var httpClient=new HttpClient())
            {
                using (var response=await httpClient.GetAsync("https://localhost:7210/api/Products/"+id))
                {
                    string apiResponse=await response.Content.ReadAsStringAsync();  
                    e = JsonConvert.DeserializeObject<Products>(apiResponse);
                }
            }
                return View(e);
        }
        [HttpPost]
        public async Task<ActionResult> DeleteProducts(Products P)
        {
            TempData["AlertMessage"] = "Product Deleted Successfully...!";

            int prid = Convert.ToInt32(TempData["ProductId"]);
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync("https://localhost:7210/api/Products/" + prid))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                }
            }
            return RedirectToAction("GetAllProducts");
        }
        [HttpGet]  //We call A separate post method and then it is call on the get method like in cart controller
        public async Task<IActionResult> GetProductByTrader() //It is in traders contoller in api to show the respective products to the traders.
        {
            string? token = HttpContext.Session.GetString("token");

            int id = (int)HttpContext.Session.GetInt32("TraderId");
            List<Products> p = new List<Products>();
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(BaseUrl);
                Client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", token); Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await Client.GetAsync("api/Traders/TraderId?id=" + id);
                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsStringAsync().Result;
                    p = JsonConvert.DeserializeObject<List<Products>>(Response);
                }
            }
            return View(p);


        }
    }
}

