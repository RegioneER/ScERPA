using ScERPA.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ScERPA.Areas.Administration.Models
{
    public class TenantEditInputModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo obbligatorio")]
        public int Id { get; set; }

        [MaxLength(250), Required(AllowEmptyStrings = false, ErrorMessage = "Campo obbligatorio")]
        public string Nome { get; set; } = string.Empty;
        
        [MaxLength(1000), Required(AllowEmptyStrings = false, ErrorMessage = "Campo obbligatorio")]
        public string Parent { get; set; } = string.Empty;
        
        [MaxLength(1000), Required(AllowEmptyStrings = false, ErrorMessage = "Campo obbligatorio")]
        public string Url { get; set; } = string.Empty;
        
        [MaxLength(1000), Required(AllowEmptyStrings = false, ErrorMessage = "Campo obbligatorio")]
        public string Logo { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo obbligatorio")]
        public string ConfigurationId { get; set; } = string.Empty;

    }
}
