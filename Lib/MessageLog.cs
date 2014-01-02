using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;

namespace Lib
{
    public class MessageLog
    {
        public enum LogEntryType
        {
            Information = 0,
            Warning = 1,
            Error = 2
        };

        static public int LogException(Exception e)
        {
            return LogException(e, LogEntryType.Error);
        }

        static public int LogException(Exception e, LogEntryType type)
        {
            string msg = "Exception:\r\n" + e.ToString();
            return LogMessage(msg, type);
        }

        public static int LogMessage(string message, LogEntryType type)
        {
            try
            {
                //always log to text file
                LogMessageToTextFile(type, message);
                return 0;
            }
            catch
            {
                //logging failed, catch all to avoid infinite loop trying to log failed log...
                return -1;
            }

        }

        #region private implementation of logging

        private static string errorLogFileName;
        private static string applicationName;
        /// <summary>
        /// static constructor to initialize static variables
        /// </summary>
        static MessageLog()
        {
            applicationName = AppDomain.CurrentDomain.FriendlyName;
            errorLogFileName = Config.ErrorLogFile;
        }

        private static int LogMessageToTextFile(LogEntryType type, string message)
        {
            try
            {

                string logmsg = "----------------------------\r\n" + //separator for new log
                    "Local time: " + DateTime.Now.ToString() + "\r\n" + //local time
                    //summary info                
                    "Application Name: " + applicationName + "\r\n" +
                    "Log Type: " + type.ToString() + "\r\n" +
                    "Error Message:\r\n" + message + "\r\n";
                
                //allow error log to be encoded by local time
                string logFileName = string.Format(errorLogFileName, DateTime.Now);
                using (StreamWriter w = File.AppendText(logFileName))
                {
                    w.Write(logmsg);
                }

                return 0; //logged to local file
            }
            catch (Exception e)
            {
                //ignore errors, avoid infinite loop of trying to log errors
#if DEBUG
                System.Console.WriteLine(e.ToString());
                System.Diagnostics.Debug.WriteLine(e.ToString());
#endif

                return -1;
            }
        }

        #endregion
    }
}
