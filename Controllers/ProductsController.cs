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
       
        public async Task<List<Products>> ReturnAllProducts() //this is for search method we get the products by search option.
        {
            List<Products> products = new List<Products>();
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(BaseUrl);
                Client.DefaultRequestHeaders.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await Client.GetAsync("api/Products"); //get the list of products from the api so we deserialize this action method
                if (Res.IsSuccessStatusCode)
                {
                    var ProductResponse = Res.Content.ReadAsStringAsync().Result;
                    products = JsonConvert.DeserializeObject<List<Products>>(ProductResponse); //deserialize the products from an api to the client
                }
                return products;
            }
        }
        public async Task<IActionResult> GetAllProducts()
        {
            ViewBag.CustomerName = HttpContext.Session.GetString("CustomerName");
            var products = await ReturnAllProducts(); //here we see the list of products for customers
            return View(products);
        }
            [HttpPost]
        public async Task<Products> GetProductById(int id)  //its just a method calling on another action method
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
                    P2 = JsonConvert.DeserializeObject<Products>(Response); //we use deserialize here for getting the products from the api

                }
                return P2;
            }

        }
        [HttpPost]
        public async Task<List<Categories>> CategoryChoose() //its just a method calling on another action method
        //It is one of the calling a method it should call on the get method (create products method)

        {
            List<Categories> c = new List<Categories>(); //categories list for creating the products in the view method
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(BaseUrl);
                Client.DefaultRequestHeaders.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await Client.GetAsync("api/Admin"); //go and hit an api and bring back the data from an api(deserialize)
                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsStringAsync().Result;
                    c = JsonConvert.DeserializeObject<List<Categories>>(Response); //we use deserialize here and get thye categories list from the api
                }
                return c;
            }
        }



        [HttpGet]
        
        public async Task<ActionResult> ProductsCreate()
        {
            ViewBag.TraderName = HttpContext.Session.GetString("TraderName");

            var categorylist =await CategoryChoose();  //deserialize the category list and call it here.
            ViewBag.categorylist = new SelectList(categorylist,"CategoryId","CategoryName"); //this is one of the method to show the category name instead of categoryid.
            return View();   //scaffolded views.
        }
        [HttpPost]
        public async Task<ActionResult> ProductsCreate(Products P)
        {
            ViewBag.TraderName = HttpContext.Session.GetString("TraderName");

            TempData["AlertMessage"] = "Product Created Successfully...!";
            P.TraderId = (int)HttpContext.Session.GetInt32("TraderId");

            using (var httpClient=new HttpClient())
            {
                httpClient.BaseAddress = new Uri(BaseUrl);
                httpClient.DefaultRequestHeaders.Clear();  //here we serialize the data and send the data to the api from the client app
                StringContent content = new StringContent(JsonConvert.SerializeObject(P), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("api/Products/AddProduct", content);//we give the respective url here to post the products to an api
                return RedirectToAction("GetProductByTrader");
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
            ViewBag.TraderName = HttpContext.Session.GetString("TraderName");

            Products P = await GetProductById(id);  //go and get the products by deserializing it.
            var CategoryList = await CategoryChoose();  //go and get the categories by deserializing it.
            ViewBag.CategoryList = new SelectList(CategoryList, "CategoryId", "CategoryName"); //when we want to change the category we can use it(this is for dropdown).
            return View(P); //after wards it returns the same view.
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
                                                //after we edit the products row by the id it can be serialize into the api.
                                                //first serialize it and then go and save in the respective api.
                var response = await httpClient.PutAsync("api/Products/" + P,content);
                                                //put,post,delete--->serializeobject
                                                //get,getbyid------->deserializeobject
                return RedirectToAction("GetProductByTrader");
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
        public async Task<ActionResult> DeleteProducts(int id) //asp-route-id=@item.productid
        {
            ViewBag.TraderName = HttpContext.Session.GetString("TraderName");  //this is set in the trader login and get the tradername from there
                                                                               //and use it here

            TempData["ProductId"]=id;  //set the product id here using the tempdata like session
            Products e=new Products();
            using (var httpClient=new HttpClient())
            {
                using (var response=await httpClient.GetAsync("https://localhost:7210/api/Products/" +id)) //here we use get the products by the id we
                                                                                                           //put the respective url here
                {
                    string apiResponse=await response.Content.ReadAsStringAsync();  //deserialize it here that means get the
                                                                                    //product details from an api by id
                    e = JsonConvert.DeserializeObject<Products>(apiResponse);//after hitting an api we deserialize here and shows to the traders.
                }
            }
                return View(e); //it can show the view here (are you sure you want to delete this)
        }
        
        [HttpPost]
        public async Task<IActionResult> DeleteProducts(Products P) //when we pass a products as a argument
                                                                    //we must include the input tag in the html file
                                                                    //then only it works.
        {

            TempData["AlertMessage"] = "Product Deleted Successfully...!";

            int prid = Convert.ToInt32(TempData["ProductId"]); //productid set in the tempdata in the get method (delete).
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(BaseUrl);

                StringContent content = new StringContent(JsonConvert.SerializeObject(P), Encoding.UTF8, "application/json");

                var response =await  httpClient.DeleteAsync("api/Products/" + P.ProductId);//check the url here and then delete the products.
                return RedirectToAction("GetProductByTrader");

            }
        }
        [HttpGet]  //We call A separate post method and then it is call on the get method like in cart controller
        public async Task<IActionResult> GetProductByTrader() //It is in traders contoller in api to show the respective products to the traders.
        {
            string? token = HttpContext.Session.GetString("token");
            ViewBag.TraderName = HttpContext.Session.GetString("TraderName");


            int id = (int)HttpContext.Session.GetInt32("TraderId");
            List<Products> p = new List<Products>();
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(BaseUrl);
                Client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", token); Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await Client.GetAsync("api/Traders/TraderId?id=" + id); //go and hit the api and get the products by the traderid
                                                                                                  //here we deserialize the trader's product details
                                                                                                  //which are the products added by the traders
                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsStringAsync().Result;
                    p = JsonConvert.DeserializeObject<List<Products>>(Response); //deserialize the products list here.
                }
            }
            return View(p);   //in this view we show the product list by the traders.
        }
    }
}

