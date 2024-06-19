DECLARE @columns AS NVARCHAR(MAX), @sql AS NVARCHAR(MAX);

-- Создаем список столбцов с префиксом 'S', используемый в PIVOT
SELECT @columns = STUFF(
    (
        SELECT ', ' + QUOTENAME('S' + CAST(id AS VARCHAR(10)))
        FROM (
            SELECT TOP 10000 id FROM MSCADA.dbo.MSPDB_Params ORDER BY id
        ) AS filtered_ids
        FOR XML PATH(''), TYPE
    ).value('.', 'NVARCHAR(MAX)'), 
    1, 2, ''
);

-- Создаем динамический SQL
SET @sql = '
WITH DataPrep AS (
    SELECT
        h.Dtm,
        ''S'' + CAST(p.id AS VARCHAR(10)) AS id_params,
        h.Value,
        ROW_NUMBER() OVER (PARTITION BY p.id, h.Dtm ORDER BY h.Dtm DESC) AS rn
    FROM MSCADA.dbo.MSPDB_Params_Desc d
    JOIN MSCADA.dbo.MSPDB_hrs h ON d.id_params = h.id_param
    JOIN MSCADA.dbo.MSPDB_dat dat on d.id_params = dat.id_param
    JOIN MSCADA.dbo.MSPDB_Params p ON d.id_params = p.id
    WHERE p.id IN (SELECT TOP 20 id FROM MSCADA.dbo.MSPDB_Params ORDER BY id)
)
SELECT * FROM
(
    SELECT Dtm, id_params, Value FROM DataPrep WHERE rn = 1
) AS SourceTable
PIVOT
(
    MAX(Value)
    FOR id_params IN (' + @columns + ')
) AS PivotTable
ORDER BY Dtm;
';

EXEC sp_executesql @sql;
