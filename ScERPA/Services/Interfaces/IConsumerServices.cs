using ScERPA.Areas.Administration.ViewModels;
using ScERPA.Models;
using ScERPA.ViewModels;

namespace ScERPA.Services.Interfaces
{
    public interface IConsumerServices
    {

        public Task<List<Area>> GetUserAreasListAsync(string userId);

        public Task<List<Servizio>> GetUserAreaServicesListAsync(string userId, int areaId);

        public Task<List<Finalita>> GetUserServiceFinalitiesListAsync(string userId,string serviceName);

        public Task<Purpouse> GetUserFinalityCurrentPurpouseIDAsync(string userId, int finalitaId);

        public Task<Soglia> GetFinalityThresholdAsync(string purpouseId);

        public Task<List<Area>> GetUserAreasCachedListAsync(string userId, bool forceNoCache = false);

        public Task<List<ServizioMenuViewModel>> GetUserAreaServicesCachedListAsync(string userId, int areaId, bool forceNoCache = false);

    }
}
