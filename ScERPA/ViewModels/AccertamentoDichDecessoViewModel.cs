using ScERPA.Models.EditInputModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ScERPA.ViewModels
{
    public class AccertamentoDichDecessoViewModel
    {
        public AccertamentoMultiploViewModel AccertamentoMultiplo { get; set; } = new();

        public List<RisultatoAccertamentoDichDecessoViewModel>? risultati { get; set; } = new();


    }
}
