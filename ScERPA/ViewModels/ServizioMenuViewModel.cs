using System.Drawing.Printing;

namespace ScERPA.ViewModels
{
    public class ServizioMenuViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descrizione { get; set; } = string.Empty;
        public string Sezione { get; set; } = string.Empty;

        public int Ordinale { get; set; }

    }
}
