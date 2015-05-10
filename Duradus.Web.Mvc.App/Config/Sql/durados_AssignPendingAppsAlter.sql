ALTER PROCEDURE [dbo].[durados_AssignPendingApps] 
	-- Add the parameters for the stored procedure here
	@newUser nvarchar(256)
	,@appName nvarchar(250)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    declare @userId int
    declare @appId int
    
	select @userId = ID from dbo.durados_User where Username = @newUser
	select @appId = Id from dbo.durados_App where Name = @appName and Deleted=0
	
	if (@userId is not null And @appId is not null)
	begin
	-- Insert statements for procedure here
		insert into durados_UserApp(UserId, AppId, [Role])
		select @userId as UserId, AppId, case when ([Role] = 'Admin' or [Role] = 'Developer') then 'Admin' else 'User' end as [Role] from durados_UserApp_Pending where Username = @newUser and AppId=@AppId
		
		delete durados_UserApp_Pending
		where Username = @newUser  and AppId=@appId
	end
END
