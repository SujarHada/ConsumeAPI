using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using ConsumeWebAPI.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;


namespace ContactsAPI.Controllers
{
    public class AccessController : Controller
    {

        string baseURL = "https://localhost:7127/api/";


        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }




        [HttpPost]
        public async Task<IActionResult> Login(VMLogin modelLogin)
        {



            IList<admin> users = new List<admin>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("Access");

                if (response.IsSuccessStatusCode)
                {
                    string results = await response.Content.ReadAsStringAsync();
                    users = JsonConvert.DeserializeObject<List<admin>>(results);
                }
                else
                {
                    Console.WriteLine("Error calling web API");
                }
            }



            foreach (var user in users)
            {
                string useremail = user.email;
                string userpw = user.passWord;

                if (modelLogin.Email == useremail && modelLogin.PassWord == userpw)

                {
                        List<Claim> claims = new List<Claim>()
                     {
                        new Claim(ClaimTypes.NameIdentifier, modelLogin.Email),
                        new Claim("OtherProperties","Example Role")
                     };

                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        AuthenticationProperties properties = new AuthenticationProperties()
                        {

                            AllowRefresh = true,
                            IsPersistent = modelLogin.KeepLoggedIn

                        };

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity), properties);

                        return RedirectToAction("Index", "Home");
                   
                }

                else
                {

                    TempData["ValidateMessage"] = "User not found";
                    return View();
                }
            }
            return View(); 
        }
    }
}
