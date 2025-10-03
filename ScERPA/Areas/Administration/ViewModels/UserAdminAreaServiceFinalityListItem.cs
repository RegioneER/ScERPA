using Microsoft.Build.ObjectModelRemoting;

namespace ScERPA.Areas.Administration.ViewModels
{
    public class UserAdminAreaServiceFinalityListItem
    {
        public int Id { get; set; }
        public string Finalita { get; set; } = string.Empty;
        public int ServizioId { get; set; } 
        public string Servizio { get; set; } = string.Empty;
        public int AreaId {  get; set; }
        public string Area { get; set; } = string.Empty;

    }
}
