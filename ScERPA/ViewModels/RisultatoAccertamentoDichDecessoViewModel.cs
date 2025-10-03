using ScERPA.Models.Dictionaries;
using ScERPA.Models.Enums.ANPR;
using ScERPA.Services.Interfaces;
using NuGet.Protocol.Plugins;
using ScERPA.Extensions;
using System.Globalization;


namespace ScERPA.ViewModels
{
    public class RisultatoAccertamentoDichDecessoViewModel : IElementoSchedaReport
    {
        public AnprGeneralitaViewModel Generalita { get; set; } = new();
        public AnprDecessoViewModel Decesso { get; set; } = new();

        private TipologiaMotivoCancellazioneNoAire _motiviDiCancellazioneNoAire;
        private TipologiaMotivoCancellazioneAire _motiviDiCancellazioneAire;
        private DateOnly? _dataCancellazione;


        public string MotivoDiCancellazioneNoAire
        {
            get
            {
                return _motiviDiCancellazioneNoAire.GetDisplayName();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _motiviDiCancellazioneNoAire = TipologiaMotivoCancellazioneNoAire.Altro;
                }
                else
                {
                    int codiceMotivoDiCancellazioneNoAire;
                    _motiviDiCancellazioneNoAire = int.TryParse(value, out codiceMotivoDiCancellazioneNoAire) ? (TipologiaMotivoCancellazioneNoAire)codiceMotivoDiCancellazioneNoAire : TipologiaMotivoCancellazioneNoAire.Altro;
                }

            }
        }

        public string MotivoDiCancellazioneAire
        {
            get
            {
                return _motiviDiCancellazioneAire.GetDisplayName();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _motiviDiCancellazioneAire = TipologiaMotivoCancellazioneAire.Altro;
                }
                else
                {
                    int codiceMotivoDiCancellazioneAire;
                    _motiviDiCancellazioneAire = int.TryParse(value, out codiceMotivoDiCancellazioneAire) ? (TipologiaMotivoCancellazioneAire)codiceMotivoDiCancellazioneAire : TipologiaMotivoCancellazioneAire.Altro;
                }

            }
        }


        public string MotivoDiCancellazione
        {
            get
            {
                if (this.Generalita.SoggettoAIRE.ToUpper() == "SI")
                {
                    return _motiviDiCancellazioneAire.GetDisplayName();
                }
                else if (this.Generalita.SoggettoAIRE.ToUpper() == "NO")
                {
                    return _motiviDiCancellazioneNoAire.GetDisplayName();
                }
                else
                    return "";                
            }

        }

        public string NoteCancellazione { get; set; } = "";

        public string DataCancellazione
        {
            get
            {
                return _dataCancellazione?.ToString("d") ?? "";
            }
            set
            {
                DateOnly tempdata;
                _dataCancellazione = DateOnly.TryParse(value, CultureInfo.InvariantCulture, out tempdata) ? tempdata : null;
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
            dati.Add("Motivo di cancellazione", MotivoDiCancellazione);
            dati.Add("Data di cancellazione", DataCancellazione);
            dati.Add("note di cancellazione", NoteCancellazione);
            dati.Add("Deceduto", Decesso.Deceduto);
            dati.Add("Data decesso", Decesso.DataDecesso);
            dati.Add("Anno atto decesso", Decesso.AnnoAtto);
            dati.Add("Data formazione atto decesso", Decesso.DataFormazioneAtto);
            dati.Add("Nome del comune atto decesso", Decesso.NomeComuneAtto);
            dati.Add("Codice istat del comune atto decesso", Decesso.CodIstatComuneAtto);
            dati.Add("Provincia del comune atto decesso", Decesso.ProvinciaComuneAtto);
            dati.Add("Località atto decesso", Decesso.LocalitaComuneAtto);
            dati.Add("Numero atto decesso", Decesso.NumeroAtto);
            dati.Add("Parte atto decesso", Decesso.ParteAtto);
            dati.Add("Serie atto decesso", Decesso.SerieAtto);
            dati.Add("Volume atto decesso", Decesso.VolumeAtto);
            dati.Add("Ufficio municipio atto decesso", Decesso.UfficioMunicipioAtto);
            dati.Add("Trascrizione atto decesso", Decesso.TrascrizioneAtto);
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
