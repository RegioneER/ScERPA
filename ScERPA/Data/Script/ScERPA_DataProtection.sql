IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Aree] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(250) NOT NULL,
    [Ordinale] int NOT NULL,
    CONSTRAINT [PK_Aree] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [Nome] nvarchar(250) NOT NULL,
    [Cognome] nvarchar(250) NOT NULL,
    [Matricola] nvarchar(20) NULL,
    [CodiceFiscale] nvarchar(16) NULL,
    [Attivo] bit NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);

CREATE TABLE [Configurazioni] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(500) NOT NULL,
    [Ambiente] int NOT NULL,
    [ApiManagerClientId] nvarchar(500) NULL,
    [ApiManagerClientSecret] nvarchar(500) NULL,
    [ApiManagerOauthEndpoint] nvarchar(max) NULL,
    CONSTRAINT [PK_Configurazioni] PRIMARY KEY ([Id])
);

CREATE TABLE [ListaTenant] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(250) NOT NULL,
    [Parent] nvarchar(1000) NOT NULL,
    [Url] nvarchar(1000) NOT NULL,
    [Logo] nvarchar(1000) NOT NULL,
    CONSTRAINT [PK_ListaTenant] PRIMARY KEY ([Id])
);

CREATE TABLE [Sezioni] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(250) NOT NULL,
    [AreaId] int NOT NULL,
    [Ordinale] int NOT NULL,
    CONSTRAINT [PK_Sezioni] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Sezioni_Aree_AreaId] FOREIGN KEY ([AreaId]) REFERENCES [Aree] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(128) NOT NULL,
    [ProviderKey] nvarchar(128) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(128) NOT NULL,
    [Name] nvarchar(128) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [ConfigurazioneTenant] (
    [ConfigurazioniId] int NOT NULL,
    [ListaTenantId] int NOT NULL,
    CONSTRAINT [PK_ConfigurazioneTenant] PRIMARY KEY ([ConfigurazioniId], [ListaTenantId]),
    CONSTRAINT [FK_ConfigurazioneTenant_Configurazioni_ConfigurazioniId] FOREIGN KEY ([ConfigurazioniId]) REFERENCES [Configurazioni] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ConfigurazioneTenant_ListaTenant_ListaTenantId] FOREIGN KEY ([ListaTenantId]) REFERENCES [ListaTenant] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [UtentiTenant] (
    [UtenteId] nvarchar(450) NOT NULL,
    [TenantId] int NOT NULL,
    CONSTRAINT [PK_UtentiTenant] PRIMARY KEY ([UtenteId], [TenantId]),
    CONSTRAINT [FK_UtentiTenant_AspNetUsers_UtenteId] FOREIGN KEY ([UtenteId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UtentiTenant_ListaTenant_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [ListaTenant] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Servizi] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(500) NOT NULL,
    [Descrizione] nvarchar(1000) NOT NULL,
    [Cod] nvarchar(250) NULL,
    [AreaId] int NOT NULL,
    [SezioneId] int NULL,
    [Attivo] bit NOT NULL,
    CONSTRAINT [PK_Servizi] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Servizi_Aree_AreaId] FOREIGN KEY ([AreaId]) REFERENCES [Aree] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Servizi_Sezioni_SezioneId] FOREIGN KEY ([SezioneId]) REFERENCES [Sezioni] ([Id])
);

