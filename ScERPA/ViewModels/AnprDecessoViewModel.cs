using ScERPA.Models.Dictionaries;
using ScERPA.Models.Enums.ANPR;
using ScERPA.Services.Interfaces;
using NuGet.Protocol.Plugins;
using ScERPA.Extensions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Globalization;


namespace ScERPA.ViewModels
{
    public class AnprDecessoViewModel
    {
        private DateOnly? _dataDecesso;
        private DateOnly? _dataFormazioneAtto;
        private bool _deceduto;


        public string Deceduto
        {
            get
            {
                return _deceduto ? "Si" : "No";
            }
            set
            {
                _deceduto = (value == "S");
            }

        } 

        public string DataDecesso
        {
            get
            {
                return _dataDecesso?.ToString("d") ?? "";
            }
            set
            {
                DateOnly tempdata;
                _dataDecesso = DateOnly.TryParse(value, CultureInfo.InvariantCulture, out tempdata) ? tempdata : null;
            }
        }


        public string DataFormazioneAtto
        {
            get
            {
                return _dataFormazioneAtto?.ToString("d") ?? "";
            }
            set
            {
                DateOnly tempdata;
                _dataFormazioneAtto = DateOnly.TryParse(value, CultureInfo.InvariantCulture, out tempdata) ? tempdata : null;
            }
        }
            
        public string NumeroAtto { get; set; } = "";
        public string ParteAtto { get; set; } = "";
        public string SerieAtto { get; set; } = "";
        public string VolumeAtto { get; set; } = "";
        public string AnnoAtto { get; set; } = "";
        public string UfficioMunicipioAtto { get; set; } = "";
        public string TrascrizioneAtto { get; set; } = "";
        public string NomeComuneAtto { get; set; } = "";

        public string CodIstatComuneAtto { get; set; } = "";

        public string ProvinciaComuneAtto { get; set; } = "";

        public string LocalitaComuneAtto { get; set; } = "";

        public string Nazione { get; set; } = "";

        public string CodNazione { get; set; } = "";

        public string Provincia { get; set; } = "";

        public string Localita { get; set; } = "";

        public string CodIstatComune { get; set; } = "";

        public string LuogoEccezionale { get; set; } = ""; 
        


    }
}
