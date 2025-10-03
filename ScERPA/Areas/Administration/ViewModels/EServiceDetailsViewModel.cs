using Microsoft.Identity.Client;

namespace ScERPA.Areas.Administration.ViewModels
{
    public class EServiceDetailsViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = "";  
        public string Area { get; set; } = "";
        public string Sezione { get; set; } = "";
        public string Descrizione { get; set; } = "";
        public string Cod { get; set; } = "";
        public string Attivo { get; set; } = "ND";
        public string Indirizzo { get; set; } = "";


    }
}
