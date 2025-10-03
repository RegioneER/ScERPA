using ScERPA.Services;
using ScERPA.Services.Interfaces;
using ScERPA.ViewModels.RchiesteMassive;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace ScERPA.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class RichiesteMassiveController : Controller
    {
        private readonly ILogger<RichiesteMassiveController> _logger;
        private readonly IUtilities _utilities;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICommonServices _commonServices;
        private readonly IMassiveRequestsServices _massiveRequestsService;
        public RichiesteMassiveController(ILogger<RichiesteMassiveController> logger, IHttpContextAccessor httpContextAccessor, IUtilities utilities, ICommonServices commonServices, IMassiveRequestsServices massiveRequestsService)
        {
            _logger = logger;
            _utilities = utilities;
            _httpContextAccessor = httpContextAccessor;
            _commonServices = commonServices; 
            _massiveRequestsService = massiveRequestsService;
        }
        public async Task<IActionResult> Index()
        {
            MassiveRequestsListViewModel vm = new();
            _commonServices.CurrentMenu = "E-Services";

            ViewData["Title"] = "Elenco delle richieste massive";


            vm = await _massiveRequestsService.GetMassiveRequestsListViewModelAsync(null, 1);

            return View(vm);
        }

        public async Task<IActionResult> AppendRequest(int idRichiestaMassiva, int page)
        {
            MassiveRequestsListViewModel vm = new();
            _commonServices.CurrentMenu = "E-Services";
            ViewData["Title"] = "Elenco delle richieste massive";

            //devo verificare esista la richiesta con id inserito
            if (idRichiestaMassiva != 0 )
            {

                await _massiveRequestsService.AppendRequestAsync(idRichiestaMassiva);

            }
            else throw new InvalidDataException("Non è possibile eseguire la richiesta massiva specificata");

            vm = await _massiveRequestsService.GetMassiveRequestsListViewModelAsync(null, page);

            TempData["EsitoPositivo"] = $"Accodata la richiesta per l'esecuzione";

            return View(nameof(Index),vm);
        }

    }
}
