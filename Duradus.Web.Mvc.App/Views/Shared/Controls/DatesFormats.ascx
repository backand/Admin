<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Durados" %>
<%@ Import Namespace="Durados.Web.Mvc.UI.Helpers" %>
<%
    string datesDictionary = ViewHelper.GetJsonDatesFormats();
    string timesDictionary = ViewHelper.GetJsonTimesFormats();    
%>
<script type="text/javascript">

var gVD = '<%=Durados.Web.Mvc.Infrastructure.General.GetRootPath()%>';

var DateFormats= {
    dateType: {
        dateOnly: 30,
        dateAndTime: 31,
        timeOnly: 32
    },
    getDefaultDateType: function() {
        return DateFormats.dateType.dateOnly;
    },
    getValidDateType: function(dateFormat) {
        dateFormat = $.trim(dateFormat);
        var dateType = DateFormats.dateType;
        var validDateType = DateFormats.getDefaultDateType();
        var hasTime = false;
        var hasDate = false;

        if(dateFormat != '') {
            $.each(duradosTimesFormats, function(){
                if (dateFormat.match(this.Csharp + "$")) {
                    dateFormat = $.trim(dateFormat.replace(this.Csharp,''));
                    hasTime = true;
                    return false;
                }
            });
        }

        if(dateFormat != '') {
            $.each(duradosDatesFormats, function(){
                if (dateFormat == this.Csharp) {
                    hasDate = true;
                    return false;
                }
            });
        }

        if(hasDate) {
            if(hasTime) {
                validDateType = dateType.dateAndTime;
            }
            else {
                validDateType = dateType.dateOnly;
            }
        }
        else if(hasTime) {
            validDateType = dateType.timeOnly;
        }
        else {
            validDateType = dateType.dateOnly;
        }

        return validDateType;
    }
}

function duradosGetArray(arr) {
if (!arr || !(arr instanceof Array)) return []; //obj.constructor == Array;
return arr;
}

var duradosDatesFormats = duradosGetArray(<%=datesDictionary  %>);
var duradosTimesFormats = duradosGetArray(<%=timesDictionary  %>);

function duradosGetJQueryDateFormat(df, dateType) {
    if(df==null || df=='') {
        df='MM/dd/yyyy';
    }

    df=$.trim(df);
    var dateFormat = df;
    var timeFormat = df;
    var originalFormat = df;

    //Init time format
    if(df != '') {
        $.each(duradosTimesFormats, function(){
            if (df.match(this.Csharp + "$")) {
                timeFormat = this.JQuery;
                df = $.trim(df.replace(this.Csharp,''));
                return false;
            }
        });
    }

    //Init date format
    if(df != '') {
        $.each(duradosDatesFormats, function(){
            if(df == this.Csharp) {
                dateFormat = this.JQuery;
                return false;
            }
        });
    }

    return {
        dateFormat: dateFormat,
        timeFormat: timeFormat
    }
}

function duradosGetSpryDateFormat(df) {
    df=$.trim(df);
    var dateFormat = '';
    var timeFormat = '';
    var originalFormat = df;

    //Init time format
    if(df != '') {
        $.each(duradosTimesFormats, function(){
            if (df.match(this.Csharp + "$")) {
                timeFormat = this.Spry;
                df = $.trim(df.replace(this.Csharp,''));
                return false;
            }
        });
    }

    //Init date format
    if(df != '') {
        $.each(duradosDatesFormats, function(){
            if(df == this.Csharp) {
                dateFormat = this.Spry;
                return false;
            }
        });
    }

    if(dateFormat != '' || timeFormat!='') {
        return $.trim(dateFormat + ' ' + timeFormat);
    }
    else {
        return originalFormat;
    }
}

</script>
