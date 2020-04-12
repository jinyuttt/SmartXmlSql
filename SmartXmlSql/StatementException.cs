#region   文件版本注释
/************************************************************************
*CLR版本  ：4.0.30319.42000
*机器名称 ：DESKTOP-730PC6V
*项目名称 ：SmartXmlSql
*项目描述 ：
*命名空间 ：SmartXmlSql
*文件名称 ：StatementException.cs
*版本号   :   2020|V1.0.0.0 
---------------------------------------------------------------------
* Copyright @ jinyu 2020. All rights reserved.
---------------------------------------------------------------------

***********************************************************************/
#endregion


using System;

namespace SmartXmlSql
{

    /// <summary>
    /// 解析异常
    /// </summary>
    public class StatementException:Exception
    {
       public string ErrorCode { get; set; }

        public string  ErrorParam { get; set; }

        public StatementException(string code,string errNode, string Msg):base(Msg)
        {
            ErrorCode = code;
            ErrorParam = errNode;
        }
        public override string ToString()
        {
            return string.Format("{0}:{1} {2}", ErrorCode, ErrorParam, Message);
        }
    }
}
