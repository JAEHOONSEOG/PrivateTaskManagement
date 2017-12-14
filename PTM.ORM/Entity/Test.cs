using System;
using System.Data.OleDb;
using PTM.ORM.Attribute;
using PTM.ORM.Common;

namespace PTM.ORM.Entity
{
    [Table("Test")]
    public sealed class Test : IEntity
    {
        [Column("idx", OleDbType.Integer, Key = true, Identity = true)]
        private int idx;

        public int Idx
        {
            get { return this.idx; }
        }

        [Column("data", OleDbType.VarChar)]
        private String data;

        public String Data
        {
            get { return this.data; }
            set { this.data = value; }
        }
    }
}
