using ExemploLogin.Data;
using ExemploLogin.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ExemploLogin.Controllers
{

    public class LoginController : Controller
    {
        private readonly DataContext dataContext;
        public LoginController(DataContext dc)
        {
            dataContext = dc;
        }
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated) {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Login login)
        {
            if (ModelState.IsValid)
            {
                bool fazerLogin = dataContext.Logins.Any(x => x.Email == login.Email && x.Senha == login.Senha);

                if (fazerLogin == true)
                {
                    Login usuarioBanco = dataContext.Logins.FirstOrDefault(x => x.Email == login.Email);

                    List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, usuarioBanco.ID.ToString()),
                new Claim(ClaimTypes.Name, usuarioBanco.Email),
                new Claim(ClaimTypes.Role, "A"),
            };

                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    AuthenticationProperties authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                        IsPersistent = true,
                    };

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Erro = "Usuário e/ou senha inválidos";
            return View();
        }

        public IActionResult Logoff()
        {
            HttpContext.SignOutAsync(); //Sair do Sistema
            return RedirectToAction("Index"); //Redireciona oara a ação Index da controller
                                              //
        }
    }
}
