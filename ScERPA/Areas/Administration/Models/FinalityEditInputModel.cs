using ScERPA.Data;
using ScERPA.Models;
using ScERPA.Models.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace ScERPA.Areas.Administration.Models
{
    public class FinalityEditInputModel : IValidatableObject
    {
        private ScERPAContext? _repository;


        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo obbligatorio")]
        public AmbientiEnum Ambiente { get; set; } = AmbientiEnum.Test;

        [MaxLength(500), Required(AllowEmptyStrings = false, ErrorMessage = "Campo obbligatorio")]
        public string Nome { get; set; } = string.Empty;
        [MaxLength(1000), Required(AllowEmptyStrings = false, ErrorMessage = "Campo obbligatorio")]
        public string Descrizione { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "E' obbligatorio inserire un valore")]
        public string DataDal { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");
        public string? DataAl { get; set; }

        [Range(1, Int32.MaxValue,ErrorMessage ="Campo obbligatorio e deve essere maggiore di 0")]
        public int MaxChiamate { get; set; } = 0;

        public UnitaTempoChiamateEnum UnitaTempoChiamate { get; set; } = UnitaTempoChiamateEnum.Giorno;

        public int IntervalloTempoChiamate { get; set; } = 1;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo obbligatorio")]
        public string AreaId { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo obbligatorio")]        
        public string ServizioId { get; set; } = string.Empty;

        public int TenantId { get; set; }

        [RegularExpression(@"^$|^[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-4[0-9a-fA-F]{3}\-(8|9|a|b)[0-9a-fA-F]{3}\-[0-9a-fA-F]{12}$", ErrorMessage = "Formato non valido")]
        public string? PurpouseCod { get; set; } = string.Empty;


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            DateOnly dataDal;
            DateOnly dataAl;
            bool convertitaDataDal;
            bool convertitaDataAl;
            int servizioId;

            convertitaDataDal = DateOnly.TryParse(DataDal, out dataDal);
            convertitaDataAl = DateOnly.TryParse(DataAl, out dataAl);

            _repository =  validationContext.GetService<ScERPAContext>();

            if (!convertitaDataDal)
            {
                yield return new ValidationResult(
                   $"La Data Dal deve essere una data valida",
                    new[] { nameof(DataDal) });
            } else if(this.Id == 0 && dataDal < DateOnly.FromDateTime(DateTime.Now))
            {
                yield return new ValidationResult(
                  $"La Data Dal deve essere una data maggiore o uguale ad oggi",
                   new[] { nameof(DataDal) });
            }

            if(!string.IsNullOrEmpty(DataAl) && !convertitaDataAl)
            {
                yield return new ValidationResult(
                   $"La Data Al deve essere una data valida",
                    new[] { nameof(DataAl) });
            } else if(convertitaDataDal && convertitaDataAl && dataAl <= dataDal)
            {
                yield return new ValidationResult(
                   $"La Data Al deve essere maggiore di Data Dal",
                    new[] { nameof(DataAl) });
            }



            if(!int.TryParse(ServizioId,out servizioId))
            {
                yield return new ValidationResult(
                    $"Deve essere associato a un servizio valido",
                    new[] { nameof(ServizioId) });
            } else if(_repository is not null && !(_repository.Servizi.Where(x=>x.Id == servizioId).Any()))
            {
                yield return new ValidationResult(
                    $"Servizio inesistente",
                    new[] { nameof(ServizioId) });
            }

            if(_repository is not null && _repository.Purpouses.Where(x=>x.Valore==this.PurpouseCod && x.FinalitaId != Id).Any())
            {
                yield return new ValidationResult(
                    $"Non è possibile inserire un Purpouse ID già presente", 
                    new[] { nameof(PurpouseCod) });
            }


        }
    }
}
