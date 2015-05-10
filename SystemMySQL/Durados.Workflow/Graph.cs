using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Runtime.Serialization;

namespace Durados.Workflow
{
    [DataContract]
    public class Graph
    {
        public Graph()
        {
            Nodes = new List<Node>();
        }

        public Graph(View view, object controller, WorkflowAction action)
        {
            //Load graph  [action = WorkflowAction.CompleteStep]
            IEnumerable<Rule> rules = view.GetRules().Where(r => r.WorkflowAction == action).OrderBy(r => r.Name);

            ViewName = view.Name;

            View statusesView = ((ParentField)view.WorkFlowStepsField).ParentView;

            string pk = statusesView.PrimaryKeyFileds[0].Name;

            string val = statusesView.DisplayField.Name;

            object DefaultNode = ((ParentField)view.WorkFlowStepsField).DefaultValue;

            DataTable dataTable = ((ILoader)controller).GetDataTable(statusesView);

            DataColumn[] keys = new DataColumn[1];
            keys[0] = dataTable.Columns[pk];//"Id"
            dataTable.PrimaryKey = keys;

            Nodes = new List<Node>();            

            foreach (Rule rule in rules) // TODO - make virtual method LoadArrows()
            {
                string[] where = rule.WhereCondition.Split('=');

                if (where.Length < 2) continue;

                string NodeId = where[1].Trim();

                if (DefaultNode != null && DefaultNode.ToString() == NodeId)
                {
                    List<Arrow> startArrows = new List<Arrow>();
                    Arrow start = new Arrow();

                    start.Label = "";
                    start.NodeId = NodeId;

                    startArrows.Add(start);

                    AddNode("0", "Start", startArrows);
                }

                string RuleName = string.Empty;

                try
                {
                    DataRow row = dataTable.Rows.Find(Convert.ToInt32(NodeId));
                    if (row != null)
                    {
                        RuleName = row[val].ToString();
                    }
                }
                catch { RuleName = rule.Name; }

                List<Arrow> arrows = new List<Arrow>();

                //List<string> ids;

                foreach (Parameter param in rule.Parameters.Values)
                {
                    string[] ids = param.Value.Split('~');//.ToList<string>();

                    string label = param.Name;

                    if (label.Replace(" ", string.Empty) == "1=1")
                    {
                        label = "";
                    } 

                    foreach (string nodeid in ids)
                    {
                        Arrow arrow = new Arrow();

                        arrow.Label = label;

                        arrow.NodeId = nodeid;

                        arrows.Add(arrow);
                    }
                    //arrows = arrows.Union(ids).ToList<string>();

                }

                AddNode(NodeId, RuleName, arrows);

            }

        }

        [DataMember]
        public List<Node> Nodes { get; private set; }

        [DataMember]
        public string ViewName { get; private set; }

        public virtual int AddNode(string id, string name, List<Arrow> arrows)
        {
            Nodes.Add(new Node(id, name, arrows));

            return Nodes.Count();
        }
    }

    [DataContract]
    public class Node //EndPoint
    {
        public Node(string id, string name, List<Arrow> arrows)
        {
            this.Id = id;
            this.Name = name;
            this.Arrows = arrows;
        }
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public List<Arrow> Arrows { get; set; } //EndPoints Ids

        //[DataMember]
        //public string Description { get; set; }

    }

    [DataContract]
    public class Arrow
    {
        [DataMember]
        public string NodeId { get; set; }

        [DataMember]
        public string Label { get; set; }
    }

}
