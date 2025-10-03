using ScERPA.Areas.Administration.Models;
using ScERPA.Areas.Administration.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ScERPA.ViewModels.RchiesteMassive
{
    public class MassiveRequestsListViewModel
    {
        public Paging paging { get; set; } = new();

        public FinalitiesSearchPanel searchPanel { get; set; } = new();

        public List<MassiveRequestsListItemViewModel> Elenco { get; set; } = new();

        public List<SelectListItem> Aree { get; set; } = new();

        public List<SelectListItem> Servizi { get; set; } = new();

    }
}
