using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SmartXmlSql
{
    /// <summary>
    /// 验证类，无用
    /// </summary>
    public class EmitEntityCompile
    {
        public delegate Dictionary<string, SqlValue> SqlCreateDic<T>( T obj);

        private static ConcurrentDictionary<string, object> dicCache = new ConcurrentDictionary<string, object>();


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
        /// 构造转换动态方法（核心代码）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static DynamicMethod BuildMethod<T>(T obj)
        {
          
            DynamicMethod method = new DynamicMethod("Create"+obj.GetType().Name, MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, typeof(Dictionary<string, SqlValue>),
                    new Type[] { typeof(T) }, typeof(EntityContext).Module, true);
            ILGenerator generator = method.GetILGenerator();
           
            LocalBuilder result = generator.DeclareLocal(typeof(Dictionary<string, SqlValue>));

            generator.Emit(OpCodes.Newobj, typeof(Dictionary<string, SqlValue>).GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);


            var properties = obj.GetType().GetProperties();

            foreach (var column in properties)
            {
                PropertyInfo property = column;
                generator.Emit(OpCodes.Ldloc, result);

                //第二组,属性设置
                // generator.Emit(OpCodes.Ldloc, sqlv);

                generator.Emit(OpCodes.Ldstr, "@" + property.Name.ToLower());
                generator.Emit(OpCodes.Newobj, typeof(SqlValue).GetConstructor(Type.EmptyTypes));
                generator.Emit(OpCodes.Dup);
                generator.Emit(OpCodes.Ldstr, property.PropertyType.Name);
                generator.Emit(OpCodes.Call, typeof(SqlValue).GetProperty("DataType").GetSetMethod());
                generator.Emit(OpCodes.Dup);
                generator.Emit(OpCodes.Ldarg_0);

                generator.Emit(OpCodes.Call, property.GetMethod);//获取值

                if (property.PropertyType.IsEnum)
                {

                    generator.Emit(OpCodes.Unbox_Any, property.PropertyType);

                }
                else
                {
                    var cur = Nullable.GetUnderlyingType(property.PropertyType);
                    if (cur == null)
                    {
                        cur = property.PropertyType;
                    }
                    generator.Emit(OpCodes.Call, ConvertMethods[cur]);//调用强转方法赋值

                }

                generator.Emit(OpCodes.Call, typeof(SqlValue).GetProperty("Value").GetSetMethod());//
                generator.Emit(OpCodes.Call, typeof(Dictionary<string, SqlValue>).GetMethod("set_Item"));//

            }

            generator.Emit(OpCodes.Ldloc, result);
            generator.Emit(OpCodes.Ret);
            return method;
        }


        /// <summary>
        /// 创建委托
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static SqlCreateDic<T> CreateParamMethod<T>(T obj)
        {
            SqlCreateDic<T> entity = null;
            object sql = null;
            if (dicCache.TryGetValue(typeof(T).Name, out sql))
            {
                entity = sql as SqlCreateDic<T>;
            }
            else
            {
                entity = (SqlCreateDic<T>)BuildMethod<T>(obj).CreateDelegate(typeof(SqlCreateDic<T>));
                dicCache[typeof(T).Name] = entity;
            }


            return entity;
        }


    }
}
