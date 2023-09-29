using Hikari;

using SmartXmlSql;
using SmartXmlSql.DBTables;
using SmartXmlSql.Entitys;
using System.Data;

namespace ConsoleApp1
{
    internal class HariCode
    {
       

        public void Create(string code) {


             HikariConfig hikariConfig = new HikariConfig();
             hikariConfig.DBType = "PostgreSQL";
             hikariConfig.ConnectString = "Server = 127.0.0.1; Port = 5432; User Id = postgres; Password = 123456; Database = postgres;Pooling=true; ";
            //hikariConfig.DriverDir = "DBDrivers";
            //hikariConfig.DriverDLL = "XXXX.dll";
            //hikariConfig.DBTypeXml = "DBType.xml";
            HikariDataSource hikariDataSource = new HikariDataSource(hikariConfig);
            var connection1 = hikariDataSource.GetConnection();
           // string sql = "SELECT C\r\n\t.relname,\r\n\tA.attname AS NAME,\r\n\tA.attnotnull AS NOTNULL,\r\n\tformat_type ( A.atttypid, A.atttypmod ) AS TYPE,\r\n\tcol_description ( A.attrelid, A.attnum ) AS COMMENT \r\nFROM\r\n\tpg_class AS C,\r\n\tpg_attribute AS A \r\nWHERE\r\n\tC.relname IN ( SELECT tablename FROM pg_tables WHERE tablename NOT LIKE'pg%' AND tablename NOT LIKE'sql_%' ORDER BY tablename ) \r\nAND A.attrelid = C.oid \r\nAND A.attnum > 0\r\n";
            BuilderFactory.DbConnection=connection1;
            BuilderFactory.DbDataAdapter = hikariDataSource.DataAdapter;
            BuilderFactory.Create("PostgreSQL");
            //builder.DbConnection = connection1;
            //builder.DbDataAdapter = hikariDataSource.DataAdapter;

            //builder.Create();




            //foreach (DataTable item in ds.Tables)
            //{
            //    var ss = new TableInfo() { TableName = item.TableName };

            //    foreach (DataColumn col in item.Columns)
            //    {
            //        ss.ColumnInfos.Add(new ColumnInfo() { ColumnName = col.ColumnName, ColumnType = col.DataType, Description = "编号" });

            //    }
            //    DBCodeEntity.CreateCodeFile(ss, ss.TableName + ".cs", "mytest", "myserver");
            //    // ss.ColumnInfos.Add(new ColumnInfo() { ColumnName = "id", ColumnType = typeof(int), Description = "编号" });
            //    //  ss.ColumnInfos.Add(new ColumnInfo() { ColumnName = "name", ColumnType = typeof(string), Description = "名称" });
            //}

        }
    }
}
