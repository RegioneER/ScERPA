using ScERPA.Data;
using ScERPA.Models;
using ScERPA.Services.Interfaces;
using ScERPA.Models.Enums;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Caching.Memory;
using ScERPA.ViewModels;

namespace ScERPA.Services
{
    public class ConsumerServices : IConsumerServices
    {
        private readonly ScERPAContext _repository;
        private readonly ICommonServices _commonServices;
        private readonly ILogger<ConsumerServices>  _logger;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;


        public ConsumerServices(ScERPAContext repository, ICommonServices commonServices, ILogger<ConsumerServices> logger, IConfiguration configuration, IMemoryCache memoryCache)
        {
            _repository=repository;
            _commonServices = commonServices;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        

        public async Task<List<Area>> GetUserAreasListAsync(string userId)
        {
            List<Area> aree;
            int currentTenantId = _commonServices.IdTenantCorrente;   

            AmbientiEnum ambienteId = _commonServices.AmbienteCorrente;

           

            IEnumerable<Finalita> listaFinalitaUtente = (from utente in _repository.Users
                                                         from finalita in utente.ListaFinalita
                                                         from tenant in utente.Tenants
                                                         where utente.Id == userId && tenant.Id==currentTenantId && finalita.Ambiente == ambienteId && finalita.DataDal.Date <= DateTime.Now.Date && ((finalita.DataAl == null ? DateTime.Now.Date : finalita.DataAl.Value.Date) >= DateTime.Now.Date) && finalita.TenantId == currentTenantId
                                                         select finalita);

            _logger.LogInformation("finalità {Numero}", await listaFinalitaUtente.AsQueryable().AsNoTracking().CountAsync());

            IEnumerable<Area> listaAree = from area in _repository.Aree
                                              join servizio in _repository.Servizi on area.Id equals servizio.AreaId
                                              join finalita in listaFinalitaUtente on servizio.Id equals finalita.ServizioId
                                              where servizio.Attivo
                                              select area;

           
            aree = await listaAree.AsQueryable().AsNoTracking().Distinct().OrderBy(x=>x.Ordinale).ToListAsync();


            if (aree.Count == 0)
            {
                _logger.LogDebug("aree {Numero}", aree.Count);
            }

            return aree;
        }

        public async Task<List<Finalita>> GetUserServiceFinalitiesListAsync(string userId, string serviceName)
        {
            AmbientiEnum ambienteId = _commonServices.AmbienteCorrente;
            int currentTenantId = _commonServices.IdTenantCorrente;
            int serviceId;
            List<Finalita> listaFinalitaServizioUtente;

            IEnumerable<int> cercaServizio = from servizio in _repository.Servizi
                                             where servizio.Nome.ToUpper() == serviceName.ToUpper() && servizio.Attivo
                                             select servizio.Id;

            serviceId = await cercaServizio.AsQueryable().FirstOrDefaultAsync();

            IEnumerable<Finalita> listaFinalitaUtente = (from utente in _repository.Users
                                                         from finalita in utente.ListaFinalita
                                                         from tenant in utente.Tenants
                                                         where finalita.ServizioId== serviceId &&  utente.Id == userId && tenant.Id == currentTenantId && finalita.Ambiente == ambienteId && finalita.DataDal.Date <= DateTime.Now.Date && ((finalita.DataAl == null ? DateTime.Now.Date : finalita.DataAl.Value.Date) >= DateTime.Now.Date) && finalita.TenantId == currentTenantId
                                                         select finalita);


            listaFinalitaServizioUtente = await listaFinalitaUtente.AsQueryable().AsNoTracking().ToListAsync();

            return listaFinalitaServizioUtente;
        }

        public async Task<List<Servizio>> GetUserAreaServicesListAsync(string userId,int areaId) 
        {
            List<Servizio> servizi;

            AmbientiEnum ambienteId = _commonServices.AmbienteCorrente;
            int currentTenantId = _commonServices.IdTenantCorrente;


            IEnumerable<Finalita> listaFinalitaUtente = (from utente in _repository.Users
                                                         from finalita in utente.ListaFinalita
                                                         from tenant in utente.Tenants
                                                         where utente.Id == userId && tenant.Id == currentTenantId && finalita.Ambiente == ambienteId && finalita.DataDal.Date <= DateTime.Now.Date && ((finalita.DataAl == null ? DateTime.Now.Date : finalita.DataAl.Value.Date) >= DateTime.Now.Date) && finalita.TenantId == currentTenantId
                                                         select finalita);

            IEnumerable<Servizio> listaServizi = from servizio in _repository.Servizi 
                                          join finalita in listaFinalitaUtente on servizio.Id equals finalita.ServizioId
                                          where servizio.Attivo && servizio.AreaId== areaId
                                          select servizio;

            servizi = await listaServizi.AsQueryable().AsNoTracking().Include(x=>x.Sezione).Distinct().ToListAsync();

            return servizi;

        }

        public async Task<Purpouse> GetUserFinalityCurrentPurpouseIDAsync(string userId, int finalitaId)
        {
           
            AmbientiEnum ambienteId = _commonServices.AmbienteCorrente;
            int currentTenantId = _commonServices.IdTenantCorrente;

            Purpouse risultatoRicerca;

            IEnumerable<Finalita> listaFinalitaUtente = (from utente in _repository.Users
                                                         from finalita in utente.ListaFinalita
                                                         from tenant in utente.Tenants
                                                         where utente.Id == userId && tenant.Id == currentTenantId && finalita.Ambiente == ambienteId && finalita.DataDal.Date <= DateTime.Now.Date && ((finalita.DataAl == null ? DateTime.Now.Date : finalita.DataAl.Value.Date) >= DateTime.Now.Date) && finalita.TenantId == currentTenantId
                                                         select finalita);

            IEnumerable<Purpouse> listaPurpouse = from purpouse in _repository.Purpouses
                                                  join finalita in listaFinalitaUtente on purpouse.FinalitaId equals finalita.Id
                                                  where finalita.Id == finalitaId && purpouse.DataAl == null
                                                  select purpouse;

            risultatoRicerca = await listaPurpouse.AsQueryable().SingleAsync();

            return risultatoRicerca;
        }

        public async Task<Soglia> GetFinalityThresholdAsync(string purpouseId)
        {
            Soglia soglia = new();
            Finalita? finalitaCorrente;
            DateTime istanteInizioConteggio;

            //cerco la finalità e le soglie impostate
            var cercaFinalita = from purpouse in _repository.Purpouses
                       join finalita in _repository.ListaFinalita on purpouse.FinalitaId equals finalita.Id
                       where purpouse.Valore==purpouseId
                       select finalita;

            finalitaCorrente = await cercaFinalita.SingleOrDefaultAsync();          

            if(finalitaCorrente is not null)
            {
                soglia.SogliaChiamate = finalitaCorrente.MaxChiamate;

                DateTime InizioGiorno = new DateTime( DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Local);
                istanteInizioConteggio = InizioGiorno;

                switch (finalitaCorrente.UnitaTempoChiamate)
                {
                    case UnitaTempoChiamateEnum.Minuti:
                        istanteInizioConteggio = DateTime.Now.AddMinutes(-finalitaCorrente.IntervalloTempoChiamate);
                        break;
                    case UnitaTempoChiamateEnum.Ore:
                        istanteInizioConteggio = DateTime.Now.AddHours(-finalitaCorrente.IntervalloTempoChiamate);
                        break;
                    case UnitaTempoChiamateEnum.Giorno:
                        istanteInizioConteggio= InizioGiorno;
                        break;               
                }

                if (istanteInizioConteggio < InizioGiorno) istanteInizioConteggio = InizioGiorno;

                //cerco le chiamate effettuate (non fallite) con successo ed eventualmente con errore
                var calcolaChiamate = from chiamata in _repository.Chiamate
                                      where chiamata.Ambiente == _commonServices.AmbienteCorrente
                                      && chiamata.FinalitaId == finalitaCorrente.Id
                                      && chiamata.TimestampInvocazioneRichiesta >= istanteInizioConteggio
                                      && (chiamata.CodiceRisposta < ApiResultStatus.Failed)
                                      select chiamata.Id;

                soglia.ChiamateEffettuate = await calcolaChiamate.CountAsync();

            }
                                 

            return soglia;
        }

        public async Task<List<Area>> GetUserAreasCachedListAsync(string userId, bool forceNoCache = false)
        {
            var aree = _memoryCache.Get<List<Area>>($"Aree_{userId}_{_commonServices.IdTenantCorrente}");
            if(aree is null || forceNoCache)
            {
                aree = await GetUserAreasListAsync(userId);
                _memoryCache.Set($"Aree_{userId}_{_commonServices.IdTenantCorrente}", aree, DateTime.Now.AddHours(1));
            }
            return aree;
        }

        public async Task<List<ServizioMenuViewModel>> GetUserAreaServicesCachedListAsync(string userId, int areaId, bool forceNoCache = false)
        {
            var serviziMenu = _memoryCache.Get<List<ServizioMenuViewModel>>($"ServiziArea_{areaId}_{userId}_{_commonServices.IdTenantCorrente}");
            if(serviziMenu is null || forceNoCache)
            {
                var servizi = await GetUserAreaServicesListAsync(userId, areaId);
            
                 serviziMenu = servizi.AsQueryable().AsNoTracking().Select(x => new ServizioMenuViewModel { Id = x.Id, Nome= x.Nome, Descrizione= x.Descrizione,Sezione = (x.Sezione == null ?   "" : x.Sezione.Nome),Ordinale = (x.Sezione == null  ? 0 : x.Sezione.Ordinale)}).OrderBy(x => x.Ordinale).ThenBy(x=>x.Descrizione).ToList();
               
                _memoryCache.Set($"ServiziArea_{areaId}_{userId}_{_commonServices.IdTenantCorrente}", serviziMenu, DateTime.Now.AddHours(1));
            }
            return serviziMenu;
        }
    }
}

