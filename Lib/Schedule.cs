using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib
{
    /// <summary>
    /// manage scheduled jobs meta data and current status
    /// </summary>
    public class Schedule
    {
        //-- meta
        /// <summary>
        /// Configuration: time of the day this schedule should be executed
        /// </summary>
        public TimeSpan StartTime;
        /// <summary>
        /// Configuration: Total number of seconds for random delay from TimeOfDay
        /// </summary>
        public int RandomDelaySec;
        public TimeSpan RetryInterval;
        public int MaxRetry;

        public DateTime NextRunTime;
        
        //-- current execution status
        /// <summary>
        /// retry count
        /// </summary>
        public int RetryCount = 0;

        private Random rnd;

        public Schedule(TimeSpan startTime, int randomDelaySec, TimeSpan retryInterval, 
            int maxRetry, int rndSeed)
        {
            //schedules are created in almost the same second
            //rnd = new Random((int)(DateTime.Now.TimeOfDay.TotalSeconds));
            rnd = new Random(rndSeed);
            this.StartTime = startTime;
            this.RandomDelaySec = randomDelaySec;
            this.MaxRetry = maxRetry;
            this.RetryInterval = retryInterval;
 
            NextRunTime = GetNextRun();
            Console.WriteLine("Scheduled next task @{0}", NextRunTime);
        }
        /// <summary>
        /// calculate next runtime based on current time, schedule, plus a random delay
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        private DateTime GetNextRun()
        {
            //backdoor for testing
            if (StartTime == new TimeSpan(0))
            {
                return DateTime.Now;
            }
            
            DateTime now = DateTime.Now;
            int randSec = rnd.Next(RandomDelaySec);
            if (now.TimeOfDay.TotalSeconds >= StartTime.TotalSeconds)
            {
                //tomorrow
                return DateTime.Today.AddDays(1).Add(StartTime).AddSeconds(randSec);
            }
            //today
            return DateTime.Today.Add(StartTime).AddSeconds(randSec);
        }

        /// <summary>
        /// check if should run task based on this schedule
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        public bool CheckSchedule()
        {
            DateTime now = DateTime.Now;
            return now > NextRunTime;
        }
        
        /// <summary>
        /// report execution status of the current task, so we can schedule retry, next run, ...
        /// </summary>
        public void CompleteTask(bool success)
        {
            if(!success)
            {
                RetryCount++;
                
                if (RetryCount <= MaxRetry)
                {
                    //schedule retry
                    NextRunTime = DateTime.Now.Add(RetryInterval);
                    Console.WriteLine("Task failed. Scheduled retry #{0}@{1}", RetryCount, NextRunTime);
                    return;    
                }
                Console.WriteLine("Exceeds max retry, task failed");
            }
            //done, start the next scheduled task
            RetryCount = 0;
            NextRunTime = GetNextRun();
            Console.WriteLine("Scheduled next task @{0}", NextRunTime);
        }
    }
}
