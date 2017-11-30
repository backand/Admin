-- drop database ___;
create database ___;
create database backand_security;
USE ___;

/****** Object:  Table  `backand_ActionTemplate`    Script Date: 10/22/2017 1:13:11 PM ******/


CREATE TABLE  `backand_ActionTemplate`(
	`Id` INT(11)  PRIMARY KEY AUTO_INCREMENT  NOT NULL  ,
	`name` VARCHAR(250) NOT NULL,
	`shortDescription` VARCHAR(1000) NULL,
	`documentation` LONGTEXT NULL,
	`createdDate`   DATETIME NULL  DEFAULT current_timestamp,
	`updatedDate`   DATETIME NULL,
	`createdBy` VARCHAR(50) NULL,
	`updatedBy` VARCHAR(50) NULL,
	`ruleName` VARCHAR(50) NULL,
	`objectName` VARCHAR(50) NULL,
	`action` VARCHAR(50) NULL,
	`ruleType` VARCHAR(50) NULL,
	`condition` VARCHAR(1500) NULL,
	`parameters` VARCHAR(1500) NULL,
	`code` LONGTEXT NULL,
	`executeCommand` LONGTEXT NULL,
	`executeMessage` VARCHAR(1000) NULL,
	`json` LONGTEXT NULL,
	`backands`  TINYINT(1) NULL,
	`approved`  TINYINT(1) NULL,
	`approvedDate`   DATETIME NULL,
	`contributors` VARCHAR(1000) NULL,
	`category` INT NULL,
	`ordinal` INT NULL
    )
;
/****** Object:  Table  `backand_model`    Script Date: 10/22/2017 1:13:11 PM ******/




CREATE TABLE  `backand_model`(
	`Id` INT(11)  PRIMARY KEY AUTO_INCREMENT  NOT NULL,
	`appName` VARCHAR(50) NULL,
	`username` VARCHAR(50) NULL,
	`timestamp`   DATETIME NULL,
	`input`   LONGTEXT  NULL,
	`output`   LONGTEXT  NULL,
	`valid` VARCHAR(50) NULL,
	`errorMessage` VARCHAR(1500) NULL,
	`errorTrace`   LONGTEXT  NULL,
	`action` VARCHAR(50) NULL   DEFAULT 'model'
    );

/****** Object:  Table  `durados_App`    Script Date: 10/22/2017 1:13:11 PM ******/




CREATE TABLE  `durados_App`(
	`Id` INT(11)  PRIMARY KEY AUTO_INCREMENT NOT NULL,
	`Name` VARCHAR(250) NOT NULL,
	`Creator` INT NULL,
	`CreatedDate`   DATETIME NOT NULL   DEFAULT current_timestamp,
	`ToDelete`  TINYINT(1) NOT NULL   DEFAULT 0,
	`ToDeleteDate`   DATETIME NULL,
	`Deleted`  TINYINT(1) NOT NULL  DEFAULT 0,
	`DeletedDate`   DATETIME NULL,
	`DataSourceTypeId` INT(5) NULL,
	`SqlConnectionId` INT(11) NULL,
	`UsesSpecificBinary`  TINYINT(1) NULL,
	`Url` VARCHAR(250) NULL,
	`Image` VARCHAR(250) NULL   DEFAULT  'backand.png',
	`ExistingDataSource`  TINYINT(1) NULL,
	`TemplateId` INT NULL,
	`ExcelFileName` VARCHAR(250) NULL,
	`SpecificDOTNET` VARCHAR(250) NULL,
	`SpecificJS` VARCHAR(250) NULL,
	`SpecificCss` VARCHAR(250) NULL,
	`UseAsTemplate`  TINYINT(1) NULL,
	`Description` VARCHAR(800) NULL,
	`TemplateFile` VARCHAR(250) NULL,
	`SystemSqlConnectionId` INT(11) NULL,
	`PrivateAuthentication`  TINYINT(1) NULL,
	`SecuritySqlConnectionId` INT(11) NULL,
	`Title` VARCHAR(250) NULL,
	`Basic`  TINYINT(1) NULL,
	`Guid`  VARCHAR(36)  NOT NULL , -- CONSTRAINT `DF_durados_App_Guid`  DEFAULT (newid())
	`ConfigChangesIndication` INT NULL,
	`ThemeId` INT NULL,
	`CustomThemePath` VARCHAR(500) NULL,
	`DatabaseStatus` INT NULL DEFAULT 1,
	`TemplateStatus` INT NULL,
	`CodeStatus` INT NULL,
	`HostingStatus` INT NULL,
	`SignUpToken`  VARCHAR(36)  NOT NULL, --  DEFAULT (newid()),
	`AnonymousToken`  VARCHAR(36)  NOT NULL, --  CONSTRAINT `DF_durados_App_AnonymousToken`  DEFAULT (newid()),
	`PaymentStatus` INT NOT NULL  DEFAULT 0,
	`PaymentLocked`  TINYINT(1) NOT NULL  DEFAULT 0,
	`IsAuthApp`  TINYINT(1) NOT NULL DEFAULT 0,
	`Environment` VARCHAR(250) NULL,
	`EnvVar`   LONGTEXT  NULL,
	`ProductType` INT NOT NULL  DEFAULT 1
    );
    ALTER TABLE durados_App AUTO_INCREMENT=20000;


/****** Object:  Table  `durados_AppLimits`    Script Date: 10/22/2017 1:13:11 PM ******/




CREATE TABLE  `durados_AppLimits`(
	`Id` INT(11)  AUTO_INCREMENT PRIMARY KEY   NOT NULL,
	`Name` VARCHAR(50) NOT NULL,
	`Limit` INT NOT NULL,
	`AppId` INT(11) NOT NULL
);


/****** Object:  Table  `durados_AppPlan`    Script Date: 10/22/2017 1:13:11 PM ******/




CREATE TABLE  `durados_AppPlan`(
	`Id` INT(11)  AUTO_INCREMENT PRIMARY KEY NOT NULL,
	`AppId` INT(11) NULL,
	`PlanId` INT(11) NULL,
	`PurchaseDate`   DATETIME NULL   DEFAULT current_timestamp
);



/****** Object:  Table  `durados_ExternaInstance`    Script Date: 10/22/2017 1:13:11 PM ******/




CREATE TABLE  `durados_ExternaInstance`(
	`Id` INT(11)  AUTO_INCREMENT PRIMARY KEY  NOT NULL,
	`InstanceName` VARCHAR(500) NULL,
	`DbName` VARCHAR(250) NULL,
	`IsActive`  TINYINT(1) NULL,
	`Provider` VARCHAR(250) NULL,
	`Endpoint` VARCHAR(500) NULL,
	`SequrityGroup` VARCHAR(250) NULL,
	`Region` VARCHAR(250) NULL,
	`Version` VARCHAR(250) NULL,
	`Capacity` INT NULL,
	`SqlConnectionId` INT(11) NULL

);



/****** Object:  Table  `durados_Folder`    Script Date: 10/22/2017 1:13:11 PM ******/




CREATE TABLE  `durados_Html`(
	`Name` VARCHAR(50) PRIMARY KEY NOT NULL,
	`text`   LONGTEXT  NOT NULL

);
 


/****** Object:  Table  `Durados_Log`    Script Date: 10/22/2017 1:13:11 PM ******/


CREATE TABLE  `Durados_Log`(
	`Id` INT(11)  AUTO_INCREMENT PRIMARY KEY NOT NULL,
	`ApplicationName` VARCHAR(250) NULL,
	`Username` VARCHAR(250) NULL,
	`MachineName` VARCHAR(250) NULL,
	`Time`   DATETIME NULL,
	`Controller` VARCHAR(250) NULL,
	`Action` VARCHAR(250) NULL,
	`MethodName` VARCHAR(250) NULL,
	`LogType` INT NULL,
	`ExceptionMessage` VARCHAR(800) NULL,
	`Trace` VARCHAR(800) NULL,
	`FreeText` VARCHAR(800) NULL,
	`Guid`  VARCHAR(36)  NULL
    );

 


/****** Object:  Table  `durados_SqlConnection`    Script Date: 10/22/2017 1:13:11 PM ******/




