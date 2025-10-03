using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ScERPA.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Aree",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Ordinale = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aree", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Cognome = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Matricola = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CodiceFiscale = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    Attivo = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Configurazioni",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Ambiente = table.Column<int>(type: "int", nullable: false),
                    ApiManagerClientId = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApiManagerClientSecret = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApiManagerOauthEndpoint = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configurazioni", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataProtectionKeys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FriendlyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Xml = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataProtectionKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ListaTenant",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Parent = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Logo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListaTenant", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sezioni",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    Ordinale = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sezioni", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sezioni_Aree_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Aree",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConfigurazioneTenant",
                columns: table => new
                {
                    ConfigurazioniId = table.Column<int>(type: "int", nullable: false),
                    ListaTenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurazioneTenant", x => new { x.ConfigurazioniId, x.ListaTenantId });
                    table.ForeignKey(
                        name: "FK_ConfigurazioneTenant_Configurazioni_ConfigurazioniId",
                        column: x => x.ConfigurazioniId,
                        principalTable: "Configurazioni",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConfigurazioneTenant_ListaTenant_ListaTenantId",
                        column: x => x.ListaTenantId,
                        principalTable: "ListaTenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UtentiTenant",
                columns: table => new
                {
                    UtenteId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtentiTenant", x => new { x.UtenteId, x.TenantId });
                    table.ForeignKey(
                        name: "FK_UtentiTenant_AspNetUsers_UtenteId",
                        column: x => x.UtenteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UtentiTenant_ListaTenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "ListaTenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Servizi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Descrizione = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Cod = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    SezioneId = table.Column<int>(type: "int", nullable: true),
                    Attivo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servizi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Servizi_Aree_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Aree",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Servizi_Sezioni_SezioneId",
                        column: x => x.SezioneId,
                        principalTable: "Sezioni",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IndirizziChiamata",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ambiente = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Indirizzo = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    ServizioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndirizziChiamata", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndirizziChiamata_ListaTenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "ListaTenant",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IndirizziChiamata_Servizi_ServizioId",
                        column: x => x.ServizioId,
                        principalTable: "Servizi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ListaFinalita",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ambiente = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Descrizione = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DataDal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaxChiamate = table.Column<int>(type: "int", nullable: false),
                    UnitaTempoChiamate = table.Column<int>(type: "int", nullable: false),
                    IntervalloTempoChiamate = table.Column<int>(type: "int", nullable: false),
                    DataAl = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ServizioId = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListaFinalita", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListaFinalita_ListaTenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "ListaTenant",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ListaFinalita_Servizi_ServizioId",
                        column: x => x.ServizioId,
                        principalTable: "Servizi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Purpouses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Valore = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DataDal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAl = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FinalitaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purpouses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Purpouses_ListaFinalita_FinalitaId",
                        column: x => x.FinalitaId,
                        principalTable: "ListaFinalita",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RichiesteMassive",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdAccodamento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FinalitaId = table.Column<int>(type: "int", nullable: false),
                    Stato = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TimestampCreazioneRichiesta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimestampUltimaElaborazione = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ambiente = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RichiesteMassive", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RichiesteMassive_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RichiesteMassive_ListaFinalita_FinalitaId",
                        column: x => x.FinalitaId,
                        principalTable: "ListaFinalita",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RichiesteMassive_ListaTenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "ListaTenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UtentiFinalita",
                columns: table => new
                {
                    UtenteId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FinalitaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtentiFinalita", x => new { x.UtenteId, x.FinalitaId });
                    table.ForeignKey(
                        name: "FK_UtentiFinalita_AspNetUsers_UtenteId",
                        column: x => x.UtenteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UtentiFinalita_ListaFinalita_FinalitaId",
                        column: x => x.FinalitaId,
                        principalTable: "ListaFinalita",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Chiamate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ambiente = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    FinalitaId = table.Column<int>(type: "int", nullable: false),
                    PurpouseID = table.Column<int>(type: "int", nullable: false),
                    OperationGUID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimestampCreazioneRichiesta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimestampInvocazioneRichiesta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TimestampRispostaRichiesta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CodiceRisposta = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chiamate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chiamate_ListaFinalita_FinalitaId",
                        column: x => x.FinalitaId,
                        principalTable: "ListaFinalita",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Chiamate_ListaTenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "ListaTenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Chiamate_Purpouses_PurpouseID",
                        column: x => x.PurpouseID,
                        principalTable: "Purpouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ElementiRichiesteMassive",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RichiestaMassivaId = table.Column<int>(type: "int", nullable: false),
                    Input = table.Column<string>(type: "nvarchar(max)", maxLength: 32000, nullable: false),
                    Output = table.Column<string>(type: "nvarchar(max)", maxLength: 32000, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdAccodamento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tentativo = table.Column<int>(type: "int", nullable: false),
                    TimestampCreazioneRichiesta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimestampUltimaElaborazione = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CodiceRisposta = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElementiRichiesteMassive", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElementiRichiesteMassive_RichiesteMassive_RichiestaMassivaId",
                        column: x => x.RichiestaMassivaId,
                        principalTable: "RichiesteMassive",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Aree",
                columns: new[] { "Id", "Nome", "Ordinale" },
                values: new object[] { 1, "Anpr", 1 });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1", null, "SuperAdmin", "SUPERADMIN" },
                    { "2", null, "Admin", "ADMIN" },
                    { "3", null, "Manager", "MANAGER" },
                    { "4", null, "Consumer", "CONSUMER" }
                });

            migrationBuilder.InsertData(
                table: "ListaTenant",
                columns: new[] { "Id", "Logo", "Nome", "Parent", "Url" },
                values: new object[,]
                {
                    { 1, "/Images/logo_rer.png", "Giunta", "Giunta Regione Emilia-Romagna", "https://www.regione.emilia-romagna.it" },
                    { 2, "/Images/logo_rer.png", "Assemblea", "Assemblea Legislativa Regione Emilia-Romagna", "https://www.assemblea.emr.it/" },
                    { 3, "/Images/Logo-ProCiv-ER.jpg", "Agenzia di Protezione Civile", "Agenzia di Protezione Civile", "https://protezionecivile.regione.emilia-romagna.it/" },
                    { 4, "/Images/logo_agrea.png", "AGREA", "AGREA", "https://agrea.regione.emilia-romagna.it/" },
                    { 5, "/Images/logo_intercenter.png", "INTERCENTER", "INTERCENTER", "https://intercenter.regione.emilia-romagna.it/" },
                    { 6, "/Images/logo_arl.png", "Agenzia per il Lavoro", "Agenzia per il Lavoro", "https://www.agenzialavoro.emr.it/" }
                });

            migrationBuilder.InsertData(
                table: "Sezioni",
                columns: new[] { "Id", "AreaId", "Nome", "Ordinale" },
                values: new object[,]
                {
                    { 1, 1, "Verifica", 1 },
                    { 2, 1, "Accertamento", 2 },
                    { 3, 1, "Accertamento Massivo", 3 }
                });

            migrationBuilder.InsertData(
                table: "Servizi",
                columns: new[] { "Id", "AreaId", "Attivo", "Cod", "Descrizione", "Nome", "SezioneId" },
                values: new object[,]
                {
                    { 1, 1, true, "C006", "Verifica dichiarazione cittadinanza italiana", "VerificaCittadinanzaItaliana", 1 },
                    { 2, 1, true, "C015", "Accertamento generalità", "AccertamentoGeneralita", 2 },
                    { 3, 1, true, "C016", "Accertamento dichiarazione di decesso", "AccertamentoDichDecesso", 2 },
                    { 4, 1, true, "C018", "Accertamento cittadinanza", "AccertamentoCittadinanza", 2 },
                    { 5, 1, true, "C019", "Accertamento esistenza in vita", "AccertamentoEsistenzaVita", 2 },
                    { 6, 1, true, "C020", "Accertamento residenza", "AccertamentoResidenza", 2 },
                    { 7, 1, true, "C021", "Accertamento stato di famiglia", "AccertamentoStatoDiFamiglia", 2 },
                    { 8, 1, true, "C023", "Accertamento vedovanza", "AccertamentoVedovanza", 2 },
                    { 9, 1, true, "C024", "Accertamento paternità", "AccertamentoPaternita", 2 },
                    { 10, 1, true, "C025", "Accertamento maternità", "AccertamentoMaternita", 2 },
                    { 11, 1, true, "C030", "Accertamento identificativo unico nazionale", "AccertamentoIdentificativoUnicoNazionale", 2 },
                    { 12, 1, false, "C020", "Accertamento residenza massivo", "AccertamentoResidenzaMassivo", 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Chiamate_FinalitaId",
                table: "Chiamate",
                column: "FinalitaId");

            migrationBuilder.CreateIndex(
                name: "IX_Chiamate_PurpouseID",
                table: "Chiamate",
                column: "PurpouseID");

            migrationBuilder.CreateIndex(
                name: "IX_Chiamate_TenantId",
                table: "Chiamate",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigurazioneTenant_ListaTenantId",
                table: "ConfigurazioneTenant",
                column: "ListaTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ElementiRichiesteMassive_RichiestaMassivaId",
                table: "ElementiRichiesteMassive",
                column: "RichiestaMassivaId");

            migrationBuilder.CreateIndex(
                name: "IX_IndirizziChiamata_ServizioId",
                table: "IndirizziChiamata",
                column: "ServizioId");

            migrationBuilder.CreateIndex(
                name: "IX_IndirizziChiamata_TenantId",
                table: "IndirizziChiamata",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ListaFinalita_ServizioId",
                table: "ListaFinalita",
                column: "ServizioId");

            migrationBuilder.CreateIndex(
                name: "IX_ListaFinalita_TenantId",
                table: "ListaFinalita",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Purpouses_FinalitaId",
                table: "Purpouses",
                column: "FinalitaId");

            migrationBuilder.CreateIndex(
                name: "IX_RichiesteMassive_FinalitaId",
                table: "RichiesteMassive",
                column: "FinalitaId");

            migrationBuilder.CreateIndex(
                name: "IX_RichiesteMassive_TenantId",
                table: "RichiesteMassive",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_RichiesteMassive_UserID",
                table: "RichiesteMassive",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Servizi_AreaId",
                table: "Servizi",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Servizi_SezioneId",
                table: "Servizi",
                column: "SezioneId");

            migrationBuilder.CreateIndex(
                name: "IX_Sezioni_AreaId",
                table: "Sezioni",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_UtentiFinalita_FinalitaId",
                table: "UtentiFinalita",
                column: "FinalitaId");

            migrationBuilder.CreateIndex(
                name: "IX_UtentiTenant_TenantId",
                table: "UtentiTenant",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Chiamate");

            migrationBuilder.DropTable(
                name: "ConfigurazioneTenant");

            migrationBuilder.DropTable(
                name: "DataProtectionKeys");

            migrationBuilder.DropTable(
                name: "ElementiRichiesteMassive");

            migrationBuilder.DropTable(
                name: "IndirizziChiamata");

            migrationBuilder.DropTable(
                name: "UtentiFinalita");

            migrationBuilder.DropTable(
                name: "UtentiTenant");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Purpouses");

            migrationBuilder.DropTable(
                name: "Configurazioni");

            migrationBuilder.DropTable(
                name: "RichiesteMassive");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ListaFinalita");

            migrationBuilder.DropTable(
                name: "ListaTenant");

            migrationBuilder.DropTable(
                name: "Servizi");

            migrationBuilder.DropTable(
                name: "Sezioni");

            migrationBuilder.DropTable(
                name: "Aree");
        }
    }
}
