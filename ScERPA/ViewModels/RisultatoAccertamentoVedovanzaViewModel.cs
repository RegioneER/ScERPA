using ScERPA.Models.Dictionaries;
using ScERPA.Models.Enums.ANPR;
using ScERPA.Services.Interfaces;
using NuGet.Protocol.Plugins;
using ScERPA.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace ScERPA.ViewModels
{
    public class RisultatoAccertamentoVedovanzaViewModel : IElementoSchedaReport
    {
        public AnprGeneralitaViewModel Generalita { get; set; } = new();

        public ConiugeDecedutoViewModel Coniuge { get; set; } = new();

        private Models.Enums.ANPR.TipologiaDatoStatoCivile _tipoStatoCivile;

        private bool _statoCivileNonDisponibile;

        private bool _vedovanza = false;

        public string Vedovanza
        {
            get
            {
                return _vedovanza ? "Si" : "No";
            }
            set
            {
                _vedovanza = value == "S";
            }
        }


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
            List<Dictionary<string, string>> DatiRelazionati = new();

            Dictionary<string, string> dato = new();
            dato.Add("Codice Fiscale Controllato", Generalita.CodiceFiscale);
            dato.Add("Codice Fiscale coniuge", Coniuge.Generalita.CodiceFiscale);
            dato.Add("Codice Fiscale coniuge verificato", Coniuge.Generalita.CodiceFiscaleVerificato);
            dato.Add("Cognome", Coniuge.Generalita.Cognome);
            dato.Add("Nome", Coniuge.Generalita.Nome);
            dato.Add("Data di nascita", Coniuge.Generalita.DataNascita);
            dato.Add("Sesso", Coniuge.Generalita.Sesso);
            dato.Add("Soggetto AIRE", Coniuge.Generalita.SoggettoAIRE);
            dato.Add("Anno espatrio", Coniuge.Generalita.AnnoEspatrio);
            dato.Add("Nazione", Coniuge.Generalita.Nazione);
            dato.Add("Codice Nazione", Coniuge.Generalita.CodNazione);
            dato.Add("Provincia", Coniuge.Generalita.Provincia);
            dato.Add("Localita", Coniuge.Generalita.Localita);
            dato.Add("Codice Istat Comune", Coniuge.Generalita.CodIstatComune);
            dato.Add("Luogo eccezionale", Coniuge.Generalita.LuogoEccezionale);
            dato.Add("Senza cognome", Coniuge.Generalita.SenzaCognome);
            dato.Add("Senza nome", Coniuge.Generalita.SenzaNome);
            dato.Add("Senza giorno nascita", Coniuge.Generalita.SenzaGiornoNascita);
            dato.Add("Senza giorno e mese nascita", Coniuge.Generalita.SenzaGiornoMeseNascita);
            dato.Add("Deceduto", Coniuge.Decesso.Deceduto);
            dato.Add("Data decesso", Coniuge.Decesso.DataDecesso);
            dato.Add("Anno atto decesso", Coniuge.Decesso.AnnoAtto);
            dato.Add("Data formazione atto decesso", Coniuge.Decesso.DataFormazioneAtto);
            dato.Add("Nome del comune atto decesso", Coniuge.Decesso.NomeComuneAtto);
            dato.Add("Codice istat del comune atto decesso", Coniuge.Decesso.CodIstatComuneAtto);
            dato.Add("Provincia del comune atto decesso", Coniuge.Decesso.ProvinciaComuneAtto);
            dato.Add("Località atto decesso", Coniuge.Decesso.LocalitaComuneAtto);
            dato.Add("Numero atto decesso", Coniuge.Decesso.NumeroAtto);
            dato.Add("Parte atto decesso", Coniuge.Decesso.ParteAtto);
            dato.Add("Serie atto decesso", Coniuge.Decesso.SerieAtto);
            dato.Add("Volume atto decesso", Coniuge.Decesso.VolumeAtto);
            dato.Add("Ufficio municipio atto decesso", Coniuge.Decesso.UfficioMunicipioAtto);
            dato.Add("Trascrizione atto decesso", Coniuge.Decesso.TrascrizioneAtto);

            DatiRelazionati.Add(dato);

            return DatiRelazionati;
        }
    }
}
