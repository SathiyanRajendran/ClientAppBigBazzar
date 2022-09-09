using ClientAppBigBazzar.Models;
using ClientAppBigBazzar.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ClientAppBigBazzar.Controllers
{
    public class TradersController : Controller
    {
        string BaseUrl = "https://localhost:7210/";
        public async Task<IActionResult> GetAllTraders()
        {
            List<Traders> traders = new List<Traders>();
            using (var Client=new HttpClient())
            {
                Client.BaseAddress = new Uri(BaseUrl);
                Client.DefaultRequestHeaders.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await Client.GetAsync("api/Traders");
                if (Res.IsSuccessStatusCode)
                {
                    var TraderResponse = Res.Content.ReadAsStringAsync().Result;
                    traders = JsonConvert.DeserializeObject<List<Traders>>(TraderResponse);
                }
                return View(traders);
            }
        }
        [HttpGet]
        public IActionResult TraderRegistration()
        {
            return View();
        }
        [HttpPost]
        public async Task <IActionResult> TraderRegistration(Traders traders)
        {
            TempData["AlertMessage"] = "You are Successfully register to our BigBazzar Platform Please Check your registered Mail Inbox...!";

            // Send Mail Confirmation for Login
            var senderEmail = new MailAddress("rentalsystem3@gmail.com", "BigBazzar");

            var receiverEmail = new MailAddress(traders.TraderEmail, "Receiver");

            var password = "gusmmfohuxvryrfx";

            String b = "<a href=\"https://localhost:7241/Traders/TraderLogin\">Here</a>";

            var sub = "Hello " + traders.TraderName + "! Welcome To BigBazzar";

            var body = "Your Login Mail Id: " + traders.TraderEmail + " And your Password is :" + traders.Password + "Login" + b;



            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderEmail.Address, password)
            };
            using (var mess = new MailMessage(senderEmail, receiverEmail)
            {
                Subject = sub,
                Body = body
            })
            {
                smtp.Send(mess);
                ViewBag.Message = String.Format("Registered Successfully!!\\ Please Check Your Mail to Login.");
            }



            //------------------------------------------------------------------------------------------------------------------

            using (var httpClient=new HttpClient())
            {
                httpClient.BaseAddress = new Uri(BaseUrl);
                StringContent content = new StringContent(JsonConvert.SerializeObject(traders), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("api/Traders", content);
                return RedirectToAction("GetAllTraders");

            }
        }
        [HttpGet]
        public async Task<IActionResult> UpdateTrader(int id)  //HTTPPUT
        {
            Traders t = new Traders();
            using (var httpClient = new HttpClient())
            {

                using (var response = await httpClient.GetAsync("https://localhost:7210/api/Traders/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    t = JsonConvert.DeserializeObject<Traders>(apiResponse);
                }
            }
            return View(t);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateTrader(Traders t)
        {
            //var v = new List<Products>();
            //t.Products = v;
            Traders p1 = new Traders();
            using (var httpClient = new HttpClient())
            {
                int id = t.TraderId;
                StringContent content1 = new StringContent(JsonConvert.SerializeObject(t), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PutAsync("https://localhost:7210/api/Traders/" + id, content1))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    ViewBag.Result = "Success";
                    p1 = JsonConvert.DeserializeObject<Traders>(apiResponse);
                }
            }
            return RedirectToAction("GetAllTraders");
        }
        [HttpGet]
        public async Task<IActionResult> DeleteTrader(int id)   //HTTPDELETE
        {
            TempData["TraderId"] = id; //Tempdata is used to transfer data from one action to another action method
                                   //in same or different controller.
                                   //from the view to the controller.
                                   //from the controler to the view.
            Traders e = new Traders();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:7210/api/Traders/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    e = JsonConvert.DeserializeObject<Traders>(apiResponse);

                }
            }
            return View(e);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteTrader(Traders p)
        {
            int trid = Convert.ToInt32(TempData["TraderId"]);
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync("https://localhost:7210/api/Traders/" + trid))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                }
            }
            return RedirectToAction("GetAllTraders");
        }
        [HttpGet]
        public async Task<IActionResult> TraderLogin()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> TraderLogin(Traders T)
        {
            //----------------------
            TraderToken Tt = new TraderToken();
            //--------------------------
            using (HttpClient httpClient = new HttpClient())
            {
                T.ConfirmPassword = T.Password;
                httpClient.BaseAddress = new Uri(BaseUrl);
                StringContent content = new StringContent(JsonConvert.SerializeObject(T), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("api/Traders/Login", content);

                if (response.IsSuccessStatusCode)
                {
                    var TraderResponse = response.Content.ReadAsStringAsync().Result;
                    //--------------------------
                    Tt = JsonConvert.DeserializeObject<TraderToken>(TraderResponse);
                    if (Tt == null ||Tt.traders==null)
                    {
                        ViewBag.ErrorMessage = "Invalid Credentials";
                        return View();
                    }
                    //--------------------------
                    //TempData["TraderId"] = Tt.traders.TraderId;
                    HttpContext.Session.SetInt32("TraderId", Tt.traders.TraderId); //this is for view purpose what are the products addded by the traders
                   
                    
                    
                    HttpContext.Session.SetString("TraderName",Tt.traders.TraderName);  //this is for the view purpose after login
                    //-------------------------
                    string token = Tt.Token;
                    HttpContext.Session.SetString("token", token);
                    //-------------------------
                    return RedirectToAction("GetProductByTrader","Products");
                }
                else
                {
                    ViewBag.ErrorMessage="Invalid Credentials";
                    return View();
                }
            }
        }
    }
}

