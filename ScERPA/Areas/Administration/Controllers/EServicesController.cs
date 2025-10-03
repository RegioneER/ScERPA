using ScERPA.Areas.Administration.ViewModels;
using ScERPA.Controllers;
using ScERPA.Models;
using ScERPA.Services;
using ScERPA.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ScERPA.Areas.Administration.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin,Manager")]
    [Area("Administration")]
    public class EServicesController : Controller
    {
        private readonly ILogger<EServicesController> _logger;
        private readonly IUtilities _utilities;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICommonServices _commonServices;
        private readonly IAdministationEServicesServices _administationEServicesServices;

        public EServicesController(ILogger<EServicesController> logger, IHttpContextAccessor httpContextAccessor, IUtilities utilities, ICommonServices commonServices, IAdministationEServicesServices administationEServicesServices)
        {
            _logger = logger;
            _utilities = utilities;
            _httpContextAccessor = httpContextAccessor;
            _commonServices = commonServices;
            _administationEServicesServices = administationEServicesServices;
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Index()
        {

            EServicesListViewModel vm;

            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value ?? "";

            _logger.LogInformation("L'utente {CurrentUserId} ha richiesto l'elenco dei Servizi", currentUserID);

            _commonServices.CurrentMenu = "E-Services";
            ViewData["Title"] = "Elenco dei servizi";

            vm = await _administationEServicesServices.GetEServicesListViewModelAsync();
            
            return View(vm);
        }

        public async Task<IActionResult> Details(int Id)
        {
            EServiceDetailsViewModel vm;

            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value ?? "";

            _logger.LogInformation("L'utente {CurrentUserId} ha richiesto il dettaglio dell'area {Id}", currentUserID, Id);

            _commonServices.CurrentMenu = "E-Services";
            ViewData["Title"] = "Servizio";

            vm = await _administationEServicesServices.GetEServiceDetailsViewModelAsync(Id);

            return View(vm);

        }

    }
}
