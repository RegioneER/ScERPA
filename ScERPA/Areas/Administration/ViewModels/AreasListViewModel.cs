using System.ComponentModel.DataAnnotations;

namespace ScERPA.Areas.Administration.ViewModels
{
    public class AreasListViewModel
    {
        public List<AreasListItemViewModel> Elenco { get; set; } = new();

    }
}
