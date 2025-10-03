namespace ScERPA.Areas.Administration.ViewModels
{
    public class UserAdminUserProfileViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Cognome { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Attivo { get; set; }
        public List<string> Ruoli { get; set; } = new();
        public List<string> Tenants { get; set; } = new();

    }
}
