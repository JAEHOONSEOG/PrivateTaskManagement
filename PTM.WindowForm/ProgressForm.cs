using System.Windows.Forms;
using System.Drawing;
using System;

namespace PTM.WindowForm
{
    class ProgressForm : Form
    {
        private ProgressBar progressBar1;
        public ProgressForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.progressBar1 = new ProgressBar();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar1.Location = new System.Drawing.Point(0, 0);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(450, 30);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 0;
            this.progressBar1.MarqueeAnimationSpeed = 10;
            // 
            // ProgressForm
            // 
            this.ClientSize = new System.Drawing.Size(450, 30);
            this.Controls.Add(this.progressBar1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ProgressForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);

        }
    }
}
