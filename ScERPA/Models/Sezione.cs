using System.ComponentModel.DataAnnotations;

namespace ScERPA.Models
{
    public class Sezione
    {
        public int Id { get; set; }
        [MaxLength(250), Required(ErrorMessage = "Campo obbligatorio")]
        public string Nome { get; set; } = string.Empty;
        
        public int AreaId { get; set; }
        public int Ordinale { get; set; } = 0;
        public Area Area { get; set; } = null!;
    }
  
}
