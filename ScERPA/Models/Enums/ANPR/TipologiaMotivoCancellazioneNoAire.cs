using System.ComponentModel.DataAnnotations;

namespace ScERPA.Models.Enums.ANPR
{
    public enum TipologiaMotivoCancellazioneAire
    {
        [Display(Name = "Morte accertata")]
        Morte_accertata = 1,
        [Display(Name = "Morte presunta giudizialmente dichiarata")]
        Morte_presunta_giudizialmente_dichiarata = 2,      
        [Display(Name = "Iscrizione nell’APR di un comune non subentrato per trasferimento dall’estero")]
        Iscrizione_nell_APR_di_un_comune_non_subentrato_per_trasferimento_dall_estero = 3,
        [Display(Name = "Iscrizione nell’APR di un comune non subentrato per immigrazione dall’estero (art. 14 D.P.R 31/01/1958, n. 136)")]
        Iscrizione_nell_APR_di_un_comune_non_subentrato_per_immigrazione_dall_estero = 4,
        [Display(Name = "Irreperibilità presunta")]
        Irreperibilita_presunta = 5,
        [Display(Name = "Perdita della cittadinanza")]
        Perdita_della_cittadinanza = 6,
        [Display(Name = "Trasferimento nell’AIRE di altro comune non subentrato")]
        Trasferimento_nell_AIRE_di_altro_comune_non_subentrato = 7,
        [Display(Name = "Altro")]
        Altro = 8,
        [Display(Name = "Doppia iscrizione")]
        Doppia_iscrizione = 10        
    }
}
