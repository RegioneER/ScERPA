using System.ComponentModel.DataAnnotations;

namespace ScERPA.Models.Enums.ANPR
{
    public enum TipologiaIndirizzo
    {
        [Display(Name = "Residenza")]
        Residenza = 1,
        [Display(Name = "Dimora abituale")]
        Dimora_abituale = 2,
        [Display(Name = "Domicilio eletto")]
        Domicilio_eletto = 3,
        [Display(Name = "Residenza estera")]
        Residenza_estera = 4,
        [Display(Name = "Presso/per località italiana")]
        Presso_per_località_italiana = 5,
        [Display(Name = "Presso/per località estera")]
        Presso_per_località_estera = 6,
        [Display(Name = "Ultima residenza italiana")]
        Ultima_residenza_italiana = 7,
        [Display(Name = "Residenza temporanea")]
        Residenza_temporanea = 8,
        [Display(Name = "Altro")]
        Altro = 9,
        [Display(Name = "Revisione onomastica stradale")]
        Revisione_onomastica_stradale = 10,
        [Display(Name = "Rettifica post accertamenti")]
        Rettifica_post_accertamenti = 11
    }
}
