using ScERPA.Models.Dictionaries;
using ScERPA.Models.Enums.ANPR;
using ScERPA.Services.Interfaces;
using NuGet.Protocol.Plugins;
using ScERPA.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Globalization;


namespace ScERPA.ViewModels
{
    public class AnprGeneralitaViewModel 
    {
        private DateOnly? _dataNascita;
        private bool _codiceFiscaleVerificato;
        private bool _senzaNome;
        private bool _senzaCognome;
        private bool _senzaGiornoNascita;
        private bool _senzaGiornoMeseNascita;
        private bool _soggettoAIRE;

        public string CodiceFiscale { get; set; } = "";

        public string CodiceFiscaleVerificato
        {
            get
            {
                return _codiceFiscaleVerificato ? "Si" : "No";
            }
            set
            {
                _codiceFiscaleVerificato = (value == "1");
            }
        }

        public string SoggettoAIRE
        {
            get
            {
                return _soggettoAIRE ? "Si" : "No";
            }
            set
            {
                _soggettoAIRE = (value == "S");
            }
        }

        public string SenzaNome
        {
            get
            {
                return _senzaNome ? "Si" : "No";
            }
            set
            {
                _senzaNome = (value == "1");
            }
        }

        public string SenzaCognome
        {
            get
            {
                return _senzaCognome ? "Si" : "No";
            }
            set
            {
                _senzaCognome = (value == "1");
            }
        }

        public string SenzaGiornoNascita
        {
            get
            {
                return _senzaGiornoNascita ? "Si" : "No";
            }
            set
            {
                _senzaGiornoNascita = (value == "1");
            }
        }

        public string SenzaGiornoMeseNascita
        {
            get
            {
                return _senzaGiornoMeseNascita ? "Si" : "No";
            }
            set
            {
                _senzaGiornoMeseNascita = (value == "1");
            }
        }

        public string AnnoEspatrio { get; set; } = "";

        public string Cognome { get; set; } = "";

        public string Nome { get; set; } = "";
        public string DataNascita
        {
            get
            {
                return _dataNascita?.ToString("d") ?? "";
            }
            set
            {
                DateOnly tempdata;                
                _dataNascita = DateOnly.TryParse(value, CultureInfo.InvariantCulture, out tempdata) ? tempdata : null;
            }
        }

        public string Sesso { get; set; } = "";
        public string Nazione { get; set; } = "";

        public string CodNazione { get; set; } = "";

        public string Provincia { get; set; } = "";

        public string Localita { get; set; } = "";

        public string CodIstatComune { get; set; } = "";

        public string LuogoEccezionale { get; set; } = "";      

    }
}
