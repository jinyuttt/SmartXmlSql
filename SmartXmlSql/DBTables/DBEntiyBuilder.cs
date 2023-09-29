using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using SmartXmlSql.Entitys;

namespace SmartXmlSql.DBTables
{
    public  class DBEntiyBuilder
    {

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DBName { get; set; } = string.Empty;

        /// <summary>
        /// 数据库所有表的SQL，字段tablename
        /// </summary>
        public string AllTableSql { get; set; } = string.Empty;

        /// <summary>
        /// 代码名称空间
        /// </summary>
        public string CodeNameSpace { get; set; } = string.Empty;

       
        /// <summary>
        /// 数据库连接
        /// </summary>
        public IDbConnection DbConnection { get; set; }

        /// <summary>
        /// 数据库适配器
        /// </summary>
        public IDbDataAdapter DbDataAdapter { get; set; }

        /// <summary>
        /// 代码输出路径
        /// </summary>
        public string OutFile { get; set; } = string.Empty;

        public void Create()
        {
            if (DbConnection == null)
            {
                throw new Exception("DbConnection" + "不能为空");
            }
            if(DbDataAdapter == null)
            {
                throw new Exception("DbDataAdapter" + "不能为空");
            }
            if(string.IsNullOrEmpty(DBName))
            {
                DBName = DbConnection.Database;
            }
            if (string.IsNullOrEmpty(OutFile))
            {
                string dir = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
                while (!dirInfo.Name.EndsWith("bin"))
                {
                    dirInfo = dirInfo.Parent;
                }
                OutFile = dirInfo.Parent.CreateSubdirectory("DBEntity").FullName;
            }
            if (string.IsNullOrEmpty(CodeNameSpace))
            {
                
                StackTrace stackTrace = new StackTrace();
                CodeNameSpace = stackTrace.GetFrame(1).GetType().Namespace;
            }

            var cmd = DbConnection.CreateCommand();
            cmd.CommandText = AllTableSql;
            DbDataAdapter.SelectCommand = cmd;
            DataSet ds = new DataSet();
            DbDataAdapter.Fill(ds);
            cmd.Dispose();
            //
            foreach (DataTable dataTable in ds.Tables)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    if (row != null)
                    {
                        string tb = row["tablename"].ToString();
                         var cmdt = DbConnection.CreateCommand();
                        cmdt.CommandText = string.Format("select * from {0} where 1=2", tb);


                        DbDataAdapter.SelectCommand = cmdt;
                        DataSet dst = new DataSet();
                        DbDataAdapter.Fill(dst);
                        dst.Tables[0].TableName = tb;

                        DBCodeUtil.CreateDBEntity(dst, CodeNameSpace, OutFile);

                    }
                }
            }
        }
    }
}
