namespace SmartXmlSql.DBTables
{
    /// <summary>
    /// PostgreSQL数据库
    /// </summary>
    public class PostgreSQLEntiyBuilder : DBEntiyBuilder
    {
        public PostgreSQLEntiyBuilder()
        {

            AllTableSql = "SELECT tablename  FROM  pg_tables WHERE  tablename NOT LIKE'pg%'  AND tablename NOT LIKE'sql_%'  ORDER BY tablename; ";

        }


    }
}
