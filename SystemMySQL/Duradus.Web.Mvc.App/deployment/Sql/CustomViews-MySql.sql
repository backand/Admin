 USE `__DB_NAME__`;


-- DROP TABLE  IF EXISTS `durados_CustomViews`;

CREATE TABLE `durados_CustomViews` (
    `Id` INT(11) NOT NULL AUTO_INCREMENT,
    `UserId` INT(11) NOT NULL,
    `ViewName` VARCHAR(300) NOT NULL,
    `CustomView` varchar(8000) CHARACTER SET utf8 NOT NULL,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `IX_durados_CustomViews` (`UserId` , `ViewName` (255))
)  ENGINE=INNODB DEFAULT CHARSET=LATIN1;

DROP PROCEDURE IF  EXISTS `durados_UpdateUserCustomView`;
-- DELIMITER $$
CREATE PROCEDURE `durados_UpdateUserCustomView`( 
UserId int,
ViewName varchar(300),
CustomView varchar(8000) CHARACTER SET utf8 
)
BEGIN
DECLARE _rowcount int;
	
    SET  TRANSACTION ISOLATION LEVEL SERIALIZABLE;
START TRANSACTION;
 
	UPDATE `durados_CustomViews` SET `CustomView` = CustomView WHERE `durados_CustomViews`.`UserId` = UserId   AND `durados_CustomViews`.`ViewName` = ViewName;

	set _rowcount = ROW_COUNT();
	-- set @err =mysql_error();@err = 0 AND 

	IF _rowcount = 0 
    THEN 
		INSERT INTO `durados_CustomViews` (`UserId`, `ViewName`, `CustomView`) VALUES (UserId, ViewName, CustomView);
	END IF;
COMMIT;
END -- $$

-- call durados_UpdateUserCustomView(1,'link','customeView222')