CREATE TABLE [IndirizziChiamata] (
    [Id] int NOT NULL IDENTITY,
    [Ambiente] int NOT NULL,
    [TenantId] int NULL,
    [Indirizzo] nvarchar(max) NOT NULL,
    [ServizioId] int NOT NULL,
    CONSTRAINT [PK_IndirizziChiamata] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_IndirizziChiamata_ListaTenant_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [ListaTenant] ([Id]),
    CONSTRAINT [FK_IndirizziChiamata_Servizi_ServizioId] FOREIGN KEY ([ServizioId]) REFERENCES [Servizi] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [ListaFinalita] (
    [Id] int NOT NULL IDENTITY,
    [Ambiente] int NOT NULL,
    [Nome] nvarchar(500) NOT NULL,
    [Descrizione] nvarchar(1000) NOT NULL,
    [DataDal] datetime2 NOT NULL,
    [MaxChiamate] int NOT NULL,
    [UnitaTempoChiamate] int NOT NULL,
    [IntervalloTempoChiamate] int NOT NULL,
    [DataAl] datetime2 NULL,
    [ServizioId] int NOT NULL,
    [TenantId] int NULL,
    CONSTRAINT [PK_ListaFinalita] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ListaFinalita_ListaTenant_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [ListaTenant] ([Id]),
    CONSTRAINT [FK_ListaFinalita_Servizi_ServizioId] FOREIGN KEY ([ServizioId]) REFERENCES [Servizi] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Purpouses] (
    [Id] int NOT NULL IDENTITY,
    [Valore] nvarchar(500) NOT NULL,
    [DataDal] datetime2 NOT NULL,
    [DataAl] datetime2 NULL,
    [FinalitaId] int NOT NULL,
    CONSTRAINT [PK_Purpouses] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Purpouses_ListaFinalita_FinalitaId] FOREIGN KEY ([FinalitaId]) REFERENCES [ListaFinalita] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [UtentiFinalita] (
    [UtenteId] nvarchar(450) NOT NULL,
    [FinalitaId] int NOT NULL,
    CONSTRAINT [PK_UtentiFinalita] PRIMARY KEY ([UtenteId], [FinalitaId]),
    CONSTRAINT [FK_UtentiFinalita_AspNetUsers_UtenteId] FOREIGN KEY ([UtenteId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UtentiFinalita_ListaFinalita_FinalitaId] FOREIGN KEY ([FinalitaId]) REFERENCES [ListaFinalita] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Chiamate] (
    [Id] int NOT NULL IDENTITY,
    [Ambiente] int NOT NULL,
    [TenantId] int NOT NULL,
    [FinalitaId] int NOT NULL,
    [PurpouseID] int NOT NULL,
    [OperationGUID] nvarchar(max) NOT NULL,
    [UserID] nvarchar(max) NOT NULL,
    [TimestampCreazioneRichiesta] datetime2 NOT NULL,
    [TimestampInvocazioneRichiesta] datetime2 NULL,
    [TimestampRispostaRichiesta] datetime2 NULL,
    [CodiceRisposta] int NULL,
    CONSTRAINT [PK_Chiamate] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Chiamate_ListaFinalita_FinalitaId] FOREIGN KEY ([FinalitaId]) REFERENCES [ListaFinalita] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Chiamate_ListaTenant_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [ListaTenant] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Chiamate_Purpouses_PurpouseID] FOREIGN KEY ([PurpouseID]) REFERENCES [Purpouses] ([Id]) ON DELETE NO ACTION
);

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Nome', N'Ordinale') AND [object_id] = OBJECT_ID(N'[Aree]'))
    SET IDENTITY_INSERT [Aree] ON;
INSERT INTO [Aree] ([Id], [Nome], [Ordinale])
VALUES (1, N'Anpr', 1);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Nome', N'Ordinale') AND [object_id] = OBJECT_ID(N'[Aree]'))
    SET IDENTITY_INSERT [Aree] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
    SET IDENTITY_INSERT [AspNetRoles] ON;
INSERT INTO [AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName])
VALUES (N'1', NULL, N'SuperAdmin', N'SUPERADMIN'),
(N'2', NULL, N'Admin', N'ADMIN'),
(N'3', NULL, N'Manager', N'MANAGER'),
(N'4', NULL, N'Consumer', N'CONSUMER');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
    SET IDENTITY_INSERT [AspNetRoles] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AreaId', N'Nome', N'Ordinale') AND [object_id] = OBJECT_ID(N'[Sezioni]'))
    SET IDENTITY_INSERT [Sezioni] ON;
INSERT INTO [Sezioni] ([Id], [AreaId], [Nome], [Ordinale])
VALUES (1, 1, N'Verifica', 1),
(2, 1, N'Accertamento', 2);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AreaId', N'Nome', N'Ordinale') AND [object_id] = OBJECT_ID(N'[Sezioni]'))
    SET IDENTITY_INSERT [Sezioni] OFF;

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;

CREATE INDEX [IX_Chiamate_FinalitaId] ON [Chiamate] ([FinalitaId]);

