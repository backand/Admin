<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConnectToMSSQL.ascx.cs" Inherits="Durados.Web.Mvc.App.Website.Support.ConnectToMSSQL" %>
  <div class="info-container">
      <h1>Enable remote connections to SQL Server</h1>
    <div> <ul class="supportContext">
     <li><a href="#ssremote">Step 1: Allow remote connections on to SQL Server instance.</a></li>
     <li><a  href="#ssconfig">Step 2: Enable SQL Server Instance TCP/IP Potocol.</a></li>
     <li><a href="#firewalsettings">Step 3: Enable SQL Server in the local FireWall rules or the VM.</a></li>
     <li><a href="#azure">Azure - Add Endpoints to VM.</a></li>
   <%--   <li><a href="#dynamicport">Having more than one SQL server instance on a machine.</a></li>--%>
 </ul></div>
    <div><h3 id="ssremote">Step 1: Allow remote connections on to SQL Server instance.</h3><br />

The first thing you want to check is if Remote Connections are enabled on your SQL Server database. In SQL Server  you do this by opening SQL Server 2008 Management Studio, connect to the server in question, right click the server…
<br/><img class="imgSuppport"     alt="Sql server Properites"  src="./Images/SQLServer2008_ServerProperties.png" /><br />
       
     
        and open the Server <i>Properties</i>.
        <br/><img class="imgSuppport" src="./Images/SQLServer2008_ServerProperties_1.png" alt="" />
        Navigate to <i>Connections</i> and ensure that <i>Allow remote connections to this server</i> is checked. Check if this solves the problem. If it does, here you go, continue to create your <%=Durados.Database.ShortProductName %> console.
</div>
    <div>
            <h3 id="ssconfig">Step 2: Protocols for MSSQLServer</h3><br />
    The next thing to check is the SQL Server Network Configuration. Open the SQL Server Configuration Manager, 
    <br/><img class="imgSuppport" src="./Images/SQLSERVERConfigurationManager.PNG" /><br />
    unfold the node SQL Server Network Configuration and select Protocols for MSSQLServer (or whatever the name of your SQL Server instance is).
         <br /> <img class="imgSuppport" src="./Images/TCP_IP_Properties.png" alt=""/><br />
         Make sure that TCP/IP is enabled and try again. If this doesn't resolved your problems then you need to check the...
         </div>
    <div>
     <h3 id="firewalsettings">Step 3: Enable SQL Server in the local FireWall rules or the VM.</h3><br />
     Click <i>Start</i>, click <i>All Programs</i>, click <i>Administrative Tools</i>, and then click <i>Windows Firewall with Advanced Security</i> and go to "Inbound Rules".
     <br />Look for SQL server rule, if you find one make sure it opens the port of your SQL server instance port ( If you have only one instance then it is probably 1433 but just to make sure, go to <i>SQL Server Configuration Manager->SQL Server Network Configuration->TCP/IP->IP Address->TCP Port</i> and get it from their.
     <br /> If you don't have such a rule then you need to create one.
     <h3 class="clickMe">Create new Inbound Rule</h3><br /><div class="supportUnfold"><br />
     <img class="imgSuppport" src="./Images/FirewalRulesNewRule.png" />
     Set rule type <i>Program</i> (default) and click <i>Next</i>
<br/><img class="imgSuppport" src="./Images/FirewalRulesRuleType.png" /> 
Browse for you sqlserver.exe file and set it as the rule program
<br/><img class="imgSuppport" src="./Images/FirewalRulesThisProgram.png" />
click <i>Next</i> until the last step , here you need to enter a name for your rule  
<br/><img class="imgSuppport" src="./Images/FirewalRulesName.png" />
Click <i>Finish</i> <br /><br />
If you need to restrict access then open the rule you have just created and go to <i>Scope</i> tab.
Add the Ip Address of <%=Durados.Database.ShortProductName %> in the <i>Remote Ip Address</i><br/><img class="imgSuppport" src="./Images/FirewalRulesScope.png" />

Now you just need  to do the same for sqlbrowser.exe (if it not already exists) and you are.... done.
</div>
</div> 
    <p>
<h3 id="azure">Azure End Point</h3><br />
If your dababase resides on Azure VM then you need to create two end points, one for SQL server and the second for SQL browser.<span  class="clickMe">click here for instractions</span> 
<br />
<div  class="supportUnfold">To create an Endpoint enter <a href="https://manage.windowsazure.com/#Workspaces/All/dashboard">Windows Azure Management Portal,</a> <br />

 Click Virtual Machines <i>(1)</i>, and then select the virtual machine <i>(2)</i>  that you want to configure.<br />
 <img class="imgSuppport" src="./Images/EndPointsr.png" /><br />
Click Endpoints <i>(3)</i>. The Endpoints page lists all endpoints for the virtual machine.
<br/><img class="imgSuppport" src="./Images/EndPointsr2.PNG" />

1. Click Add at the buttom of the list page. The Add Endpoint dialog box appears.<br /><br />

2. Choose whether to add the endpoint to a load-balanced set and then click the arrow to continue.<br /><br />

3. In Name, type a name for the endpoint.<br /><br />

4. In protocol, specify TCP for SQL server port  or UDP for SQL browser port.<br /><br />

5. In Public Port and Private Port, type 1433 port for SQL server and 1434 for SQL browser. These port numbers can be different. The public port is the entry point for communication from outside of Windows Azure and is used by the Windows Azure load balancer. You can use the private port and firewall rules on the virtual machine to redirect traffic in a way that is appropriate for your application.<br /><br />

6. Click Create a load-balancing set if this endpoint will be the first one in a load-balanced set.<br /><br />

7. Click the check mark to create the endpoint.<br /><br />

You will now see the endpoint listed on the Endpoints page.
<br /><br />

 <h3 >Having more than one SQL server instance on a machine.</h3><br />
 
 If  you have more than one instance of SQL server then the non default instance gets a dynamic port, in this case you need to configure a static port.<br />
 1. Go to  <i>SQL server configuration manager</i>.<br /><br />
 2. Unfold the node <i>SQL Server Network Configuration</i>.<br /><br />
 3. Select the Protocols of  your non default instance<br /><br />
 4. Click on <i>TCP/IP</i> settings and set the <i>AllIP -> TCP Port</i> to a specific port number such as 1435.<br /><br />
 5. Press the "Ok" button.(You'll need to restart the SQL server service for these changes to take effect).<br /><br />
 6. In  <a href="https://manage.windowsazure.com/#Workspaces/All/dashboard">Windows Azure Management Portal,</a>use the speciifed port numbers in the Endpoints settings.

 </div>
 


  
   </div>
   </div>