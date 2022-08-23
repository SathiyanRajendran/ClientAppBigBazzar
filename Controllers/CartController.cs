using ClientAppBigBazzar.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace ClientAppBigBazzar.Controllers
{
    public class CartController : Controller
    {
        string BaseUrl = "https://localhost:7210/";

        [HttpGet]
        public IActionResult AddToCart(int id)
        {                             //Cart is added by the customers to their carts.
            TempData["ProductId"]=id;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(Carts cart)
        {
            cart.CustomerId = HttpContext.Session.GetInt32("CustomerId");
            cart.ProductId = Convert.ToInt32(TempData["ProductId"]);
            using (var httpClient=new HttpClient())
            {
                httpClient.BaseAddress = new Uri(BaseUrl);
                StringContent content = new StringContent(JsonConvert.SerializeObject(cart), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("api/Carts", content);
                return RedirectToAction("CartsByCustomerId","Cart");
            }
        }
        [HttpPost]
        public async Task<List<Carts>> GetCartbyCustomerId(int id)  //get cart list of a particular customerid
        {
            List<Carts> carts = new List<Carts>();
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(BaseUrl);
                Client.DefaultRequestHeaders.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await Client.GetAsync("api/Carts/" + id);
                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsStringAsync().Result;
                    carts = JsonConvert.DeserializeObject<List<Carts>>(Response);
                }

            }
            return carts;
        }
        [HttpGet]
        public async Task<ActionResult> CartsByCustomerId()
        {
            int id = (int)HttpContext.Session.GetInt32("CustomerId");
            List<Carts> C=await GetCartbyCustomerId(id);
            //It should be set on the post method(GetCartbyCustomerId(id)) and call it on get method(GetCartbyCustomerId(id)) so it results to use of minimum API's
            return View(C);
        }
        [HttpGet]
        public async Task<ActionResult> RemoveCart(int? id) //Delete Carts by id=cartId
        {
            int custid = (int)HttpContext.Session.GetInt32("CustomerId");
            List<Carts> carts=await GetCartbyCustomerId(custid);
            var TempCart=new Carts();
            //here we store the list of carts added by the customers,then it goes to the loop
            //whether cartid=id we give it will delete on the respective page.
            foreach(var cart in carts)
            {
                if(cart.CartId==id)
                {
                    TempCart = cart;
                    break;
                }
            }
            return View(TempCart);
        }
        [HttpPost]
        public async Task<ActionResult> RemoveCart(int id)
        {

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(BaseUrl);
                StringContent content = new StringContent(JsonConvert.SerializeObject(id), Encoding.UTF8, "application/json");
                var response = await httpClient.DeleteAsync("api/Carts?id=" + id);
                return RedirectToAction("CartsByCustomerId");
            }
        }
        [HttpGet]
        public async Task<ActionResult> UpdateCart(int id)
        {
            int custid = (int)HttpContext.Session.GetInt32("CustomerId");
            Carts t = new Carts();
            using (var httpClient = new HttpClient())
            {

                using (var response = await httpClient.GetAsync("https://localhost:7210/api/Carts/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    t = JsonConvert.DeserializeObject<Carts>(apiResponse);
                }
            }
            return View(t);
        }
        [HttpPost]
        public async Task<ActionResult> UpdateCart(Carts c)
        {
            Carts p1 = new Carts();
            using (var httpClient = new HttpClient())
            {
                int id = c.CartId;
                StringContent content1 = new StringContent(JsonConvert.SerializeObject(c), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PutAsync("https://localhost:7210/api/Carts/" + id, content1))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    ViewBag.Result = "Success";
                    p1 = JsonConvert.DeserializeObject<Carts>(apiResponse);
                }
            }
            return RedirectToAction("CartsByCustomerId");
        }
    }
}
