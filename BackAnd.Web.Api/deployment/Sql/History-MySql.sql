 USE `__DB_NAME__`;


-- DROP TABLE IF EXISTS `durados_Action`;
CREATE TABLE `durados_Action` (
  `Id` int(11) NOT NULL,
  `Name` varchar(50) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


INSERT INTO `durados_Action` (`Id`,`Name`) VALUES (1,'Insert');
INSERT INTO `durados_Action` (`Id`,`Name`) VALUES (2,'Update');
INSERT INTO `durados_Action` (`Id`,`Name`) VALUES (3,'Delete');
-- 
-- IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'`DF_durados_ChangeHistory_UpdateDate`') AND type = 'D')
-- BEGIN
-- ALTER TABLE `durados_ChangeHistory` DROP CONSTRAINT `DF_durados_ChangeHistory_UpdateDate`
-- END
-- 
-- /****** Object:  Table `durados_ChangeHistory`    Script Date: 12/01/2011 13:22:27 ******/
-- IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'`durados_ChangeHistory`') AND type in (N'U'))
-- DROP TABLE `durados_ChangeHistory`

-- DROP TABLE IF EXISTS `durados_ChangeHistory`;
/****** Object:  Table `durados_ChangeHistory`    Script Date: 12/01/2011 13:22:30 ******/


CREATE TABLE `durados_ChangeHistory`(
	`id` INT AUTO_INCREMENT NOT NULL,
	`ViewName` VARCHAR(150) NOT NULL,
	`PK` VARCHAR(250) NOT NULL,
	`ActionId` INT NOT NULL,
	`UpdateDate` DateTime NOT NULL DEFAULT NOW(),
	`UpdateUserId` INT NOT NULL,
	`Comments` varchar(8000) CHARACTER SET utf8 DEFAULT  NULL,
	`TransactionName` VARCHAR(50) NULL,
	`Version` VARCHAR(50) NULL,
	`Workspace` VARCHAR(50) NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


-- ALTER TABLE `durados_ChangeHistory` ADD  CONSTRAINT `DF_durados_ChangeHistory_UpdateDate`  DEFAULT (getdate()) FOR `UpdateDate`

/****** Object:  Table `durados_ChangeHistoryField`    Script Date: 12/01/2011 13:23:02 ******/
-- IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'`durados_ChangeHistoryField`') AND type in (N'U'))
-- DROP TABLE IF EXISTS `durados_ChangeHistoryField`;

/****** Object:  Table `durados_ChangeHistoryField`    Script Date: 12/01/2011 13:23:05 ******/


CREATE TABLE `durados_ChangeHistoryField`(
	`Id` INT AUTO_INCREMENT NOT NULL,
	`ChangeHistoryId` INT NOT NULL,
	`FieldName` VARCHAR(500) NOT NULL,
	`ColumnNames` VARCHAR(500) NOT NULL,
	`OldValue` LONGTEXT CHARACTER SET utf8  NOT NULL,
	`NewValue` LONGTEXT CHARACTER SET utf8  NOT NULL,
	`OldValueKey` varchar(5000) CHARACTER SET utf8  NULL,
	`NewValueKey` varchar(5000) CHARACTER SET utf8  NULL,
 PRIMARY KEY (`Id`)
) ;

/****** Object:  View `durados_v_ChangeHistory`    Script Date: 12/01/2011 13:23:40 ******/
-- IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'`durados_v_ChangeHistory`'))
DROP VIEW IF  EXISTS`durados_v_ChangeHistory`;
CREATE VIEW `durados_v_ChangeHistory` AS
    SELECT 
        IFNULL(`durados_ChangeHistoryField`.`Id`,
                -(`durados_ChangeHistory`.`id`)) AS `AutoId`,
        `durados_ChangeHistory`.`ViewName` AS `ViewName`,
        `durados_ChangeHistory`.`PK` AS `PK`,
        `durados_ChangeHistory`.`ActionId` AS `ActionId`,
        `durados_ChangeHistory`.`UpdateDate` AS `UpdateDate`,
        `durados_ChangeHistory`.`UpdateUserId` AS `UpdateUserId`,
        `durados_ChangeHistoryField`.`FieldName` AS `FieldName`,
        `durados_ChangeHistoryField`.`ColumnNames` AS `ColumnNames`,
        `durados_ChangeHistoryField`.`OldValue` AS `OldValue`,
        `durados_ChangeHistoryField`.`NewValue` AS `NewValue`,
        `durados_ChangeHistoryField`.`Id` AS `Id`,
        `durados_ChangeHistoryField`.`ChangeHistoryId` AS `ChangeHistoryId`,
        `durados_ChangeHistory`.`Comments` AS `Comments`,
        `durados_ChangeHistory`.`Version` AS `Version`,
        `durados_ChangeHistory`.`Workspace` AS `Workspace`,
        (CASE
            WHEN (`durados_ChangeHistory`.`Workspace` = 'Admin') THEN 1
            ELSE 0
        END) AS `Admin`
    FROM
        (`durados_ChangeHistory`
        LEFT JOIN `durados_ChangeHistoryField` ON ((`durados_ChangeHistory`.`id` = `durados_ChangeHistoryField`.`ChangeHistoryId`)));



