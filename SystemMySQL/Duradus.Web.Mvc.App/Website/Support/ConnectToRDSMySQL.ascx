<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConnectToRDSMySQL.ascx.cs" Inherits="Durados.Web.Mvc.App.Website.Support.ConnectToRDSMySQL" %>
 
    <div class="info-container">
      <h1 >Enable remote connections to MySql on Amazon RDS</h1>
       <p><br />

Step 1: Enter into your AWS console, EC2 tab, Click security groups on the left of the console.<br /><br />
Setp 2: Click your group&nbsp; in the center menu (e.g. group id SG-…).<br /><br />
Setp 3: Click inbound tab (lower part of the screen).<br /><br />
Step 4: Select MYSQL from the list, in the Source field type 137.117.97.68/32 and save the rule.<br /><br />
</p>
<img class="imgSuppport" src="Images/RDSSGR1.png" />

<div>
<h3>Few Notes:</h3><br />
1. Make sure RDS DB Instance is available (check it's status).<br />
2. Make sure you are modifing the correct security group(check the RDS DB instance Security Group Name).<br />

</div>
</div>

   
  