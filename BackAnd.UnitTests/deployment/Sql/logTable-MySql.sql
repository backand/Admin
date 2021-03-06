USE `Logs` ;
CREATE TABLE `Durados_Log` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `ApplicationName` varchar(250) DEFAULT NULL,
  `Username` varchar(250) DEFAULT NULL,
  `MachineName` varchar(250) DEFAULT NULL,
  `Time` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `Controller` varchar(250) DEFAULT NULL,
  `Action` varchar(250) DEFAULT NULL,
  `MethodName` varchar(250) DEFAULT NULL,
  `LogType` int(11) DEFAULT NULL,
 `ExceptionMessage` varchar(4000) CHARACTER SET utf8 DEFAULT NULL,
  `Trace` varchar(8000) CHARACTER SET utf8 DEFAULT NULL,
   `FreeText` varchar(8000) CHARACTER SET utf8 DEFAULT NULL,
  `Guid` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=2721 DEFAULT CHARSET=latin1;
