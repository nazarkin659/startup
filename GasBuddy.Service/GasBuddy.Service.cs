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
    public partial class GasBuddyMain : ServiceBase
    {
        System.Timers.Timer _timer;
        DateTime _scheduleTime;
        private static Random random = new Random((int)DateTime.Now.Ticks);
        private int Interval;

        public GasBuddyMain()
        {
            

            Interval = int.Parse(System.Configuration.ConfigurationManager.AppSettings["IntervalBetweenProcessingInMinutes"]);

            _timer = new System.Timers.Timer();
            //_scheduleTime = DateTime.Now.AddMinutes(3); // Schedule to run once a day at 7:00 a.m.

            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            SiAuto.Main.LogMessage("Start service");
            //Thread.Sleep(new TimeSpan(0, 0, 15));
            try
            {
                // For first time, set amount of seconds between current time and schedule time
                _timer.AutoReset = false;
                _timer.Interval = Interval * 60 * 1000;
                _timer.Elapsed += new System.Timers.ElapsedEventHandler(DoJob);
                _timer.Enabled = true;
            }
            catch (Exception e)
            {
                SiAuto.Main.LogException(string.Format("Service1 => OnStart"), e);
            }
        }

        /// <summary>
        /// Based on provided date returns random run time for the next day.
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="randomDiapazon">0 - ?? hours</param>
        /// <returns>Seconds.</returns>
        private static double? GetNextRunTime(DateTime datetime, int randomDiapazon = 4)
        {
            if (datetime != null)
            {
                double r = (24 * 60 * 60) - (datetime.TimeOfDay.TotalSeconds);

                int runBetweenStartSeconds = 0;
                int runBetweenStopSeconds = randomDiapazon * 60 * 60;
                double toAdd = random.Next(runBetweenStartSeconds, runBetweenStopSeconds); //actual runtime

                double actualStartTimeSeconds = r + toAdd;

                return actualStartTimeSeconds;
            }

            return null;
        }

        private void PrepareUsers(string userName = null)
        {
            using (var dbConnection = new GasBuddy.Infrastructure.Base.Db())
            {
                if (string.IsNullOrWhiteSpace(userName))
                    dbConnection.Database.ExecuteSqlCommand("exec [PrepareUsers]");
                else
                    dbConnection.Database.ExecuteSqlCommand("exec [PrepareUsers] @UserName={0}", userName);
            }
        }

        protected override void OnStop()
        {
        }

        private bool InsertUsersInQueue()
        {
            List<User> users = UserFunc.GetUsers(500);
            if (users.IsNullOrEmpty())
                throw new Exception("Service1 => InsertUsersInQueue: Can't get users.");
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

        private User GetUnproceedUser(out ProcessQueue queue)
        {
            User u = null;
            queue = ProcessQueueF.GetNextQueue();
            if (queue != null)
                u = UserFunc.GetUserIDByProcessQueue(queue);

            return u;
        }

        protected void DoJob(object sender, System.Timers.ElapsedEventArgs e)
        {
            // 1. Process Schedule Task
            // ----------------------------------
            ProcessQueue queue = null;
            User userToProcess = null;
            try
            {
                SiAuto.Main.EnterMethod("Service1 => Timer_Elapsed");

                userToProcess = GetUnproceedUser(out queue);


                if (userToProcess == null)
                {
                    SiAuto.Main.LogMessage("Service1 => Timer_Elapsed: No user to process [{0}]", DateTime.Now.ToShortTimeString());
                    //stop service till next day.
                    double? nextRunTime = GetNextRunTime(DateTime.Now, 8);
                    if (nextRunTime == null)
                    {
                        SiAuto.Main.LogError("Can't get next run time date. [{0}]", DateTime.Now.ToShortTimeString());
                        this.Stop();
                    }

                    SiAuto.Main.LogColored(System.Drawing.Color.Blue, "Next Run [{0}] - [{1}]", DateTime.Now.AddDays(1).ToShortDateString(), new TimeSpan(0, 0, (int)(DateTime.Now.TimeOfDay.TotalSeconds + nextRunTime)));
                    _timer.Interval = (double)nextRunTime * 1000; //set internal in seconds.
                }
                else
                {
                    SiAuto.Main.LogSeparator();
                    SiAuto.Main.LogColored(Level.Message, System.Drawing.Color.Blue, "User [{0}]", userToProcess.UserName);
                    SiAuto.Main.LogSeparator();


                    if (userToProcess.PrizesToReport == 0 ||
                       (userToProcess.PrizeEntriesReported != null &&
                        userToProcess.PrizeEntriesReported < userToProcess.PrizesToReport &&
                        CommonAction.isReadyToReportPrices(ref userToProcess) &&
                        GasBuddy.UserAction.isReported(ref userToProcess)))
                    {
                        ProcessQueueF.RemoveRecord(queue);
                    }
                    else
                    {
                        //failed count ++
                        queue.FailCount++;

                        ProcessQueueF.UpdateRecordInQueue(queue);
                    }

                    //TODO: call repository, update user.
                    UserFunc.UpdateUser(userToProcess);
                }
            }
            catch (Exception ex)
            {
                SiAuto.Main.LogException(string.Format("Service1 => Timer_Elapsed: Queue [{0}] User [{1}]", queue.ID, userToProcess.UserName ?? ""), ex);
            }
            finally
            {
                if (_timer.Interval != Interval * 60 * 1000)
                {
                    _timer.Interval = Interval * 60 * 1000;
                }
                _timer.AutoReset = true;
                SiAuto.Main.LeaveMethod("Service1 => Timer_Elapsed");
            }

            // 2. If tick for the first time, reset next run to every 24 hours
            //if (_timer.Interval != 60 * 10 * 1000)
            //{
            //    _timer.Interval = 60 * 10 * 1000;
            //}
        }
    }
}
