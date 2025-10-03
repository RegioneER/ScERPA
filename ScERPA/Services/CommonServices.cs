using ScERPA.Data;
using ScERPA.Models.Enums;
using ScERPA.Services.Interfaces;
using ScERPA.Models;
using Microsoft.CodeAnalysis.Host;
using System.Text;
using Org.BouncyCastle.Asn1.X9;
using System.Security.Claims;
using Microsoft.CodeAnalysis.Differencing;
using NuGet.Protocol.Core.Types;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;
using System.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace ScERPA.Services
{
    public class CommonServices : ICommonServices
    {
        private readonly IConfiguration _configuration;
        private readonly AmbientiEnum _ambitenteCorrente;
        private readonly ScERPAContext _repository;
        private readonly IUtilities _utilities;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _cdnRER;
        private readonly string _bootstrapitaliaVersion;
        private readonly string _jqueryVersion;
        private readonly string _echartsVersion;
        private readonly string _bootstrapitaliaBase;
        private readonly string _bootstrapitaliaCss;
        private readonly string _jqueryBase;
        private readonly string _echartsBase;
        private readonly string _fontPath;
        private readonly string _spritesPath;
        private readonly string _logoutUrl;
        private readonly string _accessibilitytUrl;
        private readonly bool _sperimental;
        private readonly IMemoryCache _memoryCache;

        public CommonServices(IConfiguration configuration, ScERPAContext repository, IUtilities utilities,IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _ambitenteCorrente = configuration.GetSection("Common").GetValue<AmbientiEnum>("CurrentEnvironment");
            _repository = repository;
            _utilities = utilities;
            _httpContextAccessor= httpContextAccessor;
            _webHostEnvironment = webHostEnvironment;
            _cdnRER = configuration.GetSection("Common").GetValue<string>("cdnRER") ?? "";
            _bootstrapitaliaVersion = configuration.GetSection("Common").GetValue<string>("bootstrapitalia") ?? "";
            _jqueryVersion = configuration.GetSection("Common").GetValue<string>("jquery") ?? "";
            _echartsVersion = configuration.GetSection("Common").GetValue<string>("echarts") ?? "";
            _bootstrapitaliaBase = $"{_cdnRER}/bootstrap-italia/{_bootstrapitaliaVersion}/dist";
            _jqueryBase = $"{_cdnRER}/jquery/{_jqueryVersion}/dist";
            _echartsBase = $"{_cdnRER}/echarts/{_echartsVersion}/dist";
            _fontPath = _bootstrapitaliaBase + "/fonts";
            _spritesPath = "/Images" + "/svg";
            _bootstrapitaliaCss = configuration.GetSection("Common").GetValue<string>("bootstrapitaliaCss") + ""; 
            _memoryCache = memoryCache;
            _sperimental= configuration.GetSection("Common").GetValue<bool>("sperimental");
            _logoutUrl = configuration.GetSection("Common").GetValue<string>("logoutUrl")??"";
            _accessibilitytUrl= configuration.GetSection("Common").GetValue<string>("accessibilityUrl") ?? "";
        }

        public AmbientiEnum AmbienteCorrente {
            get {
                return _ambitenteCorrente;
            }
        }


        public bool Sperimental
        {
            get
            {
                return _sperimental;
            }
        }

        public string CdnRER
        {
            get
            {
                return _cdnRER;
            }
        }


        public string AccessibilitytUrl
        {
            get
            {
                return _accessibilitytUrl;
            }
        }

        public string LogoutUrl
        {
            get
            {
                return _logoutUrl;
            }
        }


        public string BootstrapitaliaVersion
        {
            get
            {
                return _bootstrapitaliaVersion;
            }
        }

        public string JqueryVersion
        {
            get
            {
                return _jqueryVersion;
            }
        }


        public string EChartsVersion
        {
            get
            {
                return _echartsVersion;
            }
        }

        public string BootstrapitaliaBase
        {
            get
            {
                return _bootstrapitaliaBase;
            }
        }

        public string BootstrapitaliaCss
        {
            get
            {
                return _bootstrapitaliaCss;
            }
        }

        public string JqueryBase
        {
            get
            {
                return _jqueryBase;
            }
        }


        public string EChartsBase
        {
            get
            {
                return _echartsBase;
            }
        }

        public string FontPath
        {
            get
            {
                return _fontPath;
            }
        }

        public string SpritesPath
        {
            get
            {
                return _spritesPath;
            }
        }

        public string BrandTitle {
            get
            {
                string brandTitle = "ScERPA";

                switch(_ambitenteCorrente)
                {
                    case AmbientiEnum.Test:
                        brandTitle = brandTitle + " (" + _ambitenteCorrente.ToString()+")"; 
                        break;

                    case AmbientiEnum.Produzione:
                        break;

                }
                return brandTitle;
              
            }

        }

        public List<Tenant> ListaTenant
        {
         
            get
            {
                var listaTenant = _memoryCache.Get<List<Tenant>>("listaTenant");
                if (listaTenant is null) {
                    listaTenant = _repository.ListaTenant.ToList();
                    _memoryCache.Set("listaTenant", listaTenant,DateTime.Now.AddHours(1));
                   
                } 
               

                return listaTenant;
            }

        }

        public Tenant? TenantCorrente(int tenantId)
        {

            var tenantCorrente =  _memoryCache.Get<Tenant?>($"tenant_{tenantId}");

            if(tenantCorrente is null)
            {
                tenantCorrente = _repository.ListaTenant.FirstOrDefault(t => t.Id == tenantId);
                _memoryCache.Set($"tenant_{tenantId}", tenantCorrente, DateTime.Now.AddHours(1));
            
            } 
            return tenantCorrente;
        }

        public int IdTenantCorrente
        {
            get
            {
                int id = 0; 
                id = int.Parse("0" + _httpContextAccessor.HttpContext?.User?.FindFirst("CurrentTenantId")?.Value);
                return id;
            }
        }

       public string CurrentMenu { get; set; } = string.Empty;
 

       public string SitoTenant
        {
            get
            {
                string indirizzo = "https://www.regione.emilia-romagna.it";
                return indirizzo;
            }
        }

       public string PercorsoWWW
       {
            get
            {
                return _webHostEnvironment.WebRootPath;
            }
       }


    }
}
