 USE `__DB_NAME__`;


-- DROP TABLE IF EXISTS `durados_Action`;
CREATE TABLE `durados_Action` (
  `Id` int(11) NOT NULL,
  `Name` varchar(1000) NOT NULL,
  `CloudVendor` varchar(50) NOT NULL,
  `AwsRegion` varchar(50) NOT NULL,
  `AccessKeyId` varchar(50) NOT NULL,
  `EncryptedSecretAccessKey` varchar(8000) NOT NULL,
  `LambdaArnPrefix` varchar(1000) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


