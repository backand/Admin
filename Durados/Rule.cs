using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Durados.Config.Attributes;

namespace Durados
{
    public class Rule
    {
        
        public Rule()
        {
            Parameters = new Dictionary<string, Parameter>();
            UseSqlParser = true;
            Code = "function backandCallback(newRow, oldRow, parameters, userProfile) { \n/* Your code here.\nThe function result will be returned to the client as a json. */ \n}";
            Category = "general";
        }

        public bool ShouldTrigger(Durados.TriggerDataAction dataAction)
        {
            return DataAction == dataAction || (DataAction == TriggerDataAction.AfterCreateOrEdit && (dataAction == TriggerDataAction.AfterCreate || dataAction == TriggerDataAction.AfterEdit)) ;
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public int ID { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string Name { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string Category { get; set; }


        [Durados.Config.Attributes.ColumnProperty()]
        public TriggerDataAction DataAction { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string DatabaseViewName { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public WorkflowAction WorkflowAction { get; set; }
        
        [Durados.Config.Attributes.ColumnProperty()]
        public ActionType ActionType { get; set; }

        
        [Durados.Config.Attributes.ColumnProperty()]
        public string WhereCondition { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string InputParameters { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public bool UseSqlParser { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string Views { get; set; }

        public string[] GetAdditionalViewNames()
        {
            if (string.IsNullOrEmpty(Views))
                return null;
            return Views.Split(',');
        }

        [Durados.Config.Attributes.ChildrenProperty(TableName = "Parameter", Type = typeof(Parameter), DictionaryKeyColumnName = "Name")]
        public Dictionary<string, Parameter> Parameters { get; private set; }
        
        
        public override string ToString()
        {
            return Name;
        }

        //public Dictionary<string, Parameter> GetParameters()
        //{

        //    Dictionary<string, Parameter> prevParameters = new Dictionary<string,Parameter>(Parameters);
        //    Dictionary<string, Parameter> newParameters;
        //    var props = this.GetType().GetProperties().Where(
        //        prop => Attribute.IsDefined(prop, typeof(Durados.Config.Attributes.ParameterAttribute))
        //            && (((ParameterAttribute)prop.GetCustomAttributes(typeof(ParameterAttribute), false)[0]).WFAction != null)//.Select(c=>c.GetCustomAttributes(typeof(Durados.Config.Attributes.ParameterAttribute), false));
        //     && (((ParameterAttribute)prop.GetCustomAttributes(typeof(ParameterAttribute), false)[0]).WFAction.Equals(this.WorkflowAction.ToString())));//.Select(c=>c.GetCustomAttributes(typeof(Durados.Config.Attributes.ParameterAttribute), false));

        //    var pairs = from p1 in props
        //                let at1 = ((ParameterAttribute)p1.GetCustomAttributes(typeof(ParameterAttribute), false)[0])
        //                join p2 in props
        //                    on at1.Pair equals ((ParameterAttribute)p2.GetCustomAttributes(typeof(ParameterAttribute), false)[0]).Pair
        //                let at2 = ((ParameterAttribute)p2.GetCustomAttributes(typeof(ParameterAttribute), false)[0])
        //                let v1 = Convert.ToString(p1.GetValue(this, null))
        //                let v2 = Convert.ToString(p2.GetValue(this, null))
        //                where at1.Role == ParameterRole.Name  && at2.Role == ParameterRole.Value && !string.IsNullOrEmpty(v1)
        //                select new Durados.Parameter() { Name = v1, Value = v2, UseSqlParser = true };

        //    newParameters = pairs.ToDictionary(k => k.Name, v => v);
        //    var singles = from p in props
        //                  let n = ((ParameterAttribute)p.GetCustomAttributes(typeof(ParameterAttribute), false)[0])
        //                  let v = Convert.ToString(p.GetValue(this, null))
        //                  where n.Role == ParameterRole.None && !string.IsNullOrEmpty(v)
        //                  select new Durados.Parameter() { Name = n.ParameterName, Value = v, UseSqlParser = true };

        //    foreach (KeyValuePair<string, Durados.Parameter> p in singles.ToDictionary(k => k.Name, v => v))
        //        newParameters.Add(p.Key, p.Value); ;
        //    foreach (KeyValuePair<string, Durados.Parameter> p in newParameters)
        //        if (!prevParameters.ContainsKey(p.Key))
        //            prevParameters.Add(p.Key, p.Value);
        //    return prevParameters;
        //}
        public Dictionary<string, Parameter> GetParameters()
        {

            Dictionary<string, Parameter> prevParameters = new Dictionary<string, Parameter>(Parameters);
            /*
             * This code is intende to combine 2 mechanizims/sources for the rules prameters. 
             * the old one was a table with key value (e.g key:to , value:a@b.c)
             * the new one was the rules properties (e.g. NotifyTo)
             * this code combine them to 1 dictionary.
             * for parametets that thier dictionary key is a token(e.g. to, bcc,)in the old mechanizim, the new mechanizim will have that key in the ParameterName in the Prameter Attribute
            
             */
            var props = this.GetType().GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(Durados.Config.Attributes.ParameterAttribute))
                    && (((ParameterAttribute)prop.GetCustomAttributes(typeof(ParameterAttribute), false)[0]).WFAction != null)//.Select(c=>c.GetCustomAttributes(typeof(Durados.Config.Attributes.ParameterAttribute), false));
             && (((ParameterAttribute)prop.GetCustomAttributes(typeof(ParameterAttribute), false)[0]).WFAction.Equals(this.WorkflowAction.ToString())));//.Select(c=>c.GetCustomAttributes(typeof(Durados.Config.Attributes.ParameterAttribute), false));

            var pairs = from p1 in props
                        let at1 = ((ParameterAttribute)p1.GetCustomAttributes(typeof(ParameterAttribute), false)[0])
                        join p2 in props
                            on at1.Pair equals ((ParameterAttribute)p2.GetCustomAttributes(typeof(ParameterAttribute), false)[0]).Pair
                            into p1_p2
                        from pt in p1_p2.DefaultIfEmpty()

                        let at2 = (pt!=null)?((ParameterAttribute)pt.GetCustomAttributes(typeof(ParameterAttribute), false)[0]):null
                        let v1 = (at2 != null) ? Convert.ToString(p1.GetValue(this, null)) : at1.ParameterName
                        let v2 = (at2 != null) ? Convert.ToString(pt.GetValue(this, null)) : Convert.ToString(p1.GetValue(this, null))
                        where (at1.Role == ParameterRole.Name || (at1.Role == ParameterRole.None && !string.IsNullOrEmpty(v2) )) && (at2 ==null || at2.Role == ParameterRole.Value) && !string.IsNullOrEmpty(v1) 
                        select new Durados.Parameter() { Name = v1, Value = v2, UseSqlParser = true };

            Dictionary<string, Parameter> newParameters = pairs.ToDictionary(k => k.Name, v => v);
        
            foreach (KeyValuePair<string, Durados.Parameter> p in newParameters)
                if (!prevParameters.ContainsKey(p.Key))
                    prevParameters.Add(p.Key, p.Value);
            return prevParameters;
        }

        [Durados.Config.Attributes.Parameter(WFAction = "Notify", ParameterName = "to")]
        [Durados.Config.Attributes.ColumnProperty()]
        public string NotifyTo { get; set; }

        [Durados.Config.Attributes.Parameter( WFAction="Notify", ParameterName = "cc")]
        [Durados.Config.Attributes.ColumnProperty()]
        public string NotifyCC { get; set; }

        [Durados.Config.Attributes.Parameter(WFAction = "Notify", ParameterName = "bcc")]
        [Durados.Config.Attributes.ColumnProperty()]
        public string NotifyBCC { get; set; }

        [Durados.Config.Attributes.Parameter(WFAction = "Notify", ParameterName = "from")]
        [Durados.Config.Attributes.ColumnProperty()]
        public string NotifyFrom { get; set; }

        [Durados.Config.Attributes.Parameter(WFAction = "Notify", ParameterName = "subject")]
        [Durados.Config.Attributes.ColumnProperty()]
        public string NotifySubject { get; set; }

        [Durados.Config.Attributes.Parameter(WFAction = "Notify", ParameterName = "message")]
        [Durados.Config.Attributes.ColumnProperty()]
        public string NotifyMessage { get; set; }

        [Durados.Config.Attributes.Parameter(WFAction = "JavaScript", ParameterName = "code")]
        [Durados.Config.Attributes.ColumnProperty()]
        public string Code { get; set; }

        [Durados.Config.Attributes.Parameter(WFAction = "NodeJS", ParameterName = "fileName")]
        [Durados.Config.Attributes.ColumnProperty()]
        public string FileName { get; set; }

        [Durados.Config.Attributes.Parameter(WFAction = "NodeJS", ParameterName = "functionName")]
        [Durados.Config.Attributes.ColumnProperty()]
        public string FunctionName { get; set; }

        [Durados.Config.Attributes.Parameter(WFAction = "Execute", Pair = "exec1", Role = Durados.Config.Attributes.ParameterRole.Name)]
        [Durados.Config.Attributes.ColumnProperty()]
        public string ExecuteCommand { get; set; }

        [Durados.Config.Attributes.Parameter(WFAction = "Execute", Pair = "exec1", Role = Durados.Config.Attributes.ParameterRole.Value)]
        [Durados.Config.Attributes.ColumnProperty()]
        public string ExecuteMessage { get; set; }

        [Durados.Config.Attributes.Parameter(WFAction = "Execute", ParameterName ="in")]
        [Durados.Config.Attributes.ColumnProperty()]
        public string ExecuteInParameters { get; set; }

        [Durados.Config.Attributes.Parameter(WFAction = "Execute", ParameterName = "out")]
        [Durados.Config.Attributes.ColumnProperty()]
        public string ExecuteOutParameters { get; set; }

        [Durados.Config.Attributes.Parameter(WFAction = "Validate", Pair = "validate1", Role = Durados.Config.Attributes.ParameterRole.Name)]
        [Durados.Config.Attributes.ColumnProperty()]
        public string ValidateCommand1 { get; set; }

        [Durados.Config.Attributes.Parameter(WFAction = "Validate", Pair = "validate1", Role = Durados.Config.Attributes.ParameterRole.Value)]
        [Durados.Config.Attributes.ColumnProperty()]
        public string ValidateMessage1 { get; set; }

        [Durados.Config.Attributes.Parameter(WFAction = "Validate", Pair = "validate2", Role = Durados.Config.Attributes.ParameterRole.Name)]
        [Durados.Config.Attributes.ColumnProperty()]
        public string ValidateCommand2 { get; set; }

        [Durados.Config.Attributes.Parameter(WFAction = "Validate", Pair = "validate2", Role = Durados.Config.Attributes.ParameterRole.Value)]
        [Durados.Config.Attributes.ColumnProperty()]
        public string ValidateMessage2 { get; set; }

        [Durados.Config.Attributes.Parameter(WFAction = "Validate", Pair = "validate3", Role = Durados.Config.Attributes.ParameterRole.Name)]
        [Durados.Config.Attributes.ColumnProperty()]
        public string ValidateCommand3 { get; set; }

        [Durados.Config.Attributes.Parameter(WFAction = "Validate", Pair = "validate3", Role = Durados.Config.Attributes.ParameterRole.Value)]
        [Durados.Config.Attributes.ColumnProperty()]
        public string ValidateMessage3 { get; set; }

        [Durados.Config.Attributes.Parameter(WFAction = "Validate", Pair = "validate4", Role = Durados.Config.Attributes.ParameterRole.Name)]
        [Durados.Config.Attributes.ColumnProperty()]
        public string ValidateCommand4 { get; set; }

        [Durados.Config.Attributes.Parameter(WFAction = "Validate", Pair = "validate4", Role = Durados.Config.Attributes.ParameterRole.Value)]
        [Durados.Config.Attributes.ColumnProperty()]
        public string ValidateMessage4 { get; set; }

        [Durados.Config.Attributes.Parameter(WFAction = "WebService",  ParameterName = "URL")]
        [Durados.Config.Attributes.ColumnProperty()]
        public string WebService1 { get; set; }


        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "If true page takes the page roles, otherwise the page takes the workspace roles")]
        public bool Precedent { get; set; }

        private string allowSelectRoles;

        [Durados.Config.Attributes.ColumnProperty(DoNotCopy = true, Description = "Hide/Show page to users with these roles.")]
        public string AllowSelectRoles
        {
            get
            {
                return allowSelectRoles;
            }
            set
            {
                allowSelectRoles = value;
            }
        }

        [Durados.Config.Attributes.ColumnProperty()]
        public string LambdaName { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public string LambdaArn { get; set; }

        [Durados.Config.Attributes.ColumnProperty()]
        public int CloudSecurity { get; set; }

    }


}
