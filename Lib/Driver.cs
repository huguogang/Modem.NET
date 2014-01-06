using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace Lib
{
    /// <summary>
    /// with a interface similar to the ServiceBase. This class can be
    /// shread between console app and windows services for easy debugging
    /// </summary>
    public class Driver
    {
        protected SerialPort port = null;
        public void OnStart(string[] args)
        {
            try
            {
                Schedule s1 = new Schedule(Config.Schedule1_StartTime, Config.RandomDelay_s,
                  new TimeSpan(0, Config.RetryInterval_min, 0), Config.MaxRetry, 1);
                Schedule s2 = new Schedule(Config.Schedule2_StartTime, Config.RandomDelay_s,
                  new TimeSpan(0, Config.RetryInterval_min, 0), Config.MaxRetry, 2);

                port = new SerialPort(Config.PortName, Config.BaudRate,
                    Config.Parity, Config.DataBits, Config.StopBits);
                port.Open();
                Modem m = new Modem(port);
                ATResponseCode code;
                string logMsg;
                while (true)
                {
                    if (s1.CheckSchedule())
                    {
                        code = m.Dial(Config.DialNumber);
                        logMsg = string.Format("Dialing response: {0}", code);
                        MessageLog.LogMessage(logMsg, MessageLog.LogEntryType.Information);
                        if (code != ATResponseCode.AT_OK)
                        {
                            //report failure
                            s1.CompleteTask(false);
                        }
                        else
                        {
                            code = m.Dial(Config.DialData);
                            logMsg = string.Format("Send data response: {0}", code);
                            MessageLog.LogMessage(logMsg, MessageLog.LogEntryType.Information);
                            s1.CompleteTask(true);
                        }
                    }
                    else if (s2.CheckSchedule())
                    {
                        code = m.Dial(Config.DialNumber);
                        logMsg = string.Format("Dialing response: {0}", code);
                        MessageLog.LogMessage(logMsg, MessageLog.LogEntryType.Information);
                        if (code != ATResponseCode.AT_OK)
                        {
                            //report failure
                            s2.CompleteTask(false);
                        }
                        else
                        {
                            code = m.Dial(Config.DialData);
                            logMsg = string.Format("Send data response: {0}", code);
                            MessageLog.LogMessage(logMsg, MessageLog.LogEntryType.Information);
                            s2.CompleteTask(true);
                        }
                    }
                    else
                    {
                        //idle, yield
                        Thread.Sleep(30000);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                MessageLog.LogException(e);
                //TODO: certail exceptions should have a recovery strategy
            }
        }

        public void OnStop()
        {
            if (port != null && port.IsOpen)
            {
                port.Close();
            }
        }
    }
}
