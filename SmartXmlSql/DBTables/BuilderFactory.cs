using System;
using System.Data;

namespace SmartXmlSql.DBTables
{

    /// <summary>
    /// 输出代码
    /// </summary>
    public class BuilderFactory
    {
        /// <summary>
        /// 名称空间
        /// </summary>
        public static string CodeNameSpace { get; set; } = string.Empty;

        /// <summary>
        /// 输出路径
        /// </summary>
        public static string OutFile { get; set; } = string.Empty;

        /// <summary>
        /// 数据库连接
        /// </summary>
        public static IDbConnection DbConnection { get; set; }

        /// <summary>
        /// 数据库适配器对象
        /// </summary>
        public static  IDbDataAdapter DbDataAdapter { get; set; }

        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="name">数据库类型</param>
        /// <param name="db">数据库名称</param>
        /// <exception cref="Exception"></exception>
        public static void Create(string name,string db="")
        {
            string tmp = " SmartXmlSql.DBTables."+ name + "EntiyBuilder";
         
            Type t=  Type.GetType(tmp);
            if(t == null)
            {
                throw new Exception("没有找到"+t.Name);
            }
            DBEntiyBuilder builder= (DBEntiyBuilder)Activator.CreateInstance(t);
            builder.DBName = db;
            builder.OutFile = OutFile;
            builder.CodeNameSpace = CodeNameSpace;
            builder.DbConnection = DbConnection;
            builder.DbDataAdapter = DbDataAdapter;
            builder.Create();
        }
    }
}