CREATE TABLE  `durados_SqlConnection`(
	`Id` INT(11)  AUTO_INCREMENT  PRIMARY KEY  NOT NULL,
	`ServerName` VARCHAR(250) NULL,
	`Catalog` VARCHAR(250) NULL,
	`Username` VARCHAR(250) NULL,
	`IntegratedSecurity`  TINYINT(1) NULL,
	`DuradosUser` INT(11) NULL,
	`SqlProductId` INT(5) NULL,
	`ProductPort` VARCHAR(50) NULL,
	`SshRemoteHost` VARCHAR(50) NULL,
	`SshPort` INT NULL,
	`SshUser` VARCHAR(50) NULL,
	`SshUses`  TINYINT(1) NULL,
	`Password` VARCHAR(256) NULL,
	`SshPassword` VARCHAR(256) NULL,
	`SslUses`  TINYINT(1) NULL,
	`SshPrivateKey` VARCHAR(4000) NULL,
	`CreateDate`   DATETIME NULL  DEFAULT current_timestamp,
	`PasswordOPEN` VARCHAR(400) NULL,
	`SshPasswordOPEN` VARCHAR(400) NULL
);
/****** Object:  Table  `durados_SqlProduct`    Script Date: 10/22/2017 1:13:11 PM ******/

CREATE TABLE  `durados_SqlProduct`(
	`Id` INT(5)  AUTO_INCREMENT  PRIMARY KEY NOT NULL,
	`Name` VARCHAR(50) NULL,
	`Ordinal` INT NULL
);



/****** Object:  Table  `durados_User`    Script Date: 10/22/2017 1:13:11 PM ******/




CREATE TABLE  `durados_User`(
	`Id` INT(11)  AUTO_INCREMENT  PRIMARY KEY  NOT NULL,
	`Username` VARCHAR(256) NOT NULL,
	`FirstName` VARCHAR(50) NOT NULL,
	`LastName` VARCHAR(50) NOT NULL,
	`Email` VARCHAR(250) NOT NULL,
	`Password` VARCHAR(120) NULL,
	`Role` VARCHAR(256) NOT NULL,
	`Guid`  VARCHAR(36)  NOT NULL ,-- CONSTRAINT `DF_durados_User_Guid`  DEFAULT (newid()),
	`Signature` VARCHAR(4000) NULL,
	`SignatureHTML` VARCHAR(4000) NULL,
	`IsApproved`  TINYINT(1) NULL,
	`NewUser`  TINYINT(1) NULL,
	`Comments` VARCHAR(800) NULL
);
 


/****** Object:  Table  `durados_UserApp`    Script Date: 10/22/2017 1:13:11 PM ******/




CREATE TABLE  `durados_UserApp`(
	`Id` INT(11)  AUTO_INCREMENT  PRIMARY KEY  NOT NULL,
	`UserId` INT(11) NOT NULL,
	`AppId` INT(11) NOT NULL,
	`Category` VARCHAR(250) NULL,
	`Role` VARCHAR(50) NULL);



/****** Object:  Table  `durados_UserRole`    Script Date: 10/22/2017 1:13:11 PM ******/




CREATE TABLE  `durados_UserRole`(
	`Name` VARCHAR(256)  PRIMARY KEY  NOT NULL,
	`Description` VARCHAR(50) NOT NULL,
	`FirstView` VARCHAR(200) NULL);



/****** Object:  Table  `durados_UserSocial`    Script Date: 10/22/2017 1:13:11 PM ******/




CREATE TABLE  `durados_UserSocial`(
	`Id` INT(11)  AUTO_INCREMENT  PRIMARY KEY  NOT NULL,
	`UserId` INT NOT NULL,
	`Provider` VARCHAR(50) NOT NULL,
	`SocialId` VARCHAR(50) NOT NULL,
	`AppId` INT(11) NULL);



/****** Object:  Table  `durados_ValidGuid`    Script Date: 10/22/2017 1:13:11 PM ******/
CREATE TABLE`durados_DataSourceType`(
	`Id` INT(5)  PRIMARY KEY AUTO_INCREMENT NOT NULL,
	`Name` VARCHAR(50) NULL,
	`Ordinal` INT NULL);



CREATE TABLE  `durados_ValidGuid`(
	`Id`  VARCHAR(36)  PRIMARY KEY   NOT NULL,
	`UserGuid`  VARCHAR(36)  NULL,
	`Time`   DATETIME NULL DEFAULT current_timestamp,
	`Used`  TINYINT(1) NULL DEFAULT 0,
	`approvedByAdmin`  TINYINT(1) NULL DEFAULT 0);



/****** Object:  View  `v_durados_User`    Script Date: 10/22/2017 1:13:11 PM ******/

CREATE TABLE `backand_Environment`(
	`Id` VARCHAR(250) PRIMARY KEY  NOT NULL 
    );

CREATE TABLE`durados_Plan`(
	`Id` INT(11)  PRIMARY KEY AUTO_INCREMENT NOT NULL,
	`Name` VARCHAR(50) NULL,
	`Ordinal` INT NULL);

CREATE TABLE `durados_Invite`(
	`Id` INT(11)  AUTO_INCREMENT  PRIMARY KEY NOT NULL,
	`Username` VARCHAR(50) NULL,
	`AppId` INT(11) NULL);
 
 

INSERT `backand_Environment` (`Id`) VALUES ('development'),('production'),('qa'),('staging');

INSERT INTO `durados_SqlProduct`(`Id`,`Name`,`Ordinal`) VALUES (1,'SQL Server om Premise',1), (2,'SQL Azure',2), (3,'MySQL',3), (4,'Postgre',4);

INSERT INTO `durados_DataSourceType`(`Id`,`Name`,`Ordinal`) VALUES (1,'Blank',1), (2,'Existing Database',2),  (4,'Template',4);

ALTER TABLE `durados_AppLimits` 
ADD UNIQUE INDEX `UNIQUE_AppId_Name` (`AppId` ASC, `Name` ASC);

CREATE VIEW  `v_durados_User`
AS
SELECT ID, Username, FirstName, LastName, Email, Password, Role, Guid, Signature, SignatureHTML, 1 AS IsApproved, FirstName + ' ' + LastName AS FullName, 
               NewUser, Comments
FROM  durados_User;


ALTER TABLE  `durados_App`   ADD  CONSTRAINT `FK_durados_App_backand_Environment` FOREIGN KEY(`Environment`)
REFERENCES  `backand_Environment` (`Id`);


ALTER TABLE  `durados_App` ADD  CONSTRAINT `FK_durados_App_durados_DataSourceType` FOREIGN KEY(`DataSourceTypeId`)
REFERENCES  `durados_DataSourceType` (`Id`);



ALTER TABLE  `durados_App` ADD  CONSTRAINT `FK_durados_App_durados_SqlConnection` FOREIGN KEY(`SqlConnectionId`)
REFERENCES  `durados_SqlConnection` (`Id`);



ALTER TABLE  `durados_App` ADD  CONSTRAINT `FK_durados_App_durados_SqlConnection1` FOREIGN KEY(`SystemSqlConnectionId`)
REFERENCES  `durados_SqlConnection` (`Id`);



ALTER TABLE  `durados_App` ADD  CONSTRAINT `FK_durados_App_durados_SqlConnection2` FOREIGN KEY(`SecuritySqlConnectionId`)
REFERENCES  `durados_SqlConnection` (`Id`);



ALTER TABLE  `durados_App` ADD  CONSTRAINT `FK_durados_App_durados_Template` FOREIGN KEY(`TemplateId`)
REFERENCES  `durados_App` (`Id`);

ALTER TABLE  `durados_App` ADD  CONSTRAINT `FK_durados_App_durados_User` FOREIGN KEY(`Creator`)
REFERENCES  `durados_User` (`Id`);



ALTER TABLE  `durados_AppLimits`   ADD  CONSTRAINT `FK_durados_AppLimits_durados_App` FOREIGN KEY(`AppId`)
REFERENCES  `durados_App` (`Id`);



