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

CREATE TABLE [Habilidades] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(max) NULL,
    [Dano] int NOT NULL,
    CONSTRAINT [PK_Habilidades] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Usuarios] (
    [Id] int NOT NULL IDENTITY,
    [Username] nvarchar(max) NULL,
    [PasswordHash] varbinary(max) NULL,
    [PasswordSalt] varbinary(max) NULL,
    [Foto] varbinary(max) NULL,
    [Latitude] nvarchar(max) NULL,
    [Longitude] nvarchar(max) NULL,
    [DataAcesso] datetime2 NULL,
    CONSTRAINT [PK_Usuarios] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Personagens] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(max) NULL,
    [PontosVida] int NOT NULL,
    [Forca] int NOT NULL,
    [Defesa] int NOT NULL,
    [Inteligencia] int NOT NULL,
    [Classe] int NOT NULL,
    [FotoPersonagem] varbinary(max) NULL,
    [UsuarioId] int NULL,
    CONSTRAINT [PK_Personagens] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Personagens_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Armas] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(max) NULL,
    [Dano] int NOT NULL,
    [PersonagemId] int NOT NULL,
    CONSTRAINT [PK_Armas] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Armas_Personagens_PersonagemId] FOREIGN KEY ([PersonagemId]) REFERENCES [Personagens] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [PersonagemHabilidades] (
    [PersonagemId] int NOT NULL,
    [HabilidadeId] int NOT NULL,
    CONSTRAINT [PK_PersonagemHabilidades] PRIMARY KEY ([PersonagemId], [HabilidadeId]),
    CONSTRAINT [FK_PersonagemHabilidades_Habilidades_HabilidadeId] FOREIGN KEY ([HabilidadeId]) REFERENCES [Habilidades] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PersonagemHabilidades_Personagens_PersonagemId] FOREIGN KEY ([PersonagemId]) REFERENCES [Personagens] ([Id]) ON DELETE CASCADE
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Dano', N'Nome') AND [object_id] = OBJECT_ID(N'[Habilidades]'))
    SET IDENTITY_INSERT [Habilidades] ON;
INSERT INTO [Habilidades] ([Id], [Dano], [Nome])
VALUES (1, 39, N'Adormecer'),
(2, 41, N'Congelar'),
(3, 37, N'Hipnotizar');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Dano', N'Nome') AND [object_id] = OBJECT_ID(N'[Habilidades]'))
    SET IDENTITY_INSERT [Habilidades] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Classe', N'Defesa', N'Forca', N'FotoPersonagem', N'Inteligencia', N'Nome', N'PontosVida', N'UsuarioId') AND [object_id] = OBJECT_ID(N'[Personagens]'))
    SET IDENTITY_INSERT [Personagens] ON;
INSERT INTO [Personagens] ([Id], [Classe], [Defesa], [Forca], [FotoPersonagem], [Inteligencia], [Nome], [PontosVida], [UsuarioId])
VALUES (1, 1, 23, 17, NULL, 33, N'Frodo', 100, NULL),
(2, 1, 25, 15, NULL, 30, N'Sam', 100, NULL),
(3, 3, 21, 18, NULL, 35, N'Galadriel', 100, NULL),
(4, 2, 18, 18, NULL, 37, N'Gandalf', 100, NULL),
(5, 1, 17, 20, NULL, 31, N'Hobbit', 100, NULL),
(6, 3, 13, 21, NULL, 34, N'Celeborn', 100, NULL),
(7, 2, 11, 25, NULL, 35, N'Radagast', 100, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Classe', N'Defesa', N'Forca', N'FotoPersonagem', N'Inteligencia', N'Nome', N'PontosVida', N'UsuarioId') AND [object_id] = OBJECT_ID(N'[Personagens]'))
    SET IDENTITY_INSERT [Personagens] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'DataAcesso', N'Foto', N'Latitude', N'Longitude', N'PasswordHash', N'PasswordSalt', N'Username') AND [object_id] = OBJECT_ID(N'[Usuarios]'))
    SET IDENTITY_INSERT [Usuarios] ON;
INSERT INTO [Usuarios] ([Id], [DataAcesso], [Foto], [Latitude], [Longitude], [PasswordHash], [PasswordSalt], [Username])
VALUES (1, NULL, NULL, NULL, NULL, 0x68A9E4DC932AC33D40E22530674B3E3F80CE701BDBD185B3AD743C06E973A5E849E8825A639AF741DE03FC68EA0E817D15BAB0E81242B86D6DF47D8ACD1A7438, 0x86D20D5F57341DE5861E85F65552048A733CA330900BDB1DEF45B4B04BA403ADF2BE03557424126536E8946A33B7B8D1CDF26AD9F6E68D3F9F3B7AB201DFC22D8F581DD0ED617659E7CF59CBB47E8078EF7D5D74796B9FB01F5B7FDF0939CDF0EDE84B477143596447410FF782DC53926CBE5AE78FDD34A0775B6EA669B66757, N'UsuarioAdmin');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'DataAcesso', N'Foto', N'Latitude', N'Longitude', N'PasswordHash', N'PasswordSalt', N'Username') AND [object_id] = OBJECT_ID(N'[Usuarios]'))
    SET IDENTITY_INSERT [Usuarios] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Dano', N'Nome', N'PersonagemId') AND [object_id] = OBJECT_ID(N'[Armas]'))
    SET IDENTITY_INSERT [Armas] ON;
INSERT INTO [Armas] ([Id], [Dano], [Nome], [PersonagemId])
VALUES (1, 35, N'Arco e Flecha', 1),
(2, 33, N'Espada', 2),
(3, 31, N'Machado', 3);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Dano', N'Nome', N'PersonagemId') AND [object_id] = OBJECT_ID(N'[Armas]'))
    SET IDENTITY_INSERT [Armas] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'HabilidadeId', N'PersonagemId') AND [object_id] = OBJECT_ID(N'[PersonagemHabilidades]'))
    SET IDENTITY_INSERT [PersonagemHabilidades] ON;
INSERT INTO [PersonagemHabilidades] ([HabilidadeId], [PersonagemId])
VALUES (1, 1),
(2, 1),
(2, 2),
(2, 3),
(3, 3),
(3, 4),
(1, 5),
(2, 6),
(3, 7);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'HabilidadeId', N'PersonagemId') AND [object_id] = OBJECT_ID(N'[PersonagemHabilidades]'))
    SET IDENTITY_INSERT [PersonagemHabilidades] OFF;
GO

CREATE UNIQUE INDEX [IX_Armas_PersonagemId] ON [Armas] ([PersonagemId]);
GO

CREATE INDEX [IX_PersonagemHabilidades_HabilidadeId] ON [PersonagemHabilidades] ([HabilidadeId]);
GO

CREATE INDEX [IX_Personagens_UsuarioId] ON [Personagens] ([UsuarioId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20220414232421_MigrationGeral', N'5.0.15');
GO

COMMIT;
GO

