using System.ComponentModel.DataAnnotations;

namespace ScERPA.Areas.Administration.ViewModels
{

    public class TenantsListViewModel
    {
        public List<TenantsListItemViewModel> Elenco { get; set; } = new();

    }

}
