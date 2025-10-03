using System.ComponentModel.DataAnnotations;

namespace ScERPA.Models
{
    public class Tenant
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(250),Required(ErrorMessage ="Campo obbligatorio")]
        public string Nome { get; set; } = string.Empty;

        public List<ApplicationUser> ListaUtenti { get; set; } = new List<ApplicationUser>();
        [Display(Name ="Configurazione attuale")]
        public List<Configurazione> Configurazioni { get; set; } = new List<Configurazione>();
        [MaxLength(1000), Required(ErrorMessage = "Campo obbligatorio")]
        public string Parent { get; set; } = string.Empty;
        [MaxLength(1000), Required(ErrorMessage = "Campo obbligatorio")]
        public string Url { get; set; } = string.Empty;
        [MaxLength(1000), Required(ErrorMessage = "Campo obbligatorio")]
        public string Logo { get; set; } = string.Empty;

    }
}
