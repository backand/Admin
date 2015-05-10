
INSERT INTO [dbo].[durados_Html]([Name],[Text]) VALUES('passwordResetConfirmationMessageInSite','<div>Hello [username],<br><br><br>Please click on the <a href=''[Url]/change-password?id=[Guid]'' >Email Verification</a><br> to complete the password reset process.<br><br>Sincerely,<br><br>[Product] Team</Div>')
GO



CREATE TABLE [dbo].[durados_ValidGuid](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY  ,
	[UserGuid] [uniqueidentifier] NULL,
	[Time] [datetime] NULL DEFAULT getdate(),
	[Used] [bit] NULL  DEFAULT ((0)),
	[approvedByAdmin] [bit] NULL DEFAULT ((0)),
)


GO

declare @superGuid nvarchar(100)
-- for qa use
-- SET @superGuid= '4BBA6F93-163B-4B0F-9C71-F3C577A693F9'
--67E99B5D-C726-475F-807D-1C63352C67E0
--for production use 
--B94538EB-12B1-41CE-8D3C-BD7D22C4F78F
-- SET @superGuid='B94538EB-12B1-41CE-8D3C-BD7D22C4F78F'
INSERT INTO [dbo].[durados_ValidGuid]  ([Id],[UserGuid],[Time],[Used],[approvedByAdmin]) VALUES(@superGuid,@superGuid,getdate(),0,1)
GO



/****** Object:  StoredProcedure [dbo].[durados_SetValidGuid]    Script Date: 8/13/2014 14:10:46 ******/

CREATE PROCEDURE [dbo].[durados_SetValidGuid]
       @userGuid as nvarchar(50),
       @timeSpan as int,
	   @id  nvarchar(150) =null   OUTPUT
 
AS
BEGIN
		select @id=cast(newid()   as nvarchar(150))
		declare @exists uniqueidentifier =null
		select @exists=Id from durados_ValidGuid where userGuid = @userGuid AND ( ([time]>=GETDATE() AND isNull(used,0) =0) OR ApprovedByAdmin =1)
		if @exists is null
		begin
			
			declare @now datetime =DATEADD(mi,@timeSpan, getdate() )
			insert into durados_ValidGuid (Id,userGuid,[Time]) Values (@Id,@userGuid,@now)
		end
		else
		begin
			set @id=@exists
		end
		
END
GO


/****** Object:  StoredProcedure [dbo].[durados_IsValidGuid]    Script Date: 8/13/2014 14:11:07 ******/


CREATE PROCEDURE [dbo].[durados_IsValidGuid]
	@id as uniqueidentifier,
	@userGuid  uniqueidentifier = null OUTPUT
AS
BEGIN
		
		declare @approvedAlways bit =0
		select @approvedAlways=ApprovedByAdmin,@userGuid=userGuid from durados_ValidGuid where Id = @Id AND ( ([time]>=GETDATE() AND isNull(used,0) =0) OR ApprovedByAdmin =1)
		if(@userGuid is not null AND @approvedAlways=0 )
			UPDATE durados_ValidGuid  SET used = 1 WHERE Id =@id
		select @userGuid
END
GO


