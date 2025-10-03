using ScERPA.Models.EditInputModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ScERPA.ViewModels
{
    public class AccertamentoGeneralitaViewModel
    {
        public AccertamentoMultiploViewModel AccertamentoMultiplo { get; set; } = new();

        public List<RisultatoAccertamentoGeneralitaViewModel>? risultati { get; set; } = new();


    }
}
