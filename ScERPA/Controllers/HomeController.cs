using ScERPA.Models;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using ScERPA.Services.Interfaces;
using ScERPA.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Principal;
using System.Security.Claims;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using ScERPA.Services;
using NuGet.Protocol.Core.Types;
using ScERPA.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using iText.Pdfua.Checkers.Utils;

namespace ScERPA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUtilities _utilities;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ICommonServices _commonServices;
        private readonly ScERPAContext _repository;

        public HomeController(ILogger<HomeController> logger,IUtilities utilities, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ICommonServices commonServices, ScERPAContext repository)
        {
            _logger = logger;
            _utilities = utilities;
            _userManager = userManager;
            _signInManager = signInManager;
            _commonServices = commonServices;
            _repository = repository;
        }


        [AllowAnonymous]
        public IActionResult Index()
        {

            string userLocation = _utilities.GetRemoteIPV4(Request?.HttpContext?.Connection?.RemoteIpAddress?.ToString());

            _commonServices.CurrentMenu = "";

            TempData["remoteIp"] = userLocation;

            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction(nameof(IndexAuthenticated));
            }

            return View();
        }


        [AllowAnonymous]
        public async Task<IActionResult> LoginWithIAM()
        {
            //provo a fare il login con IAM
            var headers = HttpContext?.Request?.Headers;
            if (headers != null)
            {

                string username = "";
                string dominio = "";


                if (headers.ContainsKey("USERNAME") )
                {
                    username = headers["USERNAME"].ToString();
                }
                else if(string.IsNullOrEmpty(username))
                {
                    _logger.LogError("Manca username in header");
                    return RedirectToAction(nameof(Error));
                }
                    

                if (headers.ContainsKey("DOMAIN"))
                {
                    dominio = headers["DOMAIN"].ToString();
                }
                else if (string.IsNullOrEmpty(dominio))
                {
                    _logger.LogError("Manca dominio in header");
                    return RedirectToAction(nameof(Error));
                }

                string usernameCompleto = $"{dominio}\\{username}";

                if (!string.IsNullOrEmpty(usernameCompleto.Replace("\\", "")))
                {
                    var user = await _signInManager.UserManager.FindByNameAsync(usernameCompleto);

                    IEnumerable<Tenant> listaTenantUtente = (from utente in _repository.Users
                                                             from tenant in utente.Tenants
                                                             where utente.Id == user.Id
                                                             select tenant);


                    if (user != null && user.Attivo && listaTenantUtente.Any())
                    {
                        var claims = await _signInManager.UserManager.GetClaimsAsync(user);

                        if (!claims.Any(c => c.Type == ClaimTypes.NameIdentifier))
                        {
                            await _signInManager.UserManager.AddClaimAsync(user, new Claim(ClaimTypes.NameIdentifier, Convert.ToString(user.Id)));
                        };
                        if (!claims.Any(c => c.Type == ClaimTypes.Name))
                        {
                            await _signInManager.UserManager.AddClaimAsync(user, new Claim(ClaimTypes.Name, Convert.ToString(user.UserName??"")));
                        };
                        if (!claims.Any(c => c.Type == ClaimTypes.Surname))
                        {
                            await _signInManager.UserManager.AddClaimAsync(user, new Claim(ClaimTypes.Surname, user.Cognome));
                        };
                        if (!claims.Any(c => c.Type == ClaimTypes.GivenName))
                        {
                            await _signInManager.UserManager.AddClaimAsync(user, new Claim(ClaimTypes.GivenName, user.Nome));
                        };
                        if (!claims.Any(c => c.Type == "CurrentTenantId"))
                        {
                            await _signInManager.UserManager.AddClaimAsync(user, new Claim("CurrentTenantId", listaTenantUtente?.FirstOrDefault()?.Id.ToString()));
                        }
                        else
                        {
                            await _signInManager.UserManager.RemoveClaimAsync(user, claims.FirstOrDefault(c => c.Type == "CurrentTenantId"));
                            await _signInManager.UserManager.AddClaimAsync(user, new Claim("CurrentTenantId", listaTenantUtente?.FirstOrDefault()?.Id.ToString()));
                        };


                        var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(user);

                        await _signInManager.Context.SignInAsync(IdentityConstants.ApplicationScheme,
                                                                claimsPrincipal,
                                                                new AuthenticationProperties { IsPersistent = false });

                        await _signInManager.UserManager.GetRolesAsync(user);

                        _logger.LogInformation("L'utente {Username} ha fatto login.", usernameCompleto);

                    }
                    else
                    {
                        _logger.LogWarning("utente {Username} inesistente oppure non attivo oppure senza tenant", usernameCompleto);
                        return Redirect("/Home/Error");
                    }

                }


                return RedirectToAction(nameof(IndexAuthenticated));
            }

            _logger.LogError("impossibile autenticare con IAM");
            return RedirectToAction(nameof(Error));
        }

        [Authorize]
        [ResponseCache(NoStore = true, Duration = 0)]
        public async Task<IActionResult> IndexAuthenticated()
        {
            string userLocation = _utilities.GetRemoteIPV4(Request?.HttpContext?.Connection?.RemoteIpAddress?.ToString());
           
            
            if(!_signInManager.IsSignedIn(User))
            {
                _logger.LogDebug("L'utente risulta loggato in IndexAuthenticated");
            }
            _commonServices.CurrentMenu = "";

            TempData["remoteIp"] = userLocation;

            HttpContext.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate"); // HTTP 1.1.
            HttpContext.Response.Headers.Append("Pragma", "no-cache"); // HTTP 1.0.
            HttpContext.Response.Headers.Append("Expires", "0"); // Proxies.

            return View();
        }

        [Authorize(Roles ="SuperAdmin")]
        public async Task<IActionResult> SuperAdminChangeCurrentTenant(int newCurrentTenantId)
        {
            string? userId = _userManager.GetUserId(User);
            ApplicationUser? user = await _userManager.FindByIdAsync(userId ?? "");
            if(user is not null)
            {
                var claims = await _signInManager.UserManager.GetClaimsAsync(user);

                if (!claims.Any(c => c.Type == "CurrentTenantId"))
                {
                    await _signInManager.UserManager.AddClaimAsync(user, new Claim("CurrentTenantId", newCurrentTenantId.ToString()));
                }
                else
                {
                    await _signInManager.UserManager.RemoveClaimAsync(user, claims.First(c => c.Type == "CurrentTenantId"));
                    await _signInManager.UserManager.AddClaimAsync(user, new Claim("CurrentTenantId", newCurrentTenantId.ToString()));
                }
                await _signInManager.RefreshSignInAsync(user);
            }


            return RedirectToAction(nameof(IndexAuthenticated));
        }


        [Route("/Home/Error")]
        [Route("/Error")]
        [HttpGet, HttpPost, HttpHead, HttpPut]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [IgnoreAntiforgeryToken]
        [AllowAnonymous]
        public IActionResult Error()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>();

            _commonServices.CurrentMenu = "";
            _logger.LogCritical("Errore: {Messaggio} {Stacktrace}",exception?.Error.Message.ToString(), exception?.Error?.StackTrace?.ToString());
            HttpContext.Response.StatusCode = 200;
            return View(new ViewModels.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [AllowAnonymous]
        [ResponseCache(NoStore = true, Duration = 0)]
        public IActionResult LogoutToApplicationList()
        {
            var returnUrl = _commonServices.LogoutUrl;

            HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
            
            return Redirect(returnUrl);
        }
    }
}