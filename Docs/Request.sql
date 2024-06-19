DECLARE @columns AS NVARCHAR(MAX), @sql AS NVARCHAR(MAX);

-- Создаем список столбцов с префиксом 'S', используемый в PIVOT
SELECT @columns = STUFF(
    (
        SELECT ', ' + QUOTENAME('S' + CAST(id AS VARCHAR(10)))
        FROM (
            SELECT id FROM MSCADA.dbo.MSPDB_Params WHERE id IN (SELECT value FROM STRING_SPLIT(@selected_ids, ','))
        ) AS filtered_ids
        FOR XML PATH(''), TYPE
    ).value('.', 'NVARCHAR(MAX)'), 
    1, 2, ''
);

-- Создаем динамический SQL
SET @sql = '
WITH DataPrep AS (
    SELECT
        h.Dtm AS [DataTime],
        DAY(h.Dtm) AS [Day],
        MONTH(h.Dtm) AS [Month],
        YEAR(h.Dtm) AS [Year],
        DATEPART(HOUR, h.Dtm) AS [Hour],
        DATEPART(MINUTE, h.Dtm) AS [Minute],
        ''S'' + CAST(p.id AS VARCHAR(10)) AS id_params,
		CASE WHEN h.Value IS NULL THEN ''NA'' ELSE CAST(h.Value AS VARCHAR(255)) END AS Value,
        ROW_NUMBER() OVER (PARTITION BY p.id, h.Dtm ORDER BY h.Dtm DESC) AS rn
    FROM MSCADA.dbo.MSPDB_Params_Desc d
    JOIN MSCADA.dbo.MSPDB_hrs h ON d.id_params = h.id_param
    JOIN MSCADA.dbo.MSPDB_dat dat on d.id_params = dat.id_param
    JOIN MSCADA.dbo.MSPDB_Params p ON d.id_params = p.id
    WHERE p.id IN (SELECT value FROM STRING_SPLIT(@selected_ids, '','')) AND h.Dtm BETWEEN @start_date AND @end_date
)
SELECT * FROM
(
    SELECT [DataTime], [Day], [Month], [Year], [Hour], [Minute], id_params, Value FROM DataPrep WHERE rn = 1
) AS SourceTable
PIVOT
(
    MAX(Value)
    FOR id_params IN (' + @columns + ')
) AS PivotTable
ORDER BY [Year], [Month], [Day], [Hour], [Minute];
';

EXEC sp_executesql @sql, N'@selected_ids NVARCHAR(MAX), @start_date DATETIME, @end_date DATETIME', @selected_ids, @start_date, @end_date;