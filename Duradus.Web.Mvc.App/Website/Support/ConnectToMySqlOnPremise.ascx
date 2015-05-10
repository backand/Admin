<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConnectToMySqlOnPremise.ascx.cs" Inherits="Durados.Web.Mvc.App.Website.Support.ConnectToMySqlOnPremise" %>
<div class="info-container">
      <h1>Enable remote connections to MySql On Premise</h1>
    <%--<div class="info-text"> <ul class="supportContext">
    
     <li><a href="#firewalsettings">Step 1: Enable MySql in the local FireWall rules or the VM.</a></li>
    
   <%--   <li><a href="#dynamicport">Having more than one SQL server instance on a machine.</a></li>--%>
 <%--</ul>></div>--%>
 
    <div class="info-text">
    <h3 id="mysqlsettings">Step 1: Enable Reomte Access on your MySql Server Instance</h3><br /><br />
    In order to enable  remote access to MySql Server you need to set priviliges for username@137.117.97.68 on your schema.<br /><br />
    Open your MySql console and enter the following commands <br />(replace 'username' and 'schemaname' with the appropriate names)<br /><br />

    <p style="font-style:italic">
    CREATE USER  'username'@'137.117.97.68' IDENTIFIED BY 'password';<br /><br />

    GRANT ALL ON schemaname.* TO 'username'@'137.117.97.68' ;<br /><br />

    FLUSH PRIVILEGES;<br /><br />
    <br />
    </p>

    For example, with the following credentials:<br /><br />
    <p style="font-style:italic">
    CREATE USER  'myusername'@'137.117.97.68' IDENTIFIED BY 'mypassword';<br /><br />

    GRANT ALL ON mydatabase.* TO 'myusername'@'137.117.97.68' ;<br /><br />

    FLUSH PRIVILEGES;<br /><br />
    </p>

    This is how step 2 should look:<br />
    <img src="./Images/step2.png" />

    </div>
    <div class="info-text">
     <h3 id="firewalsettings">Step 2: Enable MySQL Server in the local FireWall rules or the VM.</h3><br />
     Click <i>Start</i>, click <i>All Programs</i>, click <i>Administrative Tools</i>, and then click <i>Windows Firewall with Advanced Security</i> and go to "Inbound Rules".
     <br />Look for MySQL server rule, if you find one make sure it opens the port of your MySQL server instance port ( If you have only one instance then it is probably 3306).
     <br /> If you don't have such a rule then you need to create one.
     <span class="clickMe">Create new Inbound Rule</span><br /><div class="supportUnfold"><br />
     <br/><img class="imgSuppport" src="./Images/MysqlOnPremiseFirewalConfigruler-7.png" /> 
     In the first tab set rule type <i>Port</i> (default) and click <i>Next</i><br />
      <img class="imgSuppport" src="./Images/MysqlOnPremiseFirewalConfigruler-1.png" /><br />
     In <i>Specific local ports</i> enter 3306.<br />
     <img class="imgSuppport" src="./Images/MysqlOnPremiseFirewalConfigruler-3.png" /><br />

Click Next in the <i>Action</i> and <i>Profile</i> tabs(use the defaults).<br />

In the <i>Name</i> enter a name for your rule.<br />
 <img class="imgSuppport" src="./Images/MysqlOnPremiseFirewalConfigruler-5.png" /><br />
Click <i>Finish</i> <br /><br />
If you need to restrict access then open the rule you have just created and go to <i>Scope</i> tab.
Add the Ip Address of <%=Durados.Database.ShortProductName %> in the <i>Remote Ip Address</i><br/>
<img class="imgSuppport" src="./Images/FirewalRulesScope.png" />


</div>
</div> 
<div class="info-text">
<span>Step 3: Make sure your MySql Server service is running</span><br />
 Click <i>Start</i>, click <i>All Programs</i>, click <i>Administrative Tools</i>, and then click <i>Services</i><br />
 Start your MySQL Server service<br />
 <img class="imgSupport"  src="Images/MysqlOnPremiseConfigr.png" />
</div>
    <div class="info-text">
<h3 id="azure">Azure End Point</h3><br />
If your dababase resides on Azure VM then you need to create an end point,  for MySQL server.<div  class="clickMe">click here for instractions</div> 
<br />
<div  class="supportUnfold">To create an Endpoint enter <a href="https://manage.windowsazure.com/#Workspaces/All/dashboard">Windows Azure Management Portal,</a> <br />

 Click Virtual Machines , and then select the virtual machine that you want to configure.<br />
<%-- <img class="imgSuppport" src="./Images/EndPointsr.png" /><br />--%>
Click Endpoints . The Endpoints page lists all endpoints for the virtual machine.<br />


1. Click Add at the buttom of the list page. The Add Endpoint dialog box appears.<br /><br />

2. Choose whether to add the endpoint to a load-balanced set and then click the arrow to continue.<br /><br />

3. In Name, type a name for the endpoint.<br /> 
<br/><img class="imgSuppport" src="./Images/MysqlOnPremiseAzurEndpoint-1.png" /><br />

4. In protocol, specify TCP for MySQL server port.<br /><br />

5. In Public Port and Private Port, type 3306 port for MySQL server. These port numbers can be different. The public port is the entry point for communication from outside of Windows Azure and is used by the Windows Azure load balancer. You can use the private port and firewall rules on the virtual machine to redirect traffic in a way that is appropriate for your application.<br /><br />

6. Click Create a load-balancing set if this endpoint will be the first one in a load-balanced set.<br /><br />

7. Click the check mark to create the endpoint.<br /><br />

You will now see the endpoint listed on the Endpoints page.
<br /><br />

 

 </div>
 


  
   </div>
   </div>