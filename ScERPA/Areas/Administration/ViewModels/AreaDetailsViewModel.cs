using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;

namespace ScERPA.Areas.Administration.ViewModels
{
    public class AreaDetailsViewModel
    {

        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public int Ordinale { get; set; } = 0;

        public string Sezioni { get; set; } = ""; 

    }
}
