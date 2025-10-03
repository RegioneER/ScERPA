using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ScERPA.Areas.Administration.ViewModels
{
    public class FinalityDetailsViewModel
    {

        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Descrizione { get; set; } = string.Empty;

        public string Area { get; set; } = string.Empty;

        public int AreaId { get; set; }

        public string Servizio { get; set; } = string.Empty;

        public int ServizioId { get; set; }
        [DisplayName("Servizio Attivo")]
        public string Attivo { get; set; } = string.Empty;

        public string DataDal { get; set; } = string.Empty;

        public string DataAl { get; set; } = string.Empty;
        [DisplayName("Chiamate Massime")]
        public string ChiamateMassime { get; set; } = string.Empty;

        [DisplayName("Codice purpouse corrente")]
        public string PurpouseCod { get; set; } = string.Empty;
        [DisplayName("Codice valido dal")]
        public string PurpouseCodDataDal { get; set; } = string.Empty;


    }
}
