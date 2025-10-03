using ScERPA.Models.EditInputModels;
using ScERPA.Services;
using ScERPA.Services.Interfaces;
using ScERPA.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using ScERPA.Models.Enums;
using ScERPA.Models;
using iText.Layout.Element;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using ScERPA.Models.Exceptions;
using System.Globalization;
using ScERPA.Models.Enums.ANPR;


namespace ScERPA.Controllers
{
    [Authorize(Roles ="Consumer")]
    public class AnprController : Controller
    {
        private readonly ILogger<AnprController> _logger;
        private readonly IUtilities _utilities;
        private readonly IAnprServices _anprServices;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICommonServices _commonServices;
        private readonly IConsumerServices _consumerServices;     

        public AnprController(ILogger<AnprController> logger, IHttpContextAccessor httpContextAccessor, IUtilities utilities, IAnprServices anprServices,ICommonServices commonServices, IConsumerServices consumerServices)
        {
            _logger = logger;
            _anprServices = anprServices;
            _utilities = utilities;
            _httpContextAccessor = httpContextAccessor;
            _commonServices= commonServices;
            _consumerServices = consumerServices;
        }


        [HttpGet]
        public async Task<IActionResult> AccertamentoResidenzaMassivo()
        {
            string servizio = _httpContextAccessor?.HttpContext?.GetRouteValue("action")?.ToString() + "";
            string user = User.FindFirstValue(ClaimTypes.NameIdentifier) + "";
            AccertamentoMassivoViewModel vm = new();

            _commonServices.CurrentMenu = "Anpr";
            ViewData["Title"] = "Anpr - Accertamento residenza massivo";

            vm = await GetAccertamentoMassivoViewModel(user, servizio, null);
          
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AccertamentoResidenzaMassivo(AccertamentoMassivoEditInputModel inputModel)
        {
            AccertamentoMassivoViewModel vm = new();
            DateOnly DataRiferimentoVerifica;
            string userLocation = _utilities.GetRemoteIPV4(Request?.HttpContext?.Connection?.RemoteIpAddress?.ToString());
            string userID = _httpContextAccessor?.HttpContext?.User?.Identity?.Name?.ToString() + "";
            string LoA = "LOA1";
            string purpouseID = "";
            string operationGuid = Guid.NewGuid().ToString();
            const string codiceStato = "100";
            const string descrizioneStato = "ITALIA";
            string user = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value + "";
            string servizio = _httpContextAccessor?.HttpContext?.GetRouteValue("action")?.ToString() + "";
            Purpouse purpouse;
            int idFInalita;

            _commonServices.CurrentMenu = "Anpr";
            ViewData["Title"] = "Anpr - Accertamento residenza massivo";

            vm = await GetAccertamentoMassivoViewModel(user, servizio, inputModel);

            try
            {
                if (ModelState.IsValid)
                {
                    if(inputModel.FileRichiesta != null && inputModel.FileRichiesta.Length>0 && inputModel.FileRichiesta.ContentType== "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        MemoryStream stream  = new();
                        await inputModel.FileRichiesta.CopyToAsync(stream);


                        if (int.TryParse(inputModel.FinalitaId, out idFInalita))
                        {

                            int idRichiesta = await _anprServices.CreaRichistaMassiva(user, idFInalita, stream);
                        }
                        else throw new FinalityNotFoundException("Finalità non valida");
                        
                    } else throw new FileLoadException("Il file caricato non è valido");
                }
            }
            catch (FileLoadException ex)
            {
                TempData["EsitoAttenzione"] = ex.Message;
                return View(vm);
            }
            catch (FinalityNotFoundException ex)
            {
                TempData["EsitoAttenzione"] = ex.Message;
                return View(vm);
            }

            return RedirectToAction("Index", "RichiesteMassive");
        }


        [HttpGet]
        public async Task<IActionResult> AccertamentoIdentificativoUnicoNazionale()
        {
            string servizio = _httpContextAccessor?.HttpContext?.GetRouteValue("action")?.ToString() + "";
            string user = User.FindFirstValue(ClaimTypes.NameIdentifier) + "";
            AccertamentoIdentificativoUnicoNazionaleViewModel vm = new();

            _commonServices.CurrentMenu = "Anpr";
            ViewData["Title"] = "Anpr - Accertamento identificativo unico nazionale";

            vm.AccertamentoMultiplo = await GetAccertamentoMultiploViewModel(user, servizio, null);
            vm.risultati = null;
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AccertamentoIdentificativoUnicoNazionale(AccertamentoMultiploEditInputModel inputModel)
        {
            AccertamentoIdentificativoUnicoNazionaleViewModel vm = new();
            DateOnly DataRiferimentoVerifica;
            string userLocation = _utilities.GetRemoteIPV4(Request?.HttpContext?.Connection?.RemoteIpAddress?.ToString());
            string userID = _httpContextAccessor?.HttpContext?.User?.Identity?.Name?.ToString() + "";
            string LoA = "LOA1";
            string purpouseID = "";
            string operationGuid = Guid.NewGuid().ToString();
            const string codiceStato = "100";
            const string descrizioneStato = "ITALIA";
            string user = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value + "";
            string servizio = _httpContextAccessor?.HttpContext?.GetRouteValue("action")?.ToString() + "";
            Purpouse purpouse;

            _commonServices.CurrentMenu = "Anpr";
            ViewData["Title"] = "Anpr - Accertamento identificativo unico nazionale";

            vm.AccertamentoMultiplo = await GetAccertamentoMultiploViewModel(user, servizio, inputModel);

            try
            {

                if (ModelState.IsValid)
                {

                    DateOnly.TryParse(inputModel.DataRiferimentoVerifica, CultureInfo.InvariantCulture, out DataRiferimentoVerifica);

                    purpouse = await _consumerServices.GetUserFinalityCurrentPurpouseIDAsync(user, int.Parse(inputModel.FinalitaId));

                    purpouseID = purpouse.Valore;

                    vm.risultati = await _anprServices.ServizioAccertamentoIdentificativoUnicoNazionaleAsync(user, inputModel.CodiciFiscali, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);
                    var report = _anprServices.CreateReport(servizio, "ANPR-Accertamento generalita", vm.AccertamentoMultiplo.DescrizioneFinalita, vm.risultati.ToList<IElementoSchedaReport>(), DataRiferimentoVerifica, userID, userLocation, LoA, operationGuid);
                    if (report is not null)
                    {
                        vm.AccertamentoMultiplo.Pdf = await report.ExportAsPdfAsync(_commonServices.PercorsoWWW);
                        vm.AccertamentoMultiplo.Xlsx = await report.ExportAsExcelAsync(_commonServices.PercorsoWWW);
                    }
                }


            }
            catch (ThresholdException ex)
            {
                TempData["EsitoAttenzione"] = ex.Message;
            }

            return View(vm);
        }


        [HttpGet]
        public async Task<IActionResult> AccertamentoStatoDiFamiglia()
        {
            string servizio = _httpContextAccessor?.HttpContext?.GetRouteValue("action")?.ToString() + "";
            string user = User.FindFirstValue(ClaimTypes.NameIdentifier) + "";
            AccertamentoStatoDiFamigliaViewModel vm = new();

            _commonServices.CurrentMenu = "Anpr";

            ViewData["Title"] = "Anpr - Accertamento stato di famiglia";

            vm.AccertamentoMultiplo = await GetAccertamentoMultiploViewModel(user, servizio, null);
            vm.risultati = null;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AccertamentoStatoDiFamiglia(AccertamentoMultiploEditInputModel inputModel)
        {
            AccertamentoStatoDiFamigliaViewModel vm = new();

            DateOnly DataRiferimentoVerifica;
            string userLocation = _utilities.GetRemoteIPV4(Request?.HttpContext?.Connection?.RemoteIpAddress?.ToString());
            string userID = _httpContextAccessor?.HttpContext?.User?.Identity?.Name?.ToString() + "";
            string LoA = "LOA1";
            string purpouseID = "";
            string operationGuid = Guid.NewGuid().ToString();
            const string codiceStato = "100";
            const string descrizioneStato = "ITALIA";
            string user = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value + "";
            string servizio = _httpContextAccessor?.HttpContext?.GetRouteValue("action")?.ToString() + "";
            string descrizionefinalita;
            Purpouse purpouse;
            List<Finalita> finalita;

            _commonServices.CurrentMenu = "Anpr";
            ViewData["Title"] = "Anpr - Accertamento stato di famiglia";

            vm.AccertamentoMultiplo = await GetAccertamentoMultiploViewModel(user, servizio, inputModel);

            try
            {
                finalita = await _consumerServices.GetUserServiceFinalitiesListAsync(user, servizio);

                vm.AccertamentoMultiplo.ListaFinalita = finalita.AsQueryable().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Descrizione, Selected = x.Id.ToString() == inputModel.FinalitaId }).ToList();

                if (ModelState.IsValid)
                {
                    descrizionefinalita = (from f in finalita
                                           where f.Id.ToString() == inputModel.FinalitaId
                                           select f.Nome).Single();

                    DateOnly.TryParse(inputModel.DataRiferimentoVerifica,CultureInfo.InvariantCulture, out DataRiferimentoVerifica);


                    purpouse = await _consumerServices.GetUserFinalityCurrentPurpouseIDAsync(user, int.Parse(inputModel.FinalitaId));

                    purpouseID = purpouse.Valore;

                    vm.risultati = await _anprServices.ServizioAccertamentoStatoDiFamigliaAsync(user, inputModel.CodiciFiscali, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);

                    var report =  _anprServices.CreateReport(servizio, "ANPR-Accertamento stato di famiglia", descrizionefinalita, vm.risultati.ToList<IElementoSchedaReport>(), DataRiferimentoVerifica, userID, userLocation, LoA, operationGuid);
                    if (report is not null)
                    {
                        vm.AccertamentoMultiplo.Pdf = await report.ExportAsPdfAsync(_commonServices.PercorsoWWW);
                        vm.AccertamentoMultiplo.Xlsx = await report.ExportAsExcelAsync(_commonServices.PercorsoWWW);
                    }
                }

            } catch(ThresholdException ex) 
            {
                TempData["EsitoAttenzione"] = ex.Message;
            }


            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> AccertamentoResidenza()
        {
            string servizio = _httpContextAccessor?.HttpContext?.GetRouteValue("action")?.ToString() + "";
            string user = User.FindFirstValue(ClaimTypes.NameIdentifier) + "";
            AccertamentoResidenzaViewModel vm=new();

            _commonServices.CurrentMenu = "Anpr";
            ViewData["Title"] = "Anpr - Accertamento residenza";
            
            vm.AccertamentoMultiplo = await GetAccertamentoMultiploViewModel(user, servizio, null);
            vm.risultati = null;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]       
        public async Task<IActionResult> AccertamentoResidenza(AccertamentoMultiploEditInputModel inputModel)
        {
            AccertamentoResidenzaViewModel vm= new();
            DateOnly DataRiferimentoVerifica;
            string userLocation = _utilities.GetRemoteIPV4(Request?.HttpContext?.Connection?.RemoteIpAddress?.ToString());
            string userID = _httpContextAccessor?.HttpContext?.User?.Identity?.Name?.ToString() + "";
            string LoA = "LOA1";
            string purpouseID = "";
            string operationGuid = Guid.NewGuid().ToString();
            const string codiceStato = "100";
            const string descrizioneStato = "ITALIA";
            string user = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value + "";
            string servizio = _httpContextAccessor?.HttpContext?.GetRouteValue("action")?.ToString() + "";
            Purpouse purpouse;

            _commonServices.CurrentMenu = "Anpr";
            ViewData["Title"] = "Anpr - Accertamento residenza";

            vm.AccertamentoMultiplo = await GetAccertamentoMultiploViewModel(user, servizio, inputModel);
  
            try
            {  
                if (ModelState.IsValid)
                {

                    DateOnly.TryParse(inputModel.DataRiferimentoVerifica,CultureInfo.InvariantCulture, out DataRiferimentoVerifica);

                    purpouse = await _consumerServices.GetUserFinalityCurrentPurpouseIDAsync(user, int.Parse(inputModel.FinalitaId));

                    purpouseID = purpouse.Valore;

                    vm.risultati = await _anprServices.ServizioAccertamentoResidenzaAsync(user, inputModel.CodiciFiscali, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);

                    var report = _anprServices.CreateReport(servizio, "ANPR-Accertamento residenza", vm.AccertamentoMultiplo.DescrizioneFinalita, vm.risultati.ToList<IElementoSchedaReport>(), DataRiferimentoVerifica, userID, userLocation, LoA, operationGuid);

                    if (report is not null)
                    {
                        vm.AccertamentoMultiplo.Pdf = await report.ExportAsPdfAsync(_commonServices.PercorsoWWW);
                        vm.AccertamentoMultiplo.Xlsx = await report.ExportAsExcelAsync(_commonServices.PercorsoWWW);
                    }
                }


            } catch (ThresholdException ex)
            {
                TempData["EsitoAttenzione"] = ex.Message;
            }

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> VerificaCittadinanzaItaliana()
        {
            string servizio = _httpContextAccessor?.HttpContext?.GetRouteValue("action")?.ToString() + "";
            string user = User.FindFirstValue(ClaimTypes.NameIdentifier)+"";

            List<Finalita> finalita;

            VerificaCittadinanzaViewModel vm;

            _commonServices.CurrentMenu = "Anpr";
            ViewData["Title"] = "Anpr - Verifica cittadinanza italiana";
            vm = new();
            vm.NumeroMassimoCodiciFiscali = _anprServices.NumeroMassimoCodiciFiscali;
            finalita = await _consumerServices.GetUserServiceFinalitiesListAsync(user, servizio);
            vm.ListaFinalita =  finalita.AsQueryable().Select(x =>  new SelectListItem { Value=x.Id.ToString(), Text=x.Descrizione   }).ToList();
            vm.inputModel = new();
            vm.inputModel.CodiciFiscali = "";
            vm.risultati = null;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerificaCittadinanzaItaliana(VerificaCittadinanzaEditInputModel inputModel)
        {
            VerificaCittadinanzaViewModel vm;
            DateOnly DataRiferimentoVerifica;
            string userLocation = _utilities.GetRemoteIPV4(Request?.HttpContext?.Connection?.RemoteIpAddress?.ToString());
            string userID = _httpContextAccessor?.HttpContext?.User?.Identity?.Name?.ToString() + "";
            string LoA = "LOA1";
            string purpouseID="";
            string operationGuid = Guid.NewGuid().ToString();
            const string codiceStato = "100";
            const string descrizioneStato = "ITALIA";
            string user = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value + "";
            string servizio = _httpContextAccessor?.HttpContext?.GetRouteValue("action")?.ToString() + "";
            string descrizionefinalita;

            Purpouse purpouse;
            List<Finalita> finalita;

            _commonServices.CurrentMenu = "Anpr";
            ViewData["Title"] = "Anpr - Verifica cittadinanza";
            vm = new();
            vm.NumeroMassimoCodiciFiscali = _anprServices.NumeroMassimoCodiciFiscali;
            vm.inputModel = inputModel;

            try
            {
                finalita = await _consumerServices.GetUserServiceFinalitiesListAsync(user, servizio);

                vm.ListaFinalita = finalita.AsQueryable().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Descrizione, Selected = x.Id.ToString() == inputModel.FinalitaId }).ToList();

                if (ModelState.IsValid)
                {
                    descrizionefinalita = (from f in finalita
                                           where f.Id.ToString() == inputModel.FinalitaId
                                           select f.Nome).Single();

                    DateOnly.TryParse(inputModel.DataRiferimentoVerifica,CultureInfo.InvariantCulture, out DataRiferimentoVerifica);


                    purpouse = await _consumerServices.GetUserFinalityCurrentPurpouseIDAsync(user, int.Parse(inputModel.FinalitaId));

                    purpouseID = purpouse.Valore;

                    vm.risultati = await _anprServices.ServizioVerificaCittadinanzaAsync(user, inputModel.CodiciFiscali, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);

                    var report = _anprServices.CreateReport("VerificaCittadinanzaItaliana", "ANPR-Verifica cittadinanza italiana", descrizionefinalita, vm.risultati.ToList<IElementoSchedaReport>(), DataRiferimentoVerifica, userID, userLocation, LoA, operationGuid);

                    //imposto la sezione dati come tabella singola
                    if (report is not null)
                    {
                        if (report.Sezioni is not null && report.Sezioni.Count > 1) report.Sezioni.ToArray()[1].Tabella = new float[] { 25f, 15f, 20f, 20f, 20f };
                        vm.Pdf = await report.ExportAsPdfAsync(_commonServices.PercorsoWWW);
                        vm.Xlsx = await report.ExportAsExcelAsync(_commonServices.PercorsoWWW);
                    }
                }

            } catch (ThresholdException ex)
            {
                TempData["EsitoAttenzione"] = ex.Message;
            }

                
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> AccertamentoGeneralita()
        {
            string servizio = _httpContextAccessor?.HttpContext?.GetRouteValue("action")?.ToString() + "";
            string user = User.FindFirstValue(ClaimTypes.NameIdentifier) + "";
            AccertamentoGeneralitaViewModel vm = new();

            _commonServices.CurrentMenu = "Anpr";
            ViewData["Title"] = "Anpr - Accertamento generalità";

            vm.AccertamentoMultiplo = await GetAccertamentoMultiploViewModel(user, servizio, null);
            vm.risultati = null;
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AccertamentoGeneralita(AccertamentoMultiploEditInputModel inputModel)
        {
            AccertamentoGeneralitaViewModel vm = new();
            DateOnly DataRiferimentoVerifica;
            string userLocation = _utilities.GetRemoteIPV4(Request?.HttpContext?.Connection?.RemoteIpAddress?.ToString());
            string userID = _httpContextAccessor?.HttpContext?.User?.Identity?.Name?.ToString() + "";
            string LoA = "LOA1";
            string purpouseID = "";
            string operationGuid = Guid.NewGuid().ToString();
            const string codiceStato = "100";
            const string descrizioneStato = "ITALIA";
            string user = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value + "";
            string servizio = _httpContextAccessor?.HttpContext?.GetRouteValue("action")?.ToString() + "";
            Purpouse purpouse;

            _commonServices.CurrentMenu = "Anpr";
            ViewData["Title"] = "Anpr - Accertamento generalita";

            vm.AccertamentoMultiplo = await GetAccertamentoMultiploViewModel(user, servizio, inputModel);

            try
            {

                if (ModelState.IsValid)
                {

                    DateOnly.TryParse(inputModel.DataRiferimentoVerifica,CultureInfo.InvariantCulture, out DataRiferimentoVerifica);

                    purpouse = await _consumerServices.GetUserFinalityCurrentPurpouseIDAsync(user, int.Parse(inputModel.FinalitaId));

                    purpouseID = purpouse.Valore;

                    vm.risultati = await _anprServices.ServizioAccertamentoGeneralitaAsync(user, inputModel.CodiciFiscali, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);
                    var report =  _anprServices.CreateReport(servizio, "ANPR-Accertamento generalita", vm.AccertamentoMultiplo.DescrizioneFinalita, vm.risultati.ToList<IElementoSchedaReport>(), DataRiferimentoVerifica, userID, userLocation, LoA, operationGuid);
                    if (report is not null)
                    {
                        vm.AccertamentoMultiplo.Pdf = await report.ExportAsPdfAsync(_commonServices.PercorsoWWW);
                        vm.AccertamentoMultiplo.Xlsx = await report.ExportAsExcelAsync(_commonServices.PercorsoWWW);
                    }
                }


            }
            catch (ThresholdException ex)
            {
                TempData["EsitoAttenzione"] = ex.Message;
            }

            return View(vm);
        }



        [HttpGet]
        public async Task<IActionResult> AccertamentoCittadinanza()
        {
            string servizio = _httpContextAccessor?.HttpContext?.GetRouteValue("action")?.ToString() + "";
            string user = User.FindFirstValue(ClaimTypes.NameIdentifier) + "";
            AccertamentoCittadinanzaViewModel vm = new();

            _commonServices.CurrentMenu = "Anpr";
            ViewData["Title"] = "Anpr - Accertamento cittadinanza";

            vm.AccertamentoMultiplo = await GetAccertamentoMultiploViewModel(user, servizio, null);
            vm.risultati = null;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AccertamentoCittadinanza(AccertamentoMultiploEditInputModel inputModel)
        {
            AccertamentoCittadinanzaViewModel vm = new();
            DateOnly DataRiferimentoVerifica;
            string userLocation = _utilities.GetRemoteIPV4(Request?.HttpContext?.Connection?.RemoteIpAddress?.ToString());
            string userID = _httpContextAccessor?.HttpContext?.User?.Identity?.Name?.ToString() + "";
            string LoA = "LOA1";
            string purpouseID = "";
            string operationGuid = Guid.NewGuid().ToString();
            const string codiceStato = "100";
            const string descrizioneStato = "ITALIA";
            string user = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value + "";
            string servizio = _httpContextAccessor?.HttpContext?.GetRouteValue("action")?.ToString() + "";
            Purpouse purpouse;


            _commonServices.CurrentMenu = "Anpr";
            ViewData["Title"] = "Anpr - Accertamento cittadinanza";

            vm.AccertamentoMultiplo = await GetAccertamentoMultiploViewModel(user, servizio, inputModel);

            try
            {
                if (ModelState.IsValid)
                {


                    DateOnly.TryParse(inputModel.DataRiferimentoVerifica, CultureInfo.InvariantCulture, out DataRiferimentoVerifica);


                    purpouse = await _consumerServices.GetUserFinalityCurrentPurpouseIDAsync(user, int.Parse(inputModel.FinalitaId));

                    purpouseID = purpouse.Valore;

                    vm.risultati = await _anprServices.ServizioAccertamentoCittadinanzaAsync(user, inputModel.CodiciFiscali, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);
                    var report = _anprServices.CreateReport(servizio, "ANPR-Accertamento cittadinanza", vm.AccertamentoMultiplo.DescrizioneFinalita, vm.risultati.ToList<IElementoSchedaReport>(), DataRiferimentoVerifica, userID, userLocation, LoA, operationGuid);
                    if (report is not null)
                    {
                        vm.AccertamentoMultiplo.Pdf = await report.ExportAsPdfAsync(_commonServices.PercorsoWWW);
                        vm.AccertamentoMultiplo.Xlsx = await report.ExportAsExcelAsync(_commonServices.PercorsoWWW);
                    }
                }


            }
            catch (ThresholdException ex)
            {
                TempData["EsitoAttenzione"] = ex.Message;
            }

            return View(vm);
        }


        [HttpGet]
        public async Task<IActionResult> AccertamentoDichDecesso()
        {
            string servizio = _httpContextAccessor?.HttpContext?.GetRouteValue("action")?.ToString() + "";
            string user = User.FindFirstValue(ClaimTypes.NameIdentifier) + "";
            AccertamentoDichDecessoViewModel vm = new();

            _commonServices.CurrentMenu = "Anpr";
            ViewData["Title"] = "Anpr - Accertamento dichiarazione decesso";

            vm.AccertamentoMultiplo = await GetAccertamentoMultiploViewModel(user, servizio, null);
            vm.risultati = null;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AccertamentoDichDecesso(AccertamentoMultiploEditInputModel inputModel)
        {
            AccertamentoDichDecessoViewModel vm = new();
            DateOnly DataRiferimentoVerifica;
            string userLocation = _utilities.GetRemoteIPV4(Request?.HttpContext?.Connection?.RemoteIpAddress?.ToString());
            string userID = _httpContextAccessor?.HttpContext?.User?.Identity?.Name?.ToString() + "";
            string LoA = "LOA1";
            string purpouseID = "";
            string operationGuid = Guid.NewGuid().ToString();
            const string codiceStato = "100";
            const string descrizioneStato = "ITALIA";
            string user = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value + "";
            string servizio = _httpContextAccessor?.HttpContext?.GetRouteValue("action")?.ToString() + "";
            Purpouse purpouse;


            _commonServices.CurrentMenu = "Anpr";
            ViewData["Title"] = "Anpr - Accertamento dichiarazione decesso";

            vm.AccertamentoMultiplo = await GetAccertamentoMultiploViewModel(user, servizio, inputModel);

            try
            {

                if (ModelState.IsValid)
                {


                    DateOnly.TryParse(inputModel.DataRiferimentoVerifica, CultureInfo.InvariantCulture, out DataRiferimentoVerifica);


                    purpouse = await _consumerServices.GetUserFinalityCurrentPurpouseIDAsync(user, int.Parse(inputModel.FinalitaId));

                    purpouseID = purpouse.Valore;

                    vm.risultati = await _anprServices.ServizioAccertamentoDichDecessoAsync(user, inputModel.CodiciFiscali, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);

                    var report = _anprServices.CreateReport(servizio, "Anpr - Accertamento dichiarazione decesso", vm.AccertamentoMultiplo.DescrizioneFinalita, vm.risultati.ToList<IElementoSchedaReport>(), DataRiferimentoVerifica, userID, userLocation, LoA, operationGuid);
                    if (report is not null)
                    {
                        vm.AccertamentoMultiplo.Pdf = await report.ExportAsPdfAsync(_commonServices.PercorsoWWW);
                        vm.AccertamentoMultiplo.Xlsx = await report.ExportAsExcelAsync(_commonServices.PercorsoWWW);
                    }
                }


            }
            catch (ThresholdException ex)
            {
                TempData["EsitoAttenzione"] = ex.Message;
            }

            return View(vm);
        }


        [HttpGet]
        public async Task<IActionResult> AccertamentoVedovanza()
        {
            string servizio = _httpContextAccessor?.HttpContext?.GetRouteValue("action")?.ToString() + "";
            string user = User.FindFirstValue(ClaimTypes.NameIdentifier) + "";
            AccertamentoVedovanzaViewModel vm = new();

            _commonServices.CurrentMenu = "Anpr";
            ViewData["Title"] = "Anpr - Accertamento vedovanza";

            vm.AccertamentoMultiplo = await GetAccertamentoMultiploViewModel(user, servizio, null);
            vm.risultati = null;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AccertamentoVedovanza(AccertamentoMultiploEditInputModel inputModel)
        {
            AccertamentoVedovanzaViewModel vm = new();
            DateOnly DataRiferimentoVerifica;
            string userLocation = _utilities.GetRemoteIPV4(Request?.HttpContext?.Connection?.RemoteIpAddress?.ToString());
            string userID = _httpContextAccessor?.HttpContext?.User?.Identity?.Name?.ToString() + "";
            string LoA = "LOA1";
            string purpouseID = "";
            string operationGuid = Guid.NewGuid().ToString();
            const string codiceStato = "100";
            const string descrizioneStato = "ITALIA";
            string user = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value + "";
            string servizio = _httpContextAccessor?.HttpContext?.GetRouteValue("action")?.ToString() + "";
            Purpouse purpouse;


            _commonServices.CurrentMenu = "Anpr";
            ViewData["Title"] = "Anpr - Accertamento vedovanza";

            vm.AccertamentoMultiplo = await GetAccertamentoMultiploViewModel(user, servizio, inputModel);

            try
            {

                if (ModelState.IsValid)
                {

                    DateOnly.TryParse(inputModel.DataRiferimentoVerifica, CultureInfo.InvariantCulture, out DataRiferimentoVerifica);


                    purpouse = await _consumerServices.GetUserFinalityCurrentPurpouseIDAsync(user, int.Parse(inputModel.FinalitaId));

                    purpouseID = purpouse.Valore;

                    vm.risultati = await _anprServices.ServizioAccertamentoVedovanzaAsync(user, inputModel.CodiciFiscali, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);

                    var report = _anprServices.CreateReport(servizio, "Anpr - Accertamento vedovanza", vm.AccertamentoMultiplo.DescrizioneFinalita, vm.risultati.ToList<IElementoSchedaReport>(), DataRiferimentoVerifica, userID, userLocation, LoA, operationGuid);
                    if (report is not null)
                    {
                        vm.AccertamentoMultiplo.Pdf = await report.ExportAsPdfAsync(_commonServices.PercorsoWWW);
                        vm.AccertamentoMultiplo.Xlsx = await report.ExportAsExcelAsync(_commonServices.PercorsoWWW);
                    }
                }


            }
            catch (ThresholdException ex)
            {
                TempData["EsitoAttenzione"] = ex.Message;
            }

            return View(vm);
        }



        [HttpGet]
        public async Task<IActionResult> AccertamentoEsistenzaVita()
        {
            string servizio = _httpContextAccessor?.HttpContext?.GetRouteValue("action")?.ToString() + "";
            string user = User.FindFirstValue(ClaimTypes.NameIdentifier) + "";
            AccertamentoEsistenzaVitaViewModel vm = new();

            _commonServices.CurrentMenu = "Anpr";
            ViewData["Title"] = "Anpr - Accertamento esistenza in vita";

            vm.AccertamentoMultiplo = await GetAccertamentoMultiploViewModel(user, servizio, null);
            vm.risultati = null;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AccertamentoEsistenzaVita(AccertamentoMultiploEditInputModel inputModel)
        {
            AccertamentoEsistenzaVitaViewModel vm = new();
            DateOnly DataRiferimentoVerifica;
            string userLocation = _utilities.GetRemoteIPV4(Request?.HttpContext?.Connection?.RemoteIpAddress?.ToString());
            string userID = _httpContextAccessor?.HttpContext?.User?.Identity?.Name?.ToString() + "";
            string LoA = "LOA1";
            string purpouseID = "";
            string operationGuid = Guid.NewGuid().ToString();
            const string codiceStato = "100";
            const string descrizioneStato = "ITALIA";
            string user = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value + "";
            string servizio = _httpContextAccessor?.HttpContext?.GetRouteValue("action")?.ToString() + "";
            Purpouse purpouse;


            _commonServices.CurrentMenu = "Anpr";
            ViewData["Title"] = "Anpr - Accertamento esistenza in vita";

            vm.AccertamentoMultiplo = await GetAccertamentoMultiploViewModel(user, servizio, inputModel);

            try
            {

                if (ModelState.IsValid)
                {

                    DateOnly.TryParse(inputModel.DataRiferimentoVerifica, CultureInfo.InvariantCulture, out DataRiferimentoVerifica);


                    purpouse = await _consumerServices.GetUserFinalityCurrentPurpouseIDAsync(user, int.Parse(inputModel.FinalitaId));

                    purpouseID = purpouse.Valore;

                    vm.risultati = await _anprServices.ServizioAccertamentoEsistenzaVitaAsync(user, inputModel.CodiciFiscali, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);

                    var report = _anprServices.CreateReport(servizio, "Anpr - Accertamento esistenza in vita", vm.AccertamentoMultiplo.DescrizioneFinalita, vm.risultati.ToList<IElementoSchedaReport>(), DataRiferimentoVerifica, userID, userLocation, LoA, operationGuid);
                    if (report is not null)
                    {
                        vm.AccertamentoMultiplo.Pdf = await report.ExportAsPdfAsync(_commonServices.PercorsoWWW);
                        vm.AccertamentoMultiplo.Xlsx = await report.ExportAsExcelAsync(_commonServices.PercorsoWWW);
                    }
                }


            }
            catch (ThresholdException ex)
            {
                TempData["EsitoAttenzione"] = ex.Message;
            }

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> AccertamentoPaternita()
        {
            string servizio = _httpContextAccessor?.HttpContext?.GetRouteValue("action")?.ToString() + "";
            string user = User.FindFirstValue(ClaimTypes.NameIdentifier) + "";
            AccertamentoGenitoreViewModel vm = new();

            _commonServices.CurrentMenu = "Anpr";
            ViewData["Title"] = "Anpr - Accertamento paternità";

            vm.AccertamentoMultiplo = await GetAccertamentoMultiploViewModel(user, servizio, null);
            vm.risultati = null;
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AccertamentoPaternita(AccertamentoMultiploEditInputModel inputModel)
        {
            AccertamentoGenitoreViewModel vm = new();
            DateOnly DataRiferimentoVerifica;
            string userLocation = _utilities.GetRemoteIPV4(Request?.HttpContext?.Connection?.RemoteIpAddress?.ToString());
            string userID = _httpContextAccessor?.HttpContext?.User?.Identity?.Name?.ToString() + "";
            string LoA = "LOA1";
            string purpouseID = "";
            string operationGuid = Guid.NewGuid().ToString();
            const string codiceStato = "100";
            const string descrizioneStato = "ITALIA";
            string user = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value + "";
            string servizio = _httpContextAccessor?.HttpContext?.GetRouteValue("action")?.ToString() + "";
            Purpouse purpouse;


            _commonServices.CurrentMenu = "Anpr";
            ViewData["Title"] = "Anpr - Accertamento paternità";

            vm.AccertamentoMultiplo = await GetAccertamentoMultiploViewModel(user, servizio, inputModel);

            try
            {
                
                if (ModelState.IsValid)
                {


                    DateOnly.TryParse(inputModel.DataRiferimentoVerifica, CultureInfo.InvariantCulture, out DataRiferimentoVerifica);


                    purpouse = await _consumerServices.GetUserFinalityCurrentPurpouseIDAsync(user, int.Parse(inputModel.FinalitaId));

                    purpouseID = purpouse.Valore;

                    vm.risultati = await _anprServices.ServizioAccertamentoGenitoreAsync(TipologiaRicercaGenitore.Padre,user, inputModel.CodiciFiscali, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);

                    var report = _anprServices.CreateReport(servizio, "ANPR-Accertamento paternità", vm.AccertamentoMultiplo.DescrizioneFinalita, vm.risultati.ToList<IElementoSchedaReport>(), DataRiferimentoVerifica, userID, userLocation, LoA, operationGuid);
                    if (report is not null)
                    {
                        vm.AccertamentoMultiplo.Pdf = await report.ExportAsPdfAsync(_commonServices.PercorsoWWW);
                        vm.AccertamentoMultiplo.Xlsx = await report.ExportAsExcelAsync(_commonServices.PercorsoWWW);
                    }
                }


            }
            catch (ThresholdException ex)
            {
                TempData["EsitoAttenzione"] = ex.Message;
            }

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> AccertamentoMaternita()
        {
            string servizio = _httpContextAccessor?.HttpContext?.GetRouteValue("action")?.ToString() + "";
            string user = User.FindFirstValue(ClaimTypes.NameIdentifier) + "";
            AccertamentoGenitoreViewModel vm = new();

            _commonServices.CurrentMenu = "Anpr";
            ViewData["Title"] = "Anpr - Accertamento maternità";

            vm.AccertamentoMultiplo = await GetAccertamentoMultiploViewModel(user, servizio, null);
            vm.risultati = null;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AccertamentoMaternita(AccertamentoMultiploEditInputModel inputModel)
        {
            AccertamentoGenitoreViewModel vm = new();
            DateOnly DataRiferimentoVerifica;
            string userLocation = _utilities.GetRemoteIPV4(Request?.HttpContext?.Connection?.RemoteIpAddress?.ToString());
            string userID = _httpContextAccessor?.HttpContext?.User?.Identity?.Name?.ToString() + "";
            string LoA = "LOA1";
            string purpouseID = "";
            string operationGuid = Guid.NewGuid().ToString();
            const string codiceStato = "100";
            const string descrizioneStato = "ITALIA";
            string user = _httpContextAccessor?.HttpContext?.User?.Claims.FirstOrDefault()?.Value + "";
            string servizio = _httpContextAccessor?.HttpContext?.GetRouteValue("action")?.ToString() + "";
            Purpouse purpouse;


            _commonServices.CurrentMenu = "Anpr";
            ViewData["Title"] = "Anpr - Accertamento maternità";

            vm.AccertamentoMultiplo = await GetAccertamentoMultiploViewModel(user, servizio, inputModel);

            try
            {

                if (ModelState.IsValid)
                {

                    DateOnly.TryParse(inputModel.DataRiferimentoVerifica, CultureInfo.InvariantCulture, out DataRiferimentoVerifica);


                    purpouse = await _consumerServices.GetUserFinalityCurrentPurpouseIDAsync(user, int.Parse(inputModel.FinalitaId));

                    purpouseID = purpouse.Valore;

                    vm.risultati = await _anprServices.ServizioAccertamentoGenitoreAsync(TipologiaRicercaGenitore.Madre, user, inputModel.CodiciFiscali, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);

                    var report =  _anprServices.CreateReport(servizio, "ANPR-Accertamento maternità", vm.AccertamentoMultiplo.DescrizioneFinalita, vm.risultati.ToList<IElementoSchedaReport>(), DataRiferimentoVerifica, userID, userLocation, LoA, operationGuid);
                    if (report is not null)
                    {
                        vm.AccertamentoMultiplo.Pdf = await report.ExportAsPdfAsync(_commonServices.PercorsoWWW);
                        vm.AccertamentoMultiplo.Xlsx = await report.ExportAsExcelAsync(_commonServices.PercorsoWWW);
                    }
                }


            }
            catch (ThresholdException ex)
            {
                TempData["EsitoAttenzione"] = ex.Message;
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public  FileContentResult DownloadPDF(string PdfData)
        {
            
            byte[] data = Convert.FromBase64String(PdfData);
            return new FileContentResult(data,"application/pdf");
            
        }

        private async Task<AccertamentoMultiploViewModel> GetAccertamentoMultiploViewModel(string user,string servizio, AccertamentoMultiploEditInputModel? inputModel)
        {
            AccertamentoMultiploViewModel amvm = new();
            List<Finalita> finalita;

            amvm.NumeroMassimoCodiciFiscali = _anprServices.NumeroMassimoCodiciFiscali;
            amvm.Servizio = servizio;
            finalita = await _consumerServices.GetUserServiceFinalitiesListAsync(user, servizio);

            if(inputModel is null)
            {
                amvm.inputModel = new();
                amvm.inputModel.CodiciFiscali = "";

            } else
            {
                amvm.inputModel = inputModel;
            }

            amvm.ListaFinalita = finalita.AsQueryable().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Descrizione,  Selected = x.Id.ToString() == amvm.inputModel.FinalitaId }).ToList();
            return amvm;
        }

        private async Task<AccertamentoMassivoViewModel> GetAccertamentoMassivoViewModel(string user, string servizio, AccertamentoMassivoEditInputModel? inputModel)
        {
            AccertamentoMassivoViewModel amvm = new();
            List<Finalita> finalita;

   
            amvm.Servizio = servizio;
            finalita = await _consumerServices.GetUserServiceFinalitiesListAsync(user, servizio);

            if (inputModel is null)
            {
                amvm.inputModel = new();               

            }
            else
            {
                amvm.inputModel = inputModel;
            }

            amvm.ListaFinalita = finalita.AsQueryable().Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Descrizione, Selected = x.Id.ToString() == amvm.inputModel.FinalitaId }).ToList();
            return amvm;
        }



    }
}
