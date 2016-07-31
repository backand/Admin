using System;
using System.Data;
using System.Collections;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using System.Globalization;

using Durados;

namespace Durados.CodeDom
{
    public class EnumGenerator
    {
        public void Generate(string fileName, DataSet ds, String ns)
        {
            CodeDomProvider provider = new CSharpCodeProvider();
                               
            TextWriter w = new StreamWriter(new FileStream(fileName, FileMode.Create));

            try
            {

                Database db = new Database(ds);
                Generate(w, db, provider, ns);
            }
            finally
            {
                w.Close();
            }
        }

        public void Generate(TextWriter w, DataSet ds,
            CodeDomProvider provider, String ns)
        {
            Generate(w, new Database(ds), provider, ns);
        }

        public void Generate(TextWriter w, Database database,
            CodeDomProvider provider, String ns)
        {
            CodeCommentStatement c = new CodeCommentStatement(
                String.Format("Enum for {0}", database.DisplayName.Replace("DataSet", "")));
            provider.GenerateCodeFromStatement(c, w, null);

            //Gen: namespace <NAMESPACE> {
            //         using System;
            CodeNamespace cnamespace = new CodeNamespace(ns);
            cnamespace.Imports.Add(new CodeNamespaceImport("System"));

            string databaseName = database.DisplayName.Replace("DataSet", "").Replace(' ', '_');
            //Gen: public enum <typeName> 
            CodeTypeDeclaration enumViewsType =
                new CodeTypeDeclaration(databaseName + "Views");
            enumViewsType.IsEnum = true;
            //Add this so that VB does not Inherit either interface
            enumViewsType.TypeAttributes = TypeAttributes.Public;
            cnamespace.Types.Add(enumViewsType);

            foreach (View view in database.Views.Values)
            {
                enumViewsType.Members.Add(new CodeMemberField(typeof(int), view.Name));

                //Gen: public enum <typeName> 
                CodeTypeDeclaration enumViewType =
                    new CodeTypeDeclaration(view.Name);
                enumViewType.IsEnum = true;
                //Add this so that VB does not Inherit either interface
                enumViewType.TypeAttributes = TypeAttributes.Public;
                cnamespace.Types.Add(enumViewType);

                foreach (Field field in view.Fields.Values)
                {
                    enumViewType.Members.Add(new CodeMemberField(typeof(int), field.Name));
                }
            }

            provider.GenerateCodeFromNamespace(cnamespace, w, null);

        }
    }
}
