
 USE `__DB_NAME__`;

/****** Object:  Table `durados_Html`    Script Date: 01/17/2011 11:45:31 ******/
-- DROP TABLE IF EXISTS `durados_Folder`;

CREATE TABLE `durados_Folder`(
	`Id` int AUTO_INCREMENT NOT NULL,
	`Name` varchar(150) NOT NULL,
	`UserId` int NOT NULL,
	`Ordinal` int NOT NULL,
  PRIMARY KEY  
(
	`Id` ASC
));


-- DROP TABLE IF EXISTS `durados_Link`;
CREATE TABLE `durados_Link`(
	`Id` INT AUTO_INCREMENT NOT NULL,
	`UserId` INT NOT NULL,
	`LinkType` SMALLINT NOT NULL,
	`Name` varchar(250) NOT NULL,
	`Ordinal` INT NOT NULL,
	`ViewName` varchar(150) NULL,
	`ControllerName` varchar(150) NULL,	
	`Guid` varchar(150) NULL,
	`Url` varchar(500) NULL,
	`Filter` varchar(8000) CHARACTER SET utf8 DEFAULT  NULL,
	`SortColumn` varchar(150) NULL,
	`SortDirection` varchar(5) NULL,
	`PageNo` SMALLINT NOT NULL,
	`PageSize` SMALLINT NOT NULL,
	`CreationDate` DATETIME NOT NULL DEFAULT NOW(),
	`Description` varchar(2000) NULL,
	`FolderId` INT NULL,
 PRIMARY KEY   (	`Id` ASC)
) ;




ALTER TABLE `durados_Link`  ADD  CONSTRAINT `FK_durados_Link_durados_Folder` FOREIGN KEY(`FolderId`)
REFERENCES `durados_Folder` (`Id`);

-- ALTER TABLE `durados_Link` ADD CONSTRAINT `FK_durados_Link_durados_Folder`
ALTER TABLE `durados_Link` ALTER COLUMN `Ordinal` SET DEFAULT 1;

ALTER TABLE `durados_Link` ALTER COLUMN `PageNo` SET DEFAULT 1;

ALTER TABLE `durados_Link` ALTER COLUMN `PageSize` SET DEFAULT 20;
