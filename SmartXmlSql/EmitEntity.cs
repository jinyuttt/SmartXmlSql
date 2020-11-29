using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace SmartXmlSql
{
   public class EmitEntity
    {
        public delegate void EntityDataTable<T>(Dictionary<string,SqlValue> dr, T obj);

        private static readonly Dictionary<Type, MethodInfo> ConvertMethods = new Dictionary<Type, MethodInfo>()
       {
           {typeof(int),typeof(Convert).GetMethod("ToString",new Type[]{typeof(int) })},
           {typeof(Int16),typeof(Convert).GetMethod("ToString",new Type[]{typeof(Int16) })},
           {typeof(Int64),typeof(Convert).GetMethod("ToString",new Type[]{typeof(Int64) })},
           {typeof(DateTime),typeof(Convert).GetMethod("ToString",new Type[]{typeof(DateTime) })},
           {typeof(decimal),typeof(Convert).GetMethod("ToString",new Type[]{typeof(decimal) })},
           {typeof(Double),typeof(Convert).GetMethod("ToString",new Type[]{typeof(Double) })},
           {typeof(Boolean),typeof(Convert).GetMethod("ToString",new Type[]{typeof(Boolean) })},
           {typeof(string),typeof(Convert).GetMethod("ToString",new Type[]{typeof(string) })}
       };



        /// <summary>
        /// 构造转换动态方法（核心代码），根据assembly可处理datarow和datareader两种转换
        /// </summary>
        /// <typeparam name="T">返回的实体类型</typeparam>
        /// <param name="assembly">待转换数据的元数据信息</param>
        /// <returns>实体对象</returns>
        private static DynamicMethod BuildMethod<T>(DynamicAssembleInfo assembly, T obj)
        {
          
            DynamicMethod method = new DynamicMethod(typeof(T).Name, MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, typeof(T),
                    new Type[] { typeof(Dictionary<string, SqlValue>) }, typeof(EntityContext).Module, true);
            ILGenerator generator = method.GetILGenerator();
            LocalBuilder result = generator.DeclareLocal(typeof(Dictionary<string, SqlValue>));
            generator.Emit(OpCodes.Newobj, typeof(Dictionary<string, SqlValue>).GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);

            LocalBuilder sqlv = generator.DeclareLocal(typeof( SqlValue));
            var properties = obj.GetType().GetProperties();
            //foreach (var p in properties)
            //{
            //    SqlValue value = new SqlValue() { DataType = p.PropertyType.FullName, Value = p.GetValue(arg).ToString() };
            //    dic["@" + p.Name.ToLower()] = value;
            //}
            foreach (var column in properties)
            {
                PropertyInfo property = column;
                var endIfLabel = generator.DefineLabel();
                var tmpIfLabel = generator.DefineLabel();
                generator.Emit(OpCodes.Ldarg_0);
                //第一组，调用AssembleInfo的CanSetted方法，判断是否可以转换
               // generator.Emit(OpCodes.Ldstr, column.ColumnName);
               // generator.Emit(OpCodes.Call, assembly.CanSettedMethod);
              //  generator.Emit(OpCodes.Brfalse, endIfLabel);
                //第二组,属性设置
                generator.Emit(OpCodes.Ldloc, sqlv);
              
                generator.Emit(OpCodes.Ldstr, "@" + column.Name.ToLower());
                generator.Emit(OpCodes.Newobj, typeof(SqlValue).GetConstructor(Type.EmptyTypes));
                generator.Emit(OpCodes.Ldarg_0);

                generator.Emit(OpCodes.Call, column.GetMethod);//获取值
                if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                {
                    if (property.PropertyType.IsEnum)
                    {
                        if (column.PropertyType.Name == "Int32")
                        {
                            generator.Emit(OpCodes.Unbox_Any, property.PropertyType);
                        }
                        
                    }
                    else
                    {
                        LocalBuilder tmp = null;
                        var cur = Nullable.GetUnderlyingType(property.PropertyType);
                        var tmpType = cur;
                        if (cur == null)
                        {
                            cur = property.PropertyType;
                        }
                        if (column.PropertyType.Name == "String" && cur == typeof(decimal))
                        {

                            tmp = generator.DeclareLocal(typeof(object));
                            var tmpBool = generator.DeclareLocal(typeof(bool));
                            generator.Emit(OpCodes.Call, ConvertMethods[typeof(string)]);//调用强转方法转；
                            generator.Emit(OpCodes.Stloc, tmp);//
                            generator.Emit(OpCodes.Ldloc, tmp);//
                            generator.Emit(OpCodes.Call, assembly.CanScientific);//调用判断；
                            generator.Emit(OpCodes.Stloc, tmpBool);//
                            generator.Emit(OpCodes.Ldloc, tmpBool);//
                            generator.Emit(OpCodes.Brfalse_S, tmpIfLabel);//
                            generator.Emit(OpCodes.Ldloc, tmp);//
                            generator.Emit(OpCodes.Call, ConvertMethods[typeof(double)]);//调用强转方法转；
                            generator.Emit(OpCodes.Box, typeof(Double));
                            generator.Emit(OpCodes.Stloc, tmp);
                        }
                        generator.MarkLabel(tmpIfLabel);
                        //
                        if (tmp != null)
                        {
                            generator.Emit(OpCodes.Ldloc, tmp);
                        }
                        generator.Emit(OpCodes.Call, ConvertMethods[cur]);//调用强转方法赋值
                        if (tmpType != null)
                        {
                            generator.Emit(OpCodes.Newobj, property.PropertyType.GetConstructor(new Type[] { tmpType }));
                        }
                    }
                }
                //效果类似  
                else
                {
                    generator.Emit(OpCodes.Castclass, property.PropertyType);
                }
                generator.Emit(OpCodes.Call, property.GetSetMethod());//直接给属性赋值
                //效果类似  Name=row["PName"];
                generator.MarkLabel(endIfLabel);
            }
            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);
            return method;
        }

    }
}
