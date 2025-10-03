using System.ComponentModel.DataAnnotations;
using ScERPA.Areas.Administration.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Org.BouncyCastle.Bcpg;

namespace ScERPA.Areas.Administration.ViewModels
{
    public class FinalitiesListViewModel
    {
        public Paging paging { get; set; } = new();

        public List<FinalitiesListItemViewModel> Elenco { get; set; } = new();

        public FinalitiesSearchPanel searchPanel { get; set; } = new();

        public List<SelectListItem> Aree { get; set; } = new();

        public List<SelectListItem> Servizi { get; set; } = new();

    }
}
