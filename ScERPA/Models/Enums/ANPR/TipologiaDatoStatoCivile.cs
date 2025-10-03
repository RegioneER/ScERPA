using System.ComponentModel.DataAnnotations;

namespace ScERPA.Models.Enums.ANPR
{
    public enum TipologiaDatoStatoCivile
    {
        [Display(Name = "Celibe/Nubile")]
        RCelibe_Nubile = 1,
        [Display(Name = "Coniugato/a")]
        Coniugato_a = 2,
        [Display(Name = "Vedovo/a")]
        Vedovo_a = 3,
        [Display(Name = "Divorziato/a")]
        Divorziato_a = 4,
        [Display(Name = "Non classificabile/ignoto/n.c")]
        Non_classificabile = 9,
        [Display(Name = "")]
        Unito_civilmente = 6,
        [Display(Name = "Stato libero a seguito di decesso della parte unita civilmente")]
        libero_a_seguito_di_decesso = 7,
        [Display(Name = "Stato libero a seguito di scioglimento dell'unione")]
        libero_a_seguito_di_scioglimento = 8,
    }
}
