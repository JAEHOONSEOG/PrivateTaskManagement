using System;
using System.ComponentModel;
using System.Windows.Forms;
using Gecko;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace PTM.WindowForm
{
    public class MainForm : Form
    {
        private System.ComponentModel.IContainer components = null;
        private WebBrowser browser;

        public MainForm()
        {
            this.SuspendLayout();
            this.WindowState = FormWindowState.Maximized;
            this.Controls.Add(browser = new WebBrowser());
            this.ResumeLayout(false);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
