using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using PTM.Httpd;
using PTM.ORM;
using PTM.ORM.Dao;
using PTM.ORM.Entity;
using PTM.WindowForm;
using PTM.Httpd.Util;
using Newtonsoft.Json;

namespace PTM.StartConsole
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainContext());
        }
    }
}
