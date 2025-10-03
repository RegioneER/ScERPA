using ScERPA.Models;
using System.ComponentModel.DataAnnotations;

namespace ScERPA.Areas.Administration.ViewModels
{
    public class TenantDetailsViewModel
    {
        public int Id { get; set; }

        [MaxLength(250)]
        public string Nome { get; set; } = string.Empty;

        public List<Configurazione> Configurazioni { get; set; } = new List<Configurazione>();
        public string Parent { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;        
        public string Configurazione { get; set; } = string.Empty;

    }
}
