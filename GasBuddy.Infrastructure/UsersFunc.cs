using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using GasBuddy.Model;
using HelperFunctions;
using GasBuddy.Infrastructure.Base;

namespace GasBuddy.Infrastructure
{
    public class UsersFunc : Db
    {

        public static bool AddUser(List<User> users)
        {
            if (!users.IsNullOrEmpty())
            {
                using (var db = new GasBuddy.Infrastructure.Base.Db())
                {
                    db.Users.AddRange(users);
                    db.SaveChanges();
                }

                return true;
            }
            return false;
        }

        public static bool AddUser(User user)
        {
            return AddUser(new List<User> { user });
        }

        public static bool UpdateUser(List<User> users)
        {
            if (!users.IsNullOrEmpty())
            {
                using (var db = new GasBuddy.Infrastructure.Base.Db())
                {
                    foreach (User user in users)
                    {

                        user.Mobile.User = null;
                        user.Website.User = null;

                        db.Users.Attach(user);
                        db.Entry<Mobile>(user.Mobile).State = EntityState.Modified;
                        db.Entry<WebSite>(user.Website).State = EntityState.Modified;
                        db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    }

                    db.SaveChanges();
                }
                return true;
            }
            return false;
        }
        public static bool UpdateUser(User user)
        {
            return UpdateUser(new List<User> { user });
        }

        public static List<User> GetUsers(int count = 50)
        {
            List<User> users = new List<User>();
            using (var db = new Db())
            {
                foreach (User u in db.Users.Take(count))
                {
                    User newU = new User();
                    newU.UserID = u.UserID;
                    newU.UserName = u.UserName;
                    newU.Website = u.Website;
                    newU.Password = u.Password;
                    newU.Mobile = u.Mobile;
                    newU.LastModifiedDate = u.LastModifiedDate;
                    newU.CreatedDate = u.CreatedDate;
                    newU.TodayPointsReceived = u.TodayPointsReceived;
                    newU.PrizeEntriesReported = u.PrizeEntriesReported;
                    newU.PrizesToReport = u.PrizesToReport;

                    users.Add(newU);
                }

                return users;
            }
        }

        public static User GetUser(string userName)
        {
            User user = null;
            using (var db = new Db())
            {
                user = db.Users.Where(u => string.Equals(u.UserName, userName)).Include("Mobile").Include("WebSite").FirstOrDefault();
            }
            return user;
        }

        public static ContactInfo GetUserContactInfo(int userID)
        {
            ContactInfo cInfo = null;

            using (var db = new Db())
            {
                int? contactInfoID = db.UsersContactInfo.Where(o => o.UserID == userID).Select(o => o.ContactInfoID).FirstOrDefault();
                if (contactInfoID != null)
                {
                    cInfo = db.ContactInfo.Find(contactInfoID);
                }
            }

            return cInfo;
        }
    }
}
