USE [ScERPA_db]
GO

-- =======================================================
-- Create Stored Procedure
-- =======================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =================================================================================
-- Author:      <Flamigni, Stefano>
-- Description: <stored ad uso di IDM per la creazione di un utente>
-- =================================================================================
CREATE PROCEDURE [dbo].[IDM_Utente_Crea]
(
	@Username Varchar(256),
	@Cognome Varchar(250),
	@Nome Varchar(250),
	@Email varchar(255),
	@Attivo Bit = 1
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON
	IF not exists (select 1 from dbo.AspNetUsers where Username = @Username)
		BEGIN
			INSERT INTO dbo.AspNetUsers(
				Id,
				Nome,
				Cognome,
				Attivo,
				UserName,
				NormalizedUserName,
				Email,
				NormalizedEmail,			
				EmailConfirmed,
				PhoneNumberConfirmed,
				TwoFactorEnabled,
				LockoutEnabled,
				AccessFailedCount,
				SecurityStamp,
				ConcurrencyStamp
				)
			VALUES (
				NEWID(),
				@Nome,
				@Cognome,
				@Attivo,
				@Username,
				UPPER(@Username),
				@Email,
				UPPER(@Email),
				1,
				0,
				0,
				1,
				0,
				NEWID(),
				NEWID()
			)
		END
	ELSE
		BEGIN
			UPDATE dbo.AspNetUsers
			set 
				Cognome = @Cognome,
				Nome = @Nome,
				Attivo = @Attivo,
				Email = @Email,
				NormalizedUserName = UPPER(@Email)
			where Username = @Username
		END
END
GO


-- ======================================================================================
-- Author:      <Flamigni, Stefano>
-- Description: <stored ad uso di IDM la lettura dei dati di un utente>
-- ======================================================================================

CREATE PROCEDURE [dbo].[IDM_Utente_Carica]
(
	@Username Varchar(256)
)
AS
BEGIN
	SELECT 
		 [Username]
		,[Cognome]
		,[Nome]
		,[Email]
		,[Attivo]
	FROM 
		dbo.AspNetUsers
	WHERE 
		Username = @Username
END
GO

-- ======================================================================================
-- Author:      <Flamigni, Stefano>
-- Description: <stored ad uso di IDM la ricerca utenti>
-- ======================================================================================

CREATE PROCEDURE [dbo].[IDM_Utente_Elenca]
(
	@Username Varchar(256)=NULL
)
AS
BEGIN
	SELECT 
		 [Username]
		,[Cognome]
		,[Nome]
		,[Email]
		,[Attivo]
	FROM 
		dbo.AspNetUsers
	WHERE 
		(@Username IS NULL OR @Username IS NOT NULL AND Username LIKE '%'+@Username+'%')
	ORDER BY  Username	
END
GO

-- ======================================================================================
-- Author:      <Flamigni, Stefano>
-- Description: <stored ad uso di IDM la disattivazione utente>
-- ======================================================================================
CREATE PROCEDURE [dbo].[IDM_Utente_Elimina]
(
	@Username Varchar(256)
)
AS
BEGIN

	UPDATE dbo.AspNetUsers
	SET Attivo = 0
	WHERE Username = @Username

END
GO


-- ======================================================================================
-- Author:      <Flamigni, Stefano>
-- Description: <stored ad uso di IDM per attivare/disattivare un utente>
-- ======================================================================================
CREATE PROCEDURE [dbo].[IDM_Utente_Modifica_IsAttivo]
(
	@Username Varchar(256),
	@Attivo bit
)
AS
BEGIN

	UPDATE dbo.AspNetUsers
	SET
		Attivo = @Attivo
	WHERE 
		Username = @Username
END
GO