using ScERPA.Models.Dictionaries;
using ScERPA.Services.Interfaces;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using iText.StyledXmlParser.Jsoup.Parser;
using Serilog.Context;
using ScERPA.Models;
using Microsoft.EntityFrameworkCore;
using ScERPA.Data;
using Microsoft.Extensions.Logging;
using Polly;
using System.Net.Http;
using Microsoft.Extensions.Http.Resilience;
using Polly.Registry;

namespace ScERPA.Services
{
    public class AnprApiWSO2 : IAnprApi
    {
        private readonly IAuthenticationClient _authenticationClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AnprApiWSO2> _logger;
        private readonly ICommonServices _commonServices;
        private readonly IApiServices _apiServices;
        private readonly ScERPAContext _repository;

        private readonly string _address;
        private readonly bool _useDefaultCredentials;
        private readonly bool _useProxy;

        public AnprApiWSO2(ScERPAContext repository,IAuthenticationClient authenticationClient, IConfiguration configuration, ILogger<AnprApiWSO2> logger, IWebHostEnvironment webHostEnvironment, ICommonServices commonServices, IApiServices apiServices) 
        {
            _authenticationClient = authenticationClient;
            _commonServices = commonServices;
            _logger = logger;
            _configuration = configuration;
            _apiServices = apiServices;
            _repository = repository;
            _address = configuration.GetSection("Proxy").GetValue<string>("Address") + "";
            _useDefaultCredentials = configuration.GetSection("Proxy").GetValue<bool>("UseDefaultCredentials");
            _useProxy = configuration.GetSection("Proxy").GetValue<bool>("UseProxy");

        }

        private async Task<ApiResult<RispostaE002OK>> APIServizioAccertamentoAsync(string user, string IdInterrogazioneRer, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid)
        {

            ApiResult<RispostaE002OK> risposta;
            Finalita finalita;
            Servizio servizio;

            IndirizzoChiamata indirizzoChiamata;
            Chiamata chiamata = new();
            string nomeServizio = "";

            string CasoUso = "";

            string serviceEndpoint = "";


            finalita = await _apiServices.GetFinalityFromPurpouseIDAsync(purpouseID);
            if (finalita is not null)
            {
                servizio = finalita.Servizio;
                if (servizio is not null && servizio.Attivo)
                {
                    nomeServizio = servizio.Nome;
                    CasoUso = servizio.Cod + "";
                    if (servizio.IndirizziChiamata is not null)
                    {
                        indirizzoChiamata = servizio.IndirizziChiamata.AsQueryable().AsNoTracking().Where(i => i.Ambiente == _commonServices.AmbienteCorrente && i.TenantId == _commonServices.IdTenantCorrente).Single();
                        serviceEndpoint = indirizzoChiamata.Indirizzo;
                    }

                }
            }


            var requestObject = new RichiestaE002();
            requestObject.datiRichiesta = new();

            requestObject.criteriRicerca.codiceFiscale = codiceFiscale.Trim().ToUpper();
            requestObject.datiRichiesta.casoUso = CasoUso;
            requestObject.datiRichiesta.dataRiferimentoRichiesta = DataRiferimentoVerifica.ToString("yyyy-MM-dd");
            requestObject.idOperazioneClient = IdInterrogazioneRer;


            //se i campi sono nulli non vanno serializzati
            JsonSerializerOptions options = new()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };


            string json = JsonSerializer.Serialize(requestObject, options);

            using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogInformation("l'utente {UserID} dalla postazione {UserLocation} sta chiamando API {Servizio} chiamata RERAnpr {RerAnprID}", userID, userLocation, nomeServizio, requestObject.idOperazioneClient);

            chiamata.UserID = user;
            chiamata.Ambiente = _commonServices.AmbienteCorrente;
            chiamata.TenantId = _commonServices.IdTenantCorrente;
            chiamata.FinalitaId = finalita?.Id ?? 0;
            chiamata.PurpouseID = finalita?.ListaPurpouse?.AsQueryable().AsNoTracking().Where(x => x.Valore == purpouseID).Select(x => x.Id).SingleOrDefault() ?? 0; 
            chiamata.OperationGUID = operationGuid ?? "";
            chiamata.TimestampCreazioneRichiesta = DateTime.Now;
            risposta = await CallAPIAsync(chiamata, serviceEndpoint, purpouseID, json, userID, userLocation, LoA, operationGuid);

            return risposta;
        }

        public async Task<ApiResult<RispostaE002OK>> APIServizioAccertamentoCittadinanzaAsync(string user, string IdInterrogazioneRer, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid)
        {
            return await this.APIServizioAccertamentoAsync(user, IdInterrogazioneRer, codiceFiscale, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);
        }

