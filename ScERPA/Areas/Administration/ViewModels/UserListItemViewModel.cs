using System.Drawing.Printing;

namespace ScERPA.Areas.Administration.ViewModels
{
    public class UserListItemViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string CognomeNome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Attivo { get; set; } 
        public List<string> Ruoli { get; set; } = new();

    }
}
