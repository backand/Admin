
 USE `__DB_NAME__`;


-- DROP TABLE IF  EXISTS `durados_session`;

/****** Object:  Table `durados_session`    Script Date: 12/01/2011 14:58:19 ******/

CREATE TABLE `durados_session`(
	`SessionID` VARCHAR(50) NOT NULL,
	`Name` VARCHAR(250) NOT NULL,
	`Scalar` LONGTEXT NULL,
	`SerializedObject`  TEXT NULL,
	`TypeCode` VARCHAR(50) NULL,
	`ObjectType` VARCHAR(500) NULL,
 PRIMARY KEY
(
	`SessionID` ASC,
	`Name` ASC
));


/****** Object:  StoredProcedure `durados_setsession`    Script Date: 12/01/2011 14:58:48 ******/
-- (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'`durados_setsession`') AND type in (N'P', N'PC'))
DROP PROCEDURE IF  EXISTS `durados_setsession`;

-- '(nolock) where SessionID=SessionID  and `Name`=Name    IF isExist IS NOT NULL   ' at line 13

/****** Object:  StoredProcedure `durados_setsession`    Script Date: 12/01/2011 14:58:48 ******/
-- DELIMITER $$
CREATE PROCEDURE `durados_setsession`
	
	(
	Name varchar(250) ,
	SessionID varchar(50),
	Scalar LONGTEXT ,
	TypeCode varchar(50)
	)
	
BEGIN
	DECLARE isExist varchar(250);
	
	select isExist = `Name` from durados_session  where SessionID=SessionID  and `Name`=Name;
	
	IF isExist IS NOT NULL THEN
	
        update durados_session set Scalar=Scalar, TypeCode=TypeCode, SerializedObject=null, ObjectType=null where SessionID=SessionID and `Name`=Name; 
      
	
	ELSE
	
		INSERT INTO durados_session(SessionID, `Name`, Scalar, TypeCode) values (SessionID, Name, Scalar, TypeCode);
END IF;

END
-- $$