        public async Task<ApiResult<RispostaE002OK>> APIServizioAccertamentoResidenzaAsync(string user, string IdInterrogazioneRer, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid)
        {
            return await this.APIServizioAccertamentoAsync(user, IdInterrogazioneRer, codiceFiscale, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);
        }


        public async Task<ApiResult<RispostaE002OK>> APIServizioAccertamentoStatoDiFamigliaAsync(string user, string IdInterrogazioneRer, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid)
        {
            return await this.APIServizioAccertamentoAsync(user, IdInterrogazioneRer, codiceFiscale, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);
        }

        public async Task<ApiResult<RispostaE002OK>> APIServizioAccertamentoGenitoreAsync(string user, string IdInterrogazioneRer, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid)
        {
            return await this.APIServizioAccertamentoAsync(user, IdInterrogazioneRer, codiceFiscale, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);
        }


        public async Task<ApiResult<RispostaE002OK>> APIServizioAccertamentoGeneralitaAsync(string user, string IdInterrogazioneRer, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid)
        {
            return await this.APIServizioAccertamentoAsync(user, IdInterrogazioneRer, codiceFiscale, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);
        }

        public async Task<ApiResult<RispostaE002OK>> APIServizioAccertamentoVedovanzaAsync(string user, string IdInterrogazioneRer, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid)
        {
            return await this.APIServizioAccertamentoAsync(user, IdInterrogazioneRer, codiceFiscale, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);
        }

        public async Task<ApiResult<RispostaE002OK>> APIServizioAccertamentoDichDecessoAsync(string user, string IdInterrogazioneRer, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid)
        {
            return await this.APIServizioAccertamentoAsync(user, IdInterrogazioneRer, codiceFiscale, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);
        }

        public async Task<ApiResult<RispostaE002OK>> APIServizioAccertamentoEsistenzaVitaAsync(string user, string IdInterrogazioneRer, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid)
        {
            return await this.APIServizioAccertamentoAsync(user, IdInterrogazioneRer, codiceFiscale, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);
        }

        public async Task<ApiResult<RispostaE002OK>> APIServizioVerificaDichiarazioneCittadinanzaAsync(string user, string IdInterrogazioneRer, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid)
        {

            ApiResult<RispostaE002OK> risposta;
            Finalita finalita;
            Servizio servizio;

            IndirizzoChiamata indirizzoChiamata;
            Chiamata chiamata = new();


            string CasoUso="";

            string serviceEndpoint = "";


            finalita = await _apiServices.GetFinalityFromPurpouseIDAsync(purpouseID);
            if (finalita is not null)
            {
                servizio = finalita.Servizio;
                if (servizio is not null && servizio.Attivo)
                {
                    CasoUso = servizio.Cod + "";
                    if(servizio.IndirizziChiamata is not null)
                    {
                        indirizzoChiamata = servizio.IndirizziChiamata.AsQueryable().AsNoTracking().Where(i => i.Ambiente == _commonServices.AmbienteCorrente && i.TenantId == _commonServices.IdTenantCorrente).Single();
                        serviceEndpoint = indirizzoChiamata.Indirizzo;
                    }  
            
                }
            }     

            var requestObject = new RichiestaE002();
            requestObject.datiRichiesta = new();
            requestObject.criteriRicerca.codiceFiscale = codiceFiscale.Trim().ToUpper();
            requestObject.datiRichiesta.casoUso = CasoUso;
            requestObject.datiRichiesta.dataRiferimentoRichiesta = DataRiferimentoVerifica.ToString("yyyy-MM-dd");


            requestObject.idOperazioneClient = IdInterrogazioneRer;



            requestObject.verifica = new TipoVerificaE002();

            requestObject.verifica.cittadinanza = new TipoCittadinanza { codiceStato = codiceStato, descrizioneStato = descrizioneStato };

            //se i campi sono nulli non vanno serializzati
            JsonSerializerOptions options = new()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };


            string json = JsonSerializer.Serialize(requestObject, options);

            using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogInformation("l'utente {UserID} dalla postazione {UserLocation} sta chiamando API VerificaDichiarazioneCittadinanza chiamata RERAnpr {RERAnprID}",userID, userLocation, requestObject.idOperazioneClient);

