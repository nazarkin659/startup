use Startup
CREATE TABLE UsersContactInfo
(
ID int NOT NULL PRIMARY KEY identity(1,1),
UserID int NOT NULL FOREIGN KEY REFERENCES Users(userid),
ContactInfoID int NOT NULL FOREIGN KEY REFERENCES contactinfo(id)
)