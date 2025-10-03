using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace ScERPA.Models
{
    public class Servizio
    {
        public int Id { get; set; }
        [MaxLength(500), Required(ErrorMessage = "Campo obbligatorio")]
        public string Nome { get; set; } = String.Empty;
        [MaxLength(1000), Required(ErrorMessage = "Campo obbligatorio")]
        public string Descrizione { get; set; } = String.Empty;
        [MaxLength(250)]
        public string? Cod { get; set; }
        public int AreaId { get; set; }
        public Area Area { get; set; } = null!;
        public int? SezioneId { get; set; }
        public Sezione? Sezione { get; set; }
        public bool Attivo { get; set; } = true;
        public List<Finalita>? ListaFinalita { get; set; } = new();
        public List<IndirizzoChiamata>? IndirizziChiamata { get; set; } = new();

    }
}
