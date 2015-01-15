namespace GasBuddy.Infrastructure.Base
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using GasBuddy.Model;
    using System.Data.Entity.Core.Objects;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure;

    public partial class Db : DbContext
    {
        public Db()
            : base("name=Db")
        {
        }

        public virtual DbSet<ContactInfo> ContactInfo { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UsersContactInfo> UsersContactInfo { get; set; }
        public virtual DbSet<Mobile> Mobile { get; set; }
        public virtual DbSet<WebSite> WebSite { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }


        public override int SaveChanges()
        {
            ObjectContext context = ((IObjectContextAdapter)this).ObjectContext;

            //Find all Entities that are Added/Modified that inherit from my EntityBase
            IEnumerable<ObjectStateEntry> objectStateEntries =
                from e in context.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Modified)
                where
                    e.IsRelationship == false &&
                    e.Entity != null &&
                    typeof(User).IsAssignableFrom(e.Entity.GetType())
                select e;

            var currentTime = DateTime.Now;

            foreach (var entry in objectStateEntries)
            {
                var entityBase = entry.Entity as User;

                if (entry.State == EntityState.Added)
                {
                    entityBase.CreatedDate = currentTime;
                }

                entityBase.LastModifiedDate = currentTime;
            }

            return base.SaveChanges();
        }
    }
}