CREATE INDEX [IX_Chiamate_PurpouseID] ON [Chiamate] ([PurpouseID]);

CREATE INDEX [IX_Chiamate_TenantId] ON [Chiamate] ([TenantId]);

CREATE INDEX [IX_ConfigurazioneTenant_ListaTenantId] ON [ConfigurazioneTenant] ([ListaTenantId]);

CREATE INDEX [IX_IndirizziChiamata_ServizioId] ON [IndirizziChiamata] ([ServizioId]);

CREATE INDEX [IX_IndirizziChiamata_TenantId] ON [IndirizziChiamata] ([TenantId]);

CREATE INDEX [IX_ListaFinalita_ServizioId] ON [ListaFinalita] ([ServizioId]);

CREATE INDEX [IX_ListaFinalita_TenantId] ON [ListaFinalita] ([TenantId]);

CREATE INDEX [IX_Purpouses_FinalitaId] ON [Purpouses] ([FinalitaId]);

CREATE INDEX [IX_Servizi_AreaId] ON [Servizi] ([AreaId]);

CREATE INDEX [IX_Servizi_SezioneId] ON [Servizi] ([SezioneId]);

CREATE INDEX [IX_Sezioni_AreaId] ON [Sezioni] ([AreaId]);

CREATE INDEX [IX_UtentiFinalita_FinalitaId] ON [UtentiFinalita] ([FinalitaId]);

CREATE INDEX [IX_UtentiTenant_TenantId] ON [UtentiTenant] ([TenantId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240513110105_Initial', N'9.0.5');

CREATE TABLE [RichiesteMassive] (
    [Id] int NOT NULL IDENTITY,
    [IdAccodamento] nvarchar(max) NOT NULL,
    [FinalitaId] int NOT NULL,
    [Stato] int NOT NULL,
    [UserID] nvarchar(450) NOT NULL,
    [TimestampCreazioneRichiesta] datetime2 NOT NULL,
    [TimestampUltimaElaborazione] datetime2 NOT NULL,
    [Ambiente] int NOT NULL,
    [TenantId] int NOT NULL,
    CONSTRAINT [PK_RichiesteMassive] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RichiesteMassive_AspNetUsers_UserID] FOREIGN KEY ([UserID]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RichiesteMassive_ListaFinalita_FinalitaId] FOREIGN KEY ([FinalitaId]) REFERENCES [ListaFinalita] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RichiesteMassive_ListaTenant_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [ListaTenant] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [ElementiRichiesteMassive] (
    [Id] bigint NOT NULL IDENTITY,
    [RichiestaMassivaId] int NOT NULL,
    [Input] nvarchar(max) NOT NULL,
    [Output] nvarchar(max) NULL,
    [Note] nvarchar(max) NOT NULL,
    [IdAccodamento] nvarchar(max) NOT NULL,
    [Tentativo] int NOT NULL,
    [TimestampCreazioneRichiesta] datetime2 NOT NULL,
    [TimestampUltimaElaborazione] datetime2 NOT NULL,
    [CodiceRisposta] int NOT NULL,
    CONSTRAINT [PK_ElementiRichiesteMassive] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ElementiRichiesteMassive_RichiesteMassive_RichiestaMassivaId] FOREIGN KEY ([RichiestaMassivaId]) REFERENCES [RichiesteMassive] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_ElementiRichiesteMassive_RichiestaMassivaId] ON [ElementiRichiesteMassive] ([RichiestaMassivaId]);

CREATE INDEX [IX_RichiesteMassive_FinalitaId] ON [RichiesteMassive] ([FinalitaId]);

CREATE INDEX [IX_RichiesteMassive_TenantId] ON [RichiesteMassive] ([TenantId]);

CREATE INDEX [IX_RichiesteMassive_UserID] ON [RichiesteMassive] ([UserID]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250204161548_RichiesteMassive', N'9.0.5');

CREATE TABLE [dbo].[DataProtectionKeys] (
    [Id] int NOT NULL IDENTITY,
    [FriendlyName] nvarchar(max) NULL,
    [Xml] nvarchar(max) NULL,
    CONSTRAINT [PK_DataProtectionKeys] PRIMARY KEY ([Id])
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250515124043_DataProtection', N'9.0.5');

COMMIT;
GO

