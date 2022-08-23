﻿using ClientAppBigBazzar.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;
using System.Text;
using CaptchaMvc.HtmlHelpers;
using Newtonsoft.Json.Linq;

namespace ClientAppBigBazzar.Controllers
{
    public class CustomersController : Controller
    {
       
        string BaseUrl = "https://localhost:7210/";
        [HttpGet]
        public IActionResult CustomerRegistration()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CustomerRegistration(Customers C)
        {
            TempData["AlertMessage"] = "You are Successfully register to our BigBazzar Platform Please Check your registered Mail Inbox...!";

            // Send Mail Confirmation for passsword
            var senderEmail = new MailAddress("rentalsystem3@gmail.com", "Sathiyan");

            var receiverEmail = new MailAddress(C.CustomerEmail, "Receiver");

            var password = "gusmmfohuxvryrfx";

            String b = "<a href=\"https://localhost:7241/Customers/CustomerLogin\">Here</a>";

            var sub = "Hello " + C.CustomerName + "! Welcome To BigBazzar";

            var body = "Your Login Mail Id: " + C.CustomerEmail + " And your Password is :" + C.Password + "Login" + b;



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

            ////validate google recaptcha here
            //var responses = Request["g-recaptcha-response"];
            //string secretKey = "6LfUKJ4hAAAAAARfSJgBTCH0O7wcNtXGp62UXhBl";
            //var client = new WebClient();
            //var result=client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}",secretKey,responses));
            //var obj=JObject.Parse(result);
            //var status = (bool)obj.SelectToken("success");
            //ViewBag.Message = status ? "Google reCaptcha validation success" : "Google reCaptcha validation failed";

            ////When you will post form for save data,you should check both the model validation and google recaptcha validation
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(BaseUrl);
                StringContent content = new StringContent(JsonConvert.SerializeObject(C), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("api/Customers",content);
                return RedirectToAction("CustomerLogin");
            }
        }
        [HttpGet]
        public IActionResult CustomerLogin()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CustomerLogin(Customers C)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                C.ConfirmPassword=C.Password;
                httpClient.BaseAddress = new Uri(BaseUrl);
                StringContent content = new StringContent(JsonConvert.SerializeObject(C), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("api/Customers/Login", content);

                if (response.IsSuccessStatusCode)
                {
                    var CustomerResponse = response.Content.ReadAsStringAsync().Result;
                    C = JsonConvert.DeserializeObject<Customers>(CustomerResponse);
                    //TempData["TraderId"] = C.CustomerId;
                    HttpContext.Session.SetInt32("CustomerId", C.CustomerId);
                    HttpContext.Session.SetString("CustomerName", C.CustomerName);

                    return RedirectToAction("GetAllProducts", "Products");
                }

                else
                {
                    ViewBag.ErrorMessage = "Invalid Credentials";
                    return View();  
                }

            }
        }

      

        public async Task<ActionResult> Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
