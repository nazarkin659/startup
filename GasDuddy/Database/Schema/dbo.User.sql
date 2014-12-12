CREATE TABLE [dbo].[Table]
(
	[UserID] INT NOT NULL PRIMARY KEY, 
    [UserName] NCHAR(18) NOT NULL, 
    [Password] NCHAR(18) NOT NULL, 
    [isLoggedIn] BIT NULL, 
    [Cookies] TEXT NULL
)
