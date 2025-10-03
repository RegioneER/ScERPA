using ScERPA.Models.Enums;

using System.ComponentModel.DataAnnotations;

namespace ScERPA.Models
{
    public class Finalita
    {
        public int Id { get; set; }
        public AmbientiEnum Ambiente { get; set; } = AmbientiEnum.Test;
        [MaxLength(500), Required(ErrorMessage = "Campo obbligatorio")]
        public string Nome { get; set; } = string.Empty;
        [MaxLength(1000), Required(ErrorMessage = "Campo obbligatorio")]
        public string Descrizione { get; set; } = string.Empty;
        public List<Purpouse>? ListaPurpouse { get; set; } = new List<Purpouse>();

        public List<ApplicationUser>? ListaUtenti { get; set; } = new List<ApplicationUser>();
        [Required(ErrorMessage = "Campo obbligatorio")]
        public DateTime DataDal { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,0,0,0,DateTimeKind.Local);

        [Required(ErrorMessage = "Campo obbligatorio")]
        public int MaxChiamate { get; set; } = 0;
        [Required(ErrorMessage = "Campo obbligatorio")]
        public UnitaTempoChiamateEnum UnitaTempoChiamate { get; set; } = UnitaTempoChiamateEnum.Giorno;
        [Required(ErrorMessage = "Campo obbligatorio")]
        public int IntervalloTempoChiamate { get; set; } = 1;

        public DateTime? DataAl { get; set; }

        public int ServizioId { get; set; }
        public Servizio Servizio { get; set; } = null!;

        public int? TenantId { get; set; }
        public Tenant? Tenant { get; set;} = null!;

    }
}
