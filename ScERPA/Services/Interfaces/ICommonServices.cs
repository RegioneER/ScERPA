using ScERPA.Models;
using ScERPA.Models.Enums;

namespace ScERPA.Services.Interfaces
{
    public interface ICommonServices
    {
        public AmbientiEnum AmbienteCorrente { get; }

        public string BrandTitle { get; }

        public int IdTenantCorrente { get; }

        public bool Sperimental { get; }

        public Tenant? TenantCorrente(int tenantId);

        public string SitoTenant { get; }

        public string PercorsoWWW { get; }

        public List<Tenant> ListaTenant { get; }

        public string CdnRER { get; }

        public string BootstrapitaliaVersion { get; }

        public string JqueryVersion { get; }

        public string EChartsVersion { get; }

        public string BootstrapitaliaBase { get; }

        public string BootstrapitaliaCss { get; }

        public string JqueryBase { get; }

        public string EChartsBase { get; }

        public string FontPath { get; }

        public string CurrentMenu { get; set; }

        public string SpritesPath { get; }

        public string LogoutUrl { get; }

        public string AccessibilitytUrl { get; }

    }
}
