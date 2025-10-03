using ScERPA.Models;
using ScERPA.Models.Dictionaries;

namespace ScERPA.Services.Interfaces
{
    public interface IAnprApi 
    {
        public Task<ApiResult<RispostaE002OK>> CallAPIAsync(Chiamata chiamata,string serviceEndpoint, string servicePurpouseID, string payload, string userID, string userLocation, string LoA, string? operationGuid);

        public Task<ApiResult<RispostaE002OK>> APIServizioAccertamentoCittadinanzaAsync(string user, string IdInterrogazioneRer, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid);

        public Task<ApiResult<RispostaE002OK>> APIServizioVerificaDichiarazioneCittadinanzaAsync(string user, string IdInterrogazioneRer, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid);

        public Task<ApiResult<RispostaE002OK>> APIServizioAccertamentoResidenzaAsync(string user, string IdInterrogazioneRer, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid);

        public Task<ApiResult<RispostaE002OK>> APIServizioAccertamentoGeneralitaAsync(string user, string IdInterrogazioneRer, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid);

        public Task<ApiResult<RispostaE002OK>> APIServizioAccertamentoStatoDiFamigliaAsync(string user, string IdInterrogazioneRer, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid);

        public Task<ApiResult<RispostaE002OK>> APIServizioAccertamentoGenitoreAsync(string user, string IdInterrogazioneRer, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid);

        public Task<ApiResult<RispostaE002OK>> APIServizioAccertamentoIdentificativoUnicoNazionaleAsync(string user, string IdInterrogazioneRer, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid);


        public Task<ApiResult<RispostaE002OK>> APIServizioAccertamentoVedovanzaAsync(string user, string IdInterrogazioneRer, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid);

        public Task<ApiResult<RispostaE002OK>> APIServizioAccertamentoDichDecessoAsync(string user, string IdInterrogazioneRer, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid);

        public Task<ApiResult<RispostaE002OK>> APIServizioAccertamentoEsistenzaVitaAsync(string user, string IdInterrogazioneRer, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid);


    }
}
