using ScERPA.Data;
using ScERPA.Models.DTOs;
using ScERPA.Models.Enums;
using ScERPA.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Digests;

namespace ScERPA.Services
{
    public class AnalyticsServices : IAnalyticsServices
    {
        private readonly ScERPAContext _repository;
        private readonly ICommonServices _commonservices;

        public AnalyticsServices(ScERPAContext repository, ICommonServices commonservices) 
        { 
            _repository = repository;
            _commonservices = commonservices;
        }

        public async Task<TopServicesChartDto> GetTopServices(int first)
        {
            List<AnalyticsCategoryCount> statistiche;

            statistiche = await _repository.Chiamate.AsNoTracking().Where(x => x.CodiceRisposta == ApiResultStatus.Success && x.Ambiente==_commonservices.AmbienteCorrente && x.TenantId==_commonservices.IdTenantCorrente).Include(c => c.Finalita).ThenInclude(f => f.Servizio).GroupBy(g => g.Finalita.Servizio.Descrizione).Select(s => new AnalyticsCategoryCount() { Category = s.Key, Value = s.Count() }).OrderBy(o=>o.Value).Take(first).ToListAsync<AnalyticsCategoryCount>();

            TopServicesChartDto result = new()
            {
                categories = statistiche.Select(x=>x.Category).ToArray<string>(),
                values = statistiche.Select(x => x.Value).ToArray<int>()
            };

            return result;
        }
    }
}
