using ScERPA.Areas.Administration.Models;

namespace ScERPA.Areas.Administration.ViewModels
{
    public class UsersListViewModel
    {
        public UsersListSearchPanel searchPanel { get; set; } = new();
        public List<UserListItemViewModel> Elenco { get; set; } = new();

    }
}
