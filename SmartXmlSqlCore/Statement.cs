#region   文件版本注释
/************************************************************************
*CLR版本  ：4.0.30319.42000
*项目名称 ：SmartXmlSql
*项目描述 ：
*命名空间 ：SmartXmlSql
*文件名称 ：Statement.cs
* 功能描述 ：Statement
* 创建时间 ：2020
*版本号   :   2020|V1.0.0.0 
---------------------------------------------------------------------
* Copyright @ jinyu 2020. All rights reserved.
---------------------------------------------------------------------

***********************************************************************/
#endregion



using System.Collections.Generic;

namespace SmartXmlSql
{
    public class Statement
    {
        public Statement()
        {
            Tags = new List<ITag>();
            Child = new List<ITag>();
        }

        public IList<ITag> Tags { get; set; }

        public IList<ITag> Child { get; set; }
   
        public string Key { get; set; }

        public string DB { get; set; }

        public AcessType Acess { get; set; }

        //准备扩展的字段
        public  SqlContext SqlContext { get; set; }
    }
}
