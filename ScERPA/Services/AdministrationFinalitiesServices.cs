using ScERPA.Data;
using ScERPA.Models;
using ScERPA.Services.Interfaces;
using NuGet.Protocol.Core.Types;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ScERPA.Models.Enums;
using Microsoft.EntityFrameworkCore;
using ScERPA.Areas.Administration.Models;
using ScERPA.Areas.Administration.ViewModels;
using ScERPA.ViewModels;
using ScERPA.Models.Exceptions;
using iText.StyledXmlParser.Jsoup.Nodes;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using Microsoft.Extensions.Azure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Globalization;
using iText.Kernel.Geom;
using System.Drawing.Printing;

namespace ScERPA.Services
{
    public class AdministrationFinalitiesServices : IAdministrationFinalitiesServices
    {
        private readonly ScERPAContext _repository;
        private readonly ICommonServices _commonServices;
        private readonly ILogger<AdministrationFinalitiesServices> _logger;
        private readonly IUtilities _utilities;


        public AdministrationFinalitiesServices(ScERPAContext repository, ICommonServices commonServices, ILogger<AdministrationFinalitiesServices> logger, IUtilities utilities)
        {
            _repository = repository;
            _commonServices = commonServices;
            _logger = logger;
            _utilities = utilities;
        }

        public async Task<FinalitiesListViewModel> GetFinalitiesListViewModelAsync(FinalitiesSearchPanel? searchPanel, int currentPage)
        {
            FinalitiesListViewModel vm = new();

            IQueryable<FinalitiesListItemViewModel> listaElementi;
            int areaId = 0;
            int servizioId = 0;
            const int PageSize= 15;

            searchPanel = searchPanel ?? new();
            searchPanel.AreaId = _utilities.SanitizeAsPlainText(searchPanel.AreaId ?? "");
            searchPanel.ServizioId = _utilities.SanitizeAsPlainText(searchPanel.ServizioId ?? "");
            searchPanel.DenominazioneFinalita = _utilities.SanitizeAsPlainText(searchPanel.DenominazioneFinalita ?? "");

            vm.searchPanel = searchPanel;

    
            listaElementi = from a in _repository.Aree
                            join e in _repository.Servizi on a.Id equals e.AreaId
                            join f in _repository.ListaFinalita on e.Id equals f.ServizioId
                            where f.Ambiente == _commonServices.AmbienteCorrente && f.TenantId == _commonServices.IdTenantCorrente                          
                            select new FinalitiesListItemViewModel { Id = f.Id, Area = a.Nome, AreaId= a.Id,Servizio = e.Descrizione, ServizioId=e.Id, Attivo = e.Attivo ? "Si" : "No", Nome = f.Nome, DataDal = f.DataDal.ToString("d"), DataAl = (f.DataAl == null ? "" : f.DataAl.Value.ToString("d")) };



            if(searchPanel is not null)
            {
                if (!string.IsNullOrEmpty(searchPanel.AreaId) && int.TryParse(searchPanel.AreaId, out areaId))
                {
                    listaElementi = listaElementi.Where(x => x.AreaId == areaId);
                }
                if (!string.IsNullOrEmpty(searchPanel.ServizioId) && int.TryParse(searchPanel.ServizioId, out servizioId))
                {
                    listaElementi = listaElementi.Where(x => x.ServizioId == servizioId);
                }
                if (!string.IsNullOrEmpty(searchPanel.DenominazioneFinalita))
                {
                    listaElementi = listaElementi.Where(x => EF.Functions.Like(x.Nome, $"%{searchPanel.DenominazioneFinalita}%"));
                }
            }

            listaElementi = listaElementi.AsNoTracking().OrderByDescending(x => x.Id);


            

            vm.paging.PageSize = PageSize;
            vm.paging.TotalItems = await listaElementi.CountAsync();
            vm.paging.TotalPages = (int)Math.Ceiling(vm.paging.TotalItems / (double)PageSize);
            vm.paging.PageIndex = Math.Min(Math.Max(1,currentPage), vm.paging.TotalPages);
            vm.Elenco = await listaElementi.Skip((currentPage - 1) * PageSize).Take(PageSize).ToListAsync();

            //costruisco la select di Aree
            vm.Aree = await _repository.Aree
                .AsNoTracking()
                .OrderBy(x => x.Nome)
                .Select(x => new SelectListItem { Text = _utilities.SanitizeAsPlainText(x.Nome), Value = x.Id.ToString(), Selected = x.Id == areaId })
                .ToListAsync();

            //costruisco la select di Servizi
            vm.Servizi = await APIGetServicesListAsync(areaId, servizioId);

           


            return vm;
        }

