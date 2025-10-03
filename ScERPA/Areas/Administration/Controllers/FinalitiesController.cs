using ScERPA.Areas.Administration.Models;
using ScERPA.Areas.Administration.ViewModels;
using ScERPA.Models.Exceptions;
using ScERPA.Services;
using ScERPA.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ScERPA.Areas.Administration.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    [Area("Administration")]
    public class FinalitiesController : Controller
    {
        private readonly ILogger<FinalitiesController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAdministrationFinalitiesServices _administrationFinalitiesServices;
        private readonly ICommonServices _commonServices;
        private readonly IUtilities _utilities;

        public FinalitiesController(ILogger<FinalitiesController> logger, IHttpContextAccessor httpContextAccessor,   IAdministrationFinalitiesServices administrationFinalitiesServices,ICommonServices commonServices, IUtilities utilities)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _administrationFinalitiesServices = administrationFinalitiesServices;
            _commonServices = commonServices;
            _utilities = utilities;
        }

        //Lista delle finalità
        public async Task<IActionResult> Index(string? areaId, string? servizioId,string? denominazioneFinalita,int? page)
        {
            FinalitiesListViewModel vm;
            FinalitiesSearchPanel searchPanel = new FinalitiesSearchPanel { AreaId = _utilities.SanitizeAsPlainText(areaId ?? ""), ServizioId = _utilities.SanitizeAsPlainText(servizioId ?? ""), DenominazioneFinalita = _utilities.SanitizeAsPlainText( denominazioneFinalita ?? "") }; 
            int currentPage;
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value ?? "";

            _logger.LogInformation("L'utente {CurrentUserId} ha richiesto l'elenco delle finalità", currentUserID);

            _commonServices.CurrentMenu = "E-Services";
           
            ViewData["Title"] = "Finalità";

            currentPage = page ?? 1;
            vm = await _administrationFinalitiesServices.GetFinalitiesListViewModelAsync(searchPanel, currentPage);

            return View(vm);
        }

        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FilteredIndexFinalities(FinalitiesSearchPanel searchPanel)
        {
            FinalitiesListViewModel vm = new();

            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value ?? "";

            _logger.LogInformation("L'utente {CurrentUserId} ha richiesto l'elenco delle finalità", currentUserID);

            _commonServices.CurrentMenu = "E-Services";

            ViewData["Title"] = "Finalità";

            if (ModelState.IsValid)
            {
                vm = await _administrationFinalitiesServices.GetFinalitiesListViewModelAsync(searchPanel,1);
                
            } 

            return View(nameof(Index), vm);

        }

        //Dettaglio della finalità
        public async Task<IActionResult> Details(int Id)
        {
            FinalityDetailsViewModel vm;

            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value ?? "";

            _logger.LogInformation("L'utente {CurrentUserId} ha richiesto il dettaglio della finalità {Id}", currentUserID, Id);

            _commonServices.CurrentMenu = "E-Services";
            ViewData["Title"] = "Finalità";

            vm = await _administrationFinalitiesServices.GetFinalityDetailsViewModelAsync(Id);

            return View(vm);

        }

        public async Task<IActionResult> Create()
        {
            FinalityEditCreateViewModel vm;
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value ?? "";

            _logger.LogInformation("L'utente {CurrentUserId} ha richiesto iniziato la creazione di una nuova finalità", currentUserID);

            _commonServices.CurrentMenu = "E-Services";
            ViewData["Title"] = "Nuova finalità";

            vm = await _administrationFinalitiesServices.GetFinalityEditCreateViewModelAsync(null, null);

            return View(vm);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FinalityEditInputModel inputModel)
        {
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value ?? "";

            _logger.LogInformation("L'utente {CurrentUserId} sta provando a salvare la nuova finalità", currentUserID);

            _commonServices.CurrentMenu = "E-Services";
            ViewData["Title"] = "Nuova finalità";

            if (ModelState.IsValid)
            {
                int id = await _administrationFinalitiesServices.CreateFinalityAsync(inputModel);
                TempData["EsitoPositivo"] = $"Creata la seguente finalità";
                return RedirectToAction(nameof(Details), new { Id = id });
            }

            var vm = await _administrationFinalitiesServices.GetFinalityEditCreateViewModelAsync(null, inputModel);

            return View(vm);
        }

        public async Task<IActionResult> Edit(int id)
        {
            FinalityEditCreateViewModel vm;
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value ?? "";

            _logger.LogInformation("L'utente {CurrentUserId} ha richiesto la modifica della finalità {Id}", currentUserID, id);

            _commonServices.CurrentMenu = "E-Services";
            ViewData["Title"] = "Modifica Finalità";

            vm = await _administrationFinalitiesServices.GetFinalityEditCreateViewModelAsync(id, null);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FinalityEditInputModel inputModel)
        {
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value ?? "";

            _logger.LogInformation("L'utente {CurrentUserId} ha richiesto il salvataggio delle modifiche della finalità {Id}", currentUserID, inputModel.Id);

            _commonServices.CurrentMenu = "E-Services";
            ViewData["Title"] = "Modifica Finalità";

            if (ModelState.IsValid)
            {

                int Id = await _administrationFinalitiesServices.UpdateFinalityAsync(inputModel);
                TempData["EsitoPositivo"] = $"Aggiornato la seguente finalità";
                return RedirectToAction(nameof(Details), new { Id = Id });
            }

            var vm = await _administrationFinalitiesServices.GetFinalityEditCreateViewModelAsync(inputModel.Id, inputModel);
            return View(vm);
        }


        public async Task<IActionResult> Delete(int id)
        {
            FinalityDetailsViewModel vm;
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value ?? "";

            _logger.LogInformation("L'utente {CurrentUserId} ha richiesto l'eliminazione della finalità {Id}", currentUserID, id);

            _commonServices.CurrentMenu = "E-Services";
            ViewData["Title"] = "Elimina Finalità";

            vm = await _administrationFinalitiesServices.GetFinalityDetailsViewModelAsync(id);

            return View(vm);

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value ?? "";

            _logger.LogInformation("L'utente {CurrentUserId} ha confermato la richiesta di eliminazione della finalità {Id}", currentUserID, id);

            try
            {
                await _administrationFinalitiesServices.DeleteFinalityAsync(id);
                TempData["EsitoPositivo"] = $"Cancellazione della finalità id:{id} effettuata";

            }
            catch (FinalityUndeletableException ex)
            {
                _logger.LogWarning(ex, "L'utente {CurrentUserId} ha cercato di eliminare la finalità {Id} ma non è possibile perchè collegato a dati", currentUserID, id);
                TempData["EsitoNegativo"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public async Task<IActionResult> APIGetServicesList([FromQuery] int areaId)
        {
            string json = "";
 
            var listaElementi = await _administrationFinalitiesServices.APIGetServicesListAsync(areaId,null);
            json = JsonSerializer.Serialize(listaElementi);
            return Ok(json);
        }


    }
}
