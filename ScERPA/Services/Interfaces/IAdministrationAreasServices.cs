using ScERPA.Areas.Administration.Models;
using ScERPA.Areas.Administration.ViewModels;
using ScERPA.Models;

namespace ScERPA.Services.Interfaces
{
    public interface IAdministrationAreasServices
    {
        public Task<AreasListViewModel> GetAreasListViewModelAsync();
        public Task<int> CreateAreaAsync(AreaEditInputModel inputModel);
        public Task<AreaDetailsViewModel> GetAreaViewModelAsync(int id);
        public Task<int> UpdateAreaAsync(AreaEditInputModel inputModel);
        public Task<int> DeleteAreaAsync(int id);

    }
}
