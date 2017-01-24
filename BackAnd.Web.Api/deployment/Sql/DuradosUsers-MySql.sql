USE `__DB_NAME__`;


/****** Object:  Table `durados_UserRole`    Script Date: 07/19/2011 16:41:08 ******/

-- DROP TABLE IF EXISTS `durados_UserRole`;

CREATE TABLE `durados_UserRole` (
  `Name` varchar(256)  CHARACTER set latin1  NOT NULL,
  `Description` varchar(50) NOT NULL,
  `FirstView` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`Name`)
) ;

-- DROP TABLE IF EXISTS `durados_User`;
/****** Object:  Table `durados_User`    Script Date: 07/19/2011 16:41:14 ******/
CREATE TABLE `durados_User` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `Username` varchar(256) NOT NULL,
  `FirstName` varchar(50) NOT NULL,
  `LastName` varchar(50) NOT NULL,
  `Email` varchar(250) NOT NULL,
  `Password` varchar(250) DEFAULT NULL,
  `Role` varchar(256) CHARACTER set latin1 NOT NULL,
  `Guid` varchar(64) NOT NULL,
  `Signature` varchar(4000) DEFAULT NULL,
  `SignatureHTML` varchar(4000) DEFAULT NULL,
  `IsApproved` bit(1) NOT NULL DEFAULT b'1',
  `NewUser` bit(1) DEFAULT NULL,
  `Comments` varchar(8000) CHARACTER SET utf8 DEFAULT NULL,
  PRIMARY KEY (`ID`),
  UNIQUE KEY `Guid` (`Guid`),
  UNIQUE KEY `IX_durados_Username` (`Username`(255)),
  KEY `FK_User_durados_UserRole` (`Role`),
  CONSTRAINT `FK_User_durados_UserRole` FOREIGN KEY (`Role`) REFERENCES `durados_UserRole` (`Name`)
) ENGINE=InnoDB ;

/****** Object:  View `v_User`    Script Date: 07/19/2011 16:41:38 ******/

DROP VIEW IF EXISTS `v_durados_User`;
CREATE VIEW `v_durados_User`
AS
SELECT       `durados_User`.ID,`durados_User`.Username,`durados_User`.FirstName,`durados_User`.LastName,`durados_User`.Email,`durados_User`.`Password`,`durados_User`.`Role`,`durados_User`.`Guid`, 
                        `durados_User`.`Signature`,`durados_User`.SignatureHTML, `durados_User`.`IsApproved` ,CONCAT(`durados_User`.FirstName , ' ', `durados_User`.LastName)  AS FullName, 
                        `durados_User`.NewUser,`durados_User`.Comments
FROM           `durados_User` ;

INSERT INTO `durados_UserRole` (`Name`,`Description`) values ('Developer','Full Control');
INSERT INTO `durados_UserRole` (`Name`,`Description`) values ('Admin','Power User');
INSERT INTO `durados_UserRole` (`Name`,`Description`) values ('View Owner','Power user view owner');
INSERT INTO `durados_UserRole` (`Name`,`Description`) values ('User','User');
INSERT INTO `durados_UserRole` (`Name`,`Description`) values ('Public','Use for internet public access');
INSERT INTO `durados_UserRole` (`Name`,`Description`) values ('ReadOnly','Read-Only access');

CREATE TRIGGER `newUserGuid`
BEFORE INSERT ON `durados_User`
FOR EACH ROW
BEGIN
  SET NEW.`Guid` = UUID();
END;