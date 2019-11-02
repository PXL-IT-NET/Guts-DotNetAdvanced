USE master

IF EXISTS(SELECT 1 FROM master.dbo.sysdatabases WHERE name = 'Lottery_ADO_Test') 
BEGIN
ALTER DATABASE [Lottery_ADO_Test] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
DROP DATABASE [Lottery_ADO_Test]
END
GO

CREATE DATABASE [Lottery_ADO_Test]
GO

USE [Lottery_ADO_Test]
GO

CREATE TABLE [LotteryGames] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [NumberOfNumbersInADraw] int NOT NULL,
    [MaximumNumber] int NOT NULL,
    CONSTRAINT [PK_LotteryGames] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Draws] (
    [Id] int NOT NULL IDENTITY,
    [LotteryGameId] int NOT NULL,
    [Date] datetime2 NOT NULL,
    CONSTRAINT [PK_Draws] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Draws_LotteryGames_LotteryGameId] FOREIGN KEY ([LotteryGameId]) REFERENCES [LotteryGames] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [DrawNumbers] (
    [DrawId] int NOT NULL,
    [Number] int NOT NULL,
    [Position] int NULL,
    CONSTRAINT [PK_DrawNumber] PRIMARY KEY ([DrawId], [Number]),
    CONSTRAINT [FK_DrawNumber_Draws_DrawId] FOREIGN KEY ([DrawId]) REFERENCES [Draws] ([Id]) ON DELETE CASCADE
);

GO

CREATE INDEX [IX_Draws_LotteryGameId] ON [Draws] ([LotteryGameId]);

GO