using System.Diagnostics;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using WebClient2.Data;
using WebClient2.Models;

namespace WebClient2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Login()
        { 
             return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            if (ModelState.IsValid)
            {
                // validate user
                ViewBag.ShowErr = false;
                try
                {
                    if (ValidateUser(user).Result)
                    {
                        
                        return Redirect("/Patien/Index");
                    }
                    else
                    {
                        ViewBag.ShowErr = true;
                        ViewBag.ErrorMessage = "Login Failed! Incorrect Username or Password. Please contact Admin";
                        return View();
                    }
                }
                catch (Exception ex)
                { 
                    return View();
                }
            }
            return View();
        }

        private async Task<bool> ValidateUser(User user)
        {
            // https://localhost:7127/api/Users/ValidateUser
            using (var client = new HttpClient())
            {
                // TODO: move to appsettings
                client.BaseAddress = new Uri("https://localhost:7127/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var txtBytesU = System.Text.Encoding.UTF8.GetBytes(user.Username);
                var txtBytesP = System.Text.Encoding.UTF8.GetBytes(user.Password);

                var req = new User() { Username = Convert.ToBase64String(txtBytesU), Password = Convert.ToBase64String(txtBytesP)};
                HttpContext.Session.SetString("username", user.Username);
                HttpContext.Session.SetString("password", user.Password);


                HttpResponseMessage response = await client.PostAsJsonAsync("api/Users/ValidateUser", req);

                if (response.IsSuccessStatusCode)
                {
                    
                    var c = response.Content.ReadAsStringAsync().Result;

                    return bool.Parse(c);
                }
            }

            return false;
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}