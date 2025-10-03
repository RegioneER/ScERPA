using Microsoft.AspNetCore.Mvc.Rendering;
using ScERPA.Areas.Administration.Models;

namespace ScERPA.Areas.Administration.ViewModels
{
    public class UserAminAssociatePurpouseViewModel
    {
        public UserFinalitiesEditInputModel Input { get; set; } = new();
        public string Cognome { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public List<SelectListItem> Aree { get; set; } = new();
        public List<SelectListItem> Servizi { get; set; } = new();
        public List<SelectListItem> ListaFinalita { get; set; } = new();
    }
}
