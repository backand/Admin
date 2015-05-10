<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Contact.ascx.cs" Inherits="Durados.Web.Mvc.App.Website.Contact" %>
<%
    string email = "";
    try
    {
        bool isLoggedin = System.Web.HttpContext.Current.User != null && System.Web.HttpContext.Current.User.Identity != null && !string.IsNullOrEmpty(System.Web.HttpContext.Current.User.Identity.Name);
        email = (isLoggedin) ? "value=\"" + System.Web.HttpContext.Current.User.Identity.Name + "\"" : "";
    }
    catch { }
           
%>
    <p class="desc">To contact us please complete the form below</p>
    <div class="success"></div><span class="general"></span>
    <br/>
    <div class="panel_contact pull-left">
        <img class="bg_contact_top" src="/website/assets/images/bg/bg_contact_top.jpg"/>
        <img class="bg_contact_right" src="/website/assets/images/bg/bg_contact_right.jpg"/>
        <form id="ContactForm" class="form-horizontal" role="form" action="" method="post">
            <div class="success"></div>
            <span class="general"></span>
            <div class="form-group">
                <label for="inputEmail3" class="col-sm-2 control-label text-left pull-left">Requested Subject</label>
                <div class="col-sm-10">
                    <select class="form-control" name="requeste_dpt">
                        <option value="1" <%=(Request.QueryString["o"] == "1") ? "selected" : ""%>>Technical support</option>
                        <option value="2" <%=(Request.QueryString["d"] == "true") ? "selected" : ""%>>Request for a demo</option>
                        <option value="5" <%=(Request.QueryString["o"] == "5") ? "selected" : ""%>>Contact sales</option>
                        <option value="3" <%=(Request.QueryString["o"] == "3") ? "selected" : ""%>>Questions or Suggestions</option>
                        <option value="4" <%=(Request.QueryString["o"] == "4") ? "selected" : ""%>>Billing inquiry</option>
                        <option value="10" <%=(Request.QueryString["o"] == "10") ? "selected" : ""%>>Other</option>
                    </select>
                        <span class="error-message"></span>
                </div>
            </div>
            <div class="form-group">
                <label for="inputPassword3" class="col-sm-2 control-label">Full Name</label>
                <div class="col-sm-10">
                    <input type="text" requierd="true" title="Full Name" class="form-control" id="FullName" name="FullName"/>
                        <span class="error-message"></span>
                </div>
            </div>
            <div class="form-group">
                <label for="inputPassword3" class="col-sm-2 control-label">Email Address</label>
                <div class="col-sm-10">
                    <input type="email" requierd="true" title="Email Address" class="form-control" id="Email" name="Email"<%=email%>/>
                    <span class="error-message"></span>
                </div>
            </div>
            <div class="form-group">
                <label for="inputPassword3" class="col-sm-2 control-label">Phone</label>
                <div class="col-sm-10">
                    <input type="text" requierd="true" title="Phone" class="form-control" name="Phone" id="Phone"/>
                    <span class="error-message"></span>
                </div>
            </div>
            <div class="form-group">
                <label for="inputPassword3" class="col-sm-2 control-label">Message</label>
                <div class="col-sm-10">
                    <textarea class="form-control" requierd="true" title="Message" id="Comments" name="Comments" rows="5"><%=(Request.QueryString["d"] == "true") ? "I would like to see a working demo" : ""%></textarea>
                     <span class="error-message"></span>
                </div>
            </div>
            <div class="form-group">
                <label for="inputPassword3" class="col-sm-2 control-label"></label>
                <div class="col-sm-10 text-right">
                    <button type="submit" class="btn-submit">SUBMIT</button><!--onclick="ContactUs.submit();-->
                </div>
            </div>
        </form>

    </div>

      