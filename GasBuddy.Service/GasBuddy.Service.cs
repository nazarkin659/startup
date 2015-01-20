using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Gurock.SmartInspect;
using System.Threading;
using GasBuddy.Model;
using GasBuddy.Infrastructure;
using HelperFunctions;

namespace GasBuddy.Service
{
    public partial class Service1 : ServiceBase
    {
        System.Timers.Timer _timer;
        DateTime _scheduleTime;

        public Service1()
        {
            SiAuto.Si.Connections = "tcp()";
            SiAuto.Si.Enabled = true;
            SiAuto.Si.DefaultLevel = Level.Debug;
            SiAuto.Main.LogMessage("Initializing components");


            _timer = new System.Timers.Timer();
            _scheduleTime = DateTime.Now.AddMinutes(10); // Schedule to run once a day at 7:00 a.m.


            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            SiAuto.Main.LogMessage("Start service");
            Thread.Sleep(new TimeSpan(0, 0, 15));
            try
            {
                if (!InsertUsersInQueue())
                    throw new Exception("InsertUsersInQueue failed");



                // For first time, set amount of seconds between current time and schedule time
                _timer.Enabled = true;
                _timer.Interval = _scheduleTime.Subtract(DateTime.Now).TotalSeconds * 1000;
                _timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
            }
            catch (Exception e)
            {
                SiAuto.Main.LogException(string.Format("Service1 => OnStart"), e);
            }
        }

        protected override void OnStop()
        {
        }

        private bool InsertUsersInQueue()
        {
            List<User> users = UsersFunc.GetUsers(500);
            if (users.IsNullOrEmpty())
                throw new Exception("Service1 => InsertUsersInQueue: Can't get users.");
            else
            {
                List<ProcessQueue> queues = new List<ProcessQueue>();
                foreach (var user in users)
                {
                    ProcessQueue queue = new ProcessQueue();
                    queue.UserID = user.UserID;
                    queue.Successful = false;
                    queue.RetryCount = 3;
                    queue.Priority = 0;
                    queue.FailCount = 0;

                    queues.Add(queue);
                }

                ProcessQueueF.AddRecord(queues);
                return true;
            }
        }

        private User GetUnproceedUser(out ProcessQueue queue)
        {
            User u = null;
            queue = ProcessQueueF.GetNextQueue();
            if (queue != null)
                u = UsersFunc.GetUserIDByProcessQueue(queue);

            return u;
        }

        protected void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // 1. Process Schedule Task
            // ----------------------------------
            ProcessQueue queue = null;
            User userToProcess = null;
            try
            {
                userToProcess = GetUnproceedUser(out queue);


                if (userToProcess == null)
                {
                    SiAuto.Main.LogMessage("Service1 => Timer_Elapsed: No user to process [{0}]", DateTime.Now.ToShortTimeString());
                }
                else
                {
                    if (CommonAction.isReadyToReportPrices(ref userToProcess) || GasBuddy.UserAction.Start(userToProcess)) //????!!!
                    {
                        ProcessQueueF.RemoveRecord(queue);
                    }
                    else
                    {
                        //failed count ++
                        queue.FailCount++;
                        queue.RetryCount--;

                        ProcessQueueF.UpdateRecordInQueue(queue);
                    }
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogException(string.Format("Service1 => Timer_Elapsed: Queue [{0}] User [{1}]", queue.ID, userToProcess.UserName ?? ""), ex);
            }

            // 2. If tick for the first time, reset next run to every 24 hours
            if (_timer.Interval != 60 * 3 * 1000)
            {
                _timer.Interval = 60 * 3 * 1000;
            }
        }
    }
}
