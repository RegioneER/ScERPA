using ScERPA.Models.EditInputModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ScERPA.ViewModels
{
    public class AccertamentoStatoDiFamigliaViewModel
    {
        public AccertamentoMultiploViewModel AccertamentoMultiplo { get; set; } = new();

        public List<RisultatoAccertamentoStatoDiFamigliaViewModel>? risultati { get; set; } = new();

    }
}
