using ScERPA.Models.Dictionaries;
using ScERPA.Models.Enums.ANPR;
using ScERPA.Services.Interfaces;
using NuGet.Protocol.Plugins;
using ScERPA.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace ScERPA.ViewModels
{
    public class RisultatoAccertamentoGeneralitaViewModel: IElementoSchedaReport
    {

        public AnprGeneralitaViewModel Generalita { get; set; } = new();

        private Models.Enums.ANPR.TipologiaDatoStatoCivile _tipoStatoCivile;

        private bool _statoCivileNonDisponibile;

        public string StatoCivileNonDisponibile
        {
            get
            {
                return _statoCivileNonDisponibile ? "Si" : "No";
            }
            set
            {
                _statoCivileNonDisponibile = value == "S";
            }
        }


        public string StatoCivile
        {
            get
            {
                return _tipoStatoCivile.GetDisplayName();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _tipoStatoCivile = Models.Enums.ANPR.TipologiaDatoStatoCivile.Non_classificabile;
                }
                else
                {
                    int codiceTipoStatoCivile;
                    int.TryParse(value, out codiceTipoStatoCivile);
                    _tipoStatoCivile = int.TryParse(value, out codiceTipoStatoCivile) ? (Models.Enums.ANPR.TipologiaDatoStatoCivile)codiceTipoStatoCivile : Models.Enums.ANPR.TipologiaDatoStatoCivile.Non_classificabile;
                }

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
            dati.Add("Cognome",Generalita.Cognome);
            dati.Add("Nome",Generalita.Nome);
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
            dati.Add("Stato civile", StatoCivile);
            dati.Add("Stato civile non disponibile", StatoCivileNonDisponibile);
            dati.Add("idANPR", idANPR);
            dati.Add("Id Interrogazione Anpr", this.IdInterrogazioneAnpr);
            dati.Add("Id Interrogazione Rer", this.IdInterrogazioneRer);
            dati.Add("Nota", this.Errore);

            return dati;
        }

        public List<Dictionary<string, string>> GetRelatedData()
        {
            return new List<Dictionary<string,string>>();
        }
    }
}
