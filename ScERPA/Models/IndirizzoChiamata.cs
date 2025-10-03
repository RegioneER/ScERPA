using ScERPA.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ScERPA.Models
{
    public class IndirizzoChiamata
    {
        public int Id { get; set; }

        public AmbientiEnum Ambiente { get; set; } = AmbientiEnum.Test;

        public int? TenantId { get; set; }

        public Tenant? Tenant { get; set; }
        [MaxLength(5000), Required(ErrorMessage = "Campo obbligatorio")]
        public string Indirizzo { get; set; } = string.Empty;

        public int ServizioId { get; set; }
        public Servizio Servizio { get; set; } = null!;

    }
}
