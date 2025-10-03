using ScERPA.Models;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;



namespace ScERPA.Data;

public class ScERPAContext : IdentityDbContext<ApplicationUser>, IDataProtectionKeyContext
{
    public DbSet<Area> Aree { get; set; }
    public DbSet<Sezione> Sezioni { get; set; }
    public DbSet<Finalita> ListaFinalita { get; set; }
    public DbSet<IndirizzoChiamata> IndirizziChiamata { get; set; }
    public DbSet<Purpouse> Purpouses { get; set; }
    public DbSet<Servizio> Servizi { get; set; }
    public DbSet<Tenant>  ListaTenant { get; set; }
    public DbSet<Configurazione> Configurazioni { get; set; }
    public DbSet<Chiamata> Chiamate { get; set; }
    public DbSet<RichiestaMassiva> RichiesteMassive { get; set; }
    public DbSet<ElementoRichiestaMassiva> ElementiRichiesteMassive { get; set; }

    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

    public ScERPAContext(DbContextOptions<ScERPAContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>().HasMany(e => e.ListaFinalita).WithMany(e => e.ListaUtenti).UsingEntity<UtenteFinalita>
        (
            "UtentiFinalita",
            l => l.HasOne<Finalita>().WithMany().HasForeignKey(e => e.FinalitaId).HasPrincipalKey(e => e.Id),            
            r => r.HasOne<ApplicationUser>().WithMany().HasForeignKey(e => e.UtenteId).HasPrincipalKey(e => e.Id),
            j => j.HasKey(e => new { e.UtenteId, e.FinalitaId })
             
        );

        builder.Entity<ApplicationUser>().HasMany(e => e.Tenants).WithMany(e => e.ListaUtenti).UsingEntity<UtenteTenant>
        (
            "UtentiTenant",
            l => l.HasOne<Tenant>().WithMany().HasForeignKey(e => e.TenantId).HasPrincipalKey(e => e.Id),
            r => r.HasOne<ApplicationUser>().WithMany().HasForeignKey(e => e.UtenteId).HasPrincipalKey(e => e.Id),
            j => j.HasKey(e => new { e.UtenteId, e.TenantId })

        );

        Seed(builder);

    }



