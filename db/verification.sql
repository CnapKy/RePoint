-- Read-only verification after applying the EF Core migration to RePointDb.
SET NOCOUNT ON;

SELECT MigrationId, ProductVersion FROM dbo.__EFMigrationsHistory ORDER BY MigrationId;
SELECT name AS TableName FROM sys.tables ORDER BY name;
SELECT COUNT(*) AS TableCount FROM sys.tables;

SELECT OBJECT_NAME(parent_object_id) AS TableName, name AS PrimaryKey
FROM sys.key_constraints WHERE type = 'PK' ORDER BY TableName;
SELECT OBJECT_NAME(parent_object_id) AS TableName, name AS ForeignKey,
       OBJECT_NAME(referenced_object_id) AS ReferencedTable, delete_referential_action_desc AS OnDelete
FROM sys.foreign_keys ORDER BY TableName, ForeignKey;
SELECT OBJECT_NAME(parent_object_id) AS TableName, name, definition
FROM sys.check_constraints ORDER BY TableName, name;
SELECT OBJECT_NAME(i.object_id) AS TableName, i.name AS IndexName, c.name AS ColumnName, i.is_unique
FROM sys.indexes i
JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
JOIN sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
WHERE i.name IN ('IX_ConfirmedDeliveries_HandoverRequestId', 'IX_Materials_Code')
ORDER BY i.name, ic.key_ordinal;

SELECT Name, Code FROM dbo.Materials ORDER BY Id;
SELECT Name, City, Address FROM dbo.CollectionPoints ORDER BY Id;
SELECT cp.Name AS CollectionPointName, m.Name AS MaterialName, pm.IsAccepted
FROM dbo.PointMaterials pm
JOIN dbo.CollectionPoints cp ON cp.Id = pm.CollectionPointId
JOIN dbo.Materials m ON m.Id = pm.MaterialId
ORDER BY pm.CollectionPointId, pm.MaterialId;
SELECT Name, NormalizedName FROM dbo.AspNetRoles ORDER BY Name;
