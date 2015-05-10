using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Durados
{
    public interface ITableViewer
    {

        DataView DataView { get; set; }
        
        
        string GetElementForTableView(Field field, DataRow row, string guid);


        string GetDisplayName(Field field, DataRow row, string guid);

        string GetDisplayName(View view, string guid);

        System.Data.DataView GetDataView(System.Data.DataView dataView, View view, string guid);

        void HandleFilter(object filter);

        
        bool IsEditable(Field field, DataRow row, string guid);

        bool IsVisible(Field field, Dictionary<string, Durados.Field> excludedColumn, string guid);
        
        bool IsEditable(View view);
        
        bool IsSortable(Field field, string guid);

        bool IsEditOnDblClick(View view);

        bool SelectorCheckbox(View view);

        bool ShowRowHover(View view, DataRow row);

        string GetEditCaption(View view, DataRow row, string guid);

        string GetEditTitle(View view, DataRow row);

        string GetElementForTableView(Durados.Field field, DataRow row, string guid, bool ignoreChecklistLimit);

        string GetFieldValue(Durados.Field field, DataRow row);

        string GetFieldDisplayValue(Durados.Field field, DataRow row, bool forExport);
       
    }
}