ALTER TABLE  `durados_AppPlan` ADD  CONSTRAINT `FK_durados_AppPlan_durados_App` FOREIGN KEY(`AppId`)
REFERENCES  `durados_App` (`Id`);



ALTER TABLE  `durados_AppPlan` ADD  CONSTRAINT `FK_durados_AppPlan_durados_Plan` FOREIGN KEY(`PlanId`)
REFERENCES  `durados_Plan` (`Id`);



ALTER TABLE  `durados_ExternaInstance` ADD  CONSTRAINT `FK_durados_ExternaInstance_durados_SqlConnection` FOREIGN KEY(`SqlConnectionId`)
REFERENCES  `durados_SqlConnection` (`Id`);



ALTER TABLE  `durados_SqlConnection` ADD  CONSTRAINT `FK_durados_SqlConnection_durados_SqlProduct` FOREIGN KEY(`SqlProductId`)
REFERENCES  `durados_SqlProduct` (`Id`);



ALTER TABLE  `durados_SqlConnection` ADD  CONSTRAINT `FK_durados_SqlConnection_durados_User` FOREIGN KEY(`DuradosUser`)
REFERENCES  `durados_User` (`ID`)
ON DELETE SET NULL;



ALTER TABLE  `durados_User` ADD  CONSTRAINT `FK_User_durados_UserRole` FOREIGN KEY(`Role`)
REFERENCES  `durados_UserRole` (`Name`);



ALTER TABLE  `durados_UserApp` ADD  CONSTRAINT `FK_durados_UserApp_durados_App` FOREIGN KEY(`AppId`)
REFERENCES  `durados_App` (`Id`)
ON DELETE CASCADE;


ALTER TABLE  `durados_UserApp` ADD  CONSTRAINT `FK_durados_UserApp_durados_User` FOREIGN KEY(`UserId`)
REFERENCES  `durados_User` (`ID`);

ALTER TABLE  `durados_UserSocial`  ADD  CONSTRAINT `FK_durados_UserSocial_durados_User` FOREIGN KEY(`UserId`)
REFERENCES  `durados_User` (`ID`)
ON UPDATE CASCADE
ON DELETE CASCADE;

INSERT INTO durados_UserRole(`Name`,`Description`,`FirstView`)
 VALUES ('Admin','Admin',NULL)
,('Developer','Developer',NULL)
,('Public','Use for internet public access',NULL)
,('User','User',NULL)
,('View Owner','Power user view owner',NULL);


DELIMITER $$

DROP TRIGGER IF EXISTS ___.durados_App_BEFORE_INSERT$$
USE `___`$$
CREATE DEFINER = CURRENT_USER TRIGGER `___`.`durados_App_BEFORE_INSERT` BEFORE INSERT ON `durados_App` FOR EACH ROW
BEGIN

  SET NEW.`Guid` = UUID();
  SET NEW.`SignUpToken` = UUID();
  SET NEW.`AnonymousToken` = UUID();


END$$
DELIMITER ;
DROP procedure IF EXISTS `durados_GetExternalAvailableInstance`;

DELIMITER $$

CREATE PROCEDURE `durados_GetExternalAvailableInstance` (IN productId varchar(150))
BEGIN
IF IFNULL(productId,'') = ''   THEN SET productId = 3; END IF;
SELECT 
    SqlConnectionId
FROM
    `durados_ExternaInstance`
        INNER JOIN
    durados_SqlConnection ON durados_SqlConnection.Id = durados_ExternaInstance.SqlConnectionId
WHERE
    durados_SqlConnection.SqlProductId = productId
        AND IsActive = 1
LIMIT 1;
 
END$$

DELIMITER ;
DROP function IF EXISTS `f_report_connection_type`;
                        DELIMITER $$
                        USE `___`$$
                        CREATE FUNCTION `f_report_connection_type` (_id int)
                        RETURNS INTEGER
                        BEGIN
	                        DECLARE _ResultVar INT;
		                        SELECT CASE 
			                        WHEN ServerName IN(SELECT ServerName 
					                        FROM `durados_ExternaInstance` INNER JOIN durados_SqlConnection  ON durados_SqlConnection.Id = durados_ExternaInstance.SqlConnectionId)
					                        THEN 2  
				                        ELSE 1 END INTO _ResultVar 
			                        FROM durados_SqlConnection AS c 
			                        WHERE id=_id;
                        RETURN _ResultVar;
                        END$$

                        DELIMITER ;



DROP procedure IF EXISTS `durados_SetValidGuid`;

DELIMITER $$

CREATE  PROCEDURE `durados_SetValidGuid`(userGuid VARCHAR(50),timeSpan INT,INOUT id  VARCHAR(36)     )
BEGIN
	
	SET @user_Guid  := userGuid ;-- '457ccc1a-be1c-11e7-84d6-123ae72e6bb4'
    
    
    SET @exists := (  SELECT `Id`  FROM durados_ValidGuid WHERE `UserGuid` = @user_Guid );
	SELECT @exists as YYY;
    
    SET @pk := UUID();
	SET @time_Span := timeSpan;
	call debug_msg(TRUE,@user_Guid);
	SELECT `Id` as tmpId FROM durados_ValidGuid WHERE`UserGuid`= @user_Guid AND ( (`time`>=NOW() AND IFNULL(used,0) =0) OR ApprovedByAdmin =1) ; 
	
	call debug_msg(TRUE,@exists);
	IF @exists  IS NULL 
	THEN
		SET @now :=date_add(NOW(), INTERVAL @time_Span MINUTE  );
		INSERT INTO durados_ValidGuid (`Id`,`UserGuid`,`Time`) VALUES (@pk,@user_Guid,@now);
		SET id:=@pk;
	ELSE
	
		SET id:=@exists;
	
	END IF;
       
END$$

DELIMITER ;


