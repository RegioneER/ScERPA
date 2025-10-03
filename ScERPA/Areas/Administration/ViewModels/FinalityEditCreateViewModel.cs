using ScERPA.Areas.Administration.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ScERPA.Areas.Administration.ViewModels
{
    public class FinalityEditCreateViewModel
    {
        public FinalityEditInputModel inputModel { get; set; } = new();

        public List<SelectListItem> Aree { get; set; } = new();

        public List<SelectListItem> Servizi { get; set; } = new();

    }
}
