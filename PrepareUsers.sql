USE Startup
GO

---This SP runs every day at 12:00 am to prepare users data to report.

ALTER PROCEDURE [PrepareUsers]

@UserName nvarchar (100) = null
AS
BEGIN

IF @UserName IS NOT NULL
BEGIN
	UPDATE dbo.Users
	SET PrizeEntriesReported = 0, TodayPointsReceived = 0,PrizesToReport = 1
	WHERE UserName = @UserName
END
ELSE 
	BEGIN 
		UPDATE dbo.Users
		SET PrizeEntriesReported = 0, TodayPointsReceived = 0, PrizesToReport = 1
	END
END
