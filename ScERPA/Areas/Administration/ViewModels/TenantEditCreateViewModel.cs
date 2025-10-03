using ScERPA.Areas.Administration.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ScERPA.Areas.Administration.ViewModels
{
    public class TenantEditCreateViewModel
    {
        public TenantEditInputModel inputModel { get; set; } = new();

        public List<SelectListItem> Configurazioni { get; set; } = new();


    }
}
