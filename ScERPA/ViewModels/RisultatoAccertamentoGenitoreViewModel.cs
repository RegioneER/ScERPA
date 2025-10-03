using ScERPA.Models.Dictionaries;
using ScERPA.Models.Enums.ANPR;
using ScERPA.Services.Interfaces;
using NuGet.Protocol.Plugins;
using ScERPA.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace ScERPA.ViewModels
{
    public class RisultatoAccertamentoGenitoreViewModel : IElementoSchedaReport
    {
        public AnprGeneralitaViewModel Generalita { get; set; } = new();
        public GenitoreViewModel Genitore { get; set; } = new();
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
            dati.Add("idANPR", idANPR);
            dati.Add("Id Interrogazione Anpr", this.IdInterrogazioneAnpr);
            dati.Add("Id Interrogazione Rer", this.IdInterrogazioneRer);
            dati.Add("Nota", this.Errore);

            return dati;
        }

        public List<Dictionary<string, string>> GetRelatedData()
        {
            List<Dictionary<string, string>> DatiRelazionati = new();

            Dictionary<string, string> dato = new();
            dato.Add("Codice Fiscale Controllato", Generalita.CodiceFiscale);
            dato.Add("Codice Fiscale genitore", Genitore.Generalita.CodiceFiscale);
            dato.Add("Codice Fiscale verificato", Genitore.Generalita.CodiceFiscaleVerificato);
            dato.Add("Cognome", Genitore.Generalita.Cognome);
            dato.Add("Nome", Genitore.Generalita.Nome);
            dato.Add("Data di nascita", Genitore.Generalita.DataNascita);
            dato.Add("Sesso", Genitore.Generalita.Sesso);
            dato.Add("Soggetto AIRE", Genitore.Generalita.SoggettoAIRE);
            dato.Add("Anno espatrio", Genitore.Generalita.AnnoEspatrio);
            dato.Add("Nazione", Genitore.Generalita.Nazione);
            dato.Add("Codice Nazione", Genitore.Generalita.CodNazione);
            dato.Add("Provincia", Genitore.Generalita.Provincia);
            dato.Add("Localita", Genitore.Generalita.Localita);
            dato.Add("Codice Istat Comune", Genitore.Generalita.CodIstatComune);
            dato.Add("Luogo eccezionale", Genitore.Generalita.LuogoEccezionale);
            dato.Add("Stato civile", Genitore.StatoCivile);
            dato.Add("Stato civile non disponibile", Genitore.StatoCivileNonDisponibile);
            dato.Add("Senza cognome", Genitore.Generalita.SenzaCognome);
            dato.Add("Senza nome", Genitore.Generalita.SenzaNome);
            dato.Add("Senza giorno nascita", Genitore.Generalita.SenzaGiornoNascita);
            dato.Add("Senza giorno e mese nascita", Genitore.Generalita.SenzaGiornoMeseNascita);
            dato.Add("idANPR", Genitore.idANPR);

            DatiRelazionati.Add(dato);

            return DatiRelazionati;

        }
    }
}
