using ScERPA.Models.Enums;
using ScERPA.Services;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ScERPA.Models
{
    public class Chiamata
    {
        public int Id { get; set; }

        public AmbientiEnum Ambiente { get; set; } = AmbientiEnum.Test;

        public int TenantId { get; set; }

        [DeleteBehavior(DeleteBehavior.Restrict)]
        public Tenant Tenant { get; set; } = null!;

        public int FinalitaId { get; set; }

        [DeleteBehavior(DeleteBehavior.Restrict)]
        public Finalita Finalita { get; set; } = null!;

        public int PurpouseID { get; set; }

        [DeleteBehavior(DeleteBehavior.Restrict)]
        public Purpouse Purpouse { get; set; } = null!;

        public string OperationGUID { get; set; } = "";

        public string UserID { get; set; } = "";

        public DateTime TimestampCreazioneRichiesta { get; set; }

        public DateTime? TimestampInvocazioneRichiesta { get; set; }

        public DateTime? TimestampRispostaRichiesta { get; set; }

        public ApiResultStatus? CodiceRisposta { get; set; } 
       
    }
}
