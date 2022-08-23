using ClientAppBigBazzar.Helper;
using ClientAppBigBazzar.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace ClientAppBigBazzar.Controllers
{
    public class CustomerRegController : Controller
    {
        private IConfiguration _configuration;
        public CustomerRegController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        string BaseUrl = "https://localhost:7210/";
        public async Task<IActionResult> GetAllCustomers()
        {
            List<CustomerReg> CustomerInfo = new List<CustomerReg>();

            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("api/Login");

                if (Res.IsSuccessStatusCode)
                {
                    var CustomerResponse = Res.Content.ReadAsStringAsync().Result;
                    CustomerInfo = JsonConvert.DeserializeObject<List<CustomerReg>>(CustomerResponse);

                }
                return View(CustomerInfo);
            }
        }
        [HttpGet]
        public IActionResult Create()
        {
            Random rnd = new Random();
            ViewBag.captcha1 = rnd.Next(0, 20);// returns random integers
            ViewBag.captcha2 = rnd.Next(10, 20);
            ViewBag.resultCaptcha = ViewBag.captcha1 + ViewBag.captcha2;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(RegisterForm reg)
        {
            EmailHelper main = new EmailHelper(_configuration);
            //var EncodePassword= GetMd5Hash(reg.Password);
            //reg.Password=EncodePassword;
            CustomerReg Uobj = new CustomerReg();
            //Assign the value for this fields
            Uobj.CustomerName = reg.CustomerName;

            Uobj.CustomerEmail = reg.EmailId;
            Uobj.Password = reg.Password;
            Uobj.ConfirmPassword = reg.ConfirmPassword;
            Uobj.SecurityQuestion = reg.SecurityQuestion;
            Uobj.Answer = reg.Answer;
            Uobj.Status = false;
            Uobj.CreationDate = DateTime.Now;
            Uobj.SecurityCode = Helper.RandomHelper.RandomString(6);



            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://localhost:7210/");
                StringContent content = new StringContent(JsonConvert.SerializeObject(Uobj), Encoding.UTF8, "application/json");
                if (reg.Captcha == reg.resultCaptcha)
                {
                    using (var response = await httpClient.PostAsync("api/Login", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {
                            //string apiResponse = await response.Content.ReadAsStringAsync();
                            //string result = JsonConvert.DeserializeObject<string>(apiResponse);
                            try
                            {

                                //sending email to user
                                string body = "<!DOCTYPE html>" +
                                                "<html> " +
                                                    "<body style=\"background -color:#ff7f26;text-align:center;\"> " +
                                                    "<h3 style=\"color:#051a80;\">Welcome </h3> " + Uobj.CustomerName +
                                                    "<h3>Thankyou for registration!</h3>" +
                                                    "<h3>Security Code: </h3>" + Uobj.SecurityCode +
                                                    "<h4>Please click on the following link to verify your account.</h4> " +
                                                    "<a href='https://localhost:7210/CustomerReg/Activate'>Verify</a>" +
                                "</body> " +
                                                "</html>";


                                main.Send(_configuration["Gmail:Username"], Uobj.CustomerEmail, "Successfully Registered!", body);
                                HttpContext.Session.SetString("username", Uobj.CustomerName);
                                HttpContext.Session.SetString("purpose", "login");
                                return View("AccountActivate");


                            }
                            catch
                            {

                                Random rnd = new Random();
                                ViewBag.captcha1 = rnd.Next(0, 20);// returns random integers >= 10 and < 19
                                ViewBag.captcha2 = rnd.Next(10, 20);
                                ViewBag.resultCaptcha = ViewBag.captcha1 + ViewBag.captcha2;
                                reg.EmailId = "";
                                return View("RegisterForm", reg);
                            }

                        }
                        else
                        {

                            Random rnd = new Random();
                            ViewBag.captcha1 = rnd.Next(0, 20);// returns random integers >= 10 and < 19
                            ViewBag.captcha2 = rnd.Next(10, 20);
                            ViewBag.resultCaptcha = ViewBag.captcha1 + ViewBag.captcha2;

                            reg.CustomerName = null;
                            reg.Captcha = null;
                            return View("RegisterForm", reg);
                        }

                    }
                }
                else
                {
                    Random rnd = new Random();
                    ViewBag.captcha1 = rnd.Next(0, 20);// returns random integers >= 10 and < 19
                    ViewBag.captcha2 = rnd.Next(10, 20);
                    ViewBag.resultCaptcha = ViewBag.captcha1 + ViewBag.captcha2;
                    ViewBag.captchaError = "Invalid Captcha";
                    return View("RegisterForm", reg);
                }
            }
        }
        [HttpGet]
        public IActionResult Activate()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Activate(string securitycode)
        {
            List<CustomerReg> CustomerInfo = new List<CustomerReg>();

            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await client.GetAsync("api/UserSetups");

                if (Res.IsSuccessStatusCode)
                {
                    var CustomerResponse = Res.Content.ReadAsStringAsync().Result;
                    CustomerInfo = JsonConvert.DeserializeObject<List<CustomerReg>>(CustomerResponse);

                }
            }
            CustomerReg obj = null;
            foreach (var i in CustomerInfo)
            {

                if (i.SecurityCode == securitycode)
                {
                    int seconds = DateTime.Now.Subtract((DateTime)i.CreationDate).Seconds;
                    if (seconds < 60)
                    {
                        obj = i;
                    }


                }
            }
            if (obj != null)
            {

                if (HttpContext.Session.GetString("purpose") == "login")
                {
                    obj.Status = true;
                    using (var httpClient = new HttpClient())
                    {
                        string username = obj.CustomerName;
                        StringContent content1 = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
                        using (var response = await httpClient.PutAsync("https://localhost:7210/api/Login/" + username, content1))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                try
                                {
                                    string body = "Dear " + obj.CustomerName + "\nYour account is activated!\nNow you can use your user credentials to log in to your account";
                                    EmailHelper main = new EmailHelper(_configuration);
                                    main.Send(_configuration["Gmail:username"], obj.CustomerEmail, "Verification Process Done!", body);

                                    return RedirectToAction("Login", "Login");
                                }
                                catch
                                {

                                    return View();
                                }

                            }
                            else
                            {

                                return View();
                            }
                        }

                    }
                }
                else if (HttpContext.Session.GetString("purpose") == "reset")
                {
                    using (var httpClient = new HttpClient())
                    {
                        string username = obj.CustomerName;
                        StringContent content1 = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
                        using (var response = await httpClient.PutAsync("https://localhost:7210/api/Login/" + username, content1))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                try
                                {
                                    //string body = "You have requested to reset your password\nNow you can update your password";
                                    //MainHelper main = new MainHelper(_configuration);
                                    //main.Send(_configuration["Gmail:username"], obj.EmailId, "Reset-Password Approved!", body);
                                    //_notyf.Success("Successfully verified", 3);
                                    HttpContext.Session.SetString("username", obj.CustomerName);
                                    return RedirectToAction("ResetPassword", "CustomerReg");

                                }
                                catch
                                {

                                    return View();
                                }

                            }
                            else
                            {
                                return View();
                            }
                        }

                    }

                }

            }
            else
            {
                ViewBag.invalidCode = "Invalid Security code!";
                return View();
            }
            return View();
        }

    }
}

