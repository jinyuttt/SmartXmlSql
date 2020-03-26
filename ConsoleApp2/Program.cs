using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            Create();
        }
        public static void Create()
        {
            var type = typeof(Person);

            var asmName = new AssemblyName("MyClass");

            //首先就需要定义一个程序集
            var defAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave);
            var defModuleBuilder = defAssembly.DefineDynamicModule("MyModule", "MyAssembly.dll");
            //定义一个类
            var defClassBuilder = defModuleBuilder.DefineType("MyClass", TypeAttributes.Public);
            //定义一个方法
            var methodBldr = defClassBuilder.DefineMethod("MyMethod",
                MethodAttributes.Public,
                  typeof(bool),//返回类型
                  new Type[] { typeof(object) }//参数的类型
             );
            var generator = methodBldr.GetILGenerator();
            generator.DeclareLocal(type);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Isinst, type);
            generator.Emit(OpCodes.Stloc_0);
            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Call, type.GetProperty("Name").GetGetMethod());
            generator.Emit(OpCodes.Call, typeof(string).GetMethod("IsNullOrEmpty"));
            generator.Emit(OpCodes.Ret);//方法结束，返回
            //创建类型
            defClassBuilder.CreateType();

            //保存程序集
            defAssembly.Save("MyAssemblydll");
        }

    }
}
