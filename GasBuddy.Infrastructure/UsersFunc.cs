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
            try
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
            }
            catch (Exception e)
            {
                //SiAuto.Main.LogException(e);
            }
            return false;
        }

        public static bool AddUser(User user)
        {
            return AddUser(new List<User> { user });
        }

        public static bool UpdateUser(List<User> users)
        {
            try
            {
                if (!users.IsNullOrEmpty())
                {
                    using (var db = new GasBuddy.Infrastructure.Base.Db())
                    {
                        foreach (User user in users)
                        {
                            db.Users.Attach(user);
                            db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                        }

                        db.SaveChanges();
                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                //SiAuto.Main.LogException(e);
            }
            return false;
        }
        public static bool UpdateUser(User user)
        {
            return UpdateUser(new List<User> { user });
        }

        public static List<User> GetUsers(int count = 50)
        {
            List<User> users = null;

            using (var db = new Db())
            {
                users = db.Users.Take(count).ToList();
            }

            return users;
        }
    }
}
