-- ===================================
-- (19) Limitar horas diarias por empleado en HorasTrabajadas
-- ===================================
IF OBJECT_ID('PlaniFy.TRG_HorasTrabajadas_LimiteDiario', 'TR') IS NOT NULL
    DROP TRIGGER PlaniFy.TRG_HorasTrabajadas_LimiteDiario;
GO

CREATE TRIGGER PlaniFy.TRG_HorasTrabajadas_LimiteDiario
ON PlaniFy.HorasTrabajadas
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT *
        FROM (
            SELECT ht.idEmpleado,
                   ht.Fecha,
                   SUM(ht.Cantidad) AS TotalHoras
            FROM PlaniFy.HorasTrabajadas ht
            INNER JOIN (
                SELECT DISTINCT idEmpleado, Fecha
                FROM inserted
            ) cambios
                ON cambios.idEmpleado = ht.idEmpleado
               AND cambios.Fecha = ht.Fecha
            GROUP BY ht.idEmpleado, ht.Fecha
        ) AS TotalesDiarios
        WHERE TotalesDiarios.TotalHoras > 8
    )
    BEGIN
        RAISERROR('No se pueden registrar más de 8 horas por día para un mismo empleado.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END;
GO
