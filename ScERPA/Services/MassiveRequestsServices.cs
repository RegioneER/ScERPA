using ScERPA.Data;
using ScERPA.Models;
using ScERPA.Models.EditInputModels;
using ScERPA.Services.Interfaces;
using iText.IO.Source;
using iText.StyledXmlParser.Css.Resolve.Shorthand.Impl;
using Microsoft.DiaSymReader;
using ScERPA.Models.Enums;
using ScERPA.Areas.Administration.Models;
using ScERPA.Areas.Administration.ViewModels;
using ScERPA.ViewModels.RchiesteMassive;
using Microsoft.EntityFrameworkCore;

namespace ScERPA.Services
{
    public class MassiveRequestsServices : IMassiveRequestsServices
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MassiveRequestsServices> _logger;
        private readonly ICommonServices _commonServices;
        private readonly ScERPAContext _repository;

        public MassiveRequestsServices(IConfiguration configuration, ILogger<MassiveRequestsServices> logger,ICommonServices commonServices, ScERPAContext repository)
        {
            _configuration = configuration;
            _logger = logger;
            _commonServices = commonServices;
            _repository = repository;
        }

        public async Task AppendRequestAsync(int idRichiesta)
        {
            RichiestaMassiva? richiesta;

            richiesta = await _repository.RichiesteMassive.FindAsync(idRichiesta);
            if (richiesta is not null)
            {
                richiesta.Stato = RichiestaMassivaEnum.In_corso_di_lavorazione;
                _repository.Update(richiesta);
                await _repository.SaveChangesAsync();
            }
            else throw new KeyNotFoundException();

        }

        public async Task<int> CreateMassiveRequestWithDataAsync(string user, int idFinalita, List<string> dati)
        {
            int idRichiestaMassiva=0;
            RichiestaMassiva richiesta;
            int TenantId = _commonServices.IdTenantCorrente;
            AmbientiEnum Ambiente = _commonServices.AmbienteCorrente;


            if (dati.Count > 0)
            {
                richiesta = new RichiestaMassiva();
                richiesta.FinalitaId = idFinalita;
                richiesta.UserID = user;
                richiesta.IdAccodamento = Guid.NewGuid().ToString();
                richiesta.TimestampCreazioneRichiesta= DateTime.Now;
                richiesta.TimestampUltimaElaborazione= richiesta.TimestampCreazioneRichiesta;
                richiesta.Stato = RichiestaMassivaEnum.Inserita;
                richiesta.Ambiente = Ambiente;
                richiesta.TenantId = TenantId;
                foreach (string datoInput in dati)
                {
                    DateTime creazione = DateTime.Now;
                    richiesta.ListaElementiRichiestaMassiva.Add(
                            new ElementoRichiestaMassiva
                            {
                                Input = datoInput,
                                Tentativo = 0,
                                TimestampCreazioneRichiesta = creazione,
                                TimestampUltimaElaborazione = creazione,
                                CodiceRisposta = 0
                            });
                };

                await _repository.AddAsync(richiesta);

                await _repository.SaveChangesAsync();

                idRichiestaMassiva = richiesta.Id;

            };

            return idRichiestaMassiva;
        }

        public async Task<MassiveRequestsListViewModel> GetMassiveRequestsListViewModelAsync(FinalitiesSearchPanel? searchPanel, int currentPage) 
        {

            MassiveRequestsListViewModel vm = new();
            IQueryable<MassiveRequestsListItemViewModel> listaElementi;
            int areaId = 0;
            int servizioId = 0;
            const int PageSize = 15;

            listaElementi = from a in _repository.Aree
                            join e in _repository.Servizi on a.Id equals e.AreaId
                            join f in _repository.ListaFinalita on e.Id equals f.ServizioId
                            join rm in _repository.RichiesteMassive on f.Id equals rm.FinalitaId
                            join rme in _repository.ElementiRichiesteMassive on rm.Id equals rme.RichiestaMassivaId into GruppoElementi
                            where f.Ambiente == _commonServices.AmbienteCorrente && f.TenantId == _commonServices.IdTenantCorrente
                            select new MassiveRequestsListItemViewModel { 
                                Id = rm.Id, 
                                Area = a.Nome, 
                                AreaId = a.Id, 
                                Servizio = e.Descrizione, 
                                ServizioId = e.Id, 
                                Finalita = f.Descrizione,
                                FinalitaId = f.Id,
                                DataOraCreazione=rm.TimestampCreazioneRichiesta.ToString(),
                                DataOraEsecuzione = rm.TimestampUltimaElaborazione.ToString(),
                                Stato = rm.Stato.ToString(),
                                Elementi = GruppoElementi.Count()
                                
                            };

            listaElementi = listaElementi.AsNoTracking().OrderByDescending(x => x.Id);

            vm.paging.PageSize = PageSize;
            vm.paging.TotalItems = await listaElementi.CountAsync();
            vm.paging.TotalPages = (int)Math.Ceiling(vm.paging.TotalItems / (double)PageSize);
            vm.paging.PageIndex = Math.Min(Math.Max(1, currentPage), vm.paging.TotalPages);
            vm.Elenco = await listaElementi.Skip((currentPage - 1) * PageSize).Take(PageSize).ToListAsync();

            /*
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
             
             */


            return vm;
        }

       

    }
}