INSERT `backand_ActionTemplate` (`id`, `name`, `shortDescription`, `documentation`, `createdDate`, `updatedDate`, `createdBy`, `updatedBy`, `ruleName`, `objectName`, `action`, `ruleType`, `condition`, `parameters`, `code`, `executeCommand`, `executeMessage`, `json`, `backands`, `approved`, `approvedDate`, `contributors`, `category`, `ordinal`) VALUES (3, N'MailGun', N'A set of powerful APIs that enable you to send, receive and track email from your app effortlessly ', NULL, CAST(N'2015-08-16 19:35:01.343' AS DateTime), NULL, NULL, NULL, N'SendEmailMailgun', NULL, N'OnDemand', N'JavaScript', N'true', NULL, N'/* globals
  $http - service for AJAX calls - $http({method:"GET",url:CONSTS.apiUrl + "/1/objects/yourObject" , headers: {"Authorization":userProfile.token}});
  CONSTS - CONSTS.apiUrl for Backands API URL
*/
''use strict'';
function backandCallback(userInput, dbRow, parameters, userProfile) {
	// write your code here
	
	var apiBaseUrl = "https://api.mailgun.net/v3/sandbox4bffb0c5ad584267a13048507cc4b502.mailgun.org/messages";

    var apiKey = "api:key-bbdfb498e1c10e8cb3f818ad1b4139b7";
                  
    var encodedAuth = btoa(apiKey);
    //console.log(encodedAuth);
    
    /*curl -s --user ''api:key-bbdfb498e1c10e8cb3f818ad1b4139b7'' \
    https://api.mailgun.net/v3/sandbox4bffb0c5ad584267a13048507cc4b502.mailgun.org/messages \
    -d from=''Excited User <mailgun@sandbox4bffb0c5ad584267a13048507cc4b502.mailgun.org>'' \
    -d to=itay@backand.com \
    -d subject=''Hello'' \
    -d text=''Testing some Mailgun awesomness!''*/
    
	var response = $http(
	    {
	        method:"POST",
	        url: apiBaseUrl, 
	        headers: {
	            "Authorization": "Basic " + encodedAuth,
	            "Content-Type" : "multipart/form-data; charset=utf-8",
	        },
	        data: {
	            from: ''Excited User <mailgun@sandbox4bffb0c5ad584267a13048507cc4b502.mailgun.org>'', 
	            to: ''itay@backand.com'', 
	            subject: ''Hello'', 
	            text: ''Testing some Mailgun awesomness!''
	        }
	        
	    }
	);
	
    console.logareturn {};
}', NULL, NULL, N'{"imageUrl":"assets/images/actions/mailgun.png"}', 1, 1, NULL, NULL, 1, 3);

INSERT `backand_ActionTemplate` (`id`, `name`, `shortDescription`, `documentation`, `createdDate`, `updatedDate`, `createdBy`, `updatedBy`, `ruleName`, `objectName`, `action`, `ruleType`, `condition`, `parameters`, `code`, `executeCommand`, `executeMessage`, `json`, `backands`, `approved`, `approvedDate`, `contributors`, `category`, `ordinal`) VALUES (4, N'Mandrill', N'Mandrill is a transactional email platform from MailChimp', NULL, CAST(N'2015-08-16 19:36:18.757' AS DateTime), NULL, NULL, NULL, N'SendEmailMandrill', NULL, N'OnDemand', N'JavaScript', N'true', NULL, N'/* globals
  $http - Service for AJAX calls 
  CONSTS - CONSTS.apiUrl for Backands API URL
*/
''use strict'';
function backandCallback(userInput, dbRow, parameters, userProfile) {
	// write your code here
    var response = $http({
      method: "POST",
      url: "https://mandrillapp.com/api/1.0/messages/send.json",
      data: {"key":"kPE24YlgNINqzULeTzlkJw",
            "message":{"html":"<p>Example HTML content<\/p>","text":"Example text content",
            "subject":"example subject 123",
            "from_email":"itay@backand.com","from_name":"itay her",
            "to":`{"email":"itay@backand.com","name":"itay to","type":"to"}`,
            "headers":{"Reply-To":"message.reply@backand.com"}}}
    });
    console.log(response);
	return {};
}', NULL, NULL, N'{"imageUrl":"assets/images/actions/mandrill.png"}', 1, 1, NULL, NULL, 1, 4)
;
INSERT `backand_ActionTemplate` (`id`, `name`, `shortDescription`, `documentation`, `createdDate`, `updatedDate`, `createdBy`, `updatedBy`, `ruleName`, `objectName`, `action`, `ruleType`, `condition`, `parameters`, `code`, `executeCommand`, `executeMessage`, `json`, `backands`, `approved`, `approvedDate`, `contributors`, `category`, `ordinal`) VALUES (5, N'Stripe', N'Stripe is a suite of APIs that powers commerce for businesses of all sizes.', NULL, CAST(N'2015-08-16 19:39:08.833' AS DateTime), NULL, NULL, NULL, N'StripePayment', NULL, N'OnDemand', N'JavaScript', N'true', N'amount, token', N'/* globals
  $http - service for AJAX calls - $http({method:"GET",url:CONSTS.apiUrl + "/1/objects/yourObject" , headers: {"Authorization":userProfile.token}});
  CONSTS - CONSTS.apiUrl for Backands API URL
*/
''use strict'';
function backandCallback(userInput, dbRow, parameters, userProfile) {
	// write your code here
    
    /*var response = $http({
      method: "GET",
      url: "https://api.stripe.com/v1/charges",
      params: {},
      headers: {"Authorization": "Basic " + user}
    });*/
    
    var user = btoa("sk_test_hx4i19p4CJVwJzdf7AajsbBr:");
    
    var response = $http({
      method: "POST",
      url: "https://api.stripe.com/v1/charges",
      params: {
          "amount":parameters.amount,
          "currency":"usd",
          "source": parameters.token
      },
      headers: {
          "Authorization": "Basic " + user,
          "Accept" : "application/json",
          "Content-Type": "application/x-www-form-urlencoded"
      }
    });
    
    
    /*curl https://api.stripe.com/v1/charges \
   -u sk_test_hx4i19p4CJVwJzdf7AajsbBr: \
   -H "Idempotency-Key: icrLf2csFYhT5LZs" \
   -d amount=400 \
   -d source=tok_16WkaaKsreSeTjOvvuOEkFTM \
   -d currency=usd*/
    
	return {"data":response};
}', NULL, NULL, N'{"imageUrl":"assets/images/actions/stripe.png","docsUrl":"http://docs.backand.com/en/latest/scenarios/stripe/index.html"}', 1, 1, NULL, NULL, 2, 5)
;
INSERT `backand_ActionTemplate` (`id`, `name`, `shortDescription`, `documentation`, `createdDate`, `updatedDate`, `createdBy`, `updatedBy`, `ruleName`, `objectName`, `action`, `ruleType`, `condition`, `parameters`, `code`, `executeCommand`, `executeMessage`, `json`, `backands`, `approved`, `approvedDate`, `contributors`, `category`, `ordinal`) VALUES (6, N'S3', N'Opt for virtually unlimited cloud & internet storage with Amazon S3. Quickly store & retrieve vast amounts of data from anywhere on the web.', NULL, CAST(N'2015-08-16 21:27:32.103' AS DateTime), NULL, NULL, NULL, N'S3FileUpload', NULL, N'OnDemand', N'JavaScript', N'true', N'filename, filedata', N'/* globals
  $http - service for AJAX calls - $http({method:"GET",url:CONSTS.apiUrl + "/1/objects/yourObject" , headers: {"Authorization":userProfile.token}});
  CONSTS - CONSTS.apiUrl for Backands API URL
*/
''use strict'';
function backandCallback(userInput, dbRow, parameters, userProfile) {
	// write your code here

	var data = 
    {
        // enter your aws key
        "key" : "<your client key>", 

        // enter your aws secret key
        "secret" : "<your secret key>", 

        // this should be sent in post body
        "filename" : parameters.filename, 
        "filedata" : parameters.filedata,         

        "region" : "US Standard",
        "bucket" : "backand-free-upload"

    }
    var response = $http({method:"PUT",url:CONSTS.apiUrl + "/1/file/s3" , 
               data: data, headers: {"Authorization":userProfile.token}});

    return response;
}', NULL, NULL, N'{"imageUrl":"assets/images/actions/s3.png","docsUrl":"http://docs.backand.com/en/latest/apidocs/customactions/index.html#third-party-integrations"}', 1, 1, NULL, NULL, 3, 7)
;
INSERT `backand_ActionTemplate` (`id`, `name`, `shortDescription`, `documentation`, `createdDate`, `updatedDate`, `createdBy`, `updatedBy`, `ruleName`, `objectName`, `action`, `ruleType`, `condition`, `parameters`, `code`, `executeCommand`, `executeMessage`, `json`, `backands`, `approved`, `approvedDate`, `contributors`, `category`, `ordinal`) VALUES (21, N'PayPal', N'PayPal is a suite of APIs for secure transfer of funds between member accounts.', NULL, CAST(N'2015-10-07 19:39:08.833' AS DateTime), NULL, NULL, NULL, N'PayPalPayment', NULL, N'OnDemand', N'JavaScript', N'true', N'amount,process,paymentId,payerId', N'/* globals
 $http - Service for AJAX calls
 CONSTS - CONSTS.apiUrl for Backands API URL
 cookie - save session data in the server
 */
''use strict'';
function backandCallback(userInput, dbRow, parameters, userProfile) {

  var paypalUrl = ''https://api.sandbox.paypal.com/'';

  // PayPal has a 2 stage process where: // the first stage prepares the payment and a returns a url for the user to pay
  // and the sconde stage (where the payment is actually done) is where the application send an approval API call to PayPal
  if (parameters.process == ''payment'') {
    var payment = postPayment();
    return payment.links`1`.href;
  }
  else if (parameters.process == ''approval'') {
    return postApproval();
  }

  function postPayment() {
    var authorization = "Bearer " + getAccessToken().access_token;
    var payment = {
      "intent": "sale",
      "redirect_urls": {
        "return_url": "http://localhost:3000/#/paypal",
        "cancel_url": "http://localhost:3000/#/paypal?fail=true"
      },
      "payer": {"payment_method": "paypal"},
      "transactions": `
        {
          "amount": {
            "total": parameters.amount,
            "currency": "USD"
          }
        }
      `
    };
    try {
      return $http({
        method: ''POST'',
        url: paypalUrl + ''v1/payments/payment'',
        data: payment,
        headers: {
          "Content-Type": "application/json",
          "Accept-Language": "en_US",
          "Authorization": authorization

        }
      });
    }
    catch (err) {
      if (err.name == 401) {
        cookie.remove(''paypal_token'');
        return postPayment();
      }
      else {
        throw err;
      }
    }
  }

  function postApproval() {
    var authorization = "Bearer " + getAccessToken().access_token;
    var payer = {"payer_id": parameters.payerId};
    try {
      return $http({
        method: ''POST'',
        url: paypalUrl + ''v1/payments/payment/'' + parameters.paymentId + ''/execute/'',
        data: JSON.stringify(payer),
        headers: {"Content-Type": "application/json", "Accept-Language": "en_US", "Authorization": authorization}
      });
    }
    catch (err) {
      if (err.name == 401) {
        cookie.remove(''paypal_token'');
        return postApproval();
      }
      else {
        throw err;
      }
    }
  }

  function getAccessToken() {
    var token = cookie.get(''paypal_token'');
    if (!token) {
      var ClientId = ''YOUR_PayPal_CLIENT_ID'';
      var Secret = ''YOUR_PayPal_SECRET_KEY'';
      var user = btoa(ClientId + ":" + Secret);

      try {
        token = $http(
          {
            method: ''POST'',
            url: paypalUrl + ''v1/oauth2/token'',
            data: ''grant_type=client_credentials'',
            headers: {
              "Accept-Language": "en_US",
              "Authorization": "Basic " + user
            }

          });
      }
      catch (err) {
        if (err.name == 401) {
          var e = new Error("Unauthorized (401), check client id and secret");
          e.name = err.name;
          throw e;
        }
        else {
          throw err;
        }
      }
      cookie.put(''paypal_token'', token);
    }
    return token;
  }
  
}
', NULL, NULL, N'{"imageUrl":"assets/images/actions/paypal.png","docsUrl":"http://docs.backand.com/en/latest/scenarios/PayPal/index.html"}', 1, 1, NULL, NULL, 2, 6)
;
INSERT `backand_ActionTemplate` (`id`, `name`, `shortDescription`, `documentation`, `createdDate`, `updatedDate`, `createdBy`, `updatedBy`, `ruleName`, `objectName`, `action`, `ruleType`, `condition`, `parameters`, `code`, `executeCommand`, `executeMessage`, `json`, `backands`, `approved`, `approvedDate`, `contributors`, `category`, `ordinal`) VALUES (22, N'Segment', N'Segment collect customer data and send it to your tools for analytics and  marketing automation', NULL, CAST(N'2015-10-08 08:39:16.717' AS DateTime), NULL, NULL, NULL, N'SegmentAnalitics', NULL, N'OnDemand', N'JavaScript', N'true', N'userId,activeApp', N'/* globals
 $http - Service for AJAX calls
 CONSTS - CONSTS.apiUrl for Backands API URL
 */
''use strict'';
function backandCallback(userInput, dbRow, parameters, userProfile) {
  // write your code here

  var writeKey = ''YOUR_SEGMENT_IO_WRITE_KEY'';
  var authorization = ''Basic '' + btoa(writeKey);
  var segmentUrl = ''https://api.segment.io/v1/identify'';

  var segmontPostResponse = $http(
    {
      method: ''POST'',
      url: ''https://api.segment.io/v1/identify'',
      data: {
        "userId": parameters.userId,
        "traits": {
          "email": parameters.userId,
          "ActiveApps": parameters.activeApp
        },


        "integrations": {
          "All": false,
          "Woopra": true
        }
      },
      headers: {''Authorization'': authorization, ''Accept'': ''application/json'', ''Content-Type'': ''application/json''}

    });

  return segmontPostResponse;
}', NULL, NULL, N'{"imageUrl":"assets/images/actions/segment.png","docsUrl":"http://docs.backand.com/en/latest/scenarios/segment/index.html"}', 1, 1, NULL, NULL, 4, 8)
;
INSERT `backand_ActionTemplate` (`id`, `name`, `shortDescription`, `documentation`, `createdDate`, `updatedDate`, `createdBy`, `updatedBy`, `ruleName`, `objectName`, `action`, `ruleType`, `condition`, `parameters`, `code`, `executeCommand`, `executeMessage`, `json`, `backands`, `approved`, `approvedDate`, `contributors`, `category`, `ordinal`) VALUES (23, N'Realtime Communication', N'Backand’s Real-Time Communication sends events and JSON-formatted data to any authorized connected client. Using this action you can send real-time information to your application based on server-side JavaScript logic.', NULL, CAST(N'2015-11-24 14:17:58.437' AS DateTime), NULL, NULL, NULL, N'SendRealtimeEvent', NULL, N'OnDemand', N'JavaScript', N'true', NULL, N'/* globals
  $http - Service for AJAX calls 
  CONSTS - CONSTS.apiUrl for Backands API URL
  Config - Global Configuration
  socket - Send realtime database communication
*/
''use strict'';
function backandCallback(userInput, dbRow, parameters, userProfile) {
    
    //Send to array of users
    //socket.emitUsers("items_updated",userInput, `"user2@gmail.com","user1@gmail.com"`);

    //Send to specific role 
    //socket.emitRole("items_updated",userInput, "User");
    
	//Send to all users
    //socket.emitAll("items_updated",userInput);
    
	return {};
}', NULL, NULL, N'{"imageUrl":"assets/images/actions/logo-realtime.png","docsUrl":"http://docs.backand.com/en/latest/apidocs/realtime/index.html"}', 1, 1, NULL, NULL, 5, 2)
;
INSERT `backand_ActionTemplate` (`id`, `name`, `shortDescription`, `documentation`, `createdDate`, `updatedDate`, `createdBy`, `updatedBy`, `ruleName`, `objectName`, `action`, `ruleType`, `condition`, `parameters`, `code`, `executeCommand`, `executeMessage`, `json`, `backands`, `approved`, `approvedDate`, `contributors`, `category`, `ordinal`) VALUES (24, N'Backand Storage', N'Easily access your account''s S3 file storage, without opening an AWS account', NULL, CAST(N'2016-01-14 14:30:03.067' AS DateTime), NULL, NULL, NULL, N'files', NULL, N'OnDemand', N'JavaScript', N'true', N'filename, filedata', N'/* globals
  $http - Service for AJAX calls 
  CONSTS - CONSTS.apiUrl for Backands API URL
  Config - Global Configuration
  socket - Send realtime database communication
  files - file handler, performs upload and delete of files
  request - the current http request
*/
''use strict'';
function backandCallback(userInput, dbRow, parameters, userProfile) {
	// upload file
    if (request.method == "POST"){
        var url = files.upload(parameters.filename, parameters.filedata);
        return {"url": url};
    }
    // delete file
    else if (request.method == "DELETE"){
        files.delete(parameters.filename);
        return {};    
    }
	
}', NULL, NULL, N'{"imageUrl":"assets/images/actions/file-upload.png","docsUrl":"http://backand.github.io/HelpPages/en/latest/apidocs/files/index.html"}', 1, 1, NULL, NULL, 5, 3)
;
INSERT `backand_ActionTemplate` (`id`, `name`, `shortDescription`, `documentation`, `createdDate`, `updatedDate`, `createdBy`, `updatedBy`, `ruleName`, `objectName`, `action`, `ruleType`, `condition`, `parameters`, `code`, `executeCommand`, `executeMessage`, `json`, `backands`, `approved`, `approvedDate`, `contributors`, `category`, `ordinal`) VALUES (25, N'SendGrid', N'SendGrid is a transactional and marketing email platform', NULL, CAST(N'2016-01-18 10:18:25.690' AS DateTime), NULL, NULL, NULL, N'SendEmailSendGrid', NULL, N'OnDemand', N'JavaScript', N'true', N'to,message', N'/* globals
  $http - Service for AJAX calls 
  CONSTS - CONSTS.apiUrl for Backands API URL
  Config - Global Configuration
  socket - Send realtime database communication
*/
''use strict'';
function backandCallback(userInput, dbRow, parameters, userProfile) {
 // write your code here
    var res = $http({
        method:"POST",
        url:"https://api.sendgrid.com/api/mail.send.json",
        data:
            "from=" + userProfile.username +
            "&to="  + parameters.to +
            "&subject=hellow"+
            "&html="+ parameters.message,

         headers: {"Accept":"Accept:application/json",
                "Content-Type": "application/x-www-form-urlencoded",
                "Authorization":"Bearer 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"
         }
     });

     return res;
}', NULL, NULL, N'{"imageUrl":"assets/images/actions/logo-send-grid.jpg"}', 1, 1, NULL, NULL, 1, 9)
;
INSERT `backand_ActionTemplate` (`id`, `name`, `shortDescription`, `documentation`, `createdDate`, `updatedDate`, `createdBy`, `updatedBy`, `ruleName`, `objectName`, `action`, `ruleType`, `condition`, `parameters`, `code`, `executeCommand`, `executeMessage`, `json`, `backands`, `approved`, `approvedDate`, `contributors`, `category`, `ordinal`) VALUES (27, N'Twilio', N'Twilio enables you to make voice, video calls and sends and receive global SMS, MMS and
IP messages  around the globe  and more', NULL, CAST(N'2016-01-25 13:20:43.387' AS DateTime), NULL, NULL, NULL, N'SendSMSWithTwilio', NULL, N'OnDemand', N'JavaScript', N'true', N'to, message', N'/* globals
  $http - Service for AJAX calls
  CONSTS - CONSTS.apiUrl for Backands API URL
  Config - Global Configuration
  socket - Send realtime database communication
  files - file handler, performs upload and delete of files
  request - the current http request
*/
''use strict'';
function backandCallback(userInput, dbRow, parameters, userProfile) {
	// write your code here
	var ACCOUNT_SID = ''AC2933cadba659d1f15bd409333e3bc38b'';
    var AUTH_TOKEN = ''e924350f6eefb5dd1ae011b49d5cb5dd'';

    var FROM_PHONE_NUM = ''GetEat'';

    var basicUrl = ''https://api.twilio.com/2010-04-01/Accounts/'' + ACCOUNT_SID + ''/'';
    var action = ''Messages.json'' // twilio have many services

    return $http({
        method: "POST",
        url: basicUrl + action,
        data:
            ''Body='' + parameters.message +
            ''&To=''  + parameters.to +
            ''&From=''+ FROM_PHONE_NUM,
        headers: {
            "Accept":"Accept:application/json",
            "Content-Type": "application/x-www-form-urlencoded",
            "Authorization": ''basic '' + btoa(ACCOUNT_SID + '':'' + AUTH_TOKEN)
        }
    });

}', NULL, NULL, N'{"imageUrl":"assets/images/actions/twilio.png","docsUrl":"http://docs.backand.com/en/latest/scenarios/twilio/index.htm"}', 1, 1, NULL, NULL, 1, 4)
;
INSERT `backand_ActionTemplate` (`id`, `name`, `shortDescription`, `documentation`, `createdDate`, `updatedDate`, `createdBy`, `updatedBy`, `ruleName`, `objectName`, `action`, `ruleType`, `condition`, `parameters`, `code`, `executeCommand`, `executeMessage`, `json`, `backands`, `approved`, `approvedDate`, `contributors`, `category`, `ordinal`) VALUES (32, N'Netmera', N'Netmera is a cloud based service that can be used to send Push Notifications to various platforms, among other services such as Exception reporting.', NULL, CAST(N'2016-03-20 13:20:43.387' AS DateTime), NULL, NULL, NULL, N'NetmeraPushNotification', NULL, N'OnDemand', N'JavaScript', N'true', N'notificationTitle, notificationContent', N'/* globals
 $http - Service for AJAX calls
 CONSTS - CONSTS.apiUrl for Backands API URL
 Config - Global Configuration
 socket - Send realtime database communication
 files - file handler, performs upload and delete of files
 request - the current http request
 */
''use strict'';
function backandCallback(userInput, dbRow, parameters, userProfile) {
    
    // Netmera REST API key. Get it from https://cp.netmera.com/<<your-app-name>>/admin/overview when logged in
    var NETMERA_API_KEY = ''IERqFD2KavCtgQD-4cfQGfn512JvBN81_xSon1L-sbt_BxrkySGwqw'';
    var basicUrl = ''https://api.netmera.com/push/1.2/'';
    var action = ''notification'';

    // Notification title for logging purposes (does not appear on the notification itself)
    var notificationTitle = parameters.notificationTitle;
    
    // Notification message
    var notificationContent = parameters.notificationContent;
    
    return $http({
        method: "POST",
        url: basicUrl + action,
        data: {
            target: {
                "platforms": `"ANDROID", "IOS"`
            },
            notification: {
                "title": notificationTitle,
                "notificationMsg": notificationContent
            }
        },
        headers: {
            "X-netmera-api-key": NETMERA_API_KEY,
            "Content-Type": "application/json"
        }
    });

    // Netmera has a more advanced API that supports filters by platforms or by tags
    // You can define tags on the Netmera CP
    
}', NULL, NULL, N'{"imageUrl":"assets/images/actions/logo-netmera.png","docsUrl":"http://docs.backand.com/en/latest/scenarios/netmera/index.htm"}', 1, 1, NULL, NULL, 6, 1)
;
INSERT `backand_ActionTemplate` (`id`, `name`, `shortDescription`, `documentation`, `createdDate`, `updatedDate`, `createdBy`, `updatedBy`, `ruleName`, `objectName`, `action`, `ruleType`, `condition`, `parameters`, `code`, `executeCommand`, `executeMessage`, `json`, `backands`, `approved`, `approvedDate`, `contributors`, `category`, `ordinal`) VALUES (33, N'PushWoosh', N'Pushwoosh is a cloud based service that can be used to send Push Notifications to various platforms. . It offers a friendly site where push notifications can be sent and customized.', NULL, CAST(N'2016-03-20 13:20:43.387' AS DateTime), NULL, NULL, NULL, N'PushWooshPushNotification', NULL, N'OnDemand', N'JavaScript', N'true', N'notificationContent', N'/* globals
  $http - Service for AJAX calls 
  CONSTS - CONSTS.apiUrl for Backands API URL
  Config - Global Configuration
  socket - Send realtime database communication
  files - file handler, performs upload and delete of files
  request - the current http request
*/
''use strict'';
function backandCallback(userInput, dbRow, parameters, userProfile) {
    
    // Get the code from the application section in the PushWoosh control panel - https://cp.pushwoosh.com/v2/applications
    var APPLICATION_CODE = ''B8676-923DE'';
    
    // Get the auth token from the API Access tab in the PushWoosh control panel - https://cp.pushwoosh.com/v2/api_access
    var AUTH_TOKEN = ''5QrTsm8K0KHH81q4V6QZz0T5TasqkKVZk4yVpoDpmlhZpAEvDdOMdFP4Isxuq04D4juS1qsiCS8ceYWO6AUb'';
    
    //PushWoosh base REST API Url
    var basicUrl = ''https://cp.pushwoosh.com/json/1.3/'';
    
    //Send message API Action
    var action = ''createMessage'';
    
    // Notification message
    var notificationContent = parameters.notificationContent;

    return $http({
        method: "POST",
        url: basicUrl + action,
        data:
        {
            "request": {
                "application": APPLICATION_CODE,
                "auth": AUTH_TOKEN,
                "notifications": `
                    {
                        "send_date": "now",
                        "ignore_user_timezone": true,
                        "content": notificationContent
                    }
                `
            }
        },
        headers: {
            "Accept":"application/json",
            "Content-Type": "application/json"
        }
    });
    
    // It''s also possible to send a targeted message using a filter
    // In the following example we''re sending a targeted message only to Android and iOS devices
    // See more filter possibilities at http://docs.pushwoosh.com/docs/createtargetedmessage

    //var action = ''createTargetedMessage'';
    //
    //return $http({
    //    method: "POST",
    //    url: basicUrl + action,
    //    data: {
    //        "request": {
    //            "auth": AUTH_TOKEN,
    //            "send_date": "now",
    //            "content": notificationContent,
    //            "devices_filter": "A(\"" + APPLICATION_CODE + "\", `\"Android\", \"iOS\"`)"
    //        }
    //    },
    //    headers: {
    //        "Accept": "application/json",
    //        "Content-Type": "application/json"
    //    }
    //});
}', NULL, NULL, N'{"imageUrl":"assets/images/actions/logo-pushwoosh.png","docsUrl":"http://docs.backand.com/en/latest/scenarios/pushwoosh/index.htm"}', 1, 1, NULL, NULL, 6, 2)
;
INSERT `backand_ActionTemplate` (`id`, `name`, `shortDescription`, `documentation`, `createdDate`, `updatedDate`, `createdBy`, `updatedBy`, `ruleName`, `objectName`, `action`, `ruleType`, `condition`, `parameters`, `code`, `executeCommand`, `executeMessage`, `json`, `backands`, `approved`, `approvedDate`, `contributors`, `category`, `ordinal`) VALUES (34, N'Facebook Messenger Bot', N'Introducing new tools to help you build your bot in Facebook Messenger. Messenger bots are programs that receive messages from an interface, process those messages, and then send results back to the caller.', NULL, CAST(N'2016-09-08 12:14:38.243' AS DateTime), NULL, NULL, NULL, N'FBMessengerBot', NULL, N'OnDemand', N'JavaScript', N'true', NULL, N'
/* globals
  $http - Service for AJAX calls 
  CONSTS - CONSTS.apiUrl for Backands API URL
  Config - Global Configuration
  socket - Send realtime database communication
  files - file handler, performs upload and delete of files
  request - the current http request
*/
''use strict'';
function backandCallback(userInput, dbRow, parameters, userProfile) {
	// write your code here

    var PAGE_ACCESS_TOKEN = ''PAGE_ACCESS_TOKEN'';

    console.log({"FBMessengerBot start": request});
    
    //GET method is only to verify the webhook when adding it from FB UI
    if (request.method == "GET"){
    
        if(request.query`''hub.verify_token''` == "my_test_token")
            return Number(request.query`''hub.challenge''`);
        else
    	    throw new Error(''Faild verification'');
    }

    //*************************************
    // POST code starts here
    //*************************************
    
    //Handle all the POST requests from the FB messenger UI
    if (request.method == "POST"){
        
        var data = request.body;
        if (data.object == ''page'') {
            
            // Iterate over each entry
            // There may be multiple if batched
            data.entry.forEach(function(pageEntry) {
                
                var pageID = pageEntry.id;
                var timeOfEvent = pageEntry.time;
        
                // Iterate over each messaging event
                pageEntry.messaging.forEach(function(messagingEvent) {
                    if (messagingEvent.optin) {
                        //receivedAuthentication(messagingEvent);
                    } else if (messagingEvent.message) {
                        receivedMessage(messagingEvent);
                    } else if (messagingEvent.delivery) {
                        //receivedDeliveryConfirmation(messagingEvent);
                    } else if (messagingEvent.postback) {
                        receivedPostback(messagingEvent);
                    } else {
                        console.log("Webhook received unknown messagingEvent: ", messagingEvent);
                    }
                });
            });
        }
    }
    
    //In receivedMessage, we''ve made logic to send a message back to the user. 
    //The default behavior is to echo back the text that was received in addtion to static text (''Back& bot says'').
    function receivedMessage(event) {
        
        var senderID = event.sender.id;
        var recipientID = event.recipient.id;
        var timeOfMessage = event.timestamp;
        var message = event.message;
        
        console.log("Received message for user" + senderID + " and page " + recipientID + " at " + timeOfMessage + " with message");
        console.log(JSON.stringify(message));
        
        var messageId = message.mid;
        
        // You may get a text or attachment but not both
        var messageText = message.text;
        var messageAttachments = message.attachments;
        
        if (messageText) {
        
        // If we receive a text message, check to see if it matches any special
        // keywords and send back the corresponding example. Otherwise, just echo
        // the text we received.
        switch (messageText) {
              case ''image'':
                //sendImageMessage(senderID);
                break;
            
              case ''button'':
                //sendButtonMessage(senderID);
                break;
            
              case ''backand'':
              case ''Backand'':      
                sendGenericMessage(senderID);
                break;
            
              case ''receipt'':
                //sendReceiptMessage(senderID);
                break;
            
              default:
                sendTextMessage(senderID, messageText);
            }
        } else if (messageAttachments) {
            sendTextMessage(senderID, "Message with attachment received");
        }
    }
    
    //formats the data in the request
    function sendTextMessage(recipientId, messageText) {
        var messageData = {
            recipient: {
              id: recipientId
            },
            message: {
              text: "Back& bot says: " + messageText
            }
        };
        
        callSendAPI(messageData);
    }
    
    //calls the Send API of FB
    function callSendAPI(messageData) {
        try{
            
            var response = $http({
                method: "POST",
                url:"https://graph.facebook.com/v2.6/me/messages",
                params:{
                    "access_token": PAGE_ACCESS_TOKEN
                },
                data: messageData,
                headers:{"Content-Type":"application/json"}
            });
            
            var recipientId = response.recipient_id;
            var messageId = response.message_id;
            
            console.log("Successfully sent generic message with id " + messageId + " to recipient " + recipientId); 
        }
        catch(err){
            console.error("Unable to send message.");
            console.error(err);
        }
    
    }
    
    //Sends back a Structured Message with a generic template.
    //if you send the message ''backand''
    function sendGenericMessage(recipientId) {
        var messageData = {
            recipient: {
                id: recipientId
            },
            message: {
                attachment: {
                    type: "template",
                    payload: {
                        template_type: "generic",
                        elements: `{
                            title: "Messanger BAAS",
                            subtitle: "Backand as a service for Facebook Messanger",
                            item_url: "https://www.backand.com/features/",               
                            image_url: "https://www.backand.com/wp-content/uploads/2016/01/endless.gif",
                            buttons: `{
                                type: "web_url",
                                url: "https://www.backand.com/features/",
                                title: "Open Web URL"
                            }, {
                                type: "postback",
                                title: "Call Postback",
                                payload: "Payload for first bubble",
                            }`,
                        }, {
                            title: "3rd Party Integrations",
                            subtitle: "Connect your Bot to 3rd party services and applications",
                            item_url: "https://www.backand.com/integrations/",               
                            image_url: "https://www.backand.com/wp-content/uploads/2016/01/3.png",
                            buttons: `{
                                type: "web_url",
                                url: "https://www.backand.com/integrations/",
                                title: "Open Web URL"
                            }, {
                                type: "postback",
                                title: "Call Postback",
                                payload: "Payload for second bubble",
                            }`
                        }`
                    }
                }
            }
        };  
        callSendAPI(messageData);
    }
    
    function receivedPostback(event) {
      var senderID = event.sender.id;
      var recipientID = event.recipient.id;
      var timeOfPostback = event.timestamp;
    
      // The ''payload'' param is a developer-defined field which is set in a postback 
      // button for Structured Messages. 
      var payload = event.postback.payload;
    
      console.log("Received postback for user " + senderID + " and page " + recipientID + "with payload ''" + payload + "'' " + 
        "at " + timeOfPostback);
    
      // When a postback is called, we''ll send a message back to the sender to 
      // let them know it was successful
      sendTextMessage(senderID, "Postback called");
    }
}', NULL, NULL, N'{"imageUrl":"assets/images/actions/fb-bots-logo.png","docsUrl":"https://gist.github.com/itayher/abcd197459c1718de24a8f4ead5c1c05"}', 1, 1, NULL, NULL, 7, 1)
;
INSERT `backand_ActionTemplate` (`id`, `name`, `shortDescription`, `documentation`, `createdDate`, `updatedDate`, `createdBy`, `updatedBy`, `ruleName`, `objectName`, `action`, `ruleType`, `condition`, `parameters`, `code`, `executeCommand`, `executeMessage`, `json`, `backands`, `approved`, `approvedDate`, `contributors`, `category`, `ordinal`) VALUES (37, N'node.js / Lambda', N'Backand''s server-side Node.JS Lambda actions use node.js code that is developed on your local machine, and runs as Lambda function. The action code is built like any other Node.JS project, when done deploy it.', NULL, CAST(N'2017-03-08 00:00:00.000' AS DateTime), NULL, NULL, NULL, NULL, NULL, N'OnDemand', N'NodeJS', N'true', NULL, NULL, NULL, NULL, N'{"imageUrl":"assets/images/actions/logo-nodejs.jpg","docsUrl":"http://docs.backand.com/en/latest/apidocs/customactions/#server-side-node-js-code"}', 1, 1, NULL, NULL, 5, 1)
;
INSERT `backand_ActionTemplate` (`id`, `name`, `shortDescription`, `documentation`, `createdDate`, `updatedDate`, `createdBy`, `updatedBy`, `ruleName`, `objectName`, `action`, `ruleType`, `condition`, `parameters`, `code`, `executeCommand`, `executeMessage`, `json`, `backands`, `approved`, `approvedDate`, `contributors`, `category`, `ordinal`) VALUES (38, N'SalesforceIQ', N'Quickly integrate with SalesforceIQ, bringing the full power of your CRM data to your Backand application.', NULL, CAST(N'2017-03-08 15:32:56.857' AS DateTime), NULL, NULL, NULL, N'SalesforceIQ', NULL, N'OnDemand', N'JavaScript', N'true', NULL, N'/* globals
  $http - Service for AJAX calls 
  CONSTS - CONSTS.apiUrl for Backands API URL
  Config - Global Configuration
  socket - Send realtime database communication
  files - file handler, performs upload and delete of files
  request - the current http request
*/
''use strict'';
function backandCallback(userInput, dbRow, parameters, userProfile) {
	
	var API_KEY = ''--Your API Key--'';
    var API_SECRET = ''--Your Secret--'';
	
	//get accounts
    var response = $http({
        method: "GET",
        url: "https://api.salesforceiq.com/v2/accounts",
        headers: {
            "Accept":"application/json",
            "Authorization": ''basic '' + btoa(API_KEY + '':'' + API_SECRET)
        }
    });
    
	return response;
}', NULL, NULL, N'{"imageUrl":"assets/images/actions/logo-salesforceiq.jpg","docsUrl":"http://docs.backand.com/en/latest/scenarios/salesforceiq/index.html"}', 1, 1, NULL, NULL, 8, 1)
;
INSERT `backand_ActionTemplate` (`id`, `name`, `shortDescription`, `documentation`, `createdDate`, `updatedDate`, `createdBy`, `updatedBy`, `ruleName`, `objectName`, `action`, `ruleType`, `condition`, `parameters`, `code`, `executeCommand`, `executeMessage`, `json`, `backands`, `approved`, `approvedDate`, `contributors`, `category`, `ordinal`) VALUES (43, N'SalesforceCRM', N'Integrate with Salesforce CRM, allowing you to use your Backand application to gather and act upon your valuable customer insights.', NULL, CAST(N'2017-03-08 15:39:44.257' AS DateTime), NULL, NULL, NULL, N'SalesforceCRM', NULL, N'OnDemand', N'JavaScript', N'true', NULL, N'/* globals
  $http - Service for AJAX calls 
  CONSTS - CONSTS.apiUrl for Backands API URL
  Config - Global Configuration
  socket - Send realtime database communication
  files - file handler, performs upload and delete of files
  request - the current http request
*/
''use strict'';
function backandCallback(userInput, dbRow, parameters, userProfile) {
	// write your code here
	var baseUrl = "https://eu11.salesforce.com/services/data/v37.0/";

	var loginSalesForce = function() {
 
    	var client = "-- Your Client Key --"; //Consumer Key
    	var secret = "-- Your Secret --"; //Consumer Secret
    	
    	var username = "--username--";
    	var password = "--password--";
    	
    	var loginUrl = "https://login.salesforce.com/services/oauth2/token";
    	
        var response = $http({
            method: "POST",
            url: loginUrl,
            data: "grant_type=password" +
                "&username=" + username +
                "&password=" + password +
                "&client_id=" +  client + 
                "&client_secret=" + secret,
            headers: {
                "Accept":"application/json", 
                "Content-Type": "application/x-www-form-urlencoded"
            }
        });
        
        console.log(response.access_token);
        return response.access_token;
	}
    
    //get list of Opportunity
    var accessToken = cookie.get(''sf_access_token'');
    if(accessToken == null || accessToken === ''''){
        accessToken = loginSalesForce();
        cookie.put(''sf_access_token'', accessToken);
    }
    
    var opps = $http({
        method: "GET",
        url: baseUrl + "sobjects/Opportunity",
        headers: {"Authorization": "Bearer " + accessToken}
    });

	return opps;
}', NULL, NULL, N'{"imageUrl":"assets/images/actions/logo-salesforce.jpg","docsUrl":"http://docs.backand.com/en/latest/scenarios/salesforceCRM/index.html"}', 1, 1, NULL, NULL, 8, 2)
;
INSERT `backand_ActionTemplate` (`id`, `name`, `shortDescription`, `documentation`, `createdDate`, `updatedDate`, `createdBy`, `updatedBy`, `ruleName`, `objectName`, `action`, `ruleType`, `condition`, `parameters`, `code`, `executeCommand`, `executeMessage`, `json`, `backands`, `approved`, `approvedDate`, `contributors`, `category`, `ordinal`) VALUES (45, N'External Storage', N'Easily store and remove files from your application, using any storage account on AWS, Azure or Google. Connect to your cloud provider account’s but use Back& simple API to manage the files in your app.', NULL, CAST(N'2017-10-20 13:20:43.387' AS DateTime), NULL, NULL, NULL, N'files', NULL, N'OnDemand', N'JavaScript', N'true', N'filename, filedata', N'/* globals
  $http - Service for AJAX calls 
  CONSTS - CONSTS.apiUrl for Backands API URL
  Config - Global Configuration
  socket - Send realtime database communication
  files - file handler, performs upload and delete of files
  request - the current http request
  backand - lite version of Backands SDK
*/
''use strict'';
function backandCallback(userInput, dbRow, parameters, userProfile) {
	// upload file
	var storageAccountName = ""; //The name of the account provider under the External Storage menu, e.g. Aws Storage
	var bucket = ""; //The bucket name, in Azure this is the container, e.g. backandtestupload
	var directory = null; //optional. For no directory leave it null or include it in the filename. The directoy can be with sub directories. e.g. dir1/sub1
	
    if (request.method == "POST"){
        var url = files.upload(parameters.filename, parameters.filedata, storageAccountName, bucket, directory);
        return {"url": url};
    }
    // delete file
    else if (request.method == "DELETE"){
        files.delete(parameters.filename, storageAccountName, bucket, directory);
        return {};    
    }
	
}', NULL, NULL, N'{"imageUrl":"assets/images/actions/external-storage.png","docsUrl":"http://docs.backand.com/#backand-storage"}', 1, 1, NULL, NULL, 5, 4)
;
INSERT INTO `durados_ValidGuid` (`Id`, `UserGuid`, `Used`, `approvedByAdmin`) VALUES ('bf931bf0-3757-444d-ab9c-68368324690b', 'bf931bf0-3757-444d-ab9c-68368324690b', '0', '1');


DROP procedure IF EXISTS `durados_IsValidGuid`;

DELIMITER $$

CREATE PROCEDURE `durados_IsValidGuid`(id varchar(50),	OUT userGuid  varchar(50) )
BEGIN
	
	
        
        DECLARE approvedAlways TINYINT(1) ;
		SELECT g.`UserGuid`,  g.`approvedByAdmin` INTO userGuid , approvedAlways FROM durados_ValidGuid g WHERE g.Id = id  AND (g.`approvedByAdmin` = 1 OR  ( g.`time`>=NOW() AND IFNULL(g.`used`,0) =0));
        
		IF userGuid IS NOT NULL AND approvedAlways=0 
        THEN
			UPDATE durados_ValidGuid g SET g.`used` = 1 WHERE g.`Id` =id;
		END IF;
		
		SELECT id,userGuid;



END$$

DELIMITER ;

