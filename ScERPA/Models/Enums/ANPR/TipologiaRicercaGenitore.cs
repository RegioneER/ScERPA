using System.ComponentModel.DataAnnotations;

namespace ScERPA.Models.Enums.ANPR
{
    public enum TipologiaRicercaGenitore
    {
        [Display(Name = "Madre")]
        Madre = 1,
        [Display(Name = "Padre")]
        Padre = 2,
     
    }
}
