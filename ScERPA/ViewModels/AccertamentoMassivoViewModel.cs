using ScERPA.Models.EditInputModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ScERPA.ViewModels
{
    public class AccertamentoMassivoViewModel
    {

        public AccertamentoMassivoEditInputModel inputModel { get; set; } = new();

        public List<SelectListItem> ListaFinalita { get; set; } = new();
        public int NumeroMassimoTentativi { get; set; } = 10;
        public string Servizio { get; set; } = string.Empty;
        public string DescrizioneFinalita
        {
            get
            {
                return ListaFinalita.Where(x => x.Selected).Select(x => x.Text).FirstOrDefault() ?? "";
            }
        }

    }


}
