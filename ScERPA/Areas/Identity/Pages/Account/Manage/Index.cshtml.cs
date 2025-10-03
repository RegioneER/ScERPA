// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ScERPA.Models;
using ScERPA.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Azure;

namespace ScERPA.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ICommonServices _commonServices;
        private readonly IUtilities _utilities;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ICommonServices commonServices,
            IUtilities utilities,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _commonServices = commonServices;
            _utilities = utilities;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        public string Ruoli { get; set; }

        public string Tenant { get; set; }

        public string Ip { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);


            Username = userName;
            var listaRuoli = await _userManager.GetRolesAsync(user);
            Ruoli = string.Join(",", listaRuoli.ToArray());
            var claims = await _userManager.GetClaimsAsync(user);
            int currentTenantId = int.Parse("0" + claims.FirstOrDefault(t => t.Type == "CurrentTenantId")?.Value);
            var currrentTenant = _commonServices.TenantCorrente(currentTenantId);
            Tenant = currrentTenant is null ? "Nessun tenant impostato" : _commonServices.TenantCorrente(int.Parse("0" + claims.FirstOrDefault(t => t.Type == "CurrentTenantId").Value)).Nome;
            Ip = _utilities.GetRemoteIPV4(Request?.HttpContext?.Connection?.RemoteIpAddress?.ToString());

        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Impossibile trovare l'utente con ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }


    }
}
