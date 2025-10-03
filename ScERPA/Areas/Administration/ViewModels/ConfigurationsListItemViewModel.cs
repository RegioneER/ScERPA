using ScERPA.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ScERPA.Areas.Administration.ViewModels
{
    public class ConfigurationsListItemViewModel
    {
        public int Id { get; set; }
        [MaxLength(500), Required(ErrorMessage = "Campo obbligatorio")]
        public string Nome { get; set; } = String.Empty;
        public AmbientiEnum Ambiente { get; set; } = AmbientiEnum.Test;
    }
}
