using ScERPA.Models.Dictionaries;
using ScERPA.Models.Enums.ANPR;
using ScERPA.Services.Interfaces;
using NuGet.Protocol.Plugins;
using ScERPA.Extensions;


namespace ScERPA.ViewModels
{
    public class RisultatoAccertamentoResidenzaViewModel: IElementoSchedaReport
    {
        private DateOnly? _decorrenza;
        private Models.Enums.ANPR.TipologiaIndirizzo _tipoIndirizzo;
        private bool _residenzaAIRE;


        public AnprGeneralitaViewModel Generalita { get; set; } = new();

        public string DecorrenzaResidenza {
            get {
                return  _decorrenza?.ToString("d") +"";
            }
            set
            {
                DateOnly tempdata;
                DateOnly.TryParse(value, out tempdata);
                _decorrenza = tempdata.Year == 1 && tempdata.Month == 1 && tempdata.Day == 1 ? null : tempdata;
            }
        }

        public string TipoIndirizzo
        {
            get
            {
                return _tipoIndirizzo.GetDisplayName();
            }
            set
            {
                int codiceTipoIndirizzo;
                int.TryParse(value,out codiceTipoIndirizzo);
                _tipoIndirizzo = (Models.Enums.ANPR.TipologiaIndirizzo)codiceTipoIndirizzo;
            }
        }


        public string ResidenzaAIRE
        {
            get
            {
                return _residenzaAIRE ? "Si" : "No";
            }
            set
            {
                _residenzaAIRE = value == "4";
            }
        }
        public string Nazione { get; set; } = "";

        public string CodNazione { get; set; } = "";

        public string Provincia { get; set; } = "";

        public string Localita { get; set; } = "";

        public string CodIstatComune { get; set; } = "";

        public string CAP { get; set; } = "";

        public string Frazione { get; set; } = "";

        public string Indirizzo { get; set; } = "";

        public string Civico { get; set; } = "";

        public string Presso { get; set; } = "";

        public string idANPR { get; set; } = "";

        public string IdInterrogazioneAnpr { get; set; } = "";
        public string IdInterrogazioneRer { get; set; } = "";
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
            dati.Add("Nazione Nascita", Generalita.Nazione);
            dati.Add("Codice Nazione Nascita", Generalita.CodNazione);
            dati.Add("Provincia Nascita", Generalita.Provincia);
            dati.Add("Localita Nascita", Generalita.Localita);
            dati.Add("Codice Istat Comune Nascita", Generalita.CodIstatComune);
            dati.Add("Luogo di nascita eccezionale", Generalita.LuogoEccezionale);
            dati.Add("Senza cognome", Generalita.SenzaCognome);
            dati.Add("Senza nome", Generalita.SenzaNome);
            dati.Add("Senza giorno nascita", Generalita.SenzaGiornoNascita);
            dati.Add("Senza giorno e mese nascita", Generalita.SenzaGiornoMeseNascita);
            dati.Add("Tipo Indirizzo", this.TipoIndirizzo);
            dati.Add("Residenza AIRE", this.ResidenzaAIRE);
            dati.Add("Decorrenza Residenza", this.DecorrenzaResidenza);
            dati.Add("Nazione Residenza", this.Nazione);
            dati.Add("Codice Nazione Residenza", this.CodNazione);
            dati.Add("Provincia Residenza", this.Provincia);
            dati.Add("Localita Residenza", this.Localita);
            dati.Add("Codice Istat Comune Residenza", this.CodIstatComune);
            dati.Add("CAP Residenza", this.CAP);
            dati.Add("Frazione Residenza", this.Frazione);
            dati.Add("Indirizzo Residenza", this.Indirizzo);
            dati.Add("Civico Residenza", this.Civico);
            dati.Add("idANPR", this.idANPR);
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
