using ClientAppBigBazzar.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace ClientAppBigBazzar.Controllers
{
    public class AdminController : Controller
    {
        string BaseUrl = "https://localhost:7210/";

        [HttpGet]
        public async Task<ActionResult> AdminLogin()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> AdminLogin(Admin A)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                A.ConfirmPassword = A.AdminPassword;
                httpClient.BaseAddress = new Uri(BaseUrl);
                StringContent content = new StringContent(JsonConvert.SerializeObject(A), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("api/Admin/AdminLogin", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("GetAllCategory");
                }
                else
                {
                    ViewBag.ErrorMessage = "Invalid Credentials";
                    return View(A);
                }
            }
        }
        public async Task<ActionResult> GetAllCategory()
        {
            List<Categories> c=new List<Categories>();
            using (var Client=new HttpClient())
            {
                Client.BaseAddress=new Uri(BaseUrl);
                Client.DefaultRequestHeaders.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage Res = await Client.GetAsync("api/Admin");
                if(Res.IsSuccessStatusCode)
                {
                    var Response=Res.Content.ReadAsStringAsync().Result;
                    c = JsonConvert.DeserializeObject<List<Categories>>(Response);
                }
                return View(c);
            }
        }
        [HttpGet]
        public async Task<ActionResult> AddCategory()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> AddCategory(Categories category)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(BaseUrl);
                httpClient.DefaultRequestHeaders.Clear();
                StringContent content = new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("api/Admin/AddCategory", content);
                return RedirectToAction("GetAllCategory");
            }
        }
    }
}
