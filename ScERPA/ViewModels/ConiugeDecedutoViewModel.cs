using ScERPA.Models.Dictionaries;
using ScERPA.Models.Enums.ANPR;
using ScERPA.Services.Interfaces;
using ScERPA.Extensions;
using Org.BouncyCastle.Bcpg;
using System.Globalization;

namespace ScERPA.ViewModels
{
    public class ConiugeDecedutoViewModel
    {

        public AnprGeneralitaViewModel Generalita { get; set; } = new();

        public AnprDecessoViewModel Decesso { get; set; } = new();

    }
}
