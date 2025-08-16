CREATE TABLE [dbo].[ResultadosPelea]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY(0,1),
	[nombre_resultado] varchar(255) NOT NULL,
	[descripcion] varchar(255) NOT NULL,
	[sigla] varchar(255) NOT NULL
)
GO
SET IDENTITY_INSERT [dbo].ResultadosPelea ON 

INSERT [dbo].ResultadosPelea ([id], [nombre_resultado],[descripcion],[sigla]) VALUES (0, N'Ganador Local',	N'Ocurre cuando gana el entrenador local',N'L')
INSERT [dbo].ResultadosPelea ([id], [nombre_resultado],[descripcion],[sigla]) VALUES (1, N'Ganador Visitante',	N'Ocurre cuando gana el entrenador visitante',N'V')
INSERT [dbo].ResultadosPelea ([id], [nombre_resultado],[descripcion],[sigla]) VALUES (2, N'Empate',N'Ocurre cuando ambos entrenadores se quedan sin pokemones al mismo tiempo',N'E')
INSERT [dbo].ResultadosPelea ([id], [nombre_resultado],[descripcion],[sigla]) VALUES (4, N'Cancelado',	N'El combate se cancelo por acuerdo de los entrenadores',N'C')
INSERT [dbo].ResultadosPelea ([id], [nombre_resultado],[descripcion],[sigla]) VALUES (5, N'Programado',	N'El combate esta programado y aun no comenzo',N'P')
INSERT [dbo].ResultadosPelea ([id], [nombre_resultado],[descripcion],[sigla]) VALUES (6, N'Equipo Rocket',	N'El equipo Rocket (Jessie-James-Meowth) nuevamente irrumpio el combate por lo que queda inconcluso',N'R')

SET IDENTITY_INSERT [dbo].ResultadosPelea OFF
GO
CREATE TABLE [dbo].[Combates] (
    [id]                   INT         NOT NULL IDENTITY (0, 1),
    [id_entrenador_local]  INT         NOT NULL,
    [id_entrenador_visita] INT         NOT NULL,
    [fecha_combate]        DATE        NOT NULL,
    [inicio_combate]       TIME (0)    NOT NULL,
    [final_combate]        TIME (0)    NULL,
    [resultado_combate]    INT         NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_EntrenadorLocal] FOREIGN KEY ([id_entrenador_local]) REFERENCES [dbo].[Entrenadores] ([id]),
    CONSTRAINT [FK_EntrenadorVisita] FOREIGN KEY ([id_entrenador_visita]) REFERENCES [dbo].[Entrenadores] ([id]),
	CONSTRAINT [FK_CombateResultado] FOREIGN KEY (resultado_combate) REFERENCES [dbo].ResultadosPelea ([id])
);
GO
CREATE TABLE [dbo].[Equipos] (
    [id]            INT IDENTITY (0, 1) NOT NULL,
    [id_pokemon]    INT NOT NULL,
    [id_entrenador] INT NOT NULL,
	[id_combate]	INT NOT NULL,
    CONSTRAINT [PK_Equipos] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_Equipos_Entrenadores] FOREIGN KEY ([id_entrenador]) REFERENCES [dbo].[Entrenadores] ([id]),
    CONSTRAINT [FK_Equipos_Pokemones] FOREIGN KEY ([id_pokemon]) REFERENCES [dbo].[Pokemones] ([id]),
	CONSTRAINT [FK_Equipos_Combate] FOREIGN KEY ([id_combate]) REFERENCES  [dbo].Combates ([id])
);
GO
CREATE PROCEDURE MODULO_COMBATE_INSERTAR_COMBATE(
	@id_entrenador_local int,
	@id_entrenador_visita int,
	@fecha_combate date,
	@inicio_combate time(0)
)
AS
BEGIN

	-- Los ultimos valores siempre son los mismos cuando se empieza
	-- El final del combate es NULL y el id de resultado es programado
	INSERT INTO Combates
		VALUES(@id_entrenador_local,@id_entrenador_visita,@fecha_combate,@inicio_combate,NULL,5)

	SELECT TOP 1 *
		FROM Combates
			ORDER BY id DESC
END
GO
ALTER PROCEDURE MODULO_COMBATE_INSERTAR_EQUIPO(@id_pokemon int,@id_entrenador int, @id_combate int)
AS
BEGIN
	INSERT INTO Equipos
		VALUES(@id_pokemon,@id_entrenador, @id_combate)

	SELECT TOP 1 *
		FROM Equipos
			ORDER BY id DESC
END
GO

CREATE PROCEDURE MODULO_COMBATE_INSERTAR_EQUIPO_TRANSAC(@id_pokemon int,@id_entrenador int)
AS
BEGIN

	--Este metodo lo que hace es asumir que queremos cargar los equipos del ultimo combate
	--Con esto evitamos la manipulacion desde C# de variables y mandamos a actualizar
	DECLARE @id_combate INT

	SELECT TOP 1 @id_combate = id
		FROM Combates
			ORDER BY id DESC

	INSERT INTO Equipos
		VALUES(@id_pokemon,@id_entrenador, @id_combate)

	SELECT 'OK' AS 'Codigo'

END
GO

CREATE VIEW MODULO_COMBATE_RECUPERAR_ULTIMO_COMBATE
AS
	SELECT TOP 1 *
		FROM Combates
			ORDER BY id DESC