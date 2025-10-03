using Microsoft.AspNetCore.Mvc.Rendering;
using ScERPA.Areas.Administration.Models;

namespace ScERPA.Areas.Administration.ViewModels
{
    public class UserAminEditProfileViewModel
    {

        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Cognome { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RuoloAmministratore { get; set; } = string.Empty;

        public UserProfileEditInputModel Input { get; set; } = new();
       
    }
}
