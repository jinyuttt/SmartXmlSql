using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SmartXmlSql
{

    /// <summary>
    /// 创建委托代理
    /// 内部有缓存
    /// </summary>
    public class EmitObjectCompile
    {
        public delegate Dictionary<string, SqlValue> SqlCreateDic<T>(T obj);

        /// <summary>
        /// 缓存
        /// </summary>
        private static ConcurrentDictionary<string, SqlCreateDic<object>> dicCache = new ConcurrentDictionary<string, SqlCreateDic<object>>();


        /// <summary>
        /// 转换初始化
        /// </summary>
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
        /// 构造转换动态方法（核心代码），以object作为参数  
        /// 方法定义类似  public Dictionary<string, SqlValue> CreatePerson(object obj)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static DynamicMethod BuildMethod(object  obj)
        {

            DynamicMethod method = new DynamicMethod("Create" + obj.GetType().Name, MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, typeof(Dictionary<string, SqlValue>),
                    new Type[] { typeof(object) }, typeof(EntityContext).Module, true);
            ILGenerator generator = method.GetILGenerator();

            LocalBuilder varTmp = generator.DeclareLocal(obj.GetType());
            LocalBuilder result = generator.DeclareLocal(typeof(Dictionary<string, SqlValue>));

            //Person person = P_0 as Person;
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Isinst, obj.GetType());
            generator.Emit(OpCodes.Stloc, varTmp);
            
            //Dictionary<string, SqlValue> dictionary = new Dictionary<string, SqlValue>();
            generator.Emit(OpCodes.Newobj, typeof(Dictionary<string, SqlValue>).GetConstructor(Type.EmptyTypes));
            generator.Emit(OpCodes.Stloc, result);

            // 	dictionary["@id"] = new SqlValue
            // 	{
            // 		DataType = "Int32",
            // 		Value = Convert.ToString(person.Id)
            // 	};
            var properties = obj.GetType().GetProperties();

            foreach (var column in properties)
            {
                PropertyInfo property = column;
                generator.Emit(OpCodes.Ldloc, result);
                generator.Emit(OpCodes.Ldstr, "@" + property.Name.ToLower());
                generator.Emit(OpCodes.Newobj, typeof(SqlValue).GetConstructor(Type.EmptyTypes));
                generator.Emit(OpCodes.Dup);
                generator.Emit(OpCodes.Ldstr, property.PropertyType.Name);
                generator.Emit(OpCodes.Call, typeof(SqlValue).GetProperty("DataType").GetSetMethod());
                generator.Emit(OpCodes.Dup);
                generator.Emit(OpCodes.Ldloc, varTmp);

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
        /// 创建对应的委托
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static SqlCreateDic<object> CreateParamMethod(object obj)
        {
            SqlCreateDic<object> entity = null;
           
            if (dicCache.TryGetValue(obj.GetType().Name, out entity))
            {
                return entity;
            }
            else
            {
                entity = (SqlCreateDic<object>)BuildMethod(obj).CreateDelegate(typeof(SqlCreateDic<object>));
                dicCache[obj.GetType().Name] = entity;
            }
            return entity;
        }

    }
}
