using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ScERPA.Models
{

    public class Purpouse
    {
        public int Id { get; set; }
        [MaxLength(500), Required(ErrorMessage = "Campo obbligatorio")]
        public string Valore { get; set; } = "";
        [Required(ErrorMessage = "Campo obbligatorio")]
        public DateTime DataDal { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0, DateTimeKind.Local);

        public DateTime? DataAl { get; set; }

        public int FinalitaId { get; set; }
        public Finalita Finalita { get; set; } = null!;

    }
}

