CREATE PROCEDURE [dbo].[Durados_LogInsert]
    @ApplicationName nvarchar(250) = null,
	@Username nvarchar(250) = null,
	@MachineName nvarchar(250) = null,
	@Time datetime = null,
	@Controller nvarchar(250) = null,
	@Action nvarchar(250) = null,
	@MethodName nvarchar(250) = null,
	@LogType  int = null,
    @ExceptionMessage nvarchar(max) = null,
	@Trace nvarchar(max) = null,
	@FreeText nvarchar(max) = null,
	@Guid uniqueIdentifier = null
AS
BEGIN

INSERT INTO [dbo].[Durados_Log]
			([ApplicationName]
			,[Username]
			,[MachineName]
			,[Time]
			,[Controller]
			,[Action]
			,[MethodName]
			,[LogType]
			,[ExceptionMessage]
			,[Trace]
			,[FreeText]
			,[Guid])
		VALUES
			(@ApplicationName
			,@Username
			,@MachineName
			,@Time
			,@Controller
			,@Action
			,@MethodName
			,@LogType
			,@ExceptionMessage
			,@Trace
			,@FreeText
			,@Guid)
    
    RETURN 0
END

