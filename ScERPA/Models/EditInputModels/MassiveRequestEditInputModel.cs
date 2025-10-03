using ScERPA.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ScERPA.Models.EditInputModels
{
    public class MassiveRequestEditInputModel
    {
        public int FinalitaId { get; set; }                 
        public string UserID { get; set; } = "";
        public AmbientiEnum Ambiente { get; set; } = AmbientiEnum.Test;
        public int TenantId { get; set; }
    }
}
