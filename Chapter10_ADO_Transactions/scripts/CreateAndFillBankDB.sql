USE master

IF EXISTS(SELECT 1 FROM master.dbo.sysdatabases WHERE name = 'BankDB_ADO') 
BEGIN
ALTER DATABASE [BankDB_ADO] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
DROP DATABASE [BankDB_ADO]
END
GO

CREATE DATABASE [BankDB_ADO]
GO

USE [BankDB_ADO]
GO
/****** Object:  Table [dbo].[Accounts]    Script Date: 7/09/2018 9:32:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Accounts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AccountNumber] [nvarchar](max) NULL,
	[Balance] [decimal](18, 2) NOT NULL,
	[AccountType] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Accounts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cities](
	[ZipCode] [int] NOT NULL,
	[Name] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Cities] PRIMARY KEY CLUSTERED 
(
	[ZipCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customers](
	[CustomerId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[FirstName] [nvarchar](max) NULL,
	[Address] [nvarchar](max) NULL,
	[CellPhone] [nvarchar](max) NULL,
	[ZipCode] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Customers] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Accounts]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Accounts_dbo.Customers_CustomerId] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Accounts] CHECK CONSTRAINT [FK_dbo.Accounts_dbo.Customers_CustomerId]
GO
ALTER TABLE [dbo].[Customers]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Customers_dbo.Cities_ZipCode] FOREIGN KEY([ZipCode])
REFERENCES [dbo].[Cities] ([ZipCode])
GO
ALTER TABLE [dbo].[Customers] CHECK CONSTRAINT [FK_dbo.Customers_dbo.Cities_ZipCode]
GO

BEGIN TRANSACTION
BEGIN TRY
	INSERT INTO [dbo].[Cities] (Zipcode, Name) VALUES (3770, 'Riemst');
	INSERT INTO [dbo].[Cities] (Zipcode, Name) VALUES (3700, 'Tongeren');
	INSERT INTO [dbo].[Cities] (Zipcode, Name) VALUES (3740, 'Bilzen');
	INSERT INTO [dbo].[Cities] (Zipcode, Name) VALUES (3500, 'Hasselt');
	INSERT INTO [dbo].[Cities] (Zipcode, Name) VALUES (3600, 'Genk');
	INSERT INTO [dbo].[Customers] ([Name], FirstName, [Address], ZipCode, CellPhone) VALUES ('Willems', 'Marijke', 'Beemdstraat 10', 3770, '0499/12.32.54');
	INSERT INTO [dbo].[Customers] ([Name], FirstName, [Address], ZipCode, CellPhone) VALUES ('Hendrikx', 'Wesley', 'Vaartstraat 31', 3500, '0494/16.54.87');
	INSERT INTO [dbo].[Customers] ([Name], FirstName, [Address], ZipCode, CellPhone) VALUES ('Hermans', 'Kris', 'Overweg 115', 3700, '0476/12.82.15');
	INSERT INTO [dbo].[Accounts] (AccountNumber, Balance, AccountType, CustomerId) VALUES ('123-654651-156', 2500, 1, 1);
	INSERT INTO [dbo].[Accounts] (AccountNumber, Balance, AccountType, CustomerId) VALUES ('351-854321-123', 500, 2, 1);
	INSERT INTO [dbo].[Accounts] (AccountNumber, Balance, AccountType, CustomerId) VALUES ('546-642135-546', 3000, 4, 2);
	INSERT INTO [dbo].[Accounts] (AccountNumber, Balance, AccountType, CustomerId) VALUES ('632-126845-741', 4500, 1, 3);
	COMMIT;
END TRY
BEGIN CATCH
	ROLLBACK;
END CATCH
