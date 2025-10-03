using ScERPA.Data;
using ScERPA.Models;
using ScERPA.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;

namespace ScERPA.Services
{


    public class ApiServices : IApiServices
    {
        private readonly ScERPAContext _repository;
        private readonly ICommonServices _commonServices;
        private readonly ILogger<ApiServices> _logger;

        public ApiServices(ScERPAContext repository, ICommonServices commonServices, ILogger<ApiServices> logger)
        {
            _repository = repository;
            _commonServices = commonServices;
            _logger = logger;
        }
        public async Task<Finalita> GetFinalityFromPurpouseIDAsync(string purpouseId)
        {
            Finalita risultatoRicerca;
            int currentTenantId = _commonServices.IdTenantCorrente;

            IEnumerable<Finalita> listaFinalita = from purpouse in _repository.Purpouses
                                                  join finalita in _repository.ListaFinalita on purpouse.FinalitaId equals finalita.Id
                                                  where purpouse.Valore == purpouseId && finalita.TenantId == currentTenantId && finalita.Ambiente== _commonServices.AmbienteCorrente
                                                  select finalita;


            risultatoRicerca = await listaFinalita.AsQueryable().AsNoTracking().Include(x => x.Servizio).SingleAsync();

            await _repository.Entry(risultatoRicerca).Collection(x => x.ListaPurpouse).LoadAsync();
            if(risultatoRicerca.Servizio is not null && risultatoRicerca.Servizio.Attivo)
            {
                await _repository.Entry(risultatoRicerca.Servizio).Collection(x => x.IndirizziChiamata).LoadAsync();
            }
            

            return risultatoRicerca;

        }
    }
}