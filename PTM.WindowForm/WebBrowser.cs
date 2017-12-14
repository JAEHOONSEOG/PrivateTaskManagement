using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gecko;
using System.IO;
using System.Windows.Forms;

namespace PTM.WindowForm
{
    class WebBrowser : GeckoWebBrowser
    {
        public WebBrowser()
        {
            Xpcom.EnableProfileMonitoring = false;
            var app_dir = Path.GetDirectoryName(Application.ExecutablePath);
            Xpcom.Initialize(Path.Combine(app_dir, "Firefox"));

            Dock = System.Windows.Forms.DockStyle.Fill;
            FrameEventsPropagateToMainWindow = false;
            TabIndex = 0;
            UseHttpActivityObserver = false;
            Navigate("http://localhost:9999");
        }
    }
}
