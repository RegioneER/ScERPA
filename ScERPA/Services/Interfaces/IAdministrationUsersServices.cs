
using ScERPA.Areas.Administration.Models;
using ScERPA.Areas.Administration.ViewModels;
using ScERPA.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ScERPA.Services.Interfaces
{
    public interface IAdministrationUsersServices
    {
        public Task<List<UserListItemViewModel>> GetUsersListForFinalitiesByAdminAsync(UsersListSearchPanel? searchPanel);
        public Task<UserAdminViewModel> GetConsumerAsync(string? userId);

        public Task<List<UserAdminAreaServiceFinalityListItem>> GetUserAreasServicesFinalitiesListByAdminAsync(string currentAdmin, bool? isAdmin, string userId, int? finality);

        public Task<int> RemoveAssociatonOfFinalityToUserByAdminAsync(string currentAdmin, bool? isAdmin, string userId, int finality);
        public Task<int> AssociateFinalityToUserByAdminAsync(string currentAdmin, bool? isAdmin, string userId, int finality);
        public Task<List<SelectListItem>> GetUserAreasListByAdminAsync(string currentAdmin, bool? isAdmin, string userId);
        public Task<List<SelectListItem>> GetUserServicesListByAdminAsync(string currentAdmin, bool? isAdmin, string userId, int areaId);
        public Task<List<SelectListItem>> GetUserFinalitiesListByAdminAsync(string currentAdmin, bool? isAdmin, string userId, int areaId,int serviceId);

        public Task<List<UserListItemViewModel>> GetUsersListForProfileByAdminAsync(string currentAdmin, UsersListSearchPanel? searchPanel);
        public Task<UserAdminUserProfileViewModel> GetUserProfileDetailByAdminAsync(string currentAdmin, string userId);
        public Task<UserAminEditProfileViewModel> GetUserProfileForEditByAdminAsync(string currentAdmin, string userId);
        public Task<int> UpdateUserProfileByAdminAsync(string currentAdmin, UserProfileEditInputModel inputModel);
        public Task<List<Tenant>> GetUserTenantListAsync(string userId);

        public Task<string> GetUserMaxRoleAsync(string userId);
    }
    
}
