using ScERPA.Models.EditInputModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ScERPA.ViewModels
{
    public class AccertamentoVedovanzaViewModel
    {
        public AccertamentoMultiploViewModel AccertamentoMultiplo { get; set; } = new();

        public List<RisultatoAccertamentoVedovanzaViewModel>? risultati { get; set; } = new();

    }
}
