using Microsoft.Extensions.Caching.Memory;
using SmartXmlSql.Cache;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace SmartXmlSql
{

    /// <summary>
    /// SQL处理器
    /// SmartSqlBuilder+缓存
    /// </summary>
    public class SqlGenerator
    {
        readonly SmartSqlBuilder smartSql = new SmartSqlBuilder();
        private static readonly Lazy<SqlGenerator> generator = new Lazy<SqlGenerator>();

        /// <summary>
        /// SQL语句参数
        /// </summary>
        private readonly MemoryCache Cache = null;

        /// <summary>
        /// 调用的参数缓存
        /// </summary>
        private readonly MemoryCache CacheParam = null;

        public static SqlGenerator Instance
        {
            get { return generator.Value; }
        }

        public SqlGenerator()
        {
            Cache = new MemoryCache(new MemoryCacheOptions() { ExpirationScanFrequency = TimeSpan.FromMinutes(5) });
            CacheParam = new MemoryCache(new MemoryCacheOptions() { ExpirationScanFrequency = TimeSpan.FromMinutes(1) });
        }
       
        /// <summary>
        /// SQL生成
        /// </summary>
        /// <param name="args">实体对象，SQL参数赋值使用</param>
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
            Dictionary<string, SqlValue> dic = new();

            //参数是类并且不需要生成in或者批量SQL
            if (args.Length == 1 && args[0].GetType().IsClass && args[0].GetType() != typeof(string) && sql.Key != "List" && sql.Key != "Batch" && sql.Key != "Array")
            {
                
                //遍历属性生成替换SQL
                object arg = args[0];
                var dlg=  EmitObjectCompile.CreateParamMethod(arg);
                dic= dlg(arg);
              
            }
            else
            {
                if (sql.Key != "List" && sql.Key != "Batch"&& sql.Key != "Entity"&& sql.Key != "Array")
                {
                    string strKey = string.Join("-", mth.ReflectedType.Name, mth.Name);
                    var lstKV = this.GetMthParam(strKey);
                    if (lstKV == null)
                    {
                        //遍历参数替换
                        List<SqlKV> lst = new List<SqlKV>();
                        ParameterInfo[] parameters = mth.GetParameters();
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            var p = parameters[i];
                            SqlValue value = new SqlValue() { DataType = p.ParameterType.Name, Value = args[i] };
                            if (p.ParameterType.FullName== null)
                            {
                                //说明是泛型;
                                value.DataType = args[i].GetType().Name; 

                            }
                            dic["@" + p.Name.ToLower()] = value;
                            lst.Add(new SqlKV() { Key = "@" + p.Name.ToLower(), Value = value });
                        }
                        this.SetMthParam(strKey, lst);
                    }
                    else
                    {
                        for (int i = 0; i > lstKV.Count; i++)
                        {
                            var tmp = lstKV[i];
                            tmp.Value.Value = args[i].ToString();//只是替换值
                            dic[tmp.Key] = tmp.Value;
                        }
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
                //替换SQL参数模板，生成参数化SQL
                string tmp = sqlPi[0].Substring(1);//取出替换字段部分
                sql.SQL = sql.SQL.Replace("<$" + tmp + ">", p.ToString());
                sql.SQL = sql.SQL.Replace("@" + tmp, v.ToString());//取出替参数部分
                sqlPi = SearchParam(sql.SQL);//生成后直接再次处理
            }
            else if (sql.Key == "Array" && lstRp.Count == 1 && args.Length == 1 && args[0].GetType().IsArray)
            {
                //有替换部分，并且是数组
                //形如：select * from XX where id in(<$p>)
                StringBuilder p = new StringBuilder();
                bool isStr = false;//是否是字符串数组

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
                //部分属性
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

                if (!string.IsNullOrEmpty(values))
                {
                    IList arg = args[0] as IList;
                    var dlg = EmitObjectCompile.CreateParamMethod(arg[0]);

                    List<string> rps = SearchReplace(values);
                    SqlValue v;
                    foreach (var entity in arg)
                    {
                        //一组
                        dic = dlg(entity);
                        builder.Append("(");
                        foreach (var p in rps)
                        {
                            string ky = "@" + p.Substring(2).TrimEnd('>').ToLower().Trim();
                            if (dic.TryGetValue(ky, out v))
                            {
                                if (v.DataType == "String" || v.DataType == "DateTime")
                                {
                                    builder.AppendFormat("'{0}',", v.Value);
                                }
                                else
                                {
                                    builder.AppendFormat("{0},", v.Value);
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
                //全部属性
                IList arg = args[0] as IList;
                var dlg = EmitObjectCompile.CreateParamMethod(arg[0]);

                StringBuilder builder = new StringBuilder();
                StringBuilder v = new StringBuilder();
                foreach (var p in arg)
                {
                    dic = dlg(p);
                    v.Append("(");
                    builder.Clear();
                    foreach (var kv in dic)
                    {
                        builder.AppendFormat("{0},", kv.Key.Substring(1));
                        if (kv.Value.DataType == "String" || kv.Value.DataType == "DateTime")
                        {
                            v.AppendFormat("'{0}',", kv.Value.Value);
                        }
                        else
                        {
                            v.AppendFormat("{0},", kv.Value.Value);
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
                if(builder.Length>0)
                {
                    builder.Remove(builder.Length - 1, 1);
                }
                if(v.Length>0)
                {
                    v.Remove(v.Length - 1, 1);
                }
                sql.SQL = sql.SQL.Replace(field, "(" + builder.ToString() + ")");
                sql.SQL = sql.SQL.Replace(values, v.ToString());
            }

            //如果没有处理过以上分支
            else
            {
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
                        sql.SQL = sql.SQL.Replace(p.Substring(1), value.Value.ToString());
                    }
                }
            }
            //按照SQL参数输出
            Dictionary<string, SqlValue> dicParam = new();
            foreach (var p in sqlPi)
            {
                SqlValue value = null;
                if (dic.TryGetValue(p.ToLower(), out value))
                {
                    dicParam[p] = value;
                }
            }

            //检查like
            string strSQl = sql.SQL;
            int index = strSQl.IndexOf("like @");
            while (index > -1)
            {
                string tmp = strSQl.Substring(index + 5);
                int indexlast = tmp.IndexOf(" ");
                string key = "";
                if (indexlast > -1)
                {
                    key = tmp.Substring(0, indexlast);
                }
                else
                {
                    key = tmp;
                }
                   
                SqlValue sqlValue = null;
                if (dicParam.TryGetValue(key, out sqlValue))
                {
                    sqlValue.Value = string.Format("%{0}%", sqlValue.Value);
                }
                strSQl = tmp;//截取赋值
                index = strSQl.IndexOf("like @");
            }

            
            BuilderContent content = new BuilderContent() { Sql = sql.SQL, SqlParam = dicParam };
            return content;
        }
         
        /// <summary>
        /// 提取SQL参数
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

        /// <summary>
        /// 获取SQL缓存的参数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private List<string> GetSqlParam(string sql)
        {
           return Cache.Get<List<string>>(sql);
        }

       /// <summary>
       /// 按照SQL缓存解析的SQL参数
       /// </summary>
       /// <param name="sql"></param>
       /// <param name="lst"></param>
        private void SetSqlParam(string sql,List<string> lst)
        {
            Cache.Set(sql, lst, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(15)
            });
        }

        /// <summary>
        /// 获取调用方法的缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private List<SqlKV> GetMthParam(string key)
        {
            return CacheParam.Get<List<SqlKV>>(key);
        }

       /// <summary>
       /// 缓存调用方法的参数
       /// </summary>
       /// <param name="key">方法名称</param>
       /// <param name="lst">方法参数</param>
        private void SetMthParam(string key, List<SqlKV> lst)
        {
            CacheParam.Set(key, lst, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(5)
            });
        }


    }
}
