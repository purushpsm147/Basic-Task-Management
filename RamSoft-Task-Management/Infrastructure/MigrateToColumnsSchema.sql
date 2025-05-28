-- Database Migration Script for Adding Columns Support to Tasks

-- Create Columns table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Columns')
BEGIN
    CREATE TABLE [dbo].[Columns] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(50) NOT NULL,
        [Description] NVARCHAR(MAX) NULL,
        [Order] INT NOT NULL,
        [Color] NVARCHAR(50) NULL,
        [IsDefault] BIT NOT NULL DEFAULT 0
    );

    -- Insert default columns based on existing task statuses
    INSERT INTO [dbo].[Columns] ([Name], [Order], [Color], [IsDefault])
    VALUES 
        ('Unassigned', 1, '#808080', 1),
        ('Approved', 2, '#FFA500', 0),
        ('In Progress', 3, '#1E90FF', 0),
        ('Done', 4, '#32CD32', 0);
END

-- Add ColumnId and Position columns to JiraTask
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('JiraTask') AND name = 'ColumnId')
BEGIN
    ALTER TABLE [dbo].[JiraTask]
    ADD [ColumnId] INT NULL,
        [Position] INT NOT NULL DEFAULT 0;
    
    -- Add foreign key constraint
    ALTER TABLE [dbo].[JiraTask]
    ADD CONSTRAINT [FK_JiraTask_Column] 
        FOREIGN KEY ([ColumnId]) 
        REFERENCES [dbo].[Columns] ([Id])
        ON DELETE SET NULL;
        
    -- Map existing tasks to corresponding columns based on Status
    UPDATE [dbo].[JiraTask] 
    SET [ColumnId] = 
        CASE [Status]
            WHEN 0 THEN 1  -- Unassigned maps to Column 1
            WHEN 1 THEN 2  -- Approved maps to Column 2
            WHEN 2 THEN 3  -- InProgress maps to Column 3
            WHEN 3 THEN 4  -- Done maps to Column 4
            ELSE 1         -- Default to Unassigned
        END;
    
    -- Set initial positions based on creation order within each column
    -- This CTE assigns position numbers within each column group
    WITH TaskPositions AS (
        SELECT 
            Id,
            ColumnId,
            ROW_NUMBER() OVER (PARTITION BY ColumnId ORDER BY Id) AS Position
        FROM [dbo].[JiraTask]
    )
    UPDATE j
    SET j.Position = tp.Position
    FROM [dbo].[JiraTask] j
    INNER JOIN TaskPositions tp ON j.Id = tp.Id;
END
