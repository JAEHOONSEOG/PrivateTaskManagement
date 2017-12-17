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

namespace PTM.StartConsole
{
    class Program
    {
        public Program()
        {
            Database();
            WebServer();
            WindowForm();
        }
        private void Database()
        {
            ITestDao dao = ORMFactory.GetService<ITestDao>(typeof(ITestDao));
            Test test = new Test();
            test.Data = "hello world";
            dao.Insert(test);
            IList<Test> result = dao.Select();
            foreach (var t in result)
            {
                Console.WriteLine("idx : {0}   data: {1}", t.Idx, t.Data);
            }
            test.Data = "test";
            dao.Update(test);
            result = dao.Select();
            foreach (var t in result)
            {
                Console.WriteLine("idx : {0}   data: {1}", t.Idx, t.Data);
            }
            dao.Delete(test);
            result = dao.Select();
            Console.WriteLine("Count: " + result.Count);
        }
        private void WebServer()
        {
            string webpath = Path.GetDirectoryName(Application.ExecutablePath);
            webpath = Path.Combine(webpath, "web");
            var server = ServerFactory.NewInstance(9999);
            server.SetRootPath(webpath);
            server.Set("/", (res, req) =>
            {
                //req.SetCookie("test", "aaa", DateTime.Now.AddMinutes(5));
                //req.SetSession("aaaaa", "asdfasfd");
                req.ReadFile(webpath + @"\index.html");
            });
            server.SetWebSocket(mes =>
            {
                Console.WriteLine(mes);
                WSNode node = WSNode.ToNode(mes.ToString());
                if (node.Type == 1 && "cardmenu".Equals(node.Key))
                {
                    node.Data = ReadFile(webpath + @"\flow\cardmenu.html").ToString();
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
            using(FileStream stream = new FileStream(info.FullName, FileMode.Open, FileAccess.Read))
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
