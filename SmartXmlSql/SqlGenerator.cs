using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace SmartXmlSql
{

    /// <summary>
    /// SQL处理器
    /// </summary>
    public  class SqlGenerator
    {
        readonly SmartSqlBuilder smartSql = new SmartSqlBuilder();
        private static Lazy<SqlGenerator> generator = new Lazy<SqlGenerator>();

        /// <summary>
        /// SQL语句参数
        /// </summary>
        private readonly MemoryCache Cache = null;

        private readonly MemoryCache CacheParam = null;

        public static SqlGenerator Instance
        {
            get { return generator.Value; }
        }

        public SqlGenerator()
        {
            Cache = new MemoryCache(new MemoryCacheOptions() { ExpirationScanFrequency = TimeSpan.FromMinutes(5) });
            CacheParam = new MemoryCache(new MemoryCacheOptions() { ExpirationScanFrequency = TimeSpan.FromMinutes(5) });
        }
       
        /// <summary>
        /// SQL生成
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public BuilderContent Builder(params object[] args)
        {
            StackTrace stackTrace = new StackTrace();
            var mth = stackTrace.GetFrame(1).GetMethod();
            string className = mth.ReflectedType.Name;
            string name = mth.Name;
            SqlDef sql = null;
            if (args.Length == 0)
            {
                sql = smartSql.Build(className, name);
            }
            else
            {
                sql = smartSql.Build(className, name, args[0]);
            }

            sql.SQL = SQLRegular(sql.SQL);
            //SQL参数提取
            Dictionary<string, SqlValue> dic = new Dictionary<string, SqlValue>();

            //参数是类并且不需要生成in或者批量SQL
            if (args.Length == 1 && args[0].GetType().IsClass && args[0].GetType() != typeof(string) && sql.Key != "List" && sql.Key != "Batch")
            {
                
                //遍历属性生成替换SQL
                object arg = args[0];
                var dlg=  EmitEntity.CreateParamMethod<object>(arg);
                dic= dlg(arg);
                //var properties = arg.GetType().GetProperties();
                //foreach (var p in properties)
                //{
                //    SqlValue value = new SqlValue() { DataType = p.PropertyType.FullName, Value = p.GetValue(arg).ToString() };
                //    dic["@" + p.Name.ToLower()] = value;
                //}
            }
            else
            {
                var lstKV = this.GetMthParam(mth.Name);
                if (lstKV == null)
                {
                    //遍历参数替换
                    List<SqlKV> lst = new List<SqlKV>();
                    ParameterInfo[] parameters = mth.GetParameters();
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var p = parameters[i];
                        SqlValue value = new SqlValue() { DataType = p.ParameterType.FullName, Value = args[i].ToString() };
                        dic["@" + p.Name.ToLower()] = value;
                        lst.Add(new SqlKV() { Key = "@" + p.Name.ToLower(), Value = value });

                    }
                    this.SetMthParam(mth.Name, lst);
                }
                else
                {
                    for(int i=0;i>lstKV.Count;i++)
                    {
                        var tmp = lstKV[i];
                        tmp.Value.Value = args[i].ToString();//只是替换值
                        dic[tmp.Key] = tmp.Value;
                    }
                }
            }
            //获取SQL参数结构化

            List<string> sqlPi = null;
            sqlPi = GetSqlParam(sql.SQL);
            if(sqlPi==null)
            {
                sqlPi = SearchParam(sql.SQL);
                SetSqlParam(sql.SQL, sqlPi);
            }
            List<string> lstRp = SearchReplace(sql.SQL);
            Dictionary<string, SqlValue> dicParam = new Dictionary<string, SqlValue>();
            if (sql.Key == "Entity" && sqlPi.Count == 1)
            {
                //形如：insert into kk (<$p>) values(@p) 
                //生成参数化SQL语句
                //按照参数或者实体属性替换
                StringBuilder p = new StringBuilder();
                StringBuilder v = new StringBuilder();
                foreach (var kv in dic)
                {
                    p.Append(kv.Key.Substring(1) + ",");
                    v.Append(kv.Key + ",");

                }
                p.Remove(p.Length - 1, 1);
                v.Remove(v.Length - 1, 1);
                //
                string tmp = sqlPi[0].Substring(1);
                sql.SQL = sql.SQL.Replace("<$" + tmp + ">", p.ToString());
                sql.SQL = sql.SQL.Replace("@" + tmp, v.ToString());
                sqlPi = SearchParam(sql.SQL);//生成后直接再次处理
            }
            else if (sql.Key == "Array" && lstRp.Count == 1 && args.Length == 1 && args[0].GetType().IsArray)
            {

                //形如：select * from XX where id in(<$p>)
                StringBuilder p = new StringBuilder();
                bool isStr = false;

                Array array = (Array)args[0];
                if (array.GetValue(0).GetType() == typeof(string))
                {
                    isStr = true;
                }
                foreach (var arg in array)
                {
                    if (isStr)
                    {
                        p.Append("'" + arg + "',");
                    }
                    else
                    {
                        p.Append(arg + ",");
                    }
                }
                p.Remove(p.Length - 1, 1);
                //
                lstRp = SearchReplace(sql.SQL);
                string rpin = "";
                foreach (var rp in lstRp)
                {
                    rpin = rp;
                    if (rp.StartsWith("$"))
                    {
                        break;
                    }
                }
                sql.SQL = sql.SQL.Replace(rpin, p.ToString());
            }
            else if (sql.Key == "List")
            {
                //insert into XX(XXX,XXX,XXX)values(<$YYY>,<$YYY>,<$YYY>)
                //
                //按照SQL参数拼接批量插入：这里不能使用参数，而是整体取出values后面部分
                List<string> templelte = SearchValues(sql.SQL);
                string values = "";
                StringBuilder builder = new StringBuilder();
                foreach (string str in templelte)
                {
                    if (str.Contains("<$"))
                    {
                        values = str;
                        break;
                    }
                }
                Dictionary<string, PropertyInfo> dicpro = new Dictionary<string, PropertyInfo>();
                if (!string.IsNullOrEmpty(values))
                {
                    IList arg = args[0] as IList;
                    var properties = arg[0].GetType().GetProperties();
                    foreach (var p in properties)
                    {

                        dicpro["@" + p.Name.ToLower()] = p;
                    }
                    List<string> rps = SearchReplace(values);
                    PropertyInfo v;
                    foreach (var entity in arg)
                    {
                        //一组
                        builder.Append("(");
                        foreach (var p in rps)
                        {
                            string ky = "@" + p.Substring(2).TrimEnd('>').ToLower().Trim();
                            if (dicpro.TryGetValue(ky, out v))
                            {
                                if (v.PropertyType == typeof(string) || v.PropertyType == typeof(DateTime))
                                {
                                    builder.AppendFormat("'{0}',", v.GetValue(entity));
                                }
                                else
                                {
                                    builder.AppendFormat("{0},", v.GetValue(entity));
                                }

                            }

                        }
                        builder.Remove(builder.Length - 1, 1);
                        builder.Append("),");
                    }
                    builder.Remove(builder.Length - 1, 1);//移除最后一个逗号
                    sql.SQL = sql.SQL.Replace(values, builder.ToString());

                }
            }

            else if (sql.Key == "Batch")
            {

                //形如：insert into kk (<$p>) values(@p) 
                IList arg = args[0] as IList;
                var properties = arg[0].GetType().GetProperties();
                Dictionary<string, PropertyInfo> dicpro = new Dictionary<string, PropertyInfo>();
                foreach (var p in properties)
                {

                    dicpro[p.Name.ToLower()] = p;
                }

                StringBuilder builder = new StringBuilder();
                StringBuilder v = new StringBuilder();
                foreach (var p in arg)
                {
                    v.Append("(");
                    builder.Clear();
                    foreach (var kv in dicpro)
                    {
                        builder.AppendFormat("{0},", kv.Key);
                        if (kv.Value.PropertyType == typeof(string) || kv.Value.PropertyType == typeof(DateTime))
                        {
                            v.AppendFormat("'{0}',", kv.Value.GetValue(p));
                        }
                        else
                        {
                            v.AppendFormat("{0},", kv.Value.GetValue(p));
                        }
                    }
                    v.Remove(v.Length - 1, 1);
                    v.Append("),");
                }
                //
                var lst = SearchValues(sql.SQL);
                string values = "";
                string field = "";
                foreach (var str in lst)
                {

                    if (str.Contains("<$"))
                    {
                        field = str;

                    }
                    else
                    {
                        values = str;
                    }
                }
                //
                sql.SQL = sql.SQL.Replace(field, "(" + builder.ToString() + ")");
                sql.SQL = sql.SQL.Replace(values, v.ToString());
            }
            if (lstRp == null)
            {
                //没有替换过则替换
                lstRp = SearchReplace(sql.SQL);
                foreach (var p in lstRp)
                {
                    //去除标记
                    if (!p.StartsWith("$"))
                    {
                        continue;
                    }
                    string key = "@" + p.Substring(1);
                    SqlValue value;
                    if (dic.TryGetValue(key, out value))
                    {
                        //替换同名参数
                        sql.SQL = sql.SQL.Replace(p.Substring(1), value.Value);
                    }

                }

            }
            //按照SQL参数输出
            foreach (var p in sqlPi)
            {
                SqlValue value = null;
                if (dic.TryGetValue(p.ToLower(), out value))
                {
                    dicParam[p] = value;
                }
            }

            BuilderContent content = new BuilderContent() { Sql = sql.SQL, SqlParam = dicParam };
            return content;
        }
         
        /// <summary>
        /// SQL参数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private List<string>  SearchParam(string sql)
        {
            List<string> sqlPi = new List<string>();
            Regex regex = new Regex(@"(?<!@)[^\w$#@]@(?!@)[\w$#@]+");
            MatchCollection matchs = regex.Matches(sql);
            foreach (Match match in matchs)
            {
                sqlPi.Add(match.Groups[0].Value.Substring(match.Groups[0].Value.IndexOf("@")));
            }
            return sqlPi;
        }

        /// <summary>
        /// SQL替换词(<$p>)
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private List<string> SearchReplace(string sql)
        {
            //version = Regex.Replace(str, @"(.*\()(.*)(\).*)", "$2"); //小括号()

            //Regex rgx = new Regex(@"(?i)(?<=\[)(.*)(?=\])");//中括号[]
            //string tmp = rgx.Match(CvoName).Value;//中括号[]

            //string sheetData = Regex.Match(LinkData, @"\{(.*)\}", RegexOptions.Singleline).Groups[1].Value;//大括号{}
            List<string> sqlPi = new List<string>();
          
            Regex regex = new Regex(@"<([^<>]*)>");
            MatchCollection matchs = regex.Matches(sql);
            foreach (Match match in matchs)
            {
                sqlPi.Add(match.Groups[0].Value);
            }
            return sqlPi;
        }

        /// <summary>
        /// 提取小括号中部分(包括小括号)
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private List<string> SearchValues(string sql)
        {
            List<string> sqlPi = new List<string>();
            Regex regex = new Regex(@"\([^\(\)]*?\)");//小括号
            MatchCollection matchs = regex.Matches(sql);
            foreach (Match match in matchs)
            {
                sqlPi.Add(match.Groups[0].Value);
            }
            return sqlPi;
        }


        /// <summary>
        /// 规整SQL,去除多余空格
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private string SQLRegular(string sql)
        {
            string str = "";
            str = Regex.Replace(sql, "\\s{2,}", " ");
            return str.ToLower().Trim();
        }


        private List<string> GetSqlParam(string sql)
        {
           return Cache.Get<List<string>>(sql);
        }

        private void SetSqlParam(string sql,List<string> lst)
        {
            Cache.Set(sql, lst, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(15)
            });
        }

        private List<SqlKV> GetMthParam(string key)
        {
            return CacheParam.Get<List<SqlKV>>(key);
        }

        private void SetMthParam(string key, List<SqlKV> lst)
        {
            CacheParam.Set(key, lst, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(15)
            });
        }


    }
}
