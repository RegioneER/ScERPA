using System.ComponentModel.DataAnnotations;

namespace ScERPA.Models.Enums.ANPR
{
    public enum TipologiaRelazioniParentela
    {
        [Display(Name = "Intestatario scheda")]
        Intestatario_Scheda = 1,
        [Display(Name = "Marito/Moglie")]
        Marito_Moglie = 2,
        [Display(Name = "Figlio/Figlia")]
        Figlio_Figlia = 3,
        [Display(Name = "Nipote (discendente)")]
        Nipote_discendente = 4,
        [Display(Name = "Pronipote (discendente)")]
        Pronipote_discendente = 5,
        [Display(Name = "Padre/Madre")]
        Padre_Madre = 6,
        [Display(Name = "Nonno/Nonna")]
        Nonno_Nonna = 7,
        [Display(Name = "Bisnonno/Bisnonna")]
        Bisnonno_Bisnonna = 8,
        [Display(Name = "Fratello/Sorella")]
        Fratello_Sorella = 9,
        [Display(Name = "Nipote (collaterale)")]
        Nipote_collaterale = 10,
        [Display(Name = "Zia/Zio (collaterale)")]
        Zio_Zia_Collaterale = 11,
        [Display(Name = "Cugino/Cugina")]
        Cugino_Cugina = 12,
        [Display(Name = "Altro parente")]
        Altro_Parente = 13,
        [Display(Name = "Figliastro/Figliastra")]
        Figliastro_Figliastra = 14,
        [Display(Name = "Patrigno/Matrigna")]
        Patrigno_Matrigna = 15,
        [Display(Name = "Genero/Nuora")]
        Genero_Nuora = 16,
        [Display(Name = "Suocero/Suocera")]
        Suocero_Suocera = 17,
        [Display(Name = "Cognato/Cognata")]
        Cognato_Cognata = 18,
        [Display(Name = "Fratellastro/Sorellastra")]
        Fratellastro_Sorellastra = 19,
        [Display(Name = "Nipote (affine)")]
        Nipote_Affine = 20,
        [Display(Name = "Zio/Zia (affine)")]
        Zio_Zia_Affine = 21,
        [Display(Name = "Altro affine")]
        Altro_Affine = 22,
        [Display(Name = "Convivente (con vincoli di adozione o affettivi)")]
        Convivente_con_vincoli_di_adozione_o_affettivi = 23,
        [Display(Name = "Responsabile della convivenza non affettiva")]
        Responsabile_della_convivenza_non_affettiva = 24,
        [Display(Name = "Convivente in convivenza non affettiva")]
        Convivente_in_convivenza_non_affettiva = 25,
        [Display(Name = "Tutore")]
        Tutore = 26,
        [Display(Name = "Unito civilmente")]
        Unito_civilmente = 28,
        [Display(Name = "Adottato")]
        Adottato = 80,
        [Display(Name = "Nipote")]
        Nipote = 81,
        [Display(Name = "Non definito/comunicato")]
        Non_definito_comunicato = 99,
    }
}
