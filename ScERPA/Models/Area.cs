using System.ComponentModel.DataAnnotations;

namespace ScERPA.Models
{
    public class Area
    {

        public int Id { get; set; }
        [MaxLength(250), Required(ErrorMessage = "Campo obbligatorio")]
        public string Nome { get; set; } = string.Empty;
        public int Ordinale { get; set; } = 0;
        public List<Sezione>? Sezioni { get; set; } = new();

    }
}
