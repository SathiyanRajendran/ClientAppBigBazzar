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
            ViewBag.CustomerName = HttpContext.Session.GetString("CustomerName");

            TempData["ProductId"]=id; //when the product is added to their cart it needs product id so we set a product id here.
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(Carts cart)
        {
            TempData["AlertMessage"] = "Product Added Successfully to your cart...!";

            cart.CustomerId = HttpContext.Session.GetInt32("CustomerId"); //here we get the customerid then only which customer add the products to their carts.
            cart.ProductId = Convert.ToInt32(TempData["ProductId"]);//here we get the productid from the get method for add the products to their carts.
            using (var httpClient=new HttpClient())
            {
                httpClient.BaseAddress = new Uri(BaseUrl);
                StringContent content = new StringContent(JsonConvert.SerializeObject(cart), Encoding.UTF8, "application/json");//here we serialize the data
                                                                                                                                //(object to string and it can store on the api)
                                                                                                                                //Json is a format that encodes object in a string
                var response = await httpClient.PostAsync("api/Carts", content);//then it hits an api and take the 200 response.

                return RedirectToAction("CartsByCustomerId","Cart");
            }
        }
        [HttpPost]
        public async Task<List<Carts>> GetCartbyCustomerId(int id)  //get cart list of a particular customerid here id denotes the customerid
                                                                    //view is not a compulsory here we can call one function on it
                                                                    //and call it on any of the get function here.
        {
            List<Carts> carts = new List<Carts>(); //here we get the list of carts owned by the customers
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri(BaseUrl);
                Client.DefaultRequestHeaders.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await Client.GetAsync("api/Carts/" + id); //(api/carts/{id}) we pass the customerid here.
                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsStringAsync().Result;
                    carts = JsonConvert.DeserializeObject<List<Carts>>(Response); //Deserialize means converting json format string to object readable format.
                                                                                  //It comes from the api to client app after hitting 200 response
                }

            }
            return carts; //it will return list of carts here after 200 response owned by the particular customers.
        }
        [HttpGet]
        public async Task<ActionResult> CartsByCustomerId()  //Here we use the get metod simply call that function here and hits the api gives 200 response.
        {
            ViewBag.CustomerName = HttpContext.Session.GetString("CustomerName");
            int id = (int)HttpContext.Session.GetInt32("CustomerId");// AFTER CUSTOMER LOGIN CUSTOMERID SET THERE AND GET IT HERE

            List<Carts> C=await GetCartbyCustomerId(id);//IT CAN SHOW THE LIST OF CARTS OWNED BY THE CUSTOMERS.

            //It should be set on the post method(GetCartbyCustomerId(id)) and
            //call it on get method(GetCartbyCustomerId(id)) so it results to use of minimum API's

            return View(C);//IT WILL RETURN THE LIST OF CARTS.
        }
        [HttpGet]
        public async Task<ActionResult> RemoveCart(int? id) //CART IS REMOVED BY THE CARTID(ID=CARTID)
        {
            int custid = (int)HttpContext.Session.GetInt32("CustomerId");//CUSTOMERID IS SET ON THE LOGIN AND GET IT HERE 
            List<Carts> carts=await GetCartbyCustomerId(custid);//THEN I CAN SHOW THE LIST OF CARTS
            var CartNew=new Carts(); //I CREATE A NEW OBJECT HERE
            //here we store the list of carts added by the customers,then it goes to the loop
            //whether cartid=id we give it will delete on the respective page.
            foreach(var cart in carts)
            {
                if(cart.CartId==id)
                {
                    CartNew = cart;   //CARTNEW MEANS WHICH I WANT TO REMOVE THAT CAN BE SHOWN HERE WHENEVER IT EQUALS IT GOES TO BREAK RETURN THE CART PRODUCT
                    break;             // CARTNEW=DELETECART
                }
            }
            return View(CartNew);
        }
        [HttpPost]
        public async Task<ActionResult> RemoveCart(int id)  //when we scaffolded a delete view it always look for an id
                                                            //so we pass id as a parameter then only it works.
        {
            TempData["AlertMessage"] = "Product Removed Successfully from your cart...!";

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(BaseUrl);
                StringContent content = new StringContent(JsonConvert.SerializeObject(id), Encoding.UTF8, "application/json");
                //here we serialize the data
                //(object to string and it can store on the api)
                //Json is a format that encodes object in a string
                var response = await httpClient.DeleteAsync("api/Carts/" + id); //(API/CARTS/{ID})
                return RedirectToAction("CartsByCustomerId");
            }
        }
        public async Task<IActionResult> UpdateCart(int id)
        {
            ViewBag.CustomerName = HttpContext.Session.GetString("CustomerName");

            int custid = (int)HttpContext.Session.GetInt32("CustomerId");//GET THE CUSTOMERID BY SET IT ON CUSTOMER LOGIN ACTION.
            List<Carts> CustomerCarts = (List<Carts>) await GetCartbyCustomerId(custid);//AFTERWARDS IT SHOWS THE LIST OF CARTS.
            var TempCart = new Carts();   //TEMPCART=UPDATECART
            foreach (var cart in CustomerCarts)//IT CAN GOES TO LOOP AND CHECK THE CART WHICH IT CAN BE UPDATED BY THE CUSTOMER
                                               //HERE CUSTOMER UPDATED THE PRODUCT QUANTITY
            {
                if (cart.CartId == id)
                {
                    TempCart = cart;
                    HttpContext.Session.SetInt32("ProductId", (int)TempCart.ProductId);// HERE WE SET THE PRODUCTID AND CALL IT ON POST METHOD.
                                                                                      //TEMPCART IS PRESENT IT ON THE CARTS
                    break;
                }
            }
            return View(TempCart);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCart(Carts c)
        {
            TempData["AlertMessage"] = "Product Updated Successfully in your cart...!";

            c.CustomerId = HttpContext.Session.GetInt32("CustomerId");  //WE GET THE CUSTOMERID HERE FROM THE LOGIN PAGE
            c.ProductId = HttpContext.Session.GetInt32("ProductId");  //WE GET THE PRODUCTID HERE FROM THE GET METHOD
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(BaseUrl);
                StringContent content = new StringContent(JsonConvert.SerializeObject(c), Encoding.UTF8, "application/json");
                //AFTER UPDATING IT WE CAN SERIALIZE THE DATAS FROM THE DATA OBJECT TO STRING(JSON FORMAT)
                var response = await httpClient.PutAsync("api/Carts/"+c.CartId,content);  //(API/CARTS/{ID})
                return RedirectToAction("CartsByCustomerId");
            }
        }
    }
}
