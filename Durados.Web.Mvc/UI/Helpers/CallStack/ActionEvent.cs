﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.UI.Helpers.CallStack
{
    public class ActionEvent : IActionEvent
    {

        public ActionEvent(int time, Event @event, string objectName, string actionName, string id, object data)
        {
            Time = time;
            Event = @event;
            ObjectName = objectName;
            ActionName = actionName;
            Id = id;
            Data = data;
        }
        [JsonIgnore]
        public int Time { get; private set; }
        [JsonIgnore]
        public Event Event { get; private set; }
        [JsonIgnore]
        public string ObjectName { get; private set; }
        [JsonIgnore]
        public string ActionName { get; private set; }
        public string Id { get; private set; }
        public object Data { get; private set; }

        
    }
}
