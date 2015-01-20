USE Startup
GO

---This SP runs every day at 12:00 am to prepare users data to report.

ALTER PROCEDURE [PrepareUsers]

@UserName nvarchar (100)
AS
BEGIN

IF @UserName IS NOT NULL
BEGIN
	UPDATE dbo.Users
	SET PrizeEntriesReported = 0, TodayPointsReceived = 0
	WHERE UserName = @UserName
END
ELSE 
	BEGIN 
		UPDATE dbo.Users
		SET PrizeEntriesReported = 0, TodayPointsReceived = 0
	END
END
