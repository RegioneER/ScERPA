using ScERPA.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration.Json;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;
using System.Net.Security;
using System.Globalization;
using System.Threading;
using Microsoft.Extensions.Configuration;
using System.Text;
using Serilog.Context;
using SQLitePCL;
using ScERPA.Data;
using Microsoft.EntityFrameworkCore;
using ScERPA.Models;
using System.Runtime.CompilerServices;
using System.Runtime.Caching;
using Microsoft.Extensions.Caching.Memory;
using NuGet.Protocol.Core.Types;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc;


namespace ScERPA.Services
{

    public class JSonOauthResponse
    {
        public string? access_token { get; set; }
        public string? scope { get; set; }
        public string? token_type { get; set; }
        public int? expires_in { get; set; }

    }

    public class AuthenticationClientWSO2 : IAuthenticationClient
    {
        private readonly ILogger<AuthenticationClientWSO2> _logger;
        private readonly ICommonServices _commonServices;
        private readonly ScERPAContext _repository;
        private readonly IUtilities _utilities;
        private readonly string? _address;
        private readonly bool _useDefaultCredentials;
        private readonly bool _useProxy;
        private readonly string _oauthEndpoint;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly int _currentTenantId;
        private readonly IMemoryCache _memoryCache;


        public AuthenticationClientWSO2(IConfiguration configuration,ICommonServices commonServices, ILogger<AuthenticationClientWSO2> logger, ScERPAContext repository,IUtilities utilities, IMemoryCache memoryCache)
        {
            _logger = logger;

            _address = configuration.GetSection("Proxy").GetValue<string>("Address");
            _useDefaultCredentials = configuration.GetSection("Proxy").GetValue<bool>("UseDefaultCredentials");
            _useProxy = configuration.GetSection("Proxy").GetValue<bool>("UseProxy");
            _repository = repository;
            _commonServices = commonServices;
            _currentTenantId = commonServices.IdTenantCorrente;
            _utilities = utilities;
            _clientId = this.TenantCorrenteClientId;
            _clientSecret = this.TenantCorrenteClientSecret;
            _oauthEndpoint = this.TenantCorrenteClientOauthEndpoint;
            _memoryCache = memoryCache;

        }
        public async Task<ApiResult<string>> GetAuthTokenAsync(string? operationGuid, string? clientId, string? clientSecret)
        {
            string? authtoken = null;
            HttpClientHandler clienthandler;
            HttpClient client;
            string clientid = clientId == null ? _clientId : clientId;
            string clientsecret = clientSecret == null ? _clientSecret : clientSecret;
            ApiResult<string> risultato = new(operationGuid);

            var cachedAuthToken = _memoryCache.Get<string>(clientid);
            if(cachedAuthToken != null)
            {
                risultato.Status = ApiResultStatus.Success;
                risultato.Data = cachedAuthToken;
                using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogInformation("Uso un cachedtoken wso2");

            } else
            {
                clienthandler = new HttpClientHandler();
                if (_useProxy)
                {
                    clienthandler.AllowAutoRedirect = true;
                    clienthandler.Proxy = new WebProxy()
                    {
                        Address = new Uri(_address + ""),
                        UseDefaultCredentials = this._useDefaultCredentials
                    };
                }
                
                var dict = new Dictionary<string, string>();
                dict.Add("client_id", clientid);
                dict.Add("client_secret", clientsecret);
                dict.Add("grant_type", "client_credentials");

                var request = new HttpRequestMessage(HttpMethod.Post, _oauthEndpoint) { Content = new FormUrlEncodedContent(dict) };
                request.Headers.Clear();
                request.Headers.Add("x-requested-with", "XMLHttpRequest");
                request.Headers.TryAddWithoutValidation("Accept", "application/json");
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogInformation("Provo a staccare token wso2");
                using (client = new HttpClient(clienthandler))
                {
                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseStream = await response.Content.ReadAsStringAsync();
                        var rispostaAutenticazione = JsonSerializer.Deserialize<JSonOauthResponse>(responseStream);
                        authtoken = rispostaAutenticazione?.access_token;
                        if (!string.IsNullOrEmpty(authtoken))
                        {
                            using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogInformation("Riuscito lo stacco token wso2");
                            risultato.Data = authtoken;
                            risultato.Status = ApiResultStatus.Success;



                            JwtSecurityToken? token = new JwtSecurityToken(authtoken);

                            if (!token.Audiences.Contains(clientid) || token.ValidFrom > DateTime.UtcNow || token.ValidTo < DateTime.UtcNow) {

                                throw new UnauthorizedAccessException("Token wso2 non valido");
                            }
                            else
                            {
                                _memoryCache.Set(clientid, authtoken, token.ValidTo.ToLocalTime().AddSeconds(-10));
                            }
                               
                        }
                        else
                        {
                            using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogError("Fallito lo stacco token wso2: nullo o vuoto");
                            risultato.Status = ApiResultStatus.FailedApiManagerGetToken;
                        }

                    }
                    else
                    {
                        using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogError("Fallito lo stacco token wso2: errore");
                        risultato.Status = ApiResultStatus.FailedApiManagerGetToken;
                    }
                }
            }




