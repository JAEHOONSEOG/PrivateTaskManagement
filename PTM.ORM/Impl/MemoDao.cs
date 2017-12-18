using System;
using System.Collections.Generic;
using System.Data.OleDb;
using PTM.ORM.Dao;
using PTM.ORM.Entity;
using PTM.ORM.Common;

namespace PTM.ORM.Impl
{
    class MemoDao : Dao<Memo>, IMemoDao
    {
        public MemoDao(Database db) : base(db.GetConnetcion())
        {

        }
        public int Delete(Memo entity)
        {
            return base.DeleteByEntity(entity);
        }

        public int Insert(Memo entity)
        {
            return base.InsertByEntity(entity);
        }

        public int InsertAndScope(Memo entity)
        {
            return base.InsertByEntity(entity, true);
        }

        public IList<Memo> Select()
        {
            return base.SelectAll();
        }

        public int Update(Memo entity)
        {
            return base.UpdateByEntity(entity);
        }

        public Memo GetEneity(int idx)
        {
            return base.Transaction(() =>
            {
                Memo ret = null;
                Console.WriteLine(idx);
                this.ExcuteReader("select idx,title,contents,recentlydate from PTMMemo where idx=@idx", new List<OleDbParameter>()
                {
                    CreateParameter("@idx",idx,OleDbType.Integer)
                },
                (dr) =>
                {
                    if (dr.Read())
                    {
                        ret = new Memo();
                        ret.Idx = dr.GetInt32(0);
                        ret.Title = dr.GetString(1);
                        ret.Contents = dr.GetString(2);
                        ret.RecentlyDate = dr.GetDateTime(3);
                    }
                });
                return ret;
            });
        }
    }
}
