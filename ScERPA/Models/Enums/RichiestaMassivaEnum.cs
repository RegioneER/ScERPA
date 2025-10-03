using System.ComponentModel.DataAnnotations;

namespace ScERPA.Models.Enums
{
    public enum RichiestaMassivaEnum
    {
        [Display(Name = "Inserita")]
        Inserita = 0,
        [Display(Name = "In corso di lavorazione")]
        In_corso_di_lavorazione = 1,
        [Display(Name = "Conclusa con esito positivo")]
        Conclusa_con_esito_positivo = 2,
        [Display(Name = "Conclusa con esito negativo")]
        Conclusa_con_esito_negativo = 3,
        [Display(Name = "Annullata")]
        Annullata = 4,
        [Display(Name = "Sospesa")]
        Sospesa = 5
    }
}
