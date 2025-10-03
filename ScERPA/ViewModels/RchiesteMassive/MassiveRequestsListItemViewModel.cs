using Microsoft.Identity.Client;
using System.Drawing;

namespace ScERPA.ViewModels.RchiesteMassive
{
    public class MassiveRequestsListItemViewModel
    {
        public int Id { get; set; }
        public string Area { get; set; }
        public int AreaId { get; set; }
        public string Servizio { get; set; }
        public int ServizioId { get; set; }
        public string Finalita { get; set; }
        public int FinalitaId { get; set; }
        public int Elementi { get; set; }
        public string DataOraCreazione { get; set; }
        public string DataOraEsecuzione { get; set; }
        public string Stato { get; set; }

    }
}
