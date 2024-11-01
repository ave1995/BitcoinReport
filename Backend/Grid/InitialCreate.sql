IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [bitcoin_detail] (
    [Guid] uniqueidentifier NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [Note] nvarchar(max) NULL,
    CONSTRAINT [PK_bitcoin_detail] PRIMARY KEY ([Guid])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241031202326_InitialCreate', N'8.0.10');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [bitcoin_detail] ADD [Time] time NOT NULL DEFAULT '00:00:00';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241101110016_AddingTime', N'8.0.10');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [bitcoin_detail] ([Guid], [Price], [Note], [Time])
VALUES 
    (NEWID(), 1666916.83, N'Initial purchase of Bitcoin.', '09:30:00'),
    (NEWID(), 1634931.35, N'', '10:15:00'),
    (NEWID(), 1633690.43, N'', '11:00:00'),
    (NEWID(), 1632348.2, N'', '12:45:00'),
    (NEWID(), 1632981.33, N'Price reached new highs.', '14:30:00');
GO

COMMIT;
GO