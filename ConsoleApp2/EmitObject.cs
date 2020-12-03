using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class EmitObject
    {
        public delegate Dictionary<string, SqlValue> SqlCreateDic<T>(T obj);

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
        private static DynamicMethod BuildMethod(object obj)
        {

            var assemblyName = new AssemblyName("Kitty");
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("KittyModule", "Kitty.dll");
            var typeBuilder = moduleBuilder.DefineType("HelloKittyClass", TypeAttributes.Public);
            var method = typeBuilder.DefineMethod("SayHelloMethod", MethodAttributes.Public | MethodAttributes.Static,
   typeof(Dictionary<string, SqlValue>),
   new Type[] { typeof(object) });

         
            ILGenerator generator = method.GetILGenerator();
            LocalBuilder varTmp = generator.DeclareLocal(obj.GetType());
            LocalBuilder result = generator.DeclareLocal(typeof(Dictionary<string, SqlValue>));


            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Isinst,obj.GetType());
            generator.Emit(OpCodes.Stloc, varTmp);

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
            // generator.Emit(OpCodes.Stloc, resultTmp);
            // generator.Emit(OpCodes.Ldloc, resultTmp);
            generator.Emit(OpCodes.Ret);

            typeBuilder.CreateType();
            assemblyBuilder.Save("Kitty.dll");
            return null;
        }


        public static SqlCreateDic<object> CreateParamMethod<T>(T obj)
        {
            SqlCreateDic<object> entity = null;
            object sql = null;
            if (dicCache.TryGetValue(typeof(T).Name, out sql))
            {
                entity = sql as SqlCreateDic<object>;
            }
            else
            {
                entity = (SqlCreateDic<object>)BuildMethod(obj).CreateDelegate(typeof(SqlCreateDic<object>));
                dicCache[typeof(T).Name] = entity;
            }


            return entity;
        }


    }
}
