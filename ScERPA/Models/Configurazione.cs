using ScERPA.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ScERPA.Models
{
    public class Configurazione
    {
        public int Id { get; set; }
        [MaxLength(500), Required(ErrorMessage = "Campo obbligatorio")]
        public string Nome { get; set; } = String.Empty;
        public AmbientiEnum Ambiente { get; set; } = AmbientiEnum.Test;
        [MaxLength(500)]
        [Display(Name ="Client ID")]
        public string? ApiManagerClientId { get; set; }
        [MaxLength(500)]
        [Display(Name = "Client Secret")]
        public string? ApiManagerClientSecret { get; set; }

        [MaxLength(5000)]
        [Display(Name = "Indirizzo servizio token")]
        public string? ApiManagerOauthEndpoint { get; set; }
        public List<Tenant>? ListaTenant { get; set; } = new();
    }
}
