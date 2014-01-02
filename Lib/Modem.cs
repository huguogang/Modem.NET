using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace Lib
{
    public class Modem
    {
        protected SerialPort serialPort;
        protected int maxRetry = 3;
        protected int poll_interval_ms = 500;

        public Modem(SerialPort port)
        {            
            serialPort = port;
            //init port
            EnsurePortOpen();
            //we will handle read timeout manually, system implementation is not consistent
            serialPort.ReadTimeout = 0;
            serialPort.WriteTimeout = SerialPort.InfiniteTimeout;
            serialPort.DiscardInBuffer();
            serialPort.DiscardOutBuffer();
            serialPort.NewLine = "\r";
            serialPort.Encoding = Encoding.ASCII;
        }

        public ATResponseCode Dial(string number)
        {
            EnsurePortOpen();
            string cmd = "ATDT" + number;
            return ExecuteATCommand(cmd, Config.DialTimeout_s);
        }

        public void SoftReset()
        {
            EnsurePortOpen();
            string cmd = "ATZ0";
            ExecuteATCommand(cmd, ATResponseCode.AT_OK);
        }
                
        public void SpeakerOnOff(bool isSpeakerOn)
        {
            EnsurePortOpen();
            string cmd;
            if (isSpeakerOn)
            {
                cmd = "ATM1";
            }
            else
            {
                cmd = "ATM0";
            }
            ExecuteATCommand(cmd, ATResponseCode.AT_OK);
        }

        public void EchoOnOff(bool isEchoOn)
        {
            EnsurePortOpen();
            string cmd;
            if (isEchoOn)
            {
                cmd = "ATE1";
            }
            else
            {
                cmd = "ATE0";
            }
            ExecuteATCommand(cmd, ATResponseCode.AT_OK);
        }

        public void SetSRegister(int register, int value)
        {
            EnsurePortOpen();
            string cmd = "ATS" + register + "=" + value;
            ExecuteATCommand(cmd, ATResponseCode.AT_OK);
        }

        protected ATResponseCode ExecuteATCommand(string cmd, 
            ATResponseCode expectedResponse, int timeout_s = 1)
        {
            ATResponseCode rCode = ExecuteATCommand(cmd, timeout_s);
            if (rCode != expectedResponse)
            {
                String msg = String.Format(
                    "Error executing command. Command: {0}, Expected Response: {1}, Actual Response: {2}",
                    cmd, expectedResponse, rCode);
                throw new ApplicationException();
            }
            return rCode;
        }

        protected ATResponseCode ExecuteATCommand(string cmd, int timeout_s = 1)
        {
            DateTime start = DateTime.Now;
            //does not work, seems to be writing too fast
            //serialPort.WriteLine(cmd);
            string outS = cmd + "\r";
            int len = outS.Length;
            for (int i = 0; i < len; i++)
            {
                serialPort.Write(outS.Substring(i, 1));
                //KLUDGE: this delay is required for command to be accepted by modem
                Thread.Sleep(50);
            }
            
            String response;
            ATResponseCode rCode;
            while((rCode = ParseResponse(response = ReadLine(timeout_s))) == ATResponseCode.AT_OTHER)
            {
                Console.WriteLine(response);
                if ((DateTime.Now - start).TotalSeconds > timeout_s)
                {
                    return rCode;
                }
            }
            return rCode;
        }
        /// <summary>
        /// read until line feed or carriage return
        /// </summary>
        /// <param name="timeout_s"></param>
        /// <returns></returns>
        protected string ReadLine(int timeout_s)
        {
            DateTime start = DateTime.Now;
            string ret = "";
            Console.WriteLine(timeout_s);
            while (serialPort.BytesToRead > 0 || (DateTime.Now - start).TotalSeconds < timeout_s)
            {
                if (serialPort.BytesToRead > 0)
                {
                    char c = (char)serialPort.ReadChar();
                    if (c == '\n' || c == '\r' || c == '\0')
                    {
                        return ret;
                    }
                    ret += c;
                }
                else
                {
                    Thread.Sleep(poll_interval_ms);
                }
            }
            //timeout, return whatever we've got
            return ret;
        }

        protected ATResponseCode ParseResponse(string resp)
        {
            resp = resp.Trim();
            if (resp.StartsWith("OK"))
            {
                return ATResponseCode.AT_OK;
            }
            else if (resp.StartsWith("CONNECT"))
            {
                return ATResponseCode.AT_CONNECT;
            }
            else if (resp.StartsWith("RING"))
            {
                return ATResponseCode.AT_RING;
            }
            else if (resp.StartsWith("NO CARRIER"))
            {
                return ATResponseCode.AT_NOCARRIER;
            }
            else if (resp.StartsWith("ERROR"))
            {
                return ATResponseCode.AT_ERROR;
            }
            else if (resp.StartsWith("NO DIALTONE"))
            {
                return ATResponseCode.AT_NODIALTONE;
            }
            else if (resp.StartsWith("BUSY"))
            {
                return ATResponseCode.AT_BUSY;
            }
            else if (resp.StartsWith("NO ANSWER"))
            {
                return ATResponseCode.AT_NOANSWER;
            }
            else if (resp.StartsWith("OK"))
            {
                return ATResponseCode.AT_OK;
            }
            return ATResponseCode.AT_OTHER;
        }

        private void EnsurePortOpen()
        {
            if (!serialPort.IsOpen)
            {
                serialPort.Open();
            }
        }
    }
}
