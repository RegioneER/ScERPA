using System.ComponentModel.DataAnnotations;

namespace ScERPA.Areas.Administration.ViewModels
{
    public class FinalitiesListItemViewModel
    {

        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Area { get; set; } = string.Empty;

        public int AreaId { get; set; }

        public string Servizio { get; set; } = string.Empty;

        public int ServizioId { get; set; }

        public string Attivo { get; set; } = string.Empty;

        public string DataDal { get; set; } = string.Empty;

        public string DataAl { get; set; } = string.Empty;


    }
}
