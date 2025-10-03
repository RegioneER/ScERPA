using ScERPA.Areas.Administration.Models;
using ScERPA.Areas.Administration.ViewModels;
using ScERPA.Models;
using ScERPA.Services;
using ScERPA.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ScERPA.Areas.Administration.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Area("Administration")]
    public class EServicesAreasController : Controller
    {
        private readonly ILogger<EServicesAreasController> _logger;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IAdministrationAreasServices _administrationAreasServices;

        private readonly ICommonServices _commonServices;

        public EServicesAreasController(ILogger<EServicesAreasController> logger, IHttpContextAccessor httpContextAccessor,  IAdministrationAreasServices administrationAreasServices, ICommonServices commonServices)
        {
            _logger = logger;
 
            _httpContextAccessor = httpContextAccessor;
     
            _administrationAreasServices = administrationAreasServices;

            _commonServices = commonServices;
        }

        public async Task<IActionResult> Index()
        {
            AreasListViewModel vm;
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value ?? "";

            _logger.LogInformation("L'utente {CurrentUserId} ha richiesto l'elenco delle Aree", currentUserID);


            _commonServices.CurrentMenu = "E-Services";
            ViewData["Title"] = "Elenco delle aree";

            vm = await _administrationAreasServices.GetAreasListViewModelAsync();
            
            return View(vm);
        }

        public async Task<IActionResult> Details(int Id)
        {
            AreaDetailsViewModel vm;
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value ?? "";

            _logger.LogInformation("L'utente {CurrentUserId} ha richiesto il dettaglio dell'area {Id}", currentUserID, Id);

            _commonServices.CurrentMenu = "E-Services";
            ViewData["Title"] = "Area";

            vm = await _administrationAreasServices.GetAreaViewModelAsync(Id);

            return View(vm);

        }

    }
}
