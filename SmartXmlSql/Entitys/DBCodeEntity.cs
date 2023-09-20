using System.CodeDom;
using System.CodeDom.Compiler;

namespace SmartXmlSql.Entitys
{
    /// <summary>
    /// 创建源代码
    /// </summary>
    public class DBCodeEntity
    {
        /// <summary>
        /// 生成实体代码，动态编译的方式
        /// </summary>
        /// <param name="tableInfo">表信息</param>
        /// <param name="filePath">路径</param>
        /// <param name="codeClassName">类名称</param>
        /// <param name="codeNamespace">命名空间</param>
        public static void CreateCodeFile(TableInfo tableInfo, string filePath, string codeClassName, string codeNamespace)
        {

            CodeCompileUnit compileUnit = new CodeCompileUnit();

       
            CodeNamespace samples = new CodeNamespace(codeNamespace);
          
            compileUnit.Namespaces.Add(samples);

         
            samples.Imports.Add(new CodeNamespaceImport("System"));

           
            if (string.IsNullOrEmpty(codeClassName))
            {
                codeClassName = tableInfo.TableName;
            }
            CodeTypeDeclaration myClass = new CodeTypeDeclaration(codeClassName);
          
            myClass.Attributes = MemberAttributes.Public;
            myClass.TypeAttributes = System.Reflection.TypeAttributes.Public;
            foreach (var f in tableInfo.ColumnInfos)
            {
                //
                CodeMemberField field = new CodeMemberField();
                field.Attributes = MemberAttributes.Private;
                field.Name = f.ColumnName.ToLower();
                field.Type = new CodeTypeReference(f.ColumnType);
                myClass.Members.Add(field);
                //
                CodeMemberProperty property1 = new CodeMemberProperty();
                f.ColumnName =f.ColumnName.Substring(0,1).ToUpper()+f.ColumnName.Substring(1);    
                property1.Name = f.ColumnName;
                property1.Type = new CodeTypeReference(f.ColumnType);
                property1.Attributes = MemberAttributes.Public|MemberAttributes.Final;
                  property1.HasGet = true;
                 property1.HasSet = true;
                property1.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), f.ColumnName.ToLower())));

                property1.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), f.ColumnName.ToLower()),

                     new CodePropertySetValueReferenceExpression()));
                  myClass.Members.Add(property1);

                CodeComment comment = new CodeComment(f.Description,false);
         
                CodeCommentStatement commentStatement = new CodeCommentStatement(comment);
               
                property1.Comments.Add(commentStatement);


            }
            samples.Types.Add(myClass);
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            options.BlankLinesBetweenMembers = true;
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filePath))
            {
                provider.GenerateCodeFromCompileUnit(compileUnit, sw, options);
            }
        }
    }
}

