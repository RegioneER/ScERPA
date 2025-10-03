using ScERPA.Models.EditInputModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ScERPA.ViewModels
{
    public class VerificaCittadinanzaViewModel
    {
        public VerificaCittadinanzaEditInputModel inputModel { get; set; } = new();

        public List<SelectListItem> ListaFinalita { get; set; } = new();

        public List<RisultatoVerificaCittadinanzaViewModel>? risultati { get; set; } = new();

        public string? Pdf { get; set; }

        public string? Xlsx { get; set; }

        public int NumeroMassimoCodiciFiscali { get; set; } = 1;

    }
}