            return risultato;

        }


        private string TenantCorrenteClientId
        {

            get
            {
                string clientId = "";

                var query = from config in _repository.Configurazioni
                            let listaTenant = config.ListaTenant
                            from tenant in listaTenant
                            where tenant.Id == _currentTenantId && config.Ambiente == _commonServices.AmbienteCorrente
                            select config;

                //var configurazioni = _repository.Configurazioni.Include(configurazione => configurazione.ListaTenant.Where(tenant => tenant.Id == _currentTenantId)).Where(configurazione => configurazione.Ambiente == _commonServices.AmbienteCorrente ).ToList();
                //var configurazione = configurazioni.Where(c => c.ListaTenant?.Count == 1).Select(c=>c).SingleOrDefault();
                var configurazione = query.SingleOrDefault();
                if (configurazione is not null )
                {
                    clientId = _utilities.Decrypt(configurazione.ApiManagerClientId ?? "");
                }
                return clientId;
            }
        }


        private string TenantCorrenteClientSecret
        {

            get
            {
                string clientSecret = "";

                var query = from config in _repository.Configurazioni
                            let listaTenant = config.ListaTenant
                            from tenant in listaTenant
                            where tenant.Id == _currentTenantId && config.Ambiente == _commonServices.AmbienteCorrente
                            select config;

                //var configurazioni = _repository.Configurazioni.Include(configurazione => configurazione.ListaTenant).Where(tenant => tenant.Id == _currentTenantId).Where(configurazione => configurazione.Ambiente == _commonServices.AmbienteCorrente).ToList();
                //var configurazione = configurazioni.Where(c => c.ListaTenant?.Count == 1).Select(c=>c).SingleOrDefault();
                var configurazione = query.SingleOrDefault();
                if (configurazione is not null )
                {
                    clientSecret = _utilities.Decrypt(configurazione.ApiManagerClientSecret ?? "");
                }
                return clientSecret;
            }
        }

        private string TenantCorrenteClientOauthEndpoint
        {

            get
            {
                string oauthEndpoint = "";

                var query = from config in _repository.Configurazioni
                            let listaTenant = config.ListaTenant
                            from tenant in listaTenant
                            where tenant.Id == _currentTenantId && config.Ambiente == _commonServices.AmbienteCorrente
                            select config;

                //var query = _repository.Configurazioni.Include(configurazione => configurazione.ListaTenant.Where(tenant => tenant.Id == _currentTenantId)).Where(configurazione => configurazione.Ambiente == _commonServices.AmbienteCorrente).Select(c => c);
                //var configurazioni = query.ToList();
                var configurazione = query.SingleOrDefault();
                if (configurazione is not null )
                {
                    oauthEndpoint = configurazione.ApiManagerOauthEndpoint ?? "";
                }
                return oauthEndpoint;
            }
        }

    }
}

