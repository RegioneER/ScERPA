using ScERPA.Data;
using ScERPA.Models;
using ScERPA.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;


namespace ScERPA.Areas.Administration.Models
{
    public class ConfigurationEditInputModel : IValidatableObject
    {
        private IConfiguration? _configuration;

        public int Id { get; set; }

        [MaxLength(500), Required(AllowEmptyStrings = false, ErrorMessage = "Campo obbligatorio")]
        public string Nome { get; set; } = String.Empty;
        public AmbientiEnum Ambiente { get; set; } = AmbientiEnum.Test;

        [MaxLength(500), Required(AllowEmptyStrings = false, ErrorMessage = "Campo obbligatorio")]
        [Display(Name = "Client Id")]
        public string ApiManagerClientId { get; set; } = string.Empty;

        [MaxLength(500), Required(AllowEmptyStrings = false, ErrorMessage = "Campo obbligatorio")]
        [Display(Name = "Client Secret")]
        public string ApiManagerClientSecret { get; set; } = string.Empty;


        [MaxLength(5000), Required(AllowEmptyStrings = false, ErrorMessage = "Campo obbligatorio, inserire un url valido")]
        [Display(Name = "Indirizzo servizio token")]
        public string ApiManagerOauthEndpoint { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            _configuration =  validationContext.GetService<IConfiguration>();

            Uri? ValidateUri;
            string[] whitelist = _configuration?.GetSection("WSO2:urlWhiteList").Get<string[]>() ?? Array.Empty<string>();

            if (!Uri.TryCreate(ApiManagerOauthEndpoint, UriKind.Absolute, out ValidateUri))
            {
                yield return new ValidationResult(
                  $"Campo obbligatorio, inserire un url valido",
                   new[] { nameof(ApiManagerOauthEndpoint) });
            } else
            {
                string domain = ValidateUri.GetLeftPart(UriPartial.Authority).ToString().ToLower();
                if(!whitelist.Contains(domain))
                {
                    yield return new ValidationResult(
                    $"Campo obbligatorio, inserire un url valido",
                    new[] { nameof(ApiManagerOauthEndpoint) });
                }

            }

          
         
        }
    }
}
