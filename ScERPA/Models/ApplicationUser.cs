using Azure;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;

namespace ScERPA.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        [MaxLength(250), Required(ErrorMessage = "Campo obbligatorio")]
        public string Nome { get; set; } = "";
        [PersonalData]
        [MaxLength(250), Required(ErrorMessage = "Campo obbligatorio")]
        public string Cognome { get; set; } = "";
        [PersonalData]
        [MaxLength(20)]
        public string? Matricola { get; set; } = "";
        [PersonalData]
        [MaxLength(16)]
        public string? CodiceFiscale { get; set; } = "";
        public bool Attivo { get; set; } = false;
        public List<Finalita>? ListaFinalita { get; set; } = new();

        public List<Tenant>? Tenants { get; set; } = new();

    }

}
