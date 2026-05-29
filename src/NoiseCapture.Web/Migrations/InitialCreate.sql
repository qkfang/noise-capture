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
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260529061037_InitialCreate'
)
BEGIN
    CREATE TABLE [NoiseLogEntries] (
        [Id] int NOT NULL IDENTITY,
        [RecordedAtSydney] datetimeoffset NOT NULL,
        [Intensity] nvarchar(32) NOT NULL,
        [Loudness] nvarchar(32) NOT NULL,
        [Tone] nvarchar(32) NOT NULL,
        [Note] nvarchar(2000) NOT NULL,
        [ContinuedFromLast] bit NOT NULL,
        CONSTRAINT [PK_NoiseLogEntries] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260529061037_InitialCreate'
)
BEGIN
    CREATE TABLE [NoiseLogEntryLocations] (
        [NoiseLogEntryId] int NOT NULL,
        [SortOrder] int NOT NULL,
        [Value] nvarchar(128) NOT NULL,
        CONSTRAINT [PK_NoiseLogEntryLocations] PRIMARY KEY ([NoiseLogEntryId], [SortOrder]),
        CONSTRAINT [FK_NoiseLogEntryLocations_NoiseLogEntries_NoiseLogEntryId] FOREIGN KEY ([NoiseLogEntryId]) REFERENCES [NoiseLogEntries] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260529061037_InitialCreate'
)
BEGIN
    CREATE TABLE [NoiseLogEntryNoiseSources] (
        [NoiseLogEntryId] int NOT NULL,
        [SortOrder] int NOT NULL,
        [Value] nvarchar(128) NOT NULL,
        CONSTRAINT [PK_NoiseLogEntryNoiseSources] PRIMARY KEY ([NoiseLogEntryId], [SortOrder]),
        CONSTRAINT [FK_NoiseLogEntryNoiseSources_NoiseLogEntries_NoiseLogEntryId] FOREIGN KEY ([NoiseLogEntryId]) REFERENCES [NoiseLogEntries] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260529061037_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_NoiseLogEntries_RecordedAtSydney] ON [NoiseLogEntries] ([RecordedAtSydney]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260529061037_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260529061037_InitialCreate', N'10.0.8');
END;

COMMIT;
GO

