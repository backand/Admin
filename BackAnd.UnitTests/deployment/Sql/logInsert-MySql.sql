
CREATE PROCEDURE `Durados_LogInsert`(
    ApplicationName nvarchar(250)  ,
	Username nvarchar(250) ,
	MachineName nvarchar(250) ,
	Time datetime ,
	Controller nvarchar(250),
	Action nvarchar(250) ,
	MethodName nvarchar(250),
	LogType  int ,
    ExceptionMessage 	VARCHAR(4000) CHARACTER SET utf8 ,
	Trace VARCHAR(8000) CHARACTER SET utf8 ,
	FreeText VARCHAR(8000) CHARACTER SET utf8 ,
	Guid VARCHAR(64) 
)
BEGIN
     IF Guid='' THEN SELECT UUID() into  Guid; END IF;
INSERT INTO `Durados_Log`
(
		`ApplicationName`,
		`Username`,
		`MachineName`,
		`Time`,
		`Controller`,
		`Action`,
		`MethodName`,
		`LogType`,
		`ExceptionMessage`,
		`Trace`,
		`FreeText`,
		`Guid`)

		VALUES
			(ApplicationName
			,Username
			,MachineName
			,Time
			,Controller
			,Action
			,MethodName
			,LogType
			,ExceptionMessage
			,Trace
			,FreeText
			,Guid);
    
   
END