    private static void Seed(ModelBuilder builder)
    {
        builder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Id = "1", Name = "SuperAdmin", NormalizedName = "SUPERADMIN", ConcurrencyStamp=null},
                new IdentityRole() { Id = "2", Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = null },
                new IdentityRole() { Id = "3", Name = "Manager", NormalizedName = "MANAGER", ConcurrencyStamp = null },
                new IdentityRole() { Id = "4", Name = "Consumer", NormalizedName = "CONSUMER", ConcurrencyStamp = null }
        );

        builder.Entity<Area>().HasData(
                new Area() { Id=1,Nome="Anpr",Ordinale=1}
        );

        builder.Entity<Sezione>().HasData(
           new Sezione() { Id = 1, Nome = "Verifica", AreaId = 1, Ordinale = 1},
           new Sezione() { Id = 2, Nome = "Accertamento", AreaId = 1, Ordinale = 2},
           new Sezione() { Id = 3, Nome = "Accertamento Massivo",AreaId = 1, Ordinale = 3 }
        );

        builder.Entity<Servizio>().HasData( 
            new Servizio()
            {
                Id = 1,
                AreaId = 1,
                SezioneId = 1,
                Attivo = true,
                Nome = "VerificaCittadinanzaItaliana",
                Descrizione = "Verifica dichiarazione cittadinanza italiana",
                Cod= "C006"
            }, new Servizio()
            {
                Id = 2,
                AreaId = 1,
                SezioneId = 2,
                Attivo = true,
                Nome = "AccertamentoGeneralita",
                Descrizione = "Accertamento generalità",
                Cod = "C015"
            }, new Servizio()
            {
                Id = 3,
                AreaId = 1,
                SezioneId = 2,
                Attivo = true,
                Nome = "AccertamentoDichDecesso",
                Descrizione = "Accertamento dichiarazione di decesso",
                Cod = "C016"
            }, new Servizio()
            {
                Id = 4,
                AreaId = 1,
                SezioneId = 2,
                Attivo = true,
                Nome = "AccertamentoCittadinanza",
                Descrizione = "Accertamento cittadinanza",
                Cod = "C018"
            }, new Servizio()
            {
                Id = 5,
                AreaId = 1,
                SezioneId = 2,
                Attivo = true,
                Nome = "AccertamentoEsistenzaVita",
                Descrizione = "Accertamento esistenza in vita",
                Cod = "C019"
            }, new Servizio()
            {
                Id = 6,
                AreaId = 1,
                SezioneId = 2,
                Attivo = true,
                Nome = "AccertamentoResidenza",
                Descrizione = "Accertamento residenza",
                Cod= "C020"
            }, new Servizio()
            {
                Id = 7,
                AreaId = 1,
                SezioneId = 2,
                Attivo = true,
                Nome = "AccertamentoStatoDiFamiglia",
                Descrizione = "Accertamento stato di famiglia",
                Cod = "C021"
            }, new Servizio()
            {
                Id = 8,
                AreaId = 1,
                SezioneId = 2,
                Attivo = true,
                Nome = "AccertamentoVedovanza",
                Descrizione = "Accertamento vedovanza",
                Cod = "C023"
            }, new Servizio()
            {
                Id = 9,
                AreaId = 1,
                SezioneId = 2,
                Attivo = true,
                Nome = "AccertamentoPaternita",
                Descrizione = "Accertamento paternità",
                Cod = "C024"
            }, new Servizio()
            {
                Id = 10,
                AreaId = 1,
                SezioneId = 2,
                Attivo = true,
                Nome = "AccertamentoMaternita",
                Descrizione = "Accertamento maternità",
                Cod = "C025"
            }, new Servizio()
            {
                Id = 11,
                AreaId = 1,
                SezioneId = 2,
                Attivo = true,
                Nome = "AccertamentoIdentificativoUnicoNazionale",
                Descrizione = "Accertamento identificativo unico nazionale",
                Cod = "C030"
            }, new Servizio()
            {
                Id = 12,
                AreaId = 1,
                SezioneId = 3,
                Attivo = false,
                Nome = "AccertamentoResidenzaMassivo",
                Descrizione = "Accertamento residenza massivo",
                Cod = "C020"
            }
         );

        builder.Entity<Tenant>().HasData(
            new Tenant() {
                Id=1,
                Nome = "Giunta",
                Parent= "Giunta Regione Emilia-Romagna",
                Url = "https://www.regione.emilia-romagna.it",
                Logo = "/Images/logo_rer.png"
            },
            new Tenant()
            {
                Id = 2,
                Nome = "Assemblea",
                Parent = "Assemblea Legislativa Regione Emilia-Romagna",
                Url = "https://www.assemblea.emr.it/",
                Logo = "/Images/logo_rer.png"
            },
            new Tenant()
            {
                Id = 3,
                Nome = "Agenzia di Protezione Civile",
                Parent = "Agenzia di Protezione Civile",
                Url = "https://protezionecivile.regione.emilia-romagna.it/",
                Logo = "/Images/Logo-ProCiv-ER.jpg"
            },
            new Tenant()
            {
                Id = 4,
                Nome = "AGREA",
                Parent = "AGREA",
                Url = "https://agrea.regione.emilia-romagna.it/",
                Logo = "/Images/logo_agrea.png"
            },
            new Tenant()
            {
                Id = 5,
                Nome = "INTERCENTER",
                Parent = "INTERCENTER",
                Url = "https://intercenter.regione.emilia-romagna.it/",
                Logo = "/Images/logo_intercenter.png"
            }, new Tenant()
            {
                Id = 6,
                Nome = "Agenzia per il Lavoro",
                Parent = "Agenzia per il Lavoro",
                Url = "https://www.agenzialavoro.emr.it/",
                Logo = "/Images/logo_arl.png"
            }
        );


    }

}
