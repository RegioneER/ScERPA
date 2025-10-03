using System.ComponentModel.DataAnnotations;

namespace ScERPA.Areas.Administration.ViewModels
{
    public class AreasListItemViewModel
    {

        public int Id { get; set; }

        [MaxLength(250)]
        public string Nome { get; set; } = string.Empty;



    }
}
