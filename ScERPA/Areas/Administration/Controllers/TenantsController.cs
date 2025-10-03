using ScERPA.Areas.Administration.Models;
using ScERPA.Areas.Administration.ViewModels;
using ScERPA.Models;
using ScERPA.Models.Exceptions;
using ScERPA.Services;
using ScERPA.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace ScERPA.Areas.Administration.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Area("Administration")]
    public class TenantsController : Controller
    {
        private readonly ILogger<TenantsController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAdministrationTentantsServices _administrationTentantServices;
        private readonly ICommonServices _commonServices;

        public TenantsController(ILogger<TenantsController> logger, IHttpContextAccessor httpContextAccessor, IAdministrationTentantsServices administrationTentantServices,ICommonServices commonServices)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;        
            _administrationTentantServices = administrationTentantServices;
            _commonServices = commonServices;
        }
        public async Task<IActionResult> Index()
        {
            TenantsListViewModel vm;

            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value ?? "";

            _logger.LogInformation("L'utente {CurrentUserId} ha richiesto l'elenco dei tenant", currentUserID);

            _commonServices.CurrentMenu = "Amministrazione";
            ViewData["Title"] = "Elenco dei tenants";

            vm =  await _administrationTentantServices.GetTenantsListViewModelAsync();

            return View(vm);
        }

        public async Task<IActionResult> Details(int Id)
        {
            TenantDetailsViewModel vm;
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value ?? ""; 
            
            _logger.LogInformation("L'utente {CurrentUserId} ha richiesto il dettaglio del tenant {Id}", currentUserID, Id);

            _commonServices.CurrentMenu = "Amministrazione";
            ViewData["Title"] = "Tenant";

            vm = await _administrationTentantServices.GetTenantDetailsViewModelAsync(Id);

            return View(vm);

        }

        public async Task<IActionResult> Create()
        {
            TenantEditCreateViewModel vm;
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value ?? "";

            _logger.LogInformation("L'utente {CurrentUserId} ha richiesto iniziato la creazione di un nuovo tenant", currentUserID);

            _commonServices.CurrentMenu = "Amministrazione";
            ViewData["Title"] = "Nuovo tenant";

            vm = await _administrationTentantServices.GetTenantEditCreateViewModelAsync(null,null);

            return View(vm);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TenantEditInputModel inputModel)
        {
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value ?? "";

            _logger.LogInformation("L'utente {CurrentUserId} sta provando a salvare il nuovo tenant", currentUserID);

            _commonServices.CurrentMenu = "Amministrazione";
            ViewData["Title"] = "Nuovo tenant";

            if (ModelState.IsValid)
            {
                int id = await _administrationTentantServices.CreateTenantAsync(inputModel) ;
                TempData["EsitoPositivo"] = $"Creato il seguente Tenant";
                return RedirectToAction(nameof(Details), new { Id = id });
            }
            
            var vm = await _administrationTentantServices.GetTenantEditCreateViewModelAsync(null, inputModel);

            return View(vm);
        }

        public async Task<IActionResult> Edit(int id)
        {
            TenantEditCreateViewModel vm;
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value ?? "";

            _logger.LogInformation("L'utente {CurrentUserId} ha richiesto la modifica del tenant {Id}", currentUserID, id);

            _commonServices.CurrentMenu = "Amministrazione";
            ViewData["Title"] = "Modifica Tenant";

            vm = await _administrationTentantServices.GetTenantEditCreateViewModelAsync(id, null);    

            return View(vm);  
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TenantEditInputModel inputModel)
        {
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value ?? "";

            _logger.LogInformation("L'utente {CurrentUserId} ha richiesto il salvataggio delle modifiche del tenant {Id}", currentUserID, inputModel.Id);

            _commonServices.CurrentMenu = "Amministrazione";

            ViewData["Title"] = "Modifica Tenant";

            if (ModelState.IsValid)
            {

                int Id = await _administrationTentantServices.UpdateTenantAsync(inputModel);
                TempData["EsitoPositivo"] = $"Aggiornato il seguente Tenant";
                return RedirectToAction(nameof(Details), new { Id = Id });
            }

            var vm = await _administrationTentantServices.GetTenantEditCreateViewModelAsync(inputModel.Id, inputModel);
            return View(vm);
        }

        public async Task<IActionResult> Delete(int id)
        {
            TenantDetailsViewModel vm;
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value ?? "";

            _logger.LogInformation("L'utente {CurrentUserId} ha richiesto l'eliminazione del tenant {Id}", currentUserID, id);

            _commonServices.CurrentMenu = "Amministrazione";
            ViewData["Title"] = "Elimina Tenant";

            vm = await _administrationTentantServices.GetTenantDetailsViewModelAsync(id);

            return View(vm);

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value ?? "";

            _logger.LogInformation("L'utente {CurrentUserId} ha confermato la richiesta di eliminazione del tenant {Id}", currentUserID, id);

            try
            {
                await _administrationTentantServices.DeleteTenantAsync(id);
                TempData["EsitoPositivo"] = $"Cancellazione dell'elemento id:{id} effettuata";

            }
            catch (TenantUndeletableException ex)
            {
                _logger.LogWarning(ex,"L'utente {CurrentUserId} ha cercato di eliminare il tenant {Id} ma non è possibile perchè collegato a dati", currentUserID, id);
                TempData["EsitoNegativo"] = ex.Message;
            }           

            return RedirectToAction(nameof(Index));

        }

    }
}
