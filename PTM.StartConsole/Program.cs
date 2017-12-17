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
                //Console.WriteLine(mes);
                WSNode node = WSNode.ToNode(mes.ToString());
                if (node.Type == 1)
                {
                    if ("cardmenu".Equals(node.Key))
                    {
                        node.Data = server.GetZipFile(@"/flow/cardmenu.html").ToString();
                        //node.Data = ReadFile(webpath + @"\flow\cardmenu.html").ToString();
                    }
                    else if ("memolist".Equals(node.Key))
                    {
                        node.Data = server.GetZipFile(@"/flow/memo_insert.html").ToString();
                        //node.Data = ReadFile(webpath + @"\flow\memo_insert.html").ToString();
                    }
                }
                else if (node.Type == 2)
                {
                    if ("memo_insert".Equals(node.Key))
                    {
                        IMemoDao dao = ORMFactory.GetService<IMemoDao>(typeof(IMemoDao));
                        String temp = node.Data;
                        Memo memo = new Memo();
                        var map = GetParameter(node.Data);
                        memo.Title = map.ContainsKey("title") ? map["title"] : "No title";
                        memo.Contents = map.ContainsKey("contents") ? map["contents"] : "";
                        memo.RecentlyDate = DateTime.Now;
                        int scope = dao.InsertAndScope(memo);
                        node.Data = scope.ToString();
                    }
                }
                return new WebSocketNode() { OPCode = Opcode.BINARY, Message = node.ToString2() };
            });
        }
        private Dictionary<String, String> GetParameter(String data)
        {
            Dictionary<String, String> ret = new Dictionary<string, string>();
            foreach (var b in data.Split('&'))
            {
                int pos = b.IndexOf("=");
                if (pos < 0)
                {
                    continue;
                }
                String key = b.Substring(0, pos);
                String value = b.Substring(pos + 1, b.Length - (pos + 1));
                if (ret.ContainsKey(key))
                {
                    ret[key] = value;
                }
                else
                {
                    ret.Add(key, value);
                }
            }
            return ret;
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
