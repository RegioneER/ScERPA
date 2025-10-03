using ScERPA.Areas.Administration.ViewModels;
using ScERPA.Models;


namespace ScERPA.Services.Interfaces
{
    public interface IApiServices
    {
        public Task<Finalita> GetFinalityFromPurpouseIDAsync(string purpouseId);

    }
}
