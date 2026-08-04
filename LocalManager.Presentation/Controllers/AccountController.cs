using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LocalManager.Presentation.Controllers
{
    /// <summary>
    /// Login temporal con 2 cuentas fijas, únicamente para restringir el acceso
    /// mientras el proyecto se expone en la nube (Cloudflare Tunnel) antes de
    /// implementar un sistema de usuarios real (ver TODO al final de la clase).
    /// </summary>
    [AllowAnonymous]
    public class AccountController : Controller
    {
        // Cuentas fijas de acceso. Si en algún momento se agrega un usuario más
        // o se cambia una contraseña, solo hay que editar este diccionario.
        private static readonly Dictionary<string, string> CuentasPermitidas = new(StringComparer.OrdinalIgnoreCase)
        {
            { "jesus.uc@gmail.com", "1805" },
            { "pedrozo.kun@gmail.com", "4321" },
        };

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToLocal(returnUrl);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Ingresa correo y contraseña.");
                return View();
            }

            if (!CuentasPermitidas.TryGetValue(email, out var passwordCorrecta) || passwordCorrecta != password)
            {
                ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
                return View();
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, email),
                new(ClaimTypes.Email, email),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = true });

            return RedirectToLocal(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        // TODO: reemplazar este login fijo por un sistema de usuarios en base de datos
        // (tabla Usuario + hash de contraseña) cuando se necesite dar acceso a más personas.
    }
}
