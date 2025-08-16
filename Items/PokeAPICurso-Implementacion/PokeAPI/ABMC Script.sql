CREATE VIEW MODULO_POKEMON_OBTENER_POKEMONES_COMPLETO
AS
--Le pongo completo porque podriamos hacer una vista por generación
	SELECT *
		FROM Pokemones

GO
CREATE PROCEDURE MODULO_POKEMON_OBTENER_POKEMON_ID (@id_pokemon int)
AS 
BEGIN
	SELECT *
		FROM Pokemones
			WHERE id = @id_pokemon
END

GO
CREATE PROCEDURE MODULO_POKEMON_ACTUALIZAR_POKEMON(@id_pokemon int,
@nombre varchar(255),@altura decimal(18,2),@peso decimal(18,2),
@generacion int,@id_tipo_primario int, @id_tipo_secundario int NULL)
AS
BEGIN

	UPDATE Pokemones 
		SET nombre = @nombre,
			altura = @altura,
			peso = @peso,
			generacion = @generacion,
			id_tipo_primario = @id_tipo_primario,
			id_tipo_secundario = @id_tipo_secundario
			WHERE id = @id_pokemon

	SELECT *
		FROM Pokemones
			WHERE id = @id_pokemon

	--SELECT 'OK' AS 'RESULTADO'


END
GO
CREATE PROCEDURE MODULO_POKEMON_ACTUALIZAR_POKEMON_V2(@id_pokemon int,
@nombre varchar(255),@altura decimal(18,2),@peso decimal(18,2),
@generacion int,@id_tipo_primario int, @id_tipo_secundario int NULL)
AS
BEGIN

	DECLARE @nombre_actual varchar(255)

	SET @nombre_actual = NULL

	SELECT @nombre_actual=nombre
		FROM Pokemones
			WHERE id = @id_pokemon

	IF @nombre_actual IS NOT NULL
		UPDATE Pokemones 
			SET nombre = @nombre,
				altura = @altura,
				peso = @peso,
				generacion = @generacion,
				id_tipo_primario = @id_tipo_primario,
				id_tipo_secundario = @id_tipo_secundario
				WHERE id = @id_pokemon


	IF @nombre_actual IS NULL
		SELECT 'NOT FOUND' AS 'RESULTADO'
	ELSE
		SELECT 'OK' AS 'RESULTADO'


END
GO
CREATE PROCEDURE MODULO_POKEMON_INSERTAR_POKEMON(
@nombre varchar(255),@altura decimal(18,2),@peso decimal(18,2),
@generacion int,@id_tipo_primario int, @id_tipo_secundario int NULL)
AS
BEGIN

	INSERT INTO Pokemones
		VALUES(@nombre,@altura,@peso,
		@generacion,@id_tipo_primario,@id_tipo_secundario)

	SELECT TOP 1 *
		FROM Pokemones
			ORDER BY id DESC
END
GO
CREATE PROCEDURE MODULO_POKEMON_ELIMINAR_POKEMON(@id_pokemon int)
AS
BEGIN
	DELETE FROM Pokemones
		WHERE id = @id_pokemon

	SELECT *  --No deberia devolver registros
		FROM Pokemones
			WHERE id = @id_pokemon
END