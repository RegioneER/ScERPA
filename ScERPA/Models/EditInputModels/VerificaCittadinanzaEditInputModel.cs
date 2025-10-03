using NuGet.Protocol.Core.Types;
using System.ComponentModel.DataAnnotations;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace ScERPA.Models.EditInputModels
{
    public class VerificaCittadinanzaEditInputModel : IValidatableObject
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "E' obbligatorio inserire un valore")]
        [MinLength(16, ErrorMessage = "La lunghezzza minima è {1} caratteri")]
        public string CodiciFiscali { get; set; } = "";

        [Required(AllowEmptyStrings = false, ErrorMessage = "E' obbligatorio inserire un valore")]
        public string? DataRiferimentoVerifica { get; set; } ="";

        [Required(AllowEmptyStrings = false, ErrorMessage = "E' obbligatorio selezionare un valore")]
        public string FinalitaId { get; set; } = "";

        public bool Dichiarazione { get; set; } = false;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Dichiarazione!=true)
            {
                yield return new ValidationResult(
                $"Dichiarazione obbligatoria",
                    new[] { nameof(Dichiarazione) });
            }
        }
    }
}
