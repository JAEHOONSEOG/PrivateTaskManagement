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
        private delegate void SetSizeInvoke(String size);
        private delegate void BootingInvoke();

        public MainForm(String port)
        {
            this.progress = new ProgressForm();
            this.SuspendLayout();
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.Controls.Add(browser = new WebBrowser(port));
            browser.DocumentCompleted += Browser_DocumentCompleted;
            this.ResumeLayout(false);
        }
        public void SetSize(String size)
        {
            if (InvokeRequired)
            {
                SetSizeInvoke invoke = new SetSizeInvoke(SetSizeInline);
                this.Invoke(invoke, size);
            }
            else
            {
                SetSizeInline(size);
            }
        }
        private void SetSizeInline(String size)
        {
            if (String.Equals("full", size))
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                String[] buffer = size.Split('*');
                if (buffer.Length != 2)
                {
                    this.WindowState = FormWindowState.Maximized;
                }
                else
                {
                    try
                    {
                        int width = Convert.ToInt32(buffer[0]);
                        int height = Convert.ToInt32(buffer[1]);
                        this.Size = new System.Drawing.Size(width, height);
                        this.WindowState = FormWindowState.Normal;
                    }
                    catch
                    {
                        this.WindowState = FormWindowState.Maximized;
                    }
                }
            }
        }
        private void Browser_DocumentCompleted(object sender, Gecko.Events.GeckoDocumentCompletedEventArgs e)
        {
            progress.Visible = false;
            progress.Dispose();
            this.Visible = true;
        }

        public void Booting()
        {
            if (InvokeRequired)
            {
                BootingInvoke invoke = new BootingInvoke(BootingInline);
                this.Invoke(invoke);
            }
            else
            {
                BootingInline();
            }
        }

        private void BootingInline()
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
