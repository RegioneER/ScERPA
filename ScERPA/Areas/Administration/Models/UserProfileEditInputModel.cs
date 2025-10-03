using ScERPA.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Org.BouncyCastle.Bcpg;
using System.ComponentModel.DataAnnotations;

namespace ScERPA.Areas.Administration.Models
{
    public class UserProfileEditInputModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo obbligatorio")]
        public string UserId { get; set; } = "";
        public bool Attivo { get; set; }
        public bool TenantCorrenteAttivo { get; set; }
        public bool isSuperAdmin { get; set; } = false;
        public bool isAdmin { get; set; } = false;
        public bool isManager { get; set; } = false;
        public bool isConsumer { get; set; } = false;

    }
}
