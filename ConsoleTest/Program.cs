using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

using Lib;
namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("1000");
            //Test1();
            Test2();
        }
        /// <summary>
        /// simple test of dialing functionality
        /// </summary>
        static void Test1() 
        {
            try
            {
                SerialPort port = new SerialPort(Config.PortName, Config.BaudRate,
                    Config.Parity, Config.DataBits, Config.StopBits);
                port.Open();
                Modem m = new Modem(port);
                ATResponseCode code = m.Dial(Config.DialNumber);
                if (code == ATResponseCode.AT_OK)
                {
                    code = m.Dial(Config.DialData);
                }
                port.Close();
                Console.WriteLine("response: {0}", code);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        /// <summary>
        /// full program with diagnostics output
        /// </summary>
        static void Test2()
        {
            try
            {
                Schedule s1 = new Schedule(Config.Schedule1_StartTime, Config.RandomDelay_s,
                  new TimeSpan(0, Config.RetryInterval_min, 0), Config.MaxRetry);
                SerialPort port = new SerialPort(Config.PortName, Config.BaudRate,
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
                    else
                    {
                        //idle yield
                        Thread.Sleep(30000);
                    }
                }

                port.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                MessageLog.LogException(e);
            }
        }
    }
}
