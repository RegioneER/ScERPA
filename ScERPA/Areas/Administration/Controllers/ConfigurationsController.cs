using ScERPA.Areas.Administration.Models;
using ScERPA.Areas.Administration.ViewModels;
using ScERPA.Models;
using ScERPA.Models.Exceptions;
using ScERPA.Services;
using ScERPA.Services.Interfaces;
using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace ScERPA.Areas.Administration.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Area("Administration")]
    public class ConfigurationsController : Controller
    {
        private readonly ILogger<ConfigurationsController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICommonServices _commonServices;
        private readonly IAdministrationConfigurationsServices _administrationConfigurationsServices;

        public ConfigurationsController(ILogger<ConfigurationsController> logger, IHttpContextAccessor httpContextAccessor, ICommonServices commonServices, IAdministrationConfigurationsServices administrationConfigurationsServices)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _commonServices = commonServices;
            _administrationConfigurationsServices = administrationConfigurationsServices;

        }


        public async Task<IActionResult> Index()
        {
         
            ConfigurationsListViewModel vm;

            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value ?? "";

            _logger.LogInformation("L'utente {CurrentUserId} ha richiesto l'elenco delle configurazioni", currentUserID);

            _commonServices.CurrentMenu = "Amministrazione";

            ViewData["Title"] = "Configurazioni";

            vm = await _administrationConfigurationsServices.GetConfigurationsListViewModelAsync();

            return View(vm);

        }

        public async Task<IActionResult> Details(int id)
        {
            ConfigurationDetailsViewModel vm;

            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value ?? "";

            _logger.LogInformation("L'utente {CurrentUserId} ha richiesto il dettaglio della configurazione {Id}", currentUserID, id);

            _commonServices.CurrentMenu = "Amministrazione";

            ViewData["Title"] = "Configurazione";

            vm =  await _administrationConfigurationsServices.GetConfigurationDetailsViewModelAsync(id);

            return View(vm);

        }

        public async Task<IActionResult> Create()
        {
            ConfigurationEditCreateViewModel vm;

            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value ?? "";

            _logger.LogInformation("L'utente {CurrentUserId} ha richiesto iniziato la creazione di una nuova configurazione", currentUserID);

            _commonServices.CurrentMenu = "Amministrazione";

            ViewData["Title"] = "Nuova configurazione";

            vm = await _administrationConfigurationsServices.GetConfigurationEditCreateViewModelAsync(null, null);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConfigurationEditInputModel inputModel)
        {
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value ?? "";

            _logger.LogInformation("L'utente {CurrentUserId} sta provando a salvare la nuova configurazione", currentUserID);

            _commonServices.CurrentMenu = "Amministrazione";

            if (ModelState.IsValid)
            {
                int id = await _administrationConfigurationsServices.CreateConfigurationAsync(inputModel);
                TempData["EsitoPositivo"] = $"Creata la seguente configurazione";

                return RedirectToAction(nameof(Details), new {id = id});
            }

            var vm = await _administrationConfigurationsServices.GetConfigurationEditCreateViewModelAsync(null, inputModel);
            
            return View(vm);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            ConfigurationEditCreateViewModel vm;

            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value ?? "";

            _logger.LogInformation("L'utente {CurrentUserId} ha richiesto la modifica della configurazione {Id}", currentUserID, id);

            _commonServices.CurrentMenu = "Amministrazione";

            ViewData["Title"] = "Modifica configurazione";

            vm = await _administrationConfigurationsServices.GetConfigurationEditCreateViewModelAsync(id, null);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ConfigurationEditInputModel inputModel)
        {
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value ?? "";

            _logger.LogInformation("L'utente {CurrentUserId} ha richiesto il salvataggio delle modifiche della configurazione {Id}", currentUserID, id);

            _commonServices.CurrentMenu = "Amministrazione";

            if (ModelState.IsValid)
            {
                int idModificato = await _administrationConfigurationsServices.UpdateConfigurationAsync(inputModel);
                TempData["EsitoPositivo"] = $"Aggiornata la seguente configurazione";

                return RedirectToAction(nameof(Details), new { id = idModificato});

            }

            var vm = await _administrationConfigurationsServices.GetConfigurationEditCreateViewModelAsync(inputModel.Id, inputModel);

            return View(vm);
        }

        public async Task<IActionResult> Delete(int id)
        {
            ConfigurationDetailsViewModel vm;
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value ?? "";

            _logger.LogInformation("L'utente {CurrentUserId} ha richiesto l'eliminazione della configurazione {Id}", currentUserID, id);

            _commonServices.CurrentMenu = "Amministrazione";

            ViewData["Title"] = "Elimina configurazione";

            vm = await _administrationConfigurationsServices.GetConfigurationDetailsViewModelAsync(id);

            return View(vm);
            
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value ?? "";

            _logger.LogInformation("L'utente {CurrentUserId} ha confermato la richiesta di eliminazione della configurazione {Id}", currentUserID, id);

            _commonServices.CurrentMenu = "Amministrazione";

            try
            {
                await _administrationConfigurationsServices.DeleteConfigurationAsync(id);
                TempData["EsitoPositivo"] = $"Cancellazione della configurazione id:{id} effettuata";

            }
            catch (ConfigurationUndeletableException ex)
            {
                _logger.LogWarning(ex, "L'utente {CurrentUserId} ha cercato di eliminare la configurazione {Id} ma non è possibile perchè collegato a dati", currentUserID, id);
                TempData["EsitoNegativo"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
    
        }

    }
}