        public async Task<int> CreateFinalityAsync(FinalityEditInputModel inputModel)
        {
            int id = 0;
     


            if (inputModel is not null)
            {
                DateTime dataDal;
                DateTime dataAl;
                int servizioId;
                
                
                Finalita finalita = new();
                //imposto i valori di default
                finalita.TenantId = _commonServices.IdTenantCorrente;
                finalita.Ambiente = _commonServices.AmbienteCorrente;
                finalita.IntervalloTempoChiamate = 1;
                finalita.UnitaTempoChiamate = UnitaTempoChiamateEnum.Giorno;


                if (int.TryParse(inputModel.ServizioId, out servizioId))
                {
                    finalita.ServizioId = servizioId;
                }
                else
                {
                    throw new EServiceNotFoundException("L'Id del servizio non può essere 0 o nullo per la finalità");
                }
                finalita.Nome = _utilities.SanitizeAsPlainText(inputModel.Nome);                
                finalita.Descrizione = _utilities.SanitizeAsPlainText(inputModel.Descrizione);
                if (DateTime.TryParseExact(inputModel.DataDal, new[] { "dd/MM/yyyy", "yyyy-MM-dd" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out dataDal))

                    finalita.DataDal = dataDal;
                else
                    finalita.DataDal = DateTime.Now;

                if (DateTime.TryParseExact(inputModel.DataAl, new[] { "dd/MM/yyyy", "yyyy-MM-dd" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out dataAl))
                    finalita.DataAl = dataAl;
                else
                    finalita.DataAl = null;   
                
                finalita.MaxChiamate = inputModel.MaxChiamate;

                _repository.Add<Finalita>(finalita);

                if (!string.IsNullOrEmpty(inputModel.PurpouseCod))
                {
                    Purpouse purpouse = new();
                    purpouse.Valore = _utilities.SanitizeAsPlainText(inputModel.PurpouseCod);
                    purpouse.DataDal = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day, 0, 0, 0, DateTimeKind.Local);
                    purpouse.DataAl = null!;
                    purpouse.Finalita = finalita;
                    _repository.Add<Purpouse>(purpouse);
                }
                               

                await _repository.SaveChangesAsync();

                id = finalita.Id;

                _logger.LogInformation("Creata finalità con id {Id}", id);
            }

            return id;
        }
   

        public async Task<FinalityDetailsViewModel> GetFinalityDetailsViewModelAsync(int id)
        {
            FinalityDetailsViewModel vm = new();
            Finalita? elemento;
            Purpouse? purpouse;
            string descrizioneChiamateMassime="";

            if (EntityExists(id))
            {

                elemento = await _repository.ListaFinalita.Include(x => x.Servizio).Include(x => x.Servizio.Area).Where(x => x.Id == id).FirstOrDefaultAsync();
                
                if (elemento is not null)
                {
                    vm.Id = elemento.Id;
                    vm.Servizio = _utilities.SanitizeAsPlainText(elemento.Servizio.Nome);
                    vm.ServizioId = elemento.Servizio.Id;
                    vm.Area = _utilities.SanitizeAsPlainText(elemento.Servizio.Area.Nome);
                    vm.AreaId = elemento.Servizio.AreaId;
                    vm.Nome = _utilities.SanitizeAsPlainText(elemento.Nome);
                    vm.Descrizione = elemento.Descrizione;
                    vm.Attivo = elemento.Servizio.Attivo ? "Si" : "No";
                    vm.DataDal = elemento.DataDal.ToString("d");
                    vm.DataAl = elemento.DataAl == null ? "" : elemento.DataAl.Value.ToString("d"); 

                    switch(elemento.UnitaTempoChiamate)
                    {
                        case UnitaTempoChiamateEnum.Minuti:
                            descrizioneChiamateMassime = elemento.IntervalloTempoChiamate == 1 ? $"{elemento.MaxChiamate} al minuto" : $"{elemento.MaxChiamate} su {elemento.IntervalloTempoChiamate} minuto"; 
                            break;
                        case UnitaTempoChiamateEnum.Ore:
                            descrizioneChiamateMassime = elemento.IntervalloTempoChiamate == 1 ? $"{elemento.MaxChiamate} all'ora" : $"{elemento.MaxChiamate} su {elemento.IntervalloTempoChiamate} ore";
                            break;
                        case UnitaTempoChiamateEnum.Giorno:
                            descrizioneChiamateMassime = elemento.IntervalloTempoChiamate == 1 ? $"{elemento.MaxChiamate} al giorno" : $"{elemento.MaxChiamate} su {elemento.IntervalloTempoChiamate} giorni";
                            break;
                    }

                    vm.ChiamateMassime = descrizioneChiamateMassime;
                    purpouse = this.GetCurrentPurpouseId(id);
                    if(purpouse is not null)
                    {
                        vm.PurpouseCod = _utilities.SanitizeAsPlainText(purpouse.Valore);
                        vm.PurpouseCodDataDal = purpouse.DataDal.ToString("d");
                    }
                }
                else
                {
                    throw new FinalityNotFoundException($"Non esiste una finalità con id = {id}");
                }
            }
            else
            {
                throw new FinalityNotFoundException($"Non esiste una finalità con id = {id}");
            }

            return vm;
        }
        public async Task<int> UpdateFinalityAsync(FinalityEditInputModel inputModel)
        {
            int id = 0;
            Finalita? finalita;
            DateTime dataDal;
            DateTime dataAl;
            int servizioId;
            Purpouse? currentPurpouse;

            if (inputModel is not null && EntityExists(inputModel.Id))
            {
                id = inputModel.Id;                
                finalita = _repository.ListaFinalita.Where(x => x.Id == id).Include(x => x.ListaPurpouse).Single();
                if (int.TryParse(inputModel.ServizioId, out servizioId))
                {
                    finalita.ServizioId = servizioId;
                }
                else
                {
                    throw new FinalityReferenceErrorException("Il servizio non può essere 0 per la finalità");
                }
                finalita.Nome = _utilities.SanitizeAsPlainText(inputModel.Nome);
                finalita.Descrizione = _utilities.SanitizeAsPlainText(inputModel.Descrizione);
                if (DateTime.TryParseExact(inputModel.DataDal, new[] { "dd/MM/yyyy", "yyyy-MM-dd" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out dataDal))

                    finalita.DataDal = dataDal;
                else
                    finalita.DataDal = DateTime.Now;

                if (DateTime.TryParseExact(inputModel.DataAl, new[] { "dd/MM/yyyy", "yyyy-MM-dd" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out dataAl))
                    finalita.DataAl = dataAl;
                else
                    finalita.DataAl = null;
                finalita.MaxChiamate = inputModel.MaxChiamate;

                currentPurpouse = GetCurrentPurpouseId(inputModel.Id);
                if(currentPurpouse is not null && !string.IsNullOrEmpty(inputModel.PurpouseCod) && currentPurpouse.Valore != inputModel.PurpouseCod)
                {
                    currentPurpouse.DataAl = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Local);
                    Purpouse purpouse = new();
                    purpouse.Valore = _utilities.SanitizeAsPlainText(inputModel.PurpouseCod);
                    purpouse.DataDal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Local);
                    purpouse.DataAl = null;
                    purpouse.Finalita = finalita;
                    _repository.Update<Purpouse>(currentPurpouse);
                    _repository.Add<Purpouse>(purpouse);

                }


                try
                {
                    _repository.Update(finalita);
                    await _repository.SaveChangesAsync();
                    _logger.LogInformation("Aggiornata finalità id {Id}", id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntityExists(finalita.Id))
                    {

                        throw new FinalityNotFoundException($"Tentativo di aggiornare finalità con id = {id} inesistente");
                    }
                    else
                    {
                        throw new FinalityConcurrencyErrorException($"Errore in aggiornamento finalità con id {id}");
                    }
                }

            }

            return id;
        }

        public async Task<int> DeleteFinalityAsync(int id)
        {
            Finalita? elemento;

            if (id == 0)
            {
                throw new FinalityNotFoundException($"Tentativo di cancellazione di una finalità con id = {id} inesistente inesistente o non si dispongono i permessi");
            }
            else if(EntityExists(id))
            {
                elemento = await _repository.ListaFinalita.FindAsync(id);
                if (elemento is null)
                {
                    throw new FinalityNotFoundException($"Tentativo di cancellazione di una finalità con id = {id} inesistente inesistente o non si dispongono i permessi");
                }
                else
                {
                    if (EntityDeletable(id))
                    {
                        _repository.ListaFinalita.Remove(elemento);
                        await _repository.SaveChangesAsync();
                        _logger.LogInformation("Cancellazione della finalità con id {Id}", id);
                    }
                    else
                    {
                        throw new FinalityUndeletableException($"Impossibile cancellare la finalità {id} perchè collegata a dati");

                    }

                }

            } else throw new FinalityNotFoundException($"Tentativo di cancellazione di una finalità con id = {id} inesistente o non si dispongono i permessi");

            return elemento.Id;
        }

        public Purpouse? GetCurrentPurpouseId(int finalitaId)
        {
            Purpouse? risultato;

            IEnumerable<Purpouse> listaPurpouse = from purpouse in _repository.Purpouses
                                                  where purpouse.FinalitaId == finalitaId && purpouse.DataAl == null
                                                  select purpouse;

            risultato = listaPurpouse.SingleOrDefault();

            return risultato;
            
        }

        private bool EntityExists(int id)
        {
            return (_repository.ListaFinalita?.Any(e => e.Id == id && e.TenantId==_commonServices.IdTenantCorrente)).GetValueOrDefault();
        }

        private bool EntityDeletable(int id)
        {
            int elementi = 0;

            var elementiCollegati = from f in _repository.ListaFinalita
                                    let listautenti = f.ListaUtenti
                                    from u in listautenti
                                    where f.Id == id
                                    select u.Id;

            elementi = elementiCollegati.Count();

            return elementi == 0;
        }

        public async Task<FinalityEditCreateViewModel> GetFinalityEditCreateViewModelAsync(int? id, FinalityEditInputModel? inputModel)
        {
            FinalityEditCreateViewModel vm = new();
            int areaId  = 0;
            int servizioId = 0;
            Finalita finalita;

            if (id is not null)
            {
                //sono nel caso di Edit
                if (EntityExists((int)id))
                {
                    if (inputModel is not null && ((int)id == inputModel.Id))
                    {
                        vm.inputModel = inputModel;

                    }
                    else
                    {
                        finalita = _repository.ListaFinalita.AsNoTracking().Where(x => x.Id == id).Include(x => x.ListaPurpouse).Include(x=>x.Servizio).Single();
                        vm.inputModel.Id = finalita.Id;
                        vm.inputModel.ServizioId = finalita.ServizioId.ToString();
                        vm.inputModel.AreaId = finalita.Servizio.AreaId.ToString();
                        vm.inputModel.Nome = _utilities.SanitizeAsPlainText(finalita.Nome);
                        vm.inputModel.Descrizione = _utilities.SanitizeAsPlainText(finalita.Descrizione);
                        vm.inputModel.DataDal = finalita.DataDal.ToString("yyyy-MM-dd");
                        vm.inputModel.DataAl = finalita.DataAl is null ? "" : finalita.DataAl?.ToString("yyyy-MM-dd");
                        vm.inputModel.MaxChiamate = finalita.MaxChiamate;
                        Purpouse? purpouse = GetCurrentPurpouseId(finalita.Id);
                        if (purpouse is not null) vm.inputModel.PurpouseCod = _utilities.SanitizeAsPlainText(purpouse.Valore);

                    }
                }
                else throw new FinalityNotFoundException($"Tentativo di modificare una finalità con id {id} inesistente");

            }
            else
            {
                if (inputModel is not null)
                {
                    //sono in Create ma il model non è valido quindi lo restituisco per consentire le modifiche
                    vm.inputModel = inputModel;
                }

            }

            //imposto i valori di default
            vm.inputModel.TenantId = _commonServices.IdTenantCorrente;
            vm.inputModel.Ambiente = _commonServices.AmbienteCorrente;
            vm.inputModel.IntervalloTempoChiamate = 1;
            vm.inputModel.UnitaTempoChiamate = UnitaTempoChiamateEnum.Giorno;

            int.TryParse(vm.inputModel.AreaId,out areaId);
            int.TryParse(vm.inputModel.ServizioId, out servizioId);

            //costruisco la select di Aree
            vm.Aree = await _repository.Aree
                .AsNoTracking()
                .OrderBy(x=>x.Nome)
                .Select(x => new SelectListItem { Text = _utilities.SanitizeAsPlainText(x.Nome), Value = x.Id.ToString(), Selected = x.Id == areaId })
                .ToListAsync();

            //costruisco la select di Servizi
            vm.Servizi = await APIGetServicesListAsync(areaId, servizioId);

            return vm;
        }

        public async Task<List<SelectListItem>> APIGetServicesListAsync(int areaId, int? servizioId)
        {
           return await _repository.Servizi
                .AsNoTracking()
                .Include(x => x.Area)
                .Where(x => x.AreaId == areaId)
                .OrderBy(x => x.Nome)
                .Select(x => new SelectListItem { Text = _utilities.SanitizeAsPlainText(x.Descrizione), Value = x.Id.ToString(), Selected = x.Id == servizioId })
                .ToListAsync();
        }
    }  
}