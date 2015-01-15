USE Startup
GO

ALTER PROCEDURE dbo.AddUser
(
@UserName varchar(20),
@Password varchar(20),
@Email nvarchar(100),
@PrizesToReport int = 1,
@TodayPointsReceived int = 1000
)
AS 
BEGIN

IF EXISTS( SELECT 1 FROM Startup.dbo.Users WHERE UserName = @UserName)
begin
THROW 50001,'USER already EXISTS.',1;
end

INSERT INTO dbo.Users(UserName,Password,CreatedDate,PrizesToReport,TodayPointsReceived)
Values(@UserName,@Password,Getdate(),@PrizesToReport,@TodayPointsReceived)

declare @userID int
SELECT @userID = USERID from Users
where UserName = @UserName

INSERT INTO dbo.MOBILE(CheckLoginURL,MobileID)
VALUES('https://m.gasbuddy.com/index.aspx',@userID)

INSERT INTO dbo.WEBSITE(CheckLoginURL,WebID)
VALUES('http://www.chicagogasprices.com/',@userID)

INSERT INTO dbo.ContactInfo(FirstName,LastName,Address,Unit,State,ZipCode,Email,City)
VALUES('Nazar','Petriv','4049 N Kedvale Ave','APT 48','IL',60641,@Email,'Chicago')

declare @ContacInfoID int
SELECT @ContacInfoID = ID FROM dbo.ContactInfo WHERE Email = @Email

insert into UsersContactInfo(UserID,ContactInfoID)
Select UserID, @ContacInfoID FROM dbo.Users where UserName = @UserName

END