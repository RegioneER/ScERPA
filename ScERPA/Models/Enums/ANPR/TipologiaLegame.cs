using System.ComponentModel.DataAnnotations;

namespace ScERPA.Models.Enums.ANPR
{
    public enum TipologiaLegame
    {
        [Display(Name = "Residente in italia")]
        Residente_in_Italia = 1,
        [Display(Name = "Convivenza anagrafica")]
        Convivenza_anagrafica = 2,
        [Display(Name = "Famiglia AIRE")]
        Aire = 3,

    }
}
