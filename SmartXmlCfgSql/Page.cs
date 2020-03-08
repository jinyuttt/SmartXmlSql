#region   文件版本注释
/************************************************************************
*CLR版本  ：4.0.30319.42000
*项目名称 ：SmartXmlSql
*项目描述 ：
*命名空间 ：SmartXmlSql
*文件名称 ：Page.cs
* 功能描述 ：Page
* 创建时间 ：2020
*版本号   :   2020|V1.0.0.0 
---------------------------------------------------------------------
* Copyright @ jinyu 2020. All rights reserved.
---------------------------------------------------------------------

***********************************************************************/
#endregion



using System.Linq.Expressions;

namespace SmartXmlCfgSql
{
    /// <summary>
    /// 分页使用计算表达式
    /// </summary>
    public class Page:Tag
    {
        LambdaExpression expression = null;
        public override void BuildSql()
        {
           if(expression==null)
            {
                ParameterExpression x = Expression.Parameter(typeof(int), "pageize");
                ParameterExpression y = Expression.Parameter(typeof(int), "pagenum");
                 expression =System.Linq.Dynamic. DynamicExpression.ParseLambda(new System.Linq.Expressions.ParameterExpression[] { x, y }, null, this.Sql);
            }
        }
        public override string GetSql()
        {
            return expression.Compile().DynamicInvoke(null).ToString();
        }
    }
}
