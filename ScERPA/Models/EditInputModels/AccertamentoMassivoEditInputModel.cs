
using System.ComponentModel.DataAnnotations;

namespace ScERPA.Models.EditInputModels
{
    public class AccertamentoMassivoEditInputModel
    {
        [Required(ErrorMessage = "Devi selezionare una finalità")]
        public string FinalitaId { get; set; } = "";
        [Required(ErrorMessage = "Devi selezionare un file")]
        public  IFormFile? FileRichiesta { get; set; } 
        public bool Dichiarazione { get; set; } = false;
    }
}
