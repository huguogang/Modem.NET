using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

using Lib;
namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("1000");
            Test1();
            //Test2();
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
            SerialPort port = new SerialPort(Config.PortName, Config.BaudRate,
                Config.Parity, Config.DataBits, Config.StopBits);
            Modem m = new Modem(port);
            m.Dial(Config.DialNumber);
        }
    }
}
