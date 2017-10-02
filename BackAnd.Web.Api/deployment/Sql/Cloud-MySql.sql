 USE `__DB_NAME__`;


-- DROP TABLE IF EXISTS `durados_Action`;
CREATE TABLE `durados_Cloud` (
  `Id` int AUTO_INCREMENT NOT NULL,
  `Type` varchar(50)  NOT NULL,
  `Name` varchar(250) NOT NULL,
  `CloudVendor` varchar(50) NOT NULL,
  `AwsRegion` varchar(250)  NULL,
  `AccessKeyId` varchar(50)  NULL,
  `EncryptedSecretAccessKey` varchar(500)  NULL,
  `Description` varchar(1000) Null,
  `subscriptionId` varchar(150)  NULL,
  `appId` varchar(150)  NULL,
  `password` varchar(500)  NULL,
  `tenant` varchar(150)  NULL,
  `EncryptedPrivateKey` VARCHAR(4000) NULL ,
	`ClientEmail` VARCHAR(150) NULL ,
	`ProjectName` VARCHAR(150) NULL ,
	`connectionString` VARCHAR(300) NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;