            chiamata.UserID = user;
            chiamata.Ambiente = _commonServices.AmbienteCorrente;
            chiamata.TenantId = _commonServices.IdTenantCorrente;
            chiamata.FinalitaId = finalita?.Id ?? 0;
            chiamata.PurpouseID = finalita?.ListaPurpouse?.AsQueryable().Where(x => x.Valore == purpouseID).Select(x => x.Id).SingleOrDefault() ?? 0; 
            chiamata.OperationGUID = operationGuid ?? "";
            chiamata.TimestampCreazioneRichiesta = DateTime.Now;
            risposta = await CallAPIAsync(chiamata, serviceEndpoint, purpouseID, json, userID, userLocation, LoA, operationGuid);

            return risposta;

        }

        public async Task<ApiResult<RispostaE002OK>> CallAPIAsync(Chiamata chiamata, string serviceEndpoint, string servicePurpouseID, string payload, string userID, string userLocation, string LoA, string? operationGuid)
        {
            HttpClient client;
            HttpClientHandler clienthandler;
            ApiResult<string> getAuthTokenResult;
            ApiResult<RispostaE002OK> risposta = new(operationGuid);
            RispostaE002OK? risultato = null;
            RispostaE002KO? risultatoKo = null;
            string? token;
            string responsestream;


            risposta.Status = ApiResultStatus.Failed;

            if (!string.IsNullOrEmpty(serviceEndpoint))
            {

                if (!string.IsNullOrEmpty(servicePurpouseID))
                {
                    //prendo il token di autenticazione
                    getAuthTokenResult = await _authenticationClient.GetAuthTokenAsync(operationGuid, null, null);





                    if (getAuthTokenResult.Status == ApiResultStatus.Success)
                    {
                        token = getAuthTokenResult.Data;
                        var _ = clienthandler = new HttpClientHandler();
                        clienthandler.AllowAutoRedirect = true;                 

                        if (_useProxy)
                        {
                            
                            clienthandler.Proxy = new WebProxy()
                            {
                                Address = new Uri(this._address),
                                UseDefaultCredentials = this._useDefaultCredentials
                            };                           
                        }

                        var content = new StringContent(payload, Encoding.Default, "application/json");

                        var request = new HttpRequestMessage(HttpMethod.Post, serviceEndpoint) { Content = content };

                        request.Headers.Clear();
                        request.Headers.Add("x-requested-with", "XMLHttpRequest");
                        request.Headers.Add("purposeid", servicePurpouseID);
                        request.Headers.Add("userID", userID);
                        request.Headers.Add("userLocation", userLocation);
                        request.Headers.Add("LoA", LoA);
                        request.Headers.Add("Authorization", "Bearer " + token);
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        request.Headers.TryAddWithoutValidation("Accept", "application/json");

                        //se i campi sono nulli non vanno serializzati
                        JsonSerializerOptions options = new()
                        {
                            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                        };

                        using (client = new HttpClient(clienthandler))
                        {

                            try
                            {
                                chiamata.TimestampInvocazioneRichiesta = DateTime.Now;



                                //Inserita logica di retry
                                client.Timeout = TimeSpan.FromSeconds(30);
       

                                var retryPolicy = Policy
                                                .Handle<HttpRequestException>()
                                                .OrResult<HttpResponseMessage>(r => (int)r.StatusCode >= 500 || r.StatusCode == HttpStatusCode.TooManyRequests)
                                                .WaitAndRetryAsync(
                                                        retryCount: 3,
                                                        sleepDurationProvider: (retryAttempt, response, context) =>
                                                        {
                                                            var random = new Random();
                                                            var maxDelay = TimeSpan.FromSeconds(30);
                                                            var jitterSeconds = random.NextDouble() * maxDelay.TotalSeconds;
                                                            var jitter = TimeSpan.FromSeconds(jitterSeconds);

                                                            if (response?.Result?.Headers?.RetryAfter?.Delta is TimeSpan retryAfter)
                                                            {
                                                                return retryAfter + jitter + TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                                                            }
                                                            else if ((bool)response?.Result?.Headers?.RetryAfter?.Date.HasValue)
                                                            {

                                                                var now = DateTimeOffset.UtcNow;                                                               

                                                                TimeSpan delay = (TimeSpan)(response?.Result?.Headers?.RetryAfter?.Date.Value - now) + jitter + TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                                                                return delay > TimeSpan.Zero ? delay : jitter + TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));

                                                            };                                                            

                                                            return jitter + TimeSpan.FromSeconds(Math.Pow(3, retryAttempt));
                                                        },
                                                        onRetryAsync: async (outcome, timespan, retryAttempt, context) =>
                                                        {
                                                            _logger.LogWarning($"Tentivo ulteriore di chiamata n. {retryAttempt} dopo {timespan.TotalSeconds} secodi a causa di  {outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString()}");
                                                        });



                                var response = await retryPolicy.ExecuteAsync(() => client.SendAsync(request));


                                chiamata.TimestampRispostaRichiesta = DateTime.Now;
                                using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogDebug("Response:{Response}", response);
                                try
                                {

                                    if (response.IsSuccessStatusCode)
                                    {
                                        
                                        responsestream = await response.Content.ReadAsStringAsync();

                                        using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogDebug("Response:{Response}", responsestream);
                                        if (!String.IsNullOrEmpty(responsestream))
                                        {
                                            risultato = JsonSerializer.Deserialize<RispostaE002OK>(responsestream, options);

                                            if (risultato is not null)
                                            {
                                                risposta.Status = ApiResultStatus.Success;
                                                risposta.Message = "API chiamata con successo";
                                                risposta.Data = risultato;
                                                using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogInformation("Chiamata ANPR avvenuta con successo");
                                            }
                                            else
                                            {
                                                risposta.Status = ApiResultStatus.DeserializeError;
                                                risposta.Message = "Impossibile deserializzare il risultato della chiamata API-ANPR";
                                                using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogError("{Message}", risposta.Message);
                                            }

                                        }

                                        else
                                        {
                                            risposta.Status = ApiResultStatus.EmptyResult;
                                            risposta.Message = "Impossibile ottenere il risultato della chiamata API-ANPR";
                                            using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogError("{Message}", risposta.Message);
                                        }

                                    }
                                    else if (response.StatusCode == HttpStatusCode.NotFound)
                                    {
                                        responsestream = await response.Content.ReadAsStringAsync();
                                        risposta.Status = ApiResultStatus.NotFound;
                                        risposta.Message = "Il soggetto non è presente in anagrafica";
                                        using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogWarning("{Message}", risposta.Message);
                                    }
                                    else if (response.StatusCode == HttpStatusCode.BadRequest )
                                    {
                                        risposta.Status = ApiResultStatus.BadRequest;
                                        responsestream = await response.Content.ReadAsStringAsync();
                                        risultatoKo = JsonSerializer.Deserialize<RispostaE002KO>(responsestream, options);
                                        if (risultatoKo is not null)
                                        {
                                            if (risultatoKo.listaErrori is not null && risultatoKo.listaErrori.Where(t => t.codiceErroreAnomalia == "EN148").Count() > 0)
                                            {
                                                risposta.Message = "Il soggetto risulta deceduto";
                                                using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogWarning("{Message}", risposta.Message);
                                            }
                                        }
                                        else
                                        {
                                            risposta.Status = ApiResultStatus.DeserializeError;
                                            risposta.Message = "Impossibile deserializzare la lista errori della chiamata API-ANPR";
                                            using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogWarning("{Message}", risposta.Message);
                                        }

                                    } else
                                    {
                                        risposta.Status = ApiResultStatus.FailedOnServiceProvider;
                                        risposta.Message = "La chiamata API-ANPR è fallita su ANPR";
                                        using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogError("{Message}", risposta.Message);
                                    }
                                }
                                catch
                                {
                                    risposta.Status = ApiResultStatus.FailedOnServiceProviderGeneric;
                                    risposta.Message = "La chiamata API-ANPR è fallita";
                                    using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogError("{Message}", risposta.Message);
                                }
                            }
                            catch
                            {
                                risposta.Status = ApiResultStatus.FailedServiceCall;
                                risposta.Message = "Impossibile effettuare la chiamata API-ANPR";
                                using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogError("{Message}", risposta.Message);
                            }

                        }

                    }
                    else
                    {

                        risposta.Status = ApiResultStatus.FailedApiManagerToken;
                        risposta.Message = "Impossibile ottenere il token di autorizzazione alla chiamata API-ANPR";
                        using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogError("{Message}", risposta.Message);
                    }
                } else
                {
                    risposta.Status = ApiResultStatus.FailedPurpouseConfig;
                    risposta.Message = "Configurazione della finalità errata o mancante";
                    using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogError("{Message}", risposta.Message);
                }

            }
            else
            {
                risposta.Status = ApiResultStatus.FailedServiceConfig;
                risposta.Message = "Configurazione del servizio errata o mancante";
                using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogError("{Message}", risposta.Message);
            }


            chiamata.CodiceRisposta = risposta.Status;

            _repository.Chiamate.Add(chiamata);
            await _repository.SaveChangesAsync();
            return risposta;

        }

        public async Task<ApiResult<RispostaE002OK>> APIServizioAccertamentoIdentificativoUnicoNazionaleAsync(string user, string IdInterrogazioneRer, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid)
        {
            return await this.APIServizioAccertamentoAsync(user, IdInterrogazioneRer, codiceFiscale, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);
        }
    }
}
