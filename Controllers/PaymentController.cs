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
            //It should be reflected on the customer buy in the ordercontroller in api(CustomerBuy)
        {
            var custid = HttpContext.Session.GetInt32("CustomerId");
            OrderMasters om = new OrderMasters();
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(BaseUrl);
                StringContent content = new StringContent(JsonConvert.SerializeObject(custid), Encoding.UTF8, "application/json");
                HttpResponseMessage Res = await httpClient.PostAsync("api/Order/"+custid,content);
                if (Res.IsSuccessStatusCode)
                {
                    var Response = Res.Content.ReadAsStringAsync().Result;
                    om = JsonConvert.DeserializeObject<OrderMasters>(Response);
                }
            }
            return RedirectToAction("Transaction", new { id = om.OrderMasterId });

        }
        [HttpPost]
        public async Task<OrderMasters> GetOrderMasterByOrderMasterId(int id) //for updating the ordermaster by the ordermasterid
            
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
                }
            }
            return om;
        }
        [HttpGet]
        public async Task<IActionResult> Transaction(int id)
        {
            OrderMasters om=await GetOrderMasterByOrderMasterId(id);
            return View(om);
        }
        [HttpPost]
        public async Task<IActionResult> Transaction(OrderMasters ordermaster)
        {
            var custid = HttpContext.Session.GetInt32("CustomerId");
            ordermaster.CustomerId= custid;
            using (var httpClient=new HttpClient())
            {
                httpClient.BaseAddress= new Uri(BaseUrl);
                httpClient.DefaultRequestHeaders.Clear();
                StringContent content = new StringContent(JsonConvert.SerializeObject(ordermaster), Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync("api/Order", content);
                return RedirectToAction("GetAllProducts", "Products");
            }
        }
    }
}
