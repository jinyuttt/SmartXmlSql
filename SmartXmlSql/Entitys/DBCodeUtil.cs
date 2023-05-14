using System.Collections.Generic;
using System.Data;
using System.IO;

namespace SmartXmlSql.Entitys
{
    /// <summary>
    /// 生成文件
    /// </summary>
    public class DBCodeUtil
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ds">数据库表信息</param>
        /// <param name="clsNameSpace">名称空间</param>
        /// <param name="dir">输出文件夹</param>
        /// <param name="lst">列注释信息</param>
        public static void CreateDBEntity(DataSet ds, string clsNameSpace, string dir,List<ColumnDescription> lst=null)
        {
            DirectoryInfo directory = new DirectoryInfo(dir);
            if (!directory.Exists)
            {
                directory.Create();
            }

            foreach (DataTable item in ds.Tables)
            {
                var table = new TableInfo() { TableName = item.TableName };
                ColumnDescription desp=new ColumnDescription();
                if (lst != null)
                {
                     desp = lst.Find(p => p.TableName == table.TableName);
                }
                foreach (DataColumn col in item.Columns)
                {
                    string cur = "";
                    if(desp != null)
                    {
                        var colum = desp.ColumnInfos.Find(p => p.ColumnName == col.ColumnName);
                        if(colum != null) { 
                        cur= colum.Description;
                        }
                       
                    }
                    table.ColumnInfos.Add(new ColumnInfo() { ColumnName = col.ColumnName, ColumnType = col.DataType, Description = cur });

                }
                
                string outFile = Path.Combine(dir, table.TableName + ".cs");
                DBCodeEntity.CreateCodeFile(table, outFile, table.TableName, clsNameSpace);
            }
           

        }
    }
}
