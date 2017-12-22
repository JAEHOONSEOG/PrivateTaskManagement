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
        private ProgressForm progress;

        public MainForm()
        {
            this.progress = new ProgressForm();
            this.SuspendLayout();
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.Controls.Add(browser = new WebBrowser());
            browser.DocumentCompleted += Browser_DocumentCompleted;
            this.ResumeLayout(false);
        }

        private void Browser_DocumentCompleted(object sender, Gecko.Events.GeckoDocumentCompletedEventArgs e)
        {
            progress.Visible = false;
            progress.Dispose();
            this.Visible = true;
        }

        public void Booting()
        {
            if (progress.IsDisposed)
            {
                this.Visible = true;
            }
            else
            {
                progress.Visible = true;
            }
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Visible = false;
            //base.OnFormClosing(e);
        }
        public void Exit()
        {
            this.Dispose();
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
