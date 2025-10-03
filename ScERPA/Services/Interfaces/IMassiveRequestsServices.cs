using ScERPA.Areas.Administration.Models;
using ScERPA.Models.EditInputModels;
using ScERPA.ViewModels.RchiesteMassive;

namespace ScERPA.Services.Interfaces
{
    public interface IMassiveRequestsServices
    {
        public Task<int> CreateMassiveRequestWithDataAsync(string user, int idFinalita, List<string> dati);

        public Task<MassiveRequestsListViewModel> GetMassiveRequestsListViewModelAsync(FinalitiesSearchPanel? searchPanel, int currentPage);

        public Task AppendRequestAsync(int idRichiesta);

    }
}
