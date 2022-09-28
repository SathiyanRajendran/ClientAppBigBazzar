using ClientAppBigBazzar.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace ClientAppBigBazzar.Controllers
{
    public class PaymentController : Controller
    {
        string BaseUrl = "https://localhost:7210/";

        [HttpPost]
        public async Task<IActionResult> ToBuy()
            //IT IS NOT A VIEW METHOD IT JUST A FUNCTION CALL ON THE ANOTHER METHOD.
            //It should be reflected on the customer buy in the ordercontroller in api(CustomerBuy)
        {
            var custid = HttpContext.Session.GetInt32("CustomerId"); //GET THE CUSTOMERID FROM THE LOGIN PAGE AND PURCHASE THE CART
            OrderMasters om = new OrderMasters(); //CREATE AN OBJECT FOR THAT AND INITIALIZED HERE.
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(BaseUrl);
                StringContent content = new StringContent(JsonConvert.SerializeObject(custid), Encoding.UTF8, "application/json");
                                  //SERIALIZE THE DATA FROM THE DATA OBJECT TO STRING
                                  //here serialize denotes the we send the data from client to api (data obj to string)
                HttpResponseMessage Res = await httpClient.PostAsync("api/Order/"+custid,content); //(API/ORDER/CUSTOMERID)
                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsStringAsync().Result;
                    om = JsonConvert.DeserializeObject<OrderMasters>(Response);
                           //DESERIALIZE THE DATA FROM THE STRING TO OBJECT DATA.
                           //after serializing deserialize happens convert the string to object.
                           //after that ordermaster and orderdetails are created for the customerid,then it goes to transaction table
                }
            }
            return RedirectToAction("Transaction", new { id = om.OrderMasterId }); //id denotes the ordermaster id
                                                                                   //it will goes to the transaction action and then getordermasterbyordermasterid.
                                                                                   //this 'id' represents the argument present in the 'transaction' action method

        }
        [HttpPost]
        public async Task<OrderMasters> GetOrderMasterByOrderMasterId(int id) 
                                                //HTTPGET BY ID METHOD IN API(ORDER CONTROLLER)
                                                //ordermaster id come here and it go and fetch it from an api
        {
            OrderMasters om = new OrderMasters();
            using (var Client=new HttpClient())
            {
                Client.BaseAddress = new Uri(BaseUrl);
                Client.DefaultRequestHeaders.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await Client.GetAsync("api/Order/" + id);//for getting the ordermasters by the om id.
                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsStringAsync().Result;
                    om = JsonConvert.DeserializeObject<OrderMasters>(Response);
                                      //deserialize here denotes the getting the ordermaster details from the api and show it on the view
                                      //order master is created here with amount and cardnumber null
                }
            }
            return om;
        }
        [HttpGet]
        public async Task<IActionResult> Transaction(int id)  //id denotes the ordermaster id and it goes and fetch ordermaster details from an api
        {

            OrderMasters om =await GetOrderMasterByOrderMasterId(id);
            return View(om);
        }
        [HttpPost]
        public async Task<IActionResult> Transaction(OrderMasters ordermaster) //------ 
                                                                                //two ordermasterid should be created here first for
                                                                                //total and then we paid another order master id created.
        {
            var custid = HttpContext.Session.GetInt32("CustomerId"); //GET THE CUSTOMERID FROM THE LOGIN 
            ordermaster.CustomerId= custid; //HERE CUSTID WE GET = CUSTOMERID FROM THE ORDERMASTER
            using (var httpClient=new HttpClient())
            {
                httpClient.BaseAddress= new Uri(BaseUrl);
                httpClient.DefaultRequestHeaders.Clear();
                StringContent content = new StringContent(JsonConvert.SerializeObject(ordermaster), Encoding.UTF8, "application/json");
                //AFTER WE SERIALIZE THE DATA OBJECT TO STRING 
                //in the view ordermaster table is shown we fill the cardnumber and amount we have to fill after that ordermaster id created new
                var response = await httpClient.PutAsync("api/Order", content); //(API/ORDER) CONTENT IS UPDATED ON THE DATABASE.
                //return RedirectToAction("GetAllProducts", "Products");
                if(ordermaster.AmountPaid==ordermaster.Total)
                {
                    return View("PaymentView");
                }    //it will goes to the payment view we created.
                else
                {
                    ViewBag.ErrorMessage = "Please Enter the Valid Amount";
                    return View();
                }
            }
        }
    }
}
