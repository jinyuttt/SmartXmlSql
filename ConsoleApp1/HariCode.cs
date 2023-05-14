using Hikari;

using SmartXmlSql;
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
            var ds=   hikariDataSource.ExecuteQuery("select * from student");
            foreach(DataTable item in ds.Tables)
            {
                var ss = new TableInfo() { TableName = item.TableName };

                foreach(DataColumn col in item.Columns)
                {
                    ss.ColumnInfos.Add(new ColumnInfo() { ColumnName = col.ColumnName, ColumnType = col.DataType, Description = "编号" });

                }
                DBCodeEntity.CreateCodeFile(ss, ss.TableName+".cs", "mytest", "myserver");
               // ss.ColumnInfos.Add(new ColumnInfo() { ColumnName = "id", ColumnType = typeof(int), Description = "编号" });
              //  ss.ColumnInfos.Add(new ColumnInfo() { ColumnName = "name", ColumnType = typeof(string), Description = "名称" });
            }

        }
    }
}
