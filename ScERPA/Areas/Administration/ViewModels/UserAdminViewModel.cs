namespace ScERPA.Areas.Administration.ViewModels
{
    public class UserAdminViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string Cognome { get; set; } =string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<UserAdminAreaServiceFinalityListItem> Abilitazioni { get; set; } = new();
    }
}
