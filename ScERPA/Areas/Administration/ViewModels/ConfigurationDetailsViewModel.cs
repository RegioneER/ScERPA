using ScERPA.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ScERPA.Areas.Administration.ViewModels
{
    public class ConfigurationDetailsViewModel
    {
        public int Id { get; set; }

        public string Nome { get; set; } = String.Empty;
        public AmbientiEnum Ambiente { get; set; } = AmbientiEnum.Test;
        [Display(Name = "Client ID")]
        public string? ApiManagerClientId { get; set; }
        [Display(Name = "Client Secret")]
        public string? ApiManagerClientSecret { get; set; }
        [Display(Name = "Indirizzo servizio token")]
        public string? ApiManagerOauthEndpoint { get; set; }
    }
}
