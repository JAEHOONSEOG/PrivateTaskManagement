using System;
using PTM.Httpd;

namespace PTM.StartConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = ServerFactory.NewInstance(9999);
            server.SetRootPath(@"C:\work\study");
            server.Set("/", (res, req) =>
            {
                req.SetCookie("test", "aaa", DateTime.Now.AddMinutes(5));
                req.SetSession("aaaaa", "asdfasfd");
                req.ReadFile(@"C:\work\study\index.html");
            });
            server.Set("/submit.html", (res, req) =>
            {
                if (!res.IsPost())
                {
                    return;
                }
                Console.WriteLine("session : " + res.GetSession("aaaaa"));
                Console.WriteLine("submit");
                Console.WriteLine(res.View());
                foreach (var item in res.PostString)
                {
                    Console.WriteLine("key :" + item.Key + "value : " + item.Value);
                }
                //req.StateOK();
                req.ContextType = "text/html";
                req.Body = "Test";
                Console.WriteLine(req.View());
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
