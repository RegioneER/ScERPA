using ScERPA.Areas.Administration.Models;
using ScERPA.Areas.Administration.ViewModels;
using ScERPA.Models;

namespace ScERPA.Services.Interfaces
{
    public interface IAdministrationConfigurationsServices
    {

        public Task<ConfigurationsListViewModel> GetConfigurationsListViewModelAsync();
        public Task<ConfigurationDetailsViewModel> GetConfigurationDetailsViewModelAsync(int id);
        public Task<ConfigurationEditCreateViewModel> GetConfigurationEditCreateViewModelAsync(int? id, ConfigurationEditInputModel? inputModel);
        public Task<int> CreateConfigurationAsync(ConfigurationEditInputModel inputModel);
        public Task<int> UpdateConfigurationAsync(ConfigurationEditInputModel inputModel);
        public Task<int> DeleteConfigurationAsync(int id);


    }
}
