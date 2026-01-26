-- =============================================
-- Хранимая процедура: sp_psee_ppc
-- Описание: Процедура для получения данных о потреблении энергоресурсов из АСУТП
-- Сервер: MINESRV
-- База данных: MSCADA
-- Разработано в MS SQL Server 2012 (Management Studio 14.0.17230.0)
-- =============================================

-- Параметры:
-- @days - глубина шага дней (по умолчанию 4 дня)
-- @dt - дата от которой ведется отсчет шагов в глубину (по умолчанию текущая дата)

CREATE PROCEDURE sp_psee_ppc
    @days INT = 4,
    @dt DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Если дата не указана, используем текущую дату
    IF @dt IS NULL
        SET @dt = GETDATE();
    
    -- Расчет начальной даты на основе параметров
    DECLARE @StartDate DATETIME = DATEADD(DAY, -@days, @dt);
    
    -- Основной запрос для получения данных
    SELECT 
        CAST(h.Dtm AS DATE) AS [date],           -- Метка даты
        h.Dtm AS [datetime],                     -- Метка времени
        h.Value AS [float_value],                -- Численное значение
        ISNULL(p.Dimension, '') AS [dimension],  -- Размерность
        ISNULL(p.Name, '') AS [name],            -- Наименование
        CASE 
            WHEN h.Processed = 1 THEN 'Обработано'
            ELSE 'Не обработано'
        END AS [processed_flag]                  -- Признак обработки
    FROM 
        MSCADA.dbo.MSPDB_hrs h
    LEFT JOIN 
        MSCADA.dbo.MSPDB_Params p ON h.id_param = p.id
    WHERE 
        h.Dtm >= @StartDate 
        AND h.Dtm <= @dt
    ORDER BY 
        h.Dtm DESC;
END

-- =============================================
-- Пример вызова процедуры:
-- =============================================
/*
-- Вызов с параметрами по умолчанию (4 дня от текущей даты)
EXEC sp_psee_ppc;

-- Вызов с указанием 7 дней от текущей даты
EXEC sp_psee_ppc @days = 7;

-- Вызов с указанием конкретной даты и глубины
EXEC sp_psee_ppc @days = 10, @dt = '2023-01-01';
*/
-- =============================================