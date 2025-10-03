// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Security.Principal;
using System.Threading.Tasks;
using ScERPA.Models;
using ScERPA.Services;
using ScERPA.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Net.NetworkInformation;

namespace ScERPA.Areas.Identity.Pages.Account
{
    [Authorize]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly ICommonServices _commonServices;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger, ICommonServices commonServices)
        {
            _signInManager = signInManager;
            _logger = logger;
            _commonServices = commonServices;
        }
        
        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            string username;

            username = _signInManager.UserManager.GetUserName(User);

            await _signInManager.SignOutAsync();

            HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);

            _logger.LogInformation("Utente {Username} uscito.", username);

            return RedirectToAction("LogoutToApplicationList","Home");

        }
    }
}
