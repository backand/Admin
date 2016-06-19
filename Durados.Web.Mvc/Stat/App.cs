﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Durados.DataAccess;

namespace Durados.Web.Mvc.Stat
{
    public class App
    {
        public string AppName { get; private set; }
        public App(string appName)
        {
            AppName = appName;
        }

        private View GetAppView()
        {
            return (View)Maps.Instance.DuradosMap.Database.Views["durados_App"];
        }

        private MapDataSet.durados_AppRow appRow = null;
        public MapDataSet.durados_AppRow GetAppRow()
        {
            if (appRow != null)
                return appRow;

            View appView = GetAppView();
            Field idField = appView.Fields["Id"];

            int? id = GetAppId();
            
            
            if (!id.HasValue)
            {
                Durados.Diagnostics.EventViewer.WriteEvent("Could not find app for: " + AppName);
                return null;
            }

            try
            {
                appRow = (MapDataSet.durados_AppRow)appView.GetDataRow(idField, id.Value.ToString(), false);
            }
            catch (Exception exception)
            {
                Durados.Diagnostics.EventViewer.WriteEvent("failed to GetDataRow for id: " + id.Value, exception);
                try
                {
                    ((DuradosMap)Maps.Instance.DuradosMap).AddSslAndAahKeyColumn();
                    appRow = (MapDataSet.durados_AppRow)appView.GetDataRow(idField, id.Value.ToString(), false);
                }
                catch (Exception exception2)
                {
                    //Durados.Diagnostics.EventViewer.WriteEvent(exception2);
                }
            }

            if (appRow == null)
            {
                return null;
            }

            return appRow;

        }

        private int? GetAppId()
        {
            int? id = null;
            int appId ;
            
            bool result = int.TryParse(AppName,out appId);
            
            if (result)
                 id = Maps.Instance.AppExists(appId);
            
            if(!id.HasValue)
                id=  Maps.Instance.AppExists(AppName, null);
            
            return id;
        }
    }
}
