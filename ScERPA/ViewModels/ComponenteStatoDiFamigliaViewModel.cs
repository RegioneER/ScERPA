using ScERPA.Models.Dictionaries;
using ScERPA.Models.Enums.ANPR;
using ScERPA.Services.Interfaces;
using ScERPA.Extensions;
using Org.BouncyCastle.Bcpg;
using System.Globalization;

namespace ScERPA.ViewModels
{
    public class ComponenteStatoDiFamigliaViewModel
    {
  
        private DateOnly? _dataDecorrenzaRelazioneParentela;
        private TipologiaRelazioniParentela _tipoRelazioniParentela;
        private TipologiaLegameConvivenza _tipoLegameConvivenza;
        private TipologiaLegame _tipoLegame;

        public AnprGeneralitaViewModel Generalita { get; set; } = new();


        public int TipoRelazioniParentelaCod
        {
            get
            {
                return (int)_tipoRelazioniParentela;
            }
        }

        public string DescrizioneParentelaConvivenza
        {
            get {

                switch (_tipoLegame)
                {
                    case Models.Enums.ANPR.TipologiaLegame.Convivenza_anagrafica:
                        return this.TipoLegameConvivenza;
                    case Models.Enums.ANPR.TipologiaLegame.Residente_in_Italia:
                        return this.TipoRelazioniParentela;
                    case Models.Enums.ANPR.TipologiaLegame.Aire:
                        return this.TipoRelazioniParentela; 
                    default:
                        return "";
                }


            }
        }
        public string TipoRelazioniParentela
        {
            get
            {
                return _tipoRelazioniParentela.GetDisplayName();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _tipoRelazioniParentela = Models.Enums.ANPR.TipologiaRelazioniParentela.Non_definito_comunicato;
                }
                else
                {
                    int codiceTipoRelazioniParentela;
                    int.TryParse(value, out codiceTipoRelazioniParentela);
                    _tipoRelazioniParentela = (TipologiaRelazioniParentela)codiceTipoRelazioniParentela;
                }

            }
        }

        public string TipoLegameConvivenza
        {
            get
            {
                return _tipoLegameConvivenza.GetDisplayName();
            }
            set
            {
                if(string.IsNullOrEmpty(value))
                {
                    _tipoLegameConvivenza = Models.Enums.ANPR.TipologiaLegameConvivenza.Ignoto;
                } else
                {
                    int codiceTipoLegameConvivenza;
                    int.TryParse(value, out codiceTipoLegameConvivenza);
                    _tipoLegameConvivenza = (TipologiaLegameConvivenza)codiceTipoLegameConvivenza;
                }

            }
        }


        public string TipoLegame
        {
            get
            {
                return _tipoLegame.GetDisplayName();
            }
            set
            {
                int codiceTipoLegame;
                int.TryParse(value, out codiceTipoLegame);
                _tipoLegame = (TipologiaLegame)codiceTipoLegame;
            }
        }

       
        public string DataDecorrenzaRelazioneParentela
        {
            get
            {
                return _dataDecorrenzaRelazioneParentela?.ToString("d") + "";
            }
            set
            {
                DateOnly tempdata;
                
                _dataDecorrenzaRelazioneParentela = DateOnly.TryParse(value, CultureInfo.InvariantCulture, out tempdata) ? tempdata : null ;
            }
        }

        public string idANPR { get; set; } = "";

    }
}
