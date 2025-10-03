using ScERPA.Models.Dictionaries;
using ScERPA.Services.Interfaces;

namespace ScERPA.ViewModels
{
    public class RisultatoVerificaCittadinanzaViewModel: IElementoSchedaReport
    {
        public string CodiceFiscale { get; set; } = "";
        public List<TipoCittadinanza> Cittadinanze { get; set; } = new();
        public bool CittadinanzaItaliana { get; set; } = false;
        public string IdInterrogazioneAnpr { get; set; } = "";
        public string IdInterrogazioneRer { get; set; } = "";
        public string Errore { get; set; } = "";

        public Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dati = new();
            dati.Add("Codice Fiscale", this.CodiceFiscale);
            dati.Add("Cittadinanza italiana", this.CittadinanzaItaliana ? "Si":"No");
            dati.Add("Id Interrogazione Anpr", this.IdInterrogazioneAnpr);
            dati.Add("Id Interrogazione Rer", this.IdInterrogazioneRer);
            dati.Add("Nota", this.Errore);

            return dati;
        }

        public List<Dictionary<string, string>> GetRelatedData()
        {
            return new List<Dictionary<string, string>>();
        }
    }
}
