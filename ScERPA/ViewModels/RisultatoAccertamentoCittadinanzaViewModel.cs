using ScERPA.Models.Dictionaries;
using ScERPA.Models.Enums.ANPR;
using ScERPA.Services.Interfaces;
using NuGet.Protocol.Plugins;
using ScERPA.Extensions;


namespace ScERPA.ViewModels
{
    public class RisultatoAccertamentoCittadinanzaViewModel : IElementoSchedaReport
    {
        public AnprGeneralitaViewModel Generalita { get; set; } = new();

        public string Cittadinanza { get; set; } = "";

        public string CodCittadinanza { get; set; } = "";

        public string DataCittadinanza { get; set; } = "";

        public string IdInterrogazioneAnpr { get; set; } = "";
        public string IdInterrogazioneRer { get; set; } = "";

        public string idANPR { get; set; } = "";

        public string Errore { get; set; } = "";

        public Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dati = new();
            dati.Add("Codice Fiscale", Generalita.CodiceFiscale);
            dati.Add("Codice fiscale verificato", Generalita.CodiceFiscaleVerificato);
            dati.Add("Cognome", Generalita.Cognome);
            dati.Add("Nome", Generalita.Nome);
            dati.Add("Data di nascita", Generalita.DataNascita);
            dati.Add("Sesso", Generalita.Sesso);
            dati.Add("Soggetto AIRE", Generalita.SoggettoAIRE);
            dati.Add("Anno espatrio", Generalita.AnnoEspatrio);
            dati.Add("Nazione", Generalita.Nazione);
            dati.Add("Codice Nazione", Generalita.CodNazione);
            dati.Add("Provincia", Generalita.Provincia);
            dati.Add("Localita", Generalita.Localita);
            dati.Add("Codice Istat Comune", Generalita.CodIstatComune);
            dati.Add("Luogo di nascita eccezionale", Generalita.LuogoEccezionale);
            dati.Add("Senza cognome", Generalita.SenzaCognome);
            dati.Add("Senza nome", Generalita.SenzaNome);
            dati.Add("Senza giorno nascita", Generalita.SenzaGiornoNascita);
            dati.Add("Senza giorno e mese nascita", Generalita.SenzaGiornoMeseNascita);
            dati.Add("Cittadinanza", Cittadinanza);
            dati.Add("Codice cittadinanza", CodCittadinanza);
            dati.Add("Data cittadinanza", CodCittadinanza);
            dati.Add("idANPR", idANPR);
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
