using ScERPA.Models.Dictionaries;
using ScERPA.Models.Enums.ANPR;
using ScERPA.Models.Reports;
using ScERPA.ViewModels;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace ScERPA.Services.Interfaces
{
    public interface IAnprServices
    {
        public Task<List<RisultatoVerificaCittadinanzaViewModel>> ServizioVerificaCittadinanzaAsync(string user,string codiciFiscali, DateOnly DataRiferimentoVerifica, string codiceStato,string descrizioneStato, string userID,string userLocation, string LoA, string purpouseID, string? operationGuid);
        
        public Task<List<RisultatoAccertamentoResidenzaViewModel>> ServizioAccertamentoResidenzaAsync(string user, string codiciFiscali, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid);
        
        public Task<List<RisultatoAccertamentoStatoDiFamigliaViewModel>> ServizioAccertamentoStatoDiFamigliaAsync(string user, string codiciFiscali, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid);

        public Task<List<RisultatoAccertamentoDichDecessoViewModel>> ServizioAccertamentoDichDecessoAsync(string user, string codiciFiscali, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid);

        public Task<List<RisultatoAccertamentoGeneralitaViewModel>> ServizioAccertamentoGeneralitaAsync(string user, string codiciFiscali, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid);

        public Task<List<RisultatoAccertamentoIdentificativoUnicoNazionaleViewModel>> ServizioAccertamentoIdentificativoUnicoNazionaleAsync(string user, string codiciFiscali, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid);


        public Task<List<RisultatoAccertamentoCittadinanzaViewModel>> ServizioAccertamentoCittadinanzaAsync(string user, string codiciFiscali, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid);

        public Task<List<RisultatoAccertamentoEsistenzaVitaViewModel>> ServizioAccertamentoEsistenzaVitaAsync(string user, string codiciFiscali, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid);

        public Task<List<RisultatoAccertamentoVedovanzaViewModel>> ServizioAccertamentoVedovanzaAsync(string user, string codiciFiscali, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid);

        public Task<List<RisultatoAccertamentoGenitoreViewModel>> ServizioAccertamentoGenitoreAsync(TipologiaRicercaGenitore tipoRicercaGenitore,string user, string codiciFiscali, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid);

        public Task<RisultatoVerificaCittadinanzaViewModel> ServizioVerificaSingolaCittadinanzaAsync(string user, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid);

        public Task<RisultatoAccertamentoResidenzaViewModel> ServizioAccertamentoSingolaResidenzaAsync(string user, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid);
     
        public Task<RisultatoAccertamentoStatoDiFamigliaViewModel> ServizioAccertamentoSingoloStatoDiFamigliaAsync(string user, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid);

        public Task<RisultatoAccertamentoDichDecessoViewModel> ServizioAccertamentoSingolaDichDecessoAsync(string user, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid);

        public Task<RisultatoAccertamentoGeneralitaViewModel> ServizioAccertamentoSingolaGeneralitaAsync(string user, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid);

        public Task<RisultatoAccertamentoIdentificativoUnicoNazionaleViewModel> ServizioAccertamentoSingoloIdentificativoUnicoNazionaleAsync(string user, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid);

        public Task<RisultatoAccertamentoCittadinanzaViewModel> ServizioAccertamentoSingolaCittadinanzaAsync(string user, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid);
        public Task<RisultatoAccertamentoEsistenzaVitaViewModel> ServizioAccertamentoSingolaEsistenzaVitaAsync(string user, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid);
        public Task<RisultatoAccertamentoVedovanzaViewModel> ServizioAccertamentoSingolaVedovanzaAsync(string user, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid);
        public Task<RisultatoAccertamentoGenitoreViewModel> ServizioAccertamentoSingoloGenitoreAsync(TipologiaRicercaGenitore tipoRicercaGenitore, string user, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid);


        public Report CreateReport(string nomeBreve, string titolo, string sottotitolo, List<IElementoSchedaReport> lista, DateOnly dataVerifica, string userID, string userLocation, string LoA, string? operationGuid);

        public int NumeroMassimoCodiciFiscali { get; }

        public Task<int> CreaRichistaMassiva(string user, int idFinalita, MemoryStream stream);
 
    }
}
