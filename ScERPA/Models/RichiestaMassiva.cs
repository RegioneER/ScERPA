using ScERPA.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ScERPA.Models
{
    public class RichiestaMassiva
    {
        public int Id { get; set; }

        public string  IdAccodamento { get; set; } = string.Empty;
        int TentativiMassimi { get; set; } = 10;

        public int FinalitaId { get; set; }

        public Finalita Finalita { get; set; } = null!;

        public RichiestaMassivaEnum Stato { get; set; } = RichiestaMassivaEnum.Inserita;

        public List<ElementoRichiestaMassiva>? ListaElementiRichiestaMassiva { get; set; } = new();

        public string UserID { get; set; } = "";
        public ApplicationUser User { get; set; } = null!;

        public DateTime TimestampCreazioneRichiesta { get; set; }

        public DateTime TimestampUltimaElaborazione { get; set; }

        public AmbientiEnum Ambiente { get; set; } = AmbientiEnum.Test;
        [MaxLength(500), Required(ErrorMessage = "Campo obbligatorio")]

        public int TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;

    }
}
