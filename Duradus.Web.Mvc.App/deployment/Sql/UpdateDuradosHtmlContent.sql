/****** Script for SelectTopNRows command from SSMS  ******/
--[dbo].[ChangeHtmlKeyMessage]
DROP PROCEDURE [dbo].[ChangeHtmlKeyMessage]
GO

CREATE  PROCEDURE [dbo].[ChangeHtmlKeyMessage]
	-- Add the parameters for the stored procedure here
	@messageKey AS VarChar(250) = 'newUserInvitationMessage'
	,@oldText AS VarChar(300) = '/Account/LogOn?userid=[Guid]'
	,@newText AS VarChar(300) = '/Account/LogOn?userid=[ID]'
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @querySQL as NVARCHAR(MAX)

	SET QUERY_GOVERNOR_COST_LIMIT 0;

	DECLARE @id INT
	DECLARE @Catalog NVARCHAR(250)
	DECLARE Conn_Cursor CURSOR FOR
		select c.Id, c.[Catalog]
			from [dbo].[durados_app] a with (nolock) inner join
			[dbo].[durados_SqlConnection] c with (nolock) ON a.SystemSqlConnectionId = c.Id
			where ((a.TemplateId is null) or (a.TemplateId not in (select appid from [durados_SampleApp]) and a.Id not in (select appid from [durados_SampleApp])))
			AND a.Deleted = 0 
			order by c.Id desc

	OPEN Conn_Cursor
			FETCH NEXT FROM Conn_Cursor INTO @id,@Catalog
			WHILE @@FETCH_STATUS = 0
		BEGIN
		BEGIN TRY
			
	--UPDATE durados_App_1199.[dbo].[durados_Html]  SET [Text ]=Replace(cast([Text] as nvarchar(300)),'/Account/LogOn?id=[Guid]','/Account/LogOn?userid=[Guid]')  WHERE[Name] = 'newUserInvitationMessage'
	SET @querySQL = 'UPDATE [' + @Catalog + '].dbo.[durados_Html] SET [text] = replace(cast([text] as nvarchar(max)), '''+@oldText+''', '''+@newText+''')   WHERE [Name] =''' + @messageKey + ''''
	 
	  --print(@querySQL)
		exec sp_executesql @querySQL --, N'@OldProductName VarChar(250), @ShortProductName VarChar(250)', @messageKey = @messageKey, @ShortProductName = @ShortProductName
	   
		  
		  
			FETCH NEXT FROM Conn_Cursor INTO @id,@Catalog
		END TRY
		BEGIN CATCH
		  
			FETCH NEXT FROM Conn_Cursor INTO @id,@Catalog
	  
		END CATCH
		END
			CLOSE Conn_Cursor
			DEALLOCATE Conn_Cursor


END
GO


--UPDATE [durados_App_2275].dbo.[durados_Html] SET [text] = replace(cast([text] as nvarchar(max)), '/Account/LogOn?id=[Guid]', '/Account/LogOn?userid=[Guid]')   WHERE [Name] ='newUserInvitationMessage'