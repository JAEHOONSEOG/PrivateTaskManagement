using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTM.Httpd;
using PTM.ORM;
using PTM.ORM.Dao;
using PTM.ORM.Entity;
using Newtonsoft.Json;

namespace PTM.StartConsole
{
    class Message : Dictionary<String, Action<WSNode>>
    {
        public Message()
        {
            Add("set_memo_insert", SetMemoInsert);
            Add("get_memo_list", GetMemoList);
            Add("get_memo_item", GetMemoItem);
        }
        private void Error(WSNode node)
        {
            node.Key = "error";
            node.Data = "Not exists key.";
            node.Type = -1;
        }
        private void GetMemoList(WSNode node)
        {
            IMemoDao dao = ORMFactory.GetService<IMemoDao>(typeof(IMemoDao));
            var list = dao.Select();
            list = list.OrderByDescending((m) =>
            {
                return m.RecentlyDate;
            }).ToList();
            string json = JsonConvert.SerializeObject(list);
            node.Data = json;
        }
        private void SetMemoInsert(WSNode node)
        {
            IMemoDao dao = ORMFactory.GetService<IMemoDao>(typeof(IMemoDao));
            String temp = node.Data;
            Memo memo = new Memo();
            var map = GetParameter(node.Data);
            memo.Title = map.ContainsKey("title") && !String.IsNullOrEmpty(map["title"].Trim()) ? map["title"] : "No title";
            memo.Contents = map.ContainsKey("contents") && !String.IsNullOrEmpty(map["contents"].Trim()) ? map["contents"] : "";
            memo.RecentlyDate = DateTime.Now;
            int scope = dao.InsertAndScope(memo);
            node.Data = scope.ToString();
        }
        private void GetMemoItem(WSNode node)
        {
            int idx;
            try
            {
                idx = Convert.ToInt32(node.Data);
            }
            catch (Exception e)
            {
                Error(node);
                return;
            }
            IMemoDao dao = ORMFactory.GetService<IMemoDao>(typeof(IMemoDao));
            //TODO: We can not where?
            //var item = dao.GetEneity(idx);
            var list = dao.Select();
            var item = list.Where((m) =>
            {
                return m.Idx == idx;
            }).FirstOrDefault();
            node.Data = JsonConvert.SerializeObject(item);
        }
        public void Execute(String key, WSNode node)
        {
            if (base.ContainsKey(key))
            {
                base[key](node);
                return;
            }
            Error(node);
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
    }
}
