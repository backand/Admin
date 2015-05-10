<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Durados.Web.Mvc.UI.ImageViewerInfo>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>ImageViewer</title>
    <script type="text/javascript" language=javascript>
        window.location.href = '<%=Model.Url %>';
    </script>
</head>
<body>
    <div>
        <img alt='<%=Model.Title %>' title='<%=Model.Title %>' src='<%=Model.Url %>' />
    </div>
</body>
</html>
