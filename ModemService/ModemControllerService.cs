using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

using Lib;
namespace ModemService
{
    public partial class ModemControllerService : ServiceBase
    {
        protected Driver d;

        public ModemControllerService()
        {
            InitializeComponent();
            d = new Driver();
        }

        protected override void OnStart(string[] args)
        {
            d.OnStart(args);
        }

        protected override void OnStop()
        {
            d.OnStop();
        }
    }
}
