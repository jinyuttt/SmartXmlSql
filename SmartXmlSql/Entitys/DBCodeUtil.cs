using System.Collections.Generic;
using System.IO;

namespace SmartXmlSql.Entitys
{
    /// <summary>
    /// 生成文件
    /// </summary>
    internal class DBCodeUtil
    {
        /// <summary>
        /// 通过表信息生成源码
        /// </summary>
        /// <param name="lst">数据库表信息</param>
        /// <param name="clsNameSpace">名称空间</param>
        /// <param name="dir">输出文件夹</param>
        public static void CreateDBEntity(List<TableInfo> lst,string clsNameSpace,string dir)
        {
            DirectoryInfo directory =new DirectoryInfo(dir);
            if(!directory.Exists)
            {
                directory.Create();
            }
            foreach (TableInfo table in lst)
            {
                string outFile=Path.Combine(dir, table.TableName+".cs");
                DBCodeEntity.CreateCodeFile(table, outFile,table.TableName,clsNameSpace);
            }
           
        }
    }
}
