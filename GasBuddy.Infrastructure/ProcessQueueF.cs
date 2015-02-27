using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GasBuddy.Infrastructure.Base;
using HelperFunctions;

namespace GasBuddy.Infrastructure
{
    public static class ProcessQueueF
    {
        private static Db dbConnection = null;
        public static bool AddRecord(Model.ProcessQueue queue)
        {
            return AddRecord(new List<Model.ProcessQueue> { queue });
        }

        public static bool AddRecord(List<Model.ProcessQueue> queues)
        {
            if (!queues.IsNullOrEmpty())
                using (dbConnection = new Db())
                {
                    dbConnection.ProcessQueue.AddRange(queues);
                    dbConnection.SaveChanges();
                    return true;
                }

            return false;
        }

        public static bool RemoveRecord(Model.ProcessQueue queue)
        {
            if (queue != null)
                using (dbConnection = new Db())
                {
                    dbConnection.Entry<GasBuddy.Model.ProcessQueue>(queue).State = System.Data.Entity.EntityState.Deleted;
                    dbConnection.SaveChanges();
                    return true;
                }

            return false;
        }

        public static bool UpdateRecordInQueue(Model.ProcessQueue queue)
        {
            //remove record from table.

            //add record with new data to table
            if (queue != null)
                using (dbConnection = new Db())
                {
                    dbConnection.Entry<GasBuddy.Model.ProcessQueue>(queue).State = System.Data.Entity.EntityState.Deleted;
                    dbConnection.ProcessQueue.Add(new Model.ProcessQueue { UserID = queue.UserID, Successful = queue.Successful, RetryCount = queue.RetryCount, Priority = queue.Priority, FailCount = queue.FailCount, Processing = false });
                    dbConnection.SaveChanges();
                    return true;
                }

            return false;
        }

        public static GasBuddy.Model.ProcessQueue GetNextQueue()
        {
            GasBuddy.Model.ProcessQueue queue;
            using (dbConnection = new Db())
            {
                queue = dbConnection.ProcessQueue.Where(q => !q.Successful && q.FailCount <= q.RetryCount && q.Priority != -1 && !q.Processing).FirstOrDefault();
                if (queue != null)
                {
                    queue.Processing = true;
                    dbConnection.Entry(queue).State = System.Data.Entity.EntityState.Modified;
                    dbConnection.SaveChanges();
                }
            }

            return queue;
        }

        public static bool RemoveAllQueue()
        {
            using (dbConnection = new Db())
            {
                foreach (var queue in dbConnection.ProcessQueue)
                {
                    dbConnection.Entry(queue).State = System.Data.Entity.EntityState.Deleted;
                }
                dbConnection.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// Archive ProcessQueue Table into ProcessQueueArchive.
        /// </summary>
        /// <returns></returns>
        public static bool ArchiveData()
        {
            using (dbConnection = new Db())
            {
                foreach (var queue in dbConnection.ProcessQueue)
                {
                    dbConnection.ProcessQueueArchive.Add(new Model.ProcessQueueArchive { 
                        DateAdded = DateTime.Now, 
                        FailCount = queue.FailCount, 
                        Priority = queue.Priority,
                        Processing = queue.Processing,
                        RetryCount = queue.RetryCount, 
                        Successful = queue.Successful, 
                        UserID = queue.UserID });

                    dbConnection.Entry(queue).State = System.Data.Entity.EntityState.Deleted;
                }
                dbConnection.SaveChanges();
                return true;
            }
        }
    }
}
