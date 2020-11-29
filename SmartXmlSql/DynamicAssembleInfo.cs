using System;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SmartXmlSql
{
    internal class DynamicAssembleInfo
    {
        public string MethodName;
        public Type SourceType;
        public MethodInfo CanSettedMethod;
        public MethodInfo GetValueMethod;
        public MethodInfo EnumConvert;
        public MethodInfo CanScientific;
        const string scientific_rule = "^[+-]?((\\d+\\.?\\d*)|(\\.\\d+))[Ee][+-]?\\d+$";
        public DynamicAssembleInfo(Type type)
        {
            SourceType = type;
            MethodName = "Convert" + type.Name + "To";
            CanSettedMethod = this.GetType().GetMethod("CanSetted", new Type[] { type, typeof(string) });
            GetValueMethod = type.GetMethod("get_Item", new Type[] { typeof(string) });
            EnumConvert = this.GetType().GetMethod("ConvertEnum", new Type[] { typeof(object), typeof(Type) });
            CanScientific = this.GetType().GetMethod("ScientificNotation", new Type[] { typeof(string) });
        }



        /// <summary>
        /// 判断datareader是否存在某字段并且值不为空
        /// 已经改为一次验证
        /// </summary>
        /// <param name="dr">当前的datareader</param>
        /// <param name="name">字段名</param>
        /// <returns></returns>
        public static bool CanSetted(IDataRecord dr, string name)
        {
            return !dr[name].Equals(DBNull.Value);
        }

        /// <summary>
        /// 判断datarow所在的datatable是否存在某列并且值不为空
        /// 已经修改成了一次性验证
        /// </summary>
        /// <param name="dr">当前datarow</param>
        /// <param name="name">字段名</param>
        /// <returns></returns>
        public static bool CanSetted(DataRow dr, string name)
        {
            return !dr.IsNull(name);
        }

        /// <summary>
        /// 枚举名称转枚举类型
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ConvertEnum(object obj, Type type)
        {
            return Enum.Parse(type, obj.ToString());
        }

        /// <summary>
        /// 判断是否是科学计数法
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool ScientificNotation(object obj)
        {
            string str = (string)obj;
            Regex reg = new Regex("^[+-]?((\\d+\\.?\\d*)|(\\.\\d+))[Ee][+-]?\\d+$", RegexOptions.IgnoreCase);
            return reg.IsMatch(str);
        }
    }
}