using ScERPA.Areas.Administration.Models;
using ScERPA.Areas.Administration.ViewModels;
using ScERPA.Models;
using ScERPA.Services;
using ScERPA.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Storage;
using Org.BouncyCastle.Bcpg;
using System.Text.Json;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace ScERPA.Areas.Administration.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin,Manager")]
    [Area("Administration")]
    public class UsersAdminController : Controller
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICommonServices _commonServices;
        private readonly IAdministrationUsersServices _administrationUsersServices;


        public UsersAdminController( IHttpContextAccessor httpContextAccessor,  ICommonServices commonServices, IAdministrationConfigurationsServices administrationConfigurationsServices, IAdministrationUsersServices administrationUsersServices)
        {

            _httpContextAccessor = httpContextAccessor;
            _commonServices = commonServices;
            _administrationUsersServices = administrationUsersServices;
        }


        public async Task<IActionResult> Index()
        {
            UsersListViewModel vm = new();

            _commonServices.CurrentMenu = "Utenti";
            ViewData["Title"] = "Autorizza utenti su servizi/finalità";

            vm.Elenco = await _administrationUsersServices.GetUsersListForFinalitiesByAdminAsync(null);

            return View(vm);
        }


        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FilteredIndex(UsersListSearchPanel searchPanel)
        {
            UsersListViewModel vm = new();

            _commonServices.CurrentMenu = "Utenti";
            ViewData["Title"] = "Autorizza utenti su servizi/finalità";
            if(ModelState.IsValid)
            {
                vm.Elenco = await _administrationUsersServices.GetUsersListForFinalitiesByAdminAsync(searchPanel);
            }
            return View(nameof(Index), vm);
        }

        public async Task<IActionResult> Details(string UserId)
        {
            UserAdminViewModel vm;
            bool isAdmin = (_httpContextAccessor?.HttpContext?.User?.IsInRole("SuperAdmin") ?? false) || (_httpContextAccessor?.HttpContext?.User?.IsInRole("Admin") ?? false);
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value + "";

            _commonServices.CurrentMenu = "Utenti";
            ViewData["Title"] = "Utente e autorizzazioni servizi/finalità";
            vm = await _administrationUsersServices.GetConsumerAsync(UserId);
            vm.Abilitazioni = await _administrationUsersServices.GetUserAreasServicesFinalitiesListByAdminAsync(currentUserID, isAdmin, UserId, 0);
            return View(vm);

        }



        public async Task<IActionResult> Edit(string UserId)
        {
            UserAdminViewModel vm;
            bool isAdmin = (_httpContextAccessor?.HttpContext?.User?.IsInRole("SuperAdmin") ?? false) || (_httpContextAccessor?.HttpContext?.User?.IsInRole("Admin") ?? false);
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value + "";

            _commonServices.CurrentMenu = "Utenti";
            ViewData["Title"] = "Modifica utente e autorizzazioni servizi/finalità";
            vm = await _administrationUsersServices.GetConsumerAsync(UserId);
            vm.Abilitazioni = await _administrationUsersServices.GetUserAreasServicesFinalitiesListByAdminAsync(currentUserID, isAdmin, UserId, 0);
            return View(vm);
        }
        public async Task<IActionResult> Delete(string UserId, int Purpouse)
        {
            UserAdminViewModel vm;
            bool isAdmin = (_httpContextAccessor?.HttpContext?.User?.IsInRole("SuperAdmin") ?? false) || (_httpContextAccessor?.HttpContext?.User?.IsInRole("Admin") ?? false);
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value + "";

            _commonServices.CurrentMenu = "Utenti";
            ViewData["Title"] = "Elimina utente e autorizzazioni servizi/finalità";
            vm = await _administrationUsersServices.GetConsumerAsync(UserId);
            vm.Abilitazioni = await _administrationUsersServices.GetUserAreasServicesFinalitiesListByAdminAsync(currentUserID, isAdmin, UserId, Purpouse);

            return View(vm);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string UserId, int Purpouse)
        {
            bool isAdmin = (_httpContextAccessor?.HttpContext?.User?.IsInRole("SuperAdmin") ?? false) || (_httpContextAccessor?.HttpContext?.User?.IsInRole("Admin") ?? false);
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value + "";

            await _administrationUsersServices.RemoveAssociatonOfFinalityToUserByAdminAsync(currentUserID, isAdmin, UserId, Purpouse);

            TempData["EsitoPositivo"] = $"Cancellazione della associazione utente-finalità effettuata correttamente";
            return RedirectToAction(nameof(Edit), new { UserId = UserId });

        }

        [HttpGet]
        public async Task<IActionResult> Create(string UserId)
        {
            UserAminAssociatePurpouseViewModel vm = new();
            bool isAdmin = (_httpContextAccessor?.HttpContext?.User?.IsInRole("SuperAdmin")??false) || (_httpContextAccessor?.HttpContext?.User?.IsInRole("Admin") ?? false);
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value + "";

            _commonServices.CurrentMenu = "Utenti";
            ViewData["Title"] = "Aggiungi autorizzazione a servizio/finalità";
            var utente = await _administrationUsersServices.GetConsumerAsync(UserId);
            if(utente is not null)
            {
                vm.Input.UserId = utente.UserId;
                vm.Cognome = utente.Cognome;
                vm.Nome = utente.Nome;
                vm.Email = utente.Email;
                vm.Aree = await _administrationUsersServices.GetUserAreasListByAdminAsync(currentUserID, isAdmin, UserId);
            }

            return View(vm);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAssociation(UserAminAssociatePurpouseViewModel inputModel)
        {
            string UserId = inputModel.Input.UserId;
            bool isAdmin = (_httpContextAccessor?.HttpContext?.User?.IsInRole("SuperAdmin")??false) || (_httpContextAccessor?.HttpContext?.User?.IsInRole("Admin")??false);
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value + "";

            _commonServices.CurrentMenu = "Utenti";

            if (ModelState.IsValid)
            {

                 await _administrationUsersServices.AssociateFinalityToUserByAdminAsync(currentUserID, isAdmin, inputModel.Input.UserId, int.Parse(inputModel.Input.FinalitaId));
                TempData["EsitoPositivo"] = $"Aggiunta associazione con la finalità selezionata";
                return RedirectToAction(nameof(Edit), new { UserId = UserId });
            }

            return RedirectToAction(nameof(Create), new { UserId = UserId });
        }


        [HttpGet]
        public async Task<IActionResult> APIGetServicesList([FromQuery] string userId, int areaId)
        {
            string json = "";
            bool isAdmin = (_httpContextAccessor?.HttpContext?.User?.IsInRole("SuperAdmin")??false) || (_httpContextAccessor?.HttpContext?.User?.IsInRole("Admin") ?? false);
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value + "";
            var listaElementi = await _administrationUsersServices.GetUserServicesListByAdminAsync(currentUserID,  isAdmin, userId, areaId);
            json = JsonSerializer.Serialize(listaElementi);
            return Ok(json);
        }

        [HttpGet]
        public async Task<IActionResult> APIGetFinalitiesList([FromQuery] string userId, int areaId, int serviceId)
        {
            string json = "";
            bool isAdmin = (_httpContextAccessor?.HttpContext?.User?.IsInRole("SuperAdmin")??false) || (_httpContextAccessor?.HttpContext?.User?.IsInRole("Admin")?? false);
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value + "";
            var listaElementi = await _administrationUsersServices.GetUserFinalitiesListByAdminAsync(currentUserID, isAdmin, userId, areaId, serviceId);
            json = JsonSerializer.Serialize(listaElementi);
            return Ok(json);
        }

        [HttpGet]
        public async Task<IActionResult> IndexProfiles()
        {
            UsersListViewModel vm = new();

            _commonServices.CurrentMenu = "Utenti";
            ViewData["Title"] = "Utenti da profilare";
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value + "";

            vm.Elenco = await _administrationUsersServices.GetUsersListForProfileByAdminAsync(currentUserID, null);

            return View(vm);

        }

        [HttpPost, ActionName("IndexProfiles")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FilteredIndexProfiles(UsersListSearchPanel searchPanel)
        {
            UsersListViewModel vm = new();

            _commonServices.CurrentMenu = "Utenti";
            ViewData["Title"] = "Utenti da profilare";
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value + "";

            if (ModelState.IsValid)
            {
                vm.Elenco = await _administrationUsersServices.GetUsersListForProfileByAdminAsync(currentUserID,searchPanel);
            }

            return View(nameof(IndexProfiles), vm);
        }


        public async Task<IActionResult> ProfileDetails(string UserId)
        {
            UserAdminUserProfileViewModel vm;
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value + "";

            _commonServices.CurrentMenu = "Utenti";
            ViewData["Title"] = "Profilo utente";

            vm = await _administrationUsersServices.GetUserProfileDetailByAdminAsync(currentUserID, UserId);
            
            return View(vm);

        }

        public async Task<IActionResult> EditProfile(string UserId)
        {
            UserAminEditProfileViewModel vm;
            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value + "";

            _commonServices.CurrentMenu = "Utenti";
            ViewData["Title"] = "Modifica profilo utente";
            vm = await _administrationUsersServices.GetUserProfileForEditByAdminAsync(currentUserID, UserId);
            
            return View(vm);
        }

        [HttpPost, ActionName("EditProfile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfileSave(UserProfileEditInputModel input)
        {

            string currentUserID = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value + "";

            _commonServices.CurrentMenu = "Utenti";
            if (ModelState.IsValid)
            {

                await _administrationUsersServices.UpdateUserProfileByAdminAsync(currentUserID, input);
                TempData["EsitoPositivo"] = $"Profilo utente modificato con successo";
                return RedirectToAction(nameof(ProfileDetails), new { UserId = input.UserId });
            }

            return RedirectToAction(nameof(EditProfile), new { UserId = input.UserId });
        }


    }
}
