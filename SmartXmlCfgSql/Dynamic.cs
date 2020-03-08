#region   文件版本注释
/************************************************************************
*CLR版本  ：4.0.30319.42000
*项目名称 ：SmartXmlSql
*项目描述 ：
*命名空间 ：SmartXmlSql
*文件名称 ：Dynamic.cs
* 功能描述 ：Dynamic
* 创建时间 ：2020
*版本号   :   2020|V1.0.0.0 
---------------------------------------------------------------------
* Copyright @ jinyu 2020. All rights reserved.
---------------------------------------------------------------------

***********************************************************************/
#endregion



using System.Linq.Dynamic;

using System.Reflection.Emit;

namespace SmartXmlCfgSql
{
    public delegate bool IsNotEmpty<T>(T obj);
    public class Dynamic : Tag
    {
        private IsNotEmpty<object> empty;
        private System.Linq.Expressions.LambdaExpression expression = null;
        public static IsNotEmpty<T> CreateBuilder<T>(T obj,string name)
        {
          //  var dynamicBuilder = new TDynamicBuilder<T>();
             var type = typeof(T);
            //定义一个名为DynamicCreate的动态方法，返回值typof(T)，参数typeof(IDataRecord)
            var method = new DynamicMethod("DynamicCreate", typeof(bool), new[] { typeof(T) });
            var generator = method.GetILGenerator();//创建一个MSIL生成器，为动态方法生成代码
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Call, type.GetProperty(name).GetGetMethod());
            generator.Emit(OpCodes.Call, typeof(string).GetMethod("IsNullOrEmpty"));
            generator.Emit(OpCodes.Ret);//方法结束，返回

            //完成动态方法的创建，并且创建执行该动态方法的委托，赋值到全局变量handler,handler在Build方法里Invoke
            IsNotEmpty<T> dynamicBuilder = (IsNotEmpty<T>)method.CreateDelegate(typeof(IsNotEmpty<T>));
            return dynamicBuilder;
        }


        public override void BuildSql()
        {
            if (Property == "IsNotEmpty")
            {
                if(empty==null)
                {
                    empty = CreateBuilder(this.Statement.SqlContext.Context,this.Sql);
                }
                empty.Invoke(this.Statement.SqlContext.Context);
            }
            else if(expression==null)
            {
                object obj = Statement.SqlContext.Context;
                expression = DynamicExpression.ParseLambda(obj.GetType(), typeof(bool), this.Sql);
            }
             
        }
        public override string GetSql()
        {
            bool r = false;
            if(empty!=null)
            {
                r = empty.Invoke(Statement.SqlContext.Context);
              
            }
            else
            {
                 r = (bool)expression.Compile().DynamicInvoke(Statement.SqlContext.Context);
                
            }
            if(r)
            {
                //本节点SQL是判断条件，返回父节点的SQL
                return this.Parent.Sql;
            }
            return "";
        }
    }
}
