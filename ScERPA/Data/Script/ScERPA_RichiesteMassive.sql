BEGIN TRANSACTION;
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
VALUES (N'20250204161548_RichiesteMassive', N'9.0.0');

COMMIT;
GO

