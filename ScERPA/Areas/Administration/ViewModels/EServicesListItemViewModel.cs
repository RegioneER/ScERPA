using System.ComponentModel.DataAnnotations;

namespace ScERPA.Areas.Administration.ViewModels
{
    public class EServicesListItemViewModel
    {

        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public int AreaId { get; set; } 

        public string Area { get; set; } = string.Empty;

        public string Attivo { get; set; } = string.Empty;
    }
}
