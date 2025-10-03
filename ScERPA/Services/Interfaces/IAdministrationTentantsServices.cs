using ScERPA.Areas.Administration.Models;
using ScERPA.Areas.Administration.ViewModels;
using ScERPA.Models;

namespace ScERPA.Services.Interfaces
{
    public interface IAdministrationTentantsServices
    {
        public Task<TenantsListViewModel> GetTenantsListViewModelAsync();
        public Task<TenantDetailsViewModel> GetTenantDetailsViewModelAsync(int id);
        public Task<TenantEditCreateViewModel> GetTenantEditCreateViewModelAsync(int? id, TenantEditInputModel? inputModel);
        public Task<int> CreateTenantAsync(TenantEditInputModel inputModel);
        public Task<int> UpdateTenantAsync(TenantEditInputModel inputModel);
        public Task<int> DeleteTenantAsync(int id);

    }
}
