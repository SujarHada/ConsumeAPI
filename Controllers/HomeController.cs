using ConsumeWebAPI.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using System.Net;
using System.Numerics;


namespace ConsumeWebAPI.Controllers
{
  
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private static string baseURL = "https://localhost:7127/api/";

        public object Id { get; private set; }

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

        }



        public async Task<IActionResult> Index()
        {
            IList<UserEntity> users = new List<UserEntity>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("Contacts");

                if (response.IsSuccessStatusCode)
                {
                    string results = await response.Content.ReadAsStringAsync();
                    users = JsonConvert.DeserializeObject<List<UserEntity>>(results);
                }
                else
                {
                    Console.WriteLine("Error calling web API");
                }
            }

            return View(users);
        }

        public IActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(UserEntity user)
        {
            UserEntity obj = new UserEntity()
            {
                fullName = user.fullName,
                email = user.email,
                phone = user.phone,
                address = user.address
            };

            if (user.fullName != null)
            {
                using (var client = new HttpClient())
                {
                    //put the ("+ put API name here ")
                    client.BaseAddress = new Uri(baseURL + "Contacts");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // put the ("put action name here (exmaple what ever is after /Contacts )", "obj being passed")

                    HttpResponseMessage response = await client.PostAsJsonAsync("", obj);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        Console.WriteLine("Error calling web API");
                    }
                }
            }

            return View();
        }


        public async Task<IActionResult> DeleteContact(Guid id)
               
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL + "Contacts/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));



                HttpResponseMessage response = await client.DeleteAsync($"Delete/{id}");

                if (response.IsSuccessStatusCode)
                {


                    return RedirectToAction("Index", "Home");
                }
                else
                {
                   
                    return NotFound();
                }
                

            }
        }

        [HttpGet]
        public async Task<IActionResult> Update([FromRoute] Guid id)
        {
            ForUpdate updateItem = new();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL + "Contacts/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage getData = await client.GetAsync($"GetContact/{id}");
                if (getData.IsSuccessStatusCode)
                {
                    string dataResult = getData.Content.ReadAsStringAsync().Result;
                    var item = JsonConvert.DeserializeObject<UserEntity>(dataResult);
                    updateItem = new ForUpdate()
                    {
                        fullName = item.fullName,
                        email = item.email,
                        phone = item.phone,
                        address = item.address
                    };

              
                }
                else
                {
                    Console.WriteLine("Error calling API.");
                }
            }
            return View(updateItem);
        }

        [HttpPost]

        public async Task<IActionResult> Update([FromRoute] Guid id,ForUpdate forUpdate)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL + "Contacts/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage getData = await client.PostAsJsonAsync($"Update/",forUpdate);
                if (getData.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return NotFound();
                }
            }

        }






        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Access");

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
