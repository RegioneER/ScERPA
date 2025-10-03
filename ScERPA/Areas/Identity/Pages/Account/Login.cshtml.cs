// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ScERPA.Models;
using System.Security.Claims;

using Serilog;
using ScERPA.Services.Interfaces;
using ScERPA.Data;
using Org.BouncyCastle.Tls.Crypto.Impl;
using System.Security.Principal;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace ScERPA.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly ICommonServices _commonServices;
        private readonly ScERPAContext _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger, IWebHostEnvironment environment, ICommonServices commonServices, ScERPAContext repository, IHttpContextAccessor httpContextAccessor,IConfiguration configuration)
        {
            _signInManager = signInManager;
            _logger = logger;
            _environment = environment;
            _commonServices = commonServices;
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            public string Username { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Devo ricordare il tuo account?")]
            public bool RememberMe { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {


            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
                _logger.LogError("Errore nel model di login {errore}", ErrorMessage);

            }

            //non voglio l'autologin quindi torno sempre alla home autenticata se va a buon fine il login
            //commento pertanto la gestione url di ritorno impostandola a fissa
            returnUrl ??= Url.Content("/Home/IndexAuthenticated");
            
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ReturnUrl = returnUrl;

            return Page();

            //Esempio di login dietro access manager (rimuovere il return Page(); sopra
            /*
            var headers = _httpContextAccessor?.HttpContext?.Request?.Headers;
            if(headers != null)
            {

                string username = "";
                string dominio = "";

                #if DEBUG  uso i secret per impersonificare una utenza 
                if (_environment.IsDevelopment())
                {
                    username = _configuration.GetSection("Impersonate").GetValue<string>("Username") ?? "";
                    dominio = _configuration.GetSection("Impersonate").GetValue<string>("Domain") ?? "";
                }
                #endif

                if (headers.ContainsKey("USERNAME") )
                {
                    username=headers["USERNAME"].ToString();
                } else if (_environment.IsDevelopment() && string.IsNullOrEmpty(username))
                    return Page();
                else if(string.IsNullOrEmpty(username))
                {
                    _logger.LogError("Manca username in header");
                    return LocalRedirect("/Home/Error");
                }
                    

                if (headers.ContainsKey("DOMAIN"))
                {
                    dominio=headers["DOMAIN"].ToString();
                } else if (_environment.IsDevelopment() && string.IsNullOrEmpty(dominio))
                    return Page();
                else if (string.IsNullOrEmpty(dominio))
                {
                    _logger.LogError("Manca domain in header");
                    return LocalRedirect("/Home/Error");
                }

                string usernameCompleto = $"{dominio}\\{username}";
                _logger.LogInformation("Inizio login di {UsernameCompleto}", usernameCompleto);
                

                if (!string.IsNullOrEmpty(usernameCompleto.Replace("\\","")))
                {

                    var user = await _signInManager.UserManager.FindByNameAsync(usernameCompleto);

                    IEnumerable<Tenant> listaTenantUtente = (from utente in _repository.Users
                                                             from tenant in utente.Tenants
                                                             where utente.Id == user.Id
                                                             select tenant);

                    

                    if (user != null && user.Attivo && listaTenantUtente.Any())
                    {

                        var claims = await _signInManager.UserManager.GetClaimsAsync(user);


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

                        if (!_signInManager.IsSignedIn(claimsPrincipal))
                        {
                            Thread.Sleep(1000);

                            await _signInManager.RefreshSignInAsync(user);
                            _logger.LogInformation("L'utente {Username} ha fatto login ma ho dovuto fare un refresh", usernameCompleto ?? "");
                        }
                        

                        _logger.LogInformation("L'utente {Username} ha fatto login.", usernameCompleto);
                       


                        return LocalRedirect(returnUrl);  
                      
                    }
                    else
                    {
                        _logger.LogError("utente {Username} inesistente oppure non attivo oppure senza tenant", usernameCompleto);
                        return Redirect("/Home/Error");
                    }
           

                }
                else
                {
                    if (_environment.IsDevelopment())
                        return Page();
                    else
                        return LocalRedirect("/Home/Error");
                }
            }
            else
            {
                _logger.LogWarning("in login non ho dati in header");
                if (_environment.IsDevelopment())
                    return Page();
                else
                    return LocalRedirect("/Home/Error");
            }
            */

        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("/Home/IndexAuthenticated");

            //ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var user = await _signInManager.UserManager.FindByNameAsync(Input.Username);
                IEnumerable<Tenant> listaTenantUtente = (from utente in _repository.Users
                                                         from tenant in utente.Tenants
                                                         where utente.Id == user.Id
                                                         select tenant);

                if (user != null && user.Attivo && listaTenantUtente.Any()) 
                {

                    var result = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                    
                    if (result.Succeeded )
                    {
                        user = await _signInManager.UserManager.FindByNameAsync(Input.Username);
                        var roles = await _signInManager.UserManager.GetRolesAsync(user);
                        var claims = await _signInManager.UserManager.GetClaimsAsync(user);
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
                            await _signInManager.UserManager.AddClaimAsync(user, new Claim("CurrentTenantId", listaTenantUtente.FirstOrDefault().Id.ToString()));
                        }
                        else
                        {
                            await _signInManager.UserManager.RemoveClaimAsync(user, claims.FirstOrDefault(c => c.Type == "CurrentTenantId"));
                            await _signInManager.UserManager.AddClaimAsync(user, new Claim("CurrentTenantId", listaTenantUtente.FirstOrDefault().Id.ToString()));
                        };
                        await _signInManager.CreateUserPrincipalAsync(user);
                        await _signInManager.RefreshSignInAsync(user);

                        _logger.LogInformation("L'utente {Username} ha fatto login.", Input.Username);
                        return LocalRedirect(returnUrl);
                    }
                    if (result.RequiresTwoFactor)
                    {
                        //return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                        return Redirect("/Home/Error");
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("utente {Username} bloccato", Input.Username);
                        //return RedirectToPage("./Lockout");
                        return Redirect("/Home/Error");
                    }
                    else
                    {
                        _logger.LogWarning("L'utente {Username} ha fallito il login.", Input.Username);
                        ModelState.AddModelError(string.Empty, "Tentativo di accesso fallito.");
                        return Page();
                    }
                } else
                {
                    _logger.LogWarning("utente {Username} inesistente oppure non attivo oppure senza tenant", Input.Username);
                    //return RedirectToPage("./AccessDenied");
                    return Redirect("/Home/Error");
                }
                


            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
