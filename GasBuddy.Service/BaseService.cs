using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gurock.SmartInspect;
using GasBuddy.Model;
using HelperFunctions;
using GasBuddy.Infrastructure;

namespace GasBuddy.Service
{
    public class BaseService : System.ServiceProcess.ServiceBase
    {
        public void InitializeLogger()
        {
            SiAuto.Si.Connections = "tcp()";
            SiAuto.Si.DefaultLevel = Level.Debug;
            SiAuto.Si.Enabled = true;
        }
        /// <summary>
        /// Replicating Users to ProcessQueue table.
        /// Applies only for users where PrizesToReport is not 0.
        /// </summary>
        /// <returns></returns>
        protected static bool InsertUsersInQueue()
        {
            List<User> users = UserFunc.GetUsers(500);
            if (users.IsNullOrEmpty())
                throw new Exception("Can't get users.");
            else
            {
                List<ProcessQueue> queues = new List<ProcessQueue>();
                foreach (var user in users.Where(u => u.PrizesToReport != 0))
                {
                    ProcessQueue queue = new ProcessQueue();
                    queue.UserID = user.UserID;
                    queue.Successful = false;
                    queue.RetryCount = 3;
                    queue.Priority = 0;
                    queue.FailCount = 0;
                    queue.Processing = false;

                    queues.Add(queue);
                }

                ProcessQueueF.AddRecord(queues);
                return true;
            }
        }
    }
}
