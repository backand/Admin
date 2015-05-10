
/****** Object:  Table [dbo].[durados_ApprovalStatus]    Script Date: 12/11/2011 09:17:16 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[durados_ApprovalStatus]') AND type in (N'U'))
DROP TABLE [dbo].[durados_ApprovalStatus]
GO


USE [SanDisk_Allegro_Dev_Yariv]
GO

/****** Object:  Table [dbo].[durados_ApprovalStatus]    Script Date: 12/13/2011 17:11:20 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[durados_ApprovalStatus](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Ordinal] [int] NOT NULL,
 CONSTRAINT [PK_ApprovalStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO





/****** Object:  Table [dbo].[durados_ApprovalProcessType]    Script Date: 12/11/2011 09:17:57 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[durados_ApprovalProcessType]') AND type in (N'U'))
DROP TABLE [dbo].[durados_ApprovalProcessType]
GO

/****** Object:  Table [dbo].[durados_ApprovalProcessType]    Script Date: 12/11/2011 09:17:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[durados_ApprovalProcessType](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Ordinal] [int] NOT NULL,
 CONSTRAINT [PK_ApprovalProcessType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ApprovalProcess_ApprovalProcessType]') AND parent_object_id = OBJECT_ID(N'[dbo].[durados_ApprovalProcess]'))
ALTER TABLE [dbo].[durados_ApprovalProcess] DROP CONSTRAINT [FK_ApprovalProcess_ApprovalProcessType]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ApprovalProcess_ApprovalStatus]') AND parent_object_id = OBJECT_ID(N'[dbo].[durados_ApprovalProcess]'))
ALTER TABLE [dbo].[durados_ApprovalProcess] DROP CONSTRAINT [FK_ApprovalProcess_ApprovalStatus]
GO


/****** Object:  Table [dbo].[durados_ApprovalProcess]    Script Date: 12/11/2011 09:18:38 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[durados_ApprovalProcess]') AND type in (N'U'))
DROP TABLE [dbo].[durados_ApprovalProcess]
GO


/****** Object:  Table [dbo].[durados_ApprovalProcess]    Script Date: 12/11/2011 09:18:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[durados_ApprovalProcess](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ApprovalStatusId] [int] NULL,
	[ApprovalProcessTypeId] [int] NULL,
	[Active] [bit] NULL,
	[ParentView] [nvarchar](250) NULL,
 CONSTRAINT [PK_ApprovalProcess] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[durados_ApprovalProcess]  WITH CHECK ADD  CONSTRAINT [FK_ApprovalProcess_ApprovalProcessType] FOREIGN KEY([ApprovalProcessTypeId])
REFERENCES [dbo].[durados_ApprovalProcessType] ([Id])
GO

ALTER TABLE [dbo].[durados_ApprovalProcess] CHECK CONSTRAINT [FK_ApprovalProcess_ApprovalProcessType]
GO

ALTER TABLE [dbo].[durados_ApprovalProcess]  WITH CHECK ADD  CONSTRAINT [FK_ApprovalProcess_ApprovalStatus] FOREIGN KEY([ApprovalStatusId])
REFERENCES [dbo].[durados_ApprovalStatus] ([Id])
GO

ALTER TABLE [dbo].[durados_ApprovalProcess] CHECK CONSTRAINT [FK_ApprovalProcess_ApprovalStatus]
GO


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_durados_ApprovalProcessTypeDefaultUser_durados_ApprovalProcessType]') AND parent_object_id = OBJECT_ID(N'[dbo].[durados_ApprovalProcessTypeDefaultUser]'))
ALTER TABLE [dbo].[durados_ApprovalProcessTypeDefaultUser] DROP CONSTRAINT [FK_durados_ApprovalProcessTypeDefaultUser_durados_ApprovalProcessType]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_durados_ApprovalProcessTypeDefaultUser_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[durados_ApprovalProcessTypeDefaultUser]'))
ALTER TABLE [dbo].[durados_ApprovalProcessTypeDefaultUser] DROP CONSTRAINT [FK_durados_ApprovalProcessTypeDefaultUser_User]
GO


/****** Object:  Table [dbo].[durados_ApprovalProcessTypeDefaultUser]    Script Date: 12/11/2011 09:22:25 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[durados_ApprovalProcessTypeDefaultUser]') AND type in (N'U'))
DROP TABLE [dbo].[durados_ApprovalProcessTypeDefaultUser]
GO


/****** Object:  Table [dbo].[durados_ApprovalProcessTypeDefaultUser]    Script Date: 12/11/2011 09:22:26 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[durados_ApprovalProcessTypeDefaultUser](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ApprovalProcessTypeId] [int] NOT NULL,
	[DefaultUserId] [int] NOT NULL,
 CONSTRAINT [PK_durados_ApprovalProcessTypeDefaultUser] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[durados_ApprovalProcessTypeDefaultUser]  WITH CHECK ADD  CONSTRAINT [FK_durados_ApprovalProcessTypeDefaultUser_durados_ApprovalProcessType] FOREIGN KEY([ApprovalProcessTypeId])
REFERENCES [dbo].[durados_ApprovalProcessType] ([Id])
GO

ALTER TABLE [dbo].[durados_ApprovalProcessTypeDefaultUser] CHECK CONSTRAINT [FK_durados_ApprovalProcessTypeDefaultUser_durados_ApprovalProcessType]
GO

ALTER TABLE [dbo].[durados_ApprovalProcessTypeDefaultUser]  WITH CHECK ADD  CONSTRAINT [FK_durados_ApprovalProcessTypeDefaultUser_User] FOREIGN KEY([DefaultUserId])
REFERENCES [dbo].[User] ([ID])
GO

ALTER TABLE [dbo].[durados_ApprovalProcessTypeDefaultUser] CHECK CONSTRAINT [FK_durados_ApprovalProcessTypeDefaultUser_User]
GO



IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ApprovalProcessUser_ApprovalProcess]') AND parent_object_id = OBJECT_ID(N'[dbo].[durados_ApprovalProcessUser]'))
ALTER TABLE [dbo].[durados_ApprovalProcessUser] DROP CONSTRAINT [FK_ApprovalProcessUser_ApprovalProcess]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ApprovalProcessUser_ApprovalStatus]') AND parent_object_id = OBJECT_ID(N'[dbo].[durados_ApprovalProcessUser]'))
ALTER TABLE [dbo].[durados_ApprovalProcessUser] DROP CONSTRAINT [FK_ApprovalProcessUser_ApprovalStatus]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ApprovalProcessUser_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[durados_ApprovalProcessUser]'))
ALTER TABLE [dbo].[durados_ApprovalProcessUser] DROP CONSTRAINT [FK_ApprovalProcessUser_User]
GO



IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ApprovalProcessUser_ApprovalProcess]') AND parent_object_id = OBJECT_ID(N'[dbo].[durados_ApprovalProcessUser]'))
ALTER TABLE [dbo].[durados_ApprovalProcessUser] DROP CONSTRAINT [FK_ApprovalProcessUser_ApprovalProcess]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ApprovalProcessUser_ApprovalStatus]') AND parent_object_id = OBJECT_ID(N'[dbo].[durados_ApprovalProcessUser]'))
ALTER TABLE [dbo].[durados_ApprovalProcessUser] DROP CONSTRAINT [FK_ApprovalProcessUser_ApprovalStatus]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ApprovalProcessUser_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[durados_ApprovalProcessUser]'))
ALTER TABLE [dbo].[durados_ApprovalProcessUser] DROP CONSTRAINT [FK_ApprovalProcessUser_User]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_durados_ApprovalProcessUser_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[durados_ApprovalProcessUser]'))
ALTER TABLE [dbo].[durados_ApprovalProcessUser] DROP CONSTRAINT [FK_durados_ApprovalProcessUser_User]
GO


/****** Object:  Table [dbo].[durados_ApprovalProcessUser]    Script Date: 12/11/2011 17:05:52 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[durados_ApprovalProcessUser]') AND type in (N'U'))
DROP TABLE [dbo].[durados_ApprovalProcessUser]
GO


/****** Object:  Table [dbo].[durados_ApprovalProcessUser]    Script Date: 12/11/2011 17:05:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[durados_ApprovalProcessUser](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ApprovalProcessId] [int] NULL,
	[UserId] [int] NULL,
	[ApprovalStatusId] [int] NULL,
	[SignedUserId] [int] NULL,
	[SignedDate] [date] NULL,
	[CreatedDate] [date] NULL,
	[Comment] [nvarchar](2000) NULL,
	[Message] [nvarchar](4000) NULL,
 CONSTRAINT [PK_ApprovalProcessUser] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[durados_ApprovalProcessUser]  WITH CHECK ADD  CONSTRAINT [FK_ApprovalProcessUser_ApprovalProcess] FOREIGN KEY([ApprovalProcessId])
REFERENCES [dbo].[durados_ApprovalProcess] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[durados_ApprovalProcessUser] CHECK CONSTRAINT [FK_ApprovalProcessUser_ApprovalProcess]
GO

ALTER TABLE [dbo].[durados_ApprovalProcessUser]  WITH CHECK ADD  CONSTRAINT [FK_ApprovalProcessUser_ApprovalStatus] FOREIGN KEY([ApprovalStatusId])
REFERENCES [dbo].[durados_ApprovalStatus] ([Id])
GO

ALTER TABLE [dbo].[durados_ApprovalProcessUser] CHECK CONSTRAINT [FK_ApprovalProcessUser_ApprovalStatus]
GO

ALTER TABLE [dbo].[durados_ApprovalProcessUser]  WITH CHECK ADD  CONSTRAINT [FK_ApprovalProcessUser_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([ID])
GO

ALTER TABLE [dbo].[durados_ApprovalProcessUser] CHECK CONSTRAINT [FK_ApprovalProcessUser_User]
GO

ALTER TABLE [dbo].[durados_ApprovalProcessUser]  WITH CHECK ADD  CONSTRAINT [FK_durados_ApprovalProcessUser_User] FOREIGN KEY([SignedUserId])
REFERENCES [dbo].[User] ([ID])
GO

ALTER TABLE [dbo].[durados_ApprovalProcessUser] CHECK CONSTRAINT [FK_durados_ApprovalProcessUser_User]
GO


INSERT INTO [dbo].[durados_ApprovalStatus]
           ([Id]
           ,[Name]
           ,[Ordinal])
     VALUES
           (1
           ,'Pending'
           ,1)
GO

INSERT INTO [dbo].[durados_ApprovalStatus]
           ([Id]
           ,[Name]
           ,[Ordinal])
     VALUES
           (2
           ,'Approved'
           ,2)
GO

INSERT INTO [dbo].[durados_ApprovalStatus]
           ([Id]
           ,[Name]
           ,[Ordinal])
     VALUES
           (3
           ,'Rejected'
           ,3)
GO

CREATE PROCEDURE [dbo].[durados_CreateApprovalProcess]
	-- Add the parameters for the stored procedure here
	@ApprovalProcessTypeId	INT,
	@pk						INT,
	@ParentView				nvarchar(250),
	@ID						INT output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	INSERT INTO [dbo].[durados_ApprovalProcess]
			   ([ApprovalStatusId]
			   ,[ApprovalProcessTypeId]
			   ,[Active]
			   ,[ParentView])
		 VALUES
			   (1
			   ,@ApprovalProcessTypeId
			   ,1
			   ,@ParentView)

	SET @ID = SCOPE_IDENTITY()




	INSERT INTO [dbo].[durados_ApprovalProcessUser]
			   ([ApprovalProcessId]
			   ,[UserId]
			   ,[ApprovalStatusId]
			   ,[SignedDate]
			   ,[CreatedDate])
		SELECT @ID, DefaultUserId, 1, null, getdate()
		FROM  dbo.durados_ApprovalProcessTypeDefaultUser
		WHERE (ApprovalProcessTypeId = @ApprovalProcessTypeId)
           

END


GO

CREATE PROCEDURE [dbo].[durados_CompleteApprovalProcess]
	-- Add the parameters for the stored procedure here
	@ApprovalProcessId		INT,
	@ApprovalProcessUserId	INT,
	@ApprovalProcessViewName	nvarchar(250),
	@StatusFieldName	nvarchar(250),
	@ApprovalStatusId						INT output
AS
BEGIN

	
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE dbo.durados_ApprovalProcessUser
	set dbo.durados_ApprovalProcessUser.SignedDate = getdate()
	WHERE dbo.durados_ApprovalProcessUser.Id = @ApprovalProcessUserId

	SELECT @ApprovalStatusId = CASE WHEN 3 IN (SELECT ApprovalStatusId FROM dbo.durados_ApprovalProcessUser  WITH (NOLOCK) WHERE ApprovalProcessId = @ApprovalProcessId) THEN 3 
			WHEN NOT 1 IN (SELECT ApprovalStatusId FROM dbo.durados_ApprovalProcessUser  WITH (NOLOCK) WHERE ApprovalProcessId = @ApprovalProcessId) THEN 2
			ELSE 1 END
			
	declare @PrevStatusId int 
	
	select @PrevStatusId = ApprovalStatusId 
	from dbo.durados_ApprovalProcess WITH (NOLOCK) 
	WHERE dbo.durados_ApprovalProcess.Id = @ApprovalProcessId
			
	UPDATE dbo.durados_ApprovalProcess
    SET  ApprovalStatusId = @ApprovalStatusId
	WHERE dbo.durados_ApprovalProcess.Id = @ApprovalProcessId
	
		
	-- message board
	declare @MessageBoardId int 
	declare @UserId int
	
	select @MessageBoardId = Id from dbo.durados_MessageBoard WITH (NOLOCK) where ViewName = 'durados_ApprovalProcessUser' and PK = @ApprovalProcessUserId
	select @UserId = UserId from dbo.durados_ApprovalProcessUser WITH (NOLOCK) where Id = @ApprovalProcessUserId
	
	update durados_MessageStatus
	set ActionRequired = 0
	where MessageId = @MessageBoardId and UserId = @UserId
	
	-- history
	if @PrevStatusId <> @ApprovalStatusId
	begin
		declare @HistoryId int 
		declare @OldValue nvarchar(50) 
		declare @NewValue nvarchar(50)
		
		select @OldValue = [Name] 
		from dbo.durados_ApprovalStatus WITH (NOLOCK) 
		where Id = @PrevStatusId
		
		select @NewValue = [Name] 
		from dbo.durados_ApprovalStatus WITH (NOLOCK) 
		where Id = @ApprovalStatusId
		
		INSERT INTO durados_ChangeHistory (ViewName, PK, ActionId, UpdateUserId) values (@ApprovalProcessViewName,  @ApprovalProcessId, 2, @UserId);
		SELECT @HistoryId = IDENT_CURRENT('durados_ChangeHistory')
		
		INSERT INTO durados_ChangeHistoryField (ChangeHistoryId, FieldName, ColumnNames, OldValue, NewValue, OldValueKey, NewValueKey) values (@HistoryId,  @StatusFieldName, 'ApprovalStatusId', @OldValue, @NewValue, @PrevStatusId, @ApprovalStatusId);
	end
END


GO

CREATE PROCEDURE dbo.durados_UpdateApprovalProcessMessage
	(
	@Id int = 5,
	@Message nvarchar(4000)
	)
AS
BEGIN
	SET NOCOUNT ON;
	
	update durados_ApprovalProcessUser
	set Message = @Message
	where Id = @Id
END
	
GO
