using System;
using System.Collections.Generic;
using System.Data.OleDb;
using PTM.ORM.Dao;
using PTM.ORM.Entity;
using PTM.ORM.Common;

namespace PTM.ORM.Impl
{
    class TestDao : Dao<Test>, ITestDao
    {
        public TestDao(Database db) : base(db.GetConnetcion())
        {
            
        }
        public int Delete(Test entity)
        {
            return base.DeleteByEntity(entity);
        }

        public int Insert(Test entity)
        {
            return base.InsertByEntity(entity);
        }

        public IList<Test> Select()
        {
            return base.SelectAll();
        }

        public int Update(Test entity)
        {
            return base.UpdateByEntity(entity);
        }
    }
}
