using System;
using System.Windows.Forms;
using System.IO;
using PTM.Httpd;

namespace PTM.StartConsole
{
    class Program
    {
        static void Main(string[] args)
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
                return new WebSocketNode() { OPCode = Opcode.MESSAGE, Message = "Hello world" };
            });

            Console.ReadKey();
        }
    }
}
