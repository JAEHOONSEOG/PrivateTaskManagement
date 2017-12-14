using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PTM.ORM.Common;
using PTM.ORM.Entity;

namespace PTM.ORM.Dao
{
    public interface ITestDao : IDao
    {
        IList<Test> Select();

        int Insert(Test entity);

        int Update(Test entity);

        int Delete(Test entity);
    }
}
