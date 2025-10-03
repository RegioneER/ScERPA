using ScERPA.Areas.Administration.Models;
using ScERPA.Areas.Administration.ViewModels;
using ScERPA.Models;

namespace ScERPA.Services.Interfaces
{
    public interface IAdministationEServicesServices
    {
        public Task<EServicesListViewModel> GetEServicesListViewModelAsync();
        public Task<int> CreateEServiceAsync(EServiceEditInputModel inputModel);
        public Task<EServiceDetailsViewModel> GetEServiceDetailsViewModelAsync(int id);
        public Task<int> UpdateEServiceAsync(EServiceEditInputModel inputModel);
        public Task<int> DeleteEServiceAsync(int id);


    }
}
