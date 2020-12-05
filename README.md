SmartXmlSql
==========
XML配置SQL

-----------------------------
使用  
SqlGenerator.Instance.Builder()；传入参数值

-----------------------------
SmartXmlSqlMaper.xsd 提示xml关键字

-----------------------------
升级修改
-----------------------------
1.20200922  
  修改缓存时间，保持15分钟没有使用则移除  
2.20201206  
  增加emit隐射，修复Set节点，增加例子

### 关键字说明

| 关键字    | 含义            | 使用                                                         | 说明                                                         | 样例                                                    |
| --------- | --------------- | ------------------------------------------------------------ | ------------------------------------------------------------ | ------------------------------------------------------- |
| Statement | 一个SQL语句节点 | Id属性对应调用方法;DB属性对应数据库名称;Acess对应SQL语句类型（select，update,bath）;Key对应实体关系 | Key="Entity",用实体全部属性生成SQL,需要配置替换SQL;Key="Array",生成in的部分，方法传入数组,需要配置替换SQL；Key = "List"，生成批量SQL语句，需要配置插入SQL的字段和参数，这样可以取实体的部分属性；Key ="Batch"生成批量语句，此时会用实体的全部属性生成，需要配置替换SQL。Key不是以上关键字，则按照参数匹配。 | 需要配置的替换SQL形如：insert into kk (<$p>) values(@p) |
| Where     | 生成条件语句    | 该关键字下有sql部分语句返回 则会增加where同时移除or，and开头部分 |                                                              |                                                         |
| Choose    | 选择部分        | 子节点必须是When，Othersize,并且要同时存在，可以有default节点 | 子节点相当于IF ...else                                       |                                                         |
| When      | 选择            | 属性可以是IsNotEmpty或者Dynamic，IsNotEmpty判断实体属性是否为空；Dynamic为一般判断 | 为真则使用SQL部分，否则使用Othersize节点的SQL                |                                                         |
| Othersize | 选择            | When为假则用它的SQL部分                                      |                                                              |                                                         |
| Switch    | 多项选择        | 使用子节点为真的部分SQL，子节点只能是Case，Default           |                                                              |                                                         |
| Case      | 多项中的一项    | 为真则使用它的SQL部分                                        |                                                              |                                                         |
| Set       | 更新语句的设置  | 将节点下的SQL加入SQL                                         |                                                              |                                                         |
| Default   | 直接使用该节点  | SQL语句直接拼接加入                                          |                                                              |                                                         |
| Page      | 使用分页        | 该节点必须包含pagesize,pagenum这2个变量表达式                |                                                              | (pagenum-1)*pageSize                                    |
|           |                 |                                                              |                                                              |                                                         |


