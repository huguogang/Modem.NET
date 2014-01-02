using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace Lib
{
    public class Config
    {
        public static string PortName
        {
            get
            {
                return GetConfigValue("PortName");
            }
        }

        public static int BaudRate
        {
            get
            {
                return GetIntConfig("BaudRate", 115200);
            }
        }

        public static int DataBits = 8;
        public static StopBits StopBits = StopBits.One;
        public static Parity Parity = Parity.None;

        public static string DialNumber
        {
            get
            {
                return GetConfigValue("DialNumber");
            }
        }

        /// <summary>
        /// time to wait for response before declare timeout
        /// </summary>
        public static int DialTimeout_s
        {
            get
            {
                return GetIntConfig("DialTimeout_s", 90);
            }
        }

        /// <summary>
        /// wait between re-dials, usually due to busy signal
        /// </summary>
        public static int RetryInterval_min
        {
            get
            {
                return GetIntConfig("RetryInterval_min", 15);
            }
        }
        public static int MaxRetry
        {
            get
            {
                return GetIntConfig("MaxRetry", 6);
            }
        }

        public static string ErrorLogFile
        {
            get
            {
                return GetConfigValue("ErrorLogFile", @"c:\modem\log\{0:yyyy_MM_dd}.txt");
            }
        }

        #region utility functions
        public static string GetConfigValue(string configName)
        {
            return GetConfigValue(configName, "");
        }

        public static String GetConfigValue(string configName, string defaultValue)
        {
            string tmpStr = ConfigurationManager.AppSettings.Get(configName);
            if (tmpStr != null)
            {
                return tmpStr;
            }
            return defaultValue;
        }

        public static bool GetBoolConfig(string configName, bool defaultValue)
        {
            string str = GetConfigValue(configName);

            if (str.Length > 0)
            {
                return Convert.ToBoolean(str);
            }

            return defaultValue;
        }

        public static TimeSpan GetTimeSpanConfig(string configName, TimeSpan defaultValue)
        {
            string str = GetConfigValue(configName);

            if (str.Length > 0)
            {
                return TimeSpan.Parse(str);
            }

            return defaultValue;
        }

        public static DateTime GetDateTimeConfig(string configName, DateTime defaultValue)
        {
            string str = GetConfigValue(configName);

            if (str.Length > 0)
            {
                return DateTime.Parse(str);
            }

            return defaultValue;
        }

        public static int GetIntConfig(string configName, int defaultValue)
        {
            string str = GetConfigValue(configName);

            if (str.Length > 0)
            {
                return Convert.ToInt32(str);
            }

            return defaultValue;
        }
        #endregion
    }
}
