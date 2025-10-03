using ScERPA.Models.EditInputModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ScERPA.ViewModels
{
    public class AccertamentoEsistenzaVitaViewModel
    {
        public AccertamentoMultiploViewModel AccertamentoMultiplo { get; set; } = new();

        public List<RisultatoAccertamentoEsistenzaVitaViewModel>? risultati { get; set; } = new();


    }
}
