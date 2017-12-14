using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using PTM.Httpd;
using PTM.ORM;
using PTM.ORM.Dao;
using PTM.ORM.Entity;
using PTM.WindowForm;

namespace PTM.StartConsole
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
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
            Console.WriteLine("Count: "+result.Count);

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
                return new WebSocketNode() { OPCode = Opcode.MESSAGE, Message = "Hello world" };
            });

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
