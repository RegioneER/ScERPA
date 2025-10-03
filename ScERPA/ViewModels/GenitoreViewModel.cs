using ScERPA.Models.Dictionaries;
using ScERPA.Models.Enums.ANPR;
using ScERPA.Services.Interfaces;
using ScERPA.Extensions;
using Org.BouncyCastle.Bcpg;
using System.Globalization;

namespace ScERPA.ViewModels
{
    public class GenitoreViewModel
    {
        private TipologiaDatoStatoCivile _tipoStatoCivile;

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
                    _tipoStatoCivile = TipologiaDatoStatoCivile.Non_classificabile;
                }
                else
                {
                    int codiceTipoStatoCivile;
                    int.TryParse(value, out codiceTipoStatoCivile);
                    _tipoStatoCivile = int.TryParse(value, out codiceTipoStatoCivile) ? (TipologiaDatoStatoCivile)codiceTipoStatoCivile :TipologiaDatoStatoCivile.Non_classificabile;
                }

            }
        }
        public AnprGeneralitaViewModel Generalita { get; set; } = new();

        public string idANPR { get; set; } = "";

    }
}
