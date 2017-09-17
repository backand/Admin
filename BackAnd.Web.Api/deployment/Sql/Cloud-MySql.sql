 USE `__DB_NAME__`;


-- DROP TABLE IF EXISTS `durados_Action`;
CREATE TABLE `durados_Cloud` (
  `Id` int AUTO_INCREMENT NOT NULL,
  `Name` varchar(1000) NOT NULL,
  `CloudVendor` varchar(50) NOT NULL,
  `AwsRegion` varchar(50)  NULL,
  `AccessKeyId` varchar(50)  NULL,
  `EncryptedSecretAccessKey` varchar(500)  NULL,
  `Description` varchar(1000) Null,
  `subscriptionId` varchar(150)  NULL,
  `appId` varchar(150)  NULL,
  `password` varchar(500)  NULL,
  `tenant` varchar(150)  NULL,
  `EncryptePrivateKey` VARCHAR(2000) NULL ,
	`ClientEmail` VARCHAR(150) NULL ,
	`ProjectName` VARCHAR(150) NULL ,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


