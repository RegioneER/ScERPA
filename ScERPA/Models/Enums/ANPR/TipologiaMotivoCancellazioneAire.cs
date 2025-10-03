using System.ComponentModel.DataAnnotations;

namespace ScERPA.Models.Enums.ANPR
{
    public enum TipologiaMotivoCancellazioneNoAire
    {
        [Display(Name = "Morte accertata")]
        Morte_accertata = 1,
        [Display(Name = "Morte presunta giudizialmente dichiarata")]
        Morte_presunta_giudizialmente_dichiarata = 2,
        [Display(Name = "Trasferimento di residenza in comune non subentrato")]
        Vrasferimento_di_residenza_in_comune_non_subentrato = 3,
        [Display(Name = "Trasferimento di domicilio (senza fissa dimora)")]
        Trasferimento_di_domicilio_senza_fissa_dimora = 4,
        [Display(Name = "Trasferimento di residenza all’estero di uno straniero")]
        Trasferimento_di_residenza_all_estero_di_uno_straniero = 5,
        [Display(Name = "Irreperibilita")]
        Irreperibilita = 6,
        [Display(Name = "Mancato rinnovo dimora abituale/permesso di soggiorno")]
        Mancato_rinnovo_dimora_abituale_permesso_di_soggiorno = 7,
        [Display(Name = "Rettifiche post censuarie")]
        Rettifiche_post_censuarie = 9,
        [Display(Name = "Doppia iscrizione")]
        Doppia_iscrizione = 10,
        [Display(Name = "Perdita della cittadinanza italiana")]
        Perdita_della_cittadinanza_italiana = 11,
        [Display(Name = "Ripristino posizione precedente")]
        Ripristino_posizione_precedente = 12,
        [Display(Name = "Altro")]
        Altro = 13,
    }
}
