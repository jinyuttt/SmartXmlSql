namespace SmartXmlSql.DBTables
{
    /// <summary>
    /// MySql数据库
    /// </summary>
    public class MySqlEntiyBuilder:DBEntiyBuilder
    {
        MySqlEntiyBuilder
            ()
        {
            this.AllTableSql = string.Format(" select table_name as tablename from information_schema.tables where table_schema = '{0}'", this.DBName);
        }
    }
}
