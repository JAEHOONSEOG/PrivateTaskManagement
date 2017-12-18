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
        public Program()
        {
            ORMFactory.Initialize();
            WebServer();
            WindowForm();
        }
        private void WebServer()
        {
            string path = Path.GetDirectoryName(Application.ExecutablePath);
            var server = ServerFactory.NewInstance(9999);
            var flow = new Flow();
            var message = new Message();
            server.SetDefaultFile("index.html");
            server.SetZip(path + "\\html.data");
            //server.SetRootPath(webpath);
            /*server.Set("/", (res, req) =>
            {
                //req.SetCookie("test", "aaa", DateTime.Now.AddMinutes(5));
                //req.SetSession("aaaaa", "asdfasfd");
                req.ReadFile(webpath + @"\index.html");
            });*/
            server.SetWebSocket(mes =>
            {
                Console.WriteLine(mes);
                WSNode node = WSNode.ToNode(mes.ToString());
                if (node.Type == 1)
                {
                    flow.Execute(node.Key, node);
                }
                else if (node.Type == 2)
                {
                    message.Execute(node.Key, node);
                }
                return new WebSocketNode() { OPCode = Opcode.BINARY, Message = node.ToString2() };
            });
        }

        private String2 ReadFile(String path)
        {
            FileInfo info = new FileInfo(path);
            if (!info.Exists)
            {
                return null;
            }
            String2 temp = new String2((int)info.Length);
            using (FileStream stream = new FileStream(info.FullName, FileMode.Open, FileAccess.Read))
            {
                stream.Read(temp.ToBytes(), 0, temp.Length);
            }
            return temp;
        }
        private void WindowForm()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
        [STAThread]
        static void Main(string[] args)
        {
            new Program();
        }
    }
}
