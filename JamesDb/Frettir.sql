CREATE TABLE [dbo].[Frettir]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Midill] NVARCHAR(50) NULL, 
    [Fyrirsogn] VARCHAR(120) NULL, 
    [Content] VARCHAR(500) NULL, 
    [Dagsett] DATE NULL
)
