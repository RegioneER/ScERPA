using ScERPA.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ScERPA.Areas.Administration.Models
{
    public class UserFinalitiesEditInputModel
    {
        
        [Required(AllowEmptyStrings = false,ErrorMessage = "Campo obbligatorio")]
        public string UserId { get; set; } =String.Empty;
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo obbligatorio")]
        public string AreaId { get; set; } = String.Empty;
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo obbligatorio")]
        public string ServizioId { get; set; } = String.Empty;
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo obbligatorio")]
        public string FinalitaId { get; set; } = String.Empty;


    }
}
