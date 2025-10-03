using ScERPA.Areas.Administration.Models;
using ScERPA.Areas.Administration.ViewModels;
using ScERPA.Models;
using iText.Layout.Element;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Serialization;

namespace ScERPA.Services.Interfaces
{
    public interface IAdministrationFinalitiesServices
    {
        public Task<FinalitiesListViewModel> GetFinalitiesListViewModelAsync(FinalitiesSearchPanel? searchPanel, int currentPage);
        public Task<FinalityDetailsViewModel> GetFinalityDetailsViewModelAsync(int id);
        public Task<FinalityEditCreateViewModel> GetFinalityEditCreateViewModelAsync(int? id, FinalityEditInputModel? inputModel);
        public Task<int> CreateFinalityAsync(FinalityEditInputModel inputModel);
        public Task<int> UpdateFinalityAsync(FinalityEditInputModel inputModel);
        public Task<int> DeleteFinalityAsync(int id);
        public Purpouse? GetCurrentPurpouseId(int finalitaId);

        public Task<List<SelectListItem>> APIGetServicesListAsync(int areaId, int? servizioId);

    }
}
