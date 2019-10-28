CREATE TABLE [dbo].[Said]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [PrimaryTopic] VARCHAR(50) NULL, 
    [GoogleSpeechText] VARCHAR(300) NULL, 
    [AnswerIntent] VARCHAR(100) NULL, 
    [DateTime] DATETIME NULL
)
