using System.ComponentModel.DataAnnotations;

namespace ScERPA.Models.Enums.ANPR
{
    public enum TipologiaLegameConvivenza
    {
        [Display(Name = "Responsabile convivenza")]
        Responsabile_convivenza = 1,
        [Display(Name = "Membro della convivenza")]
        Membro_della_convivenza = 2,
        [Display(Name = "Altro")]
        Altro = 3,
        [Display(Name = "Ignoto")]
        Ignoto = 9
    }
}
