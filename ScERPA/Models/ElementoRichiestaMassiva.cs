using ScERPA.Services;
using System.ComponentModel.DataAnnotations;

namespace ScERPA.Models
{
    public class ElementoRichiestaMassiva
    {
        public long Id { get; set; }
        public int RichiestaMassivaId { get; set; }
        public RichiestaMassiva Richiesta { get; set; } = null!;
        [StringLength(32000)]
        public string Input { get; set; } = null!;
        [StringLength(32000)]
        public string? Output { get; set; }
        public string Note { get; set; } = string.Empty;
        public string IdAccodamento { get; set; } = string.Empty;
        public int Tentativo { get; set; }
        public DateTime TimestampCreazioneRichiesta { get; set; }
        public DateTime TimestampUltimaElaborazione { get; set; }
        public ApiResultStatus CodiceRisposta { get; set; } = ApiResultStatus.Unknown;

    }
}
