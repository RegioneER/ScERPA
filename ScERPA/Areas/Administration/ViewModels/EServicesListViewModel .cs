using System.ComponentModel.DataAnnotations;
using ScERPA.Areas.Administration.ViewModels;

namespace ScERPA.Areas.Administration.ViewModels
{
    public class EServicesListViewModel
    {
        public List<EServicesListItemViewModel> Elenco { get; set; } = new();

    }
}
