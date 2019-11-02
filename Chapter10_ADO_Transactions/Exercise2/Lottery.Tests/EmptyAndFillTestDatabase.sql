USE [Lottery_ADO_Test]
GO

BEGIN TRANSACTION
BEGIN TRY
	DELETE FROM [dbo].[DrawNumbers];
	DELETE FROM [dbo].[Draws];
	DELETE FROM [dbo].[LotteryGames];

	SET IDENTITY_INSERT [LotteryGames] ON;
	INSERT INTO [LotteryGames] ([Id], [MaximumNumber], [Name], [NumberOfNumbersInADraw]) VALUES (1, 45, N'Normal', 6);
	INSERT INTO [LotteryGames] ([Id], [MaximumNumber], [Name], [NumberOfNumbersInADraw]) VALUES (2, 70, N'Edge', 2);
	INSERT INTO [LotteryGames] ([Id], [MaximumNumber], [Name], [NumberOfNumbersInADraw]) VALUES (666, 10, N'Evil', 10);
	SET IDENTITY_INSERT [LotteryGames] OFF;

	SET IDENTITY_INSERT [Draws] ON;

	-----------
	---Draws---
	-----------

	--Draws of Normal
	INSERT INTO [Draws] ([Id], [LotteryGameId], [Date]) VALUES (1, 1, '2018-11-01 15:00');
	INSERT INTO [Draws] ([Id], [LotteryGameId], [Date]) VALUES (2, 1, '2018-12-01 15:00');
	INSERT INTO [Draws] ([Id], [LotteryGameId], [Date]) VALUES (3, 1, '2019-05-01 15:00');
	INSERT INTO [Draws] ([Id], [LotteryGameId], [Date]) VALUES (4, 1, '2019-11-01 15:00');
	--Draws of Edge
	INSERT INTO [Draws] ([Id], [LotteryGameId], [Date]) VALUES (5, 2, '2019-11-01 00:00');
	SET IDENTITY_INSERT [Draws] OFF;

	-----------------
	---DrawNumbers---
	-----------------

	--Draw 1 of Normal
	INSERT INTO [DrawNumbers] ([DrawId], [Number], [Position]) VALUES (1, 5, 1);
	INSERT INTO [DrawNumbers] ([DrawId], [Number], [Position]) VALUES (1, 10, 2);
	INSERT INTO [DrawNumbers] ([DrawId], [Number], [Position]) VALUES (1, 15, 3);
	INSERT INTO [DrawNumbers] ([DrawId], [Number], [Position]) VALUES (1, 20, 4);
	INSERT INTO [DrawNumbers] ([DrawId], [Number], [Position]) VALUES (1, 25, 5);
	INSERT INTO [DrawNumbers] ([DrawId], [Number], [Position]) VALUES (1, 30, 6);

	--Draw 2 of Normal
	INSERT INTO [DrawNumbers] ([DrawId], [Number], [Position]) VALUES (2, 5, 1);
	INSERT INTO [DrawNumbers] ([DrawId], [Number], [Position]) VALUES (2, 10, 2);
	INSERT INTO [DrawNumbers] ([DrawId], [Number], [Position]) VALUES (2, 15, 3);
	INSERT INTO [DrawNumbers] ([DrawId], [Number], [Position]) VALUES (2, 20, 4);
	INSERT INTO [DrawNumbers] ([DrawId], [Number], [Position]) VALUES (2, 25, 5);
	INSERT INTO [DrawNumbers] ([DrawId], [Number], [Position]) VALUES (2, 30, 6);

	--Draw 3 of Normal
	INSERT INTO [DrawNumbers] ([DrawId], [Number], [Position]) VALUES (3, 40, 1);
	INSERT INTO [DrawNumbers] ([DrawId], [Number], [Position]) VALUES (3, 39, 2);
	INSERT INTO [DrawNumbers] ([DrawId], [Number], [Position]) VALUES (3, 38, 3);
	INSERT INTO [DrawNumbers] ([DrawId], [Number], [Position]) VALUES (3, 37, 4);
	INSERT INTO [DrawNumbers] ([DrawId], [Number], [Position]) VALUES (3, 36, 5);
	INSERT INTO [DrawNumbers] ([DrawId], [Number], [Position]) VALUES (3, 35, 6);

	--Draw 4 of Normal
	INSERT INTO [DrawNumbers] ([DrawId], [Number], [Position]) VALUES (4, 1, 1);
	INSERT INTO [DrawNumbers] ([DrawId], [Number], [Position]) VALUES (4, 2, 2);
	INSERT INTO [DrawNumbers] ([DrawId], [Number], [Position]) VALUES (4, 3, 3);
	INSERT INTO [DrawNumbers] ([DrawId], [Number], [Position]) VALUES (4, 4, 4);
	INSERT INTO [DrawNumbers] ([DrawId], [Number], [Position]) VALUES (4, 5, 5);
	INSERT INTO [DrawNumbers] ([DrawId], [Number], [Position]) VALUES (4, 6, 6);

	--Draw 1 of Edge
	INSERT INTO [DrawNumbers] ([DrawId], [Number], [Position]) VALUES (5, 11, 1);
	INSERT INTO [DrawNumbers] ([DrawId], [Number], [Position]) VALUES (5, 51, null); --used to check if null's are handled correctly
	
	COMMIT;
END TRY
BEGIN CATCH
	ROLLBACK;
END CATCH
