using ScERPA.Models.Dictionaries;
using ScERPA.Models.Enums.ANPR;
using ScERPA.Services.Interfaces;
using NuGet.Protocol.Plugins;
using ScERPA.Extensions;


namespace ScERPA.ViewModels
{
    public class RisultatoAccertamentoEsistenzaVitaViewModel : IElementoSchedaReport
    {
        public AnprGeneralitaViewModel Generalita { get; set; } = new();
      

        private bool _inVita=false;

        public string InVita
        {
            get
            {
                return _inVita ? "Si" : "No";
            }
            set
            {
                _inVita = value == "S";
            }
        }

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
            dati.Add("In vita", InVita);
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
