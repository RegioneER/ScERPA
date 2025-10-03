using ScERPA.Models;
using ScERPA.Models.EditInputModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ScERPA.ViewModels
{
    public class AccertamentoMultiploViewModel
    {
        public AccertamentoMultiploEditInputModel inputModel { get; set; } = new();

        public List<SelectListItem> ListaFinalita { get; set; } = new();

        public int NumeroMassimoCodiciFiscali { get; set; } = 1;

        public string Servizio { get; set; } = string.Empty;

        public string? Pdf { get; set; }

        public string? Xlsx { get; set; }

        public string DescrizioneFinalita {
            get {              
                return ListaFinalita.Where(x => x.Selected).Select(x => x.Text).FirstOrDefault() ?? "";
            }
        }
    }
